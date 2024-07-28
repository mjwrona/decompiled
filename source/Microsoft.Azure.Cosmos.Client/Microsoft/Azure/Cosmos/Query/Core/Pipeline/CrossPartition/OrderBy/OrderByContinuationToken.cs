// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy.OrderByContinuationToken
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.CosmosElements.Numbers;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy
{
  internal sealed class OrderByContinuationToken : IPartitionedToken
  {
    public OrderByContinuationToken(
      ParallelContinuationToken compositeContinuationToken,
      IReadOnlyList<OrderByItem> orderByItems,
      string rid,
      int skipCount,
      string filter)
    {
      if (orderByItems.Count == 0)
        throw new ArgumentException("orderByItems can not be empty.");
      if (string.IsNullOrWhiteSpace(rid))
        throw new ArgumentNullException("rid can not be null or empty or whitespace.");
      if (skipCount < 0)
        throw new ArgumentException("skipCount can not be negative.");
      this.ParallelContinuationToken = compositeContinuationToken ?? throw new ArgumentNullException(nameof (compositeContinuationToken));
      this.OrderByItems = orderByItems ?? throw new ArgumentNullException(nameof (orderByItems));
      this.Rid = rid;
      this.SkipCount = skipCount;
      this.Filter = filter;
    }

    [JsonProperty("compositeToken")]
    public ParallelContinuationToken ParallelContinuationToken { get; }

    [JsonProperty("orderByItems")]
    public IReadOnlyList<OrderByItem> OrderByItems { get; }

    [JsonProperty("rid")]
    public string Rid { get; }

    [JsonProperty("skipCount")]
    public int SkipCount { get; }

    [JsonProperty("filter")]
    public string Filter { get; }

    [JsonIgnore]
    public Microsoft.Azure.Documents.Routing.Range<string> Range => this.ParallelContinuationToken.Range;

    public static CosmosElement ToCosmosElement(OrderByContinuationToken orderByContinuationToken)
    {
      CosmosElement cosmosElement1 = ParallelContinuationToken.ToCosmosElement(orderByContinuationToken.ParallelContinuationToken);
      List<CosmosElement> cosmosElementList = new List<CosmosElement>();
      foreach (OrderByItem orderByItem in (IEnumerable<OrderByItem>) orderByContinuationToken.OrderByItems)
        cosmosElementList.Add(OrderByItem.ToCosmosElement(orderByItem));
      CosmosArray cosmosArray = CosmosArray.Create((IEnumerable<CosmosElement>) cosmosElementList);
      CosmosElement cosmosElement2 = orderByContinuationToken.Filter == null ? (CosmosElement) CosmosNull.Create() : (CosmosElement) CosmosString.Create(orderByContinuationToken.Filter);
      return (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
      {
        {
          "compositeToken",
          cosmosElement1
        },
        {
          "orderByItems",
          (CosmosElement) cosmosArray
        },
        {
          "rid",
          (CosmosElement) CosmosString.Create(orderByContinuationToken.Rid)
        },
        {
          "skipCount",
          (CosmosElement) CosmosNumber64.Create((Number64) (long) orderByContinuationToken.SkipCount)
        },
        {
          "filter",
          cosmosElement2
        }
      });
    }

    public static TryCatch<OrderByContinuationToken> TryCreateFromCosmosElement(
      CosmosElement cosmosElement)
    {
      if (!(cosmosElement is CosmosObject cosmosObject))
        return TryCatch<OrderByContinuationToken>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} is not an object: {1}", (object) nameof (OrderByContinuationToken), (object) cosmosElement)));
      CosmosElement cosmosElement1;
      if (!cosmosObject.TryGetValue("compositeToken", out cosmosElement1))
        return TryCatch<OrderByContinuationToken>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} is missing field: '{1}': {2}", (object) nameof (OrderByContinuationToken), (object) "compositeToken", (object) cosmosElement)));
      TryCatch<ParallelContinuationToken> fromCosmosElement = ParallelContinuationToken.TryCreateFromCosmosElement(cosmosElement1);
      if (!fromCosmosElement.Succeeded)
        return TryCatch<OrderByContinuationToken>.FromException(fromCosmosElement.Exception);
      ParallelContinuationToken result = fromCosmosElement.Result;
      CosmosArray typedCosmosElement1;
      if (!cosmosObject.TryGetValue<CosmosArray>("orderByItems", out typedCosmosElement1))
        return TryCatch<OrderByContinuationToken>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} is missing field: '{1}': {2}", (object) nameof (OrderByContinuationToken), (object) "orderByItems", (object) cosmosElement)));
      List<OrderByItem> list = typedCosmosElement1.Select<CosmosElement, OrderByItem>((Func<CosmosElement, OrderByItem>) (x => OrderByItem.FromCosmosElement(x))).ToList<OrderByItem>();
      CosmosString typedCosmosElement2;
      if (!cosmosObject.TryGetValue<CosmosString>("rid", out typedCosmosElement2))
        return TryCatch<OrderByContinuationToken>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} is missing field: '{1}': {2}", (object) nameof (OrderByContinuationToken), (object) "rid", (object) cosmosElement)));
      string rid = UtfAnyString.op_Implicit(typedCosmosElement2.Value);
      CosmosNumber64 typedCosmosElement3;
      if (!cosmosObject.TryGetValue<CosmosNumber64>("skipCount", out typedCosmosElement3))
        return TryCatch<OrderByContinuationToken>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} is missing field: '{1}': {2}", (object) nameof (OrderByContinuationToken), (object) "skipCount", (object) cosmosElement)));
      int skipCount = (int) Number64.ToLong(typedCosmosElement3.GetValue());
      CosmosElement cosmosElement2;
      if (!cosmosObject.TryGetValue("filter", out cosmosElement2))
        return TryCatch<OrderByContinuationToken>.FromException((Exception) new MalformedContinuationTokenException(string.Format("{0} is missing field: '{1}': {2}", (object) nameof (OrderByContinuationToken), (object) "filter", (object) cosmosElement)));
      string filter = !(cosmosElement2 is CosmosString cosmosString) ? (string) null : UtfAnyString.op_Implicit(cosmosString.Value);
      return TryCatch<OrderByContinuationToken>.FromResult(new OrderByContinuationToken(result, (IReadOnlyList<OrderByItem>) list, rid, skipCount, filter));
    }

    private static class PropertyNames
    {
      public const string CompositeToken = "compositeToken";
      public const string OrderByItems = "orderByItems";
      public const string Rid = "rid";
      public const string SkipCount = "skipCount";
      public const string Filter = "filter";
    }
  }
}
