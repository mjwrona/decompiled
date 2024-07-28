// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ExtendedItemColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ExtendedItemColumns : VersionControlObjectBinder<ExtendedItem>
  {
    protected SqlColumnBinder sourceServerItem = new SqlColumnBinder("SourceServerItem");
    protected SqlColumnBinder targetServerItem = new SqlColumnBinder("TargetServerItem");
    protected SqlColumnBinder localItem = new SqlColumnBinder("LocalItem");
    protected SqlColumnBinder itemId = new SqlColumnBinder("ItemId");
    protected SqlColumnBinder encoding = new SqlColumnBinder("Encoding");
    protected SqlColumnBinder deletionId = new SqlColumnBinder("DeletionId");
    protected SqlColumnBinder version = new SqlColumnBinder("VersionLocal");
    protected SqlColumnBinder pendingCommand = new SqlColumnBinder("PendingCommand");
    protected SqlColumnBinder hasOtherPendingChange = new SqlColumnBinder("HasOtherPendingChange");
    protected SqlColumnBinder lockStatus = new SqlColumnBinder("LockStatus");
    protected SqlColumnBinder lockOwner = new SqlColumnBinder("LockOwner");
    protected SqlColumnBinder versionLatest = new SqlColumnBinder("VersionLatest");
    protected SqlColumnBinder isBranch = new SqlColumnBinder("IsBranch");
    protected SqlColumnBinder checkinDate = new SqlColumnBinder("CheckinDate");

    public ExtendedItemColumns()
    {
    }

    public ExtendedItemColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override ExtendedItem Bind()
    {
      ExtendedItem extendedItem = new ExtendedItem()
      {
        SourceItemPathPair = this.GetPreDataspaceItemPathPair(this.sourceServerItem.GetServerItem(this.Reader, true)),
        TargetItemPathPair = this.GetPreDataspaceItemPathPair(this.targetServerItem.GetServerItem(this.Reader, true)),
        LocalItem = this.localItem.GetLocalItem(this.Reader, true),
        ItemId = this.itemId.GetInt32((IDataReader) this.Reader),
        DeletionId = this.deletionId.GetInt32((IDataReader) this.Reader, 0),
        VersionLocal = this.version.GetInt32((IDataReader) this.Reader, 0),
        ChangeType = VersionControlSqlResourceComponent.GetChangeType(this.pendingCommand.GetInt32((IDataReader) this.Reader, 0)),
        HasOtherPendingChange = this.hasOtherPendingChange.GetBoolean((IDataReader) this.Reader),
        LockStatus = this.lockStatus.GetLockLevel(this.Reader),
        lockOwnerId = this.lockOwner.GetGuid((IDataReader) this.Reader, true),
        VersionLatest = this.versionLatest.GetInt32((IDataReader) this.Reader, 0),
        CheckinDate = this.checkinDate.GetDateTime((IDataReader) this.Reader),
        Encoding = this.encoding.GetInt32((IDataReader) this.Reader)
      };
      extendedItem.ItemType = VersionedItemComponent.EncodingToItemType(extendedItem.Encoding);
      extendedItem.IsBranch = this.isBranch.GetBoolean((IDataReader) this.Reader);
      return extendedItem;
    }
  }
}
