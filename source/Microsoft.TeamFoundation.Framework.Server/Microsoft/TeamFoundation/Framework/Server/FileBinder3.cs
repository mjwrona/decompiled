// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileBinder3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileBinder3 : FileBinder2
  {
    public FileBinder3(FileComponent fileComponent)
      : base(fileComponent)
    {
    }

    protected override TeamFoundationFile Bind()
    {
      long int64_1 = this.FileIdColumn.GetInt64((IDataReader) this.Reader, -1L);
      int int32 = this.DataspaceIdColumn.GetInt32((IDataReader) this.Reader, -1);
      OwnerId ownerId = (OwnerId) this.OwnerIdColumn.GetByte((IDataReader) this.Reader, (byte) 0);
      DateTime dateTime = this.CreationDateColumn.GetDateTime((IDataReader) this.Reader);
      string str = this.FileNameColumn.GetString((IDataReader) this.Reader, true);
      Guid guid = this.ResourceIdColumn.GetGuid((IDataReader) this.Reader);
      byte[] bytes1 = this.ContentIdColumn.GetBytes((IDataReader) this.Reader, true);
      ContentType contentType = (ContentType) this.ContentTypeColumn.GetByte((IDataReader) this.Reader);
      CompressionType compressionType = (CompressionType) this.CompressionTypeColumn.GetByte((IDataReader) this.Reader);
      RemoteStoreId remoteStoreId = (RemoteStoreId) this.RemoteStoreIdColumn.GetByte((IDataReader) this.Reader);
      byte[] bytes2 = this.HashValueColumn.GetBytes((IDataReader) this.Reader, false);
      long int64_2 = this.FileLengthColumn.GetInt64((IDataReader) this.Reader);
      long int64_3 = this.CompressedLengthColumn.GetInt64((IDataReader) this.Reader);
      long int64_4 = this.OffsetFromColumn.GetInt64((IDataReader) this.Reader, -1L);
      long int64_5 = this.OffsetToColumn.GetInt64((IDataReader) this.Reader, -1L);
      TeamFoundationFile teamFoundationFile = new TeamFoundationFile();
      FileReference fileReference;
      if (int64_1 != -1L)
        fileReference = new FileReference()
        {
          FileId = int64_1,
          DataspaceIdentifier = this.m_fileComponent.GetDataspaceIdentifier(int32),
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
        FileLength = int64_2,
        CompressedLength = int64_3
      };
      teamFoundationFile.Content = int64_4 == -1L ? (Stream) null : (Stream) new FileBinder.TeamFoundationStream((FileBinder) this, guid, int64_3, int64_4, int64_5);
      return teamFoundationFile;
    }
  }
}
