// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryItemsByChangesetColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryItemsByChangesetColumns : VersionControlObjectBinder<PreviousHashItem>
  {
    protected SqlColumnBinder serverItem = new SqlColumnBinder("TargetServerItem");
    protected SqlColumnBinder version = new SqlColumnBinder("VersionFrom");
    protected SqlColumnBinder deletionId = new SqlColumnBinder("DeletionId");
    protected SqlColumnBinder itemId = new SqlColumnBinder("ItemId");
    protected SqlColumnBinder encoding = new SqlColumnBinder("Encoding");
    protected SqlColumnBinder fileLength = new SqlColumnBinder("FileLength");
    protected SqlColumnBinder hashValue = new SqlColumnBinder("HashValue");
    protected SqlColumnBinder previousHashValue = new SqlColumnBinder("PreviousHashValue");
    protected SqlColumnBinder checkinDate = new SqlColumnBinder("CheckinDate");
    protected SqlColumnBinder fileId = new SqlColumnBinder("FileId");
    protected SqlColumnBinder isBranch = new SqlColumnBinder("IsBranch");
    protected SqlColumnBinder propertyId = new SqlColumnBinder("PropertyId");
    protected SqlColumnBinder propertyDataspaceId = new SqlColumnBinder("PropertyDataspaceId");

    public QueryItemsByChangesetColumns()
    {
    }

    public QueryItemsByChangesetColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override PreviousHashItem Bind()
    {
      PreviousHashItem previousHashItem = new PreviousHashItem();
      previousHashItem.ItemPathPair = this.GetItemPathPair(this.serverItem.GetServerItem(this.Reader, false));
      previousHashItem.ChangesetId = this.version.GetInt32((IDataReader) this.Reader);
      previousHashItem.DeletionId = this.deletionId.GetInt32((IDataReader) this.Reader);
      previousHashItem.ItemId = this.itemId.GetInt32((IDataReader) this.Reader);
      previousHashItem.ItemDataspaceId = this.GetDataspaceIdentifier(this.propertyDataspaceId.GetInt32((IDataReader) this.Reader));
      previousHashItem.Encoding = this.encoding.GetInt32((IDataReader) this.Reader);
      previousHashItem.ItemType = VersionedItemComponent.EncodingToItemType(previousHashItem.Encoding);
      previousHashItem.FileLength = this.fileLength.GetInt64((IDataReader) this.Reader, 0L);
      previousHashItem.HashValue = VersionControlObjectBinder<PreviousHashItem>.NormalizeHashValue(this.hashValue.GetBytes((IDataReader) this.Reader, true));
      previousHashItem.PreviousHashValue = VersionControlObjectBinder<PreviousHashItem>.NormalizeHashValue(this.previousHashValue.GetBytes((IDataReader) this.Reader, true));
      previousHashItem.CheckinDate = this.checkinDate.GetDateTime((IDataReader) this.Reader);
      previousHashItem.fileId = this.fileId.GetInt32((IDataReader) this.Reader, 0);
      previousHashItem.IsBranch = this.isBranch.GetBoolean((IDataReader) this.Reader);
      previousHashItem.PropertyId = this.propertyId.GetInt32((IDataReader) this.Reader, -1);
      return previousHashItem;
    }
  }
}
