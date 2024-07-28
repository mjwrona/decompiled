// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.ExecutionHandler
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public abstract class ExecutionHandler
  {
    private Action<string> trace;
    private ExecutionInput input;

    protected ExecutionHandler(ExecutionInput input, Action<string> traceMethod)
    {
      this.input = input;
      this.trace = traceMethod;
    }

    public void ApplyParallelExecution(
      PlanEnvironment environment,
      TaskOrchestrationContainer container)
    {
      this.Apply(environment, container);
      this.UpdateVariables(container);
      this.UpdateContainerProperties(container);
    }

    protected void TraceInfo(string info) => this.trace(info);

    public abstract IDictionary<string, string> GetInvalidInputs(
      IDictionary<string, ConfigurationVariableValue> variables,
      IVssRequestContext context);

    protected abstract void Apply(PlanEnvironment environment, TaskOrchestrationContainer container);

    protected virtual void UpdateVariables(TaskOrchestrationContainer container)
    {
      if (container == null)
        throw new ArgumentNullException(nameof (container));
      int num1 = container.Children.Count<TaskOrchestrationItem>((Func<TaskOrchestrationItem, bool>) (x => x is TaskOrchestrationJob));
      int num2 = 1;
      foreach (TaskOrchestrationJob orchestrationJob in container.Children.Where<TaskOrchestrationItem>((Func<TaskOrchestrationItem, bool>) (x => x is TaskOrchestrationJob)))
      {
        orchestrationJob.Variables["system.totalJobsInPhase"] = num1.ToString((IFormatProvider) CultureInfo.CurrentCulture);
        orchestrationJob.Variables["system.jobPositionInPhase"] = num2++.ToString((IFormatProvider) CultureInfo.CurrentCulture);
        orchestrationJob.Variables["system.parallelexecutiontype"] = this.input.ParallelExecutionType.ToString();
      }
    }

    protected abstract void UpdateContainerProperties(TaskOrchestrationContainer container);
  }
}
