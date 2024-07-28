// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobReadStreamBase
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Azure.Storage.Blob
{
  internal abstract class BlobReadStreamBase : Stream
  {
    protected CloudBlob blob;
    protected BlobProperties blobProperties;
    protected long currentOffset;
    protected MultiBufferMemoryStream internalBuffer;
    protected int streamMinimumReadSizeInBytes;
    protected AccessCondition accessCondition;
    protected BlobRequestOptions options;
    protected OperationContext operationContext;
    protected ChecksumWrapper blobChecksum;
    protected volatile Exception lastException;

    protected BlobReadStreamBase(
      CloudBlob blob,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      if (options.ChecksumOptions.UseTransactionalMD5.Value)
        CommonUtility.AssertInBounds<int>("StreamMinimumReadSizeInBytes", blob.StreamMinimumReadSizeInBytes, 1, 4194304);
      if (options.ChecksumOptions.UseTransactionalCRC64.Value)
        CommonUtility.AssertInBounds<int>("StreamMinimumReadSizeInBytes", blob.StreamMinimumReadSizeInBytes, 1, 4194304);
      this.blob = blob;
      this.blobProperties = new BlobProperties(blob.Properties);
      this.currentOffset = 0L;
      this.streamMinimumReadSizeInBytes = this.blob.StreamMinimumReadSizeInBytes;
      this.internalBuffer = new MultiBufferMemoryStream(blob.ServiceClient.BufferManager);
      this.accessCondition = accessCondition;
      this.options = options;
      this.operationContext = operationContext;
      bool? nullable = this.options.ChecksumOptions.DisableContentMD5Validation;
      int num1 = nullable.Value ? 0 : (!string.IsNullOrEmpty(this.blobProperties.ContentChecksum.MD5) ? 1 : 0);
      nullable = this.options.ChecksumOptions.DisableContentCRC64Validation;
      int num2 = nullable.Value ? 0 : (!string.IsNullOrEmpty(this.blobProperties.ContentChecksum.CRC64) ? 1 : 0);
      this.blobChecksum = new ChecksumWrapper(num1 != 0, num2 != 0);
      this.lastException = (Exception) null;
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Position
    {
      get => this.currentOffset;
      set => this.Seek(value, SeekOrigin.Begin);
    }

    public override long Length => this.blobProperties.Length;

    public override long Seek(long offset, SeekOrigin origin)
    {
      if (this.lastException != null)
        throw this.lastException;
      long val;
      switch (origin)
      {
        case SeekOrigin.Begin:
          val = offset;
          break;
        case SeekOrigin.Current:
          val = this.currentOffset + offset;
          break;
        case SeekOrigin.End:
          val = this.Length + offset;
          break;
        default:
          CommonUtility.ArgumentOutOfRange(nameof (origin), (object) origin);
          throw new ArgumentOutOfRangeException(nameof (origin));
      }
      CommonUtility.AssertInBounds<long>(nameof (offset), val, 0L, this.Length);
      if (val != this.currentOffset)
      {
        long num = this.internalBuffer.Position + (val - this.currentOffset);
        if (num >= 0L && num < this.internalBuffer.Length)
          this.internalBuffer.Position = num;
        else
          this.internalBuffer.SetLength(0L);
        this.blobChecksum = (ChecksumWrapper) null;
        this.currentOffset = val;
      }
      return this.currentOffset;
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override void Flush()
    {
    }

    protected int ConsumeBuffer(byte[] buffer, int offset, int count)
    {
      int count1 = this.internalBuffer.Read(buffer, offset, count);
      this.currentOffset += (long) count1;
      this.VerifyBlobChecksum(buffer, offset, count1);
      return count1;
    }

    protected int GetReadSize() => this.currentOffset < this.Length ? (int) Math.Min((long) this.streamMinimumReadSizeInBytes, this.Length - this.currentOffset) : 0;

    protected void VerifyBlobChecksum(byte[] buffer, int offset, int count)
    {
      if (this.blobChecksum == null || this.lastException != null || count <= 0)
        return;
      this.blobChecksum.UpdateHash(buffer, offset, count);
      if (this.currentOffset == this.Length && !string.IsNullOrEmpty(this.blobProperties.ContentChecksum.MD5) && this.blobChecksum.MD5 != null)
      {
        string hash = this.blobChecksum.MD5.ComputeHash();
        if (!hash.Equals(this.blobProperties.ContentChecksum.MD5))
          this.lastException = (Exception) new IOException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Blob data corrupted (integrity check failed), Expected value is '{0}', retrieved '{1}'", (object) this.blobProperties.ContentChecksum.MD5, (object) hash));
      }
      if (this.currentOffset == this.Length && !string.IsNullOrEmpty(this.blobProperties.ContentChecksum.CRC64) && this.blobChecksum.CRC64 != null)
      {
        string hash = this.blobChecksum.CRC64.ComputeHash();
        if (!hash.Equals(this.blobProperties.ContentChecksum.CRC64))
          this.lastException = (Exception) new IOException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Blob data corrupted (integrity check failed), Expected value is '{0}', retrieved '{1}'", (object) this.blobProperties.ContentChecksum.CRC64, (object) hash));
      }
      if (true)
        return;
      this.blobChecksum.Dispose();
      this.blobChecksum = (ChecksumWrapper) null;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.internalBuffer != null)
        {
          this.internalBuffer.Dispose();
          this.internalBuffer = (MultiBufferMemoryStream) null;
        }
        if (this.blobChecksum != null)
        {
          this.blobChecksum.Dispose();
          this.blobChecksum = (ChecksumWrapper) null;
        }
      }
      base.Dispose(disposing);
    }
  }
}
