// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.AsyncStreamCopier`1
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class AsyncStreamCopier<T> : IDisposable
  {
    private StreamDescriptor streamCopyState;
    private int buffSize;
    private IBufferManager bufferManager;
    private Stream src;
    private Stream dest;
    private CancellationTokenSource cancellationTokenSourceAbort;
    private CancellationTokenSource cancellationTokenSourceTimeout;
    private CancellationTokenSource cancellationTokenSourceCombined;
    private ExecutionState<T> state;
    private Action previousCancellationDelegate;
    private bool disposed;

    public AsyncStreamCopier(
      Stream src,
      Stream dest,
      ExecutionState<T> state,
      IBufferManager bufferManager,
      int? buffSize,
      ChecksumRequested calculateChecksum,
      StreamDescriptor streamCopyState)
    {
      this.src = src;
      this.dest = dest;
      this.state = state;
      this.bufferManager = bufferManager;
      this.buffSize = buffSize ?? (bufferManager != null ? bufferManager.GetDefaultBufferSize() : 65536);
      this.streamCopyState = streamCopyState;
      if (streamCopyState == null || !calculateChecksum.HasAny || streamCopyState.ChecksumWrapper != null)
        return;
      streamCopyState.ChecksumWrapper = new ChecksumWrapper(calculateChecksum.MD5, calculateChecksum.CRC64);
    }

    public Task StartCopyStream(
      Action<ExecutionState<T>> completedDelegate,
      long? copyLength,
      long? maxLength,
      CancellationToken cancellationToken)
    {
      Task task = this.StartCopyStreamAsync(copyLength, maxLength, cancellationToken);
      task.ContinueWith((Action<Task>) (completedStreamCopyTask =>
      {
        this.state.CancelDelegate = this.previousCancellationDelegate;
        if (completedStreamCopyTask.IsFaulted)
          this.state.ExceptionRef = completedStreamCopyTask.Exception.InnerException;
        else if (completedStreamCopyTask.IsCanceled)
        {
          bool flag = false;
          try
          {
            flag = !this.cancellationTokenSourceAbort.IsCancellationRequested;
            if (!flag)
            {
              if (this.cancellationTokenSourceTimeout != null)
                this.cancellationTokenSourceTimeout.Dispose();
            }
          }
          catch (Exception ex)
          {
          }
          try
          {
            if (this.state.Req != null)
            {
              try
              {
                this.state.ReqTimedOut = flag;
                this.state.CancellationTokenSource.Cancel();
              }
              catch (Exception ex)
              {
                Logger.LogWarning(this.state.OperationContext, "Aborting the request failed with exception: {0}", (object) ex);
              }
            }
          }
          catch (Exception ex)
          {
          }
          this.state.ExceptionRef = flag ? (Exception) Exceptions.GenerateTimeoutException(this.state.Cmd != null ? this.state.Cmd.CurrentResult : (RequestResult) null, (Exception) null) : (Exception) Exceptions.GenerateCancellationException(this.state.Cmd != null ? this.state.Cmd.CurrentResult : (RequestResult) null, (Exception) null);
        }
        try
        {
          if (completedDelegate != null)
            completedDelegate(this.state);
        }
        catch (Exception ex)
        {
          this.state.ExceptionRef = ex;
        }
        this.Dispose();
      }));
      return task;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposed && disposing)
      {
        if (this.cancellationTokenSourceAbort != null)
          this.cancellationTokenSourceAbort.Dispose();
        if (this.cancellationTokenSourceTimeout != null)
        {
          this.cancellationTokenSourceTimeout.Dispose();
          this.cancellationTokenSourceCombined.Dispose();
        }
        this.state = (ExecutionState<T>) null;
      }
      this.disposed = true;
    }

    public async Task StartCopyStreamAsync(
      long? copyLength,
      long? maxLength,
      CancellationToken cancellationToken)
    {
      AsyncStreamCopier<T> asyncStreamCopier = this;
      asyncStreamCopier.cancellationTokenSourceAbort = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      lock (asyncStreamCopier.state.CancellationLockerObject)
      {
        asyncStreamCopier.previousCancellationDelegate = asyncStreamCopier.state.CancelDelegate;
        asyncStreamCopier.state.CancelDelegate = new Action(asyncStreamCopier.cancellationTokenSourceAbort.Cancel);
      }
      if (asyncStreamCopier.state.OperationExpiryTime.HasValue)
      {
        asyncStreamCopier.cancellationTokenSourceTimeout = new CancellationTokenSource(asyncStreamCopier.state.RemainingTimeout);
        asyncStreamCopier.cancellationTokenSourceCombined = CancellationTokenSource.CreateLinkedTokenSource(asyncStreamCopier.cancellationTokenSourceAbort.Token, asyncStreamCopier.cancellationTokenSourceTimeout.Token);
      }
      else
        asyncStreamCopier.cancellationTokenSourceCombined = asyncStreamCopier.cancellationTokenSourceAbort;
      await asyncStreamCopier.StartCopyStreamAsyncHelper(copyLength, maxLength, asyncStreamCopier.cancellationTokenSourceCombined.Token).ConfigureAwait(false);
    }

    private async Task StartCopyStreamAsyncHelper(
      long? copyLength,
      long? maxLength,
      CancellationToken token)
    {
      if (copyLength.HasValue && maxLength.HasValue)
        throw new ArgumentException("Cannot specify both copyLength and maxLength.");
      if (this.src.CanSeek && maxLength.HasValue)
      {
        long num = this.src.Length - this.src.Position;
        long? nullable = maxLength;
        long valueOrDefault = nullable.GetValueOrDefault();
        if (num > valueOrDefault & nullable.HasValue)
          throw new InvalidOperationException("The length of the stream exceeds the permitted length.");
      }
      if (this.src.CanSeek && copyLength.HasValue)
      {
        long num = this.src.Length - this.src.Position;
        long? nullable = copyLength;
        long valueOrDefault = nullable.GetValueOrDefault();
        if (num < valueOrDefault & nullable.HasValue)
          throw new ArgumentOutOfRangeException(nameof (copyLength), "The requested number of bytes exceeds the length of the stream remaining from the specified position.");
      }
      token.ThrowIfCancellationRequested();
      byte[] readBuff = this.bufferManager != null ? this.bufferManager.TakeBuffer(this.buffSize) : new byte[this.buffSize];
      byte[] writeBuff = this.bufferManager != null ? this.bufferManager.TakeBuffer(this.buffSize) : new byte[this.buffSize];
      try
      {
        int bytesToCopy1 = this.CalculateBytesToCopy(copyLength, 0L);
        ConfiguredTaskAwaitable<int> configuredTaskAwaitable = this.src.ReadAsync(readBuff, 0, bytesToCopy1, token).ConfigureAwait(false);
        int bytesCopied = await configuredTaskAwaitable;
        long totalBytes = (long) bytesCopied;
        AsyncStreamCopier<T>.CheckMaxLength(maxLength, totalBytes);
        byte[] numArray1 = readBuff;
        readBuff = writeBuff;
        writeBuff = numArray1;
        ExceptionDispatchInfo readException = (ExceptionDispatchInfo) null;
        while (bytesCopied > 0)
        {
          token.ThrowIfCancellationRequested();
          Task task = this.dest.WriteAsync(writeBuff, 0, bytesCopied, token);
          int bytesToCopy2 = this.CalculateBytesToCopy(copyLength, totalBytes);
          Task<int> readTask = (Task<int>) null;
          if (bytesToCopy2 > 0)
          {
            try
            {
              readTask = this.src.ReadAsync(readBuff, 0, bytesToCopy2, token);
            }
            catch (Exception ex)
            {
              readException = ExceptionDispatchInfo.Capture(ex);
            }
          }
          else
            readTask = Task.FromResult<int>(0);
          await task.ConfigureAwait(false);
          this.UpdateStreamCopyState(writeBuff, bytesCopied);
          readException?.Throw();
          configuredTaskAwaitable = readTask.WithCancellation<int>(token).ConfigureAwait(false);
          bytesCopied = await configuredTaskAwaitable;
          totalBytes += (long) bytesCopied;
          AsyncStreamCopier<T>.CheckMaxLength(maxLength, totalBytes);
          byte[] numArray2 = readBuff;
          readBuff = writeBuff;
          writeBuff = numArray2;
          readTask = (Task<int>) null;
        }
        if (copyLength.HasValue && totalBytes != copyLength.Value)
          throw new ArgumentOutOfRangeException(nameof (copyLength), "The requested number of bytes exceeds the length of the stream remaining from the specified position.");
        readException = (ExceptionDispatchInfo) null;
      }
      finally
      {
        if (this.bufferManager != null && readBuff != null)
          this.bufferManager.ReturnBuffer(readBuff);
        if (this.bufferManager != null && writeBuff != null)
          this.bufferManager.ReturnBuffer(writeBuff);
      }
      this.FinalizeStreamCopyState();
      readBuff = (byte[]) null;
      writeBuff = (byte[]) null;
    }

    private void FinalizeStreamCopyState()
    {
      if (this.streamCopyState == null)
        return;
      if (this.streamCopyState.ChecksumWrapper == null)
        return;
      try
      {
        if (this.streamCopyState.ChecksumWrapper.MD5 != null)
          this.streamCopyState.Md5 = this.streamCopyState.ChecksumWrapper.MD5.ComputeHash();
        if (this.streamCopyState.ChecksumWrapper.CRC64 == null)
          return;
        this.streamCopyState.Crc64 = this.streamCopyState.ChecksumWrapper.CRC64.ComputeHash();
      }
      catch (Exception ex)
      {
      }
      finally
      {
        this.streamCopyState.ChecksumWrapper = (ChecksumWrapper) null;
      }
    }

    private static void CheckMaxLength(long? maxLength, long totalBytes)
    {
      if (maxLength.HasValue && totalBytes > maxLength.Value)
        throw new InvalidOperationException("The length of the stream exceeds the permitted length.");
    }

    private void UpdateStreamCopyState(byte[] writeBuff, int bytesCopied)
    {
      if (this.streamCopyState == null)
        return;
      this.streamCopyState.Length += (long) bytesCopied;
      if (this.streamCopyState.ChecksumWrapper == null)
        return;
      this.streamCopyState.ChecksumWrapper.UpdateHash(writeBuff, 0, bytesCopied);
    }

    private int CalculateBytesToCopy(long? copyLength, long totalBytes)
    {
      int val1 = this.buffSize;
      if (copyLength.HasValue)
      {
        if (totalBytes > copyLength.Value)
          throw new InvalidOperationException(string.Format("Internal Error - negative copyLength requested when attempting to copy a stream.  CopyLength = {0}, totalBytes = {1}, total bytes recorded so far = {2}.", (object) copyLength.Value, (object) totalBytes, (object) this.streamCopyState.Length));
        val1 = (int) Math.Min((long) val1, copyLength.Value - totalBytes);
      }
      return val1;
    }
  }
}
