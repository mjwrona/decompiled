// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryItemsColumns15a
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryItemsColumns15a : QueryItemsColumns
  {
    protected SqlColumnBinder propertyDataspaceId = new SqlColumnBinder("PropertyDataspaceId");

    public QueryItemsColumns15a(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override Item Bind()
    {
      Item obj = new Item()
      {
        ItemPathPair = this.GetItemPathPair(this.serverItem.GetServerItem(this.Reader, false)),
        ChangesetId = this.version.GetInt32((IDataReader) this.Reader),
        DeletionId = this.deletionId.GetInt32((IDataReader) this.Reader),
        ItemId = this.itemId.GetInt32((IDataReader) this.Reader),
        ItemDataspaceId = this.GetDataspaceIdentifier(this.propertyDataspaceId.GetInt32((IDataReader) this.Reader)),
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
