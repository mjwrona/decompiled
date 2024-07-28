// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentPoolTarget
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
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
  public class AgentPoolTarget : PhaseTarget
  {
    [DataMember(Name = "AgentIds", EmitDefaultValue = false)]
    private List<int> m_agentIds;

    public AgentPoolTarget()
      : base(PhaseTargetType.Pool)
    {
    }

    private AgentPoolTarget(AgentPoolTarget targetToClone)
      : base((PhaseTarget) targetToClone)
    {
      this.Pool = targetToClone.Pool?.Clone();
      this.Workspace = targetToClone.Workspace?.Clone();
      if (targetToClone.AgentSpecification != null)
        this.AgentSpecification = new JObject(targetToClone.AgentSpecification);
      List<int> agentIds = targetToClone.m_agentIds;
      // ISSUE: explicit non-virtual call
      if ((agentIds != null ? (__nonvirtual (agentIds.Count) > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_agentIds = targetToClone.m_agentIds;
    }

    [DataMember(EmitDefaultValue = false)]
    public AgentPoolReference Pool { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public JObject AgentSpecification { get; set; }

    public List<int> AgentIds
    {
      get
      {
        if (this.m_agentIds == null)
          this.m_agentIds = new List<int>();
        return this.m_agentIds;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public WorkspaceOptions Workspace { get; set; }

    public override PhaseTarget Clone() => (PhaseTarget) new AgentPoolTarget(this);

    public override bool IsValid(TaskDefinition task)
    {
      ArgumentUtility.CheckForNull<TaskDefinition>(task, nameof (task));
      return task.RunsOn.Contains<string>("Agent", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    internal override void Validate(
      IPipelineContext context,
      BuildOptions buildOptions,
      ValidationResult result,
      IList<Step> steps,
      ISet<Demand> taskDemands)
    {
      int num = 0;
      string str = (string) null;
      AgentPoolReference pool = this.Pool;
      if (pool != null)
      {
        num = pool.Id;
        str = pool.Name?.GetValue(context)?.Value;
      }
      if (num == 0 && string.IsNullOrEmpty(str) && buildOptions.ValidateResources)
      {
        result.Errors.Add(new PipelineValidationError(PipelineStrings.QueueNotDefined()));
      }
      else
      {
        result.AddPoolReference(num, str);
        if (!buildOptions.ValidateResources)
          return;
        TaskAgentPool taskAgentPool = (TaskAgentPool) null;
        IResourceStore resourceStore = context.ResourceStore;
        if (resourceStore != null)
        {
          if (num != 0)
          {
            taskAgentPool = resourceStore.GetPool(num);
            if (taskAgentPool == null)
            {
              result.UnauthorizedResources.Pools.Add(new AgentPoolReference()
              {
                Id = num
              });
              result.Errors.Add(new PipelineValidationError(PipelineStrings.QueueNotFound((object) num)));
            }
          }
          else if (!string.IsNullOrEmpty(str))
          {
            taskAgentPool = resourceStore.GetPool(str);
            if (taskAgentPool == null)
            {
              ISet<AgentPoolReference> pools = result.UnauthorizedResources.Pools;
              AgentPoolReference agentPoolReference = new AgentPoolReference();
              agentPoolReference.Name = (ExpressionValue<string>) str;
              pools.Add(agentPoolReference);
              result.Errors.Add(new PipelineValidationError(PipelineStrings.QueueNotFound((object) str)));
            }
          }
        }
        if (taskAgentPool == null)
          return;
        this.Pool.Id = taskAgentPool.Id;
        this.Pool.Name = (ExpressionValue<string>) taskAgentPool.Name;
      }
    }

    internal override JobExecutionContext CreateJobContext(
      PhaseExecutionContext context,
      string jobName,
      int attempt,
      bool continueOnError,
      int timeoutInMinutes,
      int cancelTimeoutInMinutes,
      IJobFactory jobFactory)
    {
      throw new NotSupportedException(nameof (AgentPoolTarget));
    }

    internal override ExpandPhaseResult Expand(
      PhaseExecutionContext context,
      bool continueOnError,
      int timeoutInMinutes,
      int cancelTimeoutInMinutes,
      IJobFactory jobFactory,
      JobExpansionOptions options)
    {
      throw new NotSupportedException(nameof (AgentPoolTarget));
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<int> agentIds = this.m_agentIds;
      // ISSUE: explicit non-virtual call
      if ((agentIds != null ? (__nonvirtual (agentIds.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_agentIds = (List<int>) null;
    }
  }
}
