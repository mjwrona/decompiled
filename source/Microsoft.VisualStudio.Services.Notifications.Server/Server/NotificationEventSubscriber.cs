// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationEventSubscriber
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationEventSubscriber : ISubscriber
  {
    private Type[] s_subscribedTypes = new Type[1]
    {
      typeof (VssNotificationEvent)
    };

    public string Name => nameof (NotificationEventSubscriber);

    public SubscriberPriority Priority => SubscriberPriority.Normal;

    public EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      if (!(notificationEventArgs is VssNotificationEvent theEvent))
        throw new ArgumentException("Only NotificationEvent is supported");
      statusCode = 0;
      statusMessage = (string) null;
      properties = (ExceptionPropertyCollection) null;
      requestContext.GetService<INotificationEventService>().PublishEvent(requestContext, theEvent);
      return EventNotificationStatus.ActionPermitted;
    }

    public Type[] SubscribedTypes() => this.s_subscribedTypes;
  }
}
