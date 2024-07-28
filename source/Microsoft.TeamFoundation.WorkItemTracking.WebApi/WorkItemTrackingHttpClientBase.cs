// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.WorkItemTrackingHttpClientBase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi
{
  [ResourceArea("5264459E-E5E0-4BD8-B118-0985E68A4EC5")]
  public abstract class WorkItemTrackingHttpClientBase : WorkItemTrackingHttpClientCompatBase
  {
    public WorkItemTrackingHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public WorkItemTrackingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public WorkItemTrackingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public WorkItemTrackingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public WorkItemTrackingHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<AccountMyWorkResult> GetAccountMyWorkDataAsync(
      QueryOption? queryOption = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("def3d688-ddf5-4096-9024-69beea15cdbd");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (queryOption.HasValue)
        keyValuePairList.Add("$queryOption", queryOption.Value.ToString());
      return this.SendAsync<AccountMyWorkResult>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<AccountRecentActivityWorkItemModel2>> GetRecentActivityDataAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AccountRecentActivityWorkItemModel2>>(new HttpMethod("GET"), new Guid("1bc988f4-c15f-4072-ad35-497c87e3a909"), version: new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AccountRecentMentionWorkItemModel>> GetRecentMentionsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AccountRecentMentionWorkItemModel>>(new HttpMethod("GET"), new Guid("d60eeb6e-e18c-4478-9e94-a0094e28f41c"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkArtifactLink>> GetWorkArtifactLinkTypesAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkArtifactLink>>(new HttpMethod("GET"), new Guid("1a31de40-e318-41cd-a6c6-881077df52e3"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ArtifactUriQueryResult> QueryWorkItemsForArtifactUrisAsync(
      ArtifactUriQuery artifactUriQuery,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a9a9aa7a-8c09-44d3-ad1b-46e855c1e3d3");
      HttpContent httpContent = (HttpContent) new ObjectContent<ArtifactUriQuery>(artifactUriQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ArtifactUriQueryResult>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ArtifactUriQueryResult> QueryWorkItemsForArtifactUrisAsync(
      ArtifactUriQuery artifactUriQuery,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a9a9aa7a-8c09-44d3-ad1b-46e855c1e3d3");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ArtifactUriQuery>(artifactUriQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ArtifactUriQueryResult>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ArtifactUriQueryResult> QueryWorkItemsForArtifactUrisAsync(
      ArtifactUriQuery artifactUriQuery,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a9a9aa7a-8c09-44d3-ad1b-46e855c1e3d3");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ArtifactUriQuery>(artifactUriQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ArtifactUriQueryResult>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<AttachmentReference> CreateAttachmentAsync(
      Stream uploadStream,
      string project,
      string fileName = null,
      string uploadType = null,
      string areaPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e07b5fa4-1499-494d-a496-64b860fd64ff");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (fileName != null)
        collection.Add(nameof (fileName), fileName);
      if (uploadType != null)
        collection.Add(nameof (uploadType), uploadType);
      if (areaPath != null)
        collection.Add(nameof (areaPath), areaPath);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AttachmentReference>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<AttachmentReference> CreateAttachmentAsync(
      Stream uploadStream,
      Guid project,
      string fileName = null,
      string uploadType = null,
      string areaPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e07b5fa4-1499-494d-a496-64b860fd64ff");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (fileName != null)
        collection.Add(nameof (fileName), fileName);
      if (uploadType != null)
        collection.Add(nameof (uploadType), uploadType);
      if (areaPath != null)
        collection.Add(nameof (areaPath), areaPath);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AttachmentReference>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<AttachmentReference> CreateAttachmentAsync(
      Stream uploadStream,
      string fileName = null,
      string uploadType = null,
      string areaPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e07b5fa4-1499-494d-a496-64b860fd64ff");
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (fileName != null)
        collection.Add(nameof (fileName), fileName);
      if (uploadType != null)
        collection.Add(nameof (uploadType), uploadType);
      if (areaPath != null)
        collection.Add(nameof (areaPath), areaPath);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AttachmentReference>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task<Stream> GetAttachmentContentAsync(
      string project,
      Guid id,
      string fileName = null,
      bool? download = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e07b5fa4-1499-494d-a496-64b860fd64ff");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await trackingHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await trackingHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentContentAsync(
      Guid project,
      Guid id,
      string fileName = null,
      bool? download = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e07b5fa4-1499-494d-a496-64b860fd64ff");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await trackingHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await trackingHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentContentAsync(
      Guid id,
      string fileName = null,
      bool? download = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e07b5fa4-1499-494d-a496-64b860fd64ff");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await trackingHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await trackingHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentZipAsync(
      string project,
      Guid id,
      string fileName = null,
      bool? download = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e07b5fa4-1499-494d-a496-64b860fd64ff");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await trackingHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await trackingHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentZipAsync(
      Guid project,
      Guid id,
      string fileName = null,
      bool? download = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e07b5fa4-1499-494d-a496-64b860fd64ff");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await trackingHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await trackingHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetAttachmentZipAsync(
      Guid id,
      string fileName = null,
      bool? download = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e07b5fa4-1499-494d-a496-64b860fd64ff");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await trackingHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await trackingHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<WorkItemClassificationNode>> GetClassificationNodesAsync(
      string project,
      IEnumerable<int> ids,
      int? depth = null,
      ClassificationNodesErrorPolicy? errorPolicy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a70579d1-f53a-48ee-a5be-7be8659023b9");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      if (depth.HasValue)
        keyValuePairList.Add("$depth", depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (errorPolicy.HasValue)
        keyValuePairList.Add(nameof (errorPolicy), errorPolicy.Value.ToString());
      return this.SendAsync<List<WorkItemClassificationNode>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemClassificationNode>> GetClassificationNodesAsync(
      Guid project,
      IEnumerable<int> ids,
      int? depth = null,
      ClassificationNodesErrorPolicy? errorPolicy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a70579d1-f53a-48ee-a5be-7be8659023b9");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      if (depth.HasValue)
        keyValuePairList.Add("$depth", depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (errorPolicy.HasValue)
        keyValuePairList.Add(nameof (errorPolicy), errorPolicy.Value.ToString());
      return this.SendAsync<List<WorkItemClassificationNode>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemClassificationNode>> GetRootNodesAsync(
      string project,
      int? depth = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a70579d1-f53a-48ee-a5be-7be8659023b9");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (depth.HasValue)
        keyValuePairList.Add("$depth", depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<WorkItemClassificationNode>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemClassificationNode>> GetRootNodesAsync(
      Guid project,
      int? depth = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a70579d1-f53a-48ee-a5be-7be8659023b9");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (depth.HasValue)
        keyValuePairList.Add("$depth", depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<WorkItemClassificationNode>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemClassificationNode> CreateOrUpdateClassificationNodeAsync(
      WorkItemClassificationNode postedNode,
      string project,
      TreeStructureGroup structureGroup,
      string path = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5a172953-1b41-49d3-840a-33f79c3ce89f");
      object obj1 = (object) new
      {
        project = project,
        structureGroup = structureGroup,
        path = path
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemClassificationNode>(postedNode, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemClassificationNode>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemClassificationNode> CreateOrUpdateClassificationNodeAsync(
      WorkItemClassificationNode postedNode,
      Guid project,
      TreeStructureGroup structureGroup,
      string path = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5a172953-1b41-49d3-840a-33f79c3ce89f");
      object obj1 = (object) new
      {
        project = project,
        structureGroup = structureGroup,
        path = path
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemClassificationNode>(postedNode, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemClassificationNode>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteClassificationNodeAsync(
      string project,
      TreeStructureGroup structureGroup,
      string path = null,
      int? reclassifyId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("5a172953-1b41-49d3-840a-33f79c3ce89f");
      object routeValues = (object) new
      {
        project = project,
        structureGroup = structureGroup,
        path = path
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (reclassifyId.HasValue)
        keyValuePairList.Add("$reclassifyId", reclassifyId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await trackingHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteClassificationNodeAsync(
      Guid project,
      TreeStructureGroup structureGroup,
      string path = null,
      int? reclassifyId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("5a172953-1b41-49d3-840a-33f79c3ce89f");
      object routeValues = (object) new
      {
        project = project,
        structureGroup = structureGroup,
        path = path
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (reclassifyId.HasValue)
        keyValuePairList.Add("$reclassifyId", reclassifyId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await trackingHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<WorkItemClassificationNode> GetClassificationNodeAsync(
      string project,
      TreeStructureGroup structureGroup,
      string path = null,
      int? depth = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5a172953-1b41-49d3-840a-33f79c3ce89f");
      object routeValues = (object) new
      {
        project = project,
        structureGroup = structureGroup,
        path = path
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (depth.HasValue)
        keyValuePairList.Add("$depth", depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<WorkItemClassificationNode>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemClassificationNode> GetClassificationNodeAsync(
      Guid project,
      TreeStructureGroup structureGroup,
      string path = null,
      int? depth = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5a172953-1b41-49d3-840a-33f79c3ce89f");
      object routeValues = (object) new
      {
        project = project,
        structureGroup = structureGroup,
        path = path
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (depth.HasValue)
        keyValuePairList.Add("$depth", depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<WorkItemClassificationNode>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemClassificationNode> UpdateClassificationNodeAsync(
      WorkItemClassificationNode postedNode,
      string project,
      TreeStructureGroup structureGroup,
      string path = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("5a172953-1b41-49d3-840a-33f79c3ce89f");
      object obj1 = (object) new
      {
        project = project,
        structureGroup = structureGroup,
        path = path
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemClassificationNode>(postedNode, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemClassificationNode>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemClassificationNode> UpdateClassificationNodeAsync(
      WorkItemClassificationNode postedNode,
      Guid project,
      TreeStructureGroup structureGroup,
      string path = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("5a172953-1b41-49d3-840a-33f79c3ce89f");
      object obj1 = (object) new
      {
        project = project,
        structureGroup = structureGroup,
        path = path
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemClassificationNode>(postedNode, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemClassificationNode>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<IdentityRef>> GetEngagedUsersAsync(
      string project,
      int workItemId,
      int commentId,
      CommentReactionType reactionType,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e33ca5e0-2349-4285-af3d-d72d86781c35");
      object routeValues = (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId,
        reactionType = reactionType
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<IdentityRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRef>> GetEngagedUsersAsync(
      Guid project,
      int workItemId,
      int commentId,
      CommentReactionType reactionType,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e33ca5e0-2349-4285-af3d-d72d86781c35");
      object routeValues = (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId,
        reactionType = reactionType
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<IdentityRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Comment> AddCommentAsync(
      CommentCreate request,
      string project,
      int workItemId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object obj1 = (object) new
      {
        project = project,
        workItemId = workItemId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentCreate>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> AddCommentAsync(
      CommentCreate request,
      Guid project,
      int workItemId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object obj1 = (object) new
      {
        project = project,
        workItemId = workItemId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentCreate>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> AddWorkItemCommentAsync(
      CommentCreate request,
      string project,
      int workItemId,
      CommentFormat format,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object obj1 = (object) new
      {
        project = project,
        workItemId = workItemId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentCreate>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (format), format.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<Comment> AddWorkItemCommentAsync(
      CommentCreate request,
      Guid project,
      int workItemId,
      CommentFormat format,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object obj1 = (object) new
      {
        project = project,
        workItemId = workItemId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentCreate>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (format), format.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual async Task DeleteCommentAsync(
      string project,
      int workItemId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("608aac0a-32e1-4493-a863-b9cf4566d257"), (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 4), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteCommentAsync(
      Guid project,
      int workItemId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("608aac0a-32e1-4493-a863-b9cf4566d257"), (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 4), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<Comment> GetCommentAsync(
      string project,
      int workItemId,
      int commentId,
      bool? includeDeleted = null,
      CommentExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object routeValues = (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDeleted.HasValue)
        keyValuePairList.Add(nameof (includeDeleted), includeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<Comment>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Comment> GetCommentAsync(
      Guid project,
      int workItemId,
      int commentId,
      bool? includeDeleted = null,
      CommentExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object routeValues = (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDeleted.HasValue)
        keyValuePairList.Add(nameof (includeDeleted), includeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<Comment>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CommentList> GetCommentsAsync(
      string project,
      int workItemId,
      int? top = null,
      string continuationToken = null,
      bool? includeDeleted = null,
      CommentExpandOptions? expand = null,
      CommentSortOrder? order = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object routeValues = (object) new
      {
        project = project,
        workItemId = workItemId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (includeDeleted.HasValue)
        keyValuePairList.Add(nameof (includeDeleted), includeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (order.HasValue)
        keyValuePairList.Add(nameof (order), order.Value.ToString());
      return this.SendAsync<CommentList>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CommentList> GetCommentsAsync(
      Guid project,
      int workItemId,
      int? top = null,
      string continuationToken = null,
      bool? includeDeleted = null,
      CommentExpandOptions? expand = null,
      CommentSortOrder? order = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object routeValues = (object) new
      {
        project = project,
        workItemId = workItemId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (includeDeleted.HasValue)
        keyValuePairList.Add(nameof (includeDeleted), includeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (order.HasValue)
        keyValuePairList.Add(nameof (order), order.Value.ToString());
      return this.SendAsync<CommentList>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CommentList> GetCommentsBatchAsync(
      string project,
      int workItemId,
      IEnumerable<int> ids,
      bool? includeDeleted = null,
      CommentExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object routeValues = (object) new
      {
        project = project,
        workItemId = workItemId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      if (includeDeleted.HasValue)
        keyValuePairList.Add(nameof (includeDeleted), includeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<CommentList>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CommentList> GetCommentsBatchAsync(
      Guid project,
      int workItemId,
      IEnumerable<int> ids,
      bool? includeDeleted = null,
      CommentExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object routeValues = (object) new
      {
        project = project,
        workItemId = workItemId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      if (includeDeleted.HasValue)
        keyValuePairList.Add(nameof (includeDeleted), includeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<CommentList>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Comment> UpdateCommentAsync(
      CommentUpdate request,
      string project,
      int workItemId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object obj1 = (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentUpdate>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> UpdateCommentAsync(
      CommentUpdate request,
      Guid project,
      int workItemId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object obj1 = (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentUpdate>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Comment> UpdateWorkItemCommentAsync(
      CommentUpdate request,
      string project,
      int workItemId,
      int commentId,
      CommentFormat format,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object obj1 = (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentUpdate>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (format), format.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<Comment> UpdateWorkItemCommentAsync(
      CommentUpdate request,
      Guid project,
      int workItemId,
      int commentId,
      CommentFormat format,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("608aac0a-32e1-4493-a863-b9cf4566d257");
      object obj1 = (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentUpdate>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (format), format.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Comment>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<CommentReaction> CreateCommentReactionAsync(
      string project,
      int workItemId,
      int commentId,
      CommentReactionType reactionType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentReaction>(new HttpMethod("PUT"), new Guid("f6cb3f27-1028-4851-af96-887e570dc21f"), (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId,
        reactionType = reactionType
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CommentReaction> CreateCommentReactionAsync(
      Guid project,
      int workItemId,
      int commentId,
      CommentReactionType reactionType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentReaction>(new HttpMethod("PUT"), new Guid("f6cb3f27-1028-4851-af96-887e570dc21f"), (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId,
        reactionType = reactionType
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CommentReaction> DeleteCommentReactionAsync(
      string project,
      int workItemId,
      int commentId,
      CommentReactionType reactionType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentReaction>(new HttpMethod("DELETE"), new Guid("f6cb3f27-1028-4851-af96-887e570dc21f"), (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId,
        reactionType = reactionType
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CommentReaction> DeleteCommentReactionAsync(
      Guid project,
      int workItemId,
      int commentId,
      CommentReactionType reactionType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentReaction>(new HttpMethod("DELETE"), new Guid("f6cb3f27-1028-4851-af96-887e570dc21f"), (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId,
        reactionType = reactionType
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<CommentReaction>> GetCommentReactionsAsync(
      string project,
      int workItemId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<CommentReaction>>(new HttpMethod("GET"), new Guid("f6cb3f27-1028-4851-af96-887e570dc21f"), (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<CommentReaction>> GetCommentReactionsAsync(
      Guid project,
      int workItemId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<CommentReaction>>(new HttpMethod("GET"), new Guid("f6cb3f27-1028-4851-af96-887e570dc21f"), (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CommentVersion> GetCommentVersionAsync(
      string project,
      int workItemId,
      int commentId,
      int version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentVersion>(new HttpMethod("GET"), new Guid("49e03b34-3be0-42e3-8a5d-e8dfb88ac954"), (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CommentVersion> GetCommentVersionAsync(
      Guid project,
      int workItemId,
      int commentId,
      int version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentVersion>(new HttpMethod("GET"), new Guid("49e03b34-3be0-42e3-8a5d-e8dfb88ac954"), (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId,
        version = version
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<CommentVersion>> GetCommentVersionsAsync(
      string project,
      int workItemId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<CommentVersion>>(new HttpMethod("GET"), new Guid("49e03b34-3be0-42e3-8a5d-e8dfb88ac954"), (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<CommentVersion>> GetCommentVersionsAsync(
      Guid project,
      int workItemId,
      int commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<CommentVersion>>(new HttpMethod("GET"), new Guid("49e03b34-3be0-42e3-8a5d-e8dfb88ac954"), (object) new
      {
        project = project,
        workItemId = workItemId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemField2> CreateWorkItemFieldAsync(
      WorkItemField2 workItemField,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemField2>(workItemField, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemField2>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemField2> CreateWorkItemFieldAsync(
      WorkItemField2 workItemField,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemField2>(workItemField, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemField2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemField2> CreateWorkItemFieldAsync(
      WorkItemField2 workItemField,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemField2>(workItemField, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemField2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteWorkItemFieldAsync(
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94"), (object) new
      {
        fieldNameOrRefName = fieldNameOrRefName
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteWorkItemFieldAsync(
      string project,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94"), (object) new
      {
        project = project,
        fieldNameOrRefName = fieldNameOrRefName
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteWorkItemFieldAsync(
      Guid project,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94"), (object) new
      {
        project = project,
        fieldNameOrRefName = fieldNameOrRefName
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<WorkItemField2> GetWorkItemFieldAsync(
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemField2>(new HttpMethod("GET"), new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94"), (object) new
      {
        fieldNameOrRefName = fieldNameOrRefName
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemField2> GetWorkItemFieldAsync(
      string project,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemField2>(new HttpMethod("GET"), new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94"), (object) new
      {
        project = project,
        fieldNameOrRefName = fieldNameOrRefName
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemField2> GetWorkItemFieldAsync(
      Guid project,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemField2>(new HttpMethod("GET"), new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94"), (object) new
      {
        project = project,
        fieldNameOrRefName = fieldNameOrRefName
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemField2>> GetWorkItemFieldsAsync(
      string project,
      GetFieldsExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemField2>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemField2>> GetWorkItemFieldsAsync(
      Guid project,
      GetFieldsExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemField2>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemField2>> GetWorkItemFieldsAsync(
      GetFieldsExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemField2>>(method, locationId, version: new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemField2> UpdateWorkItemFieldAsync(
      FieldUpdate payload,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object obj1 = (object) new
      {
        fieldNameOrRefName = fieldNameOrRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FieldUpdate>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemField2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemField2> UpdateWorkItemFieldAsync(
      FieldUpdate payload,
      string project,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object obj1 = (object) new
      {
        project = project,
        fieldNameOrRefName = fieldNameOrRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FieldUpdate>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemField2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemField2> UpdateWorkItemFieldAsync(
      FieldUpdate payload,
      Guid project,
      string fieldNameOrRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b51fd764-e5c2-4b9b-aaf7-3395cf4bdd94");
      object obj1 = (object) new
      {
        project = project,
        fieldNameOrRefName = fieldNameOrRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FieldUpdate>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemField2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitHubConnectionRepoModel>> GetGithubConnectionRepositoriesAsync(
      string project,
      Guid connectionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitHubConnectionRepoModel>>(new HttpMethod("GET"), new Guid("0b3a5212-f65b-2102-0d80-1dd77ce4c700"), (object) new
      {
        project = project,
        connectionId = connectionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitHubConnectionRepoModel>> GetGithubConnectionRepositoriesAsync(
      Guid project,
      Guid connectionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitHubConnectionRepoModel>>(new HttpMethod("GET"), new Guid("0b3a5212-f65b-2102-0d80-1dd77ce4c700"), (object) new
      {
        project = project,
        connectionId = connectionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitHubConnectionRepoModel>> UpdateGithubConnectionReposAsync(
      GitHubConnectionReposBatchRequest reposOperationData,
      string project,
      Guid connectionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("15b19676-8d9e-e224-d795-ca4d1a18024d");
      object obj1 = (object) new
      {
        project = project,
        connectionId = connectionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitHubConnectionReposBatchRequest>(reposOperationData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitHubConnectionRepoModel>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitHubConnectionRepoModel>> UpdateGithubConnectionReposAsync(
      GitHubConnectionReposBatchRequest reposOperationData,
      Guid project,
      Guid connectionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("15b19676-8d9e-e224-d795-ca4d1a18024d");
      object obj1 = (object) new
      {
        project = project,
        connectionId = connectionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GitHubConnectionReposBatchRequest>(reposOperationData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GitHubConnectionRepoModel>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitHubConnectionModel>> GetGithubConnectionsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitHubConnectionModel>>(new HttpMethod("GET"), new Guid("0cf95f86-6ce1-f410-ccf6-3d8c92b3a1ef"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GitHubConnectionModel>> GetGithubConnectionsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GitHubConnectionModel>>(new HttpMethod("GET"), new Guid("0cf95f86-6ce1-f410-ccf6-3d8c92b3a1ef"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ProcessMigrationResultModel> MigrateProjectsProcessAsync(
      ProcessIdModel newProcess,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("19801631-d4e5-47e9-8166-0330de0ff1e6");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ProcessIdModel>(newProcess, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessMigrationResultModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ProcessMigrationResultModel> MigrateProjectsProcessAsync(
      ProcessIdModel newProcess,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("19801631-d4e5-47e9-8166-0330de0ff1e6");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ProcessIdModel>(newProcess, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessMigrationResultModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<QueryHierarchyItem> CreateQueryAsync(
      QueryHierarchyItem postedQuery,
      string project,
      string query,
      bool? validateWiqlOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object obj1 = (object) new
      {
        project = project,
        query = query
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryHierarchyItem>(postedQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (validateWiqlOnly.HasValue)
        collection.Add(nameof (validateWiqlOnly), validateWiqlOnly.Value.ToString());
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
      return this.SendAsync<QueryHierarchyItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<QueryHierarchyItem> CreateQueryAsync(
      QueryHierarchyItem postedQuery,
      Guid project,
      string query,
      bool? validateWiqlOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object obj1 = (object) new
      {
        project = project,
        query = query
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryHierarchyItem>(postedQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (validateWiqlOnly.HasValue)
        collection.Add(nameof (validateWiqlOnly), validateWiqlOnly.Value.ToString());
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
      return this.SendAsync<QueryHierarchyItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual async Task DeleteQueryAsync(
      string project,
      string query,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("a67d190c-c41f-424b-814d-0e906f659301"), (object) new
      {
        project = project,
        query = query
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteQueryAsync(
      Guid project,
      string query,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("a67d190c-c41f-424b-814d-0e906f659301"), (object) new
      {
        project = project,
        query = query
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<QueryHierarchyItem>> GetQueriesAsync(
      string project,
      QueryExpand? expand = null,
      int? depth = null,
      bool? includeDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (depth.HasValue)
        keyValuePairList.Add("$depth", depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeDeleted.HasValue)
        keyValuePairList.Add("$includeDeleted", includeDeleted.Value.ToString());
      return this.SendAsync<List<QueryHierarchyItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<QueryHierarchyItem>> GetQueriesAsync(
      Guid project,
      QueryExpand? expand = null,
      int? depth = null,
      bool? includeDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (depth.HasValue)
        keyValuePairList.Add("$depth", depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeDeleted.HasValue)
        keyValuePairList.Add("$includeDeleted", includeDeleted.Value.ToString());
      return this.SendAsync<List<QueryHierarchyItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<QueryHierarchyItem> GetQueryAsync(
      string project,
      string query,
      QueryExpand? expand = null,
      int? depth = null,
      bool? includeDeleted = null,
      bool? useIsoDateFormat = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object routeValues = (object) new
      {
        project = project,
        query = query
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (depth.HasValue)
        keyValuePairList.Add("$depth", depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add("$includeDeleted", str);
      }
      if (useIsoDateFormat.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = useIsoDateFormat.Value;
        string str = flag.ToString();
        collection.Add("$useIsoDateFormat", str);
      }
      return this.SendAsync<QueryHierarchyItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<QueryHierarchyItem> GetQueryAsync(
      Guid project,
      string query,
      QueryExpand? expand = null,
      int? depth = null,
      bool? includeDeleted = null,
      bool? useIsoDateFormat = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object routeValues = (object) new
      {
        project = project,
        query = query
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (depth.HasValue)
        keyValuePairList.Add("$depth", depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      bool flag;
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add("$includeDeleted", str);
      }
      if (useIsoDateFormat.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = useIsoDateFormat.Value;
        string str = flag.ToString();
        collection.Add("$useIsoDateFormat", str);
      }
      return this.SendAsync<QueryHierarchyItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<QueryHierarchyItemsResult> SearchQueriesAsync(
      string project,
      string filter,
      int? top = null,
      QueryExpand? expand = null,
      bool? includeDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$filter", filter);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (includeDeleted.HasValue)
        keyValuePairList.Add("$includeDeleted", includeDeleted.Value.ToString());
      return this.SendAsync<QueryHierarchyItemsResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<QueryHierarchyItemsResult> SearchQueriesAsync(
      Guid project,
      string filter,
      int? top = null,
      QueryExpand? expand = null,
      bool? includeDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$filter", filter);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (includeDeleted.HasValue)
        keyValuePairList.Add("$includeDeleted", includeDeleted.Value.ToString());
      return this.SendAsync<QueryHierarchyItemsResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<QueryHierarchyItem> UpdateQueryAsync(
      QueryHierarchyItem queryUpdate,
      string project,
      string query,
      bool? undeleteDescendants = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object obj1 = (object) new
      {
        project = project,
        query = query
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryHierarchyItem>(queryUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (undeleteDescendants.HasValue)
        collection.Add("$undeleteDescendants", undeleteDescendants.Value.ToString());
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
      return this.SendAsync<QueryHierarchyItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<QueryHierarchyItem> UpdateQueryAsync(
      QueryHierarchyItem queryUpdate,
      Guid project,
      string query,
      bool? undeleteDescendants = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a67d190c-c41f-424b-814d-0e906f659301");
      object obj1 = (object) new
      {
        project = project,
        query = query
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryHierarchyItem>(queryUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (undeleteDescendants.HasValue)
        collection.Add("$undeleteDescendants", undeleteDescendants.Value.ToString());
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
      return this.SendAsync<QueryHierarchyItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<QueryHierarchyItem>> GetQueriesBatchAsync(
      QueryBatchGetRequest queryGetRequest,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("549816f9-09b0-4e75-9e81-01fbfcd07426");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryBatchGetRequest>(queryGetRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<QueryHierarchyItem>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<QueryHierarchyItem>> GetQueriesBatchAsync(
      QueryBatchGetRequest queryGetRequest,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("549816f9-09b0-4e75-9e81-01fbfcd07426");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<QueryBatchGetRequest>(queryGetRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<QueryHierarchyItem>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DestroyWorkItemAsync(
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0"), (object) new
      {
        id = id
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DestroyWorkItemAsync(
      string project,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0"), (object) new
      {
        project = project,
        id = id
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DestroyWorkItemAsync(
      Guid project,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0"), (object) new
      {
        project = project,
        id = id
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<WorkItemDelete> GetDeletedWorkItemAsync(
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemDelete>(new HttpMethod("GET"), new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0"), (object) new
      {
        id = id
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemDelete> GetDeletedWorkItemAsync(
      string project,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemDelete>(new HttpMethod("GET"), new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0"), (object) new
      {
        project = project,
        id = id
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemDelete> GetDeletedWorkItemAsync(
      Guid project,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemDelete>(new HttpMethod("GET"), new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0"), (object) new
      {
        project = project,
        id = id
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemDeleteReference>> GetDeletedWorkItemsAsync(
      string project,
      IEnumerable<int> ids,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      return this.SendAsync<List<WorkItemDeleteReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemDeleteReference>> GetDeletedWorkItemsAsync(
      Guid project,
      IEnumerable<int> ids,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      return this.SendAsync<List<WorkItemDeleteReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemDeleteReference>> GetDeletedWorkItemsAsync(
      IEnumerable<int> ids,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      return this.SendAsync<List<WorkItemDeleteReference>>(method, locationId, version: new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemDeleteShallowReference>> GetDeletedWorkItemShallowReferencesAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemDeleteShallowReference>>(new HttpMethod("GET"), new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0"), version: new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemDeleteShallowReference>> GetDeletedWorkItemShallowReferencesAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemDeleteShallowReference>>(new HttpMethod("GET"), new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemDeleteShallowReference>> GetDeletedWorkItemShallowReferencesAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemDeleteShallowReference>>(new HttpMethod("GET"), new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemDelete> RestoreWorkItemAsync(
      WorkItemDeleteUpdate payload,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0");
      object obj1 = (object) new{ id = id };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemDeleteUpdate>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemDelete>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemDelete> RestoreWorkItemAsync(
      WorkItemDeleteUpdate payload,
      string project,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0");
      object obj1 = (object) new
      {
        project = project,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemDeleteUpdate>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemDelete>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemDelete> RestoreWorkItemAsync(
      WorkItemDeleteUpdate payload,
      Guid project,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b70d8d39-926c-465e-b927-b1bf0e5ca0e0");
      object obj1 = (object) new
      {
        project = project,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemDeleteUpdate>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemDelete>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItem> GetRevisionAsync(
      string project,
      int id,
      int revisionNumber,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a00c85a5-80fa-4565-99c3-bcd2181434bb");
      object routeValues = (object) new
      {
        project = project,
        id = id,
        revisionNumber = revisionNumber
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItem> GetRevisionAsync(
      Guid project,
      int id,
      int revisionNumber,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a00c85a5-80fa-4565-99c3-bcd2181434bb");
      object routeValues = (object) new
      {
        project = project,
        id = id,
        revisionNumber = revisionNumber
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItem> GetRevisionAsync(
      int id,
      int revisionNumber,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a00c85a5-80fa-4565-99c3-bcd2181434bb");
      object routeValues = (object) new
      {
        id = id,
        revisionNumber = revisionNumber
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItem>> GetRevisionsAsync(
      string project,
      int id,
      int? top = null,
      int? skip = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a00c85a5-80fa-4565-99c3-bcd2181434bb");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItem>> GetRevisionsAsync(
      Guid project,
      int id,
      int? top = null,
      int? skip = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a00c85a5-80fa-4565-99c3-bcd2181434bb");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItem>> GetRevisionsAsync(
      int id,
      int? top = null,
      int? skip = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a00c85a5-80fa-4565-99c3-bcd2181434bb");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task SendMailAsync(
      SendMailBody body,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("12438500-2f84-4fa7-9f1a-c31871b4959d");
      HttpContent httpContent = (HttpContent) new ObjectContent<SendMailBody>(body, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      WorkItemTrackingHttpClientBase trackingHttpClientBase2 = trackingHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await trackingHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SendMailAsync(
      SendMailBody body,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("12438500-2f84-4fa7-9f1a-c31871b4959d");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<SendMailBody>(body, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      WorkItemTrackingHttpClientBase trackingHttpClientBase2 = trackingHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await trackingHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SendMailAsync(
      SendMailBody body,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("12438500-2f84-4fa7-9f1a-c31871b4959d");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<SendMailBody>(body, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      WorkItemTrackingHttpClientBase trackingHttpClientBase2 = trackingHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await trackingHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTagAsync(
      string project,
      string tagIdOrName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("bc15bc60-e7a8-43cb-ab01-2106be3983a1"), (object) new
      {
        project = project,
        tagIdOrName = tagIdOrName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTagAsync(
      Guid project,
      string tagIdOrName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("bc15bc60-e7a8-43cb-ab01-2106be3983a1"), (object) new
      {
        project = project,
        tagIdOrName = tagIdOrName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<WorkItemTagDefinition> GetTagAsync(
      string project,
      string tagIdOrName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemTagDefinition>(new HttpMethod("GET"), new Guid("bc15bc60-e7a8-43cb-ab01-2106be3983a1"), (object) new
      {
        project = project,
        tagIdOrName = tagIdOrName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemTagDefinition> GetTagAsync(
      Guid project,
      string tagIdOrName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemTagDefinition>(new HttpMethod("GET"), new Guid("bc15bc60-e7a8-43cb-ab01-2106be3983a1"), (object) new
      {
        project = project,
        tagIdOrName = tagIdOrName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemTagDefinition>> GetTagsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemTagDefinition>>(new HttpMethod("GET"), new Guid("bc15bc60-e7a8-43cb-ab01-2106be3983a1"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemTagDefinition>> GetTagsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemTagDefinition>>(new HttpMethod("GET"), new Guid("bc15bc60-e7a8-43cb-ab01-2106be3983a1"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemTagDefinition> UpdateTagAsync(
      WorkItemTagDefinition tagData,
      string project,
      string tagIdOrName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("bc15bc60-e7a8-43cb-ab01-2106be3983a1");
      object obj1 = (object) new
      {
        project = project,
        tagIdOrName = tagIdOrName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTagDefinition>(tagData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemTagDefinition>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemTagDefinition> UpdateTagAsync(
      WorkItemTagDefinition tagData,
      Guid project,
      string tagIdOrName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("bc15bc60-e7a8-43cb-ab01-2106be3983a1");
      object obj1 = (object) new
      {
        project = project,
        tagIdOrName = tagIdOrName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTagDefinition>(tagData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemTagDefinition>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemTemplate> CreateTemplateAsync(
      WorkItemTemplate template,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6a90345f-a676-4969-afce-8e163e1d5642");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTemplate>(template, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemTemplate>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<WorkItemTemplateReference>> GetTemplatesAsync(
      TeamContext teamContext,
      string workitemtypename = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6a90345f-a676-4969-afce-8e163e1d5642");
      string str3 = str2;
      object routeValues = (object) new
      {
        project = str1,
        team = str3
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (workitemtypename != null)
        keyValuePairList.Add(nameof (workitemtypename), workitemtypename);
      return this.SendAsync<List<WorkItemTemplateReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteTemplateAsync(
      TeamContext teamContext,
      Guid templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("fb10264a-8836-48a0-8033-1b0ccd2748d5");
      string str3 = str2;
      Guid guid = templateId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        templateId = guid
      };
      using (await trackingHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<WorkItemTemplate> GetTemplateAsync(
      TeamContext teamContext,
      Guid templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fb10264a-8836-48a0-8033-1b0ccd2748d5");
      string str3 = str2;
      Guid guid = templateId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        templateId = guid
      };
      return this.SendAsync<WorkItemTemplate>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemTemplate> ReplaceTemplateAsync(
      WorkItemTemplate templateContent,
      TeamContext teamContext,
      Guid templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid1 = new Guid("fb10264a-8836-48a0-8033-1b0ccd2748d5");
      string str3 = str2;
      Guid guid2 = templateId;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        templateId = guid2
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTemplate>(templateContent, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid1;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemTemplate>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TemporaryQueryResponseModel> CreateTempQueryAsync(
      TemporaryQueryRequestModel postedQuery,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9f614388-a9f0-4952-ad6c-89756bd8e388");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TemporaryQueryRequestModel>(postedQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TemporaryQueryResponseModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TemporaryQueryResponseModel> CreateTempQueryAsync(
      TemporaryQueryRequestModel postedQuery,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9f614388-a9f0-4952-ad6c-89756bd8e388");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TemporaryQueryRequestModel>(postedQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TemporaryQueryResponseModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemUpdate> GetUpdateAsync(
      int id,
      int updateNumber,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemUpdate>(new HttpMethod("GET"), new Guid("6570bf97-d02c-4a91-8d93-3abe9895b1a9"), (object) new
      {
        id = id,
        updateNumber = updateNumber
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemUpdate> GetUpdateAsync(
      string project,
      int id,
      int updateNumber,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemUpdate>(new HttpMethod("GET"), new Guid("6570bf97-d02c-4a91-8d93-3abe9895b1a9"), (object) new
      {
        project = project,
        id = id,
        updateNumber = updateNumber
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemUpdate> GetUpdateAsync(
      Guid project,
      int id,
      int updateNumber,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemUpdate>(new HttpMethod("GET"), new Guid("6570bf97-d02c-4a91-8d93-3abe9895b1a9"), (object) new
      {
        project = project,
        id = id,
        updateNumber = updateNumber
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemUpdate>> GetUpdatesAsync(
      string project,
      int id,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6570bf97-d02c-4a91-8d93-3abe9895b1a9");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<WorkItemUpdate>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemUpdate>> GetUpdatesAsync(
      Guid project,
      int id,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6570bf97-d02c-4a91-8d93-3abe9895b1a9");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<WorkItemUpdate>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemUpdate>> GetUpdatesAsync(
      int id,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6570bf97-d02c-4a91-8d93-3abe9895b1a9");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<WorkItemUpdate>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemQueryResult> QueryByWiqlAsync(
      Wiql wiql,
      bool? timePrecision = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1a9c53f7-f243-4447-b110-35ef023636e4");
      HttpContent httpContent = (HttpContent) new ObjectContent<Wiql>(wiql, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        collection.Add(nameof (timePrecision), timePrecision.Value.ToString());
      if (top.HasValue)
        collection.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemQueryResult>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemQueryResult> QueryByWiqlAsync(
      Wiql wiql,
      TeamContext teamContext,
      bool? timePrecision = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1a9c53f7-f243-4447-b110-35ef023636e4");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Wiql>(wiql, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        collection.Add(nameof (timePrecision), timePrecision.Value.ToString());
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
      return this.SendAsync<WorkItemQueryResult>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WorkItemQueryResult> QueryByWiqlAsync(
      Wiql wiql,
      string project,
      bool? timePrecision = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1a9c53f7-f243-4447-b110-35ef023636e4");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Wiql>(wiql, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        collection.Add(nameof (timePrecision), timePrecision.Value.ToString());
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
      return this.SendAsync<WorkItemQueryResult>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WorkItemQueryResult> QueryByWiqlAsync(
      Wiql wiql,
      Guid project,
      bool? timePrecision = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1a9c53f7-f243-4447-b110-35ef023636e4");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Wiql>(wiql, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        collection.Add(nameof (timePrecision), timePrecision.Value.ToString());
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
      return this.SendAsync<WorkItemQueryResult>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual async Task<int> GetQueryResultCountAsync(
      TeamContext teamContext,
      Guid id,
      bool? timePrecision = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("HEAD");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      string str3 = str2;
      Guid guid = id;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        id = guid
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage response = await trackingHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return (int) ConvertUtility.ChangeType((object) trackingHttpClientBase.GetHeaderValue(response, "X-Total-Count").FirstOrDefault<string>(), typeof (int));
    }

    public virtual async Task<int> GetQueryResultCountAsync(
      string project,
      Guid id,
      bool? timePrecision = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("HEAD");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage response = await trackingHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return (int) ConvertUtility.ChangeType((object) trackingHttpClientBase.GetHeaderValue(response, "X-Total-Count").FirstOrDefault<string>(), typeof (int));
    }

    public virtual async Task<int> GetQueryResultCountAsync(
      Guid project,
      Guid id,
      bool? timePrecision = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("HEAD");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage response = await trackingHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return (int) ConvertUtility.ChangeType((object) trackingHttpClientBase.GetHeaderValue(response, "X-Total-Count").FirstOrDefault<string>(), typeof (int));
    }

    public virtual async Task<int> GetQueryResultCountAsync(
      Guid id,
      bool? timePrecision = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("HEAD");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage response = await trackingHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return (int) ConvertUtility.ChangeType((object) trackingHttpClientBase.GetHeaderValue(response, "X-Total-Count").FirstOrDefault<string>(), typeof (int));
    }

    public virtual Task<WorkItemQueryResult> QueryByIdAsync(
      TeamContext teamContext,
      Guid id,
      bool? timePrecision = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      string str3 = str2;
      Guid guid = id;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        id = guid
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<WorkItemQueryResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemQueryResult> QueryByIdAsync(
      string project,
      Guid id,
      bool? timePrecision = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<WorkItemQueryResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemQueryResult> QueryByIdAsync(
      Guid project,
      Guid id,
      bool? timePrecision = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<WorkItemQueryResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemQueryResult> QueryByIdAsync(
      Guid id,
      bool? timePrecision = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a02355f5-5f8a-4671-8e32-369d23aac83d");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (timePrecision.HasValue)
        keyValuePairList.Add(nameof (timePrecision), timePrecision.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<WorkItemQueryResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemFieldAllowedValues> GetWorkItemFieldAllowedValuesAsync(
      string project,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1d4da553-5856-4ca5-a3b3-79e0e8fcc142");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (name), name);
      return this.SendAsync<WorkItemFieldAllowedValues>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemFieldAllowedValues> GetWorkItemFieldAllowedValuesAsync(
      Guid project,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1d4da553-5856-4ca5-a3b3-79e0e8fcc142");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (name), name);
      return this.SendAsync<WorkItemFieldAllowedValues>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemIcon> GetWorkItemIconJsonAsync(
      string icon,
      string color = null,
      int? v = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4e1eb4a5-1970-4228-a682-ec48eb2dca30");
      object routeValues = (object) new{ icon = icon };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (color != null)
        keyValuePairList.Add(nameof (color), color);
      if (v.HasValue)
        keyValuePairList.Add(nameof (v), v.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<WorkItemIcon>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemIcon>> GetWorkItemIconsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemIcon>>(new HttpMethod("GET"), new Guid("4e1eb4a5-1970-4228-a682-ec48eb2dca30"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetWorkItemIconSvgAsync(
      string icon,
      string color = null,
      int? v = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4e1eb4a5-1970-4228-a682-ec48eb2dca30");
      object routeValues = (object) new{ icon = icon };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (color != null)
        keyValuePairList.Add(nameof (color), color);
      if (v.HasValue)
        keyValuePairList.Add(nameof (v), v.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await trackingHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "image/svg+xml").ConfigureAwait(false))
        httpResponseMessage = await trackingHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetWorkItemIconXamlAsync(
      string icon,
      string color = null,
      int? v = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WorkItemTrackingHttpClientBase trackingHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4e1eb4a5-1970-4228-a682-ec48eb2dca30");
      object routeValues = (object) new{ icon = icon };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (color != null)
        keyValuePairList.Add(nameof (color), color);
      if (v.HasValue)
        keyValuePairList.Add(nameof (v), v.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await trackingHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "image/xaml+xml").ConfigureAwait(false))
        httpResponseMessage = await trackingHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<ReportingWorkItemLinksBatch> GetReportingLinksByLinkTypeAsync(
      string project,
      IEnumerable<string> linkTypes = null,
      IEnumerable<string> types = null,
      string continuationToken = null,
      DateTime? startDateTime = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b5b5b6d0-0308-40a1-b3f4-b9bb3c66878f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (linkTypes != null && linkTypes.Any<string>())
        keyValuePairList.Add(nameof (linkTypes), string.Join(",", linkTypes));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      return this.SendAsync<ReportingWorkItemLinksBatch>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReportingWorkItemLinksBatch> GetReportingLinksByLinkTypeAsync(
      Guid project,
      IEnumerable<string> linkTypes = null,
      IEnumerable<string> types = null,
      string continuationToken = null,
      DateTime? startDateTime = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b5b5b6d0-0308-40a1-b3f4-b9bb3c66878f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (linkTypes != null && linkTypes.Any<string>())
        keyValuePairList.Add(nameof (linkTypes), string.Join(",", linkTypes));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      return this.SendAsync<ReportingWorkItemLinksBatch>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReportingWorkItemLinksBatch> GetReportingLinksByLinkTypeAsync(
      IEnumerable<string> linkTypes = null,
      IEnumerable<string> types = null,
      string continuationToken = null,
      DateTime? startDateTime = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b5b5b6d0-0308-40a1-b3f4-b9bb3c66878f");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (linkTypes != null && linkTypes.Any<string>())
        keyValuePairList.Add(nameof (linkTypes), string.Join(",", linkTypes));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      return this.SendAsync<ReportingWorkItemLinksBatch>(method, locationId, version: new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemRelationType> GetRelationTypeAsync(
      string relation,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemRelationType>(new HttpMethod("GET"), new Guid("f5d33bc9-5b49-4a3c-a9bd-f3cd46dd2165"), (object) new
      {
        relation = relation
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemRelationType>> GetRelationTypesAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemRelationType>>(new HttpMethod("GET"), new Guid("f5d33bc9-5b49-4a3c-a9bd-f3cd46dd2165"), version: new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      string project,
      IEnumerable<string> fields = null,
      IEnumerable<string> types = null,
      string continuationToken = null,
      DateTime? startDateTime = null,
      bool? includeIdentityRef = null,
      bool? includeDeleted = null,
      bool? includeTagRef = null,
      bool? includeLatestOnly = null,
      ReportingRevisionsExpand? expand = null,
      bool? includeDiscussionChangesOnly = null,
      int? maxPageSize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      bool flag;
      if (includeIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIdentityRef), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeTagRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeTagRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeTagRef), str);
      }
      if (includeLatestOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestOnly), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (includeDiscussionChangesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDiscussionChangesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDiscussionChangesOnly), str);
      }
      if (maxPageSize.HasValue)
        keyValuePairList.Add("$maxPageSize", maxPageSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      Guid project,
      IEnumerable<string> fields = null,
      IEnumerable<string> types = null,
      string continuationToken = null,
      DateTime? startDateTime = null,
      bool? includeIdentityRef = null,
      bool? includeDeleted = null,
      bool? includeTagRef = null,
      bool? includeLatestOnly = null,
      ReportingRevisionsExpand? expand = null,
      bool? includeDiscussionChangesOnly = null,
      int? maxPageSize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      bool flag;
      if (includeIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIdentityRef), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeTagRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeTagRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeTagRef), str);
      }
      if (includeLatestOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestOnly), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (includeDiscussionChangesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDiscussionChangesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDiscussionChangesOnly), str);
      }
      if (maxPageSize.HasValue)
        keyValuePairList.Add("$maxPageSize", maxPageSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsGetAsync(
      IEnumerable<string> fields = null,
      IEnumerable<string> types = null,
      string continuationToken = null,
      DateTime? startDateTime = null,
      bool? includeIdentityRef = null,
      bool? includeDeleted = null,
      bool? includeTagRef = null,
      bool? includeLatestOnly = null,
      ReportingRevisionsExpand? expand = null,
      bool? includeDiscussionChangesOnly = null,
      int? maxPageSize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (types != null && types.Any<string>())
        keyValuePairList.Add(nameof (types), string.Join(",", types));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      bool flag;
      if (includeIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIdentityRef), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeTagRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeTagRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeTagRef), str);
      }
      if (includeLatestOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestOnly), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (includeDiscussionChangesOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDiscussionChangesOnly.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDiscussionChangesOnly), str);
      }
      if (maxPageSize.HasValue)
        keyValuePairList.Add("$maxPageSize", maxPageSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, version: new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsPostAsync(
      ReportingWorkItemRevisionsFilter filter,
      string continuationToken = null,
      DateTime? startDateTime = null,
      ReportingRevisionsExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      HttpContent httpContent = (HttpContent) new ObjectContent<ReportingWorkItemRevisionsFilter>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsPostAsync(
      ReportingWorkItemRevisionsFilter filter,
      string project,
      string continuationToken = null,
      DateTime? startDateTime = null,
      ReportingRevisionsExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReportingWorkItemRevisionsFilter>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingRevisionsPostAsync(
      ReportingWorkItemRevisionsFilter filter,
      Guid project,
      string continuationToken = null,
      DateTime? startDateTime = null,
      ReportingRevisionsExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f828fe59-dd87-495d-a17c-7a8d6211ca6c");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReportingWorkItemRevisionsFilter>(filter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (startDateTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startDateTime), startDateTime.Value);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingDiscussionsAsync(
      string project,
      string continuationToken = null,
      int? maxPageSize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4a644469-90c5-4fcc-9a9f-be0827d369ec");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (maxPageSize.HasValue)
        keyValuePairList.Add("$maxPageSize", maxPageSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingDiscussionsAsync(
      Guid project,
      string continuationToken = null,
      int? maxPageSize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4a644469-90c5-4fcc-9a9f-be0827d369ec");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (maxPageSize.HasValue)
        keyValuePairList.Add("$maxPageSize", maxPageSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReportingWorkItemRevisionsBatch> ReadReportingDiscussionsAsync(
      string continuationToken = null,
      int? maxPageSize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4a644469-90c5-4fcc-9a9f-be0827d369ec");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (maxPageSize.HasValue)
        keyValuePairList.Add("$maxPageSize", maxPageSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<ReportingWorkItemRevisionsBatch>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItem> CreateWorkItemAsync(
      JsonPatchDocument document,
      string project,
      string type,
      bool? validateOnly = null,
      bool? bypassRules = null,
      bool? suppressNotifications = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("62d3d110-0047-428c-ad3c-4fe872c91c74");
      object obj1 = (object) new
      {
        project = project,
        type = type
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection2.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection3.Add(nameof (bypassRules), str);
      }
      if (suppressNotifications.HasValue)
      {
        List<KeyValuePair<string, string>> collection4 = collection1;
        flag = suppressNotifications.Value;
        string str = flag.ToString();
        collection4.Add(nameof (suppressNotifications), str);
      }
      if (expand.HasValue)
        collection1.Add("$expand", expand.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WorkItem> CreateWorkItemAsync(
      JsonPatchDocument document,
      Guid project,
      string type,
      bool? validateOnly = null,
      bool? bypassRules = null,
      bool? suppressNotifications = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("62d3d110-0047-428c-ad3c-4fe872c91c74");
      object obj1 = (object) new
      {
        project = project,
        type = type
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection2.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection3.Add(nameof (bypassRules), str);
      }
      if (suppressNotifications.HasValue)
      {
        List<KeyValuePair<string, string>> collection4 = collection1;
        flag = suppressNotifications.Value;
        string str = flag.ToString();
        collection4.Add(nameof (suppressNotifications), str);
      }
      if (expand.HasValue)
        collection1.Add("$expand", expand.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WorkItem> GetWorkItemTemplateAsync(
      string project,
      string type,
      string fields = null,
      DateTime? asOf = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("62d3d110-0047-428c-ad3c-4fe872c91c74");
      object routeValues = (object) new
      {
        project = project,
        type = type
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null)
        keyValuePairList.Add(nameof (fields), fields);
      if (asOf.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (asOf), asOf.Value);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItem> GetWorkItemTemplateAsync(
      Guid project,
      string type,
      string fields = null,
      DateTime? asOf = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("62d3d110-0047-428c-ad3c-4fe872c91c74");
      object routeValues = (object) new
      {
        project = project,
        type = type
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null)
        keyValuePairList.Add(nameof (fields), fields);
      if (asOf.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (asOf), asOf.Value);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemDelete> DeleteWorkItemAsync(
      string project,
      int id,
      bool? destroy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (destroy.HasValue)
        keyValuePairList.Add(nameof (destroy), destroy.Value.ToString());
      return this.SendAsync<WorkItemDelete>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemDelete> DeleteWorkItemAsync(
      Guid project,
      int id,
      bool? destroy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (destroy.HasValue)
        keyValuePairList.Add(nameof (destroy), destroy.Value.ToString());
      return this.SendAsync<WorkItemDelete>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemDelete> DeleteWorkItemAsync(
      int id,
      bool? destroy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (destroy.HasValue)
        keyValuePairList.Add(nameof (destroy), destroy.Value.ToString());
      return this.SendAsync<WorkItemDelete>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItem> GetWorkItemAsync(
      string project,
      int id,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (asOf.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (asOf), asOf.Value);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItem> GetWorkItemAsync(
      Guid project,
      int id,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (asOf.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (asOf), asOf.Value);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItem> GetWorkItemAsync(
      int id,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (asOf.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (asOf), asOf.Value);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItem>> GetWorkItemsAsync(
      string project,
      IEnumerable<int> ids,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand? expand = null,
      WorkItemErrorPolicy? errorPolicy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (asOf.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (asOf), asOf.Value);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (errorPolicy.HasValue)
        keyValuePairList.Add(nameof (errorPolicy), errorPolicy.Value.ToString());
      return this.SendAsync<List<WorkItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItem>> GetWorkItemsAsync(
      Guid project,
      IEnumerable<int> ids,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand? expand = null,
      WorkItemErrorPolicy? errorPolicy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (asOf.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (asOf), asOf.Value);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (errorPolicy.HasValue)
        keyValuePairList.Add(nameof (errorPolicy), errorPolicy.Value.ToString());
      return this.SendAsync<List<WorkItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItem>> GetWorkItemsAsync(
      IEnumerable<int> ids,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand? expand = null,
      WorkItemErrorPolicy? errorPolicy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (asOf.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (asOf), asOf.Value);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (errorPolicy.HasValue)
        keyValuePairList.Add(nameof (errorPolicy), errorPolicy.Value.ToString());
      return this.SendAsync<List<WorkItem>>(method, locationId, version: new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItem> UpdateWorkItemAsync(
      JsonPatchDocument document,
      int id,
      bool? validateOnly = null,
      bool? bypassRules = null,
      bool? suppressNotifications = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object obj1 = (object) new{ id = id };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection2.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection3.Add(nameof (bypassRules), str);
      }
      if (suppressNotifications.HasValue)
      {
        List<KeyValuePair<string, string>> collection4 = collection1;
        flag = suppressNotifications.Value;
        string str = flag.ToString();
        collection4.Add(nameof (suppressNotifications), str);
      }
      if (expand.HasValue)
        collection1.Add("$expand", expand.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WorkItem> UpdateWorkItemAsync(
      JsonPatchDocument document,
      string project,
      int id,
      bool? validateOnly = null,
      bool? bypassRules = null,
      bool? suppressNotifications = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object obj1 = (object) new
      {
        project = project,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection2.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection3.Add(nameof (bypassRules), str);
      }
      if (suppressNotifications.HasValue)
      {
        List<KeyValuePair<string, string>> collection4 = collection1;
        flag = suppressNotifications.Value;
        string str = flag.ToString();
        collection4.Add(nameof (suppressNotifications), str);
      }
      if (expand.HasValue)
        collection1.Add("$expand", expand.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<WorkItem> UpdateWorkItemAsync(
      JsonPatchDocument document,
      Guid project,
      int id,
      bool? validateOnly = null,
      bool? bypassRules = null,
      bool? suppressNotifications = null,
      WorkItemExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("72c7ddf8-2cdc-4f60-90cd-ab71c14a399b");
      object obj1 = (object) new
      {
        project = project,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(document, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      bool flag;
      if (validateOnly.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        flag = validateOnly.Value;
        string str = flag.ToString();
        collection2.Add(nameof (validateOnly), str);
      }
      if (bypassRules.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        flag = bypassRules.Value;
        string str = flag.ToString();
        collection3.Add(nameof (bypassRules), str);
      }
      if (suppressNotifications.HasValue)
      {
        List<KeyValuePair<string, string>> collection4 = collection1;
        flag = suppressNotifications.Value;
        string str = flag.ToString();
        collection4.Add(nameof (suppressNotifications), str);
      }
      if (expand.HasValue)
        collection1.Add("$expand", expand.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItem>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<WorkItem>> GetWorkItemsBatchAsync(
      WorkItemBatchGetRequest workItemGetRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("908509b6-4248-4475-a1cd-829139ba419f");
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemBatchGetRequest>(workItemGetRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<WorkItem>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<WorkItem>> GetWorkItemsBatchAsync(
      WorkItemBatchGetRequest workItemGetRequest,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("908509b6-4248-4475-a1cd-829139ba419f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemBatchGetRequest>(workItemGetRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<WorkItem>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<WorkItem>> GetWorkItemsBatchAsync(
      WorkItemBatchGetRequest workItemGetRequest,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("908509b6-4248-4475-a1cd-829139ba419f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemBatchGetRequest>(workItemGetRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<WorkItem>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemDeleteBatch> DeleteWorkItemsAsync(
      WorkItemDeleteBatchRequest deleteRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8bc57545-27e5-420d-b709-f6e3ebcc1fc1");
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemDeleteBatchRequest>(deleteRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemDeleteBatch>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemDeleteBatch> DeleteWorkItemsAsync(
      WorkItemDeleteBatchRequest deleteRequest,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8bc57545-27e5-420d-b709-f6e3ebcc1fc1");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemDeleteBatchRequest>(deleteRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemDeleteBatch>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemDeleteBatch> DeleteWorkItemsAsync(
      WorkItemDeleteBatchRequest deleteRequest,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8bc57545-27e5-420d-b709-f6e3ebcc1fc1");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemDeleteBatchRequest>(deleteRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemDeleteBatch>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ProjectWorkItemStateColors>> GetWorkItemStateColorsAsync(
      string[] projectNames,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0b83df8a-3496-4ddb-ba44-63634f4cda61");
      HttpContent httpContent = (HttpContent) new ObjectContent<string[]>(projectNames, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ProjectWorkItemStateColors>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<WorkItemNextStateOnTransition>> GetWorkItemNextStatesOnCheckinActionAsync(
      IEnumerable<int> ids,
      string action = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("afae844b-e2f6-44c2-8053-17b3bb936a40");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (ids != null)
        str = string.Join<int>(",", ids);
      keyValuePairList.Add(nameof (ids), str);
      if (action != null)
        keyValuePairList.Add(nameof (action), action);
      return this.SendAsync<List<WorkItemNextStateOnTransition>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemTypeCategory>> GetWorkItemTypeCategoriesAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemTypeCategory>>(new HttpMethod("GET"), new Guid("9b9f5734-36c8-415e-ba67-f83b45c31408"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemTypeCategory>> GetWorkItemTypeCategoriesAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemTypeCategory>>(new HttpMethod("GET"), new Guid("9b9f5734-36c8-415e-ba67-f83b45c31408"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemTypeCategory> GetWorkItemTypeCategoryAsync(
      string project,
      string category,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemTypeCategory>(new HttpMethod("GET"), new Guid("9b9f5734-36c8-415e-ba67-f83b45c31408"), (object) new
      {
        project = project,
        category = category
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemTypeCategory> GetWorkItemTypeCategoryAsync(
      Guid project,
      string category,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemTypeCategory>(new HttpMethod("GET"), new Guid("9b9f5734-36c8-415e-ba67-f83b45c31408"), (object) new
      {
        project = project,
        category = category
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<KeyValuePair<string, List<WorkItemTypeColor>>>> GetWorkItemTypeColorsAsync(
      string[] projectNames,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("958fde80-115e-43fb-bd65-749c48057faf");
      HttpContent httpContent = (HttpContent) new ObjectContent<string[]>(projectNames, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<KeyValuePair<string, List<WorkItemTypeColor>>>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<KeyValuePair<string, List<WorkItemTypeColorAndIcon>>>> GetWorkItemTypeColorAndIconsAsync(
      string[] projectNames,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f0f8dc62-3975-48ce-8051-f636b68b52e3");
      HttpContent httpContent = (HttpContent) new ObjectContent<string[]>(projectNames, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<KeyValuePair<string, List<WorkItemTypeColorAndIcon>>>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<WorkItemType> GetWorkItemTypeAsync(
      string project,
      string type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemType>(new HttpMethod("GET"), new Guid("7c8d7a76-4a09-43e8-b5df-bd792f4ac6aa"), (object) new
      {
        project = project,
        type = type
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemType> GetWorkItemTypeAsync(
      Guid project,
      string type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemType>(new HttpMethod("GET"), new Guid("7c8d7a76-4a09-43e8-b5df-bd792f4ac6aa"), (object) new
      {
        project = project,
        type = type
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemType>> GetWorkItemTypesAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemType>>(new HttpMethod("GET"), new Guid("7c8d7a76-4a09-43e8-b5df-bd792f4ac6aa"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemType>> GetWorkItemTypesAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemType>>(new HttpMethod("GET"), new Guid("7c8d7a76-4a09-43e8-b5df-bd792f4ac6aa"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemTypeFieldWithReferences>> GetWorkItemTypeFieldsWithReferencesAsync(
      string project,
      string type,
      WorkItemTypeFieldsExpandLevel? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bd293ce5-3d25-4192-8e67-e8092e879efb");
      object routeValues = (object) new
      {
        project = project,
        type = type
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemTypeFieldWithReferences>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemTypeFieldWithReferences>> GetWorkItemTypeFieldsWithReferencesAsync(
      Guid project,
      string type,
      WorkItemTypeFieldsExpandLevel? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bd293ce5-3d25-4192-8e67-e8092e879efb");
      object routeValues = (object) new
      {
        project = project,
        type = type
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemTypeFieldWithReferences>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemTypeFieldWithReferences> GetWorkItemTypeFieldWithReferencesAsync(
      string project,
      string type,
      string field,
      WorkItemTypeFieldsExpandLevel? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bd293ce5-3d25-4192-8e67-e8092e879efb");
      object routeValues = (object) new
      {
        project = project,
        type = type,
        field = field
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItemTypeFieldWithReferences>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WorkItemTypeFieldWithReferences> GetWorkItemTypeFieldWithReferencesAsync(
      Guid project,
      string type,
      string field,
      WorkItemTypeFieldsExpandLevel? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bd293ce5-3d25-4192-8e67-e8092e879efb");
      object routeValues = (object) new
      {
        project = project,
        type = type,
        field = field
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItemTypeFieldWithReferences>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemStateColor>> GetWorkItemTypeStatesAsync(
      string project,
      string type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemStateColor>>(new HttpMethod("GET"), new Guid("7c9d7a76-4a09-43e8-b5df-bd792f4ac6aa"), (object) new
      {
        project = project,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WorkItemStateColor>> GetWorkItemTypeStatesAsync(
      Guid project,
      string type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemStateColor>>(new HttpMethod("GET"), new Guid("7c9d7a76-4a09-43e8-b5df-bd792f4ac6aa"), (object) new
      {
        project = project,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemTypeTemplate> ExportWorkItemTypeDefinitionAsync(
      string project,
      string type = null,
      bool? exportGlobalLists = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8637ac8b-5eb6-4f90-b3f7-4f2ff576a459");
      object routeValues = (object) new
      {
        project = project,
        type = type
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (exportGlobalLists.HasValue)
        keyValuePairList.Add(nameof (exportGlobalLists), exportGlobalLists.Value.ToString());
      return this.SendAsync<WorkItemTypeTemplate>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemTypeTemplate> ExportWorkItemTypeDefinitionAsync(
      Guid project,
      string type = null,
      bool? exportGlobalLists = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8637ac8b-5eb6-4f90-b3f7-4f2ff576a459");
      object routeValues = (object) new
      {
        project = project,
        type = type
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (exportGlobalLists.HasValue)
        keyValuePairList.Add(nameof (exportGlobalLists), exportGlobalLists.Value.ToString());
      return this.SendAsync<WorkItemTypeTemplate>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WorkItemTypeTemplate> ExportWorkItemTypeDefinitionAsync(
      string type = null,
      bool? exportGlobalLists = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8637ac8b-5eb6-4f90-b3f7-4f2ff576a459");
      object routeValues = (object) new{ type = type };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (exportGlobalLists.HasValue)
        keyValuePairList.Add(nameof (exportGlobalLists), exportGlobalLists.Value.ToString());
      return this.SendAsync<WorkItemTypeTemplate>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ProvisioningResult> UpdateWorkItemTypeDefinitionAsync(
      WorkItemTypeTemplateUpdateModel updateModel,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8637ac8b-5eb6-4f90-b3f7-4f2ff576a459");
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTypeTemplateUpdateModel>(updateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProvisioningResult>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ProvisioningResult> UpdateWorkItemTypeDefinitionAsync(
      WorkItemTypeTemplateUpdateModel updateModel,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8637ac8b-5eb6-4f90-b3f7-4f2ff576a459");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTypeTemplateUpdateModel>(updateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProvisioningResult>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ProvisioningResult> UpdateWorkItemTypeDefinitionAsync(
      WorkItemTypeTemplateUpdateModel updateModel,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8637ac8b-5eb6-4f90-b3f7-4f2ff576a459");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTypeTemplateUpdateModel>(updateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProvisioningResult>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
