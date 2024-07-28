// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ConflictDetailsColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ConflictDetailsColumns : VersionControlObjectBinder<Conflict>
  {
    protected SqlColumnBinder conflictId = new SqlColumnBinder("ConflictId");
    protected SqlColumnBinder pendingChangeId = new SqlColumnBinder("PendingChangeId");
    protected SqlColumnBinder yourChangeType = new SqlColumnBinder("YourChangeType");
    protected SqlColumnBinder yourServerItem = new SqlColumnBinder("YourServerItem");
    protected SqlColumnBinder yourEncoding = new SqlColumnBinder("YourEncoding");
    protected SqlColumnBinder yourItemType = new SqlColumnBinder("YourItemType");
    protected SqlColumnBinder yourItemId = new SqlColumnBinder("YourItemId");
    protected SqlColumnBinder yourVersion = new SqlColumnBinder("YourVersionFrom");
    protected SqlColumnBinder yourDeletionId = new SqlColumnBinder("YourDeletionId");
    protected SqlColumnBinder yourFileId = new SqlColumnBinder("YourFileId");
    protected SqlColumnBinder yourLocalChangeType = new SqlColumnBinder("YourLocalChangeType");
    protected SqlColumnBinder yourLastMergedVersion = new SqlColumnBinder("YourLastMergedVersion");
    protected SqlColumnBinder yourServerItemSource = new SqlColumnBinder("YourServerItemSource");
    protected SqlColumnBinder baseServerItem = new SqlColumnBinder("BaseServerItem");
    protected SqlColumnBinder baseEncoding = new SqlColumnBinder("BaseEncoding");
    protected SqlColumnBinder baseItemId = new SqlColumnBinder("BaseItemId");
    protected SqlColumnBinder baseVersion = new SqlColumnBinder("BaseVersion");
    protected SqlColumnBinder baseHashValue = new SqlColumnBinder("BaseHashValue");
    protected SqlColumnBinder baseDeletionId = new SqlColumnBinder("BaseDeletionId");
    protected SqlColumnBinder baseItemType = new SqlColumnBinder("BaseItemType");
    protected SqlColumnBinder baseChangeType = new SqlColumnBinder("BaseChangeType");
    protected SqlColumnBinder theirServerItem = new SqlColumnBinder("TheirServerItem");
    protected SqlColumnBinder theirEncoding = new SqlColumnBinder("TheirEncoding");
    protected SqlColumnBinder theirItemId = new SqlColumnBinder("TheirItemId");
    protected SqlColumnBinder theirVersion = new SqlColumnBinder("TheirVersion");
    protected SqlColumnBinder theirHashValue = new SqlColumnBinder("TheirHashValue");
    protected SqlColumnBinder theirDeletionId = new SqlColumnBinder("TheirDeletionId");
    protected SqlColumnBinder theirItemType = new SqlColumnBinder("TheirItemType");
    protected SqlColumnBinder theirVersionFrom = new SqlColumnBinder("TheirVersionFrom");
    protected SqlColumnBinder theirChangeType = new SqlColumnBinder("TheirChangeType");
    protected SqlColumnBinder isShelvesetConflict = new SqlColumnBinder("IsShelvesetConflict");
    protected SqlColumnBinder theirShelvesetName = new SqlColumnBinder("TheirShelvesetName");
    protected SqlColumnBinder theirShelvesetOwnerId = new SqlColumnBinder("TheirShelvesetOwnerId");
    protected SqlColumnBinder theirLastMergedVersion = new SqlColumnBinder("TheirLastMergedVersion");
    protected SqlColumnBinder forced = new SqlColumnBinder("Forced");
    protected SqlColumnBinder sourceLocalItem = new SqlColumnBinder("SourceLocalItem");
    protected SqlColumnBinder targetLocalItem = new SqlColumnBinder("TargetLocalItem");
    protected SqlColumnBinder conflictType = new SqlColumnBinder("ConflictType");
    protected SqlColumnBinder reason = new SqlColumnBinder("Reason");
    protected SqlColumnBinder isNamespace = new SqlColumnBinder("IsNamespaceConflict");
    protected SqlColumnBinder resolution = new SqlColumnBinder("Resolution");
    protected SqlColumnBinder isResolved = new SqlColumnBinder("IsResolved");
    protected SqlColumnBinder baseFileId = new SqlColumnBinder("BaseFileId");
    protected SqlColumnBinder theirFileId = new SqlColumnBinder("TheirFileId");
    protected SqlColumnBinder conflictOptions = new SqlColumnBinder("ConflictOptions");

    public ConflictDetailsColumns()
    {
    }

    public ConflictDetailsColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override Conflict Bind()
    {
      Conflict conflict = new Conflict();
      conflict.ConflictId = this.conflictId.GetInt32((IDataReader) this.Reader, 0);
      conflict.PendingChangeId = this.pendingChangeId.GetInt32((IDataReader) this.Reader, 0);
      conflict.YourChangeType = VersionControlSqlResourceComponent.GetChangeType(this.yourChangeType.GetInt32((IDataReader) this.Reader, 0));
      conflict.YourItemPathPair = this.GetPreDataspaceItemPathPair(this.yourServerItem.GetServerItem(this.Reader, true));
      conflict.YourEncoding = this.yourEncoding.GetInt32((IDataReader) this.Reader, -2);
      conflict.YourItemType = (ItemType) this.yourItemType.GetByte((IDataReader) this.Reader, (byte) 0);
      conflict.YourItemId = this.yourItemId.GetInt32((IDataReader) this.Reader, 0);
      conflict.YourVersion = this.yourVersion.GetInt32((IDataReader) this.Reader, 0);
      conflict.YourDeletionId = this.yourDeletionId.GetInt32((IDataReader) this.Reader, 0);
      conflict.m_yourFileId = this.yourFileId.GetInt32((IDataReader) this.Reader, 0);
      conflict.YourLocalChangeType = VersionControlSqlResourceComponent.GetChangeType(this.yourLocalChangeType.GetInt32((IDataReader) this.Reader, 0));
      conflict.YourLastMergedVersion = this.yourLastMergedVersion.GetInt32((IDataReader) this.Reader, 0);
      conflict.YourItemSourcePathPair = this.GetPreDataspaceItemPathPair(this.yourServerItemSource.GetServerItem(this.Reader, true));
      conflict.BaseItemPathPair = this.GetPreDataspaceItemPathPair(this.baseServerItem.GetServerItem(this.Reader, true));
      conflict.BaseEncoding = this.baseEncoding.GetInt32((IDataReader) this.Reader, -2);
      conflict.BaseItemId = this.baseItemId.GetInt32((IDataReader) this.Reader, 0);
      conflict.BaseVersion = this.baseVersion.GetInt32((IDataReader) this.Reader, 0);
      conflict.BaseHashValue = VersionControlObjectBinder<Conflict>.NormalizeHashValue(this.baseHashValue.GetBytes((IDataReader) this.Reader, true));
      conflict.BaseDeletionId = this.baseDeletionId.GetInt32((IDataReader) this.Reader, 0);
      conflict.BaseItemType = (ItemType) this.baseItemType.GetByte((IDataReader) this.Reader, (byte) 0);
      conflict.BaseChangeType = VersionControlSqlResourceComponent.GetChangeType(this.baseChangeType.GetInt32((IDataReader) this.Reader, 0));
      conflict.TheirItemPathPair = this.GetPreDataspaceItemPathPair(this.theirServerItem.GetServerItem(this.Reader, true));
      conflict.TheirEncoding = this.theirEncoding.GetInt32((IDataReader) this.Reader, 0);
      conflict.TheirItemId = this.theirItemId.GetInt32((IDataReader) this.Reader, 0);
      conflict.TheirVersion = this.theirVersion.GetInt32((IDataReader) this.Reader, 0);
      conflict.TheirHashValue = VersionControlObjectBinder<Conflict>.NormalizeHashValue(this.theirHashValue.GetBytes((IDataReader) this.Reader, true));
      conflict.TheirDeletionId = this.theirDeletionId.GetInt32((IDataReader) this.Reader, 0);
      conflict.TheirItemType = (ItemType) this.theirItemType.GetByte((IDataReader) this.Reader, (byte) 0);
      conflict.TheirVersionFrom = this.theirVersionFrom.GetInt32((IDataReader) this.Reader, 0);
      conflict.TheirLastMergedVersion = this.theirLastMergedVersion.GetInt32((IDataReader) this.Reader, 0);
      conflict.TheirChangeType = (int) VersionControlSqlResourceComponent.GetChangeType(this.theirChangeType.GetInt32((IDataReader) this.Reader, 0));
      conflict.IsShelvesetConflict = this.isShelvesetConflict.GetBoolean((IDataReader) this.Reader, false);
      conflict.TheirShelvesetName = this.theirShelvesetName.GetString((IDataReader) this.Reader, true);
      conflict.TheirShelvesetOwnerId = this.theirShelvesetOwnerId.GetGuid((IDataReader) this.Reader, true);
      conflict.SourceLocalItem = this.sourceLocalItem.GetLocalItem(this.Reader, true);
      conflict.TargetLocalItem = this.targetLocalItem.GetLocalItem(this.Reader, true);
      conflict.Type = (ConflictType) this.conflictType.GetInt32((IDataReader) this.Reader);
      conflict.Reason = this.reason.GetInt32((IDataReader) this.Reader, 0);
      conflict.IsForced = this.forced.GetBoolean((IDataReader) this.Reader, false);
      conflict.IsResolved = this.isResolved.GetBoolean((IDataReader) this.Reader, false);
      conflict.IsNamespaceConflict = this.isNamespace.GetBoolean((IDataReader) this.Reader, false);
      conflict.ConflictOptions = this.conflictOptions.GetInt32((IDataReader) this.Reader, 0);
      switch ((MergeFlags) this.resolution.GetInt32((IDataReader) this.Reader, 0))
      {
        case MergeFlags.None:
          conflict.Resolution = Resolution.None;
          break;
        case MergeFlags.AcceptTheirs:
          conflict.Resolution = Resolution.AcceptTheirs;
          break;
        case MergeFlags.AcceptMine:
          conflict.Resolution = Resolution.AcceptYours;
          break;
        case MergeFlags.AcceptMerged:
          conflict.Resolution = Resolution.AcceptMerge;
          break;
        case MergeFlags.AcceptYoursRenameTheirs:
          conflict.Resolution = Resolution.AcceptYoursRenameTheirs;
          break;
      }
      conflict.m_baseFileId = this.baseFileId.GetInt32((IDataReader) this.Reader, 0);
      conflict.m_theirFileId = this.theirFileId.GetInt32((IDataReader) this.Reader, 0);
      return conflict;
    }
  }
}
