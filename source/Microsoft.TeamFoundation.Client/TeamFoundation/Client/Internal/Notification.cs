// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Internal.Notification
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum Notification
  {
    TeamExplorerNotificationBegin = 1024, // 0x00000400
    TeamFoundationNotificationBegin = 1024, // 0x00000400
    TeamExplorerFavoriteCreated = 1025, // 0x00000401
    TeamExplorerFavoriteDeleted = 1026, // 0x00000402
    TeamExplorerFavoriteRenamed = 1027, // 0x00000403
    TeamExplorerNotificationEnd = 1123, // 0x00000463
    VersionControlNotificationBegin = 1124, // 0x00000464
    VersionControlWorkspaceCreated = 1125, // 0x00000465
    VersionControlWorkspaceDeleted = 1126, // 0x00000466
    VersionControlWorkspaceChanged = 1127, // 0x00000467
    VersionControlPendingChangesChanged = 1128, // 0x00000468
    VersionControlGetCompleted = 1129, // 0x00000469
    VersionControlChangesetReconciled = 1130, // 0x0000046A
    VersionControlFolderContentChanged = 1131, // 0x0000046B
    VersionControlManualMergeClosed = 1132, // 0x0000046C
    VersionControlLocalWorkspaceScan = 1133, // 0x0000046D
    VersionControlNotificationEnd = 1223, // 0x000004C7
    WorkItemTrackingNotificationBegin = 1224, // 0x000004C8
    WorkItemTrackingUserQueryCreated = 1225, // 0x000004C9
    WorkItemTrackingTeamQueryCreated = 1226, // 0x000004CA
    WorkItemTrackingUserQueryDeleted = 1227, // 0x000004CB
    WorkItemTrackingTeamQueryDeleted = 1228, // 0x000004CC
    WorkItemTrackingUserQueryRenamed = 1229, // 0x000004CD
    WorkItemTrackingTeamQueryRenamed = 1230, // 0x000004CE
    WorkItemTrackingNotificationEnd = 1323, // 0x0000052B
    ReportingNotificationBegin = 1324, // 0x0000052C
    ReportingNotificationEnd = 1423, // 0x0000058F
    DocumentsNotificationBegin = 1424, // 0x00000590
    DocumentsNotificationEnd = 1523, // 0x000005F3
    BuildNotificationBegin = 1524, // 0x000005F4
    BuildNotificationBuildCreated = 1525, // 0x000005F5
    BuildNotificationBuildDeleted = 1526, // 0x000005F6
    BuildNotificationBuildRenamed = 1527, // 0x000005F7
    BuildNotificationBuildStarted = 1528, // 0x000005F8
    BuildNotificationBuildStopped = 1529, // 0x000005F9
    BuildNotificationEnd = 1623, // 0x00000657
    TfsConnectionNotificationBegin = 1624, // 0x00000658
    TfsConnectionUserChanged = 1625, // 0x00000659
    TfsConnectionNotificationEnd = 1723, // 0x000006BB
    TeamFoundationNotificationEnd = 1724, // 0x000006BC
  }
}
