// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.DocumentProducerTree`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class DocumentProducerTree<T> : IEnumerable<DocumentProducerTree<T>>, IEnumerable
  {
    private readonly DocumentProducer<T> root;
    private readonly PriorityQueue<DocumentProducerTree<T>> children;
    private readonly Func<PartitionKeyRange, string, DocumentProducerTree<T>> createDocumentProducerTreeCallback;
    private readonly IDocumentQueryClient client;
    private readonly bool deferFirstPage;
    private readonly string collectionRid;
    private readonly SemaphoreSlim executeWithSplitProofingSemaphore;

    public DocumentProducerTree(
      PartitionKeyRange partitionKeyRange,
      Func<PartitionKeyRange, string, int, DocumentServiceRequest> createRequestFunc,
      Func<DocumentServiceRequest, IDocumentClientRetryPolicy, CancellationToken, Task<FeedResponse<T>>> executeRequestFunc,
      Func<IDocumentClientRetryPolicy> createRetryPolicyFunc,
      Action<DocumentProducerTree<T>, int, double, QueryMetrics, IClientSideRequestStatistics, long, CancellationToken> produceAsyncCompleteCallback,
      IComparer<DocumentProducerTree<T>> documentProducerTreeComparer,
      IEqualityComparer<T> equalityComparer,
      IDocumentQueryClient client,
      bool deferFirstPage,
      string collectionRid,
      long initialPageSize = 50,
      string initialContinuationToken = null)
    {
      DocumentProducerTree<T> documentProducerTree = this;
      if (documentProducerTreeComparer == null)
        throw new ArgumentNullException(nameof (documentProducerTreeComparer));
      if (createRequestFunc == null)
        throw new ArgumentNullException(nameof (createRequestFunc));
      if (executeRequestFunc == null)
        throw new ArgumentNullException(nameof (executeRequestFunc));
      if (createRetryPolicyFunc == null)
        throw new ArgumentNullException(nameof (createRetryPolicyFunc));
      if (produceAsyncCompleteCallback == null)
        throw new ArgumentNullException(nameof (produceAsyncCompleteCallback));
      if (documentProducerTreeComparer == null)
        throw new ArgumentNullException(nameof (documentProducerTreeComparer));
      if (equalityComparer == null)
        throw new ArgumentNullException(nameof (equalityComparer));
      if (client == null)
        throw new ArgumentNullException(nameof (client));
      if (string.IsNullOrEmpty(collectionRid))
        throw new ArgumentException("collectionRid can not be null or empty.");
      this.root = new DocumentProducer<T>(partitionKeyRange, createRequestFunc, executeRequestFunc, createRetryPolicyFunc, (DocumentProducer<T>.ProduceAsyncCompleteDelegate) ((documentProducer, itemsBuffered, resourceUnitUsage, queryMetrics, requestStatistics, requestLength, token) => produceAsyncCompleteCallback(documentProducerTree, itemsBuffered, resourceUnitUsage, queryMetrics, requestStatistics, requestLength, token)), equalityComparer, initialPageSize, initialContinuationToken);
      this.children = new PriorityQueue<DocumentProducerTree<T>>(documentProducerTreeComparer, true);
      this.deferFirstPage = deferFirstPage;
      this.client = client;
      this.collectionRid = collectionRid;
      this.createDocumentProducerTreeCallback = DocumentProducerTree<T>.CreateDocumentProducerTreeCallback(createRequestFunc, executeRequestFunc, createRetryPolicyFunc, produceAsyncCompleteCallback, documentProducerTreeComparer, equalityComparer, client, deferFirstPage, collectionRid, initialPageSize);
      this.executeWithSplitProofingSemaphore = new SemaphoreSlim(1, 1);
    }

    public DocumentProducer<T> Root => this.root;

    public PartitionKeyRange PartitionKeyRange => this.CurrentDocumentProducerTree == this ? this.root.PartitionKeyRange : this.CurrentDocumentProducerTree.PartitionKeyRange;

    public string Filter
    {
      get => this.CurrentDocumentProducerTree == this ? this.root.Filter : this.CurrentDocumentProducerTree.Filter;
      set
      {
        if (this.CurrentDocumentProducerTree == this)
          this.root.Filter = value;
        else
          this.CurrentDocumentProducerTree.Filter = value;
      }
    }

    public DocumentProducerTree<T> CurrentDocumentProducerTree
    {
      get
      {
        if (!this.HasSplit || this.root.HasMoreResults)
          return this;
        this.children.Enqueue(this.children.Dequeue());
        return this.children.Peek().CurrentDocumentProducerTree;
      }
    }

    public bool IsAtBeginningOfPage => this.CurrentDocumentProducerTree == this ? this.root.IsAtBeginningOfPage : this.CurrentDocumentProducerTree.IsAtBeginningOfPage;

    public bool HasMoreResults
    {
      get
      {
        if (this.root.HasMoreResults)
          return true;
        return this.HasSplit && this.children.Peek().HasMoreResults;
      }
    }

    public bool HasMoreBackendResults
    {
      get
      {
        if (this.root.HasMoreBackendResults)
          return true;
        return this.HasSplit && this.children.Peek().HasMoreBackendResults;
      }
    }

    public int ItemsLeftInCurrentPage => this.CurrentDocumentProducerTree == this ? this.root.ItemsLeftInCurrentPage : this.CurrentDocumentProducerTree.ItemsLeftInCurrentPage;

    public int BufferedItemCount => this.CurrentDocumentProducerTree == this ? this.root.BufferedItemCount : this.CurrentDocumentProducerTree.BufferedItemCount;

    public bool IsActive => this.root.IsActive || this.children.Any<DocumentProducerTree<T>>((Func<DocumentProducerTree<T>, bool>) (child => child.IsActive));

    public long PageSize
    {
      get => this.CurrentDocumentProducerTree == this ? this.root.PageSize : this.CurrentDocumentProducerTree.PageSize;
      set
      {
        if (this.CurrentDocumentProducerTree == this)
          this.root.PageSize = value;
        else
          this.CurrentDocumentProducerTree.PageSize = value;
      }
    }

    public Guid ActivityId => this.CurrentDocumentProducerTree == this ? this.root.ActivityId : this.CurrentDocumentProducerTree.ActivityId;

    public T Current => this.CurrentDocumentProducerTree == this ? this.root.Current : this.CurrentDocumentProducerTree.Current;

    private bool HasSplit => this.children.Count != 0;

    public async Task<bool> MoveNextAsync(CancellationToken token)
    {
      DocumentProducerTree<T> documentProducerTree = this;
      // ISSUE: reference to a compiler-generated field
      if (DocumentProducerTree<T>.\u003C\u003Eo__38.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DocumentProducerTree<T>.\u003C\u003Eo__38.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (DocumentProducerTree<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target = DocumentProducerTree<T>.\u003C\u003Eo__38.\u003C\u003Ep__0.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p0 = DocumentProducerTree<T>.\u003C\u003Eo__38.\u003C\u003Ep__0;
      return target((CallSite) p0, await documentProducerTree.ExecuteWithSplitProofing(new Func<CancellationToken, Task<object>>(documentProducerTree.MoveNextAsyncImplementation), false, token));
    }

    public async Task<bool> MoveNextIfNotSplit(CancellationToken token)
    {
      DocumentProducerTree<T> documentProducerTree = this;
      // ISSUE: reference to a compiler-generated field
      if (DocumentProducerTree<T>.\u003C\u003Eo__39.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DocumentProducerTree<T>.\u003C\u003Eo__39.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (DocumentProducerTree<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target = DocumentProducerTree<T>.\u003C\u003Eo__39.\u003C\u003Ep__0.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p0 = DocumentProducerTree<T>.\u003C\u003Eo__39.\u003C\u003Ep__0;
      return target((CallSite) p0, await documentProducerTree.ExecuteWithSplitProofing(new Func<CancellationToken, Task<object>>(documentProducerTree.MoveNextIfNotSplitAsyncImplementation), false, token));
    }

    public Task BufferMoreDocuments(CancellationToken token) => (Task) this.ExecuteWithSplitProofing(new Func<CancellationToken, Task<object>>(this.BufferMoreDocumentsImplementation), true, token);

    public IEnumerable<DocumentProducer<T>> GetActiveDocumentProducers()
    {
      if (!this.HasSplit)
      {
        if (this.root.IsActive)
          yield return this.root;
      }
      else if (this.root.IsActive && this.root.BufferedItemCount != 0)
      {
        yield return this.root;
      }
      else
      {
        foreach (DocumentProducerTree<T> child in this.children)
        {
          foreach (DocumentProducer<T> documentProducer in child.GetActiveDocumentProducers())
            yield return documentProducer;
        }
      }
    }

    public IEnumerator<DocumentProducerTree<T>> GetEnumerator()
    {
      DocumentProducerTree<T> documentProducerTree1 = this;
      if (documentProducerTree1.children.Count == 0)
        yield return documentProducerTree1;
      foreach (DocumentProducerTree<T> child in documentProducerTree1.children)
      {
        foreach (DocumentProducerTree<T> documentProducerTree2 in child)
          yield return documentProducerTree2;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    private static Func<PartitionKeyRange, string, DocumentProducerTree<T>> CreateDocumentProducerTreeCallback(
      Func<PartitionKeyRange, string, int, DocumentServiceRequest> createRequestFunc,
      Func<DocumentServiceRequest, IDocumentClientRetryPolicy, CancellationToken, Task<FeedResponse<T>>> executeRequestFunc,
      Func<IDocumentClientRetryPolicy> createRetryPolicyFunc,
      Action<DocumentProducerTree<T>, int, double, QueryMetrics, IClientSideRequestStatistics, long, CancellationToken> produceAsyncCompleteCallback,
      IComparer<DocumentProducerTree<T>> documentProducerTreeComparer,
      IEqualityComparer<T> equalityComparer,
      IDocumentQueryClient documentClient,
      bool deferFirstPage,
      string collectionRid,
      long initialPageSize = 50)
    {
      return (Func<PartitionKeyRange, string, DocumentProducerTree<T>>) ((partitionKeyRange, continuationToken) => new DocumentProducerTree<T>(partitionKeyRange, createRequestFunc, executeRequestFunc, createRetryPolicyFunc, produceAsyncCompleteCallback, documentProducerTreeComparer, equalityComparer, documentClient, deferFirstPage, collectionRid, initialPageSize, continuationToken));
    }

    private static bool IsSplitException(DocumentClientException ex)
    {
      HttpStatusCode? statusCode = ex.StatusCode;
      HttpStatusCode httpStatusCode = HttpStatusCode.Gone;
      return statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue && ex.GetSubStatus() == SubStatusCodes.PartitionKeyRangeGone;
    }

    private async Task<object> MoveNextAsyncImplementation(CancellationToken token)
    {
      DocumentProducerTree<T> documentProducerTree = this;
      if (!documentProducerTree.HasMoreResults)
        return (object) false;
      return documentProducerTree.CurrentDocumentProducerTree == documentProducerTree ? (object) await documentProducerTree.root.MoveNextAsync(token) : (object) await documentProducerTree.CurrentDocumentProducerTree.MoveNextAsync(token);
    }

    private Task<object> MoveNextIfNotSplitAsyncImplementation(CancellationToken token) => this.HasSplit ? Task.FromResult<object>((object) false) : this.MoveNextAsyncImplementation(token);

    private async Task<object> BufferMoreDocumentsImplementation(CancellationToken token)
    {
      DocumentProducerTree<T> documentProducerTree = this;
      if (documentProducerTree.CurrentDocumentProducerTree == documentProducerTree)
      {
        if (!documentProducerTree.HasMoreBackendResults || documentProducerTree.HasSplit)
          return (object) null;
        await documentProducerTree.root.BufferMoreDocuments(token);
      }
      else
        await documentProducerTree.CurrentDocumentProducerTree.BufferMoreDocuments(token);
      return (object) null;
    }

    private async Task<object> ExecuteWithSplitProofing(
      Func<CancellationToken, Task<object>> function,
      bool functionNeedsBeReexecuted,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      while (true)
      {
        try
        {
          int num1;
          try
          {
            await this.executeWithSplitProofingSemaphore.WaitAsync();
            return await function(cancellationToken);
          }
          catch (DocumentClientException ex) when (DocumentProducerTree<T>.IsSplitException(ex))
          {
            num1 = 1;
          }
          if (num1 == 1)
          {
            DocumentProducerTree<T> splitDocumentProducerTree = this.CurrentDocumentProducerTree;
            if (splitDocumentProducerTree.BufferedItemCount == 0)
              splitDocumentProducerTree.Root.Shutdown();
            foreach (PartitionKeyRange replacementRange in await this.GetReplacementRanges(splitDocumentProducerTree.PartitionKeyRange, this.collectionRid))
            {
              DocumentProducerTree<T> replacementDocumentProducerTree = this.createDocumentProducerTreeCallback(replacementRange, splitDocumentProducerTree.root.BackendContinuationToken);
              if (!this.deferFirstPage)
              {
                int num2 = await replacementDocumentProducerTree.MoveNextAsync(cancellationToken) ? 1 : 0;
              }
              replacementDocumentProducerTree.Filter = splitDocumentProducerTree.root.Filter;
              if (replacementDocumentProducerTree.HasMoreResults && !splitDocumentProducerTree.children.TryAdd(replacementDocumentProducerTree))
                throw new InvalidOperationException("Unable to add child document producer tree");
              replacementDocumentProducerTree = (DocumentProducerTree<T>) null;
            }
            if (!functionNeedsBeReexecuted)
              return (object) true;
            splitDocumentProducerTree = (DocumentProducerTree<T>) null;
          }
        }
        finally
        {
          this.executeWithSplitProofingSemaphore.Release();
        }
      }
    }

    private async Task<List<PartitionKeyRange>> GetReplacementRanges(
      PartitionKeyRange targetRange,
      string collectionRid)
    {
      List<PartitionKeyRange> list = (await (await this.client.GetRoutingMapProviderAsync()).TryGetOverlappingRangesAsync(collectionRid, targetRange.ToRange(), true)).ToList<PartitionKeyRange>();
      string minInclusive = list.First<PartitionKeyRange>().MinInclusive;
      string maxExclusive = list.Last<PartitionKeyRange>().MaxExclusive;
      if (!minInclusive.Equals(targetRange.MinInclusive, StringComparison.Ordinal) || !maxExclusive.Equals(targetRange.MaxExclusive, StringComparison.Ordinal))
        throw new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Target range and Replacement range has mismatched min/max. Target range: [{0}, {1}). Replacement range: [{2}, {3}).", (object) targetRange.MinInclusive, (object) targetRange.MaxExclusive, (object) minInclusive, (object) maxExclusive));
      return list;
    }
  }
}
