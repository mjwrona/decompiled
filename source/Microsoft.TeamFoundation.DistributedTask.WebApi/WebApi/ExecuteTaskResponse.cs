// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ExecuteTaskResponse
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [ClientIgnore]
  [DataContract]
  public sealed class ExecuteTaskResponse
  {
    private TaskEventsConfig m_taskEventsConfig;

    [JsonConstructor]
    public ExecuteTaskResponse()
    {
    }

    public ExecuteTaskResponse(EventsConfig eventsConfig)
    {
      switch (eventsConfig)
      {
        case JobEventsConfig jobEventsConfig:
          this.m_taskEventsConfig = ExecuteTaskResponse.ToTaskEvents(jobEventsConfig);
          break;
        case TaskEventsConfig taskEventsConfig:
          this.m_taskEventsConfig = taskEventsConfig;
          break;
      }
    }

    [DataMember]
    public TaskEventsConfig TaskEvents
    {
      get
      {
        if (this.m_taskEventsConfig == null)
          this.m_taskEventsConfig = new TaskEventsConfig();
        return this.m_taskEventsConfig;
      }
      internal set => this.m_taskEventsConfig = value;
    }

    [DataMember]
    public bool WaitForLocalExecutionComplete { get; set; }

    internal static TaskEventsConfig ToTaskEvents(JobEventsConfig jobEventsConfig)
    {
      Dictionary<string, JobEventConfig> all = jobEventsConfig.All;
      TaskEventsConfig taskEvents = new TaskEventsConfig();
      foreach (string key in all.Keys)
      {
        TaskEventConfig taskEventConfig = new TaskEventConfig(all[key].Timeout);
        switch (key)
        {
          case "JobAssigned":
            taskEvents.TaskAssigned = taskEventConfig;
            continue;
          case "JobStarted":
            taskEvents.TaskStarted = taskEventConfig;
            continue;
          case "JobCompleted":
            taskEvents.TaskCompleted = taskEventConfig;
            continue;
          default:
            continue;
        }
      }
      return taskEvents;
    }
  }
}
