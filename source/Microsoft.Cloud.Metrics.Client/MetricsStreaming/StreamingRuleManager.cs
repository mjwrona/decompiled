// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsStreaming.StreamingRuleManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.MetricsStreaming.Exceptions;
using Microsoft.Cloud.Metrics.Client.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.Cloud.Metrics.Client.MetricsStreaming
{
  public sealed class StreamingRuleManager
  {
    private const int Port = 8952;
    private const string ClientVersion = "clientVersion=1";
    private const string RulesConfigurationPathPrefix = "api/v1/streamingrules";
    private const string RulesAllowlistPathPrefix = "/api/v1/allowlist";
    private readonly ConnectionInfo connectionInfo;
    private readonly HttpClient httpClient;
    private readonly StreamingBloomfilterManager streamingBloomfilterManager;

    public StreamingRuleManager(ConnectionInfo connectionInfo)
    {
      this.connectionInfo = connectionInfo ?? throw new ArgumentNullException(nameof (connectionInfo));
      this.httpClient = HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo);
      this.streamingBloomfilterManager = new StreamingBloomfilterManager(this.connectionInfo, this.httpClient);
    }

    public StreamingBloomfilterManager BloomfilterManager => this.streamingBloomfilterManager;

    public async Task<StreamingRuleResponseItem> GetStreamingRuleAsync(string ruleId, Guid? traceId = null)
    {
      if (string.IsNullOrEmpty(ruleId))
        throw new ArgumentException("'ruleId' cannot be null or empty.", nameof (ruleId));
      string str = "api/v1/streamingrules/" + ruleId;
      return JsonConvert.DeserializeObject<StreamingRuleResponseItem>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = str,
        Query = "clientVersion=1",
        Port = 8952
      }.Uri, HttpMethod.Get, this.httpClient, (string) null, "api/v1/streamingrules", traceId: traceId).ConfigureAwait(false));
    }

    public async Task<StreamingRuleResponseItem> CreateStreamingRuleAsync(
      StreamingRuleRequest streamingRuleRequest,
      Guid? traceId = null)
    {
      if (streamingRuleRequest == null)
        throw new ArgumentException("'streamingRuleRequest' cannot be null.", nameof (streamingRuleRequest));
      return JsonConvert.DeserializeObject<StreamingRuleResponseItem>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(await this.GetEndpointForAccountAsync(streamingRuleRequest.MdmAccountInfo).ConfigureAwait(false))
      {
        Path = "api/v1/streamingrules",
        Query = "clientVersion=1",
        Port = 8952
      }.Uri, HttpMethod.Post, this.httpClient, (string) null, "api/v1/streamingrules", serializedContent: JsonConvert.SerializeObject((object) streamingRuleRequest), traceId: traceId).ConfigureAwait(false));
    }

    public async Task<StreamingRuleResponseItem> ReplaceStreamingRuleAsync(
      string ruleId,
      StreamingRuleRequest streamingRuleRequest,
      Guid? traceId = null)
    {
      if (string.IsNullOrEmpty(ruleId))
        throw new ArgumentException("'ruleId' cannot be null or empty.", nameof (ruleId));
      if (streamingRuleRequest == null)
        throw new ArgumentException("'streamingRuleRequest' cannot be null.", nameof (streamingRuleRequest));
      return JsonConvert.DeserializeObject<StreamingRuleResponseItem>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(await this.GetEndpointForAccountAsync(streamingRuleRequest.MdmAccountInfo).ConfigureAwait(false))
      {
        Path = ("api/v1/streamingrules/" + ruleId),
        Query = "clientVersion=1",
        Port = 8952
      }.Uri, HttpMethod.Put, this.httpClient, (string) null, "api/v1/streamingrules", serializedContent: JsonConvert.SerializeObject((object) streamingRuleRequest), traceId: traceId).ConfigureAwait(false));
    }

    public async Task ValidateStreamingRuleAsync(
      StreamingRuleRequest streamingRuleRequest,
      Guid? traceId = null)
    {
      if (streamingRuleRequest == null)
        throw new ArgumentException("'streamingRuleRequest' cannot be null.", nameof (streamingRuleRequest));
      Uri uri = await this.GetEndpointForAccountAsync(streamingRuleRequest.MdmAccountInfo).ConfigureAwait(false);
      StreamingRuleValidationRequest validationRequest = new StreamingRuleValidationRequest()
      {
        StreamingRuleRequest = streamingRuleRequest
      };
      UriBuilder uriBuilder = new UriBuilder(uri)
      {
        Path = "api/v1/streamingrules",
        Query = "clientVersion=1",
        Port = 8952
      };
      string serializedContent = JsonConvert.SerializeObject((object) validationRequest);
      try
      {
        string str = await HttpClientHelper.GetResponseAsStringAsync(uriBuilder.Uri, HttpMethod.Put, this.httpClient, (string) null, "api/v1/streamingrules", serializedContent: serializedContent, traceId: traceId).ConfigureAwait(false);
      }
      catch (MetricsClientException ex)
      {
        throw new InvalidStreamingRuleException(ex.Message, (Exception) ex, traceId ?? Guid.Empty, ex.ResponseStatusCode);
      }
    }

    public async Task DeleteStreamingRuleAsync(string ruleId, Guid? traceId = null)
    {
      if (string.IsNullOrEmpty(ruleId))
        throw new ArgumentException("'ruleId' cannot be null or empty.", nameof (ruleId));
      string str = await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = ("api/v1/streamingrules/" + ruleId),
        Query = "clientVersion=1",
        Port = 8952
      }.Uri, HttpMethod.Delete, this.httpClient, (string) null, "api/v1/streamingrules", traceId: traceId).ConfigureAwait(false);
    }

    public async Task<ListStreamingRuleResponse> QueryStreamingRulesAsync(
      Dictionary<string, string> filters,
      string continuationToken = null,
      Guid? traceId = null)
    {
      if (filters == null || filters.Count == 0)
        throw new ArgumentException("'filters' should contain at least one filter.", nameof (filters));
      string str = string.Join("&", filters.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (filter => "filters[" + filter.Key + "]=" + HttpUtility.UrlEncode(filter.Value))).ToArray<string>());
      if (!string.IsNullOrEmpty(continuationToken))
        str = str + "&continuationToken=" + HttpUtility.UrlEncode(continuationToken);
      return JsonConvert.DeserializeObject<ListStreamingRuleResponse>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = "api/v1/streamingrules",
        Query = ("clientVersion=1&" + str),
        Port = 8952
      }.Uri, HttpMethod.Get, this.httpClient, (string) null, "api/v1/streamingrules", traceId: traceId).ConfigureAwait(false));
    }

    public async Task<IReadOnlyList<RuleOnboardingAllowDefinition>> GetOnboardingAllowlistAsync(
      Guid? traceId = null)
    {
      return JsonConvert.DeserializeObject<IReadOnlyList<RuleOnboardingAllowDefinition>>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = "/api/v1/allowlist",
        Query = "clientVersion=1",
        Port = 8952
      }.Uri, HttpMethod.Get, this.httpClient, (string) null, "/api/v1/allowlist", traceId: traceId).ConfigureAwait(false));
    }

    public async Task<StreamingRuleStatusResponse> GetStreamingRuleStatusAsync(
      string ruleId,
      Guid? traceId = null)
    {
      if (string.IsNullOrEmpty(ruleId))
        throw new ArgumentException("'ruleId' cannot be null or empty.", nameof (ruleId));
      string str = "api/v1/streamingrules/" + ruleId + "/status";
      return JsonConvert.DeserializeObject<StreamingRuleStatusResponse>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.GetGlobalEndpoint())
      {
        Path = str,
        Query = "clientVersion=1",
        Port = 8952
      }.Uri, HttpMethod.Get, this.httpClient, (string) null, "api/v1/streamingrules", traceId: traceId).ConfigureAwait(false));
    }

    private bool IsWildcardAccount(string mdmAccountInfo)
    {
      string str = !string.IsNullOrEmpty(mdmAccountInfo) ? mdmAccountInfo.Trim() : throw new ArgumentNullException(nameof (mdmAccountInfo));
      return str[str.Length - 1] == '*';
    }

    private async Task<Uri> GetEndpointForAccountAsync(string mdmAccountInfo)
    {
      if (string.IsNullOrEmpty(mdmAccountInfo))
        throw new ArgumentNullException(nameof (mdmAccountInfo));
      return this.IsWildcardAccount(mdmAccountInfo) ? this.connectionInfo.GetGlobalEndpoint() : await this.connectionInfo.GetEndpointAsync(mdmAccountInfo).ConfigureAwait(false);
    }
  }
}
