// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.MultiMachineHandler
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Well known word in VSTS")]
  public class MultiMachineHandler : ParallelExecutionHandlerBase
  {
    private readonly MultiMachineInput input;

    public MultiMachineHandler(MultiMachineInput input, Action<string> traceMethod)
      : base((ParallelExecutionInputBase) input, traceMethod)
    {
      this.input = input;
    }

    protected override void Apply(PlanEnvironment environment, TaskOrchestrationContainer container) => this.ApplyDuplicateJobs(environment, container);

    private void ApplyDuplicateJobs(
      PlanEnvironment environment,
      TaskOrchestrationContainer container)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (container == null)
        throw new ArgumentNullException(nameof (container));
      if (this.input.MaxNumberOfAgents < 2)
        return;
      List<TaskOrchestrationItem> list = container.Children.Where<TaskOrchestrationItem>((Func<TaskOrchestrationItem, bool>) (x => x is TaskOrchestrationJob)).ToList<TaskOrchestrationItem>();
      container.Children.RemoveAll((Predicate<TaskOrchestrationItem>) (x => x is TaskOrchestrationJob));
      this.TraceInfo(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Duplicate : Number of Jobs to Duplicate : {0}", (object) list.Count));
      foreach (TaskOrchestrationJob orchestrationJob1 in list)
      {
        for (int index = 1; index <= this.input.MaxNumberOfAgents; ++index)
        {
          TaskOrchestrationJob orchestrationJob2 = orchestrationJob1.Clone();
          orchestrationJob2.InstanceId = Guid.NewGuid();
          orchestrationJob2.Name = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Machine, (object) index, (object) this.input.MaxNumberOfAgents);
          foreach (TaskInstance task in orchestrationJob2.Tasks)
            task.InstanceId = Guid.NewGuid();
          container.Children.Add((TaskOrchestrationItem) orchestrationJob2);
          this.TraceInfo(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Duplicate : New Job created {0}", (object) orchestrationJob2.InstanceId));
        }
      }
      this.TraceInfo(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Duplicate : Total  Jobs created in container is {0}", (object) container.Children.Count<TaskOrchestrationItem>((Func<TaskOrchestrationItem, bool>) (x => x is TaskOrchestrationJob))));
    }
  }
}
