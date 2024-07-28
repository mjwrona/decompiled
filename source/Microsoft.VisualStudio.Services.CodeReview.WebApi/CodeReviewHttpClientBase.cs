// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.CodeReviewHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [ResourceArea("997A4743-5B0E-424B-AAFA-37B62A3E1DBF")]
  public abstract class CodeReviewHttpClientBase : VssHttpClientBase
  {
    public CodeReviewHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public CodeReviewHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public CodeReviewHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public CodeReviewHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public CodeReviewHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<Attachment> AddAttachmentAsync(
      Attachment attachment,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9d61ac01-ead6-429f-bc4d-1c18882d27c4");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Attachment>(attachment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Attachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Attachment> AddAttachmentAsync(
      Attachment attachment,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9d61ac01-ead6-429f-bc4d-1c18882d27c4");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Attachment>(attachment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Attachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteAttachmentAsync(
      string project,
      int reviewId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9d61ac01-ead6-429f-bc4d-1c18882d27c4"), (object) new
      {
        project = project,
        reviewId = reviewId,
        attachmentId = attachmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteAttachmentAsync(
      Guid project,
      int reviewId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9d61ac01-ead6-429f-bc4d-1c18882d27c4"), (object) new
      {
        project = project,
        reviewId = reviewId,
        attachmentId = attachmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<Attachment> GetAttachmentAsync(
      string project,
      int reviewId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Attachment>(new HttpMethod("GET"), new Guid("9d61ac01-ead6-429f-bc4d-1c18882d27c4"), (object) new
      {
        project = project,
        reviewId = reviewId,
        attachmentId = attachmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Attachment> GetAttachmentAsync(
      Guid project,
      int reviewId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Attachment>(new HttpMethod("GET"), new Guid("9d61ac01-ead6-429f-bc4d-1c18882d27c4"), (object) new
      {
        project = project,
        reviewId = reviewId,
        attachmentId = attachmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Attachment>> GetAttachmentsAsync(
      string project,
      int reviewId,
      DateTime? modifiedSince = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9d61ac01-ead6-429f-bc4d-1c18882d27c4");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (modifiedSince.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (modifiedSince), modifiedSince.Value);
      return this.SendAsync<List<Attachment>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Attachment>> GetAttachmentsAsync(
      Guid project,
      int reviewId,
      DateTime? modifiedSince = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9d61ac01-ead6-429f-bc4d-1c18882d27c4");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (modifiedSince.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (modifiedSince), modifiedSince.Value);
      return this.SendAsync<List<Attachment>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<IterationChanges> GetChangesAsync(
      string project,
      int reviewId,
      int iterationId,
      int? top = null,
      int? skip = null,
      int? compareTo = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a4c0c4d0-b0ed-4a6f-8751-f32c7444580e");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (skip.HasValue)
        keyValuePairList.Add("$skip", skip.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (compareTo.HasValue)
        keyValuePairList.Add("$compareTo", compareTo.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      IterationChanges changesAsync;
      using (HttpRequestMessage requestMessage = await reviewHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        IterationChanges returnObject = new IterationChanges();
        using (HttpResponseMessage response = await reviewHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.NextTop = reviewHttpClientBase.GetHeaderValue(response, "x-CodeReview-NextTop");
          returnObject.NextSkip = reviewHttpClientBase.GetHeaderValue(response, "x-CodeReview-NextSkip");
          IterationChanges iterationChanges = returnObject;
          iterationChanges.ChangeEntries = (IEnumerable<ChangeEntry>) await reviewHttpClientBase.ReadContentAsAsync<List<ChangeEntry>>(response, cancellationToken).ConfigureAwait(false);
          iterationChanges = (IterationChanges) null;
        }
        changesAsync = returnObject;
      }
      return changesAsync;
    }

    public virtual async Task<IterationChanges> GetChangesAsync(
      Guid project,
      int reviewId,
      int iterationId,
      int? top = null,
      int? skip = null,
      int? compareTo = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a4c0c4d0-b0ed-4a6f-8751-f32c7444580e");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (skip.HasValue)
        keyValuePairList.Add("$skip", skip.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (compareTo.HasValue)
        keyValuePairList.Add("$compareTo", compareTo.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      IterationChanges changesAsync;
      using (HttpRequestMessage requestMessage = await reviewHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        IterationChanges returnObject = new IterationChanges();
        using (HttpResponseMessage response = await reviewHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.NextTop = reviewHttpClientBase.GetHeaderValue(response, "x-CodeReview-NextTop");
          returnObject.NextSkip = reviewHttpClientBase.GetHeaderValue(response, "x-CodeReview-NextSkip");
          IterationChanges iterationChanges = returnObject;
          iterationChanges.ChangeEntries = (IEnumerable<ChangeEntry>) await reviewHttpClientBase.ReadContentAsAsync<List<ChangeEntry>>(response, cancellationToken).ConfigureAwait(false);
          iterationChanges = (IterationChanges) null;
        }
        changesAsync = returnObject;
      }
      return changesAsync;
    }

    public virtual async Task<Stream> GetContentAsync(
      string project,
      int reviewId,
      int iterationId,
      int changeId,
      string fileTarget,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a4c0c4d0-b0ed-4a6f-8751-f32c7444580e");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId,
        changeId = changeId,
        fileTarget = fileTarget
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await reviewHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await reviewHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetContentAsync(
      Guid project,
      int reviewId,
      int iterationId,
      int changeId,
      string fileTarget,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a4c0c4d0-b0ed-4a6f-8751-f32c7444580e");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId,
        changeId = changeId,
        fileTarget = fileTarget
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await reviewHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await reviewHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<DiscussionComment> CreateCommentAsync(
      DiscussionComment newComment,
      string project,
      int reviewId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("fac703b5-fb23-4abf-8d90-09de88cd1293");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DiscussionComment>(newComment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DiscussionComment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DiscussionComment> CreateCommentAsync(
      DiscussionComment newComment,
      Guid project,
      int reviewId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("fac703b5-fb23-4abf-8d90-09de88cd1293");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DiscussionComment>(newComment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DiscussionComment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteCommentAsync(
      string project,
      int reviewId,
      int threadId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("fac703b5-fb23-4abf-8d90-09de88cd1293"), (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteCommentAsync(
      Guid project,
      int reviewId,
      int threadId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("fac703b5-fb23-4abf-8d90-09de88cd1293"), (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<DiscussionComment> GetCommentAsync(
      string project,
      int reviewId,
      int threadId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<DiscussionComment>(new HttpMethod("GET"), new Guid("fac703b5-fb23-4abf-8d90-09de88cd1293"), (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DiscussionComment> GetCommentAsync(
      Guid project,
      int reviewId,
      int threadId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<DiscussionComment>(new HttpMethod("GET"), new Guid("fac703b5-fb23-4abf-8d90-09de88cd1293"), (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<DiscussionComment>> GetCommentsAsync(
      string project,
      int reviewId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<DiscussionComment>>(new HttpMethod("GET"), new Guid("fac703b5-fb23-4abf-8d90-09de88cd1293"), (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<DiscussionComment>> GetCommentsAsync(
      Guid project,
      int reviewId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<DiscussionComment>>(new HttpMethod("GET"), new Guid("fac703b5-fb23-4abf-8d90-09de88cd1293"), (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DiscussionComment> UpdateCommentAsync(
      DiscussionComment comment,
      string project,
      int reviewId,
      int threadId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("fac703b5-fb23-4abf-8d90-09de88cd1293");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DiscussionComment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DiscussionComment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DiscussionComment> UpdateCommentAsync(
      DiscussionComment comment,
      Guid project,
      int reviewId,
      int threadId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("fac703b5-fb23-4abf-8d90-09de88cd1293");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DiscussionComment>(comment, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DiscussionComment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task<Stream> DownloadContentAsync(
      string project,
      int reviewId,
      string contentHash,
      string downloadFileName = null,
      ReviewFileType? fileType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("38f9ad45-10bc-4c0a-99ad-beaaa51ca027");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (contentHash), contentHash);
      if (downloadFileName != null)
        keyValuePairList.Add(nameof (downloadFileName), downloadFileName);
      if (fileType.HasValue)
        keyValuePairList.Add(nameof (fileType), fileType.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await reviewHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await reviewHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> DownloadContentAsync(
      Guid project,
      int reviewId,
      string contentHash,
      string downloadFileName = null,
      ReviewFileType? fileType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("38f9ad45-10bc-4c0a-99ad-beaaa51ca027");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (contentHash), contentHash);
      if (downloadFileName != null)
        keyValuePairList.Add(nameof (downloadFileName), downloadFileName);
      if (fileType.HasValue)
        keyValuePairList.Add(nameof (fileType), fileType.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await reviewHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await reviewHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<ReviewFilesZipContent> DownloadContentZipAsync(
      string project,
      int reviewId,
      int iterationId,
      string filterBy = null,
      int? top = null,
      int? skip = null,
      string downloadFileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("38f9ad45-10bc-4c0a-99ad-beaaa51ca027");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (iterationId), iterationId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (filterBy != null)
        keyValuePairList.Add(nameof (filterBy), filterBy);
      if (top.HasValue)
        keyValuePairList.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (skip.HasValue)
        keyValuePairList.Add(nameof (skip), skip.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (downloadFileName != null)
        keyValuePairList.Add(nameof (downloadFileName), downloadFileName);
      HttpResponseMessage response;
      using (HttpRequestMessage requestMessage = await reviewHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        response = await reviewHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      response.EnsureSuccessStatusCode();
      ReviewFilesZipContent returnObject = new ReviewFilesZipContent();
      returnObject.NextTop = reviewHttpClientBase.GetHeaderValue(response, "x-CodeReview-NextTop");
      returnObject.NextSkip = reviewHttpClientBase.GetHeaderValue(response, "x-CodeReview-NextSkip");
      if (response.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        returnObject.ZipStream = (Stream) new GZipStream(await response.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        ReviewFilesZipContent reviewFilesZipContent = returnObject;
        reviewFilesZipContent.ZipStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        reviewFilesZipContent = (ReviewFilesZipContent) null;
      }
      ReviewFilesZipContent reviewFilesZipContent1 = returnObject;
      returnObject = (ReviewFilesZipContent) null;
      return reviewFilesZipContent1;
    }

    public virtual async Task<ReviewFilesZipContent> DownloadContentZipAsync(
      Guid project,
      int reviewId,
      int iterationId,
      string filterBy = null,
      int? top = null,
      int? skip = null,
      string downloadFileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("38f9ad45-10bc-4c0a-99ad-beaaa51ca027");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (iterationId), iterationId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (filterBy != null)
        keyValuePairList.Add(nameof (filterBy), filterBy);
      if (top.HasValue)
        keyValuePairList.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (skip.HasValue)
        keyValuePairList.Add(nameof (skip), skip.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (downloadFileName != null)
        keyValuePairList.Add(nameof (downloadFileName), downloadFileName);
      HttpResponseMessage response;
      using (HttpRequestMessage requestMessage = await reviewHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        response = await reviewHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      response.EnsureSuccessStatusCode();
      ReviewFilesZipContent returnObject = new ReviewFilesZipContent();
      returnObject.NextTop = reviewHttpClientBase.GetHeaderValue(response, "x-CodeReview-NextTop");
      returnObject.NextSkip = reviewHttpClientBase.GetHeaderValue(response, "x-CodeReview-NextSkip");
      if (response.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        returnObject.ZipStream = (Stream) new GZipStream(await response.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        ReviewFilesZipContent reviewFilesZipContent = returnObject;
        reviewFilesZipContent.ZipStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        reviewFilesZipContent = (ReviewFilesZipContent) null;
      }
      ReviewFilesZipContent reviewFilesZipContent1 = returnObject;
      returnObject = (ReviewFilesZipContent) null;
      return reviewFilesZipContent1;
    }

    public virtual async Task UploadContentAsync(
      Stream uploadStream,
      string project,
      int reviewId,
      string contentHash,
      string fileType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("38f9ad45-10bc-4c0a-99ad-beaaa51ca027");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (contentHash), contentHash);
      if (fileType != null)
        collection.Add(nameof (fileType), fileType);
      CodeReviewHttpClientBase reviewHttpClientBase2 = reviewHttpClientBase1;
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
      using (await reviewHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UploadContentAsync(
      Stream uploadStream,
      Guid project,
      int reviewId,
      string contentHash,
      string fileType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("38f9ad45-10bc-4c0a-99ad-beaaa51ca027");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (contentHash), contentHash);
      if (fileType != null)
        collection.Add(nameof (fileType), fileType);
      CodeReviewHttpClientBase reviewHttpClientBase2 = reviewHttpClientBase1;
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
      using (await reviewHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<ReviewFileContentInfo>> UploadContentsBatchAsync(
      Stream uploadStream,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("38f9ad45-10bc-4c0a-99ad-beaaa51ca027");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
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
      return this.SendAsync<List<ReviewFileContentInfo>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<ReviewFileContentInfo>> UploadContentsBatchAsync(
      Stream uploadStream,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("38f9ad45-10bc-4c0a-99ad-beaaa51ca027");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
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
      return this.SendAsync<List<ReviewFileContentInfo>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task<ReviewFilesZipContent> DownloadContentsBatchZipAsync(
      DownloadContentsCriteria downloadContentsCriteria,
      string project,
      int reviewId,
      int? top = null,
      int? skip = null,
      string downloadFileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4fcd8bd9-2b3c-482d-829a-592369f47277");
      object obj = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DownloadContentsCriteria>(downloadContentsCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        collection.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (skip.HasValue)
        collection.Add(nameof (skip), skip.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (downloadFileName != null)
        collection.Add(nameof (downloadFileName), downloadFileName);
      CodeReviewHttpClientBase reviewHttpClientBase2 = reviewHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpResponseMessage response;
      using (HttpRequestMessage requestMessage = await reviewHttpClientBase2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1, mediaType: "application/zip").ConfigureAwait(false))
        response = await reviewHttpClientBase1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      response.EnsureSuccessStatusCode();
      ReviewFilesZipContent returnObject = new ReviewFilesZipContent();
      returnObject.NextTop = reviewHttpClientBase1.GetHeaderValue(response, "x-CodeReview-NextTop");
      returnObject.NextSkip = reviewHttpClientBase1.GetHeaderValue(response, "x-CodeReview-NextSkip");
      if (response.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        returnObject.ZipStream = (Stream) new GZipStream(await response.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        ReviewFilesZipContent reviewFilesZipContent = returnObject;
        reviewFilesZipContent.ZipStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        reviewFilesZipContent = (ReviewFilesZipContent) null;
      }
      ReviewFilesZipContent reviewFilesZipContent1 = returnObject;
      returnObject = (ReviewFilesZipContent) null;
      return reviewFilesZipContent1;
    }

    public virtual async Task<ReviewFilesZipContent> DownloadContentsBatchZipAsync(
      DownloadContentsCriteria downloadContentsCriteria,
      Guid project,
      int reviewId,
      int? top = null,
      int? skip = null,
      string downloadFileName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4fcd8bd9-2b3c-482d-829a-592369f47277");
      object obj = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DownloadContentsCriteria>(downloadContentsCriteria, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (top.HasValue)
        collection.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (skip.HasValue)
        collection.Add(nameof (skip), skip.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (downloadFileName != null)
        collection.Add(nameof (downloadFileName), downloadFileName);
      CodeReviewHttpClientBase reviewHttpClientBase2 = reviewHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("7.2-preview.1");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpResponseMessage response;
      using (HttpRequestMessage requestMessage = await reviewHttpClientBase2.CreateRequestMessageAsync(method, locationId, routeValues, version, content, queryParameters, cancellationToken: cancellationToken1, mediaType: "application/zip").ConfigureAwait(false))
        response = await reviewHttpClientBase1.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      response.EnsureSuccessStatusCode();
      ReviewFilesZipContent returnObject = new ReviewFilesZipContent();
      returnObject.NextTop = reviewHttpClientBase1.GetHeaderValue(response, "x-CodeReview-NextTop");
      returnObject.NextSkip = reviewHttpClientBase1.GetHeaderValue(response, "x-CodeReview-NextSkip");
      if (response.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        returnObject.ZipStream = (Stream) new GZipStream(await response.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        ReviewFilesZipContent reviewFilesZipContent = returnObject;
        reviewFilesZipContent.ZipStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        reviewFilesZipContent = (ReviewFilesZipContent) null;
      }
      ReviewFilesZipContent reviewFilesZipContent1 = returnObject;
      returnObject = (ReviewFilesZipContent) null;
      return reviewFilesZipContent1;
    }

    public virtual Task<Iteration> CreateIterationAsync(
      Iteration iteration,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d2e77b94-a8c8-45e6-a163-7f1b4ae20eb9");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Iteration>(iteration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Iteration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Iteration> CreateIterationAsync(
      Iteration iteration,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d2e77b94-a8c8-45e6-a163-7f1b4ae20eb9");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Iteration>(iteration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Iteration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Iteration> GetIterationAsync(
      string project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Iteration>(new HttpMethod("GET"), new Guid("d2e77b94-a8c8-45e6-a163-7f1b4ae20eb9"), (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Iteration> GetIterationAsync(
      Guid project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Iteration>(new HttpMethod("GET"), new Guid("d2e77b94-a8c8-45e6-a163-7f1b4ae20eb9"), (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Iteration>> GetIterationsAsync(
      string project,
      int reviewId,
      bool? includeUnpublished = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d2e77b94-a8c8-45e6-a163-7f1b4ae20eb9");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeUnpublished.HasValue)
        keyValuePairList.Add(nameof (includeUnpublished), includeUnpublished.Value.ToString());
      return this.SendAsync<List<Iteration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Iteration>> GetIterationsAsync(
      Guid project,
      int reviewId,
      bool? includeUnpublished = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d2e77b94-a8c8-45e6-a163-7f1b4ae20eb9");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeUnpublished.HasValue)
        keyValuePairList.Add(nameof (includeUnpublished), includeUnpublished.Value.ToString());
      return this.SendAsync<List<Iteration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Iteration> UpdateIterationAsync(
      Iteration iteration,
      string project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d2e77b94-a8c8-45e6-a163-7f1b4ae20eb9");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Iteration>(iteration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Iteration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Iteration> UpdateIterationAsync(
      Iteration iteration,
      Guid project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d2e77b94-a8c8-45e6-a163-7f1b4ae20eb9");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Iteration>(iteration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Iteration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRef> CreateLikeAsync(
      IEnumerable<IdentityRef> users,
      string project,
      int reviewId,
      int threadId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ba6f5f68-a41c-44e7-bfa2-b1fadf1e6b91");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<IdentityRef>>(users, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRef>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<IdentityRef> CreateLikeAsync(
      IEnumerable<IdentityRef> users,
      Guid project,
      int reviewId,
      int threadId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ba6f5f68-a41c-44e7-bfa2-b1fadf1e6b91");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId,
        commentId = commentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<IdentityRef>>(users, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IdentityRef>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteLikeAsync(
      string project,
      int reviewId,
      int threadId,
      short commentId,
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("ba6f5f68-a41c-44e7-bfa2-b1fadf1e6b91"), (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId,
        commentId = commentId,
        userId = userId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteLikeAsync(
      Guid project,
      int reviewId,
      int threadId,
      short commentId,
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("ba6f5f68-a41c-44e7-bfa2-b1fadf1e6b91"), (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId,
        commentId = commentId,
        userId = userId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<IdentityRef>> GetLikesAsync(
      string project,
      int reviewId,
      int threadId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRef>>(new HttpMethod("GET"), new Guid("ba6f5f68-a41c-44e7-bfa2-b1fadf1e6b91"), (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<IdentityRef>> GetLikesAsync(
      Guid project,
      int reviewId,
      int threadId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<IdentityRef>>(new HttpMethod("GET"), new Guid("ba6f5f68-a41c-44e7-bfa2-b1fadf1e6b91"), (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PropertiesCollection> CreateOrUpdateIterationPropertiesAsync(
      JsonPatchDocument document,
      string project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1031ea92-06f3-4550-a310-8bb3059b92ff");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
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
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PropertiesCollection> CreateOrUpdateIterationPropertiesAsync(
      JsonPatchDocument document,
      Guid project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1031ea92-06f3-4550-a310-8bb3059b92ff");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
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
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PropertiesCollection> GetIterationPropertiesAsync(
      string project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PropertiesCollection>(new HttpMethod("GET"), new Guid("1031ea92-06f3-4550-a310-8bb3059b92ff"), (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PropertiesCollection> GetIterationPropertiesAsync(
      Guid project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PropertiesCollection>(new HttpMethod("GET"), new Guid("1031ea92-06f3-4550-a310-8bb3059b92ff"), (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PropertiesCollection> CreateOrUpdateReviewPropertiesAsync(
      JsonPatchDocument document,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("7cf0e9a4-ccd5-4d63-9c52-5241a213c3fe");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
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
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PropertiesCollection> CreateOrUpdateReviewPropertiesAsync(
      JsonPatchDocument document,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("7cf0e9a4-ccd5-4d63-9c52-5241a213c3fe");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
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
      return this.SendAsync<PropertiesCollection>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PropertiesCollection> GetReviewPropertiesAsync(
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PropertiesCollection>(new HttpMethod("GET"), new Guid("7cf0e9a4-ccd5-4d63-9c52-5241a213c3fe"), (object) new
      {
        project = project,
        reviewId = reviewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PropertiesCollection> GetReviewPropertiesAsync(
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PropertiesCollection>(new HttpMethod("GET"), new Guid("7cf0e9a4-ccd5-4d63-9c52-5241a213c3fe"), (object) new
      {
        project = project,
        reviewId = reviewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Reviewer>> AddReviewersAsync(
      IEnumerable<Reviewer> reviewers,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9b1869ec-b17f-4efd-8597-8c89362f2063");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<Reviewer>>(reviewers, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<Reviewer>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<Reviewer>> AddReviewersAsync(
      IEnumerable<Reviewer> reviewers,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9b1869ec-b17f-4efd-8597-8c89362f2063");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<Reviewer>>(reviewers, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<Reviewer>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteReviewerAsync(
      string project,
      int reviewId,
      Guid reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9b1869ec-b17f-4efd-8597-8c89362f2063"), (object) new
      {
        project = project,
        reviewId = reviewId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteReviewerAsync(
      Guid project,
      int reviewId,
      Guid reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9b1869ec-b17f-4efd-8597-8c89362f2063"), (object) new
      {
        project = project,
        reviewId = reviewId,
        reviewerId = reviewerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<Reviewer>> GetReviewersAsync(
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Reviewer>>(new HttpMethod("GET"), new Guid("9b1869ec-b17f-4efd-8597-8c89362f2063"), (object) new
      {
        project = project,
        reviewId = reviewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Reviewer>> GetReviewersAsync(
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Reviewer>>(new HttpMethod("GET"), new Guid("9b1869ec-b17f-4efd-8597-8c89362f2063"), (object) new
      {
        project = project,
        reviewId = reviewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Reviewer> ReplaceReviewerAsync(
      Reviewer reviewer,
      string project,
      int reviewId,
      Guid reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("9b1869ec-b17f-4efd-8597-8c89362f2063");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Reviewer>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Reviewer>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Reviewer> ReplaceReviewerAsync(
      Reviewer reviewer,
      Guid project,
      int reviewId,
      Guid reviewerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("9b1869ec-b17f-4efd-8597-8c89362f2063");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        reviewerId = reviewerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Reviewer>(reviewer, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Reviewer>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PagedList<Review>> GetReviewsAsync(
      string project,
      ReviewSearchCriteria searchCriteria,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d17478c8-387d-4359-ba97-1414ae770b76");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<Review>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<Review>> GetReviewsAsync(
      Guid project,
      ReviewSearchCriteria searchCriteria,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d17478c8-387d-4359-ba97-1414ae770b76");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<Review>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<Review>> GetReviewsAsync(
      ReviewSearchCriteria searchCriteria,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d17478c8-387d-4359-ba97-1414ae770b76");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<Review>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Review> CreateReviewAsync(
      Review review,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("eaa8ec98-2b9c-4730-96a3-4845be1558d6");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Review>(review, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Review>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Review> CreateReviewAsync(
      Review review,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("eaa8ec98-2b9c-4730-96a3-4845be1558d6");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Review>(review, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Review>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteReviewAsync(
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("eaa8ec98-2b9c-4730-96a3-4845be1558d6"), (object) new
      {
        project = project,
        reviewId = reviewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteReviewAsync(
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("eaa8ec98-2b9c-4730-96a3-4845be1558d6"), (object) new
      {
        project = project,
        reviewId = reviewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<Review> GetReviewAsync(
      string project,
      int reviewId,
      bool? includeAllProperties = null,
      int? maxChangesCount = null,
      DateTimeOffset? ifModifiedSince = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("eaa8ec98-2b9c-4730-96a3-4845be1558d6");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      if (includeAllProperties.HasValue)
        keyValuePairList1.Add(nameof (includeAllProperties), includeAllProperties.Value.ToString());
      if (maxChangesCount.HasValue)
        keyValuePairList1.Add(nameof (maxChangesCount), maxChangesCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (ifModifiedSince.HasValue)
        this.AddDateTimeToHeaders((IList<KeyValuePair<string, string>>) keyValuePairList2, "If-Modified-Since", ifModifiedSince.Value);
      return this.SendAsync<Review>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Review> GetReviewAsync(
      Guid project,
      int reviewId,
      bool? includeAllProperties = null,
      int? maxChangesCount = null,
      DateTimeOffset? ifModifiedSince = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("eaa8ec98-2b9c-4730-96a3-4845be1558d6");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      if (includeAllProperties.HasValue)
        keyValuePairList1.Add(nameof (includeAllProperties), includeAllProperties.Value.ToString());
      if (maxChangesCount.HasValue)
        keyValuePairList1.Add(nameof (maxChangesCount), maxChangesCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (ifModifiedSince.HasValue)
        this.AddDateTimeToHeaders((IList<KeyValuePair<string, string>>) keyValuePairList2, "If-Modified-Since", ifModifiedSince.Value);
      return this.SendAsync<Review>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Review> ReplaceReviewAsync(
      Review review,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("eaa8ec98-2b9c-4730-96a3-4845be1558d6");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Review>(review, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Review>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Review> ReplaceReviewAsync(
      Review review,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("eaa8ec98-2b9c-4730-96a3-4845be1558d6");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Review>(review, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Review>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Review> UpdateReviewAsync(
      Review review,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("eaa8ec98-2b9c-4730-96a3-4845be1558d6");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Review>(review, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Review>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Review> UpdateReviewAsync(
      Review review,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("eaa8ec98-2b9c-4730-96a3-4845be1558d6");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Review>(review, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Review>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<Review>> GetReviewsBatchAsync(
      string[] sourceArtifactIds,
      string project,
      bool? includeDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("16b3f95b-5ba6-4f64-a2db-1a03de11d3bc");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<string[]>(sourceArtifactIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeDeleted.HasValue)
        collection.Add(nameof (includeDeleted), includeDeleted.Value.ToString());
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
      return this.SendAsync<List<Review>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<Review>> GetReviewsBatchAsync(
      string[] sourceArtifactIds,
      Guid project,
      bool? includeDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("16b3f95b-5ba6-4f64-a2db-1a03de11d3bc");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<string[]>(sourceArtifactIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeDeleted.HasValue)
        collection.Add(nameof (includeDeleted), includeDeleted.Value.ToString());
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
      return this.SendAsync<List<Review>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<ReviewSettings> CreateReviewSettingsAsync(
      ReviewSettings reviewSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6a11b750-d84c-4f84-b96d-23526f716576");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReviewSettings>(reviewSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReviewSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ReviewSettings> CreateReviewSettingsAsync(
      ReviewSettings reviewSettings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6a11b750-d84c-4f84-b96d-23526f716576");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReviewSettings>(reviewSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReviewSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ReviewSettings> GetReviewSettingsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ReviewSettings>(new HttpMethod("GET"), new Guid("6a11b750-d84c-4f84-b96d-23526f716576"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReviewSettings> GetReviewSettingsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ReviewSettings>(new HttpMethod("GET"), new Guid("6a11b750-d84c-4f84-b96d-23526f716576"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReviewSettings> UpdateReviewSettingsAsync(
      ReviewSettings reviewSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("6a11b750-d84c-4f84-b96d-23526f716576");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReviewSettings>(reviewSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReviewSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ReviewSettings> UpdateReviewSettingsAsync(
      ReviewSettings reviewSettings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("6a11b750-d84c-4f84-b96d-23526f716576");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReviewSettings>(reviewSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReviewSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task ShareReviewAsync(
      NotificationContext userMessage,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("eb58030e-c39b-41b1-9e1f-72e23b032fb4");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NotificationContext>(userMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CodeReviewHttpClientBase reviewHttpClientBase2 = reviewHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await reviewHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task ShareReviewAsync(
      NotificationContext userMessage,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CodeReviewHttpClientBase reviewHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("eb58030e-c39b-41b1-9e1f-72e23b032fb4");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NotificationContext>(userMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CodeReviewHttpClientBase reviewHttpClientBase2 = reviewHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await reviewHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<Status> CreateIterationStatusAsync(
      Status status,
      string project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("cb958c49-f702-483a-bb3b-3454570fb72a");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Status>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Status>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Status> CreateIterationStatusAsync(
      Status status,
      Guid project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("cb958c49-f702-483a-bb3b-3454570fb72a");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Status>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Status>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Status> GetIterationStatusAsync(
      string project,
      int reviewId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Status>(new HttpMethod("GET"), new Guid("cb958c49-f702-483a-bb3b-3454570fb72a"), (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Status> GetIterationStatusAsync(
      Guid project,
      int reviewId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Status>(new HttpMethod("GET"), new Guid("cb958c49-f702-483a-bb3b-3454570fb72a"), (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Status>> GetIterationStatusesAsync(
      string project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Status>>(new HttpMethod("GET"), new Guid("cb958c49-f702-483a-bb3b-3454570fb72a"), (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Status>> GetIterationStatusesAsync(
      Guid project,
      int reviewId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Status>>(new HttpMethod("GET"), new Guid("cb958c49-f702-483a-bb3b-3454570fb72a"), (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Status> UpdateIterationStatusAsync(
      Status status,
      string project,
      int reviewId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("cb958c49-f702-483a-bb3b-3454570fb72a");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId,
        statusId = statusId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Status>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Status>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Status> UpdateIterationStatusAsync(
      Status status,
      Guid project,
      int reviewId,
      int iterationId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("cb958c49-f702-483a-bb3b-3454570fb72a");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        iterationId = iterationId,
        statusId = statusId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Status>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Status>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Status> CreateReviewStatusAsync(
      Status status,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("502d7933-25de-42e3-bc82-8478b3796655");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Status>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Status>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Status> CreateReviewStatusAsync(
      Status status,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("502d7933-25de-42e3-bc82-8478b3796655");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Status>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Status>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Status> GetReviewStatusAsync(
      string project,
      int reviewId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Status>(new HttpMethod("GET"), new Guid("502d7933-25de-42e3-bc82-8478b3796655"), (object) new
      {
        project = project,
        reviewId = reviewId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Status> GetReviewStatusAsync(
      Guid project,
      int reviewId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Status>(new HttpMethod("GET"), new Guid("502d7933-25de-42e3-bc82-8478b3796655"), (object) new
      {
        project = project,
        reviewId = reviewId,
        statusId = statusId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Status>> GetReviewStatusesAsync(
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Status>>(new HttpMethod("GET"), new Guid("502d7933-25de-42e3-bc82-8478b3796655"), (object) new
      {
        project = project,
        reviewId = reviewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Status>> GetReviewStatusesAsync(
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Status>>(new HttpMethod("GET"), new Guid("502d7933-25de-42e3-bc82-8478b3796655"), (object) new
      {
        project = project,
        reviewId = reviewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Status> UpdateReviewStatusAsync(
      Status status,
      string project,
      int reviewId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("502d7933-25de-42e3-bc82-8478b3796655");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        statusId = statusId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Status>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Status>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Status> UpdateReviewStatusAsync(
      Status status,
      Guid project,
      int reviewId,
      int statusId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("502d7933-25de-42e3-bc82-8478b3796655");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        statusId = statusId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Status>(status, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Status>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CommentThread> CreateThreadAsync(
      CommentThread newThread,
      string project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1e0bb4ec-0587-42d8-a005-3815555e766a");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentThread>(newThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CommentThread> CreateThreadAsync(
      CommentThread newThread,
      Guid project,
      int reviewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1e0bb4ec-0587-42d8-a005-3815555e766a");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentThread>(newThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CommentThread> GetThreadAsync(
      string project,
      int reviewId,
      int threadId,
      CommentTrackingCriteria trackingCriteria = null,
      DateTimeOffset? ifModifiedSince = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1e0bb4ec-0587-42d8-a005-3815555e766a");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      if (trackingCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList1, nameof (trackingCriteria), (object) trackingCriteria);
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (ifModifiedSince.HasValue)
        this.AddDateTimeToHeaders((IList<KeyValuePair<string, string>>) keyValuePairList2, "If-Modified-Since", ifModifiedSince.Value);
      return this.SendAsync<CommentThread>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CommentThread> GetThreadAsync(
      Guid project,
      int reviewId,
      int threadId,
      CommentTrackingCriteria trackingCriteria = null,
      DateTimeOffset? ifModifiedSince = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1e0bb4ec-0587-42d8-a005-3815555e766a");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId
      };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      if (trackingCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList1, nameof (trackingCriteria), (object) trackingCriteria);
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (ifModifiedSince.HasValue)
        this.AddDateTimeToHeaders((IList<KeyValuePair<string, string>>) keyValuePairList2, "If-Modified-Since", ifModifiedSince.Value);
      return this.SendAsync<CommentThread>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<CommentThread>> GetThreadsAsync(
      string project,
      int reviewId,
      DateTime? modifiedSince = null,
      CommentTrackingCriteria trackingCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1e0bb4ec-0587-42d8-a005-3815555e766a");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (modifiedSince.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (modifiedSince), modifiedSince.Value);
      if (trackingCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (trackingCriteria), (object) trackingCriteria);
      return this.SendAsync<List<CommentThread>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<CommentThread>> GetThreadsAsync(
      Guid project,
      int reviewId,
      DateTime? modifiedSince = null,
      CommentTrackingCriteria trackingCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1e0bb4ec-0587-42d8-a005-3815555e766a");
      object routeValues = (object) new
      {
        project = project,
        reviewId = reviewId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (modifiedSince.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (modifiedSince), modifiedSince.Value);
      if (trackingCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (trackingCriteria), (object) trackingCriteria);
      return this.SendAsync<List<CommentThread>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CommentThread> UpdateThreadAsync(
      CommentThread thread,
      string project,
      int reviewId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1e0bb4ec-0587-42d8-a005-3815555e766a");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentThread>(thread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CommentThread> UpdateThreadAsync(
      CommentThread thread,
      Guid project,
      int reviewId,
      int threadId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1e0bb4ec-0587-42d8-a005-3815555e766a");
      object obj1 = (object) new
      {
        project = project,
        reviewId = reviewId,
        threadId = threadId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CommentThread>(thread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CommentThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
