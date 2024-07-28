// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.INotificationObjectTrace
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public interface INotificationObjectTrace
  {
    void LogNotificationError(string message, TeamFoundationNotification notification);

    void LogNotificationWarning(string message, TeamFoundationNotification notification);

    void TraceNotificationException(
      int tracepoint,
      Exception exception,
      TeamFoundationNotification notification);

    void TraceNotificationException(
      int tracepoint,
      Exception exception,
      string message,
      TeamFoundationNotification notification);
  }
}
