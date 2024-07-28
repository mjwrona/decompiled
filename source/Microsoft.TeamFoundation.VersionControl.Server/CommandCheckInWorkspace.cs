// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandCheckInWorkspace
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.VersionControl.Common;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandCheckInWorkspace : CommandCheckIn
  {
    public CommandCheckInWorkspace(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public CheckinResult Execute(
      string workspaceName,
      string ownerName,
      string[] serverItems,
      CheckinNotificationInfo checkinNotificationInfo,
      CheckInOptions2 checkinOptions,
      Changeset info,
      bool deferCheckIn,
      int checkInTicket)
    {
      this.m_versionControlRequestContext.Validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      this.m_versionControlRequestContext.Validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      PathLength serverPathLength = this.m_versionControlRequestContext.MaxSupportedServerPathLength;
      this.m_versionControlRequestContext.Validation.checkServerItems(serverItems, nameof (serverItems), true, false, false, true, false, false, serverPathLength);
      this.m_versionControlRequestContext.Validation.checkDeferredCheckinOptions(serverItems, deferCheckIn, checkInTicket);
      if (info == null)
        info = new Changeset();
      Workspace workspace = Workspace.QueryWorkspace(this.m_versionControlRequestContext, workspaceName, ownerName);
      this.m_versionControlRequestContext.Validation.check((IValidatable) info, "changesetInfo", false);
      this.SecurityWrapper.CheckWorkspacePermission(this.m_versionControlRequestContext, 4, workspace);
      CheckinResult checkinResult = this.ExecuteInternal(workspace.Owner, workspace.Name, 0, PendingSetType.Workspace, workspace.Computer, workspace, serverItems, info, checkinNotificationInfo, checkinOptions, deferCheckIn, checkInTicket);
      this.PublishCustomerIntelligence(checkinResult);
      return checkinResult;
    }
  }
}
