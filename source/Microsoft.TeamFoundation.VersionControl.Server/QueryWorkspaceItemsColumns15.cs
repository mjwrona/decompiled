// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryWorkspaceItemsColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryWorkspaceItemsColumns15 : QueryWorkspaceItemsColumns3
  {
    protected SqlColumnBinder propertyDataspaceId = new SqlColumnBinder("PropertyDataspaceId");

    public QueryWorkspaceItemsColumns15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override WorkspaceItem Bind()
    {
      WorkspaceItem workspaceItem = new WorkspaceItem();
      workspaceItem.ItemPathPair = this.GetItemPathPair(this.targetServerItem.GetServerItem(this.Reader, true));
      workspaceItem.CommittedItemPathPair = this.GetItemPathPair(this.committedServerItem.GetServerItem(this.Reader, true));
      workspaceItem.ChangesetId = this.version.GetInt32((IDataReader) this.Reader, 0);
      workspaceItem.DeletionId = this.deletionId.GetInt32((IDataReader) this.Reader, 0);
      workspaceItem.ItemId = this.itemId.GetInt32((IDataReader) this.Reader);
      workspaceItem.LocalItem = this.localItem.GetLocalItem(this.Reader, true);
      workspaceItem.PendingChange = VersionControlSqlResourceComponent.GetChangeType(this.pendingCommand.GetInt32((IDataReader) this.Reader, 0));
      workspaceItem.FileLength = this.fileLength.GetInt64((IDataReader) this.Reader, 0L);
      workspaceItem.HashValue = VersionControlObjectBinder<WorkspaceItem>.NormalizeHashValue(this.hashValue.GetBytes((IDataReader) this.Reader, true));
      workspaceItem.CheckinDate = this.checkinDate.GetDateTime((IDataReader) this.Reader);
      workspaceItem.fileId = this.fileId.GetInt32((IDataReader) this.Reader, 0);
      workspaceItem.Encoding = this.encoding.GetInt32((IDataReader) this.Reader, -2);
      workspaceItem.ItemType = VersionedItemComponent.EncodingToItemType(workspaceItem.Encoding);
      workspaceItem.IsBranch = this.isBranch.GetBoolean((IDataReader) this.Reader);
      workspaceItem.RecursivePendingChange = VersionControlSqlResourceComponent.GetChangeType(this.recursiveCommand.GetInt32((IDataReader) this.Reader, 0));
      workspaceItem.PropertyId = this.propertyId.GetInt32((IDataReader) this.Reader, -1);
      workspaceItem.ItemDataspaceId = workspaceItem.PropertyId == -1 ? Guid.Empty : this.GetDataspaceIdentifier(this.propertyDataspaceId.GetInt32((IDataReader) this.Reader, 0));
      return workspaceItem;
    }
  }
}
