// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.CpuLoadHistory
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class CpuLoadHistory
  {
    private readonly TimeSpan monitoringInterval;
    private readonly Lazy<bool> cpuOverload;
    private readonly Lazy<string> cpuloadHistory;

    internal ReadOnlyCollection<Microsoft.Azure.Documents.Rntbd.CpuLoad> CpuLoad { get; }

    public CpuLoadHistory(ReadOnlyCollection<Microsoft.Azure.Documents.Rntbd.CpuLoad> cpuLoad, TimeSpan monitoringInterval)
    {
      this.CpuLoad = cpuLoad != null ? cpuLoad : throw new ArgumentNullException(nameof (cpuLoad));
      this.cpuloadHistory = new Lazy<string>((Func<string>) (() =>
      {
        ReadOnlyCollection<Microsoft.Azure.Documents.Rntbd.CpuLoad> cpuLoad1 = this.CpuLoad;
        // ISSUE: explicit non-virtual call
        return (cpuLoad1 != null ? (__nonvirtual (cpuLoad1.Count) == 0 ? 1 : 0) : 0) != 0 ? "empty" : string.Join<Microsoft.Azure.Documents.Rntbd.CpuLoad>(", ", (IEnumerable<Microsoft.Azure.Documents.Rntbd.CpuLoad>) this.CpuLoad);
      }));
      this.monitoringInterval = !(monitoringInterval <= TimeSpan.Zero) ? monitoringInterval : throw new ArgumentOutOfRangeException(nameof (monitoringInterval), (object) monitoringInterval, string.Format("{0} must be strictly positive", (object) nameof (monitoringInterval)));
      this.cpuOverload = new Lazy<bool>(new Func<bool>(this.GetCpuOverload), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    internal DateTime LastTimestamp => this.CpuLoad.Last<Microsoft.Azure.Documents.Rntbd.CpuLoad>().Timestamp;

    public bool IsCpuOverloaded => this.cpuOverload.Value;

    public override string ToString() => this.cpuloadHistory.Value;

    private bool GetCpuOverload()
    {
      for (int index = 0; index < this.CpuLoad.Count; ++index)
      {
        if ((double) this.CpuLoad[index].Value > 90.0)
          return true;
      }
      for (int index = 0; index < this.CpuLoad.Count - 1; ++index)
      {
        TimeSpan timeSpan = this.CpuLoad[index + 1].Timestamp.Subtract(this.CpuLoad[index].Timestamp);
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
