// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspaceAgnosticFileBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataspaceAgnosticFileBinder : FileBinder
  {
    protected SqlColumnBinder ContentIdColumn = new SqlColumnBinder("ContentId");

    protected override TeamFoundationFile Bind()
    {
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
      return new TeamFoundationFile()
      {
        Reference = (FileReference) null,
        Metadata = new FileMetadata()
        {
          ResourceId = guid,
          ContentId = bytes1,
          ContentType = contentType,
          CompressionType = compressionType,
          RemoteStoreId = remoteStoreId,
          HashValue = bytes2,
          FileLength = int64_1,
          CompressedLength = int64_2
        },
        Content = int64_3 == -1L ? (Stream) null : (Stream) new FileBinder.TeamFoundationStream((FileBinder) this, guid, int64_2, int64_3, int64_4)
      };
    }
  }
}
