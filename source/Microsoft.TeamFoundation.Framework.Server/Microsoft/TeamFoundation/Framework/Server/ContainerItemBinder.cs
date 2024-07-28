// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContainerItemBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ContainerItemBinder : ObjectBinder<FileContainerItem>
  {
    protected SqlColumnBinder ContainerIdColumn = new SqlColumnBinder("ContainerId");
    protected SqlColumnBinder PathColumn = new SqlColumnBinder("Path");
    protected SqlColumnBinder ItemTypeColumn = new SqlColumnBinder("ItemType");
    protected SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
    protected SqlColumnBinder FileIdColumn = new SqlColumnBinder("FileId");
    protected SqlColumnBinder FileLengthColumn = new SqlColumnBinder("FileLength");
    protected SqlColumnBinder FileEncodingColumn = new SqlColumnBinder("FileEncoding");
    protected SqlColumnBinder FileTypeColumn = new SqlColumnBinder("FileType");
    protected SqlColumnBinder DateCreatedColumn = new SqlColumnBinder("DateCreated");
    protected SqlColumnBinder DateLastModifiedColumn = new SqlColumnBinder("DateLastModified");
    protected SqlColumnBinder CreatedByColumn = new SqlColumnBinder("CreatedBy");
    protected SqlColumnBinder LastModifiedByColumn = new SqlColumnBinder("LastModifiedBy");
    private SqlColumnBinder ContentIdColumn = new SqlColumnBinder("ContentId");

    protected override FileContainerItem Bind()
    {
      byte[] numArray = (byte[]) null;
      if (this.ContentIdColumn.ColumnExists((IDataReader) this.Reader))
        numArray = this.ContentIdColumn.GetBytes((IDataReader) this.Reader, true);
      return new FileContainerItem()
      {
        ContainerId = this.ContainerIdColumn.GetInt64((IDataReader) this.Reader),
        Path = DBPath.DatabaseToUserPath(this.PathColumn.GetString((IDataReader) this.Reader, false), true, true),
        ItemType = (ContainerItemType) this.ItemTypeColumn.GetByte((IDataReader) this.Reader),
        Status = (ContainerItemStatus) this.StatusColumn.GetByte((IDataReader) this.Reader),
        FileId = this.FileIdColumn.GetInt32((IDataReader) this.Reader, 0),
        FileLength = this.FileLengthColumn.GetInt64((IDataReader) this.Reader, 0L),
        FileEncoding = this.FileEncodingColumn.GetInt32((IDataReader) this.Reader, 0),
        FileType = this.FileTypeColumn.GetInt32((IDataReader) this.Reader, 0),
        DateCreated = this.DateCreatedColumn.GetDateTime((IDataReader) this.Reader),
        DateLastModified = this.DateLastModifiedColumn.GetDateTime((IDataReader) this.Reader),
        CreatedBy = this.CreatedByColumn.GetGuid((IDataReader) this.Reader, false),
        LastModifiedBy = this.LastModifiedByColumn.GetGuid((IDataReader) this.Reader, false),
        ContentId = numArray
      };
    }
  }
}
