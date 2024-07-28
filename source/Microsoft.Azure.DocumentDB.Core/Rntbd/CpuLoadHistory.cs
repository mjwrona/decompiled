// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.CpuLoadHistory
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class CpuLoadHistory
  {
    private readonly ReadOnlyCollection<CpuLoad> cpuLoad;
    private readonly TimeSpan monitoringInterval;
    private readonly Lazy<bool> cpuOverload;

    public CpuLoadHistory(ReadOnlyCollection<CpuLoad> cpuLoad, TimeSpan monitoringInterval)
    {
      this.cpuLoad = cpuLoad != null ? cpuLoad : throw new ArgumentNullException(nameof (cpuLoad));
      this.monitoringInterval = !(monitoringInterval <= TimeSpan.Zero) ? monitoringInterval : throw new ArgumentOutOfRangeException(nameof (monitoringInterval), (object) monitoringInterval, string.Format("{0} must be strictly positive", (object) nameof (monitoringInterval)));
      this.cpuOverload = new Lazy<bool>(new Func<bool>(this.GetCpuOverload), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public bool IsCpuOverloaded => this.cpuOverload.Value;

    internal DateTime LastTimestamp => this.cpuLoad[this.cpuLoad.Count - 1].Timestamp;

    public override string ToString()
    {
      ReadOnlyCollection<CpuLoad> cpuLoad = this.cpuLoad;
      // ISSUE: explicit non-virtual call
      return (cpuLoad != null ? (__nonvirtual (cpuLoad.Count) == 0 ? 1 : 0) : 0) != 0 ? "empty" : string.Join<CpuLoad>(", ", (IEnumerable<CpuLoad>) this.cpuLoad);
    }

    private bool GetCpuOverload()
    {
      for (int index = 0; index < this.cpuLoad.Count; ++index)
      {
        if ((double) this.cpuLoad[index].Value > 90.0)
          return true;
      }
      for (int index = 0; index < this.cpuLoad.Count - 1; ++index)
      {
        TimeSpan timeSpan = this.cpuLoad[index + 1].Timestamp.Subtract(this.cpuLoad[index].Timestamp);
        double totalMilliseconds = timeSpan.TotalMilliseconds;
        timeSpan = this.monitoringInterval;
        double num = 1.5 * timeSpan.TotalMilliseconds;
        if (totalMilliseconds > num)
          return true;
      }
      return false;
    }
  }
}
