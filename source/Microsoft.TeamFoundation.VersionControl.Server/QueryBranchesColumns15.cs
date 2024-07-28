// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryBranchesColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryBranchesColumns15 : QueryBranchesColumns
  {
    protected SqlColumnBinder branchedFromItemDataspaceId = new SqlColumnBinder("BranchedFromItemDataspaceId");
    protected SqlColumnBinder branchedToItemDataspaceId = new SqlColumnBinder("BranchedToItemDataspaceId");

    public QueryBranchesColumns15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override BranchRelative Bind()
    {
      BranchRelative branchRelative = new BranchRelative();
      string serverItem = this.branchFrom.GetServerItem(this.Reader, true);
      if (serverItem != null)
      {
        branchRelative.BranchFromItem = new Item();
        branchRelative.BranchFromItem.ItemPathPair = new ItemPathPair(this.GetServerItemProjectNamePath(serverItem), serverItem);
        branchRelative.BranchFromItem.ChangesetId = this.branchFromVersion.GetInt32((IDataReader) this.Reader);
        branchRelative.BranchFromItem.ItemId = this.branchFromItemId.GetInt32((IDataReader) this.Reader);
        branchRelative.BranchFromItem.ItemDataspaceId = this.GetDataspaceIdentifier(this.branchedFromItemDataspaceId.GetInt32((IDataReader) this.Reader));
      }
      branchRelative.BranchToItem = new Item();
      branchRelative.BranchToItem.ItemPathPair = this.GetItemPathPair(this.branchTo.GetServerItem(this.Reader, false));
      branchRelative.BranchToItem.ChangesetId = this.branchToVersion.GetInt32((IDataReader) this.Reader);
      branchRelative.BranchToItem.CheckinDate = this.checkinDate.GetDateTime((IDataReader) this.Reader);
      branchRelative.BranchToItem.DeletionId = this.branchToDeletionId.GetInt32((IDataReader) this.Reader, 0);
      branchRelative.BranchToItem.ItemId = this.branchToItemId.GetInt32((IDataReader) this.Reader);
      branchRelative.BranchToItem.ItemDataspaceId = this.GetDataspaceIdentifier(this.branchedToItemDataspaceId.GetInt32((IDataReader) this.Reader));
      branchRelative.BranchToItem.ItemType = (ItemType) this.itemType.GetByte((IDataReader) this.Reader);
      branchRelative.IsRequestedItem = this.isRequestedItem.GetBoolean((IDataReader) this.Reader);
      branchRelative.BranchToChangeType = VersionControlSqlResourceComponent.GetChangeType((int) this.branchedToCommand.GetInt16((IDataReader) this.Reader));
      return branchRelative;
    }
  }
}
