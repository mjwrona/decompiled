// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.SubOrchestrationFailedException
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  [Serializable]
  public class SubOrchestrationFailedException : OrchestrationException
  {
    public SubOrchestrationFailedException()
    {
    }

    public SubOrchestrationFailedException(string reason)
      : base(reason)
    {
    }

    public SubOrchestrationFailedException(string reason, Exception innerException)
      : base(reason, innerException)
    {
    }

    public SubOrchestrationFailedException(
      int eventId,
      int scheduleId,
      string name,
      string version,
      string reason,
      Exception cause)
      : base(eventId, reason, cause)
    {
      this.ScheduleId = scheduleId;
      this.Name = name;
      this.Version = version;
    }

    protected SubOrchestrationFailedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public int ScheduleId { get; set; }

    public string Name { get; set; }

    public string Version { get; set; }
  }
}
