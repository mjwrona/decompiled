// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces.ILegacyTeamCreationSubscriberService
// Assembly: Microsoft.Azure.Boards.LegacyInterfaces, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C0E0C41-D39C-453E-A6CF-32A7C57153EE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.LegacyInterfaces.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces
{
  [DefaultServiceImplementation("Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations.LegacyTfsTeamCreationSubscriberService, Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations")]
  public interface ILegacyTeamCreationSubscriberService : IVssFrameworkService
  {
    EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      Action<IVssRequestContext, IdentityDescriptor, HashSet<string>> processChangedIdentityAction,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties);
  }
}
