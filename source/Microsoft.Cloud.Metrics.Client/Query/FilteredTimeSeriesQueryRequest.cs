// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.FilteredTimeSeriesQueryRequest
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using Microsoft.Online.Metrics.Serialization.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.Query
{
  public sealed class FilteredTimeSeriesQueryRequest
  {
    public FilteredTimeSeriesQueryRequest(
      MetricIdentifier metricIdentifier,
      IReadOnlyList<SamplingType> samplingTypes,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      int seriesResolutionInMinutes,
      AggregationType aggregationType,
      PropertyDefinition topPropertyDefinition,
      int numberOfResultsToReturn,
      OrderBy orderBy,
      bool zeroAsNoValueSentinel,
      IReadOnlyList<string> outputDimensionNames = null,
      bool lastValueMode = false)
      : this(metricIdentifier, (IReadOnlyList<string>) null, (string) null, (string) null, samplingTypes, dimensionFilters, startTimeUtc, endTimeUtc, seriesResolutionInMinutes, aggregationType, topPropertyDefinition, numberOfResultsToReturn, orderBy, zeroAsNoValueSentinel, false, outputDimensionNames, lastValueMode)
    {
    }

    public FilteredTimeSeriesQueryRequest(
      IReadOnlyList<string> monitoringAccountNames,
      string metricNamespace,
      string metricName,
      IReadOnlyList<SamplingType> samplingTypes,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      int seriesResolutionInMinutes,
      AggregationType aggregationType,
      PropertyDefinition topPropertyDefinition,
      int numberOfResultsToReturn,
      OrderBy orderBy,
      bool zeroAsNoValueSentinel,
      bool aggregateAcrossAccounts,
      IReadOnlyList<string> outputDimensionNames = null,
      bool lastValueMode = false)
      : this(new MetricIdentifier(), monitoringAccountNames, metricNamespace, metricName, samplingTypes, dimensionFilters, startTimeUtc, endTimeUtc, seriesResolutionInMinutes, aggregationType, topPropertyDefinition, numberOfResultsToReturn, orderBy, zeroAsNoValueSentinel, aggregateAcrossAccounts, outputDimensionNames, lastValueMode)
    {
      if (monitoringAccountNames == null || monitoringAccountNames.Count == 0)
        throw new ArgumentException("must not be null or empty", nameof (monitoringAccountNames));
      for (int index = 0; index < monitoringAccountNames.Count; ++index)
      {
        if (string.IsNullOrWhiteSpace(monitoringAccountNames[index]))
          throw new ArgumentException("All monitoring accounts must not be null or empty: " + string.Join(",", (IEnumerable<string>) monitoringAccountNames) + ".", nameof (monitoringAccountNames));
      }
      this.MetricIdentifier = new MetricIdentifier(monitoringAccountNames[0], metricNamespace, metricName);
    }

    internal FilteredTimeSeriesQueryRequest(MetricIdentifier metricIdentifier) => this.MetricIdentifier = metricIdentifier;

    [JsonConstructor]
    private FilteredTimeSeriesQueryRequest(
      MetricIdentifier metricIdentifier,
      IReadOnlyList<string> monitoringAccountNames,
      string metricNamespace,
      string metricName,
      IReadOnlyList<SamplingType> samplingTypes,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      int seriesResolutionInMinutes,
      AggregationType aggregationType,
      PropertyDefinition topPropertyDefinition,
      int numberOfResultsToReturn,
      OrderBy orderBy,
      bool zeroAsNoValueSentinel,
      bool aggregateAcrossAccounts,
      IReadOnlyList<string> outputDimensionNames = null,
      bool lastValueMode = false)
    {
      this.MetricIdentifier = metricIdentifier;
      this.MonitoringAccountNames = monitoringAccountNames;
      this.MetricNamespace = metricNamespace;
      this.MetricName = metricName;
      this.SamplingTypes = samplingTypes;
      this.DimensionFilters = dimensionFilters;
      this.StartTimeUtc = startTimeUtc;
      this.EndTimeUtc = endTimeUtc;
      this.SeriesResolutionInMinutes = seriesResolutionInMinutes;
      this.AggregationType = aggregationType;
      this.TopPropertyDefinition = topPropertyDefinition;
      this.NumberOfResultsToReturn = numberOfResultsToReturn;
      this.OrderBy = orderBy;
      this.ZeroAsNoValueSentinel = zeroAsNoValueSentinel;
      this.AggregateAcrossAccounts = aggregateAcrossAccounts;
      this.OutputDimensionNames = outputDimensionNames;
      this.LastValueMode = lastValueMode;
    }

    public IReadOnlyList<string> MonitoringAccountNames { get; }

    public string MetricNamespace { get; }

    public string MetricName { get; }

    public MetricIdentifier MetricIdentifier { get; }

    public IReadOnlyList<SamplingType> SamplingTypes { get; }

    public IReadOnlyList<DimensionFilter> DimensionFilters { get; }

    public DateTime StartTimeUtc { get; }

    public DateTime EndTimeUtc { get; }

    public int SeriesResolutionInMinutes { get; }

    public AggregationType AggregationType { get; }

    public PropertyDefinition TopPropertyDefinition { get; }

    public int NumberOfResultsToReturn { get; }

    public OrderBy OrderBy { get; }

    public bool ZeroAsNoValueSentinel { get; }

    public bool AggregateAcrossAccounts { get; }

    public IReadOnlyList<string> OutputDimensionNames { get; }

    public bool LastValueMode { get; }
  }
}
