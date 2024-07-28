// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.PreaggregateFiltersManagement.RawPreaggregateFilterQueryArguments
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.PreaggregateFiltersManagement
{
  [JsonObject]
  internal sealed class RawPreaggregateFilterQueryArguments
  {
    [JsonConstructor]
    public RawPreaggregateFilterQueryArguments(
      string monitoringAccount,
      string metricNamespace,
      string metricName,
      IEnumerable<string> preaggregateDimensionNames,
      int count,
      int offset)
    {
      if (string.IsNullOrEmpty(monitoringAccount))
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrEmpty(metricNamespace))
        throw new ArgumentNullException(nameof (metricNamespace));
      if (string.IsNullOrEmpty(metricName))
        throw new ArgumentNullException(nameof (metricName));
      if (preaggregateDimensionNames == null)
        throw new ArgumentNullException(nameof (preaggregateDimensionNames));
      if (count < 0)
        throw new ArgumentException("count cannot be negative number");
      if (offset < 0)
        throw new ArgumentException("offset cannot be negative number");
      this.MonitoringAccount = monitoringAccount;
      this.MetricNamespace = metricNamespace;
      this.MetricName = metricName;
      this.PreaggregateDimensionNames = new SortedSet<string>(preaggregateDimensionNames, (IComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string preaggregateDimensionName in this.PreaggregateDimensionNames)
      {
        if (string.IsNullOrEmpty(preaggregateDimensionName))
          throw new ArgumentException("preaggregateDimensionNames cannot have empty of null values");
      }
      this.Count = count;
      this.Offset = offset;
    }

    public string MonitoringAccount { get; }

    public string MetricNamespace { get; }

    public string MetricName { get; }

    public SortedSet<string> PreaggregateDimensionNames { get; }

    public int Count { get; }

    public int Offset { get; }
  }
}
