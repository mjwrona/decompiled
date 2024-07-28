// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations.LegacyTfsTeamCreationSubscriberService
// Assembly: Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6D9A1E77-52F6-4366-807D-D0FABA8CDE81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations
{
  public class LegacyTfsTeamCreationSubscriberService : 
    ILegacyTeamCreationSubscriberService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      Action<IVssRequestContext, IdentityDescriptor, HashSet<string>> processChangedIdentityAction,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      EventNotificationStatus notificationStatus = EventNotificationStatus.ActionPermitted;
      statusCode = 0;
      statusMessage = (string) null;
      properties = (ExceptionPropertyCollection) null;
      TeamFoundationIdentityPropertiesUpdateEvent propertiesUpdateEvent = (TeamFoundationIdentityPropertiesUpdateEvent) notificationEventArgs;
      if (notificationType == NotificationType.Notification && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && (propertiesUpdateEvent.PropertyScope == IdentityPropertyScope.Both || propertiesUpdateEvent.PropertyScope == IdentityPropertyScope.Local) && propertiesUpdateEvent.Identities != null)
      {
        foreach (TeamFoundationIdentity identity in propertiesUpdateEvent.Identities)
        {
          HashSet<string> modifiedPropertiesLog = identity.GetModifiedPropertiesLog(IdentityPropertyScope.Local);
          processChangedIdentityAction(requestContext, identity.Descriptor, modifiedPropertiesLog);
        }
      }
      return notificationStatus;
    }
  }
}
