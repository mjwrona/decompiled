// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.TaskContext
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Microsoft.VisualStudio.Services.Orchestration.History;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public class TaskContext
  {
    public TaskContext(TaskMessage message)
    {
      this.Message = message;
      this.EventData = message.Event as TaskScheduledEvent;
    }

    public TaskMessage Message { get; private set; }

    public TaskScheduledEvent EventData { get; private set; }

    public OrchestrationInstance OrchestrationInstance => this.Message.OrchestrationInstance;
  }
}
