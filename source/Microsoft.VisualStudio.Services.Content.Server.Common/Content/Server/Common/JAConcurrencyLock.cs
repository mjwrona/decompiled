// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.JAConcurrencyLock
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class JAConcurrencyLock
  {
    private ConcurrentDictionary<string, SingleConcurrencyLock> m_hostLocks;
    private static readonly JAConcurrencyLock s_instance = new JAConcurrencyLock();

    public static JAConcurrencyLock Instance => JAConcurrencyLock.s_instance;

    private JAConcurrencyLock() => this.m_hostLocks = new ConcurrentDictionary<string, SingleConcurrencyLock>();

    public IConcurrencyLock GetLock(IVssRequestContext context, string lockName, int maxConcurreny) => (IConcurrencyLock) new JAConcurrencyLock.HostLevelConcurrencyLock(maxConcurreny, context.ServiceHost.InstanceId, lockName, this.m_hostLocks);

    private class HostLevelConcurrencyLock : IConcurrencyLock
    {
      private int m_max;
      private string m_hostLockName;
      private ConcurrentDictionary<string, SingleConcurrencyLock> m_hostLocks;

      internal HostLevelConcurrencyLock(
        int max,
        Guid hostId,
        string lockName,
        ConcurrentDictionary<string, SingleConcurrencyLock> hostLocks)
      {
        this.m_max = max;
        this.m_hostLockName = hostId.ToString("n") + "_" + lockName;
        this.m_hostLocks = hostLocks;
      }

      public bool Require() => this.m_hostLocks.GetOrAdd(this.m_hostLockName, (Func<string, SingleConcurrencyLock>) (_ => new SingleConcurrencyLock(this.m_max))).Require();

      public void Release()
      {
        SingleConcurrencyLock singleConcurrencyLock;
        if (!this.m_hostLocks.TryGetValue(this.m_hostLockName, out singleConcurrencyLock))
          return;
        singleConcurrencyLock.Release();
      }
    }
  }
}
