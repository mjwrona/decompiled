// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.History.HistoryEventJsonConverter
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.Orchestration.History
{
  internal class HistoryEventJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (HistoryEvent).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JObject jobject = JObject.Load(reader);
      JToken jtoken;
      if (!jobject.TryGetValue("EventType", StringComparison.OrdinalIgnoreCase, out jtoken))
        return (object) null;
      EventType result;
      if (jtoken.Type == JTokenType.Integer)
        result = (EventType) (int) jtoken;
      else if (jtoken.Type != JTokenType.String || !Enum.TryParse<EventType>((string) jtoken, out result))
        return (object) null;
      HistoryEvent target = (HistoryEvent) null;
      switch (result)
      {
        case EventType.ExecutionStarted:
          target = (HistoryEvent) new ExecutionStartedEvent();
          break;
        case EventType.ExecutionCompleted:
          target = (HistoryEvent) new ExecutionCompletedEvent();
          break;
        case EventType.ExecutionTerminated:
          target = (HistoryEvent) new ExecutionTerminatedEvent();
          break;
        case EventType.TaskScheduled:
          target = (HistoryEvent) new TaskScheduledEvent();
          break;
        case EventType.TaskCompleted:
          target = (HistoryEvent) new TaskCompletedEvent();
          break;
        case EventType.TaskFailed:
          target = (HistoryEvent) new TaskFailedEvent();
          break;
        case EventType.SubOrchestrationInstanceCreated:
          target = (HistoryEvent) new SubOrchestrationInstanceCreatedEvent();
          break;
        case EventType.SubOrchestrationInstanceCompleted:
          target = (HistoryEvent) new SubOrchestrationInstanceCompletedEvent();
          break;
        case EventType.SubOrchestrationInstanceFailed:
          target = (HistoryEvent) new SubOrchestrationInstanceFailedEvent();
          break;
        case EventType.TimerCreated:
          target = (HistoryEvent) new TimerCreatedEvent();
          break;
        case EventType.TimerFired:
          target = (HistoryEvent) new TimerFiredEvent();
          break;
        case EventType.OrchestratorStarted:
          target = (HistoryEvent) new OrchestratorStartedEvent();
          break;
        case EventType.OrchestratorCompleted:
          target = (HistoryEvent) new OrchestratorCompletedEvent();
          break;
        case EventType.EventRaised:
          target = (HistoryEvent) new EventRaisedEvent();
          break;
        case EventType.ContinueAsNew:
          target = (HistoryEvent) new ContinueAsNewEvent();
          break;
        case EventType.GenericEvent:
          target = (HistoryEvent) new GenericEvent();
          break;
      }
      if (target != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, (object) target);
      }
      return (object) target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
