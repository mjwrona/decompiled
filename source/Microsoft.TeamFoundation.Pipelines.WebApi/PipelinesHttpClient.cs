// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.WebApi.PipelinesHttpClient
// Assembly: Microsoft.TeamFoundation.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29F2A1B3-A3F7-4291-91FA-6C4508EECB65
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Pipelines.WebApi
{
  [ResourceArea("2e0bf237-8973-4ec9-a581-9c3d679d1776")]
  public class PipelinesHttpClient : VssHttpClientBase
  {
    public PipelinesHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PipelinesHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PipelinesHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PipelinesHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PipelinesHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<List<ConfigurationFile>> GetConfigurationsAsync(
      string project,
      string repositoryType = null,
      string repositoryId = null,
      string branch = null,
      Guid? serviceConnectionId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8fc87684-9ebc-4c37-ab92-f4ac4a58cb3a");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repositoryType != null)
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (repositoryId != null)
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (branch != null)
        keyValuePairList.Add(nameof (branch), branch);
      if (serviceConnectionId.HasValue)
        keyValuePairList.Add(nameof (serviceConnectionId), serviceConnectionId.Value.ToString());
      return this.SendAsync<List<ConfigurationFile>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<ConfigurationFile>> GetConfigurationsAsync(
      Guid project,
      string repositoryType = null,
      string repositoryId = null,
      string branch = null,
      Guid? serviceConnectionId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8fc87684-9ebc-4c37-ab92-f4ac4a58cb3a");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repositoryType != null)
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (repositoryId != null)
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (branch != null)
        keyValuePairList.Add(nameof (branch), branch);
      if (serviceConnectionId.HasValue)
        keyValuePairList.Add(nameof (serviceConnectionId), serviceConnectionId.Value.ToString());
      return this.SendAsync<List<ConfigurationFile>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete("This method is no longer supported. Call CreateProjectConnection instead.")]
    public Task<Operation> CreateConnectionAsync(
      CreatePipelineConnectionInputs createConnectionInputs,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("00df4879-9216-45d5-b38d-4a487b626b2c");
      HttpContent httpContent = (HttpContent) new ObjectContent<CreatePipelineConnectionInputs>(createConnectionInputs, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Operation>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<PipelineConnection> CreateProjectConnectionAsync(
      CreatePipelineConnectionInputs createConnectionInputs,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("00df4879-9216-45d5-b38d-4a487b626b2c");
      HttpContent httpContent = (HttpContent) new ObjectContent<CreatePipelineConnectionInputs>(createConnectionInputs, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (project), project);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PipelineConnection>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<List<DetectedBuildFramework>> GetDetectedBuildFrameworksAsync(
      string project,
      string repositoryType = null,
      string repositoryId = null,
      string branch = null,
      BuildFrameworkDetectionType? detectionType = null,
      Guid? serviceConnectionId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("29a30bab-9efb-4652-bf1b-9269baca0980");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repositoryType != null)
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (repositoryId != null)
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (branch != null)
        keyValuePairList.Add(nameof (branch), branch);
      if (detectionType.HasValue)
        keyValuePairList.Add(nameof (detectionType), detectionType.Value.ToString());
      if (serviceConnectionId.HasValue)
        keyValuePairList.Add(nameof (serviceConnectionId), serviceConnectionId.Value.ToString());
      return this.SendAsync<List<DetectedBuildFramework>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<DetectedBuildFramework>> GetDetectedBuildFrameworksAsync(
      Guid project,
      string repositoryType = null,
      string repositoryId = null,
      string branch = null,
      BuildFrameworkDetectionType? detectionType = null,
      Guid? serviceConnectionId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("29a30bab-9efb-4652-bf1b-9269baca0980");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repositoryType != null)
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (repositoryId != null)
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (branch != null)
        keyValuePairList.Add(nameof (branch), branch);
      if (detectionType.HasValue)
        keyValuePairList.Add(nameof (detectionType), detectionType.Value.ToString());
      if (serviceConnectionId.HasValue)
        keyValuePairList.Add(nameof (serviceConnectionId), serviceConnectionId.Value.ToString());
      return this.SendAsync<List<DetectedBuildFramework>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<CreatedResources> CreateResourcesAsync(
      IDictionary<string, ResourceCreationParameter> creationParameters,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("43201899-7690-4870-9c79-ab69605f21ed");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IDictionary<string, ResourceCreationParameter>>(creationParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CreatedResources>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<CreatedResources> CreateResourcesAsync(
      IDictionary<string, ResourceCreationParameter> creationParameters,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("43201899-7690-4870-9c79-ab69605f21ed");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IDictionary<string, ResourceCreationParameter>>(creationParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CreatedResources>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
