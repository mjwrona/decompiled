// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.PreaggregateFiltersManagement.PreaggregateFilters
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.PreaggregateFiltersManagement
{
  [JsonObject]
  public sealed class PreaggregateFilters
  {
    public PreaggregateFilters(
      string monitoringAccount,
      string metricNamespace,
      string metricName,
      IEnumerable<string> preaggregateDimensionNames,
      IReadOnlyList<DimensionFilter> filterValues)
    {
      if (string.IsNullOrEmpty(monitoringAccount))
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrEmpty(metricNamespace))
        throw new ArgumentNullException(nameof (metricNamespace));
      if (string.IsNullOrEmpty(metricName))
        throw new ArgumentNullException(nameof (metricName));
      if (preaggregateDimensionNames == null)
        throw new ArgumentNullException(nameof (preaggregateDimensionNames));
      if (filterValues == null)
        throw new ArgumentNullException(nameof (filterValues));
      if (filterValues.Count == 0)
        throw new ArgumentException("filterValues cannot be empty");
      this.MonitoringAccount = monitoringAccount;
      this.MetricNamespace = metricNamespace;
      this.MetricName = metricName;
      SortedSet<string> sortedSet = new SortedSet<string>(preaggregateDimensionNames, (IComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.PreaggregateDimensionNames = sortedSet.Count != 0 ? (IEnumerable<string>) sortedSet : throw new ArgumentException("preaggregateDimensionNames cannot be empty");
      foreach (string preaggregateDimensionName in this.PreaggregateDimensionNames)
      {
        if (string.IsNullOrWhiteSpace(preaggregateDimensionName))
          throw new ArgumentException("preaggregateDimensionNames cannot have empty or null values");
      }
      this.DimensionFilters = filterValues;
      List<PreaggregateDimensionFilterValues> dimensionFilterValuesList = new List<PreaggregateDimensionFilterValues>();
      foreach (DimensionFilter filterValue in (IEnumerable<DimensionFilter>) filterValues)
      {
        if (filterValue.IsExcludeFilter)
          throw new ArgumentException("filterValues are not allowed to have exclude filters. Dimension Name with exclude filters:" + filterValue.DimensionName);
        dimensionFilterValuesList.Add(new PreaggregateDimensionFilterValues(filterValue.DimensionName, filterValue.DimensionValues));
      }
      this.FilterValues = (IReadOnlyList<PreaggregateDimensionFilterValues>) dimensionFilterValuesList;
    }

    [JsonConstructor]
    internal PreaggregateFilters(
      string monitoringAccount,
      string metricNamespace,
      string metricName,
      IEnumerable<string> preaggregateDimensionNames,
      IReadOnlyList<PreaggregateDimensionFilterValues> filterValues)
    {
      this.MonitoringAccount = monitoringAccount;
      this.MetricNamespace = metricNamespace;
      this.MetricName = metricName;
      this.PreaggregateDimensionNames = (IEnumerable<string>) new SortedSet<string>(preaggregateDimensionNames, (IComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.FilterValues = filterValues;
      List<DimensionFilter> dimensionFilterList = new List<DimensionFilter>(this.FilterValues.Count);
      foreach (PreaggregateDimensionFilterValues filterValue in (IEnumerable<PreaggregateDimensionFilterValues>) this.FilterValues)
        dimensionFilterList.Add(DimensionFilter.CreateIncludeFilter(filterValue.FilterDimensionName, (IEnumerable<string>) filterValue.FilterValues));
      this.DimensionFilters = (IReadOnlyList<DimensionFilter>) dimensionFilterList;
    }

    public string MonitoringAccount { get; }

    public string MetricNamespace { get; }

    public string MetricName { get; }

    public IEnumerable<string> PreaggregateDimensionNames { get; }

    [JsonIgnore]
    public IReadOnlyList<DimensionFilter> DimensionFilters { get; }

    [JsonProperty]
    internal IReadOnlyList<PreaggregateDimensionFilterValues> FilterValues { get; }
  }
}
