// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.WebApi.WikiHttpClient
// Assembly: Microsoft.TeamFoundation.Wiki.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A8C8A50-70A8-447A-B2AD-300BEAACF074
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.WebApi.dll

using Microsoft.Azure.DevOps.Comments.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
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

namespace Microsoft.TeamFoundation.Wiki.WebApi
{
  [ResourceArea("BF7D82A0-8AA5-4613-94EF-6172A5EA01F3")]
  public class WikiHttpClient : WikiCompatHttpClientBase
  {
    public WikiHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public WikiHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public WikiHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public WikiHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public WikiHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<WikiAttachmentResponse> CreateAttachmentAsync(
      Stream uploadStream,
      string project,
      string wikiIdentifier,
      string name,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("c4382d8d-fefc-40e0-92c5-49852e9e17c0");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (name), name);
      if (versionDescriptor != null)
        wikiHttpClient1.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiAttachmentResponse attachmentAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiAttachmentResponse returnObject = new WikiAttachmentResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiAttachmentResponse attachmentResponse = returnObject;
          attachmentResponse.Attachment = await wikiHttpClient1.ReadContentAsAsync<WikiAttachment>(response, cancellationToken).ConfigureAwait(false);
          attachmentResponse = (WikiAttachmentResponse) null;
        }
        attachmentAsync = returnObject;
      }
      return attachmentAsync;
    }

    public async Task<WikiAttachmentResponse> CreateAttachmentAsync(
      Stream uploadStream,
      string project,
      Guid wikiIdentifier,
      string name,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("c4382d8d-fefc-40e0-92c5-49852e9e17c0");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (name), name);
      if (versionDescriptor != null)
        wikiHttpClient1.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiAttachmentResponse attachmentAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiAttachmentResponse returnObject = new WikiAttachmentResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiAttachmentResponse attachmentResponse = returnObject;
          attachmentResponse.Attachment = await wikiHttpClient1.ReadContentAsAsync<WikiAttachment>(response, cancellationToken).ConfigureAwait(false);
          attachmentResponse = (WikiAttachmentResponse) null;
        }
        attachmentAsync = returnObject;
      }
      return attachmentAsync;
    }

    public async Task<WikiAttachmentResponse> CreateAttachmentAsync(
      Stream uploadStream,
      Guid project,
      string wikiIdentifier,
      string name,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("c4382d8d-fefc-40e0-92c5-49852e9e17c0");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (name), name);
      if (versionDescriptor != null)
        wikiHttpClient1.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiAttachmentResponse attachmentAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiAttachmentResponse returnObject = new WikiAttachmentResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiAttachmentResponse attachmentResponse = returnObject;
          attachmentResponse.Attachment = await wikiHttpClient1.ReadContentAsAsync<WikiAttachment>(response, cancellationToken).ConfigureAwait(false);
          attachmentResponse = (WikiAttachmentResponse) null;
        }
        attachmentAsync = returnObject;
      }
      return attachmentAsync;
    }

    public async Task<WikiAttachmentResponse> CreateAttachmentAsync(
      Stream uploadStream,
      Guid project,
      Guid wikiIdentifier,
      string name,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("c4382d8d-fefc-40e0-92c5-49852e9e17c0");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (name), name);
      if (versionDescriptor != null)
        wikiHttpClient1.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiAttachmentResponse attachmentAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiAttachmentResponse returnObject = new WikiAttachmentResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiAttachmentResponse attachmentResponse = returnObject;
          attachmentResponse.Attachment = await wikiHttpClient1.ReadContentAsAsync<WikiAttachment>(response, cancellationToken).ConfigureAwait(false);
          attachmentResponse = (WikiAttachmentResponse) null;
        }
        attachmentAsync = returnObject;
      }
      return attachmentAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentAttachment> CreateCommentAttachmentAsync(
      Stream uploadStream,
      string project,
      string wikiIdentifier,
      int pageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5100d976-363d-42e7-a19d-4171ecb44782");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CommentAttachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentAttachment> CreateCommentAttachmentAsync(
      Stream uploadStream,
      string project,
      Guid wikiIdentifier,
      int pageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5100d976-363d-42e7-a19d-4171ecb44782");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CommentAttachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentAttachment> CreateCommentAttachmentAsync(
      Stream uploadStream,
      Guid project,
      string wikiIdentifier,
      int pageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5100d976-363d-42e7-a19d-4171ecb44782");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CommentAttachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentAttachment> CreateCommentAttachmentAsync(
      Stream uploadStream,
      Guid project,
      Guid wikiIdentifier,
      int pageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5100d976-363d-42e7-a19d-4171ecb44782");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CommentAttachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<Stream> GetAttachmentContentAsync(
      string project,
      string wikiIdentifier,
      int pageId,
      Guid attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5100d976-363d-42e7-a19d-4171ecb44782");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<Stream> GetAttachmentContentAsync(
      string project,
      Guid wikiIdentifier,
      int pageId,
      Guid attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5100d976-363d-42e7-a19d-4171ecb44782");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<Stream> GetAttachmentContentAsync(
      Guid project,
      string wikiIdentifier,
      int pageId,
      Guid attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5100d976-363d-42e7-a19d-4171ecb44782");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<Stream> GetAttachmentContentAsync(
      Guid project,
      Guid wikiIdentifier,
      int pageId,
      Guid attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5100d976-363d-42e7-a19d-4171ecb44782");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentReaction> AddCommentReactionAsync(
      string project,
      string wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentReaction>(new HttpMethod("PUT"), new Guid("7a5bc693-aab7-4d48-8f34-36f373022063"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        commentId = commentId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentReaction> AddCommentReactionAsync(
      string project,
      Guid wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentReaction>(new HttpMethod("PUT"), new Guid("7a5bc693-aab7-4d48-8f34-36f373022063"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        commentId = commentId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentReaction> AddCommentReactionAsync(
      Guid project,
      string wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentReaction>(new HttpMethod("PUT"), new Guid("7a5bc693-aab7-4d48-8f34-36f373022063"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        commentId = commentId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentReaction> AddCommentReactionAsync(
      Guid project,
      Guid wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentReaction>(new HttpMethod("PUT"), new Guid("7a5bc693-aab7-4d48-8f34-36f373022063"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        commentId = commentId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentReaction> DeleteCommentReactionAsync(
      string project,
      string wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentReaction>(new HttpMethod("DELETE"), new Guid("7a5bc693-aab7-4d48-8f34-36f373022063"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        commentId = commentId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentReaction> DeleteCommentReactionAsync(
      string project,
      Guid wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentReaction>(new HttpMethod("DELETE"), new Guid("7a5bc693-aab7-4d48-8f34-36f373022063"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        commentId = commentId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentReaction> DeleteCommentReactionAsync(
      Guid project,
      string wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentReaction>(new HttpMethod("DELETE"), new Guid("7a5bc693-aab7-4d48-8f34-36f373022063"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        commentId = commentId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentReaction> DeleteCommentReactionAsync(
      Guid project,
      Guid wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CommentReaction>(new HttpMethod("DELETE"), new Guid("7a5bc693-aab7-4d48-8f34-36f373022063"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        commentId = commentId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<IdentityRef>> GetEngagedUsersAsync(
      string project,
      string wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("598a5268-41a7-4162-b7dc-344131e4d1fa");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        commentId = commentId,
        type = type
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<IdentityRef>> GetEngagedUsersAsync(
      string project,
      Guid wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("598a5268-41a7-4162-b7dc-344131e4d1fa");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        commentId = commentId,
        type = type
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<IdentityRef>> GetEngagedUsersAsync(
      Guid project,
      string wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("598a5268-41a7-4162-b7dc-344131e4d1fa");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        commentId = commentId,
        type = type
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<IdentityRef>> GetEngagedUsersAsync(
      Guid project,
      Guid wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("598a5268-41a7-4162-b7dc-344131e4d1fa");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        commentId = commentId,
        type = type
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Microsoft.Azure.DevOps.Comments.WebApi.Comment> AddCommentAsync(
      CommentCreateParameters request,
      string project,
      string wikiIdentifier,
      int pageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentCreateParameters>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.Azure.DevOps.Comments.WebApi.Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Microsoft.Azure.DevOps.Comments.WebApi.Comment> AddCommentAsync(
      CommentCreateParameters request,
      string project,
      Guid wikiIdentifier,
      int pageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentCreateParameters>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.Azure.DevOps.Comments.WebApi.Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Microsoft.Azure.DevOps.Comments.WebApi.Comment> AddCommentAsync(
      CommentCreateParameters request,
      Guid project,
      string wikiIdentifier,
      int pageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentCreateParameters>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.Azure.DevOps.Comments.WebApi.Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Microsoft.Azure.DevOps.Comments.WebApi.Comment> AddCommentAsync(
      CommentCreateParameters request,
      Guid project,
      Guid wikiIdentifier,
      int pageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentCreateParameters>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.Azure.DevOps.Comments.WebApi.Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task DeleteCommentAsync(
      string project,
      string wikiIdentifier,
      int pageId,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task DeleteCommentAsync(
      string project,
      Guid wikiIdentifier,
      int pageId,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task DeleteCommentAsync(
      Guid project,
      string wikiIdentifier,
      int pageId,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task DeleteCommentAsync(
      Guid project,
      Guid wikiIdentifier,
      int pageId,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Microsoft.Azure.DevOps.Comments.WebApi.Comment> GetCommentAsync(
      string project,
      string wikiIdentifier,
      int pageId,
      int id,
      bool? excludeDeleted = null,
      CommentExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (excludeDeleted.HasValue)
        keyValuePairList.Add(nameof (excludeDeleted), excludeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<Microsoft.Azure.DevOps.Comments.WebApi.Comment>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Microsoft.Azure.DevOps.Comments.WebApi.Comment> GetCommentAsync(
      string project,
      Guid wikiIdentifier,
      int pageId,
      int id,
      bool? excludeDeleted = null,
      CommentExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (excludeDeleted.HasValue)
        keyValuePairList.Add(nameof (excludeDeleted), excludeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<Microsoft.Azure.DevOps.Comments.WebApi.Comment>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Microsoft.Azure.DevOps.Comments.WebApi.Comment> GetCommentAsync(
      Guid project,
      string wikiIdentifier,
      int pageId,
      int id,
      bool? excludeDeleted = null,
      CommentExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (excludeDeleted.HasValue)
        keyValuePairList.Add(nameof (excludeDeleted), excludeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<Microsoft.Azure.DevOps.Comments.WebApi.Comment>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Microsoft.Azure.DevOps.Comments.WebApi.Comment> GetCommentAsync(
      Guid project,
      Guid wikiIdentifier,
      int pageId,
      int id,
      bool? excludeDeleted = null,
      CommentExpandOptions? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (excludeDeleted.HasValue)
        keyValuePairList.Add(nameof (excludeDeleted), excludeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<Microsoft.Azure.DevOps.Comments.WebApi.Comment>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentList> ListCommentsAsync(
      string project,
      string wikiIdentifier,
      int pageId,
      int? top = null,
      string continuationToken = null,
      bool? excludeDeleted = null,
      CommentExpandOptions? expand = null,
      CommentSortOrder? order = null,
      int? parentId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
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
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (excludeDeleted.HasValue)
        keyValuePairList.Add(nameof (excludeDeleted), excludeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (order.HasValue)
        keyValuePairList.Add(nameof (order), order.Value.ToString());
      if (parentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = parentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (parentId), str);
      }
      return this.SendAsync<CommentList>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentList> ListCommentsAsync(
      string project,
      Guid wikiIdentifier,
      int pageId,
      int? top = null,
      string continuationToken = null,
      bool? excludeDeleted = null,
      CommentExpandOptions? expand = null,
      CommentSortOrder? order = null,
      int? parentId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
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
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (excludeDeleted.HasValue)
        keyValuePairList.Add(nameof (excludeDeleted), excludeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (order.HasValue)
        keyValuePairList.Add(nameof (order), order.Value.ToString());
      if (parentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = parentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (parentId), str);
      }
      return this.SendAsync<CommentList>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentList> ListCommentsAsync(
      Guid project,
      string wikiIdentifier,
      int pageId,
      int? top = null,
      string continuationToken = null,
      bool? excludeDeleted = null,
      CommentExpandOptions? expand = null,
      CommentSortOrder? order = null,
      int? parentId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
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
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (excludeDeleted.HasValue)
        keyValuePairList.Add(nameof (excludeDeleted), excludeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (order.HasValue)
        keyValuePairList.Add(nameof (order), order.Value.ToString());
      if (parentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = parentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (parentId), str);
      }
      return this.SendAsync<CommentList>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<CommentList> ListCommentsAsync(
      Guid project,
      Guid wikiIdentifier,
      int pageId,
      int? top = null,
      string continuationToken = null,
      bool? excludeDeleted = null,
      CommentExpandOptions? expand = null,
      CommentSortOrder? order = null,
      int? parentId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
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
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (excludeDeleted.HasValue)
        keyValuePairList.Add(nameof (excludeDeleted), excludeDeleted.Value.ToString());
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (order.HasValue)
        keyValuePairList.Add(nameof (order), order.Value.ToString());
      if (parentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = parentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (parentId), str);
      }
      return this.SendAsync<CommentList>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Microsoft.Azure.DevOps.Comments.WebApi.Comment> UpdateCommentAsync(
      CommentUpdateParameters comment,
      string project,
      string wikiIdentifier,
      int pageId,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentUpdateParameters>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.Azure.DevOps.Comments.WebApi.Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Microsoft.Azure.DevOps.Comments.WebApi.Comment> UpdateCommentAsync(
      CommentUpdateParameters comment,
      string project,
      Guid wikiIdentifier,
      int pageId,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentUpdateParameters>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.Azure.DevOps.Comments.WebApi.Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Microsoft.Azure.DevOps.Comments.WebApi.Comment> UpdateCommentAsync(
      CommentUpdateParameters comment,
      Guid project,
      string wikiIdentifier,
      int pageId,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentUpdateParameters>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.Azure.DevOps.Comments.WebApi.Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Microsoft.Azure.DevOps.Comments.WebApi.Comment> UpdateCommentAsync(
      CommentUpdateParameters comment,
      Guid project,
      Guid wikiIdentifier,
      int pageId,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9b394e93-7db5-46cb-9c26-09a36aa5c895");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentUpdateParameters>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.Azure.DevOps.Comments.WebApi.Comment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task<WikiPageMoveResponse> CreatePageMoveAsync(
      WikiPageMoveParameters pageMoveParameters,
      string project,
      string wikiIdentifier,
      string comment = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e37bbe71-cbae-49e5-9a4e-949143b9d910");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPageMoveParameters>(pageMoveParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (versionDescriptor != null)
        wikiHttpClient1.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiPageMoveResponse pageMoveAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiPageMoveResponse returnObject = new WikiPageMoveResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiPageMoveResponse pageMoveResponse = returnObject;
          pageMoveResponse.PageMove = await wikiHttpClient1.ReadContentAsAsync<WikiPageMove>(response, cancellationToken).ConfigureAwait(false);
          pageMoveResponse = (WikiPageMoveResponse) null;
        }
        pageMoveAsync = returnObject;
      }
      return pageMoveAsync;
    }

    public async Task<WikiPageMoveResponse> CreatePageMoveAsync(
      WikiPageMoveParameters pageMoveParameters,
      string project,
      Guid wikiIdentifier,
      string comment = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e37bbe71-cbae-49e5-9a4e-949143b9d910");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPageMoveParameters>(pageMoveParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (versionDescriptor != null)
        wikiHttpClient1.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiPageMoveResponse pageMoveAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiPageMoveResponse returnObject = new WikiPageMoveResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiPageMoveResponse pageMoveResponse = returnObject;
          pageMoveResponse.PageMove = await wikiHttpClient1.ReadContentAsAsync<WikiPageMove>(response, cancellationToken).ConfigureAwait(false);
          pageMoveResponse = (WikiPageMoveResponse) null;
        }
        pageMoveAsync = returnObject;
      }
      return pageMoveAsync;
    }

    public async Task<WikiPageMoveResponse> CreatePageMoveAsync(
      WikiPageMoveParameters pageMoveParameters,
      Guid project,
      string wikiIdentifier,
      string comment = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e37bbe71-cbae-49e5-9a4e-949143b9d910");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPageMoveParameters>(pageMoveParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (versionDescriptor != null)
        wikiHttpClient1.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiPageMoveResponse pageMoveAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiPageMoveResponse returnObject = new WikiPageMoveResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiPageMoveResponse pageMoveResponse = returnObject;
          pageMoveResponse.PageMove = await wikiHttpClient1.ReadContentAsAsync<WikiPageMove>(response, cancellationToken).ConfigureAwait(false);
          pageMoveResponse = (WikiPageMoveResponse) null;
        }
        pageMoveAsync = returnObject;
      }
      return pageMoveAsync;
    }

    public async Task<WikiPageMoveResponse> CreatePageMoveAsync(
      WikiPageMoveParameters pageMoveParameters,
      Guid project,
      Guid wikiIdentifier,
      string comment = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e37bbe71-cbae-49e5-9a4e-949143b9d910");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPageMoveParameters>(pageMoveParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (versionDescriptor != null)
        wikiHttpClient1.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiPageMoveResponse pageMoveAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiPageMoveResponse returnObject = new WikiPageMoveResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiPageMoveResponse pageMoveResponse = returnObject;
          pageMoveResponse.PageMove = await wikiHttpClient1.ReadContentAsAsync<WikiPageMove>(response, cancellationToken).ConfigureAwait(false);
          pageMoveResponse = (WikiPageMoveResponse) null;
        }
        pageMoveAsync = returnObject;
      }
      return pageMoveAsync;
    }

    public async Task<WikiPageResponse> CreateOrUpdatePageAsync(
      WikiPageCreateOrUpdateParameters parameters,
      string project,
      string wikiIdentifier,
      string path,
      string Version,
      string comment = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPageCreateOrUpdateParameters>(parameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (versionDescriptor != null)
        wikiHttpClient1.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(Version))
        collection.Add("If-Match", Version);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiPageResponse orUpdatePageAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient1.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        orUpdatePageAsync = returnObject;
      }
      return orUpdatePageAsync;
    }

    public async Task<WikiPageResponse> CreateOrUpdatePageAsync(
      WikiPageCreateOrUpdateParameters parameters,
      string project,
      Guid wikiIdentifier,
      string path,
      string Version,
      string comment = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPageCreateOrUpdateParameters>(parameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (versionDescriptor != null)
        wikiHttpClient1.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(Version))
        collection.Add("If-Match", Version);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiPageResponse orUpdatePageAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient1.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        orUpdatePageAsync = returnObject;
      }
      return orUpdatePageAsync;
    }

    public async Task<WikiPageResponse> CreateOrUpdatePageAsync(
      WikiPageCreateOrUpdateParameters parameters,
      Guid project,
      string wikiIdentifier,
      string path,
      string Version,
      string comment = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPageCreateOrUpdateParameters>(parameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (versionDescriptor != null)
        wikiHttpClient1.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(Version))
        collection.Add("If-Match", Version);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiPageResponse orUpdatePageAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient1.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        orUpdatePageAsync = returnObject;
      }
      return orUpdatePageAsync;
    }

    public async Task<WikiPageResponse> CreateOrUpdatePageAsync(
      WikiPageCreateOrUpdateParameters parameters,
      Guid project,
      Guid wikiIdentifier,
      string path,
      string Version,
      string comment = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPageCreateOrUpdateParameters>(parameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (versionDescriptor != null)
        wikiHttpClient1.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(Version))
        collection.Add("If-Match", Version);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiPageResponse orUpdatePageAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient1.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        orUpdatePageAsync = returnObject;
      }
      return orUpdatePageAsync;
    }

    public async Task<WikiPageResponse> DeletePageAsync(
      string project,
      string wikiIdentifier,
      string path,
      string comment = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      WikiPageResponse wikiPageResponse1;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        wikiPageResponse1 = returnObject;
      }
      return wikiPageResponse1;
    }

    public async Task<WikiPageResponse> DeletePageAsync(
      string project,
      Guid wikiIdentifier,
      string path,
      string comment = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      WikiPageResponse wikiPageResponse1;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        wikiPageResponse1 = returnObject;
      }
      return wikiPageResponse1;
    }

    public async Task<WikiPageResponse> DeletePageAsync(
      Guid project,
      string wikiIdentifier,
      string path,
      string comment = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      WikiPageResponse wikiPageResponse1;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        wikiPageResponse1 = returnObject;
      }
      return wikiPageResponse1;
    }

    public async Task<WikiPageResponse> DeletePageAsync(
      Guid project,
      Guid wikiIdentifier,
      string path,
      string comment = null,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      WikiPageResponse wikiPageResponse1;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        wikiPageResponse1 = returnObject;
      }
      return wikiPageResponse1;
    }

    public async Task<WikiPageResponse> GetPageAsync(
      string project,
      string wikiIdentifier,
      string path = null,
      VersionControlRecursionType? recursionLevel = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      WikiPageResponse pageAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        pageAsync = returnObject;
      }
      return pageAsync;
    }

    public async Task<WikiPageResponse> GetPageAsync(
      string project,
      Guid wikiIdentifier,
      string path = null,
      VersionControlRecursionType? recursionLevel = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      WikiPageResponse pageAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        pageAsync = returnObject;
      }
      return pageAsync;
    }

    public async Task<WikiPageResponse> GetPageAsync(
      Guid project,
      string wikiIdentifier,
      string path = null,
      VersionControlRecursionType? recursionLevel = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      WikiPageResponse pageAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        pageAsync = returnObject;
      }
      return pageAsync;
    }

    public async Task<WikiPageResponse> GetPageAsync(
      Guid project,
      Guid wikiIdentifier,
      string path = null,
      VersionControlRecursionType? recursionLevel = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      WikiPageResponse pageAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        pageAsync = returnObject;
      }
      return pageAsync;
    }

    public async Task<Stream> GetPageTextAsync(
      string project,
      string wikiIdentifier,
      string path = null,
      VersionControlRecursionType? recursionLevel = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageTextAsync(
      string project,
      Guid wikiIdentifier,
      string path = null,
      VersionControlRecursionType? recursionLevel = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageTextAsync(
      Guid project,
      string wikiIdentifier,
      string path = null,
      VersionControlRecursionType? recursionLevel = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageTextAsync(
      Guid project,
      Guid wikiIdentifier,
      string path = null,
      VersionControlRecursionType? recursionLevel = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageZipAsync(
      string project,
      string wikiIdentifier,
      string path = null,
      VersionControlRecursionType? recursionLevel = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageZipAsync(
      string project,
      Guid wikiIdentifier,
      string path = null,
      VersionControlRecursionType? recursionLevel = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageZipAsync(
      Guid project,
      string wikiIdentifier,
      string path = null,
      VersionControlRecursionType? recursionLevel = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageZipAsync(
      Guid project,
      Guid wikiIdentifier,
      string path = null,
      VersionControlRecursionType? recursionLevel = null,
      GitVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("25d3fbc7-fe3d-46cb-b5a5-0b6f79caf27b");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        wikiHttpClient.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<WikiPageResponse> DeletePageByIdAsync(
      string project,
      string wikiIdentifier,
      int id,
      string comment = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      WikiPageResponse wikiPageResponse1;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        wikiPageResponse1 = returnObject;
      }
      return wikiPageResponse1;
    }

    public async Task<WikiPageResponse> DeletePageByIdAsync(
      string project,
      Guid wikiIdentifier,
      int id,
      string comment = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      WikiPageResponse wikiPageResponse1;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        wikiPageResponse1 = returnObject;
      }
      return wikiPageResponse1;
    }

    public async Task<WikiPageResponse> DeletePageByIdAsync(
      Guid project,
      string wikiIdentifier,
      int id,
      string comment = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      WikiPageResponse wikiPageResponse1;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        wikiPageResponse1 = returnObject;
      }
      return wikiPageResponse1;
    }

    public async Task<WikiPageResponse> DeletePageByIdAsync(
      Guid project,
      Guid wikiIdentifier,
      int id,
      string comment = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      WikiPageResponse wikiPageResponse1;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        wikiPageResponse1 = returnObject;
      }
      return wikiPageResponse1;
    }

    public async Task<WikiPageResponse> GetPageByIdAsync(
      string project,
      string wikiIdentifier,
      int id,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      WikiPageResponse pageByIdAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        pageByIdAsync = returnObject;
      }
      return pageByIdAsync;
    }

    public async Task<WikiPageResponse> GetPageByIdAsync(
      string project,
      Guid wikiIdentifier,
      int id,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      WikiPageResponse pageByIdAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        pageByIdAsync = returnObject;
      }
      return pageByIdAsync;
    }

    public async Task<WikiPageResponse> GetPageByIdAsync(
      Guid project,
      string wikiIdentifier,
      int id,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      WikiPageResponse pageByIdAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        pageByIdAsync = returnObject;
      }
      return pageByIdAsync;
    }

    public async Task<WikiPageResponse> GetPageByIdAsync(
      Guid project,
      Guid wikiIdentifier,
      int id,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      WikiPageResponse pageByIdAsync;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        pageByIdAsync = returnObject;
      }
      return pageByIdAsync;
    }

    public async Task<Stream> GetPageByIdTextAsync(
      string project,
      string wikiIdentifier,
      int id,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageByIdTextAsync(
      string project,
      Guid wikiIdentifier,
      int id,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageByIdTextAsync(
      Guid project,
      string wikiIdentifier,
      int id,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageByIdTextAsync(
      Guid project,
      Guid wikiIdentifier,
      int id,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageByIdZipAsync(
      string project,
      string wikiIdentifier,
      int id,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageByIdZipAsync(
      string project,
      Guid wikiIdentifier,
      int id,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageByIdZipAsync(
      Guid project,
      string wikiIdentifier,
      int id,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetPageByIdZipAsync(
      Guid project,
      Guid wikiIdentifier,
      int id,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await wikiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await wikiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<WikiPageResponse> UpdatePageByIdAsync(
      WikiPageCreateOrUpdateParameters parameters,
      string project,
      string wikiIdentifier,
      int id,
      string Version,
      string comment = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPageCreateOrUpdateParameters>(parameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      if (comment != null)
        collection1.Add(nameof (comment), comment);
      List<KeyValuePair<string, string>> collection2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(Version))
        collection2.Add("If-Match", Version);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection2;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiPageResponse wikiPageResponse1;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient1.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        wikiPageResponse1 = returnObject;
      }
      return wikiPageResponse1;
    }

    public async Task<WikiPageResponse> UpdatePageByIdAsync(
      WikiPageCreateOrUpdateParameters parameters,
      string project,
      Guid wikiIdentifier,
      int id,
      string Version,
      string comment = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPageCreateOrUpdateParameters>(parameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      if (comment != null)
        collection1.Add(nameof (comment), comment);
      List<KeyValuePair<string, string>> collection2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(Version))
        collection2.Add("If-Match", Version);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection2;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiPageResponse wikiPageResponse1;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient1.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        wikiPageResponse1 = returnObject;
      }
      return wikiPageResponse1;
    }

    public async Task<WikiPageResponse> UpdatePageByIdAsync(
      WikiPageCreateOrUpdateParameters parameters,
      Guid project,
      string wikiIdentifier,
      int id,
      string Version,
      string comment = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPageCreateOrUpdateParameters>(parameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      if (comment != null)
        collection1.Add(nameof (comment), comment);
      List<KeyValuePair<string, string>> collection2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(Version))
        collection2.Add("If-Match", Version);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection2;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiPageResponse wikiPageResponse1;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient1.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        wikiPageResponse1 = returnObject;
      }
      return wikiPageResponse1;
    }

    public async Task<WikiPageResponse> UpdatePageByIdAsync(
      WikiPageCreateOrUpdateParameters parameters,
      Guid project,
      Guid wikiIdentifier,
      int id,
      string Version,
      string comment = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WikiHttpClient wikiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ceddcf75-1068-452d-8b13-2d4d76e1f970");
      object obj = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPageCreateOrUpdateParameters>(parameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      if (comment != null)
        collection1.Add(nameof (comment), comment);
      List<KeyValuePair<string, string>> collection2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(Version))
        collection2.Add("If-Match", Version);
      WikiHttpClient wikiHttpClient2 = wikiHttpClient1;
      HttpMethod method = httpMethod;
      List<KeyValuePair<string, string>> additionalHeaders = collection2;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      WikiPageResponse wikiPageResponse1;
      using (HttpRequestMessage requestMessage = await wikiHttpClient2.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1).ConfigureAwait(false))
      {
        WikiPageResponse returnObject = new WikiPageResponse();
        using (HttpResponseMessage response = await wikiHttpClient1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = wikiHttpClient1.GetHeaderValue(response, "ETag");
          WikiPageResponse wikiPageResponse = returnObject;
          wikiPageResponse.Page = await wikiHttpClient1.ReadContentAsAsync<WikiPage>(response, cancellationToken).ConfigureAwait(false);
          wikiPageResponse = (WikiPageResponse) null;
        }
        wikiPageResponse1 = returnObject;
      }
      return wikiPageResponse1;
    }

    public Task<PagedList<WikiPageDetail>> GetPagesBatchAsync(
      WikiPagesBatchRequest pagesBatchRequest,
      string project,
      string wikiIdentifier,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("71323c46-2592-4398-8771-ced73dd87207");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPagesBatchRequest>(pagesBatchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) queryParams, nameof (versionDescriptor), (object) versionDescriptor);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) queryParams;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PagedList<WikiPageDetail>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<PagedList<WikiPageDetail>> GetPagesBatchAsync(
      WikiPagesBatchRequest pagesBatchRequest,
      string project,
      Guid wikiIdentifier,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("71323c46-2592-4398-8771-ced73dd87207");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPagesBatchRequest>(pagesBatchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) queryParams, nameof (versionDescriptor), (object) versionDescriptor);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) queryParams;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PagedList<WikiPageDetail>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<PagedList<WikiPageDetail>> GetPagesBatchAsync(
      WikiPagesBatchRequest pagesBatchRequest,
      Guid project,
      string wikiIdentifier,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("71323c46-2592-4398-8771-ced73dd87207");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPagesBatchRequest>(pagesBatchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) queryParams, nameof (versionDescriptor), (object) versionDescriptor);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) queryParams;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PagedList<WikiPageDetail>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<PagedList<WikiPageDetail>> GetPagesBatchAsync(
      WikiPagesBatchRequest pagesBatchRequest,
      Guid project,
      Guid wikiIdentifier,
      GitVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("71323c46-2592-4398-8771-ced73dd87207");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiPagesBatchRequest>(pagesBatchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) queryParams, nameof (versionDescriptor), (object) versionDescriptor);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) queryParams;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PagedList<WikiPageDetail>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<WikiPageDetail> GetPageDataAsync(
      string project,
      string wikiIdentifier,
      int pageId,
      int? pageViewsForDays = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("81c4e0fe-7663-4d62-ad46-6ab78459f274");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (pageViewsForDays.HasValue)
        keyValuePairList.Add(nameof (pageViewsForDays), pageViewsForDays.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<WikiPageDetail>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiPageDetail> GetPageDataAsync(
      string project,
      Guid wikiIdentifier,
      int pageId,
      int? pageViewsForDays = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("81c4e0fe-7663-4d62-ad46-6ab78459f274");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (pageViewsForDays.HasValue)
        keyValuePairList.Add(nameof (pageViewsForDays), pageViewsForDays.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<WikiPageDetail>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiPageDetail> GetPageDataAsync(
      Guid project,
      string wikiIdentifier,
      int pageId,
      int? pageViewsForDays = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("81c4e0fe-7663-4d62-ad46-6ab78459f274");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (pageViewsForDays.HasValue)
        keyValuePairList.Add(nameof (pageViewsForDays), pageViewsForDays.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<WikiPageDetail>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiPageDetail> GetPageDataAsync(
      Guid project,
      Guid wikiIdentifier,
      int pageId,
      int? pageViewsForDays = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("81c4e0fe-7663-4d62-ad46-6ab78459f274");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (pageViewsForDays.HasValue)
        keyValuePairList.Add(nameof (pageViewsForDays), pageViewsForDays.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<WikiPageDetail>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<WikiPageViewStats> CreateOrUpdatePageViewStatsAsync(
      string project,
      string wikiIdentifier,
      GitVersionDescriptor wikiVersion,
      string path,
      string oldPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("1087b746-5d15-41b9-bea6-14e325e7f880");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (wikiVersion), (object) wikiVersion);
      keyValuePairList.Add(nameof (path), path);
      if (oldPath != null)
        keyValuePairList.Add(nameof (oldPath), oldPath);
      return this.SendAsync<WikiPageViewStats>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<WikiPageViewStats> CreateOrUpdatePageViewStatsAsync(
      string project,
      Guid wikiIdentifier,
      GitVersionDescriptor wikiVersion,
      string path,
      string oldPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("1087b746-5d15-41b9-bea6-14e325e7f880");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (wikiVersion), (object) wikiVersion);
      keyValuePairList.Add(nameof (path), path);
      if (oldPath != null)
        keyValuePairList.Add(nameof (oldPath), oldPath);
      return this.SendAsync<WikiPageViewStats>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<WikiPageViewStats> CreateOrUpdatePageViewStatsAsync(
      Guid project,
      string wikiIdentifier,
      GitVersionDescriptor wikiVersion,
      string path,
      string oldPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("1087b746-5d15-41b9-bea6-14e325e7f880");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (wikiVersion), (object) wikiVersion);
      keyValuePairList.Add(nameof (path), path);
      if (oldPath != null)
        keyValuePairList.Add(nameof (oldPath), oldPath);
      return this.SendAsync<WikiPageViewStats>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<WikiPageViewStats> CreateOrUpdatePageViewStatsAsync(
      Guid project,
      Guid wikiIdentifier,
      GitVersionDescriptor wikiVersion,
      string path,
      string oldPath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("1087b746-5d15-41b9-bea6-14e325e7f880");
      object routeValues = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (wikiVersion), (object) wikiVersion);
      keyValuePairList.Add(nameof (path), path);
      if (oldPath != null)
        keyValuePairList.Add(nameof (oldPath), oldPath);
      return this.SendAsync<WikiPageViewStats>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> CreateWikiAsync(
      WikiCreateParametersV2 wikiCreateParams,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728");
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiCreateParametersV2>(wikiCreateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WikiV2>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WikiV2> CreateWikiAsync(
      WikiCreateParametersV2 wikiCreateParams,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiCreateParametersV2>(wikiCreateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WikiV2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WikiV2> CreateWikiAsync(
      WikiCreateParametersV2 wikiCreateParams,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiCreateParametersV2>(wikiCreateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WikiV2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WikiV2> DeleteWikiAsync(
      string wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WikiV2>(new HttpMethod("DELETE"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        wikiIdentifier = wikiIdentifier
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> DeleteWikiAsync(
      Guid wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WikiV2>(new HttpMethod("DELETE"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        wikiIdentifier = wikiIdentifier
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> DeleteWikiAsync(
      string project,
      string wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WikiV2>(new HttpMethod("DELETE"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> DeleteWikiAsync(
      string project,
      Guid wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WikiV2>(new HttpMethod("DELETE"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> DeleteWikiAsync(
      Guid project,
      string wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WikiV2>(new HttpMethod("DELETE"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> DeleteWikiAsync(
      Guid project,
      Guid wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WikiV2>(new HttpMethod("DELETE"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<WikiV2>> GetAllWikisAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WikiV2>>(new HttpMethod("GET"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), version: new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<WikiV2>> GetAllWikisAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WikiV2>>(new HttpMethod("GET"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<WikiV2>> GetAllWikisAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WikiV2>>(new HttpMethod("GET"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> GetWikiAsync(
      string wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WikiV2>(new HttpMethod("GET"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        wikiIdentifier = wikiIdentifier
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> GetWikiAsync(
      Guid wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WikiV2>(new HttpMethod("GET"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        wikiIdentifier = wikiIdentifier
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> GetWikiAsync(
      string project,
      string wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WikiV2>(new HttpMethod("GET"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> GetWikiAsync(
      string project,
      Guid wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WikiV2>(new HttpMethod("GET"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> GetWikiAsync(
      Guid project,
      string wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WikiV2>(new HttpMethod("GET"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> GetWikiAsync(
      Guid project,
      Guid wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WikiV2>(new HttpMethod("GET"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WikiV2> UpdateWikiAsync(
      WikiUpdateParameters updateParameters,
      string wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728");
      object obj1 = (object) new
      {
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiUpdateParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WikiV2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WikiV2> UpdateWikiAsync(
      WikiUpdateParameters updateParameters,
      Guid wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728");
      object obj1 = (object) new
      {
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiUpdateParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WikiV2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WikiV2> UpdateWikiAsync(
      WikiUpdateParameters updateParameters,
      string project,
      string wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiUpdateParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WikiV2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WikiV2> UpdateWikiAsync(
      WikiUpdateParameters updateParameters,
      string project,
      Guid wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiUpdateParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WikiV2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WikiV2> UpdateWikiAsync(
      WikiUpdateParameters updateParameters,
      Guid project,
      string wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiUpdateParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WikiV2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WikiV2> UpdateWikiAsync(
      WikiUpdateParameters updateParameters,
      Guid project,
      Guid wikiIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728");
      object obj1 = (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiUpdateParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WikiV2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
