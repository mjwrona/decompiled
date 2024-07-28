// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NotificationSubscriberBase`1
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services
{
  public abstract class NotificationSubscriberBase<TEventArg> : ISubscriber
  {
    private static readonly Type[] SubscribedTypesArray = new Type[1]
    {
      typeof (TEventArg)
    };

    public NotificationSubscriberBase()
    {
      this.Name = this.GetType().Name;
      this.Priority = SubscriberPriority.Normal;
    }

    public NotificationSubscriberBase(string name, SubscriberPriority priority)
    {
      ArgumentUtility.CheckForNull<string>(name, nameof (name));
      ArgumentUtility.CheckForDefinedEnum<SubscriberPriority>(priority, nameof (priority));
      this.Name = name;
      this.Priority = priority;
    }

    public string Name { get; protected set; }

    public SubscriberPriority Priority { get; protected set; }

    public abstract void ProcessEvent(IVssRequestContext requestContext, TEventArg eventArgs);

    Type[] ISubscriber.SubscribedTypes() => NotificationSubscriberBase<TEventArg>.SubscribedTypesArray;

    EventNotificationStatus ISubscriber.ProcessEvent(
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
      if (!(notificationEventArgs is TEventArg) || notificationType != NotificationType.Notification)
        return EventNotificationStatus.ActionPermitted;
      TEventArg eventArgs = (TEventArg) notificationEventArgs;
      try
      {
        this.ProcessEvent(requestContext, eventArgs);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "NotificationSubscriber", "Notification", ex);
      }
      return EventNotificationStatus.ActionPermitted;
    }
  }
}
