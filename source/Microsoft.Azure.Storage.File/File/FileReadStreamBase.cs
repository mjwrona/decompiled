// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.FileReadStreamBase
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Azure.Storage.File
{
  internal abstract class FileReadStreamBase : Stream
  {
    protected CloudFile file;
    protected FileProperties fileProperties;
    protected long currentOffset;
    protected MultiBufferMemoryStream internalBuffer;
    protected int streamMinimumReadSizeInBytes;
    protected AccessCondition accessCondition;
    protected FileRequestOptions options;
    protected OperationContext operationContext;
    protected ChecksumWrapper fileChecksum;
    protected Exception lastException;

    protected FileReadStreamBase(
      CloudFile file,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      if (options.ChecksumOptions.UseTransactionalMD5.Value)
        CommonUtility.AssertInBounds<int>("StreamMinimumReadSizeInBytes", file.StreamMinimumReadSizeInBytes, 1, 4194304);
      if (options.ChecksumOptions.UseTransactionalCRC64.Value)
        CommonUtility.AssertInBounds<int>("StreamMinimumReadSizeInBytes", file.StreamMinimumReadSizeInBytes, 1, 4194304);
      this.file = file;
      this.fileProperties = new FileProperties(file.Properties);
      this.currentOffset = 0L;
      this.streamMinimumReadSizeInBytes = this.file.StreamMinimumReadSizeInBytes;
      this.internalBuffer = new MultiBufferMemoryStream(file.ServiceClient.BufferManager);
      this.accessCondition = accessCondition;
      this.options = options;
      this.operationContext = operationContext;
      bool? nullable = this.options.ChecksumOptions.DisableContentMD5Validation;
      int num1 = nullable.Value ? 0 : (!string.IsNullOrEmpty(this.fileProperties.ContentChecksum.MD5) ? 1 : 0);
      nullable = this.options.ChecksumOptions.DisableContentCRC64Validation;
      int num2 = nullable.Value ? 0 : (!string.IsNullOrEmpty(this.fileProperties.ContentChecksum.CRC64) ? 1 : 0);
      this.fileChecksum = new ChecksumWrapper(num1 != 0, num2 != 0);
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

    public override long Length => this.fileProperties.Length;

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
        this.fileChecksum = (ChecksumWrapper) null;
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
      this.VerifyFileChecksum(buffer, offset, count1);
      return count1;
    }

    protected int GetReadSize() => this.currentOffset < this.Length ? (int) Math.Min((long) this.streamMinimumReadSizeInBytes, this.Length - this.currentOffset) : 0;

    protected void VerifyFileChecksum(byte[] buffer, int offset, int count)
    {
      if (this.fileChecksum == null || this.lastException != null || count <= 0)
        return;
      this.fileChecksum.UpdateHash(buffer, offset, count);
      if (this.currentOffset == this.Length && !string.IsNullOrEmpty(this.fileProperties.ContentChecksum.MD5) && this.fileChecksum.MD5 != null)
      {
        string hash = this.fileChecksum.MD5.ComputeHash();
        this.fileChecksum.Dispose();
        this.fileChecksum = (ChecksumWrapper) null;
        if (!hash.Equals(this.fileProperties.ContentChecksum.MD5))
          this.lastException = (Exception) new IOException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "File data corrupted (integrity check failed), Expected value is '{0}', retrieved '{1}'", (object) this.fileProperties.ContentChecksum.MD5, (object) hash));
      }
      if (this.currentOffset != this.Length || string.IsNullOrEmpty(this.fileProperties.ContentChecksum.CRC64) || this.fileChecksum.CRC64 == null)
        return;
      string hash1 = this.fileChecksum.CRC64.ComputeHash();
      this.fileChecksum.Dispose();
      this.fileChecksum = (ChecksumWrapper) null;
      if (hash1.Equals(this.fileProperties.ContentChecksum.CRC64))
        return;
      this.lastException = (Exception) new IOException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "File data corrupted (integrity check failed), Expected value is '{0}', retrieved '{1}'", (object) this.fileProperties.ContentChecksum.CRC64, (object) hash1));
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
        if (this.fileChecksum != null)
        {
          this.fileChecksum.Dispose();
          this.fileChecksum = (ChecksumWrapper) null;
        }
      }
      base.Dispose(disposing);
    }
  }
}
