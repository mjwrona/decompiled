// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BisEvents
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class BisEvents
  {
    private static void FireEvent(
      IVssRequestContext requestContext,
      VssNotificationEvent notificationEvent)
    {
      requestContext.TraceEnter(0, "Build", "Notification", nameof (FireEvent));
      try
      {
        requestContext.Elevate().GetService<NotificationEventService>().PublishSystemEvent(requestContext, notificationEvent);
        requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Fired event '{0}'", (object) notificationEvent.Data.GetType().Name);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Build", "Notification", ex);
      }
      requestContext.TraceLeave(0, "Build", "Notification", nameof (FireEvent));
    }

    internal static void RaiseBuildResourceChangedEvent(
      IVssRequestContext requestContext,
      VssNotificationEvent changedEvent)
    {
      BisEvents.FireEvent(requestContext, changedEvent);
    }

    internal static void RaiseBuildDefinitionChangedEvent(
      IVssRequestContext requestContext,
      VssNotificationEvent changedEvent)
    {
      BisEvents.FireEvent(requestContext, changedEvent);
    }

    public static void RaiseBuildDefinitionUpgradeCompletionEvent(
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      BuildDefinitionUpgradeCompletionEvent data = new BuildDefinitionUpgradeCompletionEvent();
      ILocationService service = requestContext.GetService<ILocationService>();
      data.TeamProjectCollectionUrl = service.GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker);
      data.Uri = definition.Uri;
      data.TeamProject = definition.TeamProject.Name;
      data.DefinitionPath = definition.FullPath;
      data.Title = ResourceStrings.BuildDefinitionUpgradeCompletionEmailTitle((object) definition.TeamProject.Name, (object) definition.Name);
      data.TimeZone = BisEvents.GetLocalTimeZoneName(DateTime.Now);
      data.TimeZoneOffset = TFCommonUtil.GetLocalTimeZoneOffset(DateTime.Now);
      VssNotificationEvent notificationEvent = new VssNotificationEvent((object) data)
      {
        ItemId = data.Name
      };
      notificationEvent.AddArtifactUri(data.Uri);
      notificationEvent.AddSystemInitiatorActor();
      notificationEvent.AddScope(VssNotificationEvent.ScopeNames.Project, definition.TeamProject.Id, definition.TeamProject.Name);
      BisEvents.FireEvent(requestContext, notificationEvent);
    }

    internal static string GetLocalTimeZoneName(DateTime date) => TimeZone.CurrentTimeZone.IsDaylightSavingTime(date) ? TimeZone.CurrentTimeZone.DaylightName : TimeZone.CurrentTimeZone.StandardName;
  }
}
