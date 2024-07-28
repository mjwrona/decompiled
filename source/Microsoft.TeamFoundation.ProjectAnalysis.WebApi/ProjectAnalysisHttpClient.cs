// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.WebApi.ProjectAnalysisHttpClient
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F7D1B59D-FE5E-4B10-AAB1-4E05CDFBD17B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.ProjectAnalysis.WebApi
{
  [ResourceArea("7658FA33-B1BF-4580-990F-FAC5896773D3")]
  public class ProjectAnalysisHttpClient : VssHttpClientBase
  {
    public ProjectAnalysisHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ProjectAnalysisHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ProjectAnalysisHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ProjectAnalysisHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ProjectAnalysisHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<ProjectLanguageAnalytics> GetProjectLanguageAnalyticsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ProjectLanguageAnalytics>(new HttpMethod("GET"), new Guid("5b02a779-1867-433f-90b7-d23ed5e33e57"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ProjectLanguageAnalytics> GetProjectLanguageAnalyticsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ProjectLanguageAnalytics>(new HttpMethod("GET"), new Guid("5b02a779-1867-433f-90b7-d23ed5e33e57"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ProjectActivityMetrics> GetProjectActivityMetricsAsync(
      string project,
      DateTime fromDate,
      AggregationType aggregationType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e40ae584-9ea6-4f06-a7c7-6284651b466b");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (fromDate), fromDate);
      keyValuePairList.Add(nameof (aggregationType), aggregationType.ToString());
      return this.SendAsync<ProjectActivityMetrics>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ProjectActivityMetrics> GetProjectActivityMetricsAsync(
      Guid project,
      DateTime fromDate,
      AggregationType aggregationType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e40ae584-9ea6-4f06-a7c7-6284651b466b");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (fromDate), fromDate);
      keyValuePairList.Add(nameof (aggregationType), aggregationType.ToString());
      return this.SendAsync<ProjectActivityMetrics>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<RepositoryActivityMetrics>> GetGitRepositoriesActivityMetricsAsync(
      string project,
      DateTime fromDate,
      AggregationType aggregationType,
      int skip,
      int top,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("df7fbbca-630a-40e3-8aa3-7a3faf66947e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (fromDate), fromDate);
      keyValuePairList.Add(nameof (aggregationType), aggregationType.ToString());
      keyValuePairList.Add("$skip", skip.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add("$top", top.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<RepositoryActivityMetrics>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<RepositoryActivityMetrics>> GetGitRepositoriesActivityMetricsAsync(
      Guid project,
      DateTime fromDate,
      AggregationType aggregationType,
      int skip,
      int top,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("df7fbbca-630a-40e3-8aa3-7a3faf66947e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (fromDate), fromDate);
      keyValuePairList.Add(nameof (aggregationType), aggregationType.ToString());
      keyValuePairList.Add("$skip", skip.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add("$top", top.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<RepositoryActivityMetrics>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<RepositoryActivityMetrics> GetRepositoryActivityMetricsAsync(
      string project,
      Guid repositoryId,
      DateTime fromDate,
      AggregationType aggregationType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("df7fbbca-630a-40e3-8aa3-7a3faf66947e");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (fromDate), fromDate);
      keyValuePairList.Add(nameof (aggregationType), aggregationType.ToString());
      return this.SendAsync<RepositoryActivityMetrics>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<RepositoryActivityMetrics> GetRepositoryActivityMetricsAsync(
      Guid project,
      Guid repositoryId,
      DateTime fromDate,
      AggregationType aggregationType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("df7fbbca-630a-40e3-8aa3-7a3faf66947e");
      object routeValues = (object) new
      {
        project = project,
        repositoryId = repositoryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (fromDate), fromDate);
      keyValuePairList.Add(nameof (aggregationType), aggregationType.ToString());
      return this.SendAsync<RepositoryActivityMetrics>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
