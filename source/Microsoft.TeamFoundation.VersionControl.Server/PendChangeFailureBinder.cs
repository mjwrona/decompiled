// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendChangeFailureBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Identity;
using System.Data;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendChangeFailureBinder : VersionControlObjectBinder<Failure>
  {
    protected SqlColumnBinder targetServerItem = new SqlColumnBinder("TargetServerItem");
    protected SqlColumnBinder offendingServerItem = new SqlColumnBinder("OffendingServerItem");
    private SqlColumnBinder offendingLocalItem = new SqlColumnBinder("OffendingLocalItem");
    private SqlColumnBinder errorCode = new SqlColumnBinder("ErrorCode");
    private SqlColumnBinder lockOwnerId = new SqlColumnBinder("LockOwnerId");
    private SqlColumnBinder lockWorkspaceName = new SqlColumnBinder("LockWorkspaceName");
    private PendChangeFailureBinder.Caller m_caller;
    private SeverityType m_severityType;
    private IdentityService m_identityService;
    private IVssRequestContext m_requestContext;

    public PendChangeFailureBinder(
      PendChangeFailureBinder.Caller caller,
      IVssRequestContext requestContext)
      : this(caller, requestContext, SeverityType.Error)
    {
    }

    public PendChangeFailureBinder(
      PendChangeFailureBinder.Caller caller,
      IVssRequestContext requestContext,
      SeverityType severityType)
    {
      this.m_caller = caller;
      this.m_requestContext = requestContext;
      this.m_identityService = requestContext.GetService<IdentityService>();
      this.m_severityType = severityType;
    }

    public PendChangeFailureBinder(
      PendChangeFailureBinder.Caller caller,
      IVssRequestContext requestContext,
      VersionControlSqlResourceComponent component)
      : this(caller, requestContext, SeverityType.Error, component)
    {
    }

    public PendChangeFailureBinder(
      PendChangeFailureBinder.Caller caller,
      IVssRequestContext requestContext,
      SeverityType severityType,
      VersionControlSqlResourceComponent component)
      : base(component)
    {
      this.m_caller = caller;
      this.m_requestContext = requestContext;
      this.m_identityService = requestContext.GetService<IdentityService>();
      this.m_severityType = severityType;
    }

    protected virtual string BindTargetServerItem() => this.targetServerItem.GetServerItem(this.Reader, true);

    protected virtual string BindOffendingServerItem() => this.offendingServerItem.GetServerItem(this.Reader, true);

    protected virtual string BindOffendingLocalItem() => this.offendingLocalItem.GetLocalItem(this.Reader, true);

    protected override Failure Bind()
    {
      Failure failure = new Failure();
      failure.ServerItem = this.BindTargetServerItem();
      string serverItem = this.BindOffendingServerItem();
      int int32 = this.errorCode.GetInt32((IDataReader) this.Reader);
      this.MapError(failure, int32, serverItem);
      return failure;
    }

    protected virtual void MapError(Failure failure, int error, string serverItem)
    {
      this.m_requestContext.Trace(700029, TraceLevel.Verbose, TraceArea.General, TraceLayer.Component, "Binding failure: {0}; {1}", (object) error, (object) serverItem);
      switch (error)
      {
        case 500021:
          failure.Code = "ItemAlreadyExists";
          failure.Message = Resources.Format("ItemExistsException", (object) this.BindOffendingLocalItem());
          break;
        case 500026:
          failure.Message = Resources.Format("PendingParentDeleteException", (object) serverItem);
          failure.Code = "PendingParentDeleteException";
          break;
        case 500031:
        case 500033:
          string identityDisplayName = VersionControlObjectBinder<Failure>.GetIdentityDisplayName(this.m_requestContext, this.m_identityService, this.lockOwnerId.GetGuid((IDataReader) this.Reader));
          string workspace = this.lockWorkspaceName.GetString((IDataReader) this.Reader, false);
          failure.Message = Resources.Format("ItemLockedException", (object) failure.ServerItem, (object) serverItem, (object) WorkspaceSpec.Combine(workspace, identityDisplayName));
          failure.Code = "ItemLockedException";
          break;
        case 500032:
          failure.Code = "PendingChildConflict";
          failure.Message = Resources.Format("PendingChildException", (object) serverItem);
          break;
        case 500039:
          failure.Code = "ChangeAlreadyPendingException";
          failure.Message = Resources.Format("TargetPathHasInCompatiblePendingChangeException", (object) serverItem);
          break;
        case 500063:
          failure.Code = "RenameWorkingFolderException";
          failure.Message = Resources.Format("RenameWorkingFolderException", (object) failure.ServerItem);
          break;
        case 500064:
          failure.Code = "TargetHasPendingChangeException";
          failure.Message = Resources.Format("TargetHasPendingChangeException", (object) serverItem);
          break;
        case 500086:
          failure.Code = "RepositoryPathTooLong";
          failure.Message = Resources.Format("RepositoryPathTooLong", (object) serverItem, (object) 248, (object) 259);
          break;
        case 500087:
          failure.Code = "LocalPathTooLong";
          failure.Message = Resources.Format("LocalPathTooLong", (object) this.BindOffendingLocalItem());
          break;
        case 500099:
          if (this.m_caller == PendChangeFailureBinder.Caller.Merge || this.m_caller == PendChangeFailureBinder.Caller.Unshelve || this.m_caller == PendChangeFailureBinder.Caller.ReconcileLocalWorkspace)
          {
            failure.Message = Resources.Format("CanNotMergeWithExistingMergeConflict", (object) serverItem);
            failure.Code = "CanNotMergeWithExistingConflict";
            break;
          }
          failure.Message = Resources.Format("CannotRollbackWithExistingConflict", (object) serverItem);
          failure.Code = "CannotRollbackWithExistingConflict";
          break;
        case 500137:
          failure.Code = "ItemNotMapped";
          failure.Message = Resources.Format("ItemNotMappedException", (object) serverItem);
          break;
        case 500143:
          if (this.m_caller == PendChangeFailureBinder.Caller.Merge || this.m_caller == PendChangeFailureBinder.Caller.Unshelve || this.m_caller == PendChangeFailureBinder.Caller.ReconcileLocalWorkspace)
          {
            failure.Code = "CannotBranchDestroyedContent";
            failure.Message = Resources.Format("CannotBranchDestroyedContentException", (object) serverItem);
            break;
          }
          failure.Code = "CannotRollbackSourceOrBaseDestroyed";
          failure.Message = Resources.Format("CannotRollbackSourceOrBaseDestroyed", (object) serverItem);
          break;
        case 500157:
          failure.Code = "AllPendingChangeWarningsNotIncluded";
          failure.Message = Resources.Format("AllPendingChangeWarningsNotIncluded");
          failure.Severity = SeverityType.Warning;
          break;
        case 500188:
          failure.Code = "CreateBranchObjectException";
          failure.Message = Resources.Format("BranchObjectNotRootOfOperationException", (object) serverItem);
          break;
        case 500206:
          failure.Code = "CannotUnshelveMergeIntoMerge";
          failure.Message = Resources.Format("CannotUnshelveMergeIntoMerge", (object) serverItem);
          break;
        case 500207:
          failure.Code = "CannotUnshelveIntoExistingConflict";
          failure.Message = Resources.Format("CannotUnshelveIntoExistingConflict", (object) serverItem);
          break;
        case 500209:
          failure.Code = "CannotUnshelveDueToParentalDelete";
          failure.Message = Resources.Format("CannotUnshelveDueToParentalDelete", (object) serverItem);
          break;
        case 500216:
          failure.Code = "CannotUnshelveDueToShelvesetParentalDelete";
          failure.Message = Resources.Format("CannotUnshelveDueToShelvesetParentalDelete", (object) serverItem);
          break;
        case 500220:
          failure.Code = "CannotRenameDueToChildConflict";
          failure.Message = Resources.Format("CannotRenameDueToChildConflictMessage", (object) failure.ServerItem, (object) serverItem);
          break;
        case 500221:
          failure.Code = "CannotRenameDueToChildConflict";
          failure.Message = Resources.Format("CannotUndoRenameDueToChildConflictMessage", (object) failure.ServerItem, (object) serverItem);
          break;
        case 500227:
          failure.Code = "RepositoryPathTooLongDetailed";
          PathLength serverPathLength = this.m_requestContext.GetVersionControlRequestContext().MaxSupportedServerPathLength;
          failure.Message = Resources.Format("RepositoryPathTooLong", (object) failure.ServerItem, (object) (int) (serverPathLength - 11), (object) (int) serverPathLength);
          break;
      }
    }

    internal enum Caller
    {
      Merge,
      Unshelve,
      Rollback,
      ReconcileLocalWorkspace,
    }
  }
}
