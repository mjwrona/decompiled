// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BasicEventSubscriber`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class BasicEventSubscriber<T> : ISubscriber
  {
    public string Name => this.GetType().Name;

    public SubscriberPriority Priority => SubscriberPriority.Normal;

    public Type[] SubscribedTypes() => new Type[1]
    {
      typeof (T)
    };

    public EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      statusCode = 0;
      statusMessage = (string) null;
      properties = (ExceptionPropertyCollection) null;
      this.ProcessEvent(requestContext, (T) notificationEventArgs);
      return EventNotificationStatus.ActionPermitted;
    }

    protected abstract void ProcessEvent(IVssRequestContext requestContext, T notificationEventArgs);
  }
}
