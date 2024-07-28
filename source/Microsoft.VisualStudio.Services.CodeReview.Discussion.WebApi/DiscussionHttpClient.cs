// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.DiscussionHttpClient
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 443E2621-CB19-4319-96B1-AE621A0F5B5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi
{
  public class DiscussionHttpClient : DiscussionHttpClientBase
  {
    public DiscussionHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DiscussionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DiscussionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DiscussionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DiscussionHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<List<DiscussionComment>> AddCommentsAsync(
      IEnumerable<DiscussionComment> newComments,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.AddCommentsAsync(new VssJsonCollectionWrapper<IEnumerable<DiscussionComment>>((IEnumerable) newComments), userState, cancellationToken);
    }

    public Task<List<DiscussionComment>> AddCommentsByDiscussionIdAsync(
      IEnumerable<DiscussionComment> newComments,
      int discussionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.AddCommentsByDiscussionIdAsync(new VssJsonCollectionWrapper<IEnumerable<DiscussionComment>>((IEnumerable) newComments), discussionId, userState, cancellationToken);
    }

    public Task<DiscussionThread> UpdateThreadAsync(
      DiscussionThread newThread,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.UpdateThreadAsync(newThread, newThread.DiscussionId, userState, cancellationToken);
    }
  }
}
