// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.MetricReader
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Query;
using Microsoft.Cloud.Metrics.Client.Query.Kqlm;
using Microsoft.Cloud.Metrics.Client.Utility;
using Microsoft.Online.Metrics.Serialization.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  public sealed class MetricReader : IMetricReader, IExportValidator
  {
    public const string ContextualHintingWildcardValue = "{{*}}";
    public readonly string DataRelativeUrl;
    public readonly string MetaDataRelativeUrl;
    public readonly string MetaDataRelativeUrlV2;
    public readonly string DistributedQueryRelativeUrl;
    public readonly string QueryServiceRelativeUrl;
    internal const string InvalidStartsWithFilterExceptionMessage = "'StartsWith' filter can only be provided with atleast one 'EqualsTo' filter";
    private const int MillisecondsPerMinute = 60000;
    private const int MaxAdditionalTracingFieldsCount = 100;
    private static readonly string[] EmptyStringArray = new string[0];
    private static readonly List<PreAggregateConfiguration> EmptyPreAggregateConfigurations = new List<PreAggregateConfiguration>();
    private readonly HttpClient httpClient;
    private readonly ConnectionInfo connectionInfo;
    private readonly MetricConfigurationManager metricConfigurationManager;
    private readonly string clientId;

    public MetricReader(ConnectionInfo connectionInfo, string clientId = "ClientAPI")
      : this(connectionInfo, HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo), clientId)
    {
    }

    public MetricReader(ConnectionInfo connectionInfo, string authHeaderValue, string clientId = "ClientAPI")
      : this(connectionInfo, HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo, authHeaderValue), clientId)
    {
    }

    internal MetricReader(ConnectionInfo connectionInfo, HttpClient httpClient, string clientId = "ClientAPI")
    {
      this.connectionInfo = connectionInfo != null ? connectionInfo : throw new ArgumentNullException(nameof (connectionInfo));
      this.DataRelativeUrl = this.connectionInfo.GetAuthRelativeUrl("v1/data/metrics");
      this.MetaDataRelativeUrl = this.connectionInfo.GetAuthRelativeUrl("v1/hint");
      this.MetaDataRelativeUrlV2 = this.connectionInfo.GetAuthRelativeUrl("v2/hint");
      this.DistributedQueryRelativeUrl = this.connectionInfo.GetAuthRelativeUrl("flight/dq/batchedReadv3");
      this.QueryServiceRelativeUrl = this.connectionInfo.GetAuthRelativeUrl("query");
      this.clientId = clientId;
      this.httpClient = httpClient;
      this.metricConfigurationManager = new MetricConfigurationManager(this.connectionInfo, httpClient);
    }

    public TimeSpan RequestTimeout
    {
      get => this.httpClient.Timeout;
      set => this.httpClient.Timeout = value;
    }

    public async Task<TimeSeries<MetricIdentifier, double?>> GetTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      TimeSeriesDefinition<MetricIdentifier> definition)
    {
      MetricReader metricReader = this;
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      return (await metricReader.GetMultipleTimeSeriesAsync(startTimeUtc, endTimeUtc, samplingType, new TimeSeriesDefinition<MetricIdentifier>[1]
      {
        definition
      }).ConfigureAwait(false)).First<TimeSeries<MetricIdentifier, double?>>();
    }

    public Task<IReadOnlyList<TimeSeries<MetricIdentifier, double?>>> GetMultipleTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      params TimeSeriesDefinition<MetricIdentifier>[] definitions)
    {
      return this.GetMultipleTimeSeriesAsync(startTimeUtc, endTimeUtc, samplingType, ((IEnumerable<TimeSeriesDefinition<MetricIdentifier>>) definitions).AsEnumerable<TimeSeriesDefinition<MetricIdentifier>>());
    }

    public Task<IReadOnlyList<TimeSeries<MetricIdentifier, double?>>> GetMultipleTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      IEnumerable<TimeSeriesDefinition<MetricIdentifier>> definitions)
    {
      return this.GetMultipleTimeSeriesAsync(startTimeUtc, endTimeUtc, new SamplingType[1]
      {
        samplingType
      }, definitions, 1, AggregationType.Automatic, (IReadOnlyDictionary<string, string>) null);
    }

    public async Task<TimeSeries<MetricIdentifier, double?>> GetTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      int seriesResolutionInMinutes,
      TimeSeriesDefinition<MetricIdentifier> definition)
    {
      MetricReader metricReader = this;
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      return (await metricReader.GetMultipleTimeSeriesAsync(startTimeUtc, endTimeUtc, samplingType, seriesResolutionInMinutes, new TimeSeriesDefinition<MetricIdentifier>[1]
      {
        definition
      }).ConfigureAwait(false)).First<TimeSeries<MetricIdentifier, double?>>();
    }

    public Task<IReadOnlyList<TimeSeries<MetricIdentifier, double?>>> GetMultipleTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      int seriesResolutionInMinutes,
      params TimeSeriesDefinition<MetricIdentifier>[] definitions)
    {
      return this.GetMultipleTimeSeriesAsync(startTimeUtc, endTimeUtc, samplingType, seriesResolutionInMinutes, ((IEnumerable<TimeSeriesDefinition<MetricIdentifier>>) definitions).AsEnumerable<TimeSeriesDefinition<MetricIdentifier>>());
    }

    public Task<IReadOnlyList<TimeSeries<MetricIdentifier, double?>>> GetMultipleTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      int seriesResolutionInMinutes,
      IEnumerable<TimeSeriesDefinition<MetricIdentifier>> definitions)
    {
      return this.GetMultipleTimeSeriesAsync(startTimeUtc, endTimeUtc, new SamplingType[1]
      {
        samplingType
      }, definitions, seriesResolutionInMinutes, AggregationType.Automatic, (IReadOnlyDictionary<string, string>) null);
    }

    public async Task<TimeSeries<MetricIdentifier, double?>> GetTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType[] samplingTypes,
      TimeSeriesDefinition<MetricIdentifier> definition,
      int seriesResolutionInMinutes = 1,
      AggregationType aggregationType = AggregationType.Automatic)
    {
      MetricReader metricReader = this;
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      return (await metricReader.GetMultipleTimeSeriesAsync(startTimeUtc, endTimeUtc, samplingTypes, (IEnumerable<TimeSeriesDefinition<MetricIdentifier>>) new TimeSeriesDefinition<MetricIdentifier>[1]
      {
        definition
      }, seriesResolutionInMinutes, aggregationType, (IReadOnlyDictionary<string, string>) null).ConfigureAwait(false)).FirstOrDefault<TimeSeries<MetricIdentifier, double?>>();
    }

    public async Task<IReadOnlyList<TimeSeries<MetricIdentifier, double?>>> GetMultipleTimeSeriesAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType[] samplingTypes,
      IEnumerable<TimeSeriesDefinition<MetricIdentifier>> definitions,
      int seriesResolutionInMinutes = 1,
      AggregationType aggregationType = AggregationType.Automatic,
      IReadOnlyDictionary<string, string> additionalTracingInformation = null)
    {
      if (definitions == null)
        throw new ArgumentNullException(nameof (definitions));
      if (samplingTypes == null || samplingTypes.Length == 0)
        throw new ArgumentException("cannot be null or empty", nameof (samplingTypes));
      if (seriesResolutionInMinutes < 1)
        throw new ArgumentException(string.Format("{0} must be >= {1}", (object) seriesResolutionInMinutes, (object) 1), nameof (samplingTypes));
      List<TimeSeriesDefinition<MetricIdentifier>> list = definitions.ToList<TimeSeriesDefinition<MetricIdentifier>>();
      if (list.Count == 0)
        throw new ArgumentException("The count of 'definitions' is 0.");
      if (list.Any<TimeSeriesDefinition<MetricIdentifier>>((Func<TimeSeriesDefinition<MetricIdentifier>, bool>) (d => d == null)))
        throw new ArgumentException("At least one of definitions are null.");
      if (startTimeUtc > endTimeUtc)
        throw new ArgumentException(string.Format("startTimeUtc [{0}] must be <= endTimeUtc [{1}]", (object) startTimeUtc, (object) endTimeUtc));
      MetricReader.NormalizeTimeRange(ref startTimeUtc, ref endTimeUtc);
      foreach (TimeSeriesDefinition<MetricIdentifier> seriesDefinition in list)
      {
        seriesDefinition.SamplingTypes = samplingTypes;
        seriesDefinition.StartTimeUtc = startTimeUtc;
        seriesDefinition.EndTimeUtc = endTimeUtc;
        seriesDefinition.SeriesResolutionInMinutes = seriesResolutionInMinutes;
        seriesDefinition.AggregationType = aggregationType;
      }
      return await this.GetMultipleTimeSeriesAsync((IList<TimeSeriesDefinition<MetricIdentifier>>) list).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TimeSeries<MetricIdentifier, double?>>> GetMultipleTimeSeriesAsync(
      IList<TimeSeriesDefinition<MetricIdentifier>> definitions,
      IReadOnlyDictionary<string, string> additionalTracingInformation = null)
    {
      IList<TimeSeriesDefinition<MetricIdentifier>> definitions1 = definitions;
      IReadOnlyDictionary<string, string> readOnlyDictionary = additionalTracingInformation;
      Guid? traceId = new Guid?();
      IReadOnlyDictionary<string, string> additionalTracingInformation1 = readOnlyDictionary;
      IReadOnlyList<TimeSeries<MetricIdentifier, double?>> multipleTimeSeriesAsync;
      using (HttpResponseMessage response = await this.GetMultipleTimeSeriesAsync((IEnumerable<TimeSeriesDefinition<MetricIdentifier>>) definitions1, traceId: traceId, additionalTracingInformation: additionalTracingInformation1).ConfigureAwait(false))
      {
        using (Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
          multipleTimeSeriesAsync = (IReadOnlyList<TimeSeries<MetricIdentifier, double?>>) MetricQueryResponseDeserializer.Deserialize(stream, (IReadOnlyList<TimeSeriesDefinition<MetricIdentifier>>) definitions.ToArray<TimeSeriesDefinition<MetricIdentifier>>()).Item2;
      }
      return multipleTimeSeriesAsync;
    }

    public async Task<IReadOnlyList<string>> GetNamespacesAsync(string monitoringAccount)
    {
      if (string.IsNullOrWhiteSpace(monitoringAccount))
        throw new ArgumentException("monitoringAccount is null or empty.");
      return (IReadOnlyList<string>) JsonConvert.DeserializeObject<string[]>(await HttpClientHelper.GetJsonResponse(new Uri(string.Format("{0}{1}/monitoringAccount/{2}/metricNamespace", (object) await this.connectionInfo.GetEndpointAsync(monitoringAccount).ConfigureAwait(false), (object) this.MetaDataRelativeUrl, (object) monitoringAccount)), HttpMethod.Get, this.httpClient, monitoringAccount, this.MetaDataRelativeUrl, clientId: this.clientId).ConfigureAwait(false));
    }

    public async Task<IReadOnlyList<string>> GetMetricNamesAsync(
      string monitoringAccount,
      string metricNamespace)
    {
      if (string.IsNullOrWhiteSpace(monitoringAccount))
        throw new ArgumentException("monitoringAccount is null or empty.");
      if (string.IsNullOrWhiteSpace(metricNamespace))
        throw new ArgumentException("metricNamespace is null or empty.");
      return (IReadOnlyList<string>) JsonConvert.DeserializeObject<string[]>(await HttpClientHelper.GetJsonResponse(new Uri(string.Format("{0}{1}/monitoringAccount/{2}/metricNamespace/{3}/metric", (object) await this.connectionInfo.GetEndpointAsync(monitoringAccount).ConfigureAwait(false), (object) this.MetaDataRelativeUrl, (object) monitoringAccount, (object) SpecialCharsHelper.EscapeTwice(metricNamespace))), HttpMethod.Get, this.httpClient, monitoringAccount, this.MetaDataRelativeUrl, clientId: this.clientId).ConfigureAwait(false));
    }

    public async Task<IReadOnlyList<string>> GetDimensionNamesAsync(MetricIdentifier metricId)
    {
      MetricConfigurationV2 metricConfigurationV2 = await this.metricConfigurationManager.Get(metricId).ConfigureAwait(false);
      return ((MetricConfigurationV2) ref metricConfigurationV2).DimensionConfigurations == null ? (IReadOnlyList<string>) MetricReader.EmptyStringArray : (IReadOnlyList<string>) ((MetricConfigurationV2) ref metricConfigurationV2).DimensionConfigurations.Select<DimensionConfiguration, string>((Func<DimensionConfiguration, string>) (d => d.Id)).ToArray<string>();
    }

    public async Task<IReadOnlyList<PreAggregateConfiguration>> GetPreAggregateConfigurationsAsync(
      MetricIdentifier metricId)
    {
      MetricConfigurationV2 metricConfigurationV2 = await this.metricConfigurationManager.Get(metricId).ConfigureAwait(false);
      return (IReadOnlyList<PreAggregateConfiguration>) (((MetricConfigurationV2) ref metricConfigurationV2).PreAggregations ?? MetricReader.EmptyPreAggregateConfigurations);
    }

    public Task<IReadOnlyList<TimeSeriesDefinition<MetricIdentifier>>> GetKnownTimeSeriesDefinitionsAsync(
      MetricIdentifier metricId,
      params DimensionFilter[] dimensionFilters)
    {
      return this.GetKnownTimeSeriesDefinitionsAsync(metricId, ((IEnumerable<DimensionFilter>) dimensionFilters).AsEnumerable<DimensionFilter>());
    }

    public async Task<IReadOnlyList<TimeSeriesDefinition<MetricIdentifier>>> GetKnownTimeSeriesDefinitionsAsync(
      MetricIdentifier metricId,
      IEnumerable<DimensionFilter> dimensionFilters)
    {
      return await this.GetKnownTimeSeriesDefinitionsAsync(metricId, dimensionFilters, DateTime.MinValue, DateTime.MaxValue).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TimeSeriesDefinition<MetricIdentifier>>> GetKnownTimeSeriesDefinitionsAsync(
      MetricIdentifier metricId,
      IEnumerable<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      bool newCombinationsOnly = false)
    {
      ((MetricIdentifier) ref metricId).Validate();
      SortedDictionary<string, Tuple<bool, IReadOnlyList<string>>> dimensionNamesAndConstraints = MetricReader.GetDimensionNamesAndConstraints(dimensionFilters);
      string uriString = string.Format("{0}{1}/monitoringAccount/{2}/metricNamespace/{3}/metricName/{4}/startTimeUtcMillis/{5}/endTimeUtcMillis/{6}", (object) await this.connectionInfo.GetEndpointAsync(((MetricIdentifier) ref metricId).MonitoringAccount).ConfigureAwait(false), (object) this.MetaDataRelativeUrl, (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricId).MonitoringAccount), (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricId).MetricNamespace), (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricId).MetricName), (object) UnixEpochHelper.GetMillis(startTimeUtc), (object) UnixEpochHelper.GetMillis(endTimeUtc));
      if (newCombinationsOnly)
        uriString = string.Format("{0}/newOnly", (object) uriString);
      Tuple<List<string>, List<List<string>>> tuple = JsonConvert.DeserializeObject<Tuple<List<string>, List<List<string>>>>(await HttpClientHelper.GetJsonResponse(new Uri(uriString), HttpMethod.Post, this.httpClient, ((MetricIdentifier) ref metricId).MonitoringAccount, this.MetaDataRelativeUrl, (object) dimensionNamesAndConstraints, this.clientId).ConfigureAwait(false));
      TimeSeriesDefinition<MetricIdentifier>[] seriesDefinitionArray;
      if (tuple != null && tuple.Item1 != null && tuple.Item2 != null)
      {
        seriesDefinitionArray = new TimeSeriesDefinition<MetricIdentifier>[tuple.Item2.Count];
        List<string> stringList1 = tuple.Item1;
        for (int index1 = 0; index1 < tuple.Item2.Count; ++index1)
        {
          List<string> stringList2 = tuple.Item2[index1];
          KeyValuePair<string, string>[] keyValuePairArray = new KeyValuePair<string, string>[stringList2.Count];
          for (int index2 = 0; index2 < stringList1.Count; ++index2)
            keyValuePairArray[index2] = new KeyValuePair<string, string>(stringList1[index2], stringList2[index2]);
          seriesDefinitionArray[index1] = new TimeSeriesDefinition<MetricIdentifier>(metricId, keyValuePairArray);
        }
      }
      else
        seriesDefinitionArray = new TimeSeriesDefinition<MetricIdentifier>[0];
      IReadOnlyList<TimeSeriesDefinition<MetricIdentifier>> definitionsAsync = (IReadOnlyList<TimeSeriesDefinition<MetricIdentifier>>) seriesDefinitionArray;
      dimensionNamesAndConstraints = (SortedDictionary<string, Tuple<bool, IReadOnlyList<string>>>) null;
      return definitionsAsync;
    }

    public async Task<IReadOnlyList<string>> GetDimensionValuesAsync(
      MetricIdentifier metricId,
      List<DimensionFilter> dimensionFilters,
      string dimensionName,
      DateTime startTimeUtc,
      DateTime endTimeUtc)
    {
      ((MetricIdentifier) ref metricId).Validate();
      if (dimensionFilters == null || dimensionFilters.Count == 0)
        throw new ArgumentException("Dimension filters cannot be empty or null");
      MetricReader.ThrowIfInvalidDimensionFilters((IReadOnlyList<DimensionFilter>) dimensionFilters);
      dimensionFilters.Sort((Comparison<DimensionFilter>) ((item1, item2) => string.Compare(item1.DimensionName, item2.DimensionName, StringComparison.OrdinalIgnoreCase)));
      IReadOnlyList<string> dimensionValuesAsync;
      using (MemoryStream ms = new MemoryStream())
      {
        Stream stream = await (await HttpClientHelper.GetResponse(new Uri(string.Format("{0}{1}/monitoringAccount/{2}/metricNamespace/{3}/metric/{4}/startTimeUtcMillis/{5}/endTimeUtcMillis/{6}/dimension/{7}", (object) await this.connectionInfo.GetEndpointAsync(((MetricIdentifier) ref metricId).MonitoringAccount).ConfigureAwait(false), (object) this.MetaDataRelativeUrlV2, (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricId).MonitoringAccount), (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricId).MetricNamespace), (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricId).MetricName), (object) UnixEpochHelper.GetMillis(startTimeUtc), (object) UnixEpochHelper.GetMillis(endTimeUtc), (object) SpecialCharsHelper.EscapeTwice(dimensionName))), HttpMethod.Post, this.httpClient, ((MetricIdentifier) ref metricId).MonitoringAccount, this.MetaDataRelativeUrlV2, false, false, clientId: this.clientId, serializedContent: JsonConvert.SerializeObject((object) dimensionFilters)).ConfigureAwait(false)).Item2.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using (stream)
          await stream.CopyToAsync((Stream) ms).ConfigureAwait(false);
        ms.Position = 0L;
        using (StreamReader reader = new StreamReader((Stream) ms, Encoding.UTF8))
          dimensionValuesAsync = (IReadOnlyList<string>) JsonSerializer.Create().Deserialize((TextReader) reader, typeof (List<string>));
      }
      return dimensionValuesAsync;
    }

    public async Task<IReadOnlyList<IQueryResult>> GetFilteredDimensionValuesAsync(
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
      long seriesResolutionInMinutes = 1)
    {
      SortedDictionary<string, Tuple<bool, IReadOnlyList<string>>> dimensionNamesAndConstraints;
      NameValueCollection query = this.BuildQueryParameters(metricId, dimensionFilters, startTimeUtc, endTimeUtc, samplingType, reducer, queryFilter, includeSeries, selectionClause, aggregationType, seriesResolutionInMinutes, out dimensionNamesAndConstraints);
      string path = string.Format("{0}/monitoringAccount/{1}/metricNamespace/{2}/metric/{3}", (object) this.DistributedQueryRelativeUrl, (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricId).MonitoringAccount), (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricId).MetricNamespace), (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricId).MetricName));
      IReadOnlyList<IQueryResult> dimensionValuesAsync = (IReadOnlyList<IQueryResult>) JsonConvert.DeserializeObject<QueryResult[]>(await HttpClientHelper.GetJsonResponse(new UriBuilder(await this.connectionInfo.GetEndpointAsync(((MetricIdentifier) ref metricId).MonitoringAccount).ConfigureAwait(false))
      {
        Path = path,
        Query = query.ToString()
      }.Uri, HttpMethod.Post, this.httpClient, ((MetricIdentifier) ref metricId).MonitoringAccount, this.DistributedQueryRelativeUrl, (object) dimensionNamesAndConstraints, this.clientId).ConfigureAwait(false));
      dimensionNamesAndConstraints = (SortedDictionary<string, Tuple<bool, IReadOnlyList<string>>>) null;
      query = (NameValueCollection) null;
      path = (string) null;
      return dimensionValuesAsync;
    }

    public async Task<QueryResultsList> GetFilteredDimensionValuesAsyncV2(
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
      long seriesResolutionInMinutes = 1)
    {
      SortedDictionary<string, Tuple<bool, IReadOnlyList<string>>> dimensionNamesAndConstraints;
      NameValueCollection query = this.BuildQueryParameters(metricId, dimensionFilters, startTimeUtc, endTimeUtc, samplingType, reducer, queryFilter, includeSeries, selectionClause, aggregationType, seriesResolutionInMinutes, out dimensionNamesAndConstraints);
      string operation = this.DistributedQueryRelativeUrl + "/V2";
      string path = string.Format("{0}/monitoringAccount/{1}/metricNamespace/{2}/metric/{3}", (object) operation, (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricId).MonitoringAccount), (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricId).MetricNamespace), (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricId).MetricName));
      QueryResultsList dimensionValuesAsyncV2 = JsonConvert.DeserializeObject<QueryResultsList>(await HttpClientHelper.GetJsonResponse(new UriBuilder(await this.connectionInfo.GetEndpointAsync(((MetricIdentifier) ref metricId).MonitoringAccount).ConfigureAwait(false))
      {
        Path = path,
        Query = query.ToString()
      }.Uri, HttpMethod.Post, this.httpClient, ((MetricIdentifier) ref metricId).MonitoringAccount, operation, (object) dimensionNamesAndConstraints, this.clientId).ConfigureAwait(false));
      dimensionNamesAndConstraints = (SortedDictionary<string, Tuple<bool, IReadOnlyList<string>>>) null;
      query = (NameValueCollection) null;
      operation = (string) null;
      path = (string) null;
      return dimensionValuesAsyncV2;
    }

    public async Task<IQueryResultListV3> GetFilteredDimensionValuesAsyncV3(
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
      IReadOnlyDictionary<string, string> additionalTracingInformation = null)
    {
      if (samplingTypes == null || samplingTypes.Count == 0)
        throw new ArgumentException("One or more sampling types must be specified.");
      for (int index = 0; index < samplingTypes.Count; ++index)
      {
        if (samplingTypes[index].Name == null)
          throw new ArgumentException("Sampling type name can not be null.");
      }
      if (selectionClause == null)
        selectionClause = new SelectionClauseV3(new PropertyDefinition(PropertyAggregationType.Average, samplingTypes[0]), int.MaxValue, OrderBy.Undefined);
      if (dimensionFilters == null)
        throw new ArgumentNullException(nameof (dimensionFilters));
      if (startTimeUtc > endTimeUtc)
        throw new ArgumentException("Start time must be before end time.");
      MetricReader.ThrowIfInvalidDimensionFilters(dimensionFilters);
      Guid? nullable = traceId;
      traceId = new Guid?(nullable ?? Guid.NewGuid());
      FilteredTimeSeriesQueryRequest request = new FilteredTimeSeriesQueryRequest(metricId, samplingTypes, dimensionFilters, startTimeUtc, endTimeUtc, (int) seriesResolutionInMinutes, aggregationType, selectionClause.PropertyDefinition, selectionClause.NumberOfResultsToReturn, selectionClause.OrderBy, false, outputDimensionNames, lastValueMode);
      ConnectionInfo connectionInfo = this.connectionInfo;
      MetricIdentifier metricIdentifier = request.MetricIdentifier;
      string monitoringAccount = ((MetricIdentifier) ref metricIdentifier).MonitoringAccount;
      Uri url = new Uri(string.Format("{0}{1}/v1/multiple/serializationVersion/{2}/maxCost/{3}?timeoutInSeconds={4}&returnRequestObjectOnFailure={5}", (object) (await connectionInfo.GetMetricsDataQueryEndpoint(monitoringAccount).ConfigureAwait(false)).OriginalString, (object) this.QueryServiceRelativeUrl, (object) (byte) 3, (object) int.MaxValue, (object) (int) this.connectionInfo.Timeout.TotalSeconds, (object) false));
      HttpMethod post = HttpMethod.Post;
      HttpClient httpClient = this.httpClient;
      IReadOnlyDictionary<string, string> tracingInformation = additionalTracingInformation;
      nullable = new Guid?();
      Guid? traceId1 = nullable;
      string traceId2 = MetricReader.BuildTraceId(tracingInformation, traceId1);
      FilteredTimeSeriesQueryRequest[] httpContent = new FilteredTimeSeriesQueryRequest[1]
      {
        request
      };
      string clientId = this.clientId;
      Tuple<string, HttpResponseMessage> tuple = await HttpClientHelper.GetResponseWithCustomTraceId(url, post, httpClient, (string) null, (string) null, traceId2, (object) httpContent, clientId).ConfigureAwait(false);
      IQueryResultListV3 dimensionValuesAsyncV3;
      using (HttpResponseMessage httpResponseMessage = tuple.Item2)
      {
        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
          throw new MetricsClientException(string.Format("Request failed with HTTP Status Code: {0}. TraceId: {1}.  Response: {2}", (object) httpResponseMessage.StatusCode, (object) traceId, (object) tuple.Item1));
        IReadOnlyList<IFilteredTimeSeriesQueryResponse> seriesQueryResponseList;
        using (Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
          seriesQueryResponseList = FilteredQueryResponseDeserializer.Deserialize(stream);
        IFilteredTimeSeriesQueryResponse seriesQueryResponse = seriesQueryResponseList != null && seriesQueryResponseList.Count != 0 ? seriesQueryResponseList[0] : throw new MetricsClientException(string.Format("Response is null or empty.  TraceId: {0}", (object) traceId));
        if (seriesQueryResponse.ErrorCode != FilteredTimeSeriesQueryResponseErrorCode.Success)
          throw new MetricsClientException(string.Format("Error occured processing the request.  Error code: {0}. {1}", (object) seriesQueryResponse.ErrorCode, (object) seriesQueryResponse.DiagnosticInfo), (Exception) null, traceId.Value, new HttpStatusCode?(httpResponseMessage.StatusCode));
        dimensionValuesAsyncV3 = (IQueryResultListV3) new QueryResultListV3(seriesQueryResponse.StartTimeUtc, seriesQueryResponse.EndTimeUtc, seriesQueryResponse.TimeResolutionInMinutes, (IReadOnlyList<IQueryResultV3>) seriesQueryResponse.FilteredTimeSeriesList);
      }
      request = (FilteredTimeSeriesQueryRequest) null;
      return dimensionValuesAsyncV3;
    }

    public async Task<HttpResponseMessage> GetTimeSeriesStreamedAsync(
      MetricIdentifier metricId,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      IReadOnlyList<SamplingType> samplingTypes,
      SelectionClauseV3 selectionClause = null,
      AggregationType aggregationType = AggregationType.Automatic,
      long seriesResolutionInMinutes = 1,
      Guid? traceId = null,
      IReadOnlyList<string> outputDimensionNames = null)
    {
      if (samplingTypes == null || samplingTypes.Count == 0)
        throw new ArgumentException("One or more sampling types must be specified.");
      if (selectionClause == null)
        selectionClause = new SelectionClauseV3(new PropertyDefinition(PropertyAggregationType.Average, samplingTypes[0]), int.MaxValue, OrderBy.Undefined);
      if (dimensionFilters == null)
        throw new ArgumentNullException(nameof (dimensionFilters));
      if (startTimeUtc > endTimeUtc)
        throw new ArgumentException("Start time must be before end time.");
      MetricReader.ThrowIfInvalidDimensionFilters(dimensionFilters);
      traceId = new Guid?(traceId ?? Guid.NewGuid());
      FilteredTimeSeriesQueryRequest request = new FilteredTimeSeriesQueryRequest(metricId, samplingTypes, dimensionFilters, startTimeUtc, endTimeUtc, (int) seriesResolutionInMinutes, aggregationType, selectionClause.PropertyDefinition, selectionClause.NumberOfResultsToReturn, selectionClause.OrderBy, false, outputDimensionNames);
      ConnectionInfo connectionInfo = this.connectionInfo;
      MetricIdentifier metricIdentifier = request.MetricIdentifier;
      string monitoringAccount = ((MetricIdentifier) ref metricIdentifier).MonitoringAccount;
      Tuple<string, HttpResponseMessage> tuple = await HttpClientHelper.GetResponse(new Uri(string.Format("{0}{1}/v1/multiple/serializationVersion/{2}/maxCost/{3}?timeoutInSeconds={4}&returnRequestObjectOnFailure={5}", (object) (await connectionInfo.GetMetricsDataQueryEndpoint(monitoringAccount).ConfigureAwait(false)).OriginalString, (object) this.QueryServiceRelativeUrl, (object) (byte) 3, (object) int.MaxValue, (object) (int) this.connectionInfo.Timeout.TotalSeconds, (object) false)), HttpMethod.Post, this.httpClient, (string) null, (string) null, (object) new FilteredTimeSeriesQueryRequest[1]
      {
        request
      }, this.clientId, traceId: traceId).ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = tuple.Item2;
      if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
      {
        httpResponseMessage.Dispose();
        throw new MetricsClientException(string.Format("Request failed with HTTP Status Code: {0}. TraceId: {1}.  Response: {2}", (object) httpResponseMessage.StatusCode, (object) traceId, (object) tuple.Item1));
      }
      HttpResponseMessage seriesStreamedAsync = httpResponseMessage;
      request = (FilteredTimeSeriesQueryRequest) null;
      return seriesStreamedAsync;
    }

    public async Task<HttpResponseMessage> GetTimeSeriesStreamedAsync(
      string traceId,
      string accountName,
      string queryString,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      bool isDebugQuery = false)
    {
      KqlMRequest queryLanguageRequest = new KqlMRequest(accountName, "[N/A]", "[N/A]", startTimeUtc, endTimeUtc, queryString);
      string uriString = string.Format("{0}{1}query/v2/language/monitoringAccount/{2}", (object) (await this.connectionInfo.GetEndpointAsync(accountName).ConfigureAwait(false)).OriginalString, (object) this.connectionInfo.GetAuthRelativeUrl(string.Empty), (object) accountName);
      Guid? nullable1 = new Guid?();
      Guid result;
      if (Guid.TryParse(traceId, out result))
        nullable1 = new Guid?(result);
      Uri url = new Uri(uriString);
      HttpMethod post = HttpMethod.Post;
      HttpClient httpClient = this.httpClient;
      string monitoringAccount = accountName;
      object obj = (object) queryLanguageRequest;
      string clientId1 = this.clientId;
      Guid? nullable2 = nullable1;
      int num = isDebugQuery ? 1 : 0;
      object httpContent = obj;
      string clientId2 = clientId1;
      Guid? traceId1 = nullable2;
      Tuple<string, HttpResponseMessage> tuple = await HttpClientHelper.GetResponse(url, post, httpClient, monitoringAccount, "GetTimeSeriesStreamedAsyncForKQL-M", false, num != 0, httpContent, clientId2, traceId: traceId1).ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = tuple.Item2;
      if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
      {
        httpResponseMessage.Dispose();
        throw new MetricsClientException(string.Format("Request failed with HTTP Status Code: {0}. TraceId: {1}.  Response: {2}", (object) httpResponseMessage.StatusCode, (object) traceId, (object) tuple.Item1));
      }
      HttpResponseMessage seriesStreamedAsync = httpResponseMessage;
      queryLanguageRequest = (KqlMRequest) null;
      return seriesStreamedAsync;
    }

    public async Task<JArray> ExecuteKqlMQueryAsync(
      string traceId,
      string accountName,
      string queryString,
      DateTime startTimeUtc,
      DateTime endTimeUtc)
    {
      KqlMRequest queryLanguageRequest = new KqlMRequest(accountName, "[N/A]", "[N/A]", startTimeUtc, endTimeUtc, queryString);
      string uriString = string.Format("{0}{1}query/v2/language/monitoringAccount/{2}", (object) (await this.connectionInfo.GetEndpointAsync(accountName).ConfigureAwait(false)).OriginalString, (object) this.connectionInfo.GetAuthRelativeUrl(string.Empty), (object) accountName);
      Guid? traceId1 = new Guid?();
      Guid result;
      if (Guid.TryParse(traceId, out result))
        traceId1 = new Guid?(result);
      Tuple<string, HttpResponseMessage> tuple = await HttpClientHelper.GetResponse(new Uri(uriString), HttpMethod.Post, this.httpClient, accountName, nameof (ExecuteKqlMQueryAsync), (object) queryLanguageRequest, this.clientId, traceId: traceId1).ConfigureAwait(false);
      JArray responseAsTable;
      using (HttpResponseMessage httpResponseMessage = tuple.Item2)
      {
        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
          throw new MetricsClientException(string.Format("Request failed with HTTP Status Code: {0}. TraceId: {1}.  Response: {2}", (object) httpResponseMessage.StatusCode, (object) traceId, (object) tuple.Item1));
        using (Stream responseFromMetrics = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
          responseAsTable = QueryLanguageResponseToDatatable.GetResponseAsTable(responseFromMetrics);
      }
      queryLanguageRequest = (KqlMRequest) null;
      return responseAsTable;
    }

    public Task<IQueryResultListV3> GetTimeSeriesAsync(
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
      IReadOnlyDictionary<string, string> additionalTracingInformation = null)
    {
      return this.GetFilteredDimensionValuesAsyncV3(metricId, dimensionFilters, startTimeUtc, endTimeUtc, samplingTypes, selectionClause, aggregationType, seriesResolutionInMinutes, traceId, outputDimensionNames, lastValueMode, additionalTracingInformation);
    }

    public Task<IReadOnlyList<MetricDefinitionV2>> GetMetricDefinitionsAsync(
      string monitoringAccount,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      Guid? traceId = null)
    {
      return this.GetMetricDefinitionsAsync(monitoringAccount, dimensionFilters, startTimeUtc, endTimeUtc, string.Empty, traceId);
    }

    public Task<IReadOnlyList<MetricDefinitionV2>> GetMetricDefinitionsAsync(
      string monitoringAccount,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      string metricNameStartsWithFilter,
      Guid? traceId = null)
    {
      return this.GetMetricDefinitionsAsync(monitoringAccount, dimensionFilters, startTimeUtc, endTimeUtc, string.Empty, 0, traceId);
    }

    public async Task<IReadOnlyList<MetricDefinitionV2>> GetMetricDefinitionsAsync(
      string monitoringAccount,
      IReadOnlyList<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      string metricNameStartsWithFilter,
      int maximumNumberOfMetricsToLook,
      Guid? traceId = null)
    {
      if (string.IsNullOrEmpty(monitoringAccount))
        throw new ArgumentNullException(nameof (monitoringAccount), "Monitoring account cannot be null or empty");
      if (dimensionFilters == null || dimensionFilters.Count == 0)
        throw new ArgumentNullException(nameof (dimensionFilters), "Dimension filters cannot be null or empty");
      if (startTimeUtc > endTimeUtc)
        throw new ArgumentException(string.Format("Start time cannot be greater than end time. StartTime:{0}, EndTime:{1}", (object) startTimeUtc, (object) endTimeUtc));
      MetricReader.ThrowIfInvalidDimensionFilters(dimensionFilters);
      bool flag = false;
      foreach (DimensionFilter dimensionFilter in (IEnumerable<DimensionFilter>) dimensionFilters)
      {
        if (dimensionFilter.DimensionValues.Count > 0)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        throw new ArgumentException("Dimension filters need to have atleast one dimension with dimension values filters");
      SortedDictionary<string, Tuple<bool, IReadOnlyList<string>>> dimensionNamesAndConstraints = MetricReader.GetDimensionNamesAndConstraints((IEnumerable<DimensionFilter>) dimensionFilters);
      string uriString = string.Format("{0}{1}/metricDefinitions/monitoringAccount/{2}/startTimeUtcMillis/{3}/endTimeUtcMillis/{4}", (object) await this.connectionInfo.GetEndpointAsync(monitoringAccount).ConfigureAwait(false), (object) this.MetaDataRelativeUrl, (object) SpecialCharsHelper.EscapeTwice(monitoringAccount), (object) UnixEpochHelper.GetMillis(startTimeUtc), (object) UnixEpochHelper.GetMillis(endTimeUtc));
      string str = string.Empty;
      if (!string.IsNullOrEmpty(metricNameStartsWithFilter))
        str = str + "metricNameStartsWithFilter=" + SpecialCharsHelper.EscapeTwice(metricNameStartsWithFilter);
      if (maximumNumberOfMetricsToLook > 0)
      {
        if (!string.IsNullOrEmpty(str))
          str += "&";
        str = str + "maximumNumberOfMetricsToLook=" + maximumNumberOfMetricsToLook.ToString();
      }
      if (!string.IsNullOrEmpty(str))
        uriString = uriString + "?" + str;
      Tuple<string, HttpResponseMessage> tuple = await HttpClientHelper.GetResponse(new Uri(uriString), HttpMethod.Post, this.httpClient, monitoringAccount, this.MetaDataRelativeUrl, (object) dimensionNamesAndConstraints, this.clientId, traceId: traceId).ConfigureAwait(false);
      IReadOnlyList<MetricDefinitionV2> definitionsAsync;
      using (tuple.Item2)
      {
        using (MemoryStream input = new MemoryStream(await tuple.Item2.Content.ReadAsByteArrayAsync().ConfigureAwait(false)))
        {
          using (BinaryReader binaryReader = new BinaryReader((Stream) input, Encoding.UTF8))
          {
            int num1 = (int) binaryReader.ReadByte();
            List<string> dimensionNames = new List<string>();
            int capacity = binaryReader.ReadInt32();
            List<MetricDefinitionV2> metricDefinitionV2List = new List<MetricDefinitionV2>(capacity);
            for (int index1 = 0; index1 < capacity; ++index1)
            {
              string monitoringAccount1 = binaryReader.ReadString();
              string metricNamespace = binaryReader.ReadString();
              string metricName = binaryReader.ReadString();
              int num2 = binaryReader.ReadInt32();
              for (int index2 = 0; index2 < num2; ++index2)
                dimensionNames.Add(binaryReader.ReadString());
              MetricDefinitionV2 metricDefinitionV2 = new MetricDefinitionV2(monitoringAccount1, metricNamespace, metricName, (IEnumerable<string>) dimensionNames);
              metricDefinitionV2List.Add(metricDefinitionV2);
              dimensionNames.Clear();
            }
            definitionsAsync = (IReadOnlyList<MetricDefinitionV2>) metricDefinitionV2List;
          }
        }
      }
      dimensionNamesAndConstraints = (SortedDictionary<string, Tuple<bool, IReadOnlyList<string>>>) null;
      return definitionsAsync;
    }

    public async Task<HttpResponseMessage> GetMultipleTimeSeriesAsync(
      IEnumerable<TimeSeriesDefinition<MetricIdentifier>> definitions,
      byte serializationVersion = 3,
      bool returnMetricNames = false,
      Guid? traceId = null,
      IReadOnlyDictionary<string, string> additionalTracingInformation = null)
    {
      TimeSeriesDefinition<MetricIdentifier>[] definitionArray = definitions != null ? definitions.ToArray<TimeSeriesDefinition<MetricIdentifier>>() : throw new ArgumentNullException(nameof (definitions));
      if (definitionArray.Length == 0)
        throw new ArgumentException("The count of 'definitions' is 0.");
      if (((IEnumerable<TimeSeriesDefinition<MetricIdentifier>>) definitionArray).Any<TimeSeriesDefinition<MetricIdentifier>>((Func<TimeSeriesDefinition<MetricIdentifier>, bool>) (d => d == null)))
        throw new ArgumentException("At least one of definitions is null.");
      string operation = string.Format("{0}/binary/version/{1}", (object) this.DataRelativeUrl, (object) serializationVersion);
      MetricIdentifier id = definitionArray[0].Id;
      string monitoringAccount = ((MetricIdentifier) ref id).MonitoringAccount;
      HttpResponseMessage multipleTimeSeriesAsync = (await HttpClientHelper.GetResponseWithCustomTraceId(new Uri(string.Format("{0}{1}/monitoringAccount/{2}/returnMetricNames/{3}", (object) (await this.connectionInfo.GetMetricsDataQueryEndpoint(monitoringAccount).ConfigureAwait(false)).OriginalString, (object) operation, (object) monitoringAccount, (object) returnMetricNames)), HttpMethod.Post, this.httpClient, monitoringAccount, operation, MetricReader.BuildTraceId(additionalTracingInformation, traceId), (object) definitionArray, this.clientId, serializationVersion: serializationVersion).ConfigureAwait(false)).Item2;
      definitionArray = (TimeSeriesDefinition<MetricIdentifier>[]) null;
      operation = (string) null;
      monitoringAccount = (string) null;
      return multipleTimeSeriesAsync;
    }

    [Obsolete]
    public async Task<HttpResponseMessage> GetFilteredTimeSeriesAsync(
      IReadOnlyList<FilteredTimeSeriesQueryRequest> filteredQueryRequests,
      byte serializationVersion,
      long maximumAllowedQueryCost,
      Guid traceId,
      bool returnRequestObjectOnFailure,
      IReadOnlyDictionary<string, string> additionalTracingInformation = null)
    {
      if (filteredQueryRequests == null)
        throw new ArgumentNullException(nameof (filteredQueryRequests));
      if (filteredQueryRequests.Count == 0)
        throw new ArgumentException("The count of 'filteredQueryRequests' is 0.");
      if (filteredQueryRequests.Any<FilteredTimeSeriesQueryRequest>((Func<FilteredTimeSeriesQueryRequest, bool>) (d => d == null)))
        throw new ArgumentException("At least one of filteredQueryRequests is null.");
      foreach (FilteredTimeSeriesQueryRequest filteredQueryRequest in (IEnumerable<FilteredTimeSeriesQueryRequest>) filteredQueryRequests)
      {
        if (filteredQueryRequest.DimensionFilters != null)
          MetricReader.ThrowIfInvalidDimensionFilters(filteredQueryRequest.DimensionFilters);
      }
      ConnectionInfo connectionInfo = this.connectionInfo;
      MetricIdentifier metricIdentifier = filteredQueryRequests[0].MetricIdentifier;
      string monitoringAccount = ((MetricIdentifier) ref metricIdentifier).MonitoringAccount;
      return (await HttpClientHelper.GetResponseWithCustomTraceId(new Uri(string.Format("{0}{1}/v1/multiple/serializationVersion/{2}/maxCost/{3}?timeoutInSeconds={4}&returnRequestObjectOnFailure={5}", (object) (await connectionInfo.GetMetricsDataQueryEndpoint(monitoringAccount).ConfigureAwait(false)).OriginalString, (object) this.QueryServiceRelativeUrl, (object) serializationVersion, (object) maximumAllowedQueryCost, (object) (int) this.connectionInfo.Timeout.TotalSeconds, (object) returnRequestObjectOnFailure)), HttpMethod.Post, this.httpClient, (string) null, (string) null, MetricReader.BuildTraceId(additionalTracingInformation, new Guid?(traceId)), (object) filteredQueryRequests, this.clientId, serializationVersion: serializationVersion).ConfigureAwait(false)).Item2;
    }

    public async Task<FilteredTimeSeriesQueryResponse> GetTimeSeriesByKqlMdm(
      string queryStatement,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      string monitoringAccount,
      Guid? traceId = null)
    {
      string uriString = string.Format("{0}{1}/v1/language/binary/monitoringAccount/{2}/serializationVersion/{3}", (object) await this.connectionInfo.GetEndpointAsync(monitoringAccount).ConfigureAwait(false), (object) this.QueryServiceRelativeUrl, (object) monitoringAccount, (object) (byte) 3);
      traceId = new Guid?(traceId ?? Guid.NewGuid());
      Uri url = new Uri(uriString);
      HttpMethod post = HttpMethod.Post;
      HttpClient httpClient = this.httpClient;
      string monitoringAccount1 = monitoringAccount;
      string serviceRelativeUrl = this.QueryServiceRelativeUrl;
      KqlMdmQueryRequest httpContent = new KqlMdmQueryRequest();
      httpContent.StartTimeUtc = startTimeUtc;
      httpContent.EndTimeUtc = endTimeUtc;
      httpContent.QueryStatement = queryStatement;
      string clientId = this.clientId;
      Guid? traceId1 = traceId;
      Tuple<string, HttpResponseMessage> tuple = await HttpClientHelper.GetResponse(url, post, httpClient, monitoringAccount1, serviceRelativeUrl, (object) httpContent, clientId, traceId: traceId1).ConfigureAwait(false);
      FilteredTimeSeriesQueryResponse timeSeriesByKqlMdm;
      using (HttpResponseMessage httpResponseMessage = tuple.Item2)
      {
        IEnumerable<string> values;
        httpResponseMessage.Headers.TryGetValues("__HandlingServerId__", out values);
        string handlingServerId = values != null ? values.FirstOrDefault<string>() : (string) null;
        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
          throw new MetricsClientException(string.Format("Request failed with HTTP Status Code: {0}. TraceId: {1}. HandlingServerId:{2}. Response: {3}", (object) httpResponseMessage.StatusCode, (object) traceId, (object) handlingServerId, (object) tuple.Item1));
        using (Stream input = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
        {
          using (BinaryReader reader = new BinaryReader(input))
          {
            int num = (int) reader.ReadByte();
            if (!reader.ReadBoolean())
              throw new MetricsClientException("The query failed and please try the query with Jarvis to see the error messages. handlingServerId:" + handlingServerId + ".");
            FilteredTimeSeriesQueryResponse seriesQueryResponse = new FilteredTimeSeriesQueryResponse();
            seriesQueryResponse.Deserialize(reader);
            seriesQueryResponse.DiagnosticInfo.TraceId = traceId.Value.ToString("B");
            seriesQueryResponse.DiagnosticInfo.HandlingServerId = handlingServerId;
            timeSeriesByKqlMdm = seriesQueryResponse;
          }
        }
      }
      return timeSeriesByKqlMdm;
    }

    public async Task<HttpResponseMessage> GetTimeSeriesByKqlMdmStreamed(
      string traceId,
      string monitoringAccount,
      string queryStatement,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      bool isDebugQuery = false)
    {
      if (string.IsNullOrEmpty(monitoringAccount))
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (startTimeUtc > endTimeUtc)
        throw new ArgumentException("startTimeUtc cannot be greater than endTimeUtc");
      if (string.IsNullOrEmpty(queryStatement))
        throw new ArgumentNullException(nameof (queryStatement));
      string uriString = string.Format("{0}{1}/v1/language/binary/monitoringAccount/{2}/serializationVersion/{3}", (object) await this.connectionInfo.GetEndpointAsync(monitoringAccount).ConfigureAwait(false), (object) this.QueryServiceRelativeUrl, (object) monitoringAccount, (object) (byte) 3);
      Guid? nullable1 = new Guid?();
      Guid result;
      if (Guid.TryParse(traceId, out result))
        nullable1 = new Guid?(result);
      Uri url = new Uri(uriString);
      HttpMethod post = HttpMethod.Post;
      HttpClient httpClient = this.httpClient;
      string monitoringAccount1 = monitoringAccount;
      object obj = (object) new KqlMdmQueryRequest()
      {
        StartTimeUtc = startTimeUtc,
        EndTimeUtc = endTimeUtc,
        QueryStatement = queryStatement
      };
      string clientId1 = this.clientId;
      Guid? nullable2 = nullable1;
      int num = isDebugQuery ? 1 : 0;
      object httpContent = obj;
      string clientId2 = clientId1;
      Guid? traceId1 = nullable2;
      Tuple<string, HttpResponseMessage> tuple = await HttpClientHelper.GetResponse(url, post, httpClient, monitoringAccount1, nameof (GetTimeSeriesByKqlMdmStreamed), false, num != 0, httpContent, clientId2, traceId: traceId1).ConfigureAwait(false);
      if (tuple.Item2.StatusCode != HttpStatusCode.OK)
      {
        IEnumerable<string> values;
        tuple.Item2.Headers.TryGetValues("__HandlingServerId__", out values);
        string str = values != null ? values.FirstOrDefault<string>() : (string) null;
        string message = string.Format("Request failed with HTTP Status Code: {0}. TraceId: {1}. HandlingServerId:{2}. Response: {3}", (object) tuple.Item2.StatusCode, (object) traceId, (object) str, (object) tuple.Item1);
        tuple.Item2.Dispose();
        throw new MetricsClientException(message);
      }
      return tuple.Item2;
    }

    public async Task<IKqlmQueryResult> ExecuteKqlmQueryAsync(
      string monitoringAccount,
      string metricNamespace,
      string queryText,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrWhiteSpace(monitoringAccount))
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrWhiteSpace(queryText))
        throw new ArgumentNullException(nameof (queryText));
      return await this.ExecuteKqlmQueryInternalAsync(MetricReader.BuildTraceId(), monitoringAccount, metricNamespace, queryText, startTimeUtc, endTimeUtc, (IReadOnlyDictionary<string, string>) null, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IKqlmQueryResult> ExecuteKqlmQueryAsync(
      string monitoringAccount,
      string metricNamespace,
      string queryText,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      IReadOnlyDictionary<string, string> queryParameters,
      CancellationToken cancellationToken,
      IReadOnlyDictionary<string, string> additionalTracingInformation = null)
    {
      if (string.IsNullOrWhiteSpace(monitoringAccount))
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrWhiteSpace(queryText))
        throw new ArgumentNullException(nameof (queryText));
      if (queryParameters == null)
        throw new ArgumentNullException(nameof (queryParameters));
      return await this.ExecuteKqlmQueryInternalAsync(MetricReader.BuildTraceId(additionalTracingInformation), monitoringAccount, metricNamespace, queryText, startTimeUtc, endTimeUtc, queryParameters, cancellationToken).ConfigureAwait(false);
    }

    async Task<ValidateKqlmQueryResult> IExportValidator.ValidateKqlmQueryAsync(
      string queryStatement,
      string monitoringAccount,
      string metricNamespace)
    {
      ValidateKqlmQueryRequest request = new ValidateKqlmQueryRequest(monitoringAccount, metricNamespace, queryStatement);
      Tuple<string, HttpResponseMessage> tuple = await HttpClientHelper.GetResponse(new Uri(string.Format("{0}{1}/v1/language/validate/monitoringAccount/{2}", (object) await this.connectionInfo.GetEndpointAsync(monitoringAccount).ConfigureAwait(false), (object) this.QueryServiceRelativeUrl, (object) monitoringAccount)), HttpMethod.Post, this.httpClient, monitoringAccount, "ValidateKqlmQuery", (object) request).ConfigureAwait(false);
      ValidateKqlmQueryResult validateKqlmQueryResult;
      using (HttpResponseMessage httpResponseMessage = tuple.Item2)
      {
        IEnumerable<string> values;
        httpResponseMessage.Headers.TryGetValues("__HandlingServerId__", out values);
        string str = values != null ? values.FirstOrDefault<string>() : (string) null;
        ValidateKqlmQueryResult result = new ValidateKqlmQueryResult();
        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
          throw new MetricsClientException(string.Format("Request failed with HTTP Status Code: {0}. HandlingServerId:{1}. Response: {2}", (object) httpResponseMessage.StatusCode, (object) str, (object) tuple.Item1));
        using (Stream input = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
        {
          using (BinaryReader reader = new BinaryReader(input))
          {
            result.Deserialize(reader);
            validateKqlmQueryResult = result;
          }
        }
      }
      request = (ValidateKqlmQueryRequest) null;
      return validateKqlmQueryResult;
    }

    internal static string BuildTraceId(
      IReadOnlyDictionary<string, string> tracingInformation = null,
      Guid? traceId = null)
    {
      traceId = new Guid?(traceId ?? Guid.NewGuid());
      if (tracingInformation == null || tracingInformation.Count == 0)
        return traceId.Value.ToString("B");
      if (tracingInformation != null && tracingInformation.Count > 100)
        throw new ArgumentException(string.Format("Additional tracing information fields cannot be more than {0}", (object) 100));
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(traceId.Value.ToString("B"));
      stringBuilder.Append(";");
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) tracingInformation)
        stringBuilder.Append(keyValuePair.Key).Append("=").Append(keyValuePair.Value).Append(";");
      return stringBuilder.ToString();
    }

    private static void ThrowIfInvalidDimensionFilters(IReadOnlyList<DimensionFilter> filters)
    {
      bool flag1 = false;
      bool flag2 = false;
      foreach (DimensionFilter filter in (IEnumerable<DimensionFilter>) filters)
      {
        if (filter.IsStartsWithFilter)
          flag1 = true;
        else
          flag2 = ((flag2 ? 1 : 0) | (filter.IsExcludeFilter || filter.DimensionValues == null ? 0 : (filter.DimensionValues.Count > 0 ? 1 : 0))) != 0;
      }
      if (flag1 && !flag2)
        throw new MetricsClientException("'StartsWith' filter can only be provided with atleast one 'EqualsTo' filter");
    }

    private static void NormalizeTimeRange(ref DateTime startTimeUtc, ref DateTime endTimeUtc)
    {
      startTimeUtc = new DateTime(startTimeUtc.Ticks / 600000000L * 600000000L);
      endTimeUtc = new DateTime(endTimeUtc.Ticks / 600000000L * 600000000L);
    }

    private static SortedDictionary<string, Tuple<bool, IReadOnlyList<string>>> GetDimensionNamesAndConstraints(
      IEnumerable<DimensionFilter> dimensionFilters)
    {
      SortedDictionary<string, Tuple<bool, IReadOnlyList<string>>> namesAndConstraints = new SortedDictionary<string, Tuple<bool, IReadOnlyList<string>>>((IComparer<string>) StringComparer.OrdinalIgnoreCase);
      bool flag1 = false;
      bool flag2 = false;
      if (dimensionFilters != null)
      {
        foreach (DimensionFilter dimensionFilter in dimensionFilters)
        {
          if (dimensionFilter.IsStartsWithFilter)
            flag1 = true;
          else
            flag2 = ((flag2 ? 1 : 0) | (dimensionFilter.IsExcludeFilter || dimensionFilter.DimensionValues == null ? 0 : (dimensionFilter.DimensionValues.Count > 0 ? 1 : 0))) != 0;
          Tuple<bool, IReadOnlyList<string>> tuple = dimensionFilter.DimensionValues != null ? Tuple.Create<bool, IReadOnlyList<string>>(dimensionFilter.IsExcludeFilter, dimensionFilter.DimensionValues) : Tuple.Create<bool, IReadOnlyList<string>>(true, (IReadOnlyList<string>) MetricReader.EmptyStringArray);
          if (namesAndConstraints.ContainsKey(dimensionFilter.DimensionName))
            throw new MetricsClientException("Only one filter can be specified for a dimension. Another filter already exists for dimension: " + dimensionFilter.DimensionName);
          namesAndConstraints.Add(dimensionFilter.DimensionName, tuple);
        }
      }
      if (flag1 && !flag2)
        throw new MetricsClientException("Starts with filter can only be provided with atleast one equals to filter");
      return namesAndConstraints;
    }

    private NameValueCollection BuildQueryParameters(
      MetricIdentifier metricId,
      IEnumerable<DimensionFilter> dimensionFilters,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType samplingType,
      Reducer reducer,
      QueryFilter queryFilter,
      bool includeSeries,
      SelectionClause selectionClause,
      AggregationType aggregationType,
      long seriesResolutionInMinutes,
      out SortedDictionary<string, Tuple<bool, IReadOnlyList<string>>> dimensionNamesAndConstraints)
    {
      NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
      long millis1 = UnixEpochHelper.GetMillis(startTimeUtc);
      long millis2 = UnixEpochHelper.GetMillis(endTimeUtc);
      if (millis1 > millis2)
        throw new ArgumentException("Start time must be before end time.");
      if (queryFilter == null)
        throw new ArgumentNullException(nameof (queryFilter));
      if (reducer == Reducer.Undefined)
        throw new ArgumentException("Reducer cannot not be undefined.  Use QueryFilter.NoFilter to get all time series.");
      if (queryFilter != QueryFilter.NoFilter && queryFilter.Operator == Operator.Undefined)
        throw new ArgumentException("Operator cannot not be undefined.  Use QueryFilter.NoFilter to get all time series.");
      queryString["SamplingType"] = samplingType.ToString();
      queryString["startTime"] = millis1.ToString();
      queryString["endTime"] = millis2.ToString();
      queryString[nameof (includeSeries)] = includeSeries.ToString();
      queryString[nameof (reducer)] = reducer.ToString();
      if (includeSeries)
      {
        queryString["seriesAggregationType"] = aggregationType.ToString();
        queryString["seriesResolution"] = (seriesResolutionInMinutes * 60000L).ToString();
      }
      if (queryFilter == QueryFilter.NoFilter)
      {
        queryString["noFilter"] = "true";
      }
      else
      {
        queryString["operator"] = queryFilter.Operator.ToString();
        queryString["operand"] = queryFilter.Operand.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      if (selectionClause != null && selectionClause != SelectionClause.AllResults)
      {
        queryString["selectionType"] = selectionClause.SelectionType.ToString();
        queryString["top"] = selectionClause.QuantityToSelect.ToString();
        queryString["orderBy"] = selectionClause.OrderBy.ToString();
      }
      dimensionNamesAndConstraints = MetricReader.GetDimensionNamesAndConstraints(dimensionFilters);
      return queryString;
    }

    private async Task<IKqlmQueryResult> ExecuteKqlmQueryInternalAsync(
      string traceId,
      string monitoringAccount,
      string metricNamespace,
      string queryText,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      IReadOnlyDictionary<string, string> queryParameters,
      CancellationToken cancellationToken)
    {
      IKqlmQueryResult kqlmQueryResult;
      try
      {
        KqlMdmQueryRequest request = new KqlMdmQueryRequest()
        {
          StartTimeUtc = startTimeUtc,
          EndTimeUtc = endTimeUtc,
          MetricNamespace = metricNamespace,
          QueryStatement = queryText,
          QueryParameters = queryParameters
        };
        Task<Tuple<string, HttpResponseMessage>> requestTask = HttpClientHelper.GetResponseWithCustomTraceId(new Uri(string.Format("{0}{1}/v1/kqlm/monitoringAccount/{2}", (object) (await this.connectionInfo.GetMetricsDataQueryEndpoint(monitoringAccount).ConfigureAwait(false)).OriginalString, (object) this.QueryServiceRelativeUrl, (object) monitoringAccount)), HttpMethod.Post, this.httpClient, monitoringAccount, "ExecuteKqlmQuery", traceId, (object) request);
        Task task = Task.Delay(-1, cancellationToken);
        object obj = (object) task;
        Task[] taskArray = new Task[2]
        {
          (Task) requestTask,
          task
        };
        if (obj == await Task.WhenAny(taskArray).ConfigureAwait(false))
        {
          obj = (object) null;
          throw new OperationCanceledException(cancellationToken);
        }
        Tuple<string, HttpResponseMessage> tuple = await requestTask.ConfigureAwait(false);
        using (HttpResponseMessage httpResponseMessage = tuple.Item2)
        {
          IEnumerable<string> values;
          httpResponseMessage.Headers.TryGetValues("__HandlingServerId__", out values);
          string str = values != null ? values.FirstOrDefault<string>() : (string) null;
          KqlmQueryResult result = new KqlmQueryResult();
          if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            throw new MetricsClientException(string.Format("Request failed with HTTP Status Code: {0}. HandlingServerId:{1}. Response: {2}", (object) httpResponseMessage.StatusCode, (object) (str ?? "n/a"), (object) tuple.Item1));
          using (Stream input = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
          {
            using (BinaryReader reader = new BinaryReader(input, Encoding.UTF8))
            {
              result.Deserialize(reader);
              kqlmQueryResult = (IKqlmQueryResult) result;
            }
          }
        }
      }
      catch (ArgumentNullException ex)
      {
        throw;
      }
      catch (OperationCanceledException ex)
      {
        throw;
      }
      catch (MetricsClientException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new MetricsClientException(ex.Message);
      }
      return kqlmQueryResult;
    }
  }
}
