// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedRangeEpk
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
  internal sealed class FeedRangeEpk : FeedRangeInternal
  {
    public static readonly FeedRangeEpk FullRange = new FeedRangeEpk(new Microsoft.Azure.Documents.Routing.Range<string>(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, true, false));

    public FeedRangeEpk(Microsoft.Azure.Documents.Routing.Range<string> range) => this.Range = range ?? throw new ArgumentNullException(nameof (range));

    public Microsoft.Azure.Documents.Routing.Range<string> Range { get; }

    internal override Task<List<Microsoft.Azure.Documents.Routing.Range<string>>> GetEffectiveRangesAsync(
      IRoutingMapProvider routingMapProvider,
      string containerRid,
      PartitionKeyDefinition partitionKeyDefinition,
      ITrace trace)
    {
      return Task.FromResult<List<Microsoft.Azure.Documents.Routing.Range<string>>>(new List<Microsoft.Azure.Documents.Routing.Range<string>>()
      {
        this.Range
      });
    }

    internal override async Task<IEnumerable<string>> GetPartitionKeyRangesAsync(
      IRoutingMapProvider routingMapProvider,
      string containerRid,
      PartitionKeyDefinition partitionKeyDefinition,
      CancellationToken cancellationToken,
      ITrace trace)
    {
      return (await routingMapProvider.TryGetOverlappingRangesAsync(containerRid, this.Range, trace)).Select<PartitionKeyRange, string>((Func<PartitionKeyRange, string>) (partitionKeyRange => partitionKeyRange.Id));
    }

    internal override void Accept(IFeedRangeVisitor visitor) => visitor.Visit(this);

    internal override void Accept<TInput>(IFeedRangeVisitor<TInput> visitor, TInput input) => visitor.Visit(this, input);

    internal override TOutput Accept<TInput, TOutput>(
      IFeedRangeVisitor<TInput, TOutput> visitor,
      TInput input)
    {
      return visitor.Visit(this, input);
    }

    internal override Task<TResult> AcceptAsync<TResult>(
      IFeedRangeAsyncVisitor<TResult> visitor,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return visitor.VisitAsync(this, cancellationToken);
    }

    internal override Task<TResult> AcceptAsync<TResult, TArg>(
      IFeedRangeAsyncVisitor<TResult, TArg> visitor,
      TArg argument,
      CancellationToken cancellationToken)
    {
      return visitor.VisitAsync(this, argument, cancellationToken);
    }

    public override string ToString() => this.Range.ToString();

    internal override TResult Accept<TResult>(IFeedRangeTransformer<TResult> transformer) => transformer.Visit(this);

    public override bool Equals(object obj) => this.Equals(obj as FeedRangeEpk);

    public bool Equals(FeedRangeEpk other) => other != null && this.Range.Min.Equals(other.Range.Min) && this.Range.Max.Equals(other.Range.Max) && this.Range.IsMinInclusive.Equals(other.Range.IsMinInclusive) && this.Range.IsMaxInclusive.Equals(other.Range.IsMaxInclusive);

    public override int GetHashCode() => this.Range.Min.GetHashCode() ^ this.Range.Max.GetHashCode() ^ this.Range.IsMinInclusive.GetHashCode() ^ this.Range.IsMaxInclusive.GetHashCode();
  }
}
