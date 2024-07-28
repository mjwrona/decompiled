// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileStatisticsRawBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileStatisticsRawBinder : ObjectBinder<FileStatistics>
  {
    protected SqlColumnBinder FileIdColumn = new SqlColumnBinder("FileId");
    protected SqlColumnBinder OwnerIdColumn = new SqlColumnBinder("OwnerId");
    protected SqlColumnBinder CreationDateColumn = new SqlColumnBinder("CreationDate");
    protected SqlColumnBinder FileNameColumn = new SqlColumnBinder("FileName");
    protected SqlColumnBinder ContentTypeColumn = new SqlColumnBinder("ContentType");
    protected SqlColumnBinder CompressionTypeColumn = new SqlColumnBinder("CompressionType");
    protected SqlColumnBinder HashValueColumn = new SqlColumnBinder("HashValue");
    protected SqlColumnBinder FileLengthColumn = new SqlColumnBinder("FileLength");
    protected SqlColumnBinder CompressedLengthColumn = new SqlColumnBinder("CompressedLength");
    protected SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
    protected SqlColumnBinder ResourceId = new SqlColumnBinder(nameof (ResourceId));
    protected SqlColumnBinder ContentId = new SqlColumnBinder(nameof (ContentId));

    protected override FileStatistics Bind()
    {
      FileStatisticsRaw fileStatisticsRaw = new FileStatisticsRaw();
      fileStatisticsRaw.FileId = (long) this.FileIdColumn.GetInt32((IDataReader) this.Reader);
      fileStatisticsRaw.OwnerId = (OwnerId) this.OwnerIdColumn.GetByte((IDataReader) this.Reader);
      fileStatisticsRaw.CreationDate = this.CreationDateColumn.GetDateTime((IDataReader) this.Reader);
      fileStatisticsRaw.FileName = this.FileNameColumn.GetString((IDataReader) this.Reader, true);
      fileStatisticsRaw.ContentType = (ContentType) this.ContentTypeColumn.GetByte((IDataReader) this.Reader);
      fileStatisticsRaw.CompressionType = (CompressionType) this.CompressionTypeColumn.GetByte((IDataReader) this.Reader);
      fileStatisticsRaw.HashValue = this.HashValueColumn.GetBytes((IDataReader) this.Reader, false);
      fileStatisticsRaw.FileLength = this.FileLengthColumn.GetInt64((IDataReader) this.Reader);
      fileStatisticsRaw.CompressedLength = this.CompressedLengthColumn.GetInt64((IDataReader) this.Reader);
      fileStatisticsRaw.DataspaceId = this.DataspaceId.GetInt32((IDataReader) this.Reader);
      fileStatisticsRaw.ResourceId = this.ResourceId.GetGuid((IDataReader) this.Reader);
      fileStatisticsRaw.ContentId = this.ContentId.GetBytes((IDataReader) this.Reader, true);
      return (FileStatistics) fileStatisticsRaw;
    }
  }
}
