// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryMappedItemsColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryMappedItemsColumns15 : QueryMappedItemsColumns
  {
    public QueryMappedItemsColumns15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override MappedItem Bind()
    {
      MappedItem mappedItem = new MappedItem();
      mappedItem.ItemPathPair = this.GetItemPathPair(this.fullPath.GetServerItem(this.Reader, false));
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
