// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ProviderPhase
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ProviderPhase : PhaseNode
  {
    public ProviderPhase()
    {
    }

    private ProviderPhase(ProviderPhase phaseToCopy)
      : base((PhaseNode) phaseToCopy)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public override PhaseType Type => PhaseType.Provider;

    [DataMember(EmitDefaultValue = false)]
    public EnvironmentDeploymentTarget EnvironmentTarget { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Provider { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, JToken> Strategy { get; set; }

    public override void Validate(PipelineBuildContext context, ValidationResult result)
    {
      base.Validate(context, result);
      IPhaseProvider phaseProvider = context.PhaseProviders.FirstOrDefault<IPhaseProvider>((Func<IPhaseProvider, bool>) (x => string.Equals(x.Provider, this.Provider, StringComparison.OrdinalIgnoreCase)));
      if (phaseProvider == null)
      {
        result.Errors.Add(new PipelineValidationError("'" + this.Provider + "' phase '" + this.Name + "' is not supported."));
      }
      else
      {
        ValidationResult validationResult = phaseProvider.Validate(context, this);
        if (validationResult == null)
          return;
        foreach (PipelineValidationError error in (IEnumerable<PipelineValidationError>) validationResult.Errors)
          result.Errors.Add(error);
        result.ReferencedResources.MergeWith(validationResult.ReferencedResources);
        foreach (ServiceEndpointReference endpoint in (IEnumerable<ServiceEndpointReference>) validationResult.ReferencedResources.Endpoints)
        {
          if (context.ResourceStore.GetEndpoint(endpoint) == null)
            result.UnauthorizedResources.AddEndpointReference(endpoint);
        }
        foreach (SecureFileReference file in (IEnumerable<SecureFileReference>) validationResult.ReferencedResources.Files)
        {
          if (context.ResourceStore.GetFile(file) == null)
            result.UnauthorizedResources.AddSecureFileReference(file);
        }
        foreach (AgentQueueReference queue in (IEnumerable<AgentQueueReference>) validationResult.ReferencedResources.Queues)
        {
          if (context.ResourceStore.GetQueue(queue) == null)
            result.UnauthorizedResources.AddAgentQueueReference(queue);
        }
        foreach (VariableGroupReference variableGroup in (IEnumerable<VariableGroupReference>) validationResult.ReferencedResources.VariableGroups)
        {
          if (context.ResourceStore.GetVariableGroup(variableGroup) == null)
            result.UnauthorizedResources.AddVariableGroupReference(variableGroup);
        }
      }
    }

    public JobExecutionContext CreateJobContext(
      PhaseExecutionContext context,
      JobInstance jobInstance)
    {
      JobExecutionContext jobContext = context.CreateJobContext(jobInstance);
      jobContext.Job.Definition.Id = jobContext.GetInstanceId();
      BuildOptions buildOptions = new BuildOptions();
      PipelineResources referenceResources = new PipelineBuilder((IPipelineContext) context).GetReferenceResources((IList<Step>) jobInstance.Definition.Steps.OfType<Step>().ToList<Step>(), jobInstance.Definition.Target);
      jobContext.ReferencedResources.MergeWith(referenceResources);
      this.UpdateJobContextVariablesFromJob(jobContext, jobInstance.Definition);
      AgentQueueTarget target1 = jobInstance.Definition.Target as AgentQueueTarget;
      AgentPoolTarget target2 = jobInstance.Definition.Target as AgentPoolTarget;
      if (target1?.Workspace != null)
        jobContext.Job.Definition.Workspace = target1.Workspace.Clone();
      if (target2?.Workspace != null)
        jobContext.Job.Definition.Workspace = target2.Workspace.Clone();
      if (target1?.Container != (ExpressionValue<string>) null)
      {
        string inputAlias = target1.Container.GetValue((IPipelineContext) jobContext).Value;
        string containerAlias = PhaseNode.ResolveContainerResource(jobContext, inputAlias);
        jobContext.Job.Definition.Container = containerAlias;
        PhaseNode.UpdateJobContextReferencedContainers(jobContext, containerAlias);
      }
      if (target1?.SidecarContainers != null)
      {
        foreach (KeyValuePair<string, ExpressionValue<string>> sidecarContainer in (IEnumerable<KeyValuePair<string, ExpressionValue<string>>>) target1?.SidecarContainers)
        {
          string inputAlias = sidecarContainer.Value.GetValue((IPipelineContext) context).Value;
          string containerAlias = PhaseNode.ResolveContainerResource(jobContext, inputAlias);
          jobContext.Job.Definition.SidecarContainers.Add(sidecarContainer.Key, containerAlias);
          PhaseNode.UpdateJobContextReferencedContainers(jobContext, containerAlias);
        }
      }
      foreach (JobStep step in (IEnumerable<JobStep>) jobInstance.Definition.Steps)
      {
        if (step.Target?.Target != null && step.Target.Target != PipelineConstants.StepContainerConstants.Host)
        {
          string target3 = step.Target.Target;
          jobInstance.Definition.Demands?.Add((Demand) AgentFeatureDemands.StepTargetVersionDemand());
          PhaseNode.UpdateJobContextReferencedContainers(jobContext, target3);
        }
      }
      if (context.Phase.Definition?.ExplicitResources != null)
      {
        ResourceReferences explicitResources = context.Phase.Definition.ExplicitResources;
        foreach (string repository in (IEnumerable<string>) explicitResources.Repositories)
          PhaseNode.UpdateJobContextReferencedRepositories(jobContext, repository);
        foreach (string queue in (IEnumerable<string>) explicitResources.Queues)
          PhaseNode.UpdateJobContextReferencedQueues(jobContext, queue);
      }
      return jobContext;
    }
  }
}
