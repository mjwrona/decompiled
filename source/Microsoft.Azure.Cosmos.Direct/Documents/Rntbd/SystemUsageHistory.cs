// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.SystemUsageHistory
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal class SystemUsageHistory
  {
    private readonly TimeSpan monitoringInterval;
    private readonly Lazy<string> loadHistory;
    private readonly Lazy<bool?> cpuHigh;
    private readonly Lazy<bool?> cpuThreadStarvation;

    internal ReadOnlyCollection<SystemUsageLoad> Values { get; }

    public SystemUsageHistory(ReadOnlyCollection<SystemUsageLoad> data, TimeSpan monitoringInterval)
    {
      this.Values = data ?? throw new ArgumentNullException(nameof (data));
      this.LastTimestamp = this.Values.Count <= 0 ? DateTime.MinValue : this.Values.Last<SystemUsageLoad>().Timestamp;
      this.loadHistory = new Lazy<string>((Func<string>) (() =>
      {
        if (this.Values == null || this.Values.Count == 0)
          return "{\"systemHistory\":\"Empty\"}";
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("{\"systemHistory\":[");
        foreach (SystemUsageLoad systemUsageLoad in this.Values)
        {
          systemUsageLoad.AppendJsonString(stringBuilder);
          stringBuilder.Append(",");
        }
        --stringBuilder.Length;
        stringBuilder.Append("]}");
        return stringBuilder.ToString();
      }));
      this.monitoringInterval = !(monitoringInterval <= TimeSpan.Zero) ? monitoringInterval : throw new ArgumentOutOfRangeException(nameof (monitoringInterval), (object) monitoringInterval, string.Format("{0} must be strictly positive", (object) nameof (monitoringInterval)));
      this.cpuHigh = new Lazy<bool?>(new Func<bool?>(this.GetCpuHigh), LazyThreadSafetyMode.ExecutionAndPublication);
      this.cpuThreadStarvation = new Lazy<bool?>(new Func<bool?>(this.GetCpuThreadStarvation), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    internal DateTime LastTimestamp { get; }

    public override string ToString() => this.loadHistory.Value;

    public void AppendJsonString(StringBuilder stringBuilder) => stringBuilder.Append(this.ToString());

    public bool? IsCpuHigh => this.cpuHigh.Value;

    public bool? IsCpuThreadStarvation => this.cpuThreadStarvation.Value;

    private bool? GetCpuHigh()
    {
      if (this.Values.Count == 0)
        return new bool?();
      bool? cpuHigh = new bool?();
      foreach (SystemUsageLoad systemUsageLoad in this.Values)
      {
        if (systemUsageLoad.CpuUsage.HasValue)
        {
          if ((double) systemUsageLoad.CpuUsage.Value > 90.0)
            return new bool?(true);
          cpuHigh = new bool?(false);
        }
      }
      return cpuHigh;
    }

    private bool? GetCpuThreadStarvation()
    {
      if (this.Values.Count == 0)
        return new bool?();
      bool? threadStarvation = new bool?();
      foreach (SystemUsageLoad systemUsageLoad in this.Values)
      {
        if (systemUsageLoad.ThreadInfo != null && systemUsageLoad.ThreadInfo.IsThreadStarving.HasValue)
        {
          if (systemUsageLoad.ThreadInfo.IsThreadStarving.Value)
            return new bool?(true);
          threadStarvation = new bool?(false);
        }
      }
      return threadStarvation;
    }
  }
}
