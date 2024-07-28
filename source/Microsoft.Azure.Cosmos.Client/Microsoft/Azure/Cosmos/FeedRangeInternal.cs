// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedRangeInternal
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  [JsonConverter(typeof (FeedRangeInternalConverter))]
  [Serializable]
  internal abstract class FeedRangeInternal : FeedRange
  {
    internal abstract Task<List<Range<string>>> GetEffectiveRangesAsync(
      IRoutingMapProvider routingMapProvider,
      string containerRid,
      PartitionKeyDefinition partitionKeyDefinition,
      ITrace trace);

    internal abstract Task<IEnumerable<string>> GetPartitionKeyRangesAsync(
      IRoutingMapProvider routingMapProvider,
      string containerRid,
      PartitionKeyDefinition partitionKeyDefinition,
      CancellationToken cancellationToken,
      ITrace trace);

    internal abstract void Accept(IFeedRangeVisitor visitor);

    internal abstract void Accept<TInput>(IFeedRangeVisitor<TInput> visitor, TInput input);

    internal abstract TOutput Accept<TInput, TOutput>(
      IFeedRangeVisitor<TInput, TOutput> visitor,
      TInput input);

    internal abstract TResult Accept<TResult>(IFeedRangeTransformer<TResult> transformer);

    internal abstract Task<TResult> AcceptAsync<TResult>(
      IFeedRangeAsyncVisitor<TResult> visitor,
      CancellationToken cancellationToken = default (CancellationToken));

    internal abstract Task<TResult> AcceptAsync<TResult, TArg>(
      IFeedRangeAsyncVisitor<TResult, TArg> visitor,
      TArg argument,
      CancellationToken cancellationToken);

    public abstract override string ToString();

    public override string ToJsonString() => JsonConvert.SerializeObject((object) this);

    public static bool TryParse(string jsonString, out FeedRangeInternal feedRangeInternal)
    {
      try
      {
        feedRangeInternal = JsonConvert.DeserializeObject<FeedRangeInternal>(jsonString);
        return true;
      }
      catch (JsonReaderException ex)
      {
        DefaultTrace.TraceError("Unable to parse FeedRange from string.");
        feedRangeInternal = (FeedRangeInternal) null;
        return false;
      }
    }
  }
}
