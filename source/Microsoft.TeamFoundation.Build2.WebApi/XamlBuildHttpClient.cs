// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.XamlBuildHttpClient
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [ResourceArea("5D6898BB-45EC-463F-95F9-54D49C71752E")]
  public sealed class XamlBuildHttpClient : VssHttpClientBase
  {
    public XamlBuildHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public XamlBuildHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public XamlBuildHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public XamlBuildHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public XamlBuildHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<BuildArtifact> GetArtifactAsync(
      string project,
      int buildId,
      string artifactName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(artifactName))
        keyValuePairList.Add(nameof (artifactName), artifactName);
      return this.SendAsync<BuildArtifact>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<BuildArtifact> GetArtifactAsync(
      Guid project,
      int buildId,
      string artifactName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(artifactName))
        keyValuePairList.Add(nameof (artifactName), artifactName);
      return this.SendAsync<BuildArtifact>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<BuildArtifact> GetArtifactAsync(
      int buildId,
      string artifactName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new{ buildId = buildId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(artifactName))
        keyValuePairList.Add(nameof (artifactName), artifactName);
      return this.SendAsync<BuildArtifact>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<Stream> GetArtifactContentZipAsync(
      string project,
      int buildId,
      string artifactName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      XamlBuildHttpClient xamlBuildHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(artifactName))
        keyValuePairList.Add(nameof (artifactName), artifactName);
      HttpRequestMessage message = await xamlBuildHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = await xamlBuildHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetArtifactContentZipAsync(
      Guid project,
      int buildId,
      string artifactName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      XamlBuildHttpClient xamlBuildHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(artifactName))
        keyValuePairList.Add(nameof (artifactName), artifactName);
      HttpRequestMessage message = await xamlBuildHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = await xamlBuildHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetArtifactContentZipAsync(
      int buildId,
      string artifactName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      XamlBuildHttpClient xamlBuildHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new{ buildId = buildId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(artifactName))
        keyValuePairList.Add(nameof (artifactName), artifactName);
      HttpRequestMessage message = await xamlBuildHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = await xamlBuildHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public Task<List<BuildArtifact>> GetArtifactsAsync(
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new{ buildId = buildId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      return this.SendAsync<List<BuildArtifact>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<BuildArtifact>> GetArtifactsAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      return this.SendAsync<List<BuildArtifact>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<BuildArtifact>> GetArtifactsAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      return this.SendAsync<List<BuildArtifact>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task DeleteBuildAsync(
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new{ buildId = buildId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      return (Task) this.SendAsync(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task DeleteBuildAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      return (Task) this.SendAsync(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task DeleteBuildAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      return (Task) this.SendAsync(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuildAsync(
      string project,
      int buildId,
      string propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(propertyFilters))
        keyValuePairList.Add(nameof (propertyFilters), propertyFilters);
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuildAsync(
      Guid project,
      int buildId,
      string propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(propertyFilters))
        keyValuePairList.Add(nameof (propertyFilters), propertyFilters);
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuildAsync(
      int buildId,
      string propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new{ buildId = buildId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(propertyFilters))
        keyValuePairList.Add(nameof (propertyFilters), propertyFilters);
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      string project,
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      IEnumerable<string> properties = null,
      int? top = null,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption? deletedFilter = null,
      BuildQueryOrder? queryOrder = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (definitions != null && definitions.Any<int>())
        keyValuePairList.Add(nameof (definitions), string.Join<int>(",", definitions));
      if (queues != null && queues.Any<int>())
        keyValuePairList.Add(nameof (queues), string.Join<int>(",", queues));
      if (!string.IsNullOrEmpty(buildNumber))
        keyValuePairList.Add(nameof (buildNumber), buildNumber);
      if (minFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minFinishTime), minFinishTime.Value);
      if (maxFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxFinishTime), maxFinishTime.Value);
      if (!string.IsNullOrEmpty(requestedFor))
        keyValuePairList.Add(nameof (requestedFor), requestedFor);
      if (reasonFilter.HasValue)
        keyValuePairList.Add(nameof (reasonFilter), reasonFilter.Value.ToString());
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (resultFilter.HasValue)
        keyValuePairList.Add(nameof (resultFilter), resultFilter.Value.ToString());
      if (tagFilters != null && tagFilters.Any<string>())
        keyValuePairList.Add(nameof (tagFilters), string.Join(",", tagFilters));
      if (properties != null && properties.Any<string>())
        keyValuePairList.Add(nameof (properties), string.Join(",", properties));
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (maxBuildsPerDefinition.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxBuildsPerDefinition.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxBuildsPerDefinition), str);
      }
      if (deletedFilter.HasValue)
        keyValuePairList.Add(nameof (deletedFilter), deletedFilter.Value.ToString());
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      Guid project,
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      IEnumerable<string> properties = null,
      int? top = null,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption? deletedFilter = null,
      BuildQueryOrder? queryOrder = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (definitions != null && definitions.Any<int>())
        keyValuePairList.Add(nameof (definitions), string.Join<int>(",", definitions));
      if (queues != null && queues.Any<int>())
        keyValuePairList.Add(nameof (queues), string.Join<int>(",", queues));
      if (!string.IsNullOrEmpty(buildNumber))
        keyValuePairList.Add(nameof (buildNumber), buildNumber);
      if (minFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minFinishTime), minFinishTime.Value);
      if (maxFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxFinishTime), maxFinishTime.Value);
      if (!string.IsNullOrEmpty(requestedFor))
        keyValuePairList.Add(nameof (requestedFor), requestedFor);
      if (reasonFilter.HasValue)
        keyValuePairList.Add(nameof (reasonFilter), reasonFilter.Value.ToString());
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (resultFilter.HasValue)
        keyValuePairList.Add(nameof (resultFilter), resultFilter.Value.ToString());
      if (tagFilters != null && tagFilters.Any<string>())
        keyValuePairList.Add(nameof (tagFilters), string.Join(",", tagFilters));
      if (properties != null && properties.Any<string>())
        keyValuePairList.Add(nameof (properties), string.Join(",", properties));
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (maxBuildsPerDefinition.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxBuildsPerDefinition.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxBuildsPerDefinition), str);
      }
      if (deletedFilter.HasValue)
        keyValuePairList.Add(nameof (deletedFilter), deletedFilter.Value.ToString());
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      IEnumerable<string> properties = null,
      int? top = null,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption? deletedFilter = null,
      BuildQueryOrder? queryOrder = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (definitions != null && definitions.Any<int>())
        keyValuePairList.Add(nameof (definitions), string.Join<int>(",", definitions));
      if (queues != null && queues.Any<int>())
        keyValuePairList.Add(nameof (queues), string.Join<int>(",", queues));
      if (!string.IsNullOrEmpty(buildNumber))
        keyValuePairList.Add(nameof (buildNumber), buildNumber);
      if (minFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minFinishTime), minFinishTime.Value);
      if (maxFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxFinishTime), maxFinishTime.Value);
      if (!string.IsNullOrEmpty(requestedFor))
        keyValuePairList.Add(nameof (requestedFor), requestedFor);
      if (reasonFilter.HasValue)
        keyValuePairList.Add(nameof (reasonFilter), reasonFilter.Value.ToString());
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (resultFilter.HasValue)
        keyValuePairList.Add(nameof (resultFilter), resultFilter.Value.ToString());
      if (tagFilters != null && tagFilters.Any<string>())
        keyValuePairList.Add(nameof (tagFilters), string.Join(",", tagFilters));
      if (properties != null && properties.Any<string>())
        keyValuePairList.Add(nameof (properties), string.Join(",", properties));
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (maxBuildsPerDefinition.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxBuildsPerDefinition.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxBuildsPerDefinition), str);
      }
      if (deletedFilter.HasValue)
        keyValuePairList.Add(nameof (deletedFilter), deletedFilter.Value.ToString());
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, version: new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Microsoft.TeamFoundation.Build.WebApi.Build> QueueBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      bool? ignoreWarnings = null,
      string checkInTicket = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add("type", "Xaml");
      if (ignoreWarnings.HasValue)
        collection.Add(nameof (ignoreWarnings), ignoreWarnings.Value.ToString());
      if (!string.IsNullOrEmpty(checkInTicket))
        collection.Add(nameof (checkInTicket), checkInTicket);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("2.2");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Microsoft.TeamFoundation.Build.WebApi.Build> QueueBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string project,
      bool? ignoreWarnings = null,
      string checkInTicket = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add("type", "Xaml");
      if (ignoreWarnings.HasValue)
        collection.Add(nameof (ignoreWarnings), ignoreWarnings.Value.ToString());
      if (!string.IsNullOrEmpty(checkInTicket))
        collection.Add(nameof (checkInTicket), checkInTicket);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("2.2");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<Microsoft.TeamFoundation.Build.WebApi.Build> QueueBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      Guid project,
      bool? ignoreWarnings = null,
      string checkInTicket = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add("type", "Xaml");
      if (ignoreWarnings.HasValue)
        collection.Add(nameof (ignoreWarnings), ignoreWarnings.Value.ToString());
      if (!string.IsNullOrEmpty(checkInTicket))
        collection.Add(nameof (checkInTicket), checkInTicket);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("2.2");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<Microsoft.TeamFoundation.Build.WebApi.Build> UpdateBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ buildId = buildId };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add("type", "Xaml");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("2.2");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<Microsoft.TeamFoundation.Build.WebApi.Build> UpdateBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add("type", "Xaml");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("2.2");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<Microsoft.TeamFoundation.Build.WebApi.Build> UpdateBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add("type", "Xaml");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("2.2");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<List<Change>> GetBuildChangesAsync(
      string project,
      int buildId,
      string continuationToken = null,
      int? top = null,
      bool? includeSourceChange = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("54572c7b-bbd3-45d4-80dc-28be08941620");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeSourceChange.HasValue)
        keyValuePairList.Add(nameof (includeSourceChange), includeSourceChange.Value.ToString());
      return this.SendAsync<List<Change>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Change>> GetBuildChangesAsync(
      Guid project,
      int buildId,
      string continuationToken = null,
      int? top = null,
      bool? includeSourceChange = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("54572c7b-bbd3-45d4-80dc-28be08941620");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeSourceChange.HasValue)
        keyValuePairList.Add(nameof (includeSourceChange), includeSourceChange.Value.ToString());
      return this.SendAsync<List<Change>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<BuildController> GetBuildControllerAsync(
      int controllerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildController>(new HttpMethod("GET"), new Guid("fcac1932-2ee1-437f-9b6f-7f696be858f6"), (object) new
      {
        controllerId = controllerId
      }, new ApiResourceVersion("2.2"), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<BuildController>> GetBuildControllersAsync(
      string name = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fcac1932-2ee1-437f-9b6f-7f696be858f6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      return this.SendAsync<List<BuildController>>(method, locationId, version: new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<DefinitionReference> GetDefinitionAsync(
      string project,
      int definitionId,
      int? revision = null,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<DefinitionReference>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<DefinitionReference> GetDefinitionAsync(
      Guid project,
      int definitionId,
      int? revision = null,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<DefinitionReference>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<DefinitionReference> GetDefinitionAsync(
      int definitionId,
      int? revision = null,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new
      {
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<DefinitionReference>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<DefinitionReference>> GetDefinitionsAsync(
      string project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<DefinitionReference>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<DefinitionReference>> GetDefinitionsAsync(
      Guid project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<DefinitionReference>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<DefinitionReference>> GetDefinitionsAsync(
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<DefinitionReference>>(method, locationId, version: new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Deployment>> GetBuildDeploymentsAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Deployment>>(new HttpMethod("GET"), new Guid("f275be9a-556a-4ee9-b72f-f9c8370ccaee"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion("2.2"), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Deployment>> GetBuildDeploymentsAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Deployment>>(new HttpMethod("GET"), new Guid("f275be9a-556a-4ee9-b72f-f9c8370ccaee"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion("2.2"), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<Stream> GetBuildLogAsync(
      string project,
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      XamlBuildHttpClient xamlBuildHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId,
        logId = logId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (startLine.HasValue)
        keyValuePairList.Add(nameof (startLine), startLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (endLine.HasValue)
        keyValuePairList.Add(nameof (endLine), endLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpRequestMessage message = await xamlBuildHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = await xamlBuildHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetBuildLogAsync(
      Guid project,
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      XamlBuildHttpClient xamlBuildHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId,
        logId = logId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (startLine.HasValue)
        keyValuePairList.Add(nameof (startLine), startLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (endLine.HasValue)
        keyValuePairList.Add(nameof (endLine), endLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpRequestMessage message = await xamlBuildHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = await xamlBuildHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public Task<List<BuildLog>> GetBuildLogsAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      return this.SendAsync<List<BuildLog>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<BuildLog>> GetBuildLogsAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      return this.SendAsync<List<BuildLog>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<Stream> GetBuildLogsZipAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      XamlBuildHttpClient xamlBuildHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      HttpRequestMessage message = await xamlBuildHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = await xamlBuildHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetBuildLogsZipAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      XamlBuildHttpClient xamlBuildHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      HttpRequestMessage message = await xamlBuildHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = await xamlBuildHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public Task<BuildReportMetadata> GetBuildReportAsync(
      string project,
      int buildId,
      string type = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("45bcaa88-67e1-4042-a035-56d3b4a7d44c");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("definitionType", "Xaml");
      if (!string.IsNullOrEmpty(type))
        keyValuePairList.Add(nameof (type), type);
      return this.SendAsync<BuildReportMetadata>(method, locationId, routeValues, new ApiResourceVersion("2.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<BuildReportMetadata> GetBuildReportAsync(
      Guid project,
      int buildId,
      string type = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("45bcaa88-67e1-4042-a035-56d3b4a7d44c");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("definitionType", "Xaml");
      if (!string.IsNullOrEmpty(type))
        keyValuePairList.Add(nameof (type), type);
      return this.SendAsync<BuildReportMetadata>(method, locationId, routeValues, new ApiResourceVersion("2.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<Stream> GetBuildReportHtmlContentAsync(
      string project,
      int buildId,
      string type = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      XamlBuildHttpClient xamlBuildHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("45bcaa88-67e1-4042-a035-56d3b4a7d44c");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("definitionType", "Xaml");
      if (!string.IsNullOrEmpty(type))
        keyValuePairList.Add(nameof (type), type);
      HttpRequestMessage message = await xamlBuildHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("2.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/html").ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = await xamlBuildHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetBuildReportHtmlContentAsync(
      Guid project,
      int buildId,
      string type = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      XamlBuildHttpClient xamlBuildHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("45bcaa88-67e1-4042-a035-56d3b4a7d44c");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("definitionType", "Xaml");
      if (!string.IsNullOrEmpty(type))
        keyValuePairList.Add(nameof (type), type);
      HttpRequestMessage message = await xamlBuildHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("2.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/html").ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = await xamlBuildHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public Task<List<ResourceRef>> GetBuildWorkItemsRefsAsync(
      string project,
      int buildId,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5a21f5d2-5642-47e4-a0bd-1356e6731bee");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<ResourceRef>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<ResourceRef>> GetBuildWorkItemsRefsAsync(
      Guid project,
      int buildId,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5a21f5d2-5642-47e4-a0bd-1356e6731bee");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("type", "Xaml");
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<ResourceRef>>(method, locationId, routeValues, new ApiResourceVersion("2.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<ResourceRef>> GetBuildWorkItemsRefsFromCommitsAsync(
      IEnumerable<string> commitIds,
      string project,
      int buildId,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5a21f5d2-5642-47e4-a0bd-1356e6731bee");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(commitIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add("type", "Xaml");
      if (top.HasValue)
        collection.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("2.2");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ResourceRef>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<List<ResourceRef>> GetBuildWorkItemsRefsFromCommitsAsync(
      IEnumerable<string> commitIds,
      Guid project,
      int buildId,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5a21f5d2-5642-47e4-a0bd-1356e6731bee");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(commitIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add("type", "Xaml");
      if (top.HasValue)
        collection.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("2.2");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ResourceRef>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }
  }
}
