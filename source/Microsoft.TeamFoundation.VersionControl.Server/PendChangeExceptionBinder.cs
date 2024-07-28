// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendChangeExceptionBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System.Data;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendChangeExceptionBinder : VersionControlObjectBinder<ServerException>
  {
    private SqlColumnBinder targetServerItem = new SqlColumnBinder("TargetServerItem");
    private SqlColumnBinder offendingServerItem = new SqlColumnBinder("OffendingServerItem");
    private SqlColumnBinder offendingLocalItem = new SqlColumnBinder("OffendingLocalItem");
    private SqlColumnBinder errorCode = new SqlColumnBinder("ErrorCode");
    private SqlColumnBinder lockOwnerId = new SqlColumnBinder("LockOwnerId");
    private SqlColumnBinder lockWorkspaceName = new SqlColumnBinder("LockWorkspaceName");
    private IdentityService m_identityService;
    private IVssRequestContext m_requestContext;

    public PendChangeExceptionBinder(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_identityService = requestContext.GetService<IdentityService>();
    }

    public PendChangeExceptionBinder(
      IVssRequestContext requestContext,
      VersionControlSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
      this.m_identityService = requestContext.GetService<IdentityService>();
    }

    protected virtual string BindOffendingServerItem() => this.offendingServerItem.GetServerItem(this.Reader, false);

    protected virtual string BindTargetServerItem() => this.targetServerItem.GetServerItem(this.Reader, false);

    protected override ServerException Bind()
    {
      string str = this.BindOffendingServerItem();
      int int32 = this.errorCode.GetInt32((IDataReader) this.Reader);
      this.m_requestContext.Trace(700028, TraceLevel.Verbose, TraceArea.General, TraceLayer.Component, "Binding failure for pending changes: {0}; {1}", (object) int32, (object) str);
      switch (int32)
      {
        case 500021:
          return (ServerException) new ItemExistsException(this.offendingLocalItem.GetLocalItem(this.Reader, false));
        case 500026:
          return (ServerException) new PendingParentDeleteException(str);
        case 500031:
        case 500033:
          string serverItem = this.BindTargetServerItem();
          string identityDisplayName = VersionControlObjectBinder<ServerException>.GetIdentityDisplayName(this.m_requestContext, this.m_identityService, this.lockOwnerId.GetGuid((IDataReader) this.Reader));
          string lockedWorkspace = this.lockWorkspaceName.GetString((IDataReader) this.Reader, false);
          return (ServerException) new ItemLockedException(this.m_requestContext, serverItem, str, identityDisplayName, lockedWorkspace);
        case 500032:
          return (ServerException) new PendingChildException(str);
        case 500039:
          return (ServerException) new ChangeAlreadyPendingException(str);
        case 500063:
          return (ServerException) new RenameWorkingFolderException(str);
        case 500064:
          return (ServerException) new TargetHasPendingChangeException(str);
        case 500086:
          return (ServerException) new RepositoryPathTooLongException(str, 259);
        case 500087:
          return (ServerException) new LocalPathTooLongException(this.offendingLocalItem.GetLocalItem(this.Reader, false));
        case 500099:
          return (ServerException) new CannotMergeWithExistingConflictException(str);
        case 500137:
          return (ServerException) new ItemNotMappedException(str);
        case 500143:
          return (ServerException) new CannotBranchDestroyedContentException(str);
        case 500209:
          return (ServerException) new CannotUnshelveDueToParentalDeleteException(str);
        case 500216:
          return (ServerException) new CannotUnshelveDueToShelvesetParentalDeleteException(str);
        case 500220:
          return (ServerException) new CannotRenameDueToChildConflictException(this.BindTargetServerItem(), str);
        case 500227:
          return (ServerException) new RepositoryPathTooLongDetailedException(str, (int) this.m_requestContext.GetVersionControlRequestContext().MaxSupportedServerPathLength);
        default:
          return (ServerException) null;
      }
    }
  }
}
