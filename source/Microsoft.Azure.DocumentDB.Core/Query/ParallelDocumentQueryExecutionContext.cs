// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.ParallelDocumentQueryExecutionContext
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections.Generic;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class ParallelDocumentQueryExecutionContext : 
    CrossPartitionQueryExecutionContext<object>
  {
    private static readonly IComparer<DocumentProducerTree<object>> MoveNextComparer = (IComparer<DocumentProducerTree<object>>) new ParallelDocumentQueryExecutionContext.ParllelDocumentProducerTreeComparer();
    private static readonly Func<DocumentProducerTree<object>, int> FetchPriorityFunction = (Func<DocumentProducerTree<object>, int>) (documentProducerTree => int.Parse(documentProducerTree.PartitionKeyRange.Id));
    private static readonly IEqualityComparer<object> EqualityComparer = (IEqualityComparer<object>) new ParallelDocumentQueryExecutionContext.ParallelEqualityComparer();

    private ParallelDocumentQueryExecutionContext(
      DocumentQueryExecutionContextBase.InitParams constructorParams,
      string rewrittenQuery)
      : base(constructorParams, rewrittenQuery, ParallelDocumentQueryExecutionContext.MoveNextComparer, ParallelDocumentQueryExecutionContext.FetchPriorityFunction, ParallelDocumentQueryExecutionContext.EqualityComparer)
    {
    }

    protected override string ContinuationToken
    {
      get
      {
        if (this.IsDone)
          return (string) null;
        IEnumerable<DocumentProducer<object>> documentProducers = this.GetActiveDocumentProducers();
        return documentProducers.Count<DocumentProducer<object>>() <= 0 ? (string) null : JsonConvert.SerializeObject((object) documentProducers.Select<DocumentProducer<object>, CompositeContinuationToken>((Func<DocumentProducer<object>, CompositeContinuationToken>) (documentProducer => new CompositeContinuationToken()
        {
          Token = documentProducer.PreviousContinuationToken,
          Range = documentProducer.PartitionKeyRange.ToRange()
        })), DefaultJsonSerializationSettings.Value);
      }
    }

    public static async Task<ParallelDocumentQueryExecutionContext> CreateAsync(
      DocumentQueryExecutionContextBase.InitParams constructorParams,
      CrossPartitionQueryExecutionContext<object>.CrossPartitionInitParams initParams,
      CancellationToken token)
    {
      ParallelDocumentQueryExecutionContext context = new ParallelDocumentQueryExecutionContext(constructorParams, initParams.PartitionedQueryExecutionInfo.QueryInfo.RewrittenQuery);
      await context.InitializeAsync(initParams.CollectionRid, initParams.PartitionKeyRanges, initParams.InitialPageSize, initParams.RequestContinuation, token);
      return context;
    }

    public override async Task<FeedResponse<object>> DrainAsync(
      int maxElements,
      CancellationToken token)
    {
      ParallelDocumentQueryExecutionContext executionContext = this;
      token.ThrowIfCancellationRequested();
      DocumentProducerTree<object> currentDocumentProducerTree = executionContext.PopCurrentDocumentProducerTree();
      if (currentDocumentProducerTree.Current == null)
      {
        int num1 = await currentDocumentProducerTree.MoveNextAsync(token) ? 1 : 0;
      }
      int itemsLeftInCurrentPage = currentDocumentProducerTree.ItemsLeftInCurrentPage;
      List<object> results = new List<object>();
      for (int i = 0; i < Math.Min(itemsLeftInCurrentPage, maxElements); ++i)
      {
        results.Add(currentDocumentProducerTree.Current);
        int num2 = await currentDocumentProducerTree.MoveNextAsync(token) ? 1 : 0;
      }
      executionContext.PushCurrentDocumentProducerTree(currentDocumentProducerTree);
      return new FeedResponse<object>((IEnumerable<object>) results, results.Count, executionContext.GetResponseHeaders(), queryMetrics: executionContext.GetQueryMetrics(), partitionedClientSideRequestStatistics: executionContext.GetRequestStats(), responseLengthBytes: executionContext.GetAndResetResponseLengthBytes());
    }

    private Task InitializeAsync(
      string collectionRid,
      List<PartitionKeyRange> partitionKeyRanges,
      int initialPageSize,
      string requestContinuation,
      CancellationToken token)
    {
      Dictionary<string, CompositeContinuationToken> targetRangeToContinuationMap = (Dictionary<string, CompositeContinuationToken>) null;
      IReadOnlyList<PartitionKeyRange> partitionKeyRanges1;
      if (string.IsNullOrEmpty(requestContinuation))
      {
        partitionKeyRanges1 = (IReadOnlyList<PartitionKeyRange>) partitionKeyRanges;
      }
      else
      {
        CompositeContinuationToken[] suppliedCompositeContinuationTokens;
        try
        {
          suppliedCompositeContinuationTokens = JsonConvert.DeserializeObject<CompositeContinuationToken[]>(requestContinuation, DefaultJsonSerializationSettings.Value);
          foreach (CompositeContinuationToken continuationToken in suppliedCompositeContinuationTokens)
          {
            if (continuationToken.Range == null || continuationToken.Range.IsEmpty)
              throw new BadRequestException("Invalid Range in the continuation token " + requestContinuation + " for Parallel~Context.");
          }
          if (suppliedCompositeContinuationTokens.Length == 0)
            throw new BadRequestException("Invalid format for continuation token " + requestContinuation + " for Parallel~Context.");
        }
        catch (JsonException ex)
        {
          throw new BadRequestException("Invalid JSON in continuation token " + requestContinuation + " for Parallel~Context, exception: " + ex.Message);
        }
        partitionKeyRanges1 = this.GetPartitionKeyRangesForContinuation(suppliedCompositeContinuationTokens, partitionKeyRanges, out targetRangeToContinuationMap);
      }
      return this.InitializeAsync(collectionRid, partitionKeyRanges1, initialPageSize, this.QuerySpec, targetRangeToContinuationMap != null ? targetRangeToContinuationMap.ToDictionary<KeyValuePair<string, CompositeContinuationToken>, string, string>((Func<KeyValuePair<string, CompositeContinuationToken>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, CompositeContinuationToken>, string>) (kvp => kvp.Value.Token)) : (Dictionary<string, string>) null, true, (string) null, (Func<DocumentProducerTree<object>, Task>) null, token);
    }

    private IReadOnlyList<PartitionKeyRange> GetPartitionKeyRangesForContinuation(
      CompositeContinuationToken[] suppliedCompositeContinuationTokens,
      List<PartitionKeyRange> partitionKeyRanges,
      out Dictionary<string, CompositeContinuationToken> targetRangeToContinuationMap)
    {
      targetRangeToContinuationMap = new Dictionary<string, CompositeContinuationToken>();
      int continuationTokens = this.FindTargetRangeAndExtractContinuationTokens<CompositeContinuationToken>(partitionKeyRanges, ((IEnumerable<CompositeContinuationToken>) suppliedCompositeContinuationTokens).Select<CompositeContinuationToken, Tuple<CompositeContinuationToken, Range<string>>>((Func<CompositeContinuationToken, Tuple<CompositeContinuationToken, Range<string>>>) (token => Tuple.Create<CompositeContinuationToken, Range<string>>(token, token.Range))), out targetRangeToContinuationMap);
      return (IReadOnlyList<PartitionKeyRange>) new PartialReadOnlyList<PartitionKeyRange>((IReadOnlyList<PartitionKeyRange>) partitionKeyRanges, continuationTokens, partitionKeyRanges.Count - continuationTokens);
    }

    private sealed class ParllelDocumentProducerTreeComparer : 
      IComparer<DocumentProducerTree<object>>
    {
      public int Compare(
        DocumentProducerTree<object> documentProducerTree1,
        DocumentProducerTree<object> documentProducerTree2)
      {
        if (documentProducerTree1 == documentProducerTree2)
          return 0;
        if (documentProducerTree1.HasMoreResults && !documentProducerTree2.HasMoreResults)
          return -1;
        return !documentProducerTree1.HasMoreResults && documentProducerTree2.HasMoreResults ? 1 : string.CompareOrdinal(documentProducerTree1.PartitionKeyRange.MinInclusive, documentProducerTree2.PartitionKeyRange.MinInclusive);
      }
    }

    private sealed class ParallelEqualityComparer : IEqualityComparer<object>
    {
      public bool Equals(object x, object y) => x == y;

      public int GetHashCode(object obj) => obj.GetHashCode();
    }
  }
}
