// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationDiagnosticLog
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public abstract class NotificationDiagnosticLog : INotificationDiagnosticLog
  {
    [DataMember]
    public string LogType { get; set; }

    [DataMember]
    public Guid Source { get; set; }

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public DateTime StartTime { get; set; }

    [DataMember]
    public DateTime EndTime { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public int Warnings { get; set; }

    [DataMember]
    public int Errors { get; set; }

    [DataMember]
    public Guid ActivityId { get; set; }

    [DataMember]
    public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

    [DataMember]
    public List<NotificationDiagnosticLogMessage> Messages { get; } = new List<NotificationDiagnosticLogMessage>();
  }
}
