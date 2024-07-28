// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryPendingChangesColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryPendingChangesColumns15 : QueryPendingChangesColumns3
  {
    protected SqlColumnBinder propertyDataspaceId = new SqlColumnBinder("PropertyDataspaceId");

    public QueryPendingChangesColumns15(
      IVssRequestContext requestContext,
      VersionControlSqlResourceComponent component)
      : base(requestContext, component)
    {
    }

    protected override PendingChange Bind()
    {
      PendingChange pendingChange = new PendingChange();
      pendingChange.ItemPathPair = this.GetItemPathPair(this.serverItem.GetServerItem(this.Reader, false));
      pendingChange.SourceItemPathPair = this.GetItemPathPair(this.existingServerItem.GetServerItem(this.Reader, true));
      if (pendingChange.ServerItem != null && pendingChange.SourceServerItem != null && VersionControlPath.EqualsCaseSensitive(pendingChange.SourceServerItem, pendingChange.ServerItem))
        pendingChange.SourceItemPathPair = new ItemPathPair();
      pendingChange.SourceVersionFrom = this.sourceVersionFrom.GetInt32((IDataReader) this.Reader, 0);
      pendingChange.SourceDeletionId = this.sourceDeletionId.GetInt32((IDataReader) this.Reader, 0);
      pendingChange.DeletionId = this.deletionId.GetInt32((IDataReader) this.Reader, 0);
      pendingChange.LockLevel = this.lockStatus.GetLockLevel(this.Reader);
      pendingChange.LocalItem = this.localItem.GetLocalItem(this.Reader, true);
      pendingChange.ItemType = (ItemType) this.itemType.GetByte((IDataReader) this.Reader);
      pendingChange.ItemId = this.itemId.GetInt32((IDataReader) this.Reader);
      pendingChange.CreationDate = this.creationTime.GetDateTime((IDataReader) this.Reader);
      pendingChange.Version = this.versionFrom.GetInt32((IDataReader) this.Reader, 0);
      pendingChange.ChangeType = VersionControlSqlResourceComponent.GetChangeType(this.command.GetInt32((IDataReader) this.Reader, 0));
      pendingChange.HashValue = VersionControlObjectBinder<PendingChange>.NormalizeHashValue(this.hashValue.GetBytes((IDataReader) this.Reader, true));
      pendingChange.Length = this.length.GetInt64((IDataReader) this.Reader, -1L);
      pendingChange.fileId = this.fileId.GetInt32((IDataReader) this.Reader, 0);
      pendingChange.UploadHashValue = VersionControlObjectBinder<PendingChange>.NormalizeHashValue(this.uploadHashValue.GetBytes((IDataReader) this.Reader, true));
      pendingChange.Encoding = this.encoding.GetInt32((IDataReader) this.Reader);
      pendingChange.PendingChangeId = this.pendingChangeId.GetInt32((IDataReader) this.Reader);
      pendingChange.uploadFileId = this.uploadFileId.GetInt32((IDataReader) this.Reader, 0);
      int int32 = this.workspaceId.GetInt32((IDataReader) this.Reader);
      if (this.m_prevPendingSet == null || this.m_prevPendingSet.workspaceId != int32)
      {
        this.m_prevPendingSet = new PendingSet();
        this.m_prevPendingSet.workspaceId = int32;
        this.m_prevPendingSet.Name = this.workspaceName.GetString((IDataReader) this.Reader, false);
        this.m_prevPendingSet.OwnerTeamFoundationId = this.ownerId.GetGuid((IDataReader) this.Reader);
        Microsoft.VisualStudio.Services.Identity.Identity identity = VersionControlObjectBinder<PendingChange>.GetIdentity(this.m_requestContext, this.m_identityService, this.m_prevPendingSet.OwnerTeamFoundationId);
        if (identity != null)
        {
          this.m_prevPendingSet.OwnerName = identity.Id.ToString();
          this.m_prevPendingSet.OwnerDisplayName = identity.DisplayName;
          this.m_prevPendingSet.Ownership = IdentityDescriptorComparer.Instance.Equals(identity.Descriptor, this.m_requestContext.UserContext) ? OwnershipState.OwnedByAuthorizedUser : OwnershipState.NotOwnedByAuthorizedUser;
        }
        else
        {
          this.m_prevPendingSet.OwnerName = this.m_prevPendingSet.OwnerName ?? this.m_prevPendingSet.OwnerTeamFoundationId.ToString();
          this.m_prevPendingSet.OwnerDisplayName = this.m_prevPendingSet.OwnerName;
          this.m_prevPendingSet.Ownership = OwnershipState.NotOwnedByAuthorizedUser;
        }
        this.m_prevPendingSet.Computer = this.computer.GetString((IDataReader) this.Reader, true);
      }
      pendingChange.pendingSet = this.m_prevPendingSet;
      pendingChange.InputIndex = this.inputIndex.GetInt32((IDataReader) this.Reader);
      pendingChange.ConflictType = this.conflictType.GetInt32((IDataReader) this.Reader, 0);
      pendingChange.PropertyId = this.propertyId.GetInt32((IDataReader) this.Reader, -1);
      pendingChange.PropertyDataspaceId = pendingChange.PropertyId == -1 ? Guid.Empty : this.GetDataspaceIdentifier(this.propertyDataspaceId.GetInt32((IDataReader) this.Reader, 0));
      return pendingChange;
    }
  }
}
