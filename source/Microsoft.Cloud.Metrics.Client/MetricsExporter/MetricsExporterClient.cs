// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsExporter.MetricsExporterClient
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Utility;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.MetricsExporter
{
  public sealed class MetricsExporterClient
  {
    private const int Port = 8902;
    private const string ClientVersion = "clientVersion=2";
    private const string JobManagementPath = "/api/v1.0/jobs";
    private readonly ConnectionInfo connectionInfo;
    private readonly HttpClient httpClient;

    public MetricsExporterClient(ConnectionInfo connectionInfo, string authHeaderValue)
    {
      this.connectionInfo = connectionInfo ?? throw new ArgumentNullException(nameof (connectionInfo));
      this.httpClient = HttpClientHelper.CreateHttpClientWithAuthInfo(connectionInfo, authHeaderValue);
    }

    public async Task<JobDefinitionResponse> AddJobAsync(
      JobDefinitionRequest jobRequest,
      Guid? traceId = null)
    {
      if (jobRequest == null)
        throw new ArgumentNullException(nameof (jobRequest));
      return JsonConvert.DeserializeObject<JobDefinitionResponse>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.Endpoint)
      {
        Path = "/api/v1.0/jobs",
        Query = "clientVersion=2",
        Port = 8902
      }.Uri, HttpMethod.Post, this.httpClient, (string) null, "/api/v1.0/jobs", serializedContent: JsonConvert.SerializeObject((object) jobRequest), traceId: traceId).ConfigureAwait(false));
    }

    public async Task<JobDefinitionResponse> GetJobAsync(string jobId, Guid? traceId = null)
    {
      if (string.IsNullOrEmpty(jobId))
        throw new ArgumentException("'jobId' cannot be null or empty.", nameof (jobId));
      string str = "/api/v1.0/jobs/" + jobId;
      return JsonConvert.DeserializeObject<JobDefinitionResponse>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.Endpoint)
      {
        Path = str,
        Query = "clientVersion=2",
        Port = 8902
      }.Uri, HttpMethod.Get, this.httpClient, (string) null, "/api/v1.0/jobs", traceId: traceId).ConfigureAwait(false));
    }

    public async Task UpdateJobAsync(string jobId, JobDefinitionRequest jobRequest, Guid? traceId = null)
    {
      if (string.IsNullOrEmpty(jobId))
        throw new ArgumentException("'jobId' cannot be null or empty.", nameof (jobId));
      if (jobRequest == null)
        throw new ArgumentException("'jobRequest' cannot be null.", nameof (jobRequest));
      string str = "/api/v1.0/jobs/" + jobId;
      Tuple<string, HttpResponseMessage> tuple = await HttpClientHelper.GetResponse(new UriBuilder(this.connectionInfo.Endpoint)
      {
        Path = str,
        Query = "clientVersion=2",
        Port = 8902
      }.Uri, HttpMethod.Put, this.httpClient, (string) null, "/api/v1.0/jobs", serializedContent: JsonConvert.SerializeObject((object) jobRequest), traceId: traceId).ConfigureAwait(false);
    }

    public async Task DeleteJobAsync(string jobId, Guid? traceId = null)
    {
      if (string.IsNullOrEmpty(jobId))
        throw new ArgumentException("'jobId' cannot be null or empty.", nameof (jobId));
      string str = "/api/v1.0/jobs/" + jobId;
      Tuple<string, HttpResponseMessage> tuple = await HttpClientHelper.GetResponse(new UriBuilder(this.connectionInfo.Endpoint)
      {
        Path = str,
        Query = "clientVersion=2",
        Port = 8902
      }.Uri, HttpMethod.Delete, this.httpClient, (string) null, "/api/v1.0/jobs", traceId: traceId).ConfigureAwait(false);
    }

    public async Task<JobDefinitionDetailsEntityResponse[]> GetAllJobsAsync(
      string stampId,
      Guid? traceId = null)
    {
      if (string.IsNullOrEmpty(stampId))
        throw new ArgumentException("'stampId' cannot be null or empty.", nameof (stampId));
      string str = "/api/v1.0/jobs";
      return JsonConvert.DeserializeObject<JobDefinitionDetailsEntityResponse[]>(await HttpClientHelper.GetResponseAsStringAsync(new UriBuilder(this.connectionInfo.Endpoint)
      {
        Path = str,
        Query = ("stampId=" + stampId + "&clientVersion=2"),
        Port = 8902
      }.Uri, HttpMethod.Get, this.httpClient, (string) null, "/api/v1.0/jobs", traceId: traceId).ConfigureAwait(false));
    }
  }
}
