// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations.LegacyTfsBuildEventProcessor
// Assembly: Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6D9A1E77-52F6-4366-807D-D0FABA8CDE81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.TeamFoundation.WorkItemTracking.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations
{
  public class LegacyTfsBuildEventProcessor : ILegacyBuildEventProcessor, IVssFrameworkService
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
      object notificationEvent,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      statusCode = 0;
      properties = (ExceptionPropertyCollection) null;
      statusMessage = (string) null;
      BuildCompletionNotificationEvent notificationEvent1 = notificationEvent as BuildCompletionNotificationEvent;
      if (notificationType == NotificationType.Notification && notificationEvent1 != null && requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).IsInProcBuildCompletionNotificationEnabled)
      {
        string fullPath = notificationEvent1.Build.Definition.FullPath;
        new DataAccessLayerImpl(requestContext).AddNewBuild(notificationEvent1.Build.BuildNumber, BuildPath.GetTeamProject(fullPath), notificationEvent1.Build.Definition.Name);
      }
      return EventNotificationStatus.ActionPermitted;
    }
  }
}
