// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ServerTarget
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ServerTarget : PhaseTarget
  {
    public ServerTarget()
      : base(PhaseTargetType.Server)
    {
    }

    private ServerTarget(ServerTarget targetToClone)
      : base((PhaseTarget) targetToClone)
    {
      this.Execution = targetToClone.Execution?.Clone();
    }

    [DataMember(EmitDefaultValue = false)]
    public ParallelExecutionOptions Execution { get; set; }

    public override PhaseTarget Clone() => (PhaseTarget) new ServerTarget(this);

    public override bool IsValid(TaskDefinition task) => task.RunsOn.Contains<string>("Server", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) || task.RunsOn.Contains<string>("ServerGate", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

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
      JobExecutionContext jobContext = (this.Execution ?? new ParallelExecutionOptions()).CreateJobContext(context, jobName, attempt, (ExpressionValue<string>) null, (IDictionary<string, ExpressionValue<string>>) null, continueOnError, timeoutInMinutes, cancelTimeoutInMinutes, jobFactory);
      context.Trace?.LeaveProperty(nameof (CreateJobContext));
      jobContext.Variables[WellKnownDistributedTaskVariables.EnableAccessToken] = (VariableValue) bool.TrueString;
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
      ExpandPhaseResult expandPhaseResult = (this.Execution ?? new ParallelExecutionOptions()).Expand(context, (ExpressionValue<string>) null, (IDictionary<string, ExpressionValue<string>>) null, continueOnError, timeoutInMinutes, cancelTimeoutInMinutes, jobFactory, options);
      IPipelineTraceWriter trace = context.Trace;
      if (trace == null)
        return expandPhaseResult;
      trace.LeaveProperty(nameof (Expand));
      return expandPhaseResult;
    }
  }
}
