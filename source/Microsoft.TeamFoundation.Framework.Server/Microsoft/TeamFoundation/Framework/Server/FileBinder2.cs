// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileBinder2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileBinder2 : FileBinder
  {
    protected SqlColumnBinder DataspaceIdColumn = new SqlColumnBinder("DataspaceId");
    protected SqlColumnBinder ContentIdColumn = new SqlColumnBinder("ContentId");
    protected FileComponent m_fileComponent;

    public FileBinder2(FileComponent fileComponent) => this.m_fileComponent = fileComponent;

    protected override TeamFoundationFile Bind()
    {
      int int32_1 = this.FileIdColumn.GetInt32((IDataReader) this.Reader, -1);
      int int32_2 = this.DataspaceIdColumn.GetInt32((IDataReader) this.Reader, -1);
      OwnerId ownerId = (OwnerId) this.OwnerIdColumn.GetByte((IDataReader) this.Reader, (byte) 0);
      DateTime dateTime = this.CreationDateColumn.GetDateTime((IDataReader) this.Reader);
      string str = this.FileNameColumn.GetString((IDataReader) this.Reader, true);
      Guid guid = this.ResourceIdColumn.GetGuid((IDataReader) this.Reader);
      byte[] bytes1 = this.ContentIdColumn.GetBytes((IDataReader) this.Reader, true);
      ContentType contentType = (ContentType) this.ContentTypeColumn.GetByte((IDataReader) this.Reader);
      CompressionType compressionType = (CompressionType) this.CompressionTypeColumn.GetByte((IDataReader) this.Reader);
      RemoteStoreId remoteStoreId = (RemoteStoreId) this.RemoteStoreIdColumn.GetByte((IDataReader) this.Reader);
      byte[] bytes2 = this.HashValueColumn.GetBytes((IDataReader) this.Reader, false);
      long int64_1 = this.FileLengthColumn.GetInt64((IDataReader) this.Reader);
      long int64_2 = this.CompressedLengthColumn.GetInt64((IDataReader) this.Reader);
      long int64_3 = this.OffsetFromColumn.GetInt64((IDataReader) this.Reader, -1L);
      long int64_4 = this.OffsetToColumn.GetInt64((IDataReader) this.Reader, -1L);
      TeamFoundationFile teamFoundationFile = new TeamFoundationFile();
      FileReference fileReference;
      if (int32_1 != -1)
        fileReference = new FileReference()
        {
          FileId = (long) int32_1,
          DataspaceIdentifier = this.m_fileComponent.GetDataspaceIdentifier(int32_2),
          ResourceId = guid,
          OwnerId = ownerId,
          CreationDate = dateTime,
          FileName = str
        };
      else
        fileReference = (FileReference) null;
      teamFoundationFile.Reference = fileReference;
      teamFoundationFile.Metadata = new FileMetadata()
      {
        ResourceId = guid,
        ContentId = bytes1,
        ContentType = contentType,
        CompressionType = compressionType,
        RemoteStoreId = remoteStoreId,
        HashValue = bytes2,
        FileLength = int64_1,
        CompressedLength = int64_2
      };
      teamFoundationFile.Content = int64_3 == -1L ? (Stream) null : (Stream) new FileBinder.TeamFoundationStream((FileBinder) this, guid, int64_2, int64_3, int64_4);
      return teamFoundationFile;
    }
  }
}
