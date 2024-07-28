// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricEnrichmentRuleManagement.StampLevelMetricEnrichmentRuleManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.MetricEnrichmentRuleManagement
{
  public sealed class StampLevelMetricEnrichmentRuleManager
  {
    private const string ClientVersion = "clientVersion=1";
    private readonly ConnectionInfo connectionInfo;
    private readonly HttpClient httpClient;
    private readonly string configurationUrlPrefix;
    private readonly JsonSerializerSettings serializerSettings;

    public StampLevelMetricEnrichmentRuleManager(ConnectionInfo connectionInfo)
    {
      this.connectionInfo = connectionInfo != null ? connectionInfo : throw new ArgumentNullException(nameof (connectionInfo));
      this.configurationUrlPrefix = this.connectionInfo.GetAuthRelativeUrl("v1/config/enrichmentrules/");
      this.httpClient = HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo);
      this.serializerSettings = new JsonSerializerSettings()
      {
        TypeNameHandling = TypeNameHandling.Auto
      };
    }

    public async Task<IReadOnlyList<MetricEnrichmentRule>> GetAllAsync()
    {
      string path = this.configurationUrlPrefix + "getAll";
      IReadOnlyList<MetricEnrichmentRule> allAsync = (IReadOnlyList<MetricEnrichmentRule>) JsonConvert.DeserializeObject<List<MetricEnrichmentRule>>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(await this.connectionInfo.GetEndpointAsync(string.Empty).ConfigureAwait(false))
      {
        Path = path,
        Query = "clientVersion=1"
      }.Uri, HttpMethod.Get, this.httpClient, string.Empty, this.configurationUrlPrefix).ConfigureAwait(false), this.serializerSettings);
      path = (string) null;
      return allAsync;
    }

    public async Task SaveAsync(MetricEnrichmentRule rule)
    {
      string message = rule != null ? rule.Validate() : throw new ArgumentNullException(nameof (rule));
      if (!string.IsNullOrEmpty(message))
        throw new ArgumentException(message);
      if (!rule.MonitoringAccountFilter.Equals("*"))
        throw new ArgumentException("Monitoring account needs to be * as this is stamp level rule.");
      string path = this.configurationUrlPrefix ?? "";
      UriBuilder uriBuilder = new UriBuilder(await this.connectionInfo.GetEndpointAsync(string.Empty).ConfigureAwait(false));
      uriBuilder.Path = path;
      uriBuilder.Query = "clientVersion=1";
      string serializedContent = JsonConvert.SerializeObject((object) rule, Formatting.Indented, this.serializerSettings);
      string str = await HttpClientHelper.GetResponseAsStringAsync(uriBuilder.Uri, HttpMethod.Post, this.httpClient, string.Empty, this.configurationUrlPrefix, serializedContent: serializedContent).ConfigureAwait(false);
      path = (string) null;
    }

    public async Task DeleteAsync(MetricEnrichmentRule rule)
    {
      string message = rule != null ? rule.Validate() : throw new ArgumentNullException(nameof (rule));
      if (!string.IsNullOrEmpty(message))
        throw new ArgumentException(message);
      string path = this.configurationUrlPrefix ?? "";
      UriBuilder uriBuilder = new UriBuilder(await this.connectionInfo.GetEndpointAsync(string.Empty).ConfigureAwait(false));
      uriBuilder.Path = path;
      uriBuilder.Query = "clientVersion=1";
      string serializedContent = JsonConvert.SerializeObject((object) rule, Formatting.Indented, this.serializerSettings);
      string str = await HttpClientHelper.GetResponseAsStringAsync(uriBuilder.Uri, HttpMethod.Delete, this.httpClient, string.Empty, this.configurationUrlPrefix, serializedContent: serializedContent).ConfigureAwait(false);
      path = (string) null;
    }
  }
}
