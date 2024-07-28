// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.AdjustableSemaphore
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class AdjustableSemaphore : IDisposable
  {
    private readonly object m_lockForUpdatingSemaphore;
    private readonly ManualResetEvent m_waitTerminator;
    private volatile int m_available;
    private volatile int m_maxCount;
    private bool m_disposedValue;

    public int Engaged
    {
      get
      {
        lock (this.m_lockForUpdatingSemaphore)
          return this.m_maxCount - this.m_available;
      }
    }

    public AdjustableSemaphore(int maxCount)
    {
      this.m_lockForUpdatingSemaphore = new object();
      this.m_waitTerminator = new ManualResetEvent(false);
      this.MaxCount = maxCount;
      this.m_available = maxCount;
    }

    public int MaxCount
    {
      get
      {
        lock (this.m_lockForUpdatingSemaphore)
          return this.m_maxCount;
      }
      set
      {
        lock (this.m_lockForUpdatingSemaphore)
        {
          if (value <= 0)
            throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Max number of threads must be greater or equal to 1. Value found: {0}", (object) value)));
          this.m_available += value - this.m_maxCount;
          this.m_maxCount = value;
          this.m_waitTerminator.Set();
        }
      }
    }

    public void Wait()
    {
      while (this.m_available <= 0)
      {
        this.m_waitTerminator.WaitOne();
        this.m_waitTerminator.Reset();
      }
      lock (this.m_lockForUpdatingSemaphore)
        --this.m_available;
    }

    public void Release()
    {
      lock (this.m_lockForUpdatingSemaphore)
      {
        if (this.m_available >= this.m_maxCount)
          throw new SemaphoreFullException("Semaphore released too many times.");
        ++this.m_available;
        this.m_waitTerminator.Set();
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposedValue)
        return;
      if (disposing && this.m_waitTerminator != null)
        this.m_waitTerminator.Dispose();
      this.m_disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
