// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.JAConcurrencyLock
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class JAConcurrencyLock
  {
    private ConcurrentDictionary<string, SingleConcurrencyLock> m_hostLocks;
    private static readonly JAConcurrencyLock s_instance = new JAConcurrencyLock();

    public static JAConcurrencyLock Instance => JAConcurrencyLock.s_instance;

    private JAConcurrencyLock() => this.m_hostLocks = new ConcurrentDictionary<string, SingleConcurrencyLock>();

    public IMigrationConcurrencyLock GetLock(Guid hostId, string lockName, int maxConcurreny) => (IMigrationConcurrencyLock) new JAConcurrencyLock.HostLevelConcurrencyLock(maxConcurreny, hostId, lockName, this.m_hostLocks);

    private class HostLevelConcurrencyLock : IMigrationConcurrencyLock
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

      public bool Acquire() => this.m_hostLocks.GetOrAdd(this.m_hostLockName, (Func<string, SingleConcurrencyLock>) (_ => new SingleConcurrencyLock(this.m_max))).Acquire();

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
