// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.RollbackFailureBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class RollbackFailureBinder : PendChangeFailureBinder
  {
    public RollbackFailureBinder(IVssRequestContext requestContext)
      : base(PendChangeFailureBinder.Caller.Rollback, requestContext)
    {
    }

    public RollbackFailureBinder(
      IVssRequestContext requestContext,
      VersionControlSqlResourceComponent component)
      : base(PendChangeFailureBinder.Caller.Rollback, requestContext, component)
    {
    }

    protected override void MapError(Failure failure, int error, string serverItem)
    {
      base.MapError(failure, error, serverItem);
      if (failure.Code != null)
        return;
      switch (error)
      {
        case 500160:
          failure.Code = "WorkspaceVersionOlderThanRollback";
          failure.Message = Resources.Format("WorkspaceVerOlderThanRollbackVerException", (object) failure.ServerItem);
          break;
        case 500162:
          failure.Code = "CannotDeleteEditItemWithConflict";
          failure.Message = Resources.Format("CannotDeleteEditItemWithConflict", (object) failure.ServerItem);
          break;
        case 500167:
          failure.Code = "RollbackConflictingPendingRename";
          failure.Message = Resources.Format("RollbackConflictingPendingRename", (object) failure.ServerItem);
          break;
        case 500186:
          failure.Code = "RollbackRenameTargetExcluded";
          failure.Message = Resources.Format("RollbackRenameTargetExcluded", (object) failure.ServerItem, (object) serverItem);
          failure.Severity = SeverityType.Warning;
          break;
        case 500187:
          failure.Code = "RollbackRenameSourceExcluded";
          failure.Message = Resources.Format("RollbackRenameSourceExcluded", (object) serverItem, (object) failure.ServerItem);
          failure.Severity = SeverityType.Warning;
          break;
        case 500189:
          failure.Code = "CannotRollbackFolderToFile";
          failure.Message = Resources.Format("CannotRollbackFolderToFile", (object) failure.ServerItem);
          break;
        case 500190:
          failure.Code = "CannotRollbackFileToFolder";
          failure.Message = Resources.Format("CannotRollbackFileToFolder", (object) failure.ServerItem);
          break;
      }
    }
  }
}
