// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedRangePartitionKeyRangeExtractor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class FeedRangePartitionKeyRangeExtractor : 
    IFeedRangeAsyncVisitor<IReadOnlyList<Range<string>>>
  {
    private readonly ContainerInternal container;

    public FeedRangePartitionKeyRangeExtractor(ContainerInternal container) => this.container = container ?? throw new ArgumentNullException(nameof (container));

    public async Task<IReadOnlyList<Range<string>>> VisitAsync(
      FeedRangePartitionKey feedRange,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PartitionKeyRangeCache partitionKeyRangeCache = await this.container.ClientContext.DocumentClient.GetPartitionKeyRangeCacheAsync((ITrace) NoOpTrace.Singleton);
      PartitionKeyDefinition partitionKeyDefinition = await this.container.GetPartitionKeyDefinitionAsync(cancellationToken);
      FeedRangePartitionKey rangePartitionKey = feedRange;
      IRoutingMapProvider routingMapProvider = (IRoutingMapProvider) partitionKeyRangeCache;
      List<Range<string>> effectiveRangesAsync = await rangePartitionKey.GetEffectiveRangesAsync(routingMapProvider, await this.container.GetCachedRIDAsync(false, (ITrace) NoOpTrace.Singleton, cancellationToken), partitionKeyDefinition, (ITrace) NoOpTrace.Singleton);
      rangePartitionKey = (FeedRangePartitionKey) null;
      routingMapProvider = (IRoutingMapProvider) null;
      IReadOnlyList<Range<string>> rangeList = (IReadOnlyList<Range<string>>) effectiveRangesAsync;
      partitionKeyRangeCache = (PartitionKeyRangeCache) null;
      partitionKeyDefinition = (PartitionKeyDefinition) null;
      return rangeList;
    }

    public async Task<IReadOnlyList<Range<string>>> VisitAsync(
      FeedRangePartitionKeyRange feedRange,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PartitionKeyRangeCache keyRangeCacheAsync = await this.container.ClientContext.DocumentClient.GetPartitionKeyRangeCacheAsync((ITrace) NoOpTrace.Singleton);
      FeedRangePartitionKeyRange partitionKeyRange = feedRange;
      IRoutingMapProvider routingMapProvider = (IRoutingMapProvider) keyRangeCacheAsync;
      List<Range<string>> effectiveRangesAsync = await partitionKeyRange.GetEffectiveRangesAsync(routingMapProvider, await this.container.GetCachedRIDAsync(false, (ITrace) NoOpTrace.Singleton, cancellationToken), (PartitionKeyDefinition) null, (ITrace) NoOpTrace.Singleton);
      partitionKeyRange = (FeedRangePartitionKeyRange) null;
      routingMapProvider = (IRoutingMapProvider) null;
      return (IReadOnlyList<Range<string>>) effectiveRangesAsync;
    }

    public async Task<IReadOnlyList<Range<string>>> VisitAsync(
      FeedRangeEpk feedRange,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PartitionKeyRangeCache partitionKeyRangeCache = await this.container.ClientContext.DocumentClient.GetPartitionKeyRangeCacheAsync((ITrace) NoOpTrace.Singleton);
      IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await partitionKeyRangeCache.TryGetOverlappingRangesAsync(await this.container.GetCachedRIDAsync(false, (ITrace) NoOpTrace.Singleton, cancellationToken), feedRange.Range, (ITrace) NoOpTrace.Singleton, false);
      partitionKeyRangeCache = (PartitionKeyRangeCache) null;
      return (IReadOnlyList<Range<string>>) overlappingRangesAsync.Select<PartitionKeyRange, Range<string>>((Func<PartitionKeyRange, Range<string>>) (pkRange => pkRange.ToRange())).ToList<Range<string>>();
    }
  }
}
