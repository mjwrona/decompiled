// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryWorkspaceItemsColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryWorkspaceItemsColumns : VersionControlObjectBinder<WorkspaceItem>
  {
    protected SqlColumnBinder targetServerItem = new SqlColumnBinder("TargetServerItem");
    protected SqlColumnBinder committedServerItem = new SqlColumnBinder("CommittedServerItem");
    protected SqlColumnBinder version = new SqlColumnBinder("VersionFrom");
    protected SqlColumnBinder deletionId = new SqlColumnBinder("DeletionId");
    protected SqlColumnBinder itemId = new SqlColumnBinder("ItemId");
    protected SqlColumnBinder encoding = new SqlColumnBinder("Encoding");
    protected SqlColumnBinder localItem = new SqlColumnBinder("LocalItem");
    protected SqlColumnBinder pendingCommand = new SqlColumnBinder("PendingCommand");
    protected SqlColumnBinder hashValue = new SqlColumnBinder("HashValue");
    protected SqlColumnBinder checkinDate = new SqlColumnBinder("CheckinDate");
    protected SqlColumnBinder fileLength = new SqlColumnBinder("FileLength");
    protected SqlColumnBinder fileId = new SqlColumnBinder("FileId");
    protected SqlColumnBinder isBranch = new SqlColumnBinder("IsBranch");

    public QueryWorkspaceItemsColumns()
    {
    }

    public QueryWorkspaceItemsColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override WorkspaceItem Bind()
    {
      WorkspaceItem workspaceItem = new WorkspaceItem();
      workspaceItem.ItemPathPair = this.GetPreDataspaceItemPathPair(this.targetServerItem.GetServerItem(this.Reader, true));
      workspaceItem.CommittedItemPathPair = this.GetPreDataspaceItemPathPair(this.committedServerItem.GetServerItem(this.Reader, true));
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
      return workspaceItem;
    }
  }
}
