// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.DeploymentGroupTarget
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  internal class DeploymentGroupTarget : PhaseTarget
  {
    [DataMember(Name = "Tags", EmitDefaultValue = false)]
    private ISet<string> m_tags;
    [DataMember(Name = "TargetIds")]
    private List<int> m_targetIds;

    public DeploymentGroupTarget()
      : base(PhaseTargetType.DeploymentGroup)
    {
    }

    private DeploymentGroupTarget(DeploymentGroupTarget targetToClone)
      : base((PhaseTarget) targetToClone)
    {
      this.DeploymentGroup = targetToClone.DeploymentGroup?.Clone();
      this.Execution = targetToClone.Execution?.Clone();
      if (targetToClone.m_tags == null || targetToClone.m_tags.Count <= 0)
        return;
      this.m_tags = (ISet<string>) new HashSet<string>((IEnumerable<string>) targetToClone.m_tags, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember]
    public DeploymentGroupReference DeploymentGroup { get; set; }

    public ISet<string> Tags
    {
      get
      {
        if (this.m_tags == null)
          this.m_tags = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_tags;
      }
    }

    public List<int> TargetIds
    {
      get
      {
        if (this.m_targetIds == null)
          this.m_targetIds = new List<int>();
        return this.m_targetIds;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public DeploymentExecutionOptions Execution { get; set; }

    public override PhaseTarget Clone() => (PhaseTarget) new DeploymentGroupTarget(this);

    public override bool IsValid(TaskDefinition task) => task.RunsOn.Contains<string>("DeploymentGroup", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    internal override void Validate(
      IPipelineContext context,
      BuildOptions buildOptions,
      ValidationResult result,
      IList<Step> steps,
      ISet<Demand> taskDemands)
    {
      this.Execution?.Validate(context, result);
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
      context.Trace?.EnterProperty(nameof (CreateJobContext));
      JobExecutionContext jobContext = new ParallelExecutionOptions().CreateJobContext(context, jobName, attempt, (ExpressionValue<string>) null, (IDictionary<string, ExpressionValue<string>>) null, continueOnError, timeoutInMinutes, cancelTimeoutInMinutes, jobFactory);
      IPipelineTraceWriter trace = context.Trace;
      if (trace == null)
        return jobContext;
      trace.LeaveProperty(nameof (CreateJobContext));
      return jobContext;
    }

    internal override ExpandPhaseResult Expand(
      PhaseExecutionContext context,
      bool continueOnError,
      int timeoutInMinutes,
      int cancelTimeoutInMinutes,
      IJobFactory jobFactory,
      JobExpansionOptions options)
    {
      context.Trace?.EnterProperty(nameof (Expand));
      ExpandPhaseResult expandPhaseResult = new ParallelExecutionOptions().Expand(context, (ExpressionValue<string>) null, (IDictionary<string, ExpressionValue<string>>) null, continueOnError, timeoutInMinutes, cancelTimeoutInMinutes, jobFactory, options);
      IPipelineTraceWriter trace = context.Trace;
      if (trace == null)
        return expandPhaseResult;
      trace.LeaveProperty(nameof (Expand));
      return expandPhaseResult;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      ISet<string> tags = this.m_tags;
      if ((tags != null ? (tags.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_tags = (ISet<string>) null;
      List<int> targetIds = this.m_targetIds;
      // ISSUE: explicit non-virtual call
      if ((targetIds != null ? (__nonvirtual (targetIds.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_targetIds = (List<int>) null;
    }
  }
}
