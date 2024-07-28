// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.EventExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.Notifications;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public static class EventExtensions
  {
    public static VssNotificationEvent ToVssNotificationEvent(this WorkItemChangedEvent e)
    {
      VssNotificationEvent notificationEvent = new VssNotificationEvent((object) e);
      if (!e.HasOnlyLinkUpdates)
        notificationEvent.SourceEventCreatedTime = new DateTime?(e.ChangedDateUtc);
      notificationEvent.ItemId = e.WorkItemId;
      notificationEvent.Actors.AddRange((IEnumerable<EventActor>) e.Actors);
      notificationEvent.AddArtifactUri(e.ArtifactUri);
      notificationEvent.AddScope(VssNotificationEvent.ScopeNames.Project, e.ProjectId, e.PortfolioProject);
      return notificationEvent;
    }
  }
}
