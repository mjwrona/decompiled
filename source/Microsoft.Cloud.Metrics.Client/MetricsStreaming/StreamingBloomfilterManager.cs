// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsStreaming.StreamingBloomfilterManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.Cloud.Metrics.Client.MetricsStreaming
{
  public sealed class StreamingBloomfilterManager
  {
    private const int Port = 8952;
    private const string ClientVersion = "clientVersion=1";
    private const string BloomfilterConfigurationPathPrefix = "/api/v1/bloomfilter";
    private const string RuleAndBloomfilterConfigurationPathPrefix = "/api/v1/streamingruleandbloomfilter";
    private readonly ConnectionInfo connectionInfo;
    private readonly HttpClient httpClient;

    public StreamingBloomfilterManager(ConnectionInfo connectionInfo, HttpClient httpClient)
    {
      this.connectionInfo = connectionInfo ?? throw new ArgumentNullException(nameof (connectionInfo));
      this.httpClient = httpClient ?? throw new ArgumentNullException(nameof (httpClient));
    }

    public async Task<StreamingBloomfilterResponseItem> CreateBloomFilterAsync(
      StreamingBloomfilterRequest streamingBloomfilterRequest,
      CancellationToken cancellationToken = default (CancellationToken),
      Guid? traceId = null)
    {
      if (streamingBloomfilterRequest == null)
        throw new ArgumentException("'streamingBloomfilterRequest' cannot be null.", nameof (streamingBloomfilterRequest));
      return JsonConvert.DeserializeObject<StreamingBloomfilterResponseItem>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = "/api/v1/bloomfilter",
        Query = "clientVersion=1",
        Port = 8952
      }.Uri, HttpMethod.Post, this.httpClient, (string) null, "/api/v1/bloomfilter", serializedContent: JsonConvert.SerializeObject((object) streamingBloomfilterRequest), traceId: traceId).ConfigureAwait(false));
    }

    public async Task<StreamingBloomfilterResponseItem> GetBloomFilterAsync(
      string bloomfilterId,
      bool includeFilterData = false,
      CancellationToken cancellationToken = default (CancellationToken),
      Guid? traceId = null)
    {
      string str = !string.IsNullOrEmpty(bloomfilterId) ? string.Format("{0}/{1}/{2}", (object) "/api/v1/bloomfilter", (object) bloomfilterId, (object) includeFilterData) : throw new ArgumentException("'bloomfilterId' cannot be null or empty.", nameof (bloomfilterId));
      return JsonConvert.DeserializeObject<StreamingBloomfilterResponseItem>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = str,
        Query = "clientVersion=1",
        Port = 8952
      }.Uri, HttpMethod.Get, this.httpClient, (string) null, "/api/v1/bloomfilter", traceId: traceId).ConfigureAwait(false));
    }

    public async Task<ListStreamingBloomfilterResponse> QueryBloomfilterAsync(
      Dictionary<string, string> filters,
      bool includeFilterData = false,
      string continuationToken = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Guid? traceId = null)
    {
      if (filters == null || filters.Count == 0)
        throw new ArgumentException("'filters' should contain at least one filter.", nameof (filters));
      string str = string.Join("&", filters.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (filter => "filters[" + filter.Key + "]=" + HttpUtility.UrlEncode(filter.Value))).ToArray<string>());
      if (!string.IsNullOrEmpty(continuationToken))
        str = string.Format("{0}&&includeFilterData={1}&continuationToken={2}", (object) str, (object) includeFilterData, (object) HttpUtility.UrlEncode(continuationToken));
      return JsonConvert.DeserializeObject<ListStreamingBloomfilterResponse>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = "/api/v1/bloomfilter",
        Query = ("clientVersion=1&" + str),
        Port = 8952
      }.Uri, HttpMethod.Get, this.httpClient, (string) null, "/api/v1/bloomfilter", traceId: traceId).ConfigureAwait(false));
    }

    public async Task DeleteBloomFilterAsync(
      string bloomfilterId,
      CancellationToken cancellationToken = default (CancellationToken),
      Guid? traceId = null)
    {
      if (string.IsNullOrEmpty(bloomfilterId))
        throw new ArgumentException("'bloomfilterId' cannot be null or empty.", nameof (bloomfilterId));
      string str = await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = ("/api/v1/bloomfilter/" + bloomfilterId),
        Query = "clientVersion=1",
        Port = 8952
      }.Uri, HttpMethod.Delete, this.httpClient, (string) null, "/api/v1/bloomfilter", traceId: traceId).ConfigureAwait(false);
    }

    public async Task<StreamingBloomfilterResponseItem> ReplaceBloomFilterAsync(
      string bloomfilterId,
      StreamingBloomfilterRequest streamingBloomfilterRequest,
      CancellationToken cancellationToken = default (CancellationToken),
      Guid? traceId = null)
    {
      if (string.IsNullOrEmpty(bloomfilterId))
        throw new ArgumentException("'bloomfilterId' cannot be null or empty.", nameof (bloomfilterId));
      if (streamingBloomfilterRequest == null)
        throw new ArgumentException("'streamingBloomfilterRequest' cannot be null.", nameof (streamingBloomfilterRequest));
      return JsonConvert.DeserializeObject<StreamingBloomfilterResponseItem>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = ("/api/v1/bloomfilter/" + bloomfilterId),
        Query = "clientVersion=1",
        Port = 8952
      }.Uri, HttpMethod.Put, this.httpClient, (string) null, "/api/v1/bloomfilter", serializedContent: JsonConvert.SerializeObject((object) streamingBloomfilterRequest), traceId: traceId).ConfigureAwait(false));
    }

    public async Task<StreamingRuleAndBloomfilterResponse> CreateAndSetBloomFilterAsync(
      StreamingRuleAndBloomfilterRequest request,
      CancellationToken cancellationToken = default (CancellationToken),
      Guid? traceId = null)
    {
      if (request == null)
        throw new ArgumentException("'request' cannot be null.", nameof (request));
      return JsonConvert.DeserializeObject<StreamingRuleAndBloomfilterResponse>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = "/api/v1/streamingruleandbloomfilter",
        Query = "clientVersion=1",
        Port = 8952
      }.Uri, HttpMethod.Post, this.httpClient, (string) null, "/api/v1/streamingruleandbloomfilter", serializedContent: JsonConvert.SerializeObject((object) request), traceId: traceId).ConfigureAwait(false));
    }
  }
}
