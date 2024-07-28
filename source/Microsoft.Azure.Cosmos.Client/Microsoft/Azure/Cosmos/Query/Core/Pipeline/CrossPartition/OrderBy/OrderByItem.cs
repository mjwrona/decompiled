// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy.OrderByItem
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using System;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy
{
  internal readonly struct OrderByItem
  {
    private const string ItemName = "item";
    private readonly CosmosObject cosmosObject;

    public OrderByItem(CosmosElement cosmosElement)
    {
      if (cosmosElement == (CosmosElement) null)
        throw new ArgumentNullException("cosmosElement must not be null.");
      this.cosmosObject = cosmosElement is CosmosObject cosmosObject ? cosmosObject : throw new ArgumentException("cosmosElement must be an object.");
    }

    public CosmosElement Item
    {
      get
      {
        CosmosElement cosmosElement;
        if (!this.cosmosObject.TryGetValue("item", out cosmosElement))
          cosmosElement = (CosmosElement) CosmosUndefined.Create();
        return cosmosElement;
      }
    }

    public static CosmosElement ToCosmosElement(OrderByItem orderByItem) => (CosmosElement) orderByItem.cosmosObject;

    public static OrderByItem FromCosmosElement(CosmosElement cosmosElement) => new OrderByItem(cosmosElement);
  }
}
