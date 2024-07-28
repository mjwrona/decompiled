// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.EventBacklogStatus
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class EventBacklogStatus
  {
    [DataMember(EmitDefaultValue = false)]
    public Guid JobId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Publisher { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int UnprocessedEvents { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime OldestPendingEventTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime LastEventProcessedTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime LastEventBatchStartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime LastJobProcessedTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime LastJobBatchStartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CaptureTime { get; set; }
  }
}
