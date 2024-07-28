// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.FileBasedMutex
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using System;
using System.IO;
using System.Threading;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal sealed class FileBasedMutex : IDisposable
  {
    private readonly string lockFile;
    private readonly FileSystemWatcher watcher;
    private readonly AutoResetEvent watcherEvent;
    private FileStream stream;
    private bool disposed;
    private static SemaphoreSlim inMemoryLock = new SemaphoreSlim(1, 1);
    private bool releaseInMemoryLock;

    public FileBasedMutex(string lockFile)
    {
      this.lockFile = lockFile;
      string directoryName = Path.GetDirectoryName(lockFile);
      string fileName = Path.GetFileName(lockFile);
      try
      {
        ReparsePointAware.CreateDirectory(directoryName);
        this.watcherEvent = new AutoResetEvent(false);
        this.watcher = new FileSystemWatcher(directoryName, fileName);
        this.watcher.Deleted += new FileSystemEventHandler(this.WatcherDeleted);
        this.watcher.EnableRaisingEvents = true;
      }
      catch
      {
      }
    }

    public bool AcquireMutex(CancellationToken token)
    {
      if (this.watcherEvent == null)
        return false;
      this.watcherEvent.Reset();
      token.Register((Action) (() =>
      {
        if (this.disposed)
          return;
        this.watcherEvent.Set();
      }));
      try
      {
        FileBasedMutex.inMemoryLock.Wait(token);
        if (this.disposed)
        {
          FileBasedMutex.inMemoryLock.Release();
          return false;
        }
        this.releaseInMemoryLock = true;
      }
      catch (OperationCanceledException ex)
      {
        return false;
      }
      while (!token.IsCancellationRequested && !this.disposed)
      {
        if (this.InternalAcquireMutex(token))
          return true;
        if (!token.IsCancellationRequested)
          this.watcherEvent.WaitOne();
      }
      return false;
    }

    public void ReleaseLock()
    {
      if (this.stream != null)
      {
        this.stream.Unlock(0L, 0L);
        this.stream.Close();
        this.stream.Dispose();
        this.stream = (FileStream) null;
        try
        {
          if (File.Exists(this.lockFile))
            ReparsePointAware.DeleteFile(this.lockFile);
        }
        catch
        {
        }
      }
      if (!this.releaseInMemoryLock)
        return;
      this.releaseInMemoryLock = false;
      FileBasedMutex.inMemoryLock.Release();
    }

    public void Dispose()
    {
      this.disposed = true;
      this.ReleaseLock();
      if (this.watcher != null)
        this.watcher.Dispose();
      if (this.watcherEvent == null)
        return;
      this.watcherEvent.Set();
      this.watcherEvent.Dispose();
    }

    private bool InternalAcquireMutex(CancellationToken token)
    {
      try
      {
        this.stream = ReparsePointAware.OpenFile(this.lockFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        this.stream.Lock(0L, 0L);
        if (!token.IsCancellationRequested && !this.disposed)
          return true;
        this.ReleaseLock();
        return false;
      }
      catch
      {
        this.stream = (FileStream) null;
        return false;
      }
    }

    private void WatcherDeleted(object sender, FileSystemEventArgs e)
    {
      if (this.disposed)
        return;
      this.watcherEvent.Set();
    }
  }
}
