// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.GetOperationColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class GetOperationColumns : VersionControlObjectBinder<GetOperation>
  {
    protected SqlColumnBinder versionServer = new SqlColumnBinder("VersionServer");
    protected SqlColumnBinder versionLocal = new SqlColumnBinder("VersionLocal");
    protected SqlColumnBinder targetServerItem = new SqlColumnBinder("TargetServerItem");
    protected SqlColumnBinder sourceLocalItem = new SqlColumnBinder("SourceLocalItem");
    protected SqlColumnBinder targetLocalItem = new SqlColumnBinder("TargetLocalItem");
    protected SqlColumnBinder deletionId = new SqlColumnBinder("DeletionId");
    protected SqlColumnBinder itemId = new SqlColumnBinder("ItemId");
    protected SqlColumnBinder pendingChangeId = new SqlColumnBinder("PendingChangeId");
    protected SqlColumnBinder pendingCommand = new SqlColumnBinder("PendingCommand");
    protected SqlColumnBinder itemType = new SqlColumnBinder("ItemType");
    protected SqlColumnBinder lockStatus = new SqlColumnBinder("LockStatus");
    protected SqlColumnBinder hasConflict = new SqlColumnBinder("HasConflict");
    protected SqlColumnBinder conflictingPendingCommand = new SqlColumnBinder("ConflictingPendingCommand");
    protected SqlColumnBinder conflictingItemId = new SqlColumnBinder("ConflictingItemId");
    protected SqlColumnBinder hashValue = new SqlColumnBinder("HashValue");
    protected SqlColumnBinder fileId = new SqlColumnBinder("FileId");
    protected SqlColumnBinder versionTo = new SqlColumnBinder("VersionTo");
    protected SqlColumnBinder isNamespaceConflict = new SqlColumnBinder("IsNamespaceConflict");
    protected SqlColumnBinder sourceServerItem = new SqlColumnBinder("SourceServerItem");
    protected SqlColumnBinder encoding = new SqlColumnBinder("Encoding");
    protected SqlColumnBinder versionServerDate = new SqlColumnBinder("VersionServerDate");
    public static readonly string EmptySourceItem = new string(new char[1]
    {
      '/'
    });

    public GetOperationColumns()
    {
    }

    public GetOperationColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override GetOperation Bind()
    {
      GetOperation getOperation = new GetOperation();
      getOperation.VersionServer = this.versionServer.GetInt32((IDataReader) this.Reader, 0);
      getOperation.VersionLocal = this.versionLocal.GetInt32((IDataReader) this.Reader, 0);
      getOperation.DeletionId = this.deletionId.GetInt32((IDataReader) this.Reader, 0);
      getOperation.TargetItemPathPair = this.GetPreDataspaceItemPathPair(this.targetServerItem.GetServerItem(this.Reader, true));
      getOperation.TargetLocalItem = this.targetLocalItem.GetLocalItem(this.Reader, true);
      getOperation.SourceLocalItem = this.sourceLocalItem.GetLocalItem(this.Reader, true);
      getOperation.ItemId = this.itemId.GetInt32((IDataReader) this.Reader, 0);
      getOperation.ItemType = (ItemType) this.itemType.GetByte((IDataReader) this.Reader, (byte) 0);
      getOperation.PendingChangeId = this.pendingChangeId.GetInt32((IDataReader) this.Reader, 0);
      getOperation.ChangeType = VersionControlSqlResourceComponent.GetChangeType(this.pendingCommand.GetInt32((IDataReader) this.Reader, 0));
      getOperation.LockLevel = this.lockStatus.GetLockLevel(this.Reader);
      getOperation.HasConflict = this.hasConflict.GetBoolean((IDataReader) this.Reader);
      getOperation.ConflictingChangeType = VersionControlSqlResourceComponent.GetChangeType(this.conflictingPendingCommand.GetInt32((IDataReader) this.Reader, 0));
      getOperation.ConflictingItemId = this.conflictingItemId.GetInt32((IDataReader) this.Reader, 0);
      getOperation.HashValue = VersionControlObjectBinder<GetOperation>.NormalizeHashValue(this.hashValue.GetBytes((IDataReader) this.Reader, true));
      getOperation.fileId = this.fileId.GetInt32((IDataReader) this.Reader, 0);
      getOperation.IsLatest = this.versionTo.GetInt32((IDataReader) this.Reader, 0) == int.MaxValue;
      getOperation.IsNamespaceConflict = this.isNamespaceConflict.GetBoolean((IDataReader) this.Reader, false) ? (byte) 1 : (byte) 2;
      getOperation.SourceItemPathPair = this.GetPreDataspaceItemPathPair(this.sourceServerItem.GetServerItem(this.Reader, true));
      if (string.IsNullOrEmpty(getOperation.SourceServerItem))
        getOperation.SourceItemPathPair = ItemPathPair.FromServerItem(GetOperationColumns.EmptySourceItem);
      getOperation.Encoding = this.encoding.GetInt32((IDataReader) this.Reader, -2);
      getOperation.VersionServerDate = this.versionServerDate.GetDateTime((IDataReader) this.Reader);
      return getOperation;
    }
  }
}
