// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamNotificationManager
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class TeamNotificationManager : ISubscriber
  {
    private static readonly Type[] s_subscribedTypes = new Type[1]
    {
      typeof (PreGroupDeletionNotification)
    };

    string ISubscriber.Name => "Team Foundation Team Service: Team Deletion Notification Provider";

    SubscriberPriority ISubscriber.Priority => SubscriberPriority.Normal;

    Type[] ISubscriber.SubscribedTypes() => TeamNotificationManager.s_subscribedTypes;

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
      EventNotificationStatus notificationStatus = EventNotificationStatus.ActionPermitted;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        PreGroupDeletionNotification deletionNotification = (PreGroupDeletionNotification) notificationEventArgs;
        if (requestContext.GetService<ITeamService>().IsDefaultTeam(requestContext, deletionNotification.Group.TeamFoundationId))
        {
          statusMessage = FrameworkResources.CanNotDeleteDefaultTeam((object) deletionNotification.Group.DisplayName);
          notificationStatus = EventNotificationStatus.ActionDenied;
        }
      }
      return notificationStatus;
    }
  }
}
