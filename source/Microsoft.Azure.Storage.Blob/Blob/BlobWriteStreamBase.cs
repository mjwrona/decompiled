// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobWriteStreamBase
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Storage.Blob
{
  internal abstract class BlobWriteStreamBase : CloudBlobStream
  {
    protected CloudBlockBlob blockBlob;
    protected CloudPageBlob pageBlob;
    protected CloudAppendBlob appendBlob;
    protected long pageBlobSize;
    protected bool newPageBlob;
    protected long currentOffset;
    protected long currentBlobOffset;
    protected int streamWriteSizeInBytes;
    protected MultiBufferMemoryStream internalBuffer;
    protected List<string> blockList;
    protected string blockIdPrefix;
    protected AccessCondition accessCondition;
    protected BlobRequestOptions options;
    protected OperationContext operationContext;
    protected CounterEventAsync noPendingWritesEvent;
    protected ChecksumWrapper blobChecksum;
    protected ChecksumWrapper blockChecksum;
    protected AsyncSemaphoreAsync parallelOperationSemaphoreAsync;
    protected volatile Exception lastException;
    protected volatile bool committed;
    protected bool disposed;

    private BlobWriteStreamBase(
      CloudBlobClient serviceClient,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      this.internalBuffer = new MultiBufferMemoryStream(serviceClient.BufferManager);
      this.accessCondition = accessCondition;
      this.currentOffset = 0L;
      this.options = options;
      this.operationContext = operationContext;
      this.noPendingWritesEvent = new CounterEventAsync();
      bool? nullable1 = this.options.ChecksumOptions.StoreContentMD5;
      int num1 = nullable1.Value ? 1 : 0;
      nullable1 = this.options.ChecksumOptions.StoreContentCRC64;
      int num2 = nullable1.Value ? 1 : 0;
      this.blobChecksum = new ChecksumWrapper(num1 != 0, num2 != 0);
      bool? nullable2 = this.options.ChecksumOptions.UseTransactionalMD5;
      int num3 = nullable2.Value ? 1 : 0;
      nullable2 = this.options.ChecksumOptions.UseTransactionalCRC64;
      int num4 = nullable2.Value ? 1 : 0;
      this.blockChecksum = new ChecksumWrapper(num3 != 0, num4 != 0);
      this.parallelOperationSemaphoreAsync = new AsyncSemaphoreAsync(options.ParallelOperationThreadCount.Value);
      this.lastException = (Exception) null;
      this.committed = false;
      this.disposed = false;
    }

    protected BlobWriteStreamBase(
      CloudBlockBlob blockBlob,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
      : this(blockBlob.ServiceClient, accessCondition, options, operationContext)
    {
      this.blockBlob = blockBlob;
      this.Blob = (CloudBlob) this.blockBlob;
      this.blockList = new List<string>();
      this.blockIdPrefix = Guid.NewGuid().ToString("N") + "-";
      this.streamWriteSizeInBytes = blockBlob.StreamWriteSizeInBytes;
    }

    protected BlobWriteStreamBase(
      CloudPageBlob pageBlob,
      long pageBlobSize,
      bool createNew,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
      : this(pageBlob.ServiceClient, accessCondition, options, operationContext)
    {
      this.currentBlobOffset = 0L;
      this.pageBlob = pageBlob;
      this.Blob = (CloudBlob) this.pageBlob;
      this.pageBlobSize = pageBlobSize;
      this.streamWriteSizeInBytes = pageBlob.StreamWriteSizeInBytes;
      this.newPageBlob = createNew;
    }

    protected BlobWriteStreamBase(
      CloudAppendBlob appendBlob,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
      : this(appendBlob.ServiceClient, accessCondition, options, operationContext)
    {
      this.accessCondition = this.accessCondition ?? new AccessCondition();
      long? appendPositionEqual = this.accessCondition.IfAppendPositionEqual;
      long length;
      if (!appendPositionEqual.HasValue)
      {
        length = appendBlob.Properties.Length;
      }
      else
      {
        appendPositionEqual = this.accessCondition.IfAppendPositionEqual;
        length = appendPositionEqual.Value;
      }
      this.currentBlobOffset = length;
      this.operationContext = this.operationContext ?? new OperationContext();
      this.appendBlob = appendBlob;
      this.Blob = (CloudBlob) this.appendBlob;
      this.parallelOperationSemaphoreAsync = new AsyncSemaphoreAsync(1);
      this.streamWriteSizeInBytes = appendBlob.StreamWriteSizeInBytes;
    }

    protected CloudBlob Blob { get; private set; }

    public override bool CanRead => false;

    public override bool CanSeek => this.pageBlob != null;

    public override bool CanWrite => true;

    public override long Length
    {
      get
      {
        if (this.pageBlob != null)
          return this.pageBlobSize;
        throw new NotSupportedException();
      }
    }

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
      this.ThrowLastExceptionIfExists();
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
      if (val % 512L != 0L)
        CommonUtility.ArgumentOutOfRange(nameof (offset), (object) offset);
      return val;
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    protected string GetCurrentBlockId() => Convert.ToBase64String(Encoding.UTF8.GetBytes(this.blockIdPrefix + this.blockList.Count.ToString("D6", (IFormatProvider) CultureInfo.InvariantCulture)));

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.blobChecksum != null)
        {
          this.blobChecksum.Dispose();
          this.blobChecksum = (ChecksumWrapper) null;
        }
        if (this.blockChecksum != null)
        {
          this.blockChecksum.Dispose();
          this.blockChecksum = (ChecksumWrapper) null;
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
