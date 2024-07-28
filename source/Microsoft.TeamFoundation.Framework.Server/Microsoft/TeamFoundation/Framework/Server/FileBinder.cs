// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileBinder : ObjectBinder<TeamFoundationFile>
  {
    protected SqlColumnBinder FileIdColumn = new SqlColumnBinder("FileId");
    protected SqlColumnBinder OwnerIdColumn = new SqlColumnBinder("OwnerId");
    protected SqlColumnBinder CreationDateColumn = new SqlColumnBinder("CreationDate");
    protected SqlColumnBinder FileNameColumn = new SqlColumnBinder("FileName");
    protected SqlColumnBinder ResourceIdColumn = new SqlColumnBinder("ResourceId");
    protected SqlColumnBinder ContentTypeColumn = new SqlColumnBinder("ContentType");
    protected SqlColumnBinder CompressionTypeColumn = new SqlColumnBinder("CompressionType");
    protected SqlColumnBinder HashValueColumn = new SqlColumnBinder("HashValue");
    protected SqlColumnBinder FileLengthColumn = new SqlColumnBinder("FileLength");
    protected SqlColumnBinder CompressedLengthColumn = new SqlColumnBinder("CompressedLength");
    protected SqlColumnBinder RemoteStoreIdColumn = new SqlColumnBinder("RemoteStoreId");
    protected SqlColumnBinder OffsetFromColumn = new SqlColumnBinder("OffsetFrom");
    protected SqlColumnBinder OffsetToColumn = new SqlColumnBinder("OffsetTo");
    protected SqlColumnBinder ContentColumn = new SqlColumnBinder("Content");

    protected override TeamFoundationFile Bind()
    {
      int int32 = this.FileIdColumn.GetInt32((IDataReader) this.Reader);
      OwnerId ownerId = (OwnerId) this.OwnerIdColumn.GetByte((IDataReader) this.Reader);
      byte[] bytes = this.HashValueColumn.GetBytes((IDataReader) this.Reader, false);
      long int64_1 = this.FileLengthColumn.GetInt64((IDataReader) this.Reader);
      ContentType contentType = (ContentType) this.ContentTypeColumn.GetByte((IDataReader) this.Reader);
      DateTime dateTime = this.CreationDateColumn.GetDateTime((IDataReader) this.Reader);
      long int64_2 = this.CompressedLengthColumn.GetInt64((IDataReader) this.Reader);
      CompressionType compressionType = (CompressionType) this.CompressionTypeColumn.GetByte((IDataReader) this.Reader);
      RemoteStoreId remoteStoreId = (RemoteStoreId) this.RemoteStoreIdColumn.GetByte((IDataReader) this.Reader);
      Guid guid = this.ResourceIdColumn.GetGuid((IDataReader) this.Reader);
      string str = this.FileNameColumn.GetString((IDataReader) this.Reader, true);
      long int64_3 = this.OffsetFromColumn.GetInt64((IDataReader) this.Reader, -1L);
      long int64_4 = this.OffsetToColumn.GetInt64((IDataReader) this.Reader, -1L);
      return new TeamFoundationFile()
      {
        Reference = new FileReference()
        {
          FileId = (long) int32,
          DataspaceIdentifier = Guid.Empty,
          ResourceId = guid,
          OwnerId = ownerId,
          CreationDate = dateTime,
          FileName = str
        },
        Metadata = new FileMetadata()
        {
          ResourceId = guid,
          ContentId = (byte[]) null,
          ContentType = contentType,
          CompressionType = compressionType,
          RemoteStoreId = remoteStoreId,
          HashValue = bytes,
          FileLength = int64_1,
          CompressedLength = int64_2
        },
        Content = int64_3 == -1L ? (Stream) null : (Stream) new FileBinder.TeamFoundationStream(this, guid, int64_2, int64_3, int64_4)
      };
    }

    internal bool BindNextChunk(Guid expectedResourceId, out long offsetFrom, out long offsetTo)
    {
      if (this.Reader.Read())
      {
        this.ResourceIdColumn.GetGuid((IDataReader) this.Reader);
        offsetFrom = this.OffsetFromColumn.GetInt64((IDataReader) this.Reader);
        offsetTo = this.OffsetToColumn.GetInt64((IDataReader) this.Reader);
        return true;
      }
      offsetFrom = 0L;
      offsetTo = 0L;
      return false;
    }

    internal int ReadContent(long offset, byte[] buffer, int bufferIndex, int length) => this.ContentColumn.GetBytes((IDataReader) this.Reader, offset, buffer, bufferIndex, length);

    protected class TeamFoundationStream : Stream
    {
      private FileBinder m_binder;
      private Guid m_resourceId;
      private long m_length;
      private long m_position;
      private long m_offsetTo;
      private long m_offsetFrom;

      public TeamFoundationStream(
        FileBinder binder,
        Guid resourceId,
        long length,
        long offsetFrom,
        long offsetTo)
      {
        this.m_binder = binder;
        this.m_resourceId = resourceId;
        this.m_length = length;
        this.m_position = -1L;
        this.m_offsetFrom = offsetFrom;
        this.m_offsetTo = offsetTo;
      }

      public override bool CanRead => true;

      public override bool CanSeek => false;

      public override bool CanWrite => false;

      public override long Length => this.m_length;

      public override long Position
      {
        get => this.m_position;
        set => throw new NotImplementedException();
      }

      protected override void Dispose(bool disposing)
      {
      }

      public override void Flush()
      {
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        int num1 = this.m_binder.ReadContent(this.m_position - this.m_offsetFrom + 1L, buffer, offset, count);
        this.m_position += (long) num1;
        if (num1 < count - offset && this.m_position >= this.m_offsetTo && this.m_offsetTo + 1L < this.m_length && this.ReadChunk())
        {
          int num2 = this.m_binder.ReadContent(this.m_position - this.m_offsetFrom + 1L, buffer, offset + num1, count - offset - num1);
          this.m_position += (long) num2;
          num1 += num2;
        }
        return num1;
      }

      public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

      public override void SetLength(long value) => throw new NotImplementedException();

      public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

      private bool ReadChunk()
      {
        long offsetFrom;
        long offsetTo;
        if (!this.m_binder.BindNextChunk(this.m_resourceId, out offsetFrom, out offsetTo))
          return false;
        this.m_offsetFrom = offsetFrom;
        this.m_offsetTo = offsetTo;
        return true;
      }
    }
  }
}
