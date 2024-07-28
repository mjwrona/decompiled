// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.ServerJobResponseConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal static class ServerJobResponseConverter
  {
    internal static SendJobResponse ToSendJobResponse(this ExecuteTaskResponse response)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      return new SendJobResponse((IDictionary<string, string>) new Dictionary<string, string>(), ServerJobResponseConverter.NormalizeToJobEvents(response.TaskEvents.All));
    }

    private static JobEventsConfig NormalizeToJobEvents(
      Dictionary<string, TaskEventConfig> taskEvents)
    {
      JobEventsConfig jobEvents = new JobEventsConfig();
      foreach (string key in taskEvents.Keys)
      {
        TaskEventConfig taskEvent = taskEvents[key];
        if (taskEvent.IsEnabled())
        {
          JobEventConfig jobEventConfig = new JobEventConfig(taskEvent.Timeout);
          switch (key)
          {
            case "TaskAssigned":
              jobEvents.JobAssigned = jobEventConfig;
              continue;
            case "TaskStarted":
              jobEvents.JobStarted = jobEventConfig;
              continue;
            case "TaskCompleted":
              jobEvents.JobCompleted = jobEventConfig;
              continue;
            default:
              continue;
          }
        }
      }
      return jobEvents;
    }
  }
}
