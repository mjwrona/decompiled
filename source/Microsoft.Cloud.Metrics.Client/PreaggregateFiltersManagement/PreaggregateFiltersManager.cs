// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.PreaggregateFiltersManagement.PreaggregateFiltersManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.PreaggregateFiltersManagement
{
  public sealed class PreaggregateFiltersManager
  {
    private readonly ConnectionInfo connectionInfo;
    private readonly HttpClient httpClient;
    private readonly string configurationUrlPrefix;

    public PreaggregateFiltersManager(ConnectionInfo connectionInfo)
    {
      this.connectionInfo = connectionInfo != null ? connectionInfo : throw new ArgumentNullException(nameof (connectionInfo));
      this.configurationUrlPrefix = this.connectionInfo.GetAuthRelativeUrl("v1/config/preaggregate/dimensionfilters/");
      this.httpClient = HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo);
    }

    public async Task<PreaggregateFilters> GetPreaggregateFiltersAsync(
      string monitoringAccount,
      string metricNamespace,
      string metricName,
      IEnumerable<string> preaggregateDimensionNames,
      int count,
      int pageOffset)
    {
      if (string.IsNullOrWhiteSpace(monitoringAccount))
        throw new ArgumentNullException(nameof (monitoringAccount));
      string serializedArguments = JsonConvert.SerializeObject((object) new RawPreaggregateFilterQueryArguments(monitoringAccount, metricNamespace, metricName, preaggregateDimensionNames, count, pageOffset));
      string path = this.configurationUrlPrefix + "monitoringAccount/" + monitoringAccount + "/getfilters";
      PreaggregateFilters preaggregateFiltersAsync = JsonConvert.DeserializeObject<PreaggregateFilters>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccount).ConfigureAwait(false))
      {
        Path = path
      }.Uri, HttpMethod.Post, this.httpClient, monitoringAccount, this.configurationUrlPrefix, serializedContent: serializedArguments).ConfigureAwait(false));
      serializedArguments = (string) null;
      path = (string) null;
      return preaggregateFiltersAsync;
    }

    public async Task AddPreaggregateFilters(
      string monitoringAccount,
      PreaggregateFilters preaggregateFilters)
    {
      if (string.IsNullOrEmpty(monitoringAccount))
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (preaggregateFilters == null)
        throw new ArgumentNullException(nameof (preaggregateFilters));
      string path = this.configurationUrlPrefix + "monitoringAccount/" + monitoringAccount + "/addfilters";
      UriBuilder uriBuilder = new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccount).ConfigureAwait(false));
      uriBuilder.Path = path;
      uriBuilder.Query = "apiVersion=1";
      string serializedContent = JsonConvert.SerializeObject((object) preaggregateFilters);
      string str = await HttpClientHelper.GetResponseAsStringAsync(uriBuilder.Uri, HttpMethod.Post, this.httpClient, monitoringAccount, this.configurationUrlPrefix, serializedContent: serializedContent).ConfigureAwait(false);
      path = (string) null;
    }

    public async Task RemovePreaggregateFilters(
      string monitoringAccount,
      PreaggregateFilters preaggregateFilters)
    {
      if (string.IsNullOrEmpty(monitoringAccount))
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (preaggregateFilters == null)
        throw new ArgumentNullException(nameof (preaggregateFilters));
      string path = this.configurationUrlPrefix + "monitoringAccount/" + monitoringAccount + "/removefilters";
      UriBuilder uriBuilder = new UriBuilder(await this.connectionInfo.GetEndpointAsync(monitoringAccount).ConfigureAwait(false));
      uriBuilder.Path = path;
      uriBuilder.Query = "apiVersion=1";
      string serializedContent = JsonConvert.SerializeObject((object) preaggregateFilters);
      string str = await HttpClientHelper.GetResponseAsStringAsync(uriBuilder.Uri, HttpMethod.Delete, this.httpClient, monitoringAccount, this.configurationUrlPrefix, serializedContent: serializedContent).ConfigureAwait(false);
      path = (string) null;
    }
  }
}
