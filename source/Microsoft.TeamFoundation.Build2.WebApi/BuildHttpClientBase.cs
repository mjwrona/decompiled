// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildHttpClientBase
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
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
  public abstract class BuildHttpClientBase : BuildHttpClientCompatBase
  {
    public BuildHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public BuildHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public BuildHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public BuildHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public BuildHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<BuildArtifact> CreateArtifactAsync(
      BuildArtifact artifact,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildArtifact>(artifact, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 5);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildArtifact>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<BuildArtifact>>) null);
    }

    public virtual Task<BuildArtifact> CreateArtifactAsync(
      BuildArtifact artifact,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildArtifact>(artifact, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 5);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildArtifact>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<BuildArtifact>>) null);
    }

    public virtual Task<BuildArtifact> GetArtifactAsync(
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
      keyValuePairList.Add(nameof (artifactName), artifactName);
      return this.SendAsync<BuildArtifact>(method, locationId, routeValues, new ApiResourceVersion(7.2, 5), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildArtifact>>) null);
    }

    public virtual Task<BuildArtifact> GetArtifactAsync(
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
      keyValuePairList.Add(nameof (artifactName), artifactName);
      return this.SendAsync<BuildArtifact>(method, locationId, routeValues, new ApiResourceVersion(7.2, 5), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildArtifact>>) null);
    }

    public virtual async Task<Stream> GetArtifactContentZipAsync(
      string project,
      int buildId,
      string artifactName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactName), artifactName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.5"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetArtifactContentZipAsync(
      Guid project,
      int buildId,
      string artifactName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactName), artifactName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.5"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<BuildArtifact>> GetArtifactsAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BuildArtifact>>(new HttpMethod("GET"), new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion(7.2, 5), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildArtifact>>>) null);
    }

    public virtual Task<List<BuildArtifact>> GetArtifactsAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BuildArtifact>>(new HttpMethod("GET"), new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion(7.2, 5), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildArtifact>>>) null);
    }

    public virtual async Task<Stream> GetFileAsync(
      string project,
      int buildId,
      string artifactName,
      string fileId,
      string fileName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactName), artifactName);
      keyValuePairList.Add(nameof (fileId), fileId);
      keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.5"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetFileAsync(
      Guid project,
      int buildId,
      string artifactName,
      string fileId,
      string fileName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1db06c96-014e-44e1-ac91-90b2d4b3e984");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactName), artifactName);
      keyValuePairList.Add(nameof (fileId), fileId);
      keyValuePairList.Add(nameof (fileName), fileName);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.5"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<Attachment>> GetAttachmentsAsync(
      string project,
      int buildId,
      string type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Attachment>>(new HttpMethod("GET"), new Guid("f2192269-89fa-4f94-baf6-8fb128c55159"), (object) new
      {
        project = project,
        buildId = buildId,
        type = type
      }, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Attachment>>>) null);
    }

    public virtual Task<List<Attachment>> GetAttachmentsAsync(
      Guid project,
      int buildId,
      string type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Attachment>>(new HttpMethod("GET"), new Guid("f2192269-89fa-4f94-baf6-8fb128c55159"), (object) new
      {
        project = project,
        buildId = buildId,
        type = type
      }, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Attachment>>>) null);
    }

    public virtual async Task<Stream> GetAttachmentAsync(
      string project,
      int buildId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("af5122d3-3438-485e-a25a-2dbbfde84ee6");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId,
        timelineId = timelineId,
        recordId = recordId,
        type = type,
        name = name
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentAsync(
      Guid project,
      int buildId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("af5122d3-3438-485e-a25a-2dbbfde84ee6");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId,
        timelineId = timelineId,
        recordId = recordId,
        type = type,
        name = name
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<DefinitionResourceReference>> AuthorizeProjectResourcesAsync(
      IEnumerable<DefinitionResourceReference> resources,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("398c85bc-81aa-4822-947c-a194a05f0fef");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<DefinitionResourceReference>>(resources, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<DefinitionResourceReference>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<DefinitionResourceReference>>>) null);
    }

    public virtual Task<List<DefinitionResourceReference>> AuthorizeProjectResourcesAsync(
      IEnumerable<DefinitionResourceReference> resources,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("398c85bc-81aa-4822-947c-a194a05f0fef");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<DefinitionResourceReference>>(resources, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<DefinitionResourceReference>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<DefinitionResourceReference>>>) null);
    }

    public virtual Task<List<DefinitionResourceReference>> GetProjectResourcesAsync(
      string project,
      string type = null,
      string id = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("398c85bc-81aa-4822-947c-a194a05f0fef");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (type != null)
        keyValuePairList.Add(nameof (type), type);
      if (id != null)
        keyValuePairList.Add(nameof (id), id);
      return this.SendAsync<List<DefinitionResourceReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<DefinitionResourceReference>>>) null);
    }

    public virtual Task<List<DefinitionResourceReference>> GetProjectResourcesAsync(
      Guid project,
      string type = null,
      string id = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("398c85bc-81aa-4822-947c-a194a05f0fef");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (type != null)
        keyValuePairList.Add(nameof (type), type);
      if (id != null)
        keyValuePairList.Add(nameof (id), id);
      return this.SendAsync<List<DefinitionResourceReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<DefinitionResourceReference>>>) null);
    }

    [Obsolete("This endpoint is deprecated. Please see the Build Status REST endpoint.")]
    public virtual Task<string> GetBadgeAsync(
      Guid project,
      int definitionId,
      string branchName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("de6a4df8-22cd-44ee-af2d-39f6aa7a4261");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      return this.SendAsync<string>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<string>>) null);
    }

    public virtual Task<List<string>> ListBranchesAsync(
      string project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      string branchName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e05d4403-9b81-4244-8763-20fde28d1976");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        keyValuePairList.Add(nameof (repository), repository);
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      return this.SendAsync<List<string>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> ListBranchesAsync(
      Guid project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      string branchName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e05d4403-9b81-4244-8763-20fde28d1976");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        keyValuePairList.Add(nameof (repository), repository);
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      return this.SendAsync<List<string>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<BuildBadge> GetBuildBadgeAsync(
      string project,
      string repoType,
      string repoId = null,
      string branchName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("21b3b9ce-fad5-4567-9ad0-80679794e003");
      object routeValues = (object) new
      {
        project = project,
        repoType = repoType
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repoId != null)
        keyValuePairList.Add(nameof (repoId), repoId);
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      return this.SendAsync<BuildBadge>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildBadge>>) null);
    }

    public virtual Task<BuildBadge> GetBuildBadgeAsync(
      Guid project,
      string repoType,
      string repoId = null,
      string branchName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("21b3b9ce-fad5-4567-9ad0-80679794e003");
      object routeValues = (object) new
      {
        project = project,
        repoType = repoType
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repoId != null)
        keyValuePairList.Add(nameof (repoId), repoId);
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      return this.SendAsync<BuildBadge>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildBadge>>) null);
    }

    public virtual Task<string> GetBuildBadgeDataAsync(
      string project,
      string repoType,
      string repoId = null,
      string branchName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("21b3b9ce-fad5-4567-9ad0-80679794e003");
      object routeValues = (object) new
      {
        project = project,
        repoType = repoType
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repoId != null)
        keyValuePairList.Add(nameof (repoId), repoId);
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      return this.SendAsync<string>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<string>>) null);
    }

    public virtual Task<string> GetBuildBadgeDataAsync(
      Guid project,
      string repoType,
      string repoId = null,
      string branchName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("21b3b9ce-fad5-4567-9ad0-80679794e003");
      object routeValues = (object) new
      {
        project = project,
        repoType = repoType
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repoId != null)
        keyValuePairList.Add(nameof (repoId), repoId);
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      return this.SendAsync<string>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<string>>) null);
    }

    public virtual Task<List<RetentionLease>> GetRetentionLeasesForBuildAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<RetentionLease>>(new HttpMethod("GET"), new Guid("3da19a6a-f088-45c4-83ce-2ad3a87be6c4"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<RetentionLease>>>) null);
    }

    public virtual Task<List<RetentionLease>> GetRetentionLeasesForBuildAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<RetentionLease>>(new HttpMethod("GET"), new Guid("3da19a6a-f088-45c4-83ce-2ad3a87be6c4"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<RetentionLease>>>) null);
    }

    public virtual async Task DeleteBuildAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion(7.2, 7), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteBuildAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion(7.2, 7), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuildAsync(
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
      if (propertyFilters != null)
        keyValuePairList.Add(nameof (propertyFilters), propertyFilters);
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, new ApiResourceVersion(7.2, 7), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuildAsync(
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
      if (propertyFilters != null)
        keyValuePairList.Add(nameof (propertyFilters), propertyFilters);
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, new ApiResourceVersion(7.2, 7), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    public virtual Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      string project,
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minTime = null,
      DateTime? maxTime = null,
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
      string branchName = null,
      IEnumerable<int> buildIds = null,
      string repositoryId = null,
      string repositoryType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (definitions != null && definitions.Any<int>())
        keyValuePairList.Add(nameof (definitions), string.Join<int>(",", definitions));
      if (queues != null && queues.Any<int>())
        keyValuePairList.Add(nameof (queues), string.Join<int>(",", queues));
      if (buildNumber != null)
        keyValuePairList.Add(nameof (buildNumber), buildNumber);
      if (minTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minTime), minTime.Value);
      if (maxTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxTime), maxTime.Value);
      if (requestedFor != null)
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
      if (continuationToken != null)
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
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      if (buildIds != null && buildIds.Any<int>())
        keyValuePairList.Add(nameof (buildIds), string.Join<int>(",", buildIds));
      if (repositoryId != null)
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (repositoryType != null)
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 7), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) null);
    }

    public virtual Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      Guid project,
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minTime = null,
      DateTime? maxTime = null,
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
      string branchName = null,
      IEnumerable<int> buildIds = null,
      string repositoryId = null,
      string repositoryType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (definitions != null && definitions.Any<int>())
        keyValuePairList.Add(nameof (definitions), string.Join<int>(",", definitions));
      if (queues != null && queues.Any<int>())
        keyValuePairList.Add(nameof (queues), string.Join<int>(",", queues));
      if (buildNumber != null)
        keyValuePairList.Add(nameof (buildNumber), buildNumber);
      if (minTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minTime), minTime.Value);
      if (maxTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxTime), maxTime.Value);
      if (requestedFor != null)
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
      if (continuationToken != null)
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
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      if (buildIds != null && buildIds.Any<int>())
        keyValuePairList.Add(nameof (buildIds), string.Join<int>(",", buildIds));
      if (repositoryId != null)
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (repositoryType != null)
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 7), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) null);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> QueueBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string project,
      bool? ignoreWarnings = null,
      string checkInTicket = null,
      int? sourceBuildId = null,
      int? definitionId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      if (ignoreWarnings.HasValue)
        collection1.Add(nameof (ignoreWarnings), ignoreWarnings.Value.ToString());
      if (checkInTicket != null)
        collection1.Add(nameof (checkInTicket), checkInTicket);
      int num;
      if (sourceBuildId.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        num = sourceBuildId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection2.Add(nameof (sourceBuildId), str);
      }
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection3.Add(nameof (definitionId), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 7);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> QueueBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      Guid project,
      bool? ignoreWarnings = null,
      string checkInTicket = null,
      int? sourceBuildId = null,
      int? definitionId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      if (ignoreWarnings.HasValue)
        collection1.Add(nameof (ignoreWarnings), ignoreWarnings.Value.ToString());
      if (checkInTicket != null)
        collection1.Add(nameof (checkInTicket), checkInTicket);
      int num;
      if (sourceBuildId.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        num = sourceBuildId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection2.Add(nameof (sourceBuildId), str);
      }
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection3.Add(nameof (definitionId), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 7);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    private protected virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> UpdateBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string project,
      int buildId,
      bool? retry = null,
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
      if (retry.HasValue)
        collection.Add(nameof (retry), retry.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 7);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    private protected virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> UpdateBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      Guid project,
      int buildId,
      bool? retry = null,
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
      if (retry.HasValue)
        collection.Add(nameof (retry), retry.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 7);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    public virtual Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> UpdateBuildsAsync(
      IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> builds,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build>>(builds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 7);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) null);
    }

    public virtual Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> UpdateBuildsAsync(
      IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> builds,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build>>(builds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 7);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) null);
    }

    public virtual Task<List<Change>> GetBuildChangesAsync(
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
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeSourceChange.HasValue)
        keyValuePairList.Add(nameof (includeSourceChange), includeSourceChange.Value.ToString());
      return this.SendAsync<List<Change>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Change>>>) null);
    }

    public virtual Task<List<Change>> GetBuildChangesAsync(
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
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeSourceChange.HasValue)
        keyValuePairList.Add(nameof (includeSourceChange), includeSourceChange.Value.ToString());
      return this.SendAsync<List<Change>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Change>>>) null);
    }

    public virtual Task<List<Change>> GetChangesBetweenBuildsAsync(
      string project,
      int? fromBuildId = null,
      int? toBuildId = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f10f0ea5-18a1-43ec-a8fb-2042c7be9b43");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (fromBuildId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = fromBuildId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (fromBuildId), str);
      }
      if (toBuildId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = toBuildId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (toBuildId), str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<Change>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Change>>>) null);
    }

    public virtual Task<List<Change>> GetChangesBetweenBuildsAsync(
      Guid project,
      int? fromBuildId = null,
      int? toBuildId = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f10f0ea5-18a1-43ec-a8fb-2042c7be9b43");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (fromBuildId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = fromBuildId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (fromBuildId), str);
      }
      if (toBuildId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = toBuildId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (toBuildId), str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<Change>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Change>>>) null);
    }

    public virtual Task<BuildController> GetBuildControllerAsync(
      int controllerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildController>(new HttpMethod("GET"), new Guid("fcac1932-2ee1-437f-9b6f-7f696be858f6"), (object) new
      {
        controllerId = controllerId
      }, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildController>>) null);
    }

    public virtual Task<List<BuildController>> GetBuildControllersAsync(
      string name = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fcac1932-2ee1-437f-9b6f-7f696be858f6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      return this.SendAsync<List<BuildController>>(method, locationId, (object) null, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildController>>>) null);
    }

    public virtual Task<BuildDefinition> CreateDefinitionAsync(
      BuildDefinition definition,
      string project,
      int? definitionToCloneId = null,
      int? definitionToCloneRevision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildDefinition>(definition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionToCloneId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionToCloneId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionToCloneId), str);
      }
      if (definitionToCloneRevision.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionToCloneRevision.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionToCloneRevision), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 7);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildDefinition>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinition>>) null);
    }

    public virtual Task<BuildDefinition> CreateDefinitionAsync(
      BuildDefinition definition,
      Guid project,
      int? definitionToCloneId = null,
      int? definitionToCloneRevision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildDefinition>(definition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionToCloneId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionToCloneId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionToCloneId), str);
      }
      if (definitionToCloneRevision.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionToCloneRevision.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionToCloneRevision), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 7);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildDefinition>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinition>>) null);
    }

    public virtual async Task DeleteDefinitionAsync(
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion(7.2, 7), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteDefinitionAsync(
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion(7.2, 7), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<BuildDefinition> GetDefinitionAsync(
      string project,
      int definitionId,
      int? revision = null,
      DateTime? minMetricsTime = null,
      IEnumerable<string> propertyFilters = null,
      bool? includeLatestBuilds = null,
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
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (includeLatestBuilds.HasValue)
        keyValuePairList.Add(nameof (includeLatestBuilds), includeLatestBuilds.Value.ToString());
      return this.SendAsync<BuildDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 7), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinition>>) null);
    }

    public virtual Task<BuildDefinition> GetDefinitionAsync(
      Guid project,
      int definitionId,
      int? revision = null,
      DateTime? minMetricsTime = null,
      IEnumerable<string> propertyFilters = null,
      bool? includeLatestBuilds = null,
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
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (includeLatestBuilds.HasValue)
        keyValuePairList.Add(nameof (includeLatestBuilds), includeLatestBuilds.Value.ToString());
      return this.SendAsync<BuildDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 7), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinition>>) null);
    }

    protected virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      string project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeAllProperties = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (repositoryId != null)
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (repositoryType != null)
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (definitionIds != null && definitionIds.Any<int>())
        keyValuePairList.Add(nameof (definitionIds), string.Join<int>(",", definitionIds));
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (builtAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (builtAfter), builtAfter.Value);
      if (notBuiltAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (notBuiltAfter), notBuiltAfter.Value);
      bool flag;
      if (includeAllProperties.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllProperties.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllProperties), str);
      }
      if (includeLatestBuilds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestBuilds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestBuilds), str);
      }
      if (taskIdFilter.HasValue)
        keyValuePairList.Add(nameof (taskIdFilter), taskIdFilter.Value.ToString());
      if (processType.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = processType.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (processType), str);
      }
      if (yamlFilename != null)
        keyValuePairList.Add(nameof (yamlFilename), yamlFilename);
      return this.SendAsync<List<BuildDefinitionReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 7), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionReference>>>) null);
    }

    protected virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      Guid project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeAllProperties = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (repositoryId != null)
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (repositoryType != null)
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (definitionIds != null && definitionIds.Any<int>())
        keyValuePairList.Add(nameof (definitionIds), string.Join<int>(",", definitionIds));
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (builtAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (builtAfter), builtAfter.Value);
      if (notBuiltAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (notBuiltAfter), notBuiltAfter.Value);
      bool flag;
      if (includeAllProperties.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllProperties.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllProperties), str);
      }
      if (includeLatestBuilds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestBuilds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestBuilds), str);
      }
      if (taskIdFilter.HasValue)
        keyValuePairList.Add(nameof (taskIdFilter), taskIdFilter.Value.ToString());
      if (processType.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = processType.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (processType), str);
      }
      if (yamlFilename != null)
        keyValuePairList.Add(nameof (yamlFilename), yamlFilename);
      return this.SendAsync<List<BuildDefinitionReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 7), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionReference>>>) null);
    }

    public virtual Task<BuildDefinition> RestoreDefinitionAsync(
      string project,
      int definitionId,
      bool deleted,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (deleted), deleted.ToString());
      return this.SendAsync<BuildDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 7), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinition>>) null);
    }

    public virtual Task<BuildDefinition> RestoreDefinitionAsync(
      Guid project,
      int definitionId,
      bool deleted,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (deleted), deleted.ToString());
      return this.SendAsync<BuildDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 7), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinition>>) null);
    }

    public virtual Task<BuildDefinition> UpdateDefinitionAsync(
      BuildDefinition definition,
      string project,
      int definitionId,
      int? secretsSourceDefinitionId = null,
      int? secretsSourceDefinitionRevision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object obj1 = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildDefinition>(definition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (secretsSourceDefinitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = secretsSourceDefinitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (secretsSourceDefinitionId), str);
      }
      if (secretsSourceDefinitionRevision.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = secretsSourceDefinitionRevision.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (secretsSourceDefinitionRevision), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 7);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildDefinition>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinition>>) null);
    }

    public virtual Task<BuildDefinition> UpdateDefinitionAsync(
      BuildDefinition definition,
      Guid project,
      int definitionId,
      int? secretsSourceDefinitionId = null,
      int? secretsSourceDefinitionRevision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object obj1 = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildDefinition>(definition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (secretsSourceDefinitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = secretsSourceDefinitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (secretsSourceDefinitionId), str);
      }
      if (secretsSourceDefinitionRevision.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = secretsSourceDefinitionRevision.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (secretsSourceDefinitionRevision), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 7);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildDefinition>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinition>>) null);
    }

    public virtual async Task<Stream> GetFileContentsAsync(
      string project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      string commitOrBranch = null,
      string path = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("29d12225-b1d9-425f-b668-6c594a981313");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        keyValuePairList.Add(nameof (repository), repository);
      if (commitOrBranch != null)
        keyValuePairList.Add(nameof (commitOrBranch), commitOrBranch);
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetFileContentsAsync(
      Guid project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      string commitOrBranch = null,
      string path = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("29d12225-b1d9-425f-b668-6c594a981313");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        keyValuePairList.Add(nameof (repository), repository);
      if (commitOrBranch != null)
        keyValuePairList.Add(nameof (commitOrBranch), commitOrBranch);
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<Folder> CreateFolderAsync(
      Folder folder,
      string project,
      string path,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("a906531b-d2da-4f55-bda7-f3e676cc50d9");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Folder>(folder, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (path), path);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Folder>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Folder>>) null);
    }

    public virtual Task<Folder> CreateFolderAsync(
      Folder folder,
      Guid project,
      string path,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("a906531b-d2da-4f55-bda7-f3e676cc50d9");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Folder>(folder, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (path), path);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Folder>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Folder>>) null);
    }

    public virtual async Task DeleteFolderAsync(
      string project,
      string path,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("a906531b-d2da-4f55-bda7-f3e676cc50d9");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      using (await buildHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteFolderAsync(
      Guid project,
      string path,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("a906531b-d2da-4f55-bda7-f3e676cc50d9");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      using (await buildHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<Folder>> GetFoldersAsync(
      string project,
      string path = null,
      FolderQueryOrder? queryOrder = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a906531b-d2da-4f55-bda7-f3e676cc50d9");
      object routeValues = (object) new
      {
        project = project,
        path = path
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      return this.SendAsync<List<Folder>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Folder>>>) null);
    }

    public virtual Task<List<Folder>> GetFoldersAsync(
      Guid project,
      string path = null,
      FolderQueryOrder? queryOrder = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a906531b-d2da-4f55-bda7-f3e676cc50d9");
      object routeValues = (object) new
      {
        project = project,
        path = path
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      return this.SendAsync<List<Folder>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Folder>>>) null);
    }

    public virtual Task<Folder> UpdateFolderAsync(
      Folder folder,
      string project,
      string path,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a906531b-d2da-4f55-bda7-f3e676cc50d9");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Folder>(folder, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (path), path);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Folder>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Folder>>) null);
    }

    public virtual Task<Folder> UpdateFolderAsync(
      Folder folder,
      Guid project,
      string path,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a906531b-d2da-4f55-bda7-f3e676cc50d9");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Folder>(folder, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (path), path);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Folder>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Folder>>) null);
    }

    public virtual Task<PipelineGeneralSettings> GetBuildGeneralSettingsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PipelineGeneralSettings>(new HttpMethod("GET"), new Guid("c4aefd19-30ff-405b-80ad-aca021e7242a"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PipelineGeneralSettings>>) null);
    }

    public virtual Task<PipelineGeneralSettings> GetBuildGeneralSettingsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PipelineGeneralSettings>(new HttpMethod("GET"), new Guid("c4aefd19-30ff-405b-80ad-aca021e7242a"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PipelineGeneralSettings>>) null);
    }

    public virtual Task<PipelineGeneralSettings> UpdateBuildGeneralSettingsAsync(
      PipelineGeneralSettings newSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("c4aefd19-30ff-405b-80ad-aca021e7242a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<PipelineGeneralSettings>(newSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PipelineGeneralSettings>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<PipelineGeneralSettings>>) null);
    }

    public virtual Task<PipelineGeneralSettings> UpdateBuildGeneralSettingsAsync(
      PipelineGeneralSettings newSettings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("c4aefd19-30ff-405b-80ad-aca021e7242a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<PipelineGeneralSettings>(newSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PipelineGeneralSettings>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<PipelineGeneralSettings>>) null);
    }

    public virtual Task<BuildRetentionHistory> GetRetentionHistoryAsync(
      int? daysToLookback = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1a9c48be-0ef5-4ec2-b94f-f053bdd2d3bf");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (daysToLookback.HasValue)
        keyValuePairList.Add(nameof (daysToLookback), daysToLookback.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<BuildRetentionHistory>(method, locationId, (object) null, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildRetentionHistory>>) null);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> GetLatestBuildAsync(
      string project,
      string definition,
      string branchName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("54481611-01f4-47f3-998f-160da0f0c229");
      object routeValues = (object) new
      {
        project = project,
        definition = definition
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> GetLatestBuildAsync(
      Guid project,
      string definition,
      string branchName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("54481611-01f4-47f3-998f-160da0f0c229");
      object routeValues = (object) new
      {
        project = project,
        definition = definition
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    public virtual Task<List<RetentionLease>> AddRetentionLeasesAsync(
      IReadOnlyList<NewRetentionLease> newLeases,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IReadOnlyList<NewRetentionLease>>(newLeases, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<RetentionLease>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<RetentionLease>>>) null);
    }

    public virtual Task<List<RetentionLease>> AddRetentionLeasesAsync(
      IReadOnlyList<NewRetentionLease> newLeases,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IReadOnlyList<NewRetentionLease>>(newLeases, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<RetentionLease>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<RetentionLease>>>) null);
    }

    public virtual async Task DeleteRetentionLeasesByIdAsync(
      string project,
      IEnumerable<int> ids,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      using (await buildHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteRetentionLeasesByIdAsync(
      Guid project,
      IEnumerable<int> ids,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      using (await buildHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<RetentionLease> GetRetentionLeaseAsync(
      string project,
      int leaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<RetentionLease>(new HttpMethod("GET"), new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4"), (object) new
      {
        project = project,
        leaseId = leaseId
      }, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<RetentionLease>>) null);
    }

    public virtual Task<RetentionLease> GetRetentionLeaseAsync(
      Guid project,
      int leaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<RetentionLease>(new HttpMethod("GET"), new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4"), (object) new
      {
        project = project,
        leaseId = leaseId
      }, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<RetentionLease>>) null);
    }

    public virtual Task<List<RetentionLease>> GetRetentionLeasesByMinimalRetentionLeasesAsync(
      string project,
      IEnumerable<MinimalRetentionLease> leasesToFetch,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (leasesToFetch != null)
        str = string.Join<MinimalRetentionLease>("|", leasesToFetch);
      keyValuePairList.Add(nameof (leasesToFetch), str);
      return this.SendAsync<List<RetentionLease>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<RetentionLease>>>) null);
    }

    public virtual Task<List<RetentionLease>> GetRetentionLeasesByMinimalRetentionLeasesAsync(
      Guid project,
      IEnumerable<MinimalRetentionLease> leasesToFetch,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (leasesToFetch != null)
        str = string.Join<MinimalRetentionLease>("|", leasesToFetch);
      keyValuePairList.Add(nameof (leasesToFetch), str);
      return this.SendAsync<List<RetentionLease>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<RetentionLease>>>) null);
    }

    public virtual Task<List<RetentionLease>> GetRetentionLeasesByOwnerIdAsync(
      string project,
      string ownerId = null,
      int? definitionId = null,
      int? runId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (ownerId != null)
        keyValuePairList.Add(nameof (ownerId), ownerId);
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (runId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = runId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (runId), str);
      }
      return this.SendAsync<List<RetentionLease>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<RetentionLease>>>) null);
    }

    public virtual Task<List<RetentionLease>> GetRetentionLeasesByOwnerIdAsync(
      Guid project,
      string ownerId = null,
      int? definitionId = null,
      int? runId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (ownerId != null)
        keyValuePairList.Add(nameof (ownerId), ownerId);
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (runId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = runId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (runId), str);
      }
      return this.SendAsync<List<RetentionLease>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<RetentionLease>>>) null);
    }

    public virtual Task<List<RetentionLease>> GetRetentionLeasesByUserIdAsync(
      string project,
      Guid userOwnerId,
      int? definitionId = null,
      int? runId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (userOwnerId), userOwnerId.ToString());
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (runId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = runId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (runId), str);
      }
      return this.SendAsync<List<RetentionLease>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<RetentionLease>>>) null);
    }

    public virtual Task<List<RetentionLease>> GetRetentionLeasesByUserIdAsync(
      Guid project,
      Guid userOwnerId,
      int? definitionId = null,
      int? runId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (userOwnerId), userOwnerId.ToString());
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (runId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = runId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (runId), str);
      }
      return this.SendAsync<List<RetentionLease>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<RetentionLease>>>) null);
    }

    public virtual Task<RetentionLease> UpdateRetentionLeaseAsync(
      RetentionLeaseUpdate leaseUpdate,
      string project,
      int leaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4");
      object obj1 = (object) new
      {
        project = project,
        leaseId = leaseId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<RetentionLeaseUpdate>(leaseUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<RetentionLease>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<RetentionLease>>) null);
    }

    public virtual Task<RetentionLease> UpdateRetentionLeaseAsync(
      RetentionLeaseUpdate leaseUpdate,
      Guid project,
      int leaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("272051e4-9af1-45b5-ae22-8d960a5539d4");
      object obj1 = (object) new
      {
        project = project,
        leaseId = leaseId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<RetentionLeaseUpdate>(leaseUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<RetentionLease>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<RetentionLease>>) null);
    }

    public virtual async Task<Stream> GetBuildLogAsync(
      string project,
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId,
        logId = logId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (startLine.HasValue)
        keyValuePairList.Add(nameof (startLine), startLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (endLine.HasValue)
        keyValuePairList.Add(nameof (endLine), endLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBuildLogAsync(
      Guid project,
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId,
        logId = logId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (startLine.HasValue)
        keyValuePairList.Add(nameof (startLine), startLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (endLine.HasValue)
        keyValuePairList.Add(nameof (endLine), endLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<string>> GetBuildLogLinesAsync(
      string project,
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId,
        logId = logId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      long num;
      if (startLine.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = startLine.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (startLine), str);
      }
      if (endLine.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = endLine.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (endLine), str);
      }
      return this.SendAsync<List<string>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> GetBuildLogLinesAsync(
      Guid project,
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId,
        logId = logId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      long num;
      if (startLine.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = startLine.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (startLine), str);
      }
      if (endLine.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = endLine.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (endLine), str);
      }
      return this.SendAsync<List<string>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<BuildLog>> GetBuildLogsAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BuildLog>>(new HttpMethod("GET"), new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildLog>>>) null);
    }

    public virtual Task<List<BuildLog>> GetBuildLogsAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BuildLog>>(new HttpMethod("GET"), new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildLog>>>) null);
    }

    public virtual async Task<Stream> GetBuildLogsZipAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBuildLogsZipAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBuildLogZipAsync(
      string project,
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId,
        logId = logId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (startLine.HasValue)
        keyValuePairList.Add(nameof (startLine), startLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (endLine.HasValue)
        keyValuePairList.Add(nameof (endLine), endLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBuildLogZipAsync(
      Guid project,
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("35a80daf-7f30-45fc-86e8-6b813d9c90df");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId,
        logId = logId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (startLine.HasValue)
        keyValuePairList.Add(nameof (startLine), startLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (endLine.HasValue)
        keyValuePairList.Add(nameof (endLine), endLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<BuildMetric>> GetProjectMetricsAsync(
      string project,
      string metricAggregationType = null,
      DateTime? minMetricsTime = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7433fae7-a6bc-41dc-a6e2-eef9005ce41a");
      object routeValues = (object) new
      {
        project = project,
        metricAggregationType = metricAggregationType
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      return this.SendAsync<List<BuildMetric>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildMetric>>>) null);
    }

    public virtual Task<List<BuildMetric>> GetProjectMetricsAsync(
      Guid project,
      string metricAggregationType = null,
      DateTime? minMetricsTime = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7433fae7-a6bc-41dc-a6e2-eef9005ce41a");
      object routeValues = (object) new
      {
        project = project,
        metricAggregationType = metricAggregationType
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      return this.SendAsync<List<BuildMetric>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildMetric>>>) null);
    }

    public virtual Task<List<BuildMetric>> GetDefinitionMetricsAsync(
      string project,
      int definitionId,
      DateTime? minMetricsTime = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d973b939-0ce0-4fec-91d8-da3940fa1827");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      return this.SendAsync<List<BuildMetric>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildMetric>>>) null);
    }

    public virtual Task<List<BuildMetric>> GetDefinitionMetricsAsync(
      Guid project,
      int definitionId,
      DateTime? minMetricsTime = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d973b939-0ce0-4fec-91d8-da3940fa1827");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      return this.SendAsync<List<BuildMetric>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildMetric>>>) null);
    }

    public virtual Task<List<BuildOptionDefinition>> GetBuildOptionDefinitionsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BuildOptionDefinition>>(new HttpMethod("GET"), new Guid("591cb5a4-2d46-4f3a-a697-5cd42b6bd332"), (object) null, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildOptionDefinition>>>) null);
    }

    public virtual Task<List<BuildOptionDefinition>> GetBuildOptionDefinitionsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BuildOptionDefinition>>(new HttpMethod("GET"), new Guid("591cb5a4-2d46-4f3a-a697-5cd42b6bd332"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildOptionDefinition>>>) null);
    }

    public virtual Task<List<BuildOptionDefinition>> GetBuildOptionDefinitionsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BuildOptionDefinition>>(new HttpMethod("GET"), new Guid("591cb5a4-2d46-4f3a-a697-5cd42b6bd332"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildOptionDefinition>>>) null);
    }

    public virtual Task<List<SourceRepositoryItem>> GetPathContentsAsync(
      string project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      string commitOrBranch = null,
      string path = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7944d6fb-df01-4709-920a-7a189aa34037");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        keyValuePairList.Add(nameof (repository), repository);
      if (commitOrBranch != null)
        keyValuePairList.Add(nameof (commitOrBranch), commitOrBranch);
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      return this.SendAsync<List<SourceRepositoryItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<SourceRepositoryItem>>>) null);
    }

    public virtual Task<List<SourceRepositoryItem>> GetPathContentsAsync(
      Guid project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      string commitOrBranch = null,
      string path = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7944d6fb-df01-4709-920a-7a189aa34037");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        keyValuePairList.Add(nameof (repository), repository);
      if (commitOrBranch != null)
        keyValuePairList.Add(nameof (commitOrBranch), commitOrBranch);
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      return this.SendAsync<List<SourceRepositoryItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<SourceRepositoryItem>>>) null);
    }

    public virtual Task<PropertiesCollection> GetBuildPropertiesAsync(
      string project,
      int buildId,
      IEnumerable<string> filter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0a6312e9-0627-49b7-8083-7d74a64849c9");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (filter != null && filter.Any<string>())
        keyValuePairList.Add(nameof (filter), string.Join(",", filter));
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PropertiesCollection>>) null);
    }

    public virtual Task<PropertiesCollection> GetBuildPropertiesAsync(
      Guid project,
      int buildId,
      IEnumerable<string> filter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0a6312e9-0627-49b7-8083-7d74a64849c9");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (filter != null && filter.Any<string>())
        keyValuePairList.Add(nameof (filter), string.Join(",", filter));
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PropertiesCollection>>) null);
    }

    public virtual Task<PropertiesCollection> UpdateBuildPropertiesAsync(
      JsonPatchDocument document,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0a6312e9-0627-49b7-8083-7d74a64849c9");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<PropertiesCollection>>) null);
    }

    public virtual Task<PropertiesCollection> UpdateBuildPropertiesAsync(
      JsonPatchDocument document,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0a6312e9-0627-49b7-8083-7d74a64849c9");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<PropertiesCollection>>) null);
    }

    public virtual Task<PropertiesCollection> GetDefinitionPropertiesAsync(
      string project,
      int definitionId,
      IEnumerable<string> filter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d9826ad7-2a68-46a9-a6e9-677698777895");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (filter != null && filter.Any<string>())
        keyValuePairList.Add(nameof (filter), string.Join(",", filter));
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PropertiesCollection>>) null);
    }

    public virtual Task<PropertiesCollection> GetDefinitionPropertiesAsync(
      Guid project,
      int definitionId,
      IEnumerable<string> filter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d9826ad7-2a68-46a9-a6e9-677698777895");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (filter != null && filter.Any<string>())
        keyValuePairList.Add(nameof (filter), string.Join(",", filter));
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PropertiesCollection>>) null);
    }

    public virtual Task<PropertiesCollection> UpdateDefinitionPropertiesAsync(
      JsonPatchDocument document,
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d9826ad7-2a68-46a9-a6e9-677698777895");
      object obj1 = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<PropertiesCollection>>) null);
    }

    public virtual Task<PropertiesCollection> UpdateDefinitionPropertiesAsync(
      JsonPatchDocument document,
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d9826ad7-2a68-46a9-a6e9-677698777895");
      object obj1 = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<PropertiesCollection>>) null);
    }

    public virtual Task<PullRequest> GetPullRequestAsync(
      string project,
      string providerName,
      string pullRequestId,
      string repositoryId = null,
      Guid? serviceEndpointId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8763ec7-9ff0-4fb4-b2b2-9d757906ff14");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repositoryId != null)
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      return this.SendAsync<PullRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PullRequest>>) null);
    }

    public virtual Task<PullRequest> GetPullRequestAsync(
      Guid project,
      string providerName,
      string pullRequestId,
      string repositoryId = null,
      Guid? serviceEndpointId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8763ec7-9ff0-4fb4-b2b2-9d757906ff14");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName,
        pullRequestId = pullRequestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (repositoryId != null)
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      return this.SendAsync<PullRequest>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PullRequest>>) null);
    }

    public virtual Task<BuildReportMetadata> GetBuildReportAsync(
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
      if (type != null)
        keyValuePairList.Add(nameof (type), type);
      return this.SendAsync<BuildReportMetadata>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildReportMetadata>>) null);
    }

    public virtual Task<BuildReportMetadata> GetBuildReportAsync(
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
      if (type != null)
        keyValuePairList.Add(nameof (type), type);
      return this.SendAsync<BuildReportMetadata>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildReportMetadata>>) null);
    }

    public virtual async Task<Stream> GetBuildReportHtmlContentAsync(
      string project,
      int buildId,
      string type = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("45bcaa88-67e1-4042-a035-56d3b4a7d44c");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (type != null)
        keyValuePairList.Add(nameof (type), type);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/html").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBuildReportHtmlContentAsync(
      Guid project,
      int buildId,
      string type = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("45bcaa88-67e1-4042-a035-56d3b4a7d44c");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (type != null)
        keyValuePairList.Add(nameof (type), type);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await buildHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/html").ConfigureAwait(false))
        httpResponseMessage = await buildHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<SourceRepositories> ListRepositoriesAsync(
      string project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      ResultSet? resultSet = null,
      bool? pageResults = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d44d1680-f978-4834-9b93-8c6e132329c9");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        keyValuePairList.Add(nameof (repository), repository);
      if (resultSet.HasValue)
        keyValuePairList.Add(nameof (resultSet), resultSet.Value.ToString());
      if (pageResults.HasValue)
        keyValuePairList.Add(nameof (pageResults), pageResults.Value.ToString());
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<SourceRepositories>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<SourceRepositories>>) null);
    }

    public virtual Task<SourceRepositories> ListRepositoriesAsync(
      Guid project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      ResultSet? resultSet = null,
      bool? pageResults = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d44d1680-f978-4834-9b93-8c6e132329c9");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        keyValuePairList.Add(nameof (repository), repository);
      if (resultSet.HasValue)
        keyValuePairList.Add(nameof (resultSet), resultSet.Value.ToString());
      if (pageResults.HasValue)
        keyValuePairList.Add(nameof (pageResults), pageResults.Value.ToString());
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<SourceRepositories>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<SourceRepositories>>) null);
    }

    public virtual Task<List<DefinitionResourceReference>> AuthorizeDefinitionResourcesAsync(
      IEnumerable<DefinitionResourceReference> resources,
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ea623316-1967-45eb-89ab-e9e6110cf2d6");
      object obj1 = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<DefinitionResourceReference>>(resources, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<DefinitionResourceReference>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<DefinitionResourceReference>>>) null);
    }

    public virtual Task<List<DefinitionResourceReference>> AuthorizeDefinitionResourcesAsync(
      IEnumerable<DefinitionResourceReference> resources,
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ea623316-1967-45eb-89ab-e9e6110cf2d6");
      object obj1 = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<DefinitionResourceReference>>(resources, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<DefinitionResourceReference>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<DefinitionResourceReference>>>) null);
    }

    public virtual Task<List<DefinitionResourceReference>> GetDefinitionResourcesAsync(
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<DefinitionResourceReference>>(new HttpMethod("GET"), new Guid("ea623316-1967-45eb-89ab-e9e6110cf2d6"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<DefinitionResourceReference>>>) null);
    }

    public virtual Task<List<DefinitionResourceReference>> GetDefinitionResourcesAsync(
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<DefinitionResourceReference>>(new HttpMethod("GET"), new Guid("ea623316-1967-45eb-89ab-e9e6110cf2d6"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<DefinitionResourceReference>>>) null);
    }

    public virtual Task<BuildResourceUsage> GetResourceUsageAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildResourceUsage>(new HttpMethod("GET"), new Guid("3813d06c-9e36-4ea1-aac3-61a485d60e3d"), (object) null, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildResourceUsage>>) null);
    }

    public virtual Task<ProjectRetentionSetting> GetRetentionSettingsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ProjectRetentionSetting>(new HttpMethod("GET"), new Guid("dadb46e7-5851-4c72-820e-ae8abb82f59f"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<ProjectRetentionSetting>>) null);
    }

    public virtual Task<ProjectRetentionSetting> GetRetentionSettingsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ProjectRetentionSetting>(new HttpMethod("GET"), new Guid("dadb46e7-5851-4c72-820e-ae8abb82f59f"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<ProjectRetentionSetting>>) null);
    }

    public virtual Task<ProjectRetentionSetting> UpdateRetentionSettingsAsync(
      UpdateProjectRetentionSettingModel updateModel,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("dadb46e7-5851-4c72-820e-ae8abb82f59f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateProjectRetentionSettingModel>(updateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProjectRetentionSetting>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<ProjectRetentionSetting>>) null);
    }

    public virtual Task<ProjectRetentionSetting> UpdateRetentionSettingsAsync(
      UpdateProjectRetentionSettingModel updateModel,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("dadb46e7-5851-4c72-820e-ae8abb82f59f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateProjectRetentionSettingModel>(updateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProjectRetentionSetting>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<ProjectRetentionSetting>>) null);
    }

    public virtual Task<List<BuildDefinitionRevision>> GetDefinitionRevisionsAsync(
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BuildDefinitionRevision>>(new HttpMethod("GET"), new Guid("7c116775-52e5-453e-8c5d-914d9762d8c4"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionRevision>>>) null);
    }

    public virtual Task<List<BuildDefinitionRevision>> GetDefinitionRevisionsAsync(
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BuildDefinitionRevision>>(new HttpMethod("GET"), new Guid("7c116775-52e5-453e-8c5d-914d9762d8c4"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionRevision>>>) null);
    }

    public virtual Task<BuildSettings> GetBuildSettingsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildSettings>(new HttpMethod("GET"), new Guid("aa8c1c9c-ef8b-474a-b8c4-785c7b191d0d"), (object) null, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildSettings>>) null);
    }

    public virtual Task<BuildSettings> GetBuildSettingsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildSettings>(new HttpMethod("GET"), new Guid("aa8c1c9c-ef8b-474a-b8c4-785c7b191d0d"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildSettings>>) null);
    }

    public virtual Task<BuildSettings> GetBuildSettingsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildSettings>(new HttpMethod("GET"), new Guid("aa8c1c9c-ef8b-474a-b8c4-785c7b191d0d"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildSettings>>) null);
    }

    public virtual Task<BuildSettings> UpdateBuildSettingsAsync(
      BuildSettings settings,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("aa8c1c9c-ef8b-474a-b8c4-785c7b191d0d");
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildSettings>(settings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildSettings>(method, locationId, (object) null, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<BuildSettings>>) null);
    }

    public virtual Task<BuildSettings> UpdateBuildSettingsAsync(
      BuildSettings settings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("aa8c1c9c-ef8b-474a-b8c4-785c7b191d0d");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildSettings>(settings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildSettings>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<BuildSettings>>) null);
    }

    public virtual Task<BuildSettings> UpdateBuildSettingsAsync(
      BuildSettings settings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("aa8c1c9c-ef8b-474a-b8c4-785c7b191d0d");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildSettings>(settings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildSettings>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<BuildSettings>>) null);
    }

    public virtual Task<List<SourceProviderAttributes>> ListSourceProvidersAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<SourceProviderAttributes>>(new HttpMethod("GET"), new Guid("3ce81729-954f-423d-a581-9fea01d25186"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<SourceProviderAttributes>>>) null);
    }

    public virtual Task<List<SourceProviderAttributes>> ListSourceProvidersAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<SourceProviderAttributes>>(new HttpMethod("GET"), new Guid("3ce81729-954f-423d-a581-9fea01d25186"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<SourceProviderAttributes>>>) null);
    }

    public virtual async Task UpdateStageAsync(
      UpdateStageParameters updateParameters,
      int buildId,
      string stageRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b8aac6c9-744b-46e1-88fc-3550969f9313");
      object obj1 = (object) new
      {
        buildId = buildId,
        stageRefName = stageRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateStageParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      BuildHttpClientBase buildHttpClientBase2 = buildHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await buildHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdateStageAsync(
      UpdateStageParameters updateParameters,
      string project,
      int buildId,
      string stageRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b8aac6c9-744b-46e1-88fc-3550969f9313");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId,
        stageRefName = stageRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateStageParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      BuildHttpClientBase buildHttpClientBase2 = buildHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await buildHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdateStageAsync(
      UpdateStageParameters updateParameters,
      Guid project,
      int buildId,
      string stageRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b8aac6c9-744b-46e1-88fc-3550969f9313");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId,
        stageRefName = stageRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateStageParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      BuildHttpClientBase buildHttpClientBase2 = buildHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await buildHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<string> GetStatusBadgeAsync(
      string project,
      string definition,
      string branchName = null,
      string stageName = null,
      string jobName = null,
      string configuration = null,
      string label = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("07acfdce-4757-4439-b422-ddd13a2fcc10");
      object routeValues = (object) new
      {
        project = project,
        definition = definition
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      if (stageName != null)
        keyValuePairList.Add(nameof (stageName), stageName);
      if (jobName != null)
        keyValuePairList.Add(nameof (jobName), jobName);
      if (configuration != null)
        keyValuePairList.Add(nameof (configuration), configuration);
      if (label != null)
        keyValuePairList.Add(nameof (label), label);
      return this.SendAsync<string>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<string>>) null);
    }

    public virtual Task<string> GetStatusBadgeAsync(
      Guid project,
      string definition,
      string branchName = null,
      string stageName = null,
      string jobName = null,
      string configuration = null,
      string label = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("07acfdce-4757-4439-b422-ddd13a2fcc10");
      object routeValues = (object) new
      {
        project = project,
        definition = definition
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (branchName != null)
        keyValuePairList.Add(nameof (branchName), branchName);
      if (stageName != null)
        keyValuePairList.Add(nameof (stageName), stageName);
      if (jobName != null)
        keyValuePairList.Add(nameof (jobName), jobName);
      if (configuration != null)
        keyValuePairList.Add(nameof (configuration), configuration);
      if (label != null)
        keyValuePairList.Add(nameof (label), label);
      return this.SendAsync<string>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<string>>) null);
    }

    public virtual Task<List<string>> AddBuildTagAsync(
      string project,
      int buildId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("PUT"), new Guid("6e6114b2-8161-44c8-8f6c-c5505782427f"), (object) new
      {
        project = project,
        buildId = buildId,
        tag = tag
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> AddBuildTagAsync(
      Guid project,
      int buildId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("PUT"), new Guid("6e6114b2-8161-44c8-8f6c-c5505782427f"), (object) new
      {
        project = project,
        buildId = buildId,
        tag = tag
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> AddBuildTagsAsync(
      IEnumerable<string> tags,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6e6114b2-8161-44c8-8f6c-c5505782427f");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(tags, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> AddBuildTagsAsync(
      IEnumerable<string> tags,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6e6114b2-8161-44c8-8f6c-c5505782427f");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(tags, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> DeleteBuildTagAsync(
      string project,
      int buildId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("DELETE"), new Guid("6e6114b2-8161-44c8-8f6c-c5505782427f"), (object) new
      {
        project = project,
        buildId = buildId,
        tag = tag
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> DeleteBuildTagAsync(
      Guid project,
      int buildId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("DELETE"), new Guid("6e6114b2-8161-44c8-8f6c-c5505782427f"), (object) new
      {
        project = project,
        buildId = buildId,
        tag = tag
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> GetBuildTagsAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("6e6114b2-8161-44c8-8f6c-c5505782427f"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> GetBuildTagsAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("6e6114b2-8161-44c8-8f6c-c5505782427f"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> UpdateBuildTagsAsync(
      UpdateTagParameters updateParameters,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6e6114b2-8161-44c8-8f6c-c5505782427f");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateTagParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> UpdateBuildTagsAsync(
      UpdateTagParameters updateParameters,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6e6114b2-8161-44c8-8f6c-c5505782427f");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateTagParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> AddDefinitionTagAsync(
      string project,
      int definitionId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("PUT"), new Guid("cb894432-134a-4d31-a839-83beceaace4b"), (object) new
      {
        project = project,
        definitionId = definitionId,
        tag = tag
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> AddDefinitionTagAsync(
      Guid project,
      int definitionId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("PUT"), new Guid("cb894432-134a-4d31-a839-83beceaace4b"), (object) new
      {
        project = project,
        definitionId = definitionId,
        tag = tag
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> AddDefinitionTagsAsync(
      IEnumerable<string> tags,
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("cb894432-134a-4d31-a839-83beceaace4b");
      object obj1 = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(tags, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> AddDefinitionTagsAsync(
      IEnumerable<string> tags,
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("cb894432-134a-4d31-a839-83beceaace4b");
      object obj1 = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(tags, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> DeleteDefinitionTagAsync(
      string project,
      int definitionId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("DELETE"), new Guid("cb894432-134a-4d31-a839-83beceaace4b"), (object) new
      {
        project = project,
        definitionId = definitionId,
        tag = tag
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> DeleteDefinitionTagAsync(
      Guid project,
      int definitionId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("DELETE"), new Guid("cb894432-134a-4d31-a839-83beceaace4b"), (object) new
      {
        project = project,
        definitionId = definitionId,
        tag = tag
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> GetDefinitionTagsAsync(
      string project,
      int definitionId,
      int? revision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("cb894432-134a-4d31-a839-83beceaace4b");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<string>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> GetDefinitionTagsAsync(
      Guid project,
      int definitionId,
      int? revision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("cb894432-134a-4d31-a839-83beceaace4b");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<string>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> UpdateDefinitionTagsAsync(
      UpdateTagParameters updateParameters,
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("cb894432-134a-4d31-a839-83beceaace4b");
      object obj1 = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateTagParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> UpdateDefinitionTagsAsync(
      UpdateTagParameters updateParameters,
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("cb894432-134a-4d31-a839-83beceaace4b");
      object obj1 = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateTagParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> DeleteTagAsync(
      string project,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("DELETE"), new Guid("d84ac5c6-edc7-43d5-adc9-1b34be5dea09"), (object) new
      {
        project = project,
        tag = tag
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> DeleteTagAsync(
      Guid project,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("DELETE"), new Guid("d84ac5c6-edc7-43d5-adc9-1b34be5dea09"), (object) new
      {
        project = project,
        tag = tag
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> GetTagsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("d84ac5c6-edc7-43d5-adc9-1b34be5dea09"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> GetTagsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("d84ac5c6-edc7-43d5-adc9-1b34be5dea09"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual async Task DeleteTemplateAsync(
      string project,
      string templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("e884571e-7f92-4d6a-9274-3f5649900835"), (object) new
      {
        project = project,
        templateId = templateId
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTemplateAsync(
      Guid project,
      string templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("e884571e-7f92-4d6a-9274-3f5649900835"), (object) new
      {
        project = project,
        templateId = templateId
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<BuildDefinitionTemplate> GetTemplateAsync(
      string project,
      string templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildDefinitionTemplate>(new HttpMethod("GET"), new Guid("e884571e-7f92-4d6a-9274-3f5649900835"), (object) new
      {
        project = project,
        templateId = templateId
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinitionTemplate>>) null);
    }

    public virtual Task<BuildDefinitionTemplate> GetTemplateAsync(
      Guid project,
      string templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildDefinitionTemplate>(new HttpMethod("GET"), new Guid("e884571e-7f92-4d6a-9274-3f5649900835"), (object) new
      {
        project = project,
        templateId = templateId
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinitionTemplate>>) null);
    }

    public virtual Task<List<BuildDefinitionTemplate>> GetTemplatesAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BuildDefinitionTemplate>>(new HttpMethod("GET"), new Guid("e884571e-7f92-4d6a-9274-3f5649900835"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionTemplate>>>) null);
    }

    public virtual Task<List<BuildDefinitionTemplate>> GetTemplatesAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BuildDefinitionTemplate>>(new HttpMethod("GET"), new Guid("e884571e-7f92-4d6a-9274-3f5649900835"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionTemplate>>>) null);
    }

    public virtual Task<BuildDefinitionTemplate> SaveTemplateAsync(
      BuildDefinitionTemplate template,
      string project,
      string templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("e884571e-7f92-4d6a-9274-3f5649900835");
      object obj1 = (object) new
      {
        project = project,
        templateId = templateId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildDefinitionTemplate>(template, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildDefinitionTemplate>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinitionTemplate>>) null);
    }

    public virtual Task<BuildDefinitionTemplate> SaveTemplateAsync(
      BuildDefinitionTemplate template,
      Guid project,
      string templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("e884571e-7f92-4d6a-9274-3f5649900835");
      object obj1 = (object) new
      {
        project = project,
        templateId = templateId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildDefinitionTemplate>(template, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildDefinitionTemplate>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinitionTemplate>>) null);
    }

    public virtual Task<Timeline> GetBuildTimelineAsync(
      string project,
      int buildId,
      Guid? timelineId = null,
      int? changeId = null,
      Guid? planId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8baac422-4c6e-4de5-8532-db96d92acffa");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId,
        timelineId = timelineId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (changeId.HasValue)
        keyValuePairList.Add(nameof (changeId), changeId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (planId.HasValue)
        keyValuePairList.Add(nameof (planId), planId.Value.ToString());
      return this.SendAsync<Timeline>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<Timeline>>) null);
    }

    public virtual Task<Timeline> GetBuildTimelineAsync(
      Guid project,
      int buildId,
      Guid? timelineId = null,
      int? changeId = null,
      Guid? planId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8baac422-4c6e-4de5-8532-db96d92acffa");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId,
        timelineId = timelineId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (changeId.HasValue)
        keyValuePairList.Add(nameof (changeId), changeId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (planId.HasValue)
        keyValuePairList.Add(nameof (planId), planId.Value.ToString());
      return this.SendAsync<Timeline>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<Timeline>>) null);
    }

    public virtual async Task RestoreWebhooksAsync(
      List<DefinitionTriggerType> triggerTypes,
      string project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("793bceb8-9736-4030-bd2f-fb3ce6d6b478");
      object obj1 = (object) new
      {
        project = project,
        providerName = providerName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<List<DefinitionTriggerType>>(triggerTypes, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        collection.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        collection.Add(nameof (repository), repository);
      BuildHttpClientBase buildHttpClientBase2 = buildHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await buildHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task RestoreWebhooksAsync(
      List<DefinitionTriggerType> triggerTypes,
      Guid project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BuildHttpClientBase buildHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("793bceb8-9736-4030-bd2f-fb3ce6d6b478");
      object obj1 = (object) new
      {
        project = project,
        providerName = providerName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<List<DefinitionTriggerType>>(triggerTypes, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        collection.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        collection.Add(nameof (repository), repository);
      BuildHttpClientBase buildHttpClientBase2 = buildHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await buildHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<RepositoryWebhook>> ListWebhooksAsync(
      string project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8f20ff82-9498-4812-9f6e-9c01bdc50e99");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        keyValuePairList.Add(nameof (repository), repository);
      return this.SendAsync<List<RepositoryWebhook>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<RepositoryWebhook>>>) null);
    }

    public virtual Task<List<RepositoryWebhook>> ListWebhooksAsync(
      Guid project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8f20ff82-9498-4812-9f6e-9c01bdc50e99");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        keyValuePairList.Add(nameof (repository), repository);
      return this.SendAsync<List<RepositoryWebhook>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<RepositoryWebhook>>>) null);
    }

    public virtual Task<List<ResourceRef>> GetBuildWorkItemsRefsAsync(
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
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<ResourceRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<ResourceRef>>>) null);
    }

    public virtual Task<List<ResourceRef>> GetBuildWorkItemsRefsAsync(
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
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<ResourceRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<ResourceRef>>>) null);
    }

    public virtual Task<List<ResourceRef>> GetBuildWorkItemsRefsFromCommitsAsync(
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
      if (top.HasValue)
        collection.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ResourceRef>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<ResourceRef>>>) null);
    }

    public virtual Task<List<ResourceRef>> GetBuildWorkItemsRefsFromCommitsAsync(
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
      if (top.HasValue)
        collection.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ResourceRef>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<List<ResourceRef>>>) null);
    }

    public virtual Task<List<ResourceRef>> GetWorkItemsBetweenBuildsAsync(
      string project,
      int fromBuildId,
      int toBuildId,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("52ba8915-5518-42e3-a4bb-b0182d159e2d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (fromBuildId), fromBuildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (toBuildId), toBuildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<ResourceRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<ResourceRef>>>) null);
    }

    public virtual Task<List<ResourceRef>> GetWorkItemsBetweenBuildsAsync(
      Guid project,
      int fromBuildId,
      int toBuildId,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("52ba8915-5518-42e3-a4bb-b0182d159e2d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (fromBuildId), fromBuildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (toBuildId), toBuildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<ResourceRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<ResourceRef>>>) null);
    }

    public virtual Task<YamlBuild> GetDefinitionYamlAsync(
      string project,
      int definitionId,
      int? revision = null,
      DateTime? minMetricsTime = null,
      IEnumerable<string> propertyFilters = null,
      bool? includeLatestBuilds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7c3df3a1-7e51-4150-8cf7-540347f8697f");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (includeLatestBuilds.HasValue)
        keyValuePairList.Add(nameof (includeLatestBuilds), includeLatestBuilds.Value.ToString());
      return this.SendAsync<YamlBuild>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<YamlBuild>>) null);
    }

    public virtual Task<YamlBuild> GetDefinitionYamlAsync(
      Guid project,
      int definitionId,
      int? revision = null,
      DateTime? minMetricsTime = null,
      IEnumerable<string> propertyFilters = null,
      bool? includeLatestBuilds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7c3df3a1-7e51-4150-8cf7-540347f8697f");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (includeLatestBuilds.HasValue)
        keyValuePairList.Add(nameof (includeLatestBuilds), includeLatestBuilds.Value.ToString());
      return this.SendAsync<YamlBuild>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<YamlBuild>>) null);
    }
  }
}
