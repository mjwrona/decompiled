// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.RemoteServices.FrameworkProjectEventSubscriberBase
// Assembly: Microsoft.TeamFoundation.Server.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 836F660A-A756-49A7-82F0-68378533B43C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.RemoteServices.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.RemoteServices
{
  internal abstract class FrameworkProjectEventSubscriberBase : ISubscriber
  {
    private static Type[] s_subscribedTypes = new Type[2]
    {
      typeof (ProjectUpdatedEvent),
      typeof (ProjectDeletedEvent)
    };

    public abstract string Name { get; }

    public abstract SubscriberPriority Priority { get; }

    public Type[] SubscribedTypes() => FrameworkProjectEventSubscriberBase.s_subscribedTypes;

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
      ProjectUpdatedEvent projectUpdatedEvent = notificationEventArgs as ProjectUpdatedEvent;
      ProjectDeletedEvent projectDeletedEvent = notificationEventArgs as ProjectDeletedEvent;
      if (projectUpdatedEvent != null)
      {
        this.ProcessEvent(requestContext, projectUpdatedEvent.ToProjectInfo());
      }
      else
      {
        if (projectDeletedEvent == null)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected notification type: {0}.", (object) notificationEventArgs.GetType()));
        this.ProcessEvent(requestContext, projectDeletedEvent.ToProjectInfo());
      }
      return EventNotificationStatus.ActionPermitted;
    }

    protected abstract void ProcessEvent(IVssRequestContext requestContext, ProjectInfo projectInfo);
  }
}
