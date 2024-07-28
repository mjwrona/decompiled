// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Utils.AsyncStreamCopier`1
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand;
using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Utils
{
  internal class AsyncStreamCopier<T> : IDisposable
  {
    private int buffSize;
    private Stream src;
    private Stream dest;
    private CancellationTokenSource cancellationTokenSourceAbort;
    private CancellationTokenSource cancellationTokenSourceTimeout;
    private CancellationTokenSource cancellationTokenSourceCombined;
    private ExecutionState<T> state;
    private Action previousCancellationDelegate;
    private bool disposed;

    public AsyncStreamCopier(Stream src, Stream dest, ExecutionState<T> state, int? buffSize)
    {
      this.src = src;
      this.dest = dest;
      this.state = state;
      this.buffSize = buffSize ?? 65536;
    }

    public Task StartCopyStream(
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
      byte[] readBuff = new byte[this.buffSize];
      byte[] writeBuff = new byte[this.buffSize];
      int bytesToCopy1 = this.CalculateBytesToCopy(copyLength, 0L);
      int count = await this.src.ReadAsync(readBuff, 0, bytesToCopy1, token).ConfigureAwait(false);
      long totalBytes = (long) count;
      AsyncStreamCopier<T>.CheckMaxLength(maxLength, totalBytes);
      byte[] numArray1 = readBuff;
      readBuff = writeBuff;
      writeBuff = numArray1;
      ExceptionDispatchInfo readException = (ExceptionDispatchInfo) null;
      while (count > 0)
      {
        token.ThrowIfCancellationRequested();
        Task task = this.dest.WriteAsync(writeBuff, 0, count, token);
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
        readException?.Throw();
        count = await readTask.WithCancellation<int>(token).ConfigureAwait(false);
        totalBytes += (long) count;
        AsyncStreamCopier<T>.CheckMaxLength(maxLength, totalBytes);
        byte[] numArray2 = readBuff;
        readBuff = writeBuff;
        writeBuff = numArray2;
        readTask = (Task<int>) null;
      }
      if (copyLength.HasValue && totalBytes != copyLength.Value)
        throw new ArgumentOutOfRangeException(nameof (copyLength), "The requested number of bytes exceeds the length of the stream remaining from the specified position.");
    }

    private static void CheckMaxLength(long? maxLength, long totalBytes)
    {
      if (maxLength.HasValue && totalBytes > maxLength.Value)
        throw new InvalidOperationException("The length of the stream exceeds the permitted length.");
    }

    private int CalculateBytesToCopy(long? copyLength, long totalBytes)
    {
      int val1 = this.buffSize;
      if (copyLength.HasValue)
      {
        if (totalBytes > copyLength.Value)
          throw new InvalidOperationException(string.Format("Internal Error - negative copyLength requested when attempting to copy a stream.  CopyLength = {0}, totalBytes = {1}.", (object) copyLength.Value, (object) totalBytes));
        val1 = (int) Math.Min((long) val1, copyLength.Value - totalBytes);
      }
      return val1;
    }
  }
}
