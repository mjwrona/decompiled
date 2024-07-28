// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.ChangeFeedQuery`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
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

namespace Microsoft.Azure.Documents.Linq
{
  internal sealed class ChangeFeedQuery<TResource> : 
    IDocumentQuery<TResource>,
    IDocumentQuery,
    IDisposable
    where TResource : Resource, new()
  {
    private const string IfNoneMatchAllHeaderValue = "*";
    private readonly ResourceType resourceType;
    private readonly DocumentClient client;
    private readonly string resourceLink;
    private readonly ChangeFeedOptions feedOptions;
    private HttpStatusCode lastStatusCode = HttpStatusCode.OK;
    private string nextIfNoneMatch;
    private string ifModifiedSince;

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

    public Task<FeedResponse<TResult>> ExecuteNextAsync<TResult>(CancellationToken cancellationToken = default (CancellationToken)) => this.ReadDocumentChangeFeedAsync<TResult>(this.resourceLink, cancellationToken);

    public Task<FeedResponse<object>> ExecuteNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.ExecuteNextAsync<object>(cancellationToken);

    public Task<FeedResponse<TResult>> ReadDocumentChangeFeedAsync<TResult>(
      string resourceLink,
      CancellationToken cancellationToken)
    {
      IDocumentClientRetryPolicy retryPolicy = this.client.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<TResult>>((Func<Task<FeedResponse<TResult>>>) (() => this.ReadDocumentChangeFeedPrivateAsync<TResult>(resourceLink, retryPolicy, cancellationToken)), (IRetryPolicy) retryPolicy, cancellationToken);
    }

    private async Task<FeedResponse<TResult>> ReadDocumentChangeFeedPrivateAsync<TResult>(
      string link,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      using (DocumentServiceResponse feedResponseAsync = await this.GetFeedResponseAsync(link, this.resourceType, retryPolicyInstance, cancellationToken))
      {
        this.lastStatusCode = feedResponseAsync.StatusCode;
        this.nextIfNoneMatch = feedResponseAsync.Headers["etag"];
        PartitionedClientSideRequestStatistics fromSingleRequest = feedResponseAsync.RequestStats == null ? (PartitionedClientSideRequestStatistics) null : PartitionedClientSideRequestStatistics.CreateFromSingleRequest(this.feedOptions.PartitionKeyRangeId == null ? (this.feedOptions.PartitionKey == null ? "ChangeFeed" : this.feedOptions.PartitionKey.ToString()) : this.feedOptions.PartitionKeyRangeId, feedResponseAsync.RequestStats);
        if (feedResponseAsync.ResponseBody == null || feedResponseAsync.ResponseBody.Length <= 0L)
          return new FeedResponse<TResult>(Enumerable.Empty<TResult>(), 0, feedResponseAsync.Headers, true, partitionedClientSideRequestStatistics: fromSingleRequest);
        long length = feedResponseAsync.ResponseBody.Length;
        int itemCount = 0;
        FeedResponse<object> feedResponse = new FeedResponse<object>(feedResponseAsync.GetQueryResponse(typeof (TResource), out itemCount), itemCount, feedResponseAsync.Headers, true, partitionedClientSideRequestStatistics: fromSingleRequest, responseLengthBytes: length);
        // ISSUE: reference to a compiler-generated field
        if (ChangeFeedQuery<TResource>.\u003C\u003Eo__15<TResult>.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          ChangeFeedQuery<TResource>.\u003C\u003Eo__15<TResult>.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, FeedResponse<TResult>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (FeedResponse<TResult>), typeof (ChangeFeedQuery<TResource>)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return ChangeFeedQuery<TResource>.\u003C\u003Eo__15<TResult>.\u003C\u003Ep__0.Target((CallSite) ChangeFeedQuery<TResource>.\u003C\u003Eo__15<TResult>.\u003C\u003Ep__0, (object) feedResponse);
      }
    }

    private async Task<DocumentServiceResponse> GetFeedResponseAsync(
      string resourceLink,
      ResourceType resourceType,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      INameValueCollection headers = (INameValueCollection) new DictionaryNameValueCollection();
      if (this.feedOptions.MaxItemCount.HasValue)
        headers.Set("x-ms-max-item-count", this.feedOptions.MaxItemCount.ToString());
      if (this.feedOptions.SessionToken != null)
        headers.Set("x-ms-session-token", this.feedOptions.SessionToken);
      if (resourceType.IsPartitioned() && this.feedOptions.PartitionKeyRangeId == null && this.feedOptions.PartitionKey == null)
        throw new ForbiddenException(RMResources.PartitionKeyRangeIdOrPartitionKeyMustBeSpecified);
      if (this.nextIfNoneMatch != null)
        headers.Set("If-None-Match", this.nextIfNoneMatch);
      if (this.ifModifiedSince != null)
        headers.Set("If-Modified-Since", this.ifModifiedSince);
      headers.Set("A-IM", "Incremental Feed");
      if (this.feedOptions.PartitionKey != null)
      {
        PartitionKeyInternal internalKey = this.feedOptions.PartitionKey.InternalKey;
        headers.Set("x-ms-documentdb-partitionkey", internalKey.ToJsonString());
      }
      if (this.feedOptions.IncludeTentativeWrites)
        headers.Set("x-ms-cosmos-include-tentative-writes", bool.TrueString);
      if (this.feedOptions.MaxServiceWaitTime.HasValue)
        headers.Set("x-ms-cosmos-max-polling-interval", this.feedOptions.MaxServiceWaitTime.Value.TotalMilliseconds.ToString());
      DocumentServiceResponse feedResponseAsync;
      using (DocumentServiceRequest request = this.client.CreateDocumentServiceRequest(Microsoft.Azure.Documents.OperationType.ReadFeed, resourceLink, resourceType, headers))
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
