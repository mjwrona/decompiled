// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ReadFeed.Pagination.ReadFeedFeedRangeStateSerializer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.ReadFeed.Pagination
{
  internal static class ReadFeedFeedRangeStateSerializer
  {
    public static CosmosElement ToCosmosElement(FeedRangeState<ReadFeedState> feedRangeState)
    {
      Dictionary<string, CosmosElement> dictionary = new Dictionary<string, CosmosElement>()
      {
        {
          "FeedRange",
          FeedRangeCosmosElementSerializer.ToCosmosElement(feedRangeState.FeedRange)
        }
      };
      if (feedRangeState.State is ReadFeedBeginningState)
      {
        dictionary["State"] = (CosmosElement) CosmosNull.Create();
      }
      else
      {
        if (!(feedRangeState.State is ReadFeedContinuationState state))
          throw new InvalidOperationException("Unknown FeedRange State.");
        dictionary["State"] = state.ContinuationToken;
      }
      return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) dictionary);
    }

    private static class PropertyNames
    {
      public const string FeedRange = "FeedRange";
      public const string State = "State";
    }

    public static class Monadic
    {
      public static TryCatch<FeedRangeState<ReadFeedState>> CreateFromCosmosElement(
        CosmosElement cosmosElement)
      {
        if (cosmosElement == (CosmosElement) null)
          throw new ArgumentNullException(nameof (cosmosElement));
        if (!(cosmosElement is CosmosObject cosmosObject))
          return TryCatch<FeedRangeState<ReadFeedState>>.FromException((Exception) new FormatException(string.Format("Expected object for ChangeFeed Continuation: {0}.", (object) cosmosElement)));
        CosmosElement cosmosElement1;
        if (!cosmosObject.TryGetValue("FeedRange", out cosmosElement1))
          return TryCatch<FeedRangeState<ReadFeedState>>.FromException((Exception) new FormatException(string.Format("Expected '{0}' for '{1}': {2}.", (object) "FeedRange", (object) "FeedRangeState", (object) cosmosElement)));
        CosmosElement cosmosElement2;
        if (!cosmosObject.TryGetValue("State", out cosmosElement2))
          return TryCatch<FeedRangeState<ReadFeedState>>.FromException((Exception) new FormatException(string.Format("Expected '{0}' for '{1}': {2}.", (object) "State", (object) "FeedRangeState", (object) cosmosElement)));
        TryCatch<FeedRangeInternal> fromCosmosElement = FeedRangeCosmosElementSerializer.MonadicCreateFromCosmosElement(cosmosElement1);
        if (fromCosmosElement.Failed)
          return TryCatch<FeedRangeState<ReadFeedState>>.FromException((Exception) new FormatException(string.Format("Failed to parse '{0}' for '{1}': {2}.", (object) "FeedRange", (object) "FeedRangeState", (object) cosmosElement), fromCosmosElement.Exception));
        ReadFeedState state = !(cosmosElement2 is CosmosNull) ? ReadFeedState.Continuation(cosmosElement2) : ReadFeedState.Beginning();
        return TryCatch<FeedRangeState<ReadFeedState>>.FromResult(new FeedRangeState<ReadFeedState>(fromCosmosElement.Result, state));
      }
    }
  }
}
