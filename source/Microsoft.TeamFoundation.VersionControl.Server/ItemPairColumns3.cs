// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemPairColumns3
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ItemPairColumns3 : ItemPairColumns
  {
    protected SqlColumnBinder itemDataspaceId = new SqlColumnBinder("ItemDataspaceId");
    protected SqlColumnBinder prevItemDataspaceId = new SqlColumnBinder("PrevItemDataspaceId");

    public ItemPairColumns3(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override ItemPair Bind()
    {
      ItemPair itemPair = new ItemPair()
      {
        ChangeType = VersionControlSqlResourceComponent.GetChangeType((int) this.command.GetInt16((IDataReader) this.Reader)),
        CurrentItem = new Item()
      };
      itemPair.CurrentItem.ChangesetId = this.versionFrom.GetInt32((IDataReader) this.Reader);
      itemPair.CurrentItem.ItemPathPair = this.GetItemPathPair(DBPath.DatabaseToServerPath(this.fullPath.GetString((IDataReader) this.Reader, false)));
      itemPair.CurrentItem.ItemId = this.itemId.GetInt32((IDataReader) this.Reader);
      itemPair.CurrentItem.ItemDataspaceId = this.GetDataspaceIdentifier(this.itemDataspaceId.GetInt32((IDataReader) this.Reader));
      itemPair.CurrentItem.Encoding = this.encoding.GetInt32((IDataReader) this.Reader);
      itemPair.CurrentItem.HashValue = VersionControlObjectBinder<ItemPair>.NormalizeHashValue(this.hashValue.GetBytes((IDataReader) this.Reader, true));
      itemPair.CurrentItem.FileLength = this.fileLength.GetInt64((IDataReader) this.Reader, 0L);
      itemPair.CurrentItem.fileId = this.fileId.GetInt32((IDataReader) this.Reader, 0);
      if (!this.prevVersionFrom.IsNull((IDataReader) this.Reader))
      {
        itemPair.PreviousItem = new Item();
        itemPair.PreviousItem.ChangesetId = this.prevVersionFrom.GetInt32((IDataReader) this.Reader);
        itemPair.PreviousItem.ItemPathPair = this.GetItemPathPair(DBPath.DatabaseToServerPath(this.prevFullPath.GetString((IDataReader) this.Reader, false)));
        itemPair.PreviousItem.ItemId = this.prevItemId.GetInt32((IDataReader) this.Reader);
        itemPair.PreviousItem.ItemDataspaceId = this.GetDataspaceIdentifier(this.prevItemDataspaceId.GetInt32((IDataReader) this.Reader));
        itemPair.PreviousItem.Encoding = this.prevEncoding.GetInt32((IDataReader) this.Reader);
        itemPair.PreviousItem.HashValue = VersionControlObjectBinder<ItemPair>.NormalizeHashValue(this.prevHashValue.GetBytes((IDataReader) this.Reader, true));
        itemPair.PreviousItem.FileLength = this.prevFileLength.GetInt64((IDataReader) this.Reader, 0L);
        itemPair.PreviousItem.fileId = this.prevFileId.GetInt32((IDataReader) this.Reader, 0);
      }
      return itemPair;
    }
  }
}
