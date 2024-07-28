// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Monitors.MonitorReader
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using Microsoft.Cloud.Metrics.Client.Utility;
using Microsoft.Online.Metrics.Serialization.Configuration;
using Microsoft.Online.Metrics.Serialization.Monitor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Monitors
{
  public sealed class MonitorReader : IMonitorReader
  {
    public const int NumAttempts = 3;
    public readonly string HealthRelativeUrl;
    public readonly string ConfigRelativeUrlV2;
    private static readonly SamplingType StatusSamplingType = new SamplingType("Status");
    private readonly HttpClient httpClient;
    private readonly ConnectionInfo connectionInfo;

    public MonitorReader(ConnectionInfo connectionInfo)
    {
      this.connectionInfo = connectionInfo != null ? connectionInfo : throw new ArgumentNullException(nameof (connectionInfo));
      this.HealthRelativeUrl = this.connectionInfo.GetAuthRelativeUrl("v3/data/health");
      this.ConfigRelativeUrlV2 = this.connectionInfo.GetAuthRelativeUrl("v2/config/metrics");
      this.httpClient = HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo);
      this.GetResponseAsStringDelegate = new Func<Uri, HttpMethod, HttpClient, string, string, object, string, string, Guid?, byte, int, Task<Tuple<string, HttpResponseMessage>>>(HttpClientHelper.GetResponse);
    }

    internal Func<Uri, HttpMethod, HttpClient, string, string, object, string, string, Guid?, byte, int, Task<Tuple<string, HttpResponseMessage>>> GetResponseAsStringDelegate { get; set; }

    public async Task<IReadOnlyList<MonitorIdentifier>> GetMonitorsAsync(
      MetricIdentifier metricIdentifier)
    {
      ((MetricIdentifier) ref metricIdentifier).Validate();
      Tuple<string, HttpResponseMessage> tuple = await this.GetResponseAsStringDelegate(new Uri(string.Format("{0}{1}/monitoringAccount/{2}/metricNamespace/{3}/metric/{4}/monitorIDs", (object) await this.connectionInfo.GetEndpointAsync(((MetricIdentifier) ref metricIdentifier).MonitoringAccount).ConfigureAwait(false), (object) this.ConfigRelativeUrlV2, (object) ((MetricIdentifier) ref metricIdentifier).MonitoringAccount, (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricIdentifier).MetricNamespace), (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricIdentifier).MetricName))), HttpMethod.Get, this.httpClient, ((MetricIdentifier) ref metricIdentifier).MonitoringAccount, this.ConfigRelativeUrlV2, (object) null, string.Empty, (string) null, new Guid?(), (byte) 3, 3).ConfigureAwait(false);
      IReadOnlyList<MonitorIdentifier> monitorsAsync;
      using (tuple.Item2)
        monitorsAsync = (IReadOnlyList<MonitorIdentifier>) JsonConvert.DeserializeObject<MonitorIdentifier[]>(tuple.Item1);
      return monitorsAsync;
    }

    public async Task<IReadOnlyList<MonitorIdentifier>> GetMonitorsAsync(
      string monitoringAccount,
      string metricNamespace = null)
    {
      if (string.IsNullOrWhiteSpace(monitoringAccount))
        throw new ArgumentException("monitoringAccount is null or empty.", monitoringAccount);
      string namespaceSegments = string.Empty;
      if (!string.IsNullOrWhiteSpace(metricNamespace))
        namespaceSegments = string.Format("metricNamespace/{0}", (object) SpecialCharsHelper.EscapeTwice(metricNamespace));
      Tuple<string, HttpResponseMessage> tuple = await this.GetResponseAsStringDelegate(new Uri(string.Format("{0}{1}/monitoringAccount/{2}/{3}/monitorIDs", (object) await this.connectionInfo.GetEndpointAsync(monitoringAccount).ConfigureAwait(false), (object) this.ConfigRelativeUrlV2, (object) monitoringAccount, (object) namespaceSegments)), HttpMethod.Get, this.httpClient, monitoringAccount, this.ConfigRelativeUrlV2, (object) null, string.Empty, (string) null, new Guid?(), (byte) 3, 3).ConfigureAwait(false);
      IReadOnlyList<MonitorIdentifier> monitorsAsync;
      using (tuple.Item2)
        monitorsAsync = (IReadOnlyList<MonitorIdentifier>) JsonConvert.DeserializeObject<MonitorIdentifier[]>(tuple.Item1);
      namespaceSegments = (string) null;
      return monitorsAsync;
    }

    public async Task<IMonitorHealthStatus> GetCurrentHeathStatusAsync(
      TimeSeriesDefinition<MonitorIdentifier> monitorInstanceDefinition)
    {
      return await this.GetCurrentHealthStatusAsync(monitorInstanceDefinition).ConfigureAwait(false);
    }

    public async Task<IMonitorHealthStatus> GetCurrentHealthStatusAsync(
      TimeSeriesDefinition<MonitorIdentifier> monitorInstanceDefinition)
    {
      MonitorReader monitorReader = this;
      if (monitorInstanceDefinition == null)
        throw new ArgumentNullException(nameof (monitorInstanceDefinition));
      return (await monitorReader.GetMultipleCurrentHeathStatusesAsync(new TimeSeriesDefinition<MonitorIdentifier>[1]
      {
        monitorInstanceDefinition
      }).ConfigureAwait(false)).First<KeyValuePair<TimeSeriesDefinition<MonitorIdentifier>, IMonitorHealthStatus>>().Value;
    }

    public Task<IReadOnlyList<KeyValuePair<TimeSeriesDefinition<MonitorIdentifier>, IMonitorHealthStatus>>> GetMultipleCurrentHeathStatusesAsync(
      params TimeSeriesDefinition<MonitorIdentifier>[] monitorInstanceDefinitions)
    {
      return this.GetMultipleCurrentHeathStatusesAsync(((IEnumerable<TimeSeriesDefinition<MonitorIdentifier>>) monitorInstanceDefinitions).AsEnumerable<TimeSeriesDefinition<MonitorIdentifier>>());
    }

    public async Task<IReadOnlyList<KeyValuePair<TimeSeriesDefinition<MonitorIdentifier>, IMonitorHealthStatus>>> GetMultipleCurrentHeathStatusesAsync(
      IEnumerable<TimeSeriesDefinition<MonitorIdentifier>> monitorInstanceDefinitions)
    {
      List<TimeSeriesDefinition<MonitorIdentifier>> definitionList = monitorInstanceDefinitions != null ? monitorInstanceDefinitions.ToList<TimeSeriesDefinition<MonitorIdentifier>>() : throw new ArgumentNullException(nameof (monitorInstanceDefinitions));
      if (definitionList.Count == 0)
        throw new ArgumentException("The count of 'monitorInstanceDefinitions' is 0.");
      MonitorIdentifier monitorIdentifier = !definitionList.Any<TimeSeriesDefinition<MonitorIdentifier>>((Func<TimeSeriesDefinition<MonitorIdentifier>, bool>) (d => d == null)) ? definitionList[0].Id : throw new ArgumentException("At least one of monitorInstanceDefinitions are null.");
      List<Dictionary<string, string>> dimensionCombinationList = new List<Dictionary<string, string>>(definitionList.Count);
      foreach (TimeSeriesDefinition<MonitorIdentifier> seriesDefinition in definitionList)
      {
        if (!seriesDefinition.Id.Equals(monitorIdentifier))
          throw new MetricsClientException("All the time series definitions must have the same monitor identifier.");
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        if (seriesDefinition.DimensionCombination != null)
        {
          foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) seriesDefinition.DimensionCombination)
            dictionary[keyValuePair.Key] = keyValuePair.Value;
        }
        dimensionCombinationList.Add(dictionary);
      }
      string operation = this.HealthRelativeUrl + "/batchedRead";
      ConnectionInfo connectionInfo = this.connectionInfo;
      MetricIdentifier metricIdentifier1 = monitorIdentifier.MetricIdentifier;
      string monitoringAccount1 = ((MetricIdentifier) ref metricIdentifier1).MonitoringAccount;
      object[] objArray = new object[6]
      {
        (object) await connectionInfo.GetEndpointAsync(monitoringAccount1).ConfigureAwait(false),
        (object) operation,
        null,
        null,
        null,
        null
      };
      MetricIdentifier metricIdentifier2 = monitorIdentifier.MetricIdentifier;
      objArray[2] = (object) ((MetricIdentifier) ref metricIdentifier2).MonitoringAccount;
      metricIdentifier2 = monitorIdentifier.MetricIdentifier;
      objArray[3] = (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricIdentifier2).MetricNamespace);
      metricIdentifier2 = monitorIdentifier.MetricIdentifier;
      objArray[4] = (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricIdentifier2).MetricName);
      objArray[5] = (object) SpecialCharsHelper.EscapeTwice(monitorIdentifier.MonitorId);
      string uriString = string.Format("{0}{1}/monitoringAccount/{2}/metricNamespace/{3}/metric/{4}/monitorId/{5}", objArray);
      Func<Uri, HttpMethod, HttpClient, string, string, object, string, string, Guid?, byte, int, Task<Tuple<string, HttpResponseMessage>>> asStringDelegate = this.GetResponseAsStringDelegate;
      Uri uri = new Uri(uriString);
      HttpMethod post = HttpMethod.Post;
      HttpClient httpClient = this.httpClient;
      metricIdentifier2 = monitorIdentifier.MetricIdentifier;
      string monitoringAccount2 = ((MetricIdentifier) ref metricIdentifier2).MonitoringAccount;
      string str = operation;
      List<Dictionary<string, string>> dictionaryList = dimensionCombinationList;
      string empty = string.Empty;
      Guid? nullable = new Guid?();
      Tuple<string, HttpResponseMessage> tuple = await asStringDelegate(uri, post, httpClient, monitoringAccount2, str, (object) dictionaryList, empty, (string) null, nullable, (byte) 3, 3).ConfigureAwait(false);
      IReadOnlyList<KeyValuePair<TimeSeriesDefinition<MonitorIdentifier>, IMonitorHealthStatus>> heathStatusesAsync;
      using (tuple.Item2)
      {
        List<MonitorHealthStatus> monitorHealthStatusList = JsonConvert.DeserializeObject<List<MonitorHealthStatus>>(tuple.Item1);
        KeyValuePair<TimeSeriesDefinition<MonitorIdentifier>, IMonitorHealthStatus>[] keyValuePairArray = new KeyValuePair<TimeSeriesDefinition<MonitorIdentifier>, IMonitorHealthStatus>[monitorHealthStatusList.Count];
        for (int index = 0; index < monitorHealthStatusList.Count; ++index)
        {
          KeyValuePair<TimeSeriesDefinition<MonitorIdentifier>, IMonitorHealthStatus> keyValuePair = new KeyValuePair<TimeSeriesDefinition<MonitorIdentifier>, IMonitorHealthStatus>(definitionList[index], (IMonitorHealthStatus) monitorHealthStatusList[index]);
          keyValuePairArray[index] = keyValuePair;
        }
        heathStatusesAsync = (IReadOnlyList<KeyValuePair<TimeSeriesDefinition<MonitorIdentifier>, IMonitorHealthStatus>>) keyValuePairArray;
      }
      definitionList = (List<TimeSeriesDefinition<MonitorIdentifier>>) null;
      monitorIdentifier = new MonitorIdentifier();
      dimensionCombinationList = (List<Dictionary<string, string>>) null;
      operation = (string) null;
      return heathStatusesAsync;
    }

    [Obsolete("We are going to retire this. Please use GetBatchWatchdogHealthHistory in Health SDK instead.")]
    public async Task<TimeSeries<MonitorIdentifier, bool?>> GetMonitorHistoryAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      TimeSeriesDefinition<MonitorIdentifier> monitorInstanceDefinition)
    {
      if (monitorInstanceDefinition == null)
        throw new ArgumentNullException(nameof (monitorInstanceDefinition));
      startTimeUtc = !(startTimeUtc > endTimeUtc) ? new DateTime(startTimeUtc.Ticks / 600000000L * 600000000L) : throw new ArgumentException(string.Format("startTimeUtc [{0}] must be <= endTimeUtc [{1}]", (object) startTimeUtc, (object) endTimeUtc));
      endTimeUtc = new DateTime(endTimeUtc.Ticks / 600000000L * 600000000L);
      string dimensionsFlattened = (string) null;
      if (monitorInstanceDefinition.DimensionCombination != null)
        dimensionsFlattened = string.Join("/", monitorInstanceDefinition.DimensionCombination.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (d => string.Join("/", new string[2]
        {
          SpecialCharsHelper.EscapeTwice(d.Key),
          SpecialCharsHelper.EscapeTwice(d.Value)
        }))));
      string operation = this.HealthRelativeUrl + "/history";
      ConnectionInfo connectionInfo = this.connectionInfo;
      MetricIdentifier metricIdentifier1 = monitorInstanceDefinition.Id.MetricIdentifier;
      string monitoringAccount1 = ((MetricIdentifier) ref metricIdentifier1).MonitoringAccount;
      Uri uri1 = await connectionInfo.GetEndpointAsync(monitoringAccount1).ConfigureAwait(false);
      object[] objArray = new object[9];
      objArray[0] = (object) uri1;
      objArray[1] = (object) operation;
      MetricIdentifier metricIdentifier2 = monitorInstanceDefinition.Id.MetricIdentifier;
      objArray[2] = (object) ((MetricIdentifier) ref metricIdentifier2).MonitoringAccount;
      MetricIdentifier metricIdentifier3 = monitorInstanceDefinition.Id.MetricIdentifier;
      objArray[3] = (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricIdentifier3).MetricNamespace);
      MetricIdentifier metricIdentifier4 = monitorInstanceDefinition.Id.MetricIdentifier;
      objArray[4] = (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricIdentifier4).MetricName);
      MonitorIdentifier id = monitorInstanceDefinition.Id;
      objArray[5] = (object) SpecialCharsHelper.EscapeTwice(id.MonitorId);
      objArray[6] = (object) UnixEpochHelper.GetMillis(startTimeUtc);
      objArray[7] = (object) UnixEpochHelper.GetMillis(endTimeUtc);
      objArray[8] = dimensionsFlattened != null ? (object) ("/" + dimensionsFlattened) : (object) string.Empty;
      string uriString = string.Format("{0}{1}/monitoringAccount/{2}/metricNamespace/{3}/metric/{4}/monitorId/{5}/from/{6}/to/{7}{8}", objArray);
      Func<Uri, HttpMethod, HttpClient, string, string, object, string, string, Guid?, byte, int, Task<Tuple<string, HttpResponseMessage>>> asStringDelegate = this.GetResponseAsStringDelegate;
      Uri uri2 = new Uri(uriString);
      HttpMethod get = HttpMethod.Get;
      HttpClient httpClient = this.httpClient;
      id = monitorInstanceDefinition.Id;
      MetricIdentifier metricIdentifier5 = id.MetricIdentifier;
      string monitoringAccount2 = ((MetricIdentifier) ref metricIdentifier5).MonitoringAccount;
      string str = operation;
      string empty = string.Empty;
      Guid? nullable = new Guid?();
      Tuple<string, HttpResponseMessage> tuple = await asStringDelegate(uri2, get, httpClient, monitoringAccount2, str, (object) null, empty, (string) null, nullable, (byte) 3, 3).ConfigureAwait(false);
      TimeSeries<MonitorIdentifier, bool?> monitorHistoryAsync;
      using (tuple.Item2)
      {
        List<bool?> nullableList = JsonConvert.DeserializeObject<List<bool?>>(tuple.Item1);
        monitorHistoryAsync = new TimeSeries<MonitorIdentifier, bool?>(startTimeUtc, endTimeUtc, 1, monitorInstanceDefinition, new List<List<bool?>>()
        {
          nullableList
        }, TimeSeriesErrorCode.Success);
      }
      dimensionsFlattened = (string) null;
      operation = (string) null;
      return monitorHistoryAsync;
    }
  }
}
