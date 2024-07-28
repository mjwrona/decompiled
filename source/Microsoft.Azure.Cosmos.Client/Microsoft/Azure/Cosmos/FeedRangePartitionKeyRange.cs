// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedRangePartitionKeyRange
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class FeedRangePartitionKeyRange : FeedRangeInternal
  {
    public FeedRangePartitionKeyRange(string partitionKeyRangeId) => this.PartitionKeyRangeId = partitionKeyRangeId;

    public string PartitionKeyRangeId { get; }

    internal override async Task<List<Range<string>>> GetEffectiveRangesAsync(
      IRoutingMapProvider routingMapProvider,
      string containerRid,
      PartitionKeyDefinition partitionKeyDefinition,
      ITrace trace)
    {
      PartitionKeyRange keyRangeByIdAsync = await routingMapProvider.TryGetPartitionKeyRangeByIdAsync(containerRid, this.PartitionKeyRangeId, trace);
      if (keyRangeByIdAsync == null)
        keyRangeByIdAsync = await routingMapProvider.TryGetPartitionKeyRangeByIdAsync(containerRid, this.PartitionKeyRangeId, trace, true);
      if (keyRangeByIdAsync == null)
      {
        string message = "The PartitionKeyRangeId: \"" + this.PartitionKeyRangeId + "\" is not valid for the current container " + containerRid + " .";
        string empty = string.Empty;
        Headers headers = new Headers();
        headers.SubStatusCode = SubStatusCodes.PartitionKeyRangeGone;
        NoOpTrace singleton = NoOpTrace.Singleton;
        throw CosmosExceptionFactory.Create(HttpStatusCode.Gone, message, empty, headers, (ITrace) singleton, (Error) null, (Exception) null);
      }
      return new List<Range<string>>()
      {
        keyRangeByIdAsync.ToRange()
      };
    }

    internal override Task<IEnumerable<string>> GetPartitionKeyRangesAsync(
      IRoutingMapProvider routingMapProvider,
      string containerRid,
      PartitionKeyDefinition partitionKeyDefinition,
      CancellationToken cancellationToken,
      ITrace trace)
    {
      return Task.FromResult<IEnumerable<string>>((IEnumerable<string>) new List<string>()
      {
        this.PartitionKeyRangeId
      });
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

    public override string ToString() => this.PartitionKeyRangeId;

    internal override TResult Accept<TResult>(IFeedRangeTransformer<TResult> transformer) => transformer.Visit(this);
  }
}
