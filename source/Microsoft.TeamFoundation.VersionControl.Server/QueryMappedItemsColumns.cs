// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryMappedItemsColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryMappedItemsColumns : VersionControlObjectBinder<MappedItem>
  {
    protected SqlColumnBinder fullPath = new SqlColumnBinder("FullPath");
    protected SqlColumnBinder itemId = new SqlColumnBinder("ItemId");
    protected SqlColumnBinder encoding = new SqlColumnBinder("Encoding");
    protected SqlColumnBinder haveVersionFrom = new SqlColumnBinder("HaveVersionFrom");
    protected SqlColumnBinder latestVersionFrom = new SqlColumnBinder("LatestVersionFrom");
    protected SqlColumnBinder fileId = new SqlColumnBinder("FileId");

    public QueryMappedItemsColumns()
    {
    }

    public QueryMappedItemsColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override MappedItem Bind()
    {
      MappedItem mappedItem = new MappedItem();
      mappedItem.ItemPathPair = this.GetPreDataspaceItemPathPair(this.fullPath.GetServerItem(this.Reader, false));
      mappedItem.ItemId = this.itemId.GetInt32((IDataReader) this.Reader);
      mappedItem.Encoding = this.encoding.GetInt32((IDataReader) this.Reader);
      mappedItem.ItemType = VersionedItemComponent.EncodingToItemType(mappedItem.Encoding);
      mappedItem.HaveChangesetId = this.haveVersionFrom.GetInt32((IDataReader) this.Reader, 0);
      mappedItem.LatestChangesetId = this.latestVersionFrom.GetInt32((IDataReader) this.Reader);
      mappedItem.fileId = this.fileId.GetInt32((IDataReader) this.Reader, 0);
      mappedItem.ChangesetId = mappedItem.HaveChangesetId > 0 ? mappedItem.HaveChangesetId : mappedItem.LatestChangesetId;
      return mappedItem;
    }
  }
}
