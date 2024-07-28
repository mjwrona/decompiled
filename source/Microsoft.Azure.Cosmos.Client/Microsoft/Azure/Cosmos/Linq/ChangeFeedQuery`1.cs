// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.ChangeFeedQuery`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal sealed class ChangeFeedQuery<TResource> : 
    IDocumentQuery<TResource>,
    IDocumentQuery,
    IDisposable
    where TResource : new()
  {
    private const string IfNoneMatchAllHeaderValue = "*";
    private readonly ResourceType resourceType;
    private readonly DocumentClient client;
    private readonly string resourceLink;
    private readonly ChangeFeedOptions feedOptions;
    private readonly string ifModifiedSince;
    private HttpStatusCode lastStatusCode = HttpStatusCode.OK;
    private string nextIfNoneMatch;

    public ChangeFeedQuery(
      DocumentClient client,
      ResourceType resourceType,
      string resourceLink,
      ChangeFeedOptions feedOptions)
    {
      this.client = client;
      this.resourceType = resourceType;
      this.resourceLink = resourceLink;
      this.feedOptions = feedOptions ?? new ChangeFeedOptions();
      if (feedOptions.PartitionKey != null && !string.IsNullOrEmpty(feedOptions.PartitionKeyRangeId))
        throw new ArgumentException(RMResources.PartitionKeyAndPartitionKeyRangeRangeIdBothSpecified, nameof (feedOptions));
      bool flag = true;
      if (feedOptions.RequestContinuation != null)
      {
        this.nextIfNoneMatch = feedOptions.RequestContinuation;
        flag = false;
      }
      if (feedOptions.StartTime.HasValue)
      {
        this.ifModifiedSince = this.ConvertToHttpTime(feedOptions.StartTime.Value);
        flag = false;
      }
      if (!flag || feedOptions.StartFromBeginning)
        return;
      this.nextIfNoneMatch = "*";
    }

    public void Dispose()
    {
    }

    public bool HasMoreResults => this.lastStatusCode != HttpStatusCode.NotModified;

    public Task<DocumentFeedResponse<TResult>> ExecuteNextAsync<TResult>(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ReadDocumentChangeFeedAsync<TResult>(this.resourceLink, cancellationToken);
    }

    public Task<DocumentFeedResponse<object>> ExecuteNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.ExecuteNextAsync<object>(cancellationToken);

    public Task<DocumentFeedResponse<TResult>> ReadDocumentChangeFeedAsync<TResult>(
      string resourceLink,
      CancellationToken cancellationToken)
    {
      IDocumentClientRetryPolicy retryPolicy = this.client.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<TResult>>((Func<Task<DocumentFeedResponse<TResult>>>) (() => this.ReadDocumentChangeFeedPrivateAsync<TResult>(resourceLink, retryPolicy, cancellationToken)), (IRetryPolicy) retryPolicy, cancellationToken);
    }

    private async Task<DocumentFeedResponse<TResult>> ReadDocumentChangeFeedPrivateAsync<TResult>(
      string link,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      using (DocumentServiceResponse feedResponseAsync = await this.GetFeedResponseAsync(link, this.resourceType, retryPolicyInstance, cancellationToken))
      {
        this.lastStatusCode = feedResponseAsync.StatusCode;
        this.nextIfNoneMatch = feedResponseAsync.Headers["etag"];
        if (feedResponseAsync.ResponseBody == null || feedResponseAsync.ResponseBody.Length <= 0L)
          return new DocumentFeedResponse<TResult>(Enumerable.Empty<TResult>(), 0, feedResponseAsync.Headers, true, requestStats: feedResponseAsync.RequestStats);
        long length = feedResponseAsync.ResponseBody.Length;
        int itemCount;
        DocumentFeedResponse<object> documentFeedResponse = new DocumentFeedResponse<object>(feedResponseAsync.GetQueryResponse(typeof (TResource), out itemCount), itemCount, feedResponseAsync.Headers, true, requestStats: feedResponseAsync.RequestStats, responseLengthBytes: length);
        // ISSUE: reference to a compiler-generated field
        if (ChangeFeedQuery<TResource>.\u003C\u003Eo__15<TResult>.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          ChangeFeedQuery<TResource>.\u003C\u003Eo__15<TResult>.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, DocumentFeedResponse<TResult>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (DocumentFeedResponse<TResult>), typeof (ChangeFeedQuery<TResource>)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return ChangeFeedQuery<TResource>.\u003C\u003Eo__15<TResult>.\u003C\u003Ep__0.Target((CallSite) ChangeFeedQuery<TResource>.\u003C\u003Eo__15<TResult>.\u003C\u003Ep__0, (object) documentFeedResponse);
      }
    }

    private async Task<DocumentServiceResponse> GetFeedResponseAsync(
      string resourceLink,
      ResourceType resourceType,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      RequestNameValueCollection headers = new RequestNameValueCollection();
      if (this.feedOptions.MaxItemCount.HasValue)
        headers.PageSize = this.feedOptions.MaxItemCount.ToString();
      if (this.feedOptions.SessionToken != null)
        headers.SessionToken = this.feedOptions.SessionToken;
      if (resourceType.IsPartitioned() && this.feedOptions.PartitionKeyRangeId == null && this.feedOptions.PartitionKey == null)
        throw new ForbiddenException(RMResources.PartitionKeyRangeIdOrPartitionKeyMustBeSpecified);
      if (this.nextIfNoneMatch != null)
        headers.IfNoneMatch = this.nextIfNoneMatch;
      if (this.ifModifiedSince != null)
        headers.IfModifiedSince = this.ifModifiedSince;
      headers.Set("A-IM", "Incremental Feed");
      if (this.feedOptions.PartitionKey != null)
      {
        PartitionKeyInternal internalKey = this.feedOptions.PartitionKey.InternalKey;
        headers.PartitionKey = internalKey.ToJsonString();
      }
      if (this.feedOptions.IncludeTentativeWrites)
        headers.IncludeTentativeWrites = bool.TrueString;
      DocumentServiceResponse feedResponseAsync;
      using (DocumentServiceRequest request = this.client.CreateDocumentServiceRequest(OperationType.ReadFeed, resourceLink, resourceType, (INameValueCollection) headers))
      {
        if (resourceType.IsPartitioned() && this.feedOptions.PartitionKeyRangeId != null)
          request.RouteTo(new PartitionKeyRangeIdentity(this.feedOptions.PartitionKeyRangeId));
        feedResponseAsync = await this.client.ReadFeedAsync(request, retryPolicyInstance, cancellationToken);
      }
      return feedResponseAsync;
    }

    private string ConvertToHttpTime(DateTime time) => time.ToUniversalTime().ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
  }
}
