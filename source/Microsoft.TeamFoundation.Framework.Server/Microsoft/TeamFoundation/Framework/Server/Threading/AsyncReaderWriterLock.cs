// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Threading.AsyncReaderWriterLock
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server.Threading
{
  public class AsyncReaderWriterLock
  {
    private readonly Task<IDisposable> readerReleaser;
    private readonly Task<IDisposable> writerReleaser;
    private readonly Queue<TaskCompletionSource<AsyncReaderWriterLock.Releaser>> waitingWriters = new Queue<TaskCompletionSource<AsyncReaderWriterLock.Releaser>>();
    private TaskCompletionSource<AsyncReaderWriterLock.Releaser> waitingReader = new TaskCompletionSource<AsyncReaderWriterLock.Releaser>();
    private int readersWaiting;
    private int status;

    public AsyncReaderWriterLock()
    {
      this.readerReleaser = Task.FromResult<IDisposable>((IDisposable) new AsyncReaderWriterLock.Releaser(this, false));
      this.writerReleaser = Task.FromResult<IDisposable>((IDisposable) new AsyncReaderWriterLock.Releaser(this, true));
    }

    public Task<IDisposable> ReaderLockAsync() => this.ReaderLockAsync(TimeSpan.MaxValue);

    public Task<IDisposable> ReaderLockAsync(TimeSpan timeout)
    {
      lock (this.waitingWriters)
      {
        if (this.status >= 0 && this.waitingWriters.Count == 0)
        {
          ++this.status;
          return this.readerReleaser;
        }
        ++this.readersWaiting;
        return this.WaitWithTimeout<IDisposable>(this.waitingReader.Task.ContinueWith<IDisposable>((Func<Task<AsyncReaderWriterLock.Releaser>, IDisposable>) (t => (IDisposable) t.Result)), timeout);
      }
    }

    public Task<IDisposable> WriterLockAsync() => this.WriterLockAsync(TimeSpan.MaxValue);

    public Task<IDisposable> WriterLockAsync(TimeSpan timeout)
    {
      lock (this.waitingWriters)
      {
        if (this.status == 0)
        {
          this.status = -1;
          return this.writerReleaser;
        }
        TaskCompletionSource<AsyncReaderWriterLock.Releaser> completionSource = new TaskCompletionSource<AsyncReaderWriterLock.Releaser>();
        this.waitingWriters.Enqueue(completionSource);
        return this.WaitWithTimeout<IDisposable>(completionSource.Task.ContinueWith<IDisposable>((Func<Task<AsyncReaderWriterLock.Releaser>, IDisposable>) (t => (IDisposable) t.Result)), timeout);
      }
    }

    private Task<T> WaitWithTimeout<T>(Task<T> task, TimeSpan timeout) => timeout == TimeSpan.MaxValue ? task : this.WaitWithTimeoutInternal<T>(task, timeout);

    private async Task<T> WaitWithTimeoutInternal<T>(Task<T> task, TimeSpan timeout)
    {
      object obj = (object) task;
      Task[] taskArray = new Task[2]
      {
        Task.Delay(timeout),
        (Task) task
      };
      if (obj != await Task.WhenAny(taskArray).ConfigureAwait(false))
      {
        obj = (object) null;
        throw new TimeoutException("Timed out waiting for async reader/writer lock.");
      }
      return await task.ConfigureAwait(false);
    }

    private void ReaderRelease()
    {
      TaskCompletionSource<AsyncReaderWriterLock.Releaser> completionSource = (TaskCompletionSource<AsyncReaderWriterLock.Releaser>) null;
      lock (this.waitingWriters)
      {
        --this.status;
        if (this.status == 0)
        {
          if (this.waitingWriters.Count > 0)
          {
            this.status = -1;
            completionSource = this.waitingWriters.Dequeue();
          }
        }
      }
      completionSource?.SetResult(new AsyncReaderWriterLock.Releaser(this, true));
    }

    private void WriterRelease()
    {
      TaskCompletionSource<AsyncReaderWriterLock.Releaser> completionSource = (TaskCompletionSource<AsyncReaderWriterLock.Releaser>) null;
      bool isWriter = false;
      lock (this.waitingWriters)
      {
        if (this.waitingWriters.Count > 0)
        {
          completionSource = this.waitingWriters.Dequeue();
          isWriter = true;
        }
        else if (this.readersWaiting > 0)
        {
          completionSource = this.waitingReader;
          this.status = this.readersWaiting;
          this.readersWaiting = 0;
          this.waitingReader = new TaskCompletionSource<AsyncReaderWriterLock.Releaser>();
        }
        else
          this.status = 0;
      }
      completionSource?.SetResult(new AsyncReaderWriterLock.Releaser(this, isWriter));
    }

    private readonly struct Releaser : IDisposable
    {
      private readonly AsyncReaderWriterLock toRelease;
      private readonly bool isWriter;

      public Releaser(AsyncReaderWriterLock toRelease, bool isWriter)
      {
        this.toRelease = toRelease;
        this.isWriter = isWriter;
      }

      public void Dispose()
      {
        if (this.toRelease == null)
          return;
        if (this.isWriter)
          this.toRelease.WriterRelease();
        else
          this.toRelease.ReaderRelease();
      }
    }
  }
}
