// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryItemsColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryItemsColumns : VersionControlObjectBinder<Item>
  {
    protected SqlColumnBinder serverItem = new SqlColumnBinder("TargetServerItem");
    protected SqlColumnBinder version = new SqlColumnBinder("VersionFrom");
    protected SqlColumnBinder deletionId = new SqlColumnBinder("DeletionId");
    protected SqlColumnBinder itemId = new SqlColumnBinder("ItemId");
    protected SqlColumnBinder encoding = new SqlColumnBinder("Encoding");
    protected SqlColumnBinder fileLength = new SqlColumnBinder("FileLength");
    protected SqlColumnBinder hashValue = new SqlColumnBinder("HashValue");
    protected SqlColumnBinder checkinDate = new SqlColumnBinder("CheckinDate");
    protected SqlColumnBinder fileId = new SqlColumnBinder("FileId");
    protected SqlColumnBinder isBranch = new SqlColumnBinder("IsBranch");

    public QueryItemsColumns()
    {
    }

    public QueryItemsColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override Item Bind()
    {
      Item obj = new Item()
      {
        ItemPathPair = this.GetPreDataspaceItemPathPair(this.serverItem.GetServerItem(this.Reader, false)),
        ChangesetId = this.version.GetInt32((IDataReader) this.Reader),
        DeletionId = this.deletionId.GetInt32((IDataReader) this.Reader),
        ItemId = this.itemId.GetInt32((IDataReader) this.Reader),
        Encoding = this.encoding.GetInt32((IDataReader) this.Reader)
      };
      obj.ItemType = VersionedItemComponent.EncodingToItemType(obj.Encoding);
      obj.FileLength = this.fileLength.GetInt64((IDataReader) this.Reader, 0L);
      obj.HashValue = VersionControlObjectBinder<Item>.NormalizeHashValue(this.hashValue.GetBytes((IDataReader) this.Reader, true));
      obj.CheckinDate = this.checkinDate.GetDateTime((IDataReader) this.Reader);
      obj.fileId = this.fileId.GetInt32((IDataReader) this.Reader, 0);
      obj.IsBranch = this.isBranch.GetBoolean((IDataReader) this.Reader);
      return obj;
    }
  }
}
