// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationTracerQueue
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class NotificationTracerQueue : SimpleTraceQueue
  {
    protected override void TraceMessage(
      IVssRequestContext requestContext,
      string area,
      string layer,
      SimpleTraceQueueMessage msg)
    {
      TeamFoundationNotification notification1 = this.Notification;
      bool flag = notification1 != null && notification1.IsSubscriptionTracingEnabled();
      if (((msg.Level == TraceLevel.Warning ? 1 : (msg.Level == TraceLevel.Error ? 1 : 0)) | (flag ? 1 : 0)) != 0)
      {
        TeamFoundationNotification notification2 = this.Notification;
        if (notification2 != null)
          notification2.AddMessage(msg.Level, msg.Message);
      }
      requestContext.Trace(msg.Tracepoint, msg.Level, "Notifications", "AsyncTraces", string.Format("Notification: {0} {1}", (object) this.Notification?.Id, (object) msg.Message));
    }

    public TeamFoundationNotification Notification { get; set; }
  }
}
