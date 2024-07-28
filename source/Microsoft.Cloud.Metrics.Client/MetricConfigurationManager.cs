// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricConfigurationManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Utility;
using Microsoft.Online.Metrics.Serialization.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client
{
  internal sealed class MetricConfigurationManager
  {
    public readonly string ConfigRelativeUrl;
    private readonly HttpClient httpClient;
    private readonly ConnectionInfo connectionInfo;

    public MetricConfigurationManager(ConnectionInfo connectionInfo)
    {
      this.connectionInfo = connectionInfo;
      this.ConfigRelativeUrl = this.connectionInfo.GetAuthRelativeUrl("v1/config/metrics");
      this.httpClient = HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo);
    }

    public MetricConfigurationManager(ConnectionInfo connectionInfo, HttpClient httpClient)
    {
      this.connectionInfo = connectionInfo;
      this.ConfigRelativeUrl = this.connectionInfo.GetAuthRelativeUrl("v1/config/metrics");
      this.httpClient = httpClient;
    }

    public async Task<MetricConfigurationV2> Get(MetricIdentifier metricIdentifier) => JsonConvert.DeserializeObject<MetricConfigurationV2>(await HttpClientHelper.GetJsonResponse(new Uri(string.Format("{0}{1}/monitoringAccount/{2}/metricNamespace/{3}/metric/{4}", (object) await this.connectionInfo.GetEndpointAsync(((MetricIdentifier) ref metricIdentifier).MonitoringAccount).ConfigureAwait(false), (object) this.ConfigRelativeUrl, (object) ((MetricIdentifier) ref metricIdentifier).MonitoringAccount, (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricIdentifier).MetricNamespace), (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricIdentifier).MetricName))), HttpMethod.Get, this.httpClient, ((MetricIdentifier) ref metricIdentifier).MonitoringAccount, this.ConfigRelativeUrl).ConfigureAwait(false));

    public async Task Delete(MetricIdentifier metricIdentifier)
    {
      string str = await HttpClientHelper.GetJsonResponse(new Uri(string.Format("{0}{1}/monitoringAccount/{2}/metricNamespace/{3}/metric/{4}", (object) await this.connectionInfo.GetEndpointAsync(((MetricIdentifier) ref metricIdentifier).MonitoringAccount).ConfigureAwait(false), (object) this.ConfigRelativeUrl, (object) ((MetricIdentifier) ref metricIdentifier).MonitoringAccount, (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricIdentifier).MetricNamespace), (object) SpecialCharsHelper.EscapeTwice(((MetricIdentifier) ref metricIdentifier).MetricName))), HttpMethod.Delete, this.httpClient, ((MetricIdentifier) ref metricIdentifier).MonitoringAccount, this.ConfigRelativeUrl).ConfigureAwait(false);
    }

    public async Task Post(MetricConfigurationV2 configuration)
    {
      string str = await HttpClientHelper.GetJsonResponse(new Uri(string.Format("{0}{1}/monitoringAccount/{2}/metricNamespace/{3}/metric/{4}", (object) await this.connectionInfo.GetEndpointAsync(((MetricConfigurationV2) ref configuration).MonitoringAccount).ConfigureAwait(false), (object) this.ConfigRelativeUrl, (object) ((MetricConfigurationV2) ref configuration).MonitoringAccount, (object) SpecialCharsHelper.EscapeTwice(((MetricConfigurationV2) ref configuration).MetricNamespace), (object) SpecialCharsHelper.EscapeTwice(((MetricConfigurationV2) ref configuration).MetricName))), HttpMethod.Post, this.httpClient, ((MetricConfigurationV2) ref configuration).MonitoringAccount, this.ConfigRelativeUrl, (object) configuration).ConfigureAwait(false);
    }
  }
}
