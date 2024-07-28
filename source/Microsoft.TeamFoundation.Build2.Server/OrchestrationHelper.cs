// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.OrchestrationHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal abstract class OrchestrationHelper
  {
    private PipelineBuilder m_pipelineBuilder;
    protected readonly ITeamFoundationBuildService2 m_tfBuildService;
    protected readonly BuildDefinition m_definition;
    protected List<TaskOrchestrationJob> m_jobs;
    private List<TaskInstance> m_containerTasksToInjectUpFront;
    protected IOrchestrationEnvironment m_environment;
    protected readonly Microsoft.VisualStudio.Services.Identity.Identity m_requestedBy;
    protected readonly Microsoft.VisualStudio.Services.Identity.Identity m_requestedFor;

    internal OrchestrationHelper(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      Microsoft.VisualStudio.Services.Identity.Identity requestedBy,
      Microsoft.VisualStudio.Services.Identity.Identity requestedFor)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckForNull<BuildProcess>(definition.Process, "Process");
      this.m_tfBuildService = requestContext.GetService<ITeamFoundationBuildService2>();
      this.m_definition = definition;
      this.m_requestedBy = requestedBy;
      this.m_requestedFor = requestedFor;
    }

    internal abstract PlanEnvironment BuildContainerEnvironment(
      IVssRequestContext requestContext,
      out List<TaskInstance> tasksToInject);

    internal abstract void SetupEnvironment(
      IVssRequestContext requestContext,
      PipelineBuilder builder);

    internal PipelineBuilder GetPipelineBuilder(
      IVssRequestContext requestContext,
      AgentPoolQueue defaultQueue = null,
      PipelineResources authorizedResources = null,
      AgentSpecification defaultAgentSpecification = null,
      bool evaluateCounters = true)
    {
      if (this.m_pipelineBuilder == null)
      {
        this.m_pipelineBuilder = this.m_definition.GetPipelineBuilder(requestContext, authorizedResources, evaluateCounters: evaluateCounters);
        if (defaultQueue != null)
          this.m_pipelineBuilder.DefaultQueue = new AgentQueueReference()
          {
            Id = defaultQueue.Id
          };
        if (defaultAgentSpecification != null)
          this.m_pipelineBuilder.DefaultAgentSpecification = defaultAgentSpecification.ToJObject();
        this.SetupEnvironment(requestContext, this.m_pipelineBuilder);
      }
      return this.m_pipelineBuilder;
    }

    internal IOrchestrationEnvironment GetContainerEnvironment(
      IVssRequestContext requestContext,
      out List<TaskInstance> tasksToInjectUpFront)
    {
      if (this.m_environment == null)
        this.m_environment = (IOrchestrationEnvironment) this.BuildContainerEnvironment(requestContext, out this.m_containerTasksToInjectUpFront);
      tasksToInjectUpFront = this.m_containerTasksToInjectUpFront;
      return this.m_environment;
    }

    internal virtual void PopulateWellKnownVariables(
      IVssRequestContext requestContext,
      IDictionary<string, VariableValue> variables)
    {
      this.m_definition.AddWellKnownVariables(requestContext, variables);
      variables["build.queuedBy"] = (VariableValue) this.m_requestedBy?.DisplayName;
      variables["build.queuedById"] = (VariableValue) this.m_requestedBy?.Id.ToString("D");
      variables["build.requestedFor"] = (VariableValue) this.m_requestedFor?.DisplayName;
      variables["build.requestedForId"] = (VariableValue) this.m_requestedFor?.Id.ToString("D");
      string str;
      if (!this.m_requestedFor.Properties.TryGetValue<string>("Mail", out str))
        return;
      variables["build.requestedForEmail"] = (VariableValue) str;
    }
  }
}
