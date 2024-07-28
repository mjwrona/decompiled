// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.FileWriteStream
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.File
{
  internal sealed class FileWriteStream : FileWriteStreamBase
  {
    internal FileWriteStream(
      CloudFile file,
      long fileSize,
      bool createNew,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
      : base(file, fileSize, createNew, accessCondition, options, operationContext)
    {
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      long currentOffset = this.currentOffset;
      long newOffset = this.GetNewOffset(offset, origin);
      long num = newOffset;
      if (currentOffset != num)
      {
        if (this.fileChecksum != null)
        {
          this.fileChecksum.Dispose();
          this.fileChecksum = (ChecksumWrapper) null;
        }
        this.Flush();
      }
      this.currentOffset = newOffset;
      this.currentFileOffset = newOffset;
      return this.currentOffset;
    }

    public override void Write(byte[] buffer, int offset, int count) => this.WriteAsync(buffer, offset, count, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

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

    public override async Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      FileWriteStream fileWriteStream = this;
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      CommonUtility.AssertInBounds<int>(nameof (offset), offset, 0, buffer.Length);
      CommonUtility.AssertInBounds<int>(nameof (count), count, 0, buffer.Length - offset);
      if (fileWriteStream.committed)
        throw new InvalidOperationException("File stream has already been committed once.");
      fileWriteStream.currentOffset += (long) count;
      int initialOffset = offset;
      int initialCount = count;
      List<Task> continueTasks = new List<Task>();
      if (fileWriteStream.lastException == null)
      {
        while (count > 0)
        {
          int maxBytesToWrite = fileWriteStream.streamWriteSizeInBytes - (int) fileWriteStream.internalBuffer.Length;
          int bytesToWrite = Math.Min(count, maxBytesToWrite);
          await fileWriteStream.internalBuffer.WriteAsync(buffer, offset, bytesToWrite, cancellationToken).ConfigureAwait(false);
          if (fileWriteStream.rangeChecksum != null)
            fileWriteStream.rangeChecksum.UpdateHash(buffer, offset, bytesToWrite);
          count -= bytesToWrite;
          offset += bytesToWrite;
          if (bytesToWrite == maxBytesToWrite)
          {
            TaskCompletionSource<bool> continuetcs = new TaskCompletionSource<bool>();
            continueTasks.Add((Task) continuetcs.Task);
            if (fileWriteStream.DispatchWriteAsync(continuetcs, cancellationToken).IsFaulted)
              fileWriteStream.ThrowLastExceptionIfExists();
            cancellationToken.ThrowIfCancellationRequested();
          }
        }
      }
      if (fileWriteStream.fileChecksum != null)
        fileWriteStream.fileChecksum.UpdateHash(buffer, initialOffset, initialCount);
      await Task.WhenAll((IEnumerable<Task>) continueTasks);
      continueTasks = (List<Task>) null;
    }

    public override void Flush() => this.FlushAsync(CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

    public override ICancellableAsyncResult BeginFlush(AsyncCallback callback, object state) => CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.FlushAsync(CancellationToken.None)), callback, state);

    public override void EndFlush(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
      FileWriteStream fileWriteStream = this;
      if (fileWriteStream.committed)
        throw new InvalidOperationException("Blob stream has already been committed once.");
      fileWriteStream.ThrowLastExceptionIfExists();
      TaskCompletionSource<bool> continueTCS = new TaskCompletionSource<bool>();
      ConfiguredTaskAwaitable configuredTaskAwaitable = fileWriteStream.DispatchWriteAsync(continueTCS, cancellationToken).ConfigureAwait(false);
      await configuredTaskAwaitable;
      int num = await continueTCS.Task ? 1 : 0;
      configuredTaskAwaitable = fileWriteStream.noPendingWritesEvent.WaitAsync().WithCancellation(cancellationToken).ConfigureAwait(false);
      await configuredTaskAwaitable;
      fileWriteStream.ThrowLastExceptionIfExists();
      continueTCS = (TaskCompletionSource<bool>) null;
    }

    protected override void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        this.disposed = true;
        if (disposing && !this.committed)
          this.CommitAsync().GetAwaiter().GetResult();
      }
      base.Dispose(disposing);
    }

    public override void Commit() => this.CommitAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    public override ICancellableAsyncResult BeginCommit(AsyncCallback callback, object state) => CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.CommitAsync()), callback, state);

    public override void EndCommit(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    public override async Task CommitAsync()
    {
      FileWriteStream fileWriteStream = this;
      await fileWriteStream.FlushAsync(CancellationToken.None).ConfigureAwait(false);
      fileWriteStream.committed = true;
      try
      {
        if (fileWriteStream.fileChecksum == null)
          return;
        if (fileWriteStream.fileChecksum.MD5 != null)
          fileWriteStream.file.Properties.ContentChecksum.MD5 = fileWriteStream.fileChecksum.MD5.ComputeHash();
        if (fileWriteStream.fileChecksum.CRC64 != null)
          fileWriteStream.file.Properties.ContentChecksum.CRC64 = fileWriteStream.fileChecksum.CRC64.ComputeHash();
        if (!fileWriteStream.fileChecksum.HasAny)
          return;
        await fileWriteStream.file.SetPropertiesAsync(fileWriteStream.accessCondition, fileWriteStream.options, fileWriteStream.operationContext).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        fileWriteStream.lastException = ex;
        throw;
      }
    }

    private async Task DispatchWriteAsync(
      TaskCompletionSource<bool> continuetcs,
      CancellationToken token)
    {
      FileWriteStream fileWriteStream = this;
      if (fileWriteStream.internalBuffer.Length == 0L)
      {
        if (continuetcs == null)
          return;
        Task.Run<bool>((Func<bool>) (() => continuetcs.TrySetResult(true)));
      }
      else
      {
        MultiBufferMemoryStream internalBuffer = fileWriteStream.internalBuffer;
        fileWriteStream.internalBuffer = new MultiBufferMemoryStream(fileWriteStream.file.ServiceClient.BufferManager);
        internalBuffer.Seek(0L, SeekOrigin.Begin);
        Checksum none = Checksum.None;
        if (fileWriteStream.rangeChecksum != null)
        {
          bool calcCrc64 = false;
          bool calcMd5 = false;
          if (fileWriteStream.rangeChecksum.MD5 != null)
          {
            none.MD5 = fileWriteStream.rangeChecksum.MD5.ComputeHash();
            calcMd5 = true;
          }
          if (fileWriteStream.rangeChecksum.CRC64 != null)
          {
            none.CRC64 = fileWriteStream.rangeChecksum.CRC64.ComputeHash();
            calcCrc64 = true;
          }
          fileWriteStream.rangeChecksum.Dispose();
          fileWriteStream.rangeChecksum = new ChecksumWrapper(calcMd5, calcCrc64);
        }
        long currentFileOffset = fileWriteStream.currentFileOffset;
        fileWriteStream.currentFileOffset += internalBuffer.Length;
        await fileWriteStream.WriteRangeAsync(continuetcs, (Stream) internalBuffer, currentFileOffset, none, token).ConfigureAwait(false);
      }
    }

    private Task WriteRangeAsync(
      TaskCompletionSource<bool> continuetcs,
      Stream rangeData,
      long offset,
      Checksum contentChecksum,
      CancellationToken token)
    {
      this.noPendingWritesEvent.Increment();
      Func<bool> func;
      return (Task) this.parallelOperationSemaphoreAsync.WaitAsync((Func<bool, CancellationToken, Task>) (async (runingInline, internalToken) =>
      {
        try
        {
          if (continuetcs != null)
            Task.Run<bool>(func ?? (func = (Func<bool>) (() => continuetcs.TrySetResult(true))));
          await this.file.WriteRangeAsync(rangeData, offset, contentChecksum, this.accessCondition, this.options, this.operationContext, (IProgress<StorageProgress>) null, internalToken).ConfigureAwait(false);
          rangeData.Dispose();
          rangeData = (Stream) null;
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
  }
}
