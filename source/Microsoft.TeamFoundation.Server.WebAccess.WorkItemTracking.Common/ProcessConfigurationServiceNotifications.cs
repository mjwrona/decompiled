// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProcessConfigurationServiceNotifications
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal static class ProcessConfigurationServiceNotifications
  {
    public static readonly Dictionary<Guid, string> ProjectConfigurationServiceNotificationsMap = new Dictionary<Guid, string>();

    static ProcessConfigurationServiceNotifications()
    {
      ProcessConfigurationServiceNotifications.ProjectConfigurationServiceNotificationsMap[SqlNotificationEventClasses.ProjectSettingsChanged] = "ProjectSettingsChanged";
      ProcessConfigurationServiceNotifications.ProjectConfigurationServiceNotificationsMap[SqlNotificationEventClasses.ProjectsProcessMigrated] = "ProjectsProcessMigrated";
      ProcessConfigurationServiceNotifications.ProjectConfigurationServiceNotificationsMap[DBNotificationIds.WorkItemTrackingProvisionedMetadataChanged] = "WorkItemTrackingProvisionedMetadataChanged";
      ProcessConfigurationServiceNotifications.ProjectConfigurationServiceNotificationsMap[DBNotificationIds.WorkItemStateDefinitionModified] = "WorkItemStateDefinitionModified";
      ProcessConfigurationServiceNotifications.ProjectConfigurationServiceNotificationsMap[DBNotificationIds.WorkItemTypeBehaviorReferenceModified] = "WorkItemTypeBehaviorReferenceModified";
      ProcessConfigurationServiceNotifications.ProjectConfigurationServiceNotificationsMap[DBNotificationIds.ProcessWorkItemMetadataDeleted] = "ProcessWorkItemMetadataDeleted";
      ProcessConfigurationServiceNotifications.ProjectConfigurationServiceNotificationsMap[SpecialGuids.WorkItemTypeletDeleted] = "WorkItemTypeletDeleted";
    }
  }
}
