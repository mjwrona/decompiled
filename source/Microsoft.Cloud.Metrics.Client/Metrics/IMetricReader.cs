// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.IMetricReader
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Query;
using Microsoft.Cloud.Metrics.Client.Query.Kqlm;
using Microsoft.Online.Metrics.Serialization.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  public interface IMetricReader
  {
    Task<TimeSeries<MetricIdentifier, double?>> GetTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      TimeSeriesDefinition<MetricIdentifier> definition);

    Task<TimeSeries<MetricIdentifier, double?>> GetTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      int seriesResolutionInMinutes,
      TimeSeriesDefinition<MetricIdentifier> definition);

    Task<TimeSeries<MetricIdentifier, double?>> GetTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType[] samplingTypes,
      TimeSeriesDefinition<MetricIdentifier> definition,
      int seriesResolutionInMinutes = 1,
      AggregationType aggregationType = AggregationType.Automatic);

    Task<IReadOnlyList<TimeSeries<MetricIdentifier, double?>>> GetMultipleTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      params TimeSeriesDefinition<MetricIdentifier>[] definitions);

    Task<IReadOnlyList<TimeSeries<MetricIdentifier, double?>>> GetMultipleTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      int seriesResolutionInMinutes,
      params TimeSeriesDefinition<MetricIdentifier>[] definitions);

    Task<IReadOnlyList<TimeSeries<MetricIdentifier, double?>>> GetMultipleTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      IEnumerable<TimeSeriesDefinition<MetricIdentifier>> definitions);

    Task<IReadOnlyList<TimeSeries<MetricIdentifier, double?>>> GetMultipleTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      int seriesResolutionInMinutes,
      IEnumerable<TimeSeriesDefinition<MetricIdentifier>> definitions);

    Task<IReadOnlyList<TimeSeries<MetricIdentifier, double?>>> GetMultipleTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType[] samplingTypes,
      IEnumerable<TimeSeriesDefinition<MetricIdentifier>> definitions,
      int seriesResolutionInMinutes = 1,
      AggregationType aggregationType = AggregationType.Automatic,
      IReadOnlyDictionary<string, string> additionalTracingInformation = null);

    Task<IReadOnlyList<TimeSeries<MetricIdentifier, double?>>> GetMultipleTimeSeriesAsync(
      IList<TimeSeriesDefinition<MetricIdentifier>> definitions,
      IReadOnlyDictionary<string, string> additionalTracingInformation = null);

    Task<IReadOnlyList<string>> GetNamespacesAsync(string monitoringAccount);

    Task<IReadOnlyList<string>> GetMetricNamesAsync(
      string monitoringAccount,
      string metricNamespace);

    Task<IReadOnlyList<string>> GetDimensionNamesAsync(MetricIdentifier metricId);

    Task<IReadOnlyList<PreAggregateConfiguration>> GetPreAggregateConfigurationsAsync(
      MetricIdentifier metricId);

    Task<IReadOnlyList<TimeSeriesDefinition<MetricIdentifier>>> GetKnownTimeSeriesDefinitionsAsync(
      MetricIdentifier metricId,
      params DimensionFilter[] dimensionFilters);

    Task<IReadOnlyList<TimeSeriesDefinition<MetricIdentifier>>> GetKnownTimeSeriesDefinitionsAsync(
      MetricIdentifier metricId,
      IEnumerable<DimensionFilter> dimensionFilters);

    Task<IReadOnlyList<string>> GetDimensionValuesAsync(
      MetricIdentifier metricId,
      List<DimensionFilter> dimensionFilters,
      string dimensionName,
      DateTime startTimeUtc,
      DateTime endTimeUtc);

    Task<IReadOnlyList<TimeSeriesDefinition<MetricIdentifier>>> GetKnownTimeSeriesDefinitionsAsync(
      MetricIdentifier metricId,
      IEnumerable<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      bool newCombinationsOnly = false);

    Task<IReadOnlyList<IQueryResult>> GetFilteredDimensionValuesAsync(
      MetricIdentifier metricId,
      IEnumerable<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      Reducer reducer,
      QueryFilter queryFilter,
      bool includeSeries,
      SelectionClause selectionClause = null,
      AggregationType aggregationType = AggregationType.Sum,
      long seriesResolutionInMinutes = 1);

    Task<QueryResultsList> GetFilteredDimensionValuesAsyncV2(
      MetricIdentifier metricId,
      IEnumerable<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      Reducer reducer,
      QueryFilter queryFilter,
      bool includeSeries,
      SelectionClause selectionClause = null,
      AggregationType aggregationType = AggregationType.Sum,
      long seriesResolutionInMinutes = 1);

    Task<IQueryResultListV3> GetFilteredDimensionValuesAsyncV3(
      MetricIdentifier metricId,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      IReadOnlyList<SamplingType> samplingTypes,
      SelectionClauseV3 selectionClause = null,
      AggregationType aggregationType = AggregationType.Automatic,
      long seriesResolutionInMinutes = 1,
      Guid? traceId = null,
      IReadOnlyList<string> outputDimensionNames = null,
      bool lastValueMode = false,
      IReadOnlyDictionary<string, string> additionalTracingInformation = null);

    Task<HttpResponseMessage> GetTimeSeriesStreamedAsync(
      MetricIdentifier metricId,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      IReadOnlyList<SamplingType> samplingTypes,
      SelectionClauseV3 selectionClause = null,
      AggregationType aggregationType = AggregationType.Automatic,
      long seriesResolutionInMinutes = 1,
      Guid? traceId = null,
      IReadOnlyList<string> outputDimensionNames = null);

    Task<IQueryResultListV3> GetTimeSeriesAsync(
      MetricIdentifier metricId,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      IReadOnlyList<SamplingType> samplingTypes,
      SelectionClauseV3 selectionClause = null,
      AggregationType aggregationType = AggregationType.Automatic,
      long seriesResolutionInMinutes = 1,
      Guid? traceId = null,
      IReadOnlyList<string> outputDimensionNames = null,
      bool lastValueMode = false,
      IReadOnlyDictionary<string, string> additionalTracingInformation = null);

    Task<IReadOnlyList<MetricDefinitionV2>> GetMetricDefinitionsAsync(
      string monitoringAccount,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      Guid? traceId = null);

    Task<IReadOnlyList<MetricDefinitionV2>> GetMetricDefinitionsAsync(
      string monitoringAccount,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      string metricNameStartsWithFilter,
      Guid? traceId = null);

    Task<IReadOnlyList<MetricDefinitionV2>> GetMetricDefinitionsAsync(
      string monitoringAccount,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      string metricNameStartsWithFilter,
      int maximumNumberOfMetricsToLook,
      Guid? traceId = null);

    Task<HttpResponseMessage> GetTimeSeriesStreamedAsync(
      string traceId,
      string accountName,
      string queryString,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      bool isDebugQuery = false);

    Task<IKqlmQueryResult> ExecuteKqlmQueryAsync(
      string monitoringAccount,
      string metricNamespace,
      string queryText,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      CancellationToken cancellationToken);

    Task<IKqlmQueryResult> ExecuteKqlmQueryAsync(
      string monitoringAccount,
      string metricNamespace,
      string queryText,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      IReadOnlyDictionary<string, string> queryParameters,
      CancellationToken cancellationToken,
      IReadOnlyDictionary<string, string> additionalTracingInformation = null);
  }
}
