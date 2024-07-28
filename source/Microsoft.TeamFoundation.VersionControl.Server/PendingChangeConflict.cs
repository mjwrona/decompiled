// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendingChangeConflict
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendingChangeConflict
  {
    internal int ErrorCode;
    internal string ServerItem;
    internal string OwnerName;
    internal string LockedWorkspace;
    internal string OffendingServerItem;
    private IVssRequestContext m_requestContext;

    public PendingChangeConflict(IVssRequestContext context) => this.m_requestContext = context;

    internal TeamFoundationServiceException toException()
    {
      this.m_requestContext.Trace(700101, TraceLevel.Verbose, TraceArea.General, TraceLayer.Component, "Binding conflict failure: {0}; {1}", (object) this.ErrorCode, (object) this.ServerItem);
      switch (this.ErrorCode)
      {
        case 500014:
          return (TeamFoundationServiceException) new ItemNotFoundException(this.ServerItem);
        case 500021:
          return (TeamFoundationServiceException) new ItemExistsException(this.ServerItem);
        case 500032:
          return (TeamFoundationServiceException) new PendingChildException(this.ServerItem);
        case 500033:
          return (TeamFoundationServiceException) new ItemLockedException(this.m_requestContext, this.ServerItem, this.OffendingServerItem, this.OwnerName, this.LockedWorkspace);
        case 500036:
          return (TeamFoundationServiceException) new FolderContentException(this.ServerItem);
        case 500039:
          return (TeamFoundationServiceException) new ChangeAlreadyPendingException(this.ServerItem);
        case 500040:
          return (TeamFoundationServiceException) new LocalItemOutOfDateException(this.ServerItem);
        case 500041:
          return (TeamFoundationServiceException) new ItemNotCheckedOutException(this.ServerItem);
        case 500048:
          return (TeamFoundationServiceException) new NotAllowedOnFolderException(this.ServerItem);
        case 500056:
          return (TeamFoundationServiceException) new ExistingParentFileException(this.ServerItem);
        case 500063:
          return (TeamFoundationServiceException) new RenameWorkingFolderException(this.ServerItem);
        case 500065:
          return (TeamFoundationServiceException) new ItemDeletedException(this.ServerItem);
        case 500067:
          return (TeamFoundationServiceException) new ContentRequiredException(this.ServerItem);
        case 500084:
          return (TeamFoundationServiceException) new PartialRenameConflictException(this.ServerItem);
        case 500095:
          return (TeamFoundationServiceException) new IncompletePendingChangeException(this.ServerItem);
        case 500103:
          return (TeamFoundationServiceException) new LatestVersionDeletedException(this.ServerItem);
        case 500118:
          return (TeamFoundationServiceException) new MergeConflictExistsException(this.ServerItem);
        case 500125:
          return (TeamFoundationServiceException) new MissingParentIsRenameOrUndeleteException(this.ServerItem);
        case 500130:
          return (TeamFoundationServiceException) new CannotCheckinPartialUndeleteException(this.ServerItem);
        case 500154:
          return (TeamFoundationServiceException) new PendingLocalVersionMismatchException(this.ServerItem);
        case 500180:
          return (TeamFoundationServiceException) new CannotCheckinDependantRenameException(this.ServerItem, false);
        case 500181:
          return (TeamFoundationServiceException) new CannotCheckinDependantRenameException(this.ServerItem, true);
        case 500182:
          return (TeamFoundationServiceException) new CannotCheckinRenameAsPendingAddConflictsException(this.OffendingServerItem, this.ServerItem);
        case 500202:
          return (TeamFoundationServiceException) new PendingDeleteConflictChangeException(this.ServerItem);
        case 500218:
          return (TeamFoundationServiceException) new InvalidProjectPendingChangeException(this.ServerItem);
        case 500222:
          return (TeamFoundationServiceException) new CannotCheckinRenameDueToChildConflictException(this.ServerItem, this.OffendingServerItem);
        default:
          return (TeamFoundationServiceException) new DatabaseConfigurationException((SqlException) null);
      }
    }
  }
}
