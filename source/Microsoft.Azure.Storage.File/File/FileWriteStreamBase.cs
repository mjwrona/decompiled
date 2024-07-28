// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.FileWriteStreamBase
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;

namespace Microsoft.Azure.Storage.File
{
  internal abstract class FileWriteStreamBase : CloudFileStream
  {
    protected CloudFile file;
    protected long fileSize;
    protected bool newFile;
    protected long currentOffset;
    protected long currentFileOffset;
    protected int streamWriteSizeInBytes;
    protected MultiBufferMemoryStream internalBuffer;
    protected AccessCondition accessCondition;
    protected FileRequestOptions options;
    protected OperationContext operationContext;
    protected CounterEventAsync noPendingWritesEvent;
    protected ChecksumWrapper fileChecksum;
    protected ChecksumWrapper rangeChecksum;
    protected volatile Exception lastException;
    protected volatile bool committed;
    protected bool disposed;
    protected AsyncSemaphoreAsync parallelOperationSemaphoreAsync;

    protected FileWriteStreamBase(
      CloudFile file,
      long fileSize,
      bool createNew,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      this.internalBuffer = new MultiBufferMemoryStream(file.ServiceClient.BufferManager);
      this.currentOffset = 0L;
      this.accessCondition = accessCondition;
      this.options = options;
      this.operationContext = operationContext;
      this.noPendingWritesEvent = new CounterEventAsync();
      bool? nullable1 = this.options.ChecksumOptions.StoreContentMD5;
      int num1 = nullable1.Value ? 1 : 0;
      nullable1 = this.options.ChecksumOptions.StoreContentCRC64;
      int num2 = nullable1.Value ? 1 : 0;
      this.fileChecksum = new ChecksumWrapper(num1 != 0, num2 != 0);
      bool? nullable2 = this.options.ChecksumOptions.UseTransactionalMD5;
      int num3 = nullable2.Value ? 1 : 0;
      nullable2 = this.options.ChecksumOptions.UseTransactionalCRC64;
      int num4 = nullable2.Value ? 1 : 0;
      this.rangeChecksum = new ChecksumWrapper(num3 != 0, num4 != 0);
      this.parallelOperationSemaphoreAsync = new AsyncSemaphoreAsync(options.ParallelOperationThreadCount.Value);
      this.lastException = (Exception) null;
      this.committed = false;
      this.disposed = false;
      this.currentFileOffset = 0L;
      this.file = file;
      this.fileSize = fileSize;
      this.streamWriteSizeInBytes = file.StreamWriteSizeInBytes;
      this.newFile = createNew;
    }

    public override bool CanRead => false;

    public override bool CanSeek => true;

    public override bool CanWrite => true;

    public override long Length => this.fileSize;

    public override long Position
    {
      get => this.currentOffset;
      set => this.Seek(value, SeekOrigin.Begin);
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    protected long GetNewOffset(long offset, SeekOrigin origin)
    {
      if (!this.CanSeek)
        throw new NotSupportedException();
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
      return val;
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.fileChecksum != null)
        {
          this.fileChecksum.Dispose();
          this.fileChecksum = (ChecksumWrapper) null;
        }
        if (this.rangeChecksum != null)
        {
          this.rangeChecksum.Dispose();
          this.rangeChecksum = (ChecksumWrapper) null;
        }
        if (this.internalBuffer != null)
        {
          this.internalBuffer.Dispose();
          this.internalBuffer = (MultiBufferMemoryStream) null;
        }
      }
      base.Dispose(disposing);
    }

    protected void ThrowLastExceptionIfExists()
    {
      if (this.lastException != null)
        throw this.lastException;
    }
  }
}
