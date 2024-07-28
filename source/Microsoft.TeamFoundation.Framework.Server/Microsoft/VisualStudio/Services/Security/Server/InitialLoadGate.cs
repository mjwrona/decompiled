// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Server.InitialLoadGate
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Security.Server
{
  public class InitialLoadGate
  {
    private readonly int m_maxLoaderCount;
    private readonly int m_maxWaiterCount;
    private object m_lock;
    private int m_available;
    private int m_waiters;

    public InitialLoadGate(int maxLoaderCount, int maxWaiterCount)
    {
      ArgumentUtility.CheckForOutOfRange(maxLoaderCount, nameof (maxLoaderCount), 1);
      this.m_maxLoaderCount = maxLoaderCount;
      this.m_maxWaiterCount = maxWaiterCount;
      this.m_lock = new object();
      this.m_available = maxLoaderCount;
    }

    public int MaxCount => this.m_maxLoaderCount;

    public int Waiters => this.m_waiters;

    public bool Wait(CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      if (this.m_waiters >= this.m_maxWaiterCount)
        throw new MaxWaiterThreadLimitExceededException(this.m_maxWaiterCount);
      lock (this.m_lock)
      {
        ++this.m_waiters;
        try
        {
          while (this.m_available < 1)
          {
            Monitor.Wait(this.m_lock, 1000);
            token.ThrowIfCancellationRequested();
          }
          --this.m_available;
        }
        finally
        {
          --this.m_waiters;
        }
      }
      return true;
    }

    public void Release()
    {
      lock (this.m_lock)
      {
        ++this.m_available;
        Monitor.Pulse(this.m_lock);
      }
    }
  }
}
