// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.INotificationDiagnosticLog
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  public interface INotificationDiagnosticLog
  {
    string LogType { get; }

    Guid Source { get; }

    Guid Id { get; }

    DateTime StartTime { get; }

    DateTime EndTime { get; }

    string Description { get; }

    Guid ActivityId { get; }

    Dictionary<string, string> Properties { get; }

    List<NotificationDiagnosticLogMessage> Messages { get; }
  }
}
