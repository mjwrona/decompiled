// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedRangeCosmosElementSerializer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal static class FeedRangeCosmosElementSerializer
  {
    private const string TypePropertyName = "type";
    private const string ValuePropertyName = "value";
    private const string MinPropertyName = "min";
    private const string MaxPropertyName = "max";
    private const string LogicalPartitionKey = "Logical Partition Key";
    private const string PhysicalPartitionKeyRangeId = "Physical Partition Key Range Id";
    private const string EffectivePartitionKeyRange = "Effective Partition Key Range";

    public static TryCatch<FeedRangeInternal> MonadicCreateFromCosmosElement(
      CosmosElement cosmosElement)
    {
      if (cosmosElement == (CosmosElement) null)
        throw new ArgumentNullException(nameof (cosmosElement));
      if (!(cosmosElement is CosmosObject cosmosObject1))
        return TryCatch<FeedRangeInternal>.FromException((Exception) new FormatException(string.Format("Expected object for feed range: {0}.", (object) cosmosElement)));
      CosmosString typedCosmosElement1;
      if (!cosmosObject1.TryGetValue<CosmosString>("type", out typedCosmosElement1))
        return TryCatch<FeedRangeInternal>.FromException((Exception) new FormatException(string.Format("expected string type property for feed range: {0}.", (object) cosmosElement)));
      CosmosElement cosmosElement1;
      if (!cosmosObject1.TryGetValue("value", out cosmosElement1))
        return TryCatch<FeedRangeInternal>.FromException((Exception) new FormatException(string.Format("expected value property for feed range: {0}.", (object) cosmosElement)));
      FeedRangeInternal result;
      switch (UtfAnyString.op_Implicit(typedCosmosElement1.Value))
      {
        case "Logical Partition Key":
          if (!(cosmosElement1 is CosmosString cosmosString1))
            return TryCatch<FeedRangeInternal>.FromException((Exception) new FormatException(string.Format("expected string value property for logical partition key feed range: {0}.", (object) cosmosElement)));
          PartitionKey partitionKey;
          if (!PartitionKey.TryParseJsonString(UtfAnyString.op_Implicit(cosmosString1.Value), out partitionKey))
            return TryCatch<FeedRangeInternal>.FromException((Exception) new FormatException(string.Format("failed to parse logical partition key value: {0}.", (object) cosmosString1.Value)));
          result = (FeedRangeInternal) new FeedRangePartitionKey(partitionKey);
          break;
        case "Physical Partition Key Range Id":
          if (!(cosmosElement1 is CosmosString cosmosString2))
            return TryCatch<FeedRangeInternal>.FromException((Exception) new FormatException(string.Format("expected string value property for physical partition key feed range: {0}.", (object) cosmosElement)));
          result = (FeedRangeInternal) new FeedRangePartitionKeyRange(UtfAnyString.op_Implicit(cosmosString2.Value));
          break;
        case "Effective Partition Key Range":
          if (!(cosmosElement1 is CosmosObject cosmosObject2))
            return TryCatch<FeedRangeInternal>.FromException((Exception) new FormatException(string.Format("expected object value property for effective partition key range feed range: {0}.", (object) cosmosElement)));
          CosmosString typedCosmosElement2;
          if (!cosmosObject2.TryGetValue<CosmosString>("min", out typedCosmosElement2))
            return TryCatch<FeedRangeInternal>.FromException((Exception) new FormatException(string.Format("expected string value property for min effective partition key value: {0}.", (object) cosmosElement)));
          CosmosString typedCosmosElement3;
          if (!cosmosObject2.TryGetValue<CosmosString>("max", out typedCosmosElement3))
            return TryCatch<FeedRangeInternal>.FromException((Exception) new FormatException(string.Format("expected string value property for max effective partition key value: {0}.", (object) cosmosElement)));
          result = (FeedRangeInternal) new FeedRangeEpk(new Range<string>(UtfAnyString.op_Implicit(typedCosmosElement2.Value), UtfAnyString.op_Implicit(typedCosmosElement3.Value), true, false));
          break;
        default:
          throw new InvalidOperationException(string.Format("unexpected feed range type: {0}", (object) typedCosmosElement1.Value));
      }
      return TryCatch<FeedRangeInternal>.FromResult(result);
    }

    public static CosmosElement ToCosmosElement(FeedRangeInternal feedRange) => feedRange != null ? feedRange.Accept<CosmosElement>((IFeedRangeTransformer<CosmosElement>) FeedRangeCosmosElementSerializer.FeedRangeToCosmosElementTransformer.Singleton) : throw new ArgumentNullException(nameof (feedRange));

    private sealed class FeedRangeToCosmosElementTransformer : IFeedRangeTransformer<CosmosElement>
    {
      public static readonly FeedRangeCosmosElementSerializer.FeedRangeToCosmosElementTransformer Singleton = new FeedRangeCosmosElementSerializer.FeedRangeToCosmosElementTransformer();
      private static readonly CosmosElement LogicalPartitionKeyCosmosElement = (CosmosElement) CosmosString.Create("Logical Partition Key");
      private static readonly CosmosElement PhysicalPartitionKeyRangeIdCosmosElement = (CosmosElement) CosmosString.Create("Physical Partition Key Range Id");
      private static readonly CosmosElement EffecitvePartitionKeyRangeIdCosmosElement = (CosmosElement) CosmosString.Create("Effective Partition Key Range");

      private FeedRangeToCosmosElementTransformer()
      {
      }

      public CosmosElement Visit(FeedRangePartitionKey feedRange) => (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
      {
        {
          "type",
          FeedRangeCosmosElementSerializer.FeedRangeToCosmosElementTransformer.LogicalPartitionKeyCosmosElement
        },
        {
          "value",
          (CosmosElement) CosmosString.Create(feedRange.PartitionKey.ToJsonString())
        }
      });

      public CosmosElement Visit(FeedRangePartitionKeyRange feedRange) => (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
      {
        {
          "type",
          FeedRangeCosmosElementSerializer.FeedRangeToCosmosElementTransformer.PhysicalPartitionKeyRangeIdCosmosElement
        },
        {
          "value",
          (CosmosElement) CosmosString.Create(feedRange.PartitionKeyRangeId)
        }
      });

      public CosmosElement Visit(FeedRangeEpk feedRange) => (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
      {
        {
          "type",
          FeedRangeCosmosElementSerializer.FeedRangeToCosmosElementTransformer.EffecitvePartitionKeyRangeIdCosmosElement
        },
        {
          "value",
          (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
          {
            {
              "min",
              (CosmosElement) CosmosString.Create(feedRange.Range.Min)
            },
            {
              "max",
              (CosmosElement) CosmosString.Create(feedRange.Range.Max)
            }
          })
        }
      });
    }
  }
}
