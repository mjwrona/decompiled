// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.DiscussionHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 443E2621-CB19-4319-96B1-AE621A0F5B5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi
{
  [ResourceArea("6823169A-2419-4015-B2FD-6FD6F026CA00")]
  public abstract class DiscussionHttpClientBase : VssHttpClientBase
  {
    public DiscussionHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DiscussionHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DiscussionHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DiscussionHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DiscussionHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<DiscussionComment> AddCommentAsync(
      DiscussionComment newComment,
      int discussionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("495211bd-b463-4578-86fe-924ea4953693");
      object obj1 = (object) new
      {
        discussionId = discussionId
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DiscussionComment>> AddCommentsByDiscussionIdAsync(
      VssJsonCollectionWrapper<IEnumerable<DiscussionComment>> newComments,
      int discussionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("495211bd-b463-4578-86fe-924ea4953693");
      object obj1 = (object) new
      {
        discussionId = discussionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VssJsonCollectionWrapper<IEnumerable<DiscussionComment>>>(newComments, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<DiscussionComment>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteCommentAsync(
      int discussionId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("495211bd-b463-4578-86fe-924ea4953693"), (object) new
      {
        discussionId = discussionId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<DiscussionComment> GetCommentAsync(
      int discussionId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<DiscussionComment>(new HttpMethod("GET"), new Guid("495211bd-b463-4578-86fe-924ea4953693"), (object) new
      {
        discussionId = discussionId,
        commentId = commentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DiscussionCommentCollection> GetCommentsAsync(
      int discussionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<DiscussionCommentCollection>(new HttpMethod("GET"), new Guid("495211bd-b463-4578-86fe-924ea4953693"), (object) new
      {
        discussionId = discussionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DiscussionComment> UpdateCommentAsync(
      DiscussionComment newComment,
      int discussionId,
      short commentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("495211bd-b463-4578-86fe-924ea4953693");
      object obj1 = (object) new
      {
        discussionId = discussionId,
        commentId = commentId
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

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DiscussionComment>> AddCommentsAsync(
      VssJsonCollectionWrapper<IEnumerable<DiscussionComment>> newComments,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("20933fc0-b6a7-4a57-8111-a7458da5441b");
      HttpContent httpContent = (HttpContent) new ObjectContent<VssJsonCollectionWrapper<IEnumerable<DiscussionComment>>>(newComments, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<DiscussionComment>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DiscussionThread> CreateThreadAsync(
      DiscussionThread newThread,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a50ddbe2-1a1d-4c55-857f-73c6a3a31722");
      HttpContent httpContent = (HttpContent) new ObjectContent<DiscussionThread>(newThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DiscussionThread>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DiscussionThreadCollection> CreateThreadsAsync(
      VssJsonCollectionWrapper<DiscussionThreadCollection> newThreads,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a50ddbe2-1a1d-4c55-857f-73c6a3a31722");
      HttpContent httpContent = (HttpContent) new ObjectContent<VssJsonCollectionWrapper<DiscussionThreadCollection>>(newThreads, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DiscussionThreadCollection>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DiscussionThreadCollection> GetThreadsByWorkItemIdAsync(
      int workItemId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a50ddbe2-1a1d-4c55-857f-73c6a3a31722");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (workItemId), workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<DiscussionThreadCollection>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DiscussionThread> GetThreadAsync(
      int discussionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<DiscussionThread>(new HttpMethod("GET"), new Guid("010054f6-d9ed-4ed2-855f-7f86bff10c02"), (object) new
      {
        discussionId = discussionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<DiscussionThread> UpdateThreadAsync(
      DiscussionThread newThread,
      int discussionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("010054f6-d9ed-4ed2-855f-7f86bff10c02");
      object obj1 = (object) new
      {
        discussionId = discussionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DiscussionThread>(newThread, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DiscussionThread>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Dictionary<string, List<DiscussionThread>>> GetThreadsAsync(
      string[] artifactUris,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("255a0b5e-3c2f-43c2-a688-36c878210ba2");
      HttpContent httpContent = (HttpContent) new ObjectContent<string[]>(artifactUris, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Dictionary<string, List<DiscussionThread>>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
