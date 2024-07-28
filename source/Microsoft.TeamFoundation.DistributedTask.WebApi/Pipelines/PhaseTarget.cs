// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PhaseTarget
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [KnownType(typeof (AgentQueueTarget))]
  [KnownType(typeof (AgentPoolTarget))]
  [KnownType(typeof (ServerTarget))]
  [KnownType(typeof (DeploymentGroupTarget))]
  [JsonConverter(typeof (PhaseTargetJsonConverter))]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class PhaseTarget
  {
    [DataMember(Name = "Demands", EmitDefaultValue = false)]
    private ISet<Demand> m_demands;

    protected PhaseTarget(PhaseTargetType type) => this.Type = type;

    protected PhaseTarget(PhaseTarget targetToClone)
    {
      this.Type = targetToClone.Type;
      this.ContinueOnError = targetToClone.ContinueOnError;
      this.TimeoutInMinutes = targetToClone.TimeoutInMinutes;
      this.CancelTimeoutInMinutes = targetToClone.CancelTimeoutInMinutes;
      ISet<Demand> demands = targetToClone.m_demands;
      if ((demands != null ? (demands.Count > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_demands = (ISet<Demand>) new HashSet<Demand>(targetToClone.m_demands.Select<Demand, Demand>((Func<Demand, Demand>) (x => x.Clone())));
    }

    [DataMember]
    public PhaseTargetType Type { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (ExpressionValueJsonConverter<bool>))]
    public ExpressionValue<bool> ContinueOnError { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (ExpressionValueJsonConverter<int>))]
    public ExpressionValue<int> TimeoutInMinutes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (ExpressionValueJsonConverter<int>))]
    public ExpressionValue<int> CancelTimeoutInMinutes { get; set; }

    public ISet<Demand> Demands
    {
      get
      {
        if (this.m_demands == null)
          this.m_demands = (ISet<Demand>) new HashSet<Demand>();
        return this.m_demands;
      }
    }

    public abstract PhaseTarget Clone();

    public abstract bool IsValid(TaskDefinition task);

    internal abstract JobExecutionContext CreateJobContext(
      PhaseExecutionContext context,
      string jobName,
      int attempt,
      bool continueOnError,
      int timeoutInMinutes,
      int cancelTimeoutInMinutes,
      IJobFactory jobFactory);

    internal void Validate(
      IPipelineContext context,
      BuildOptions buildOptions,
      ValidationResult result)
    {
      this.Validate(context, buildOptions, result, (IList<Step>) new List<Step>(), (ISet<Demand>) new HashSet<Demand>());
    }

    internal abstract ExpandPhaseResult Expand(
      PhaseExecutionContext context,
      bool continueOnError,
      int timeoutInMinutes,
      int cancelTimeoutInMinutes,
      IJobFactory jobFactory,
      JobExpansionOptions options);

    internal virtual void Validate(
      IPipelineContext context,
      BuildOptions buildOptions,
      ValidationResult result,
      IList<Step> steps,
      ISet<Demand> taskDemands)
    {
    }

    internal JobExecutionContext CreateJobContext(
      PhaseExecutionContext context,
      string jobName,
      int attempt,
      IJobFactory jobFactory)
    {
      bool continueOnError = context.Evaluate<bool>("ContinueOnError", this.ContinueOnError, false).Value;
      int timeoutInMinutes = context.Evaluate<int>("TimeoutInMinutes", this.TimeoutInMinutes, PipelineConstants.DefaultJobTimeoutInMinutes).Value;
      int cancelTimeoutInMinutes = context.Evaluate<int>("CancelTimeoutInMinutes", this.CancelTimeoutInMinutes, PipelineConstants.DefaultJobCancelTimeoutInMinutes).Value;
      return this.CreateJobContext(context, jobName, attempt, continueOnError, timeoutInMinutes, cancelTimeoutInMinutes, jobFactory);
    }

    internal ExpandPhaseResult Expand(
      PhaseExecutionContext context,
      IJobFactory jobFactory,
      JobExpansionOptions options)
    {
      bool continueOnError = context.Evaluate<bool>("ContinueOnError", this.ContinueOnError, false).Value;
      int timeoutInMinutes = context.Evaluate<int>("TimeoutInMinutes", this.TimeoutInMinutes, PipelineConstants.DefaultJobTimeoutInMinutes).Value;
      int cancelTimeoutInMinutes = context.Evaluate<int>("CancelTimeoutInMinutes", this.CancelTimeoutInMinutes, PipelineConstants.DefaultJobCancelTimeoutInMinutes).Value;
      return this.Expand(context, continueOnError, timeoutInMinutes, cancelTimeoutInMinutes, jobFactory, options);
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      ISet<Demand> demands = this.m_demands;
      if ((demands != null ? (demands.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_demands = (ISet<Demand>) null;
    }
  }
}
