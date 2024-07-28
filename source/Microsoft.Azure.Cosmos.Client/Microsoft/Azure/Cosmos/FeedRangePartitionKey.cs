// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedRangePartitionKey
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class FeedRangePartitionKey : FeedRangeInternal
  {
    public PartitionKey PartitionKey { get; }

    public FeedRangePartitionKey(PartitionKey partitionKey) => this.PartitionKey = partitionKey;

    internal override Task<List<Range<string>>> GetEffectiveRangesAsync(
      IRoutingMapProvider routingMapProvider,
      string containerRid,
      PartitionKeyDefinition partitionKeyDefinition,
      ITrace trace)
    {
      return Task.FromResult<List<Range<string>>>(new List<Range<string>>()
      {
        Range<string>.GetPointRange(this.PartitionKey.InternalKey.GetEffectivePartitionKeyString(partitionKeyDefinition))
      });
    }

    internal override async Task<IEnumerable<string>> GetPartitionKeyRangesAsync(
      IRoutingMapProvider routingMapProvider,
      string containerRid,
      PartitionKeyDefinition partitionKeyDefinition,
      CancellationToken cancellationToken,
      ITrace trace)
    {
      return (IEnumerable<string>) new List<string>()
      {
        (await routingMapProvider.TryGetRangeByEffectivePartitionKeyAsync(containerRid, this.PartitionKey.InternalKey.GetEffectivePartitionKeyString(partitionKeyDefinition), trace)).Id
      };
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

    public override string ToString() => this.PartitionKey.IsNone ? "None" : this.PartitionKey.InternalKey.ToJsonString();

    internal override TResult Accept<TResult>(IFeedRangeTransformer<TResult> transformer) => transformer.Visit(this);
  }
}
