// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ExtendedItemColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ExtendedItemColumns15 : ExtendedItemColumns3
  {
    protected SqlColumnBinder propertyDataspaceId = new SqlColumnBinder("PropertyDataspaceId");

    public ExtendedItemColumns15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override ExtendedItem Bind()
    {
      ExtendedItem extendedItem = new ExtendedItem()
      {
        SourceItemPathPair = this.GetItemPathPair(this.sourceServerItem.GetServerItem(this.Reader, true)),
        TargetItemPathPair = this.GetItemPathPair(this.targetServerItem.GetServerItem(this.Reader, true)),
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
      extendedItem.PropertyId = this.propertyId.GetInt32((IDataReader) this.Reader, -1);
      extendedItem.ItemDataspaceId = extendedItem.PropertyId == -1 ? Guid.Empty : this.GetDataspaceIdentifier(this.propertyDataspaceId.GetInt32((IDataReader) this.Reader, 0));
      return extendedItem;
    }
  }
}
