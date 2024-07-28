// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.TaskMessage
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Microsoft.VisualStudio.Services.Orchestration.History;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  [DataContract]
  public class TaskMessage
  {
    [DataMember]
    public OrchestrationInstance OrchestrationInstance;
    [DataMember]
    public HistoryEvent Event;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? FireAt;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DispatcherType;

    public TaskMessage()
    {
    }

    public TaskMessage(OrchestrationInstance instance, HistoryEvent historyEvent)
      : this(instance, historyEvent, (string) null)
    {
    }

    public TaskMessage(
      OrchestrationInstance instance,
      HistoryEvent historyEvent,
      string dispatcherType)
      : this(instance, historyEvent, dispatcherType, new DateTime?())
    {
    }

    public TaskMessage(OrchestrationInstance instance, HistoryEvent historyEvent, DateTime? fireAt)
      : this(instance, historyEvent, (string) null, fireAt)
    {
    }

    public TaskMessage(
      OrchestrationInstance instance,
      HistoryEvent historyEvent,
      string dispatcherType,
      DateTime? fireAt)
    {
      this.OrchestrationInstance = instance;
      this.DispatcherType = dispatcherType;
      this.Event = historyEvent;
      this.FireAt = fireAt;
    }
  }
}
