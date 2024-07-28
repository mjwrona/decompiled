// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemPairColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ItemPairColumns : VersionControlObjectBinder<ItemPair>
  {
    protected SqlColumnBinder versionFrom = new SqlColumnBinder("VersionFrom");
    protected SqlColumnBinder command = new SqlColumnBinder("Command");
    protected SqlColumnBinder fullPath = new SqlColumnBinder("FullPath");
    protected SqlColumnBinder itemId = new SqlColumnBinder("ItemId");
    protected SqlColumnBinder encoding = new SqlColumnBinder("Encoding");
    protected SqlColumnBinder hashValue = new SqlColumnBinder("HashValue");
    protected SqlColumnBinder fileLength = new SqlColumnBinder("FileLength");
    protected SqlColumnBinder fileId = new SqlColumnBinder("FileId");
    protected SqlColumnBinder prevVersionFrom = new SqlColumnBinder("PrevVersionFrom");
    protected SqlColumnBinder prevFullPath = new SqlColumnBinder("PrevFullPath");
    protected SqlColumnBinder prevItemId = new SqlColumnBinder("PrevItemId");
    protected SqlColumnBinder prevEncoding = new SqlColumnBinder("PrevEncoding");
    protected SqlColumnBinder prevHashValue = new SqlColumnBinder("PrevHashValue");
    protected SqlColumnBinder prevFileLength = new SqlColumnBinder("PrevFileLength");
    protected SqlColumnBinder prevFileId = new SqlColumnBinder("PrevFileId");

    public ItemPairColumns()
    {
    }

    public ItemPairColumns(VersionControlSqlResourceComponent component)
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
      itemPair.CurrentItem.ItemPathPair = ItemPathPair.FromServerItem(DBPath.DatabaseToServerPath(this.fullPath.GetString((IDataReader) this.Reader, false)));
      itemPair.CurrentItem.ItemId = this.itemId.GetInt32((IDataReader) this.Reader);
      itemPair.CurrentItem.Encoding = this.encoding.GetInt32((IDataReader) this.Reader);
      itemPair.CurrentItem.HashValue = VersionControlObjectBinder<ItemPair>.NormalizeHashValue(this.hashValue.GetBytes((IDataReader) this.Reader, true));
      itemPair.CurrentItem.FileLength = this.fileLength.GetInt64((IDataReader) this.Reader, 0L);
      itemPair.CurrentItem.fileId = this.fileId.GetInt32((IDataReader) this.Reader, 0);
      if (!this.prevVersionFrom.IsNull((IDataReader) this.Reader))
      {
        itemPair.PreviousItem = new Item();
        itemPair.PreviousItem.ChangesetId = this.prevVersionFrom.GetInt32((IDataReader) this.Reader);
        itemPair.PreviousItem.ItemPathPair = ItemPathPair.FromServerItem(DBPath.DatabaseToServerPath(this.prevFullPath.GetString((IDataReader) this.Reader, false)));
        itemPair.PreviousItem.ItemId = this.prevItemId.GetInt32((IDataReader) this.Reader);
        itemPair.PreviousItem.Encoding = this.prevEncoding.GetInt32((IDataReader) this.Reader);
        itemPair.PreviousItem.HashValue = VersionControlObjectBinder<ItemPair>.NormalizeHashValue(this.prevHashValue.GetBytes((IDataReader) this.Reader, true));
        itemPair.PreviousItem.FileLength = this.prevFileLength.GetInt64((IDataReader) this.Reader, 0L);
        itemPair.PreviousItem.fileId = this.prevFileId.GetInt32((IDataReader) this.Reader, 0);
      }
      return itemPair;
    }
  }
}
