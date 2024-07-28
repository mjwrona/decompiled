// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy.OrderByQueryResult
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy
{
  internal readonly struct OrderByQueryResult
  {
    private readonly CosmosObject cosmosObject;

    public OrderByQueryResult(CosmosElement cosmosElement)
    {
      if (cosmosElement == (CosmosElement) null)
        throw new ArgumentNullException("cosmosElement must not be null.");
      this.cosmosObject = cosmosElement is CosmosObject cosmosObject ? cosmosObject : throw new ArgumentException("cosmosElement must not be an object.");
    }

    public string Rid
    {
      get
      {
        CosmosElement cosmosElement;
        if (!this.cosmosObject.TryGetValue("_rid", out cosmosElement) && !this.cosmosObject.TryGetValue("__sys_rid", out cosmosElement))
          throw new InvalidOperationException("Underlying object does not have an '_rid' or '__sys_rid' field.");
        return cosmosElement is CosmosString cosmosString ? UtfAnyString.op_Implicit(cosmosString.Value) : throw new InvalidOperationException("'_rid' or '__sys_rid' field was not a string.");
      }
    }

    public IReadOnlyList<OrderByItem> OrderByItems
    {
      get
      {
        CosmosElement cosmosElement1;
        if (!this.cosmosObject.TryGetValue("orderByItems", out cosmosElement1))
          throw new InvalidOperationException("Underlying object does not have an 'orderByItems' field.");
        List<OrderByItem> orderByItems = cosmosElement1 is CosmosArray cosmosArray ? new List<OrderByItem>(cosmosArray.Count) : throw new InvalidOperationException("orderByItems field was not an array.");
        foreach (CosmosElement cosmosElement2 in cosmosArray)
          orderByItems.Add(new OrderByItem(cosmosElement2));
        return (IReadOnlyList<OrderByItem>) orderByItems;
      }
    }

    public CosmosElement Payload
    {
      get
      {
        CosmosElement cosmosElement;
        return !this.cosmosObject.TryGetValue("payload", out cosmosElement) ? (CosmosElement) CosmosUndefined.Create() : cosmosElement;
      }
    }
  }
}
