// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.MutexWrapper
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class MutexWrapper : IDisposable
  {
    private Stopwatch m_heldTime;
    private ITFLogger m_logger;
    private string m_name;
    private Semaphore m_semaphore;
    private bool m_owned;
    private Stopwatch m_waitTime;

    public MutexWrapper(string name, ITFLogger logger)
    {
      this.m_semaphore = new Semaphore(1, 1, name);
      this.m_name = name;
      this.m_logger = logger;
      this.m_heldTime = new Stopwatch();
      this.m_waitTime = new Stopwatch();
      this.Wait();
    }

    public void Wait()
    {
      if (this.m_owned)
      {
        this.m_logger.Warning("Mutex {0} is already owned", (object) this.m_name);
      }
      else
      {
        this.m_logger.Info("Acquiring Mutex {0}", (object) this.m_name);
        this.m_waitTime.Restart();
        try
        {
          this.m_owned = this.m_semaphore.WaitOne();
        }
        catch (AbandonedMutexException ex)
        {
          this.m_owned = true;
          this.m_logger.Info("Mutex {0} was abandoned by another process", (object) this.m_name);
        }
        this.m_waitTime.Stop();
        this.m_heldTime.Start();
        this.m_logger.Info("Acquired Mutex {0}.  Took {1}.", (object) this.m_name, (object) this.m_waitTime.Elapsed);
      }
    }

    public void Release()
    {
      if (this.m_owned)
      {
        this.m_semaphore.Release();
        this.m_owned = false;
        this.m_heldTime.Stop();
        this.m_logger.Info("Released Mutex {0}.  Held {1}.", (object) this.m_name, (object) this.m_heldTime.Elapsed);
      }
      else
        this.m_logger.Warning("Mutex {0} is not owned and cannot be released.", (object) this.m_name);
    }

    public void Dispose()
    {
      if (!this.m_owned)
        return;
      this.Release();
    }
  }
}
