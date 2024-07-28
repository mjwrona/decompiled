// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobWriteStream
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  internal sealed class BlobWriteStream : BlobWriteStreamBase
  {
    internal bool IgnoreFlush { get; set; }

    internal BlobWriteStream(
      CloudBlockBlob blockBlob,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
      : base(blockBlob, accessCondition, options, operationContext)
    {
    }

    internal BlobWriteStream(
      CloudPageBlob pageBlob,
      long pageBlobSize,
      bool createNew,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
      : base(pageBlob, pageBlobSize, createNew, accessCondition, options, operationContext)
    {
    }

    internal BlobWriteStream(
      CloudAppendBlob appendBlob,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
      : base(appendBlob, accessCondition, options, operationContext)
    {
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      long currentOffset = this.currentOffset;
      long newOffset = this.GetNewOffset(offset, origin);
      long num = newOffset;
      if (currentOffset != num)
      {
        if (this.blobChecksum != null)
        {
          this.blobChecksum.Dispose();
          this.blobChecksum = (ChecksumWrapper) null;
        }
        this.Flush();
      }
      this.currentOffset = newOffset;
      this.currentBlobOffset = newOffset;
      return this.currentOffset;
    }

    public override void Write(byte[] buffer, int offset, int count) => this.WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();

    public override async Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken token)
    {
      BlobWriteStream blobWriteStream = this;
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      CommonUtility.AssertInBounds<int>(nameof (offset), offset, 0, buffer.Length);
      CommonUtility.AssertInBounds<int>(nameof (count), count, 0, buffer.Length - offset);
      if (blobWriteStream.committed)
        throw new InvalidOperationException("Blob stream has already been committed once.");
      blobWriteStream.currentOffset += (long) count;
      int initialOffset = offset;
      int initialCount = count;
      List<Task> continueTasks = new List<Task>();
      if (blobWriteStream.lastException == null)
      {
        while (count > 0)
        {
          int maxBytesToWrite = blobWriteStream.streamWriteSizeInBytes - (int) blobWriteStream.internalBuffer.Length;
          int bytesToWrite = Math.Min(count, maxBytesToWrite);
          await blobWriteStream.internalBuffer.WriteAsync(buffer, offset, bytesToWrite, token).ConfigureAwait(false);
          if (blobWriteStream.blockChecksum != null)
            blobWriteStream.blockChecksum.UpdateHash(buffer, offset, bytesToWrite);
          count -= bytesToWrite;
          offset += bytesToWrite;
          if (bytesToWrite == maxBytesToWrite)
          {
            TaskCompletionSource<bool> continuetcs = new TaskCompletionSource<bool>();
            continueTasks.Add((Task) continuetcs.Task);
            if (blobWriteStream.DispatchWriteAsync(continuetcs, token).IsFaulted)
              blobWriteStream.ThrowLastExceptionIfExists();
            token.ThrowIfCancellationRequested();
          }
        }
      }
      if (blobWriteStream.blobChecksum != null)
        blobWriteStream.blobChecksum.UpdateHash(buffer, initialOffset, initialCount);
      await Task.WhenAll((IEnumerable<Task>) continueTasks);
      continueTasks = (List<Task>) null;
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.WriteAsync(buffer, offset, count, token)), callback, state);
    }

    public override void EndWrite(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    public override void Flush() => this.FlushAsync(CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

    public override ICancellableAsyncResult BeginFlush(AsyncCallback callback, object state) => CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.FlushAsync(CancellationToken.None)), callback, state);

    public override void EndFlush(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    public override async Task FlushAsync(CancellationToken token)
    {
      BlobWriteStream blobWriteStream = this;
      if (blobWriteStream.committed)
        throw new InvalidOperationException("Blob stream has already been committed once.");
      if (blobWriteStream.IgnoreFlush)
        return;
      blobWriteStream.ThrowLastExceptionIfExists();
      TaskCompletionSource<bool> continueTCS = new TaskCompletionSource<bool>();
      ConfiguredTaskAwaitable configuredTaskAwaitable = blobWriteStream.DispatchWriteAsync(continueTCS, token).ConfigureAwait(false);
      await configuredTaskAwaitable;
      int num = await continueTCS.Task ? 1 : 0;
      configuredTaskAwaitable = blobWriteStream.noPendingWritesEvent.WaitAsync().WithCancellation(token).ConfigureAwait(false);
      await configuredTaskAwaitable;
      blobWriteStream.ThrowLastExceptionIfExists();
      continueTCS = (TaskCompletionSource<bool>) null;
    }

    protected override void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        this.disposed = true;
        if (disposing && !this.committed)
          this.CommitAsync().ConfigureAwait(false).GetAwaiter().GetResult();
      }
      base.Dispose(disposing);
    }

    public override void Commit() => this.CommitAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    public override ICancellableAsyncResult BeginCommit(AsyncCallback callback, object state) => CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.CommitAsync()), callback, state);

    public override void EndCommit(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    public override async Task CommitAsync()
    {
      BlobWriteStream blobWriteStream = this;
      await blobWriteStream.FlushAsync(CancellationToken.None).ConfigureAwait(false);
      blobWriteStream.committed = true;
      try
      {
        if (blobWriteStream.blockBlob != null)
        {
          if (blobWriteStream.blobChecksum != null)
          {
            if (blobWriteStream.blobChecksum.MD5 != null)
              blobWriteStream.blockBlob.Properties.ContentChecksum.MD5 = blobWriteStream.blobChecksum.MD5.ComputeHash();
            if (blobWriteStream.blobChecksum.CRC64 != null)
              blobWriteStream.blockBlob.Properties.ContentChecksum.CRC64 = blobWriteStream.blobChecksum.CRC64.ComputeHash();
          }
          await blobWriteStream.blockBlob.PutBlockListAsync((IEnumerable<string>) blobWriteStream.blockList, blobWriteStream.accessCondition, blobWriteStream.options, blobWriteStream.operationContext).ConfigureAwait(false);
        }
        else
        {
          if (blobWriteStream.blobChecksum == null)
            return;
          if (blobWriteStream.blobChecksum.MD5 != null)
            blobWriteStream.Blob.Properties.ContentChecksum.MD5 = blobWriteStream.blobChecksum.MD5.ComputeHash();
          if (blobWriteStream.blobChecksum.CRC64 != null)
            blobWriteStream.Blob.Properties.ContentChecksum.CRC64 = blobWriteStream.blobChecksum.CRC64.ComputeHash();
          if (!blobWriteStream.blobChecksum.HasAny)
            return;
          await blobWriteStream.Blob.SetPropertiesAsync(blobWriteStream.accessCondition, blobWriteStream.options, blobWriteStream.operationContext).ConfigureAwait(false);
        }
      }
      catch (Exception ex)
      {
        blobWriteStream.lastException = ex;
        throw;
      }
    }

    private async Task DispatchWriteAsync(
      TaskCompletionSource<bool> continuetcs,
      CancellationToken token)
    {
      BlobWriteStream blobWriteStream = this;
      if (blobWriteStream.internalBuffer.Length == 0L)
      {
        if (continuetcs == null)
          return;
        Task.Run<bool>((Func<bool>) (() => continuetcs.TrySetResult(true)));
      }
      else
      {
        MultiBufferMemoryStream internalBuffer = blobWriteStream.internalBuffer;
        blobWriteStream.internalBuffer = new MultiBufferMemoryStream(blobWriteStream.Blob.ServiceClient.BufferManager);
        internalBuffer.Seek(0L, SeekOrigin.Begin);
        Checksum none = Checksum.None;
        if (blobWriteStream.blockChecksum != null)
        {
          bool calcCrc64 = false;
          bool calcMd5 = false;
          if (blobWriteStream.blockChecksum.MD5 != null)
          {
            none.MD5 = blobWriteStream.blockChecksum.MD5.ComputeHash();
            calcMd5 = true;
          }
          if (blobWriteStream.blockChecksum.CRC64 != null)
          {
            none.CRC64 = blobWriteStream.blockChecksum.CRC64.ComputeHash();
            calcCrc64 = true;
          }
          blobWriteStream.blockChecksum.Dispose();
          blobWriteStream.blockChecksum = new ChecksumWrapper(calcMd5, calcCrc64);
        }
        if (blobWriteStream.blockBlob != null)
        {
          string currentBlockId = blobWriteStream.GetCurrentBlockId();
          blobWriteStream.blockList.Add(currentBlockId);
          await blobWriteStream.WriteBlockAsync(continuetcs, (Stream) internalBuffer, currentBlockId, none, token).ConfigureAwait(false);
        }
        else if (blobWriteStream.pageBlob != null)
        {
          if (internalBuffer.Length % 512L != 0L)
          {
            blobWriteStream.lastException = (Exception) new IOException("Page data must be a multiple of 512 bytes.");
            throw blobWriteStream.lastException;
          }
          long currentBlobOffset = blobWriteStream.currentBlobOffset;
          blobWriteStream.currentBlobOffset += internalBuffer.Length;
          await blobWriteStream.WritePagesAsync(continuetcs, (Stream) internalBuffer, currentBlobOffset, none, token).ConfigureAwait(false);
        }
        else
        {
          long currentBlobOffset1 = blobWriteStream.currentBlobOffset;
          blobWriteStream.currentBlobOffset += internalBuffer.Length;
          long? sizeLessThanOrEqual = blobWriteStream.accessCondition.IfMaxSizeLessThanOrEqual;
          if (sizeLessThanOrEqual.HasValue)
          {
            long currentBlobOffset2 = blobWriteStream.currentBlobOffset;
            sizeLessThanOrEqual = blobWriteStream.accessCondition.IfMaxSizeLessThanOrEqual;
            long num = sizeLessThanOrEqual.Value;
            if (currentBlobOffset2 > num)
            {
              blobWriteStream.lastException = (Exception) new IOException("Append block data should not exceed the maximum blob size condition value.");
              throw blobWriteStream.lastException;
            }
          }
          await blobWriteStream.WriteAppendBlockAsync(continuetcs, (Stream) internalBuffer, currentBlobOffset1, none, token).ConfigureAwait(false);
        }
      }
    }

    private Task WriteBlockAsync(
      TaskCompletionSource<bool> continuetcs,
      Stream blockData,
      string blockId,
      Checksum blockChecksum,
      CancellationToken token)
    {
      this.noPendingWritesEvent.Increment();
      Func<bool> func;
      return (Task) this.parallelOperationSemaphoreAsync.WaitAsync((Func<bool, CancellationToken, Task>) (async (runningInline, internalToken) =>
      {
        try
        {
          if (continuetcs != null)
            Task.Run<bool>(func ?? (func = (Func<bool>) (() => continuetcs.TrySetResult(true))));
          await this.blockBlob.PutBlockAsync(blockId, blockData, blockChecksum, this.accessCondition, this.options, this.operationContext, internalToken).ConfigureAwait(false);
          blockData.Dispose();
          blockData = (Stream) null;
        }
        catch (Exception ex)
        {
          this.lastException = ex;
        }
        finally
        {
          ConfiguredTaskAwaitable configuredTaskAwaitable = this.noPendingWritesEvent.DecrementAsync().ConfigureAwait(false);
          await configuredTaskAwaitable;
          configuredTaskAwaitable = this.parallelOperationSemaphoreAsync.ReleaseAsync(internalToken).ConfigureAwait(false);
          await configuredTaskAwaitable;
        }
      }), token);
    }

    private Task WritePagesAsync(
      TaskCompletionSource<bool> continuetcs,
      Stream pageData,
      long offset,
      Checksum contentChecksum,
      CancellationToken token)
    {
      this.noPendingWritesEvent.Increment();
      Func<bool> func;
      return (Task) this.parallelOperationSemaphoreAsync.WaitAsync((Func<bool, CancellationToken, Task>) (async (runningInline, internalToken) =>
      {
        try
        {
          if (continuetcs != null)
            Task.Run<bool>(func ?? (func = (Func<bool>) (() => continuetcs.TrySetResult(true))));
          await this.pageBlob.WritePagesAsync(pageData, offset, contentChecksum, this.accessCondition, this.options, this.operationContext, internalToken).ConfigureAwait(false);
          pageData.Dispose();
          pageData = (Stream) null;
        }
        catch (Exception ex)
        {
          this.lastException = ex;
        }
        finally
        {
          ConfiguredTaskAwaitable configuredTaskAwaitable = this.noPendingWritesEvent.DecrementAsync().ConfigureAwait(false);
          await configuredTaskAwaitable;
          configuredTaskAwaitable = this.parallelOperationSemaphoreAsync.ReleaseAsync(internalToken).ConfigureAwait(false);
          await configuredTaskAwaitable;
        }
      }), token);
    }

    private Task WriteAppendBlockAsync(
      TaskCompletionSource<bool> continuetcs,
      Stream blockData,
      long offset,
      Checksum blockChecksum,
      CancellationToken token)
    {
      this.noPendingWritesEvent.Increment();
      Func<bool> func;
      return (Task) this.parallelOperationSemaphoreAsync.WaitAsync((Func<bool, CancellationToken, Task>) (async (runningInline, internalToken) =>
      {
        int previousResultsCount = this.operationContext.RequestResults.Count;
        object obj = (object) null;
        int num = 0;
        try
        {
          try
          {
            if (continuetcs != null)
              Task.Run<bool>(func ?? (func = (Func<bool>) (() => continuetcs.TrySetResult(true))));
            this.accessCondition.IfAppendPositionEqual = new long?(offset);
            long num1 = await this.appendBlob.AppendBlockAsync(blockData, blockChecksum, this.accessCondition, this.options, this.operationContext, (IProgress<StorageProgress>) null, internalToken).ConfigureAwait(false);
            blockData.Dispose();
            blockData = (Stream) null;
            goto label_13;
          }
          catch (StorageException ex)
          {
            if (this.options.AbsorbConditionalErrorsOnRetry.Value && ex.RequestInformation.HttpStatusCode == 412)
            {
              StorageExtendedErrorInformation errorInformation = ex.RequestInformation.ExtendedErrorInformation;
              if (errorInformation != null && (errorInformation.ErrorCode == BlobErrorCodeStrings.InvalidAppendCondition || errorInformation.ErrorCode == BlobErrorCodeStrings.InvalidMaxBlobSizeCondition) && this.operationContext.RequestResults.Count - previousResultsCount > 1)
              {
                Logger.LogWarning(this.operationContext, "Pre-condition failure on a retry is being ignored since the request should have succeeded in the first attempt.");
                goto label_11;
              }
            }
            this.lastException = (Exception) ex;
            goto label_13;
          }
          catch (Exception ex)
          {
            this.lastException = ex;
            goto label_13;
          }
label_11:
          num = 1;
        }
        catch (object ex)
        {
          obj = ex;
        }
label_13:
        ConfiguredTaskAwaitable configuredTaskAwaitable = this.noPendingWritesEvent.DecrementAsync().ConfigureAwait(false);
        await configuredTaskAwaitable;
        configuredTaskAwaitable = this.parallelOperationSemaphoreAsync.ReleaseAsync(internalToken).ConfigureAwait(false);
        await configuredTaskAwaitable;
        object obj1 = obj;
        if (obj1 != null)
        {
          if (!(obj1 is Exception source2))
            throw obj1;
          ExceptionDispatchInfo.Capture(source2).Throw();
        }
        if (num == 1)
          return;
        obj = (object) null;
      }), token);
    }
  }
}
