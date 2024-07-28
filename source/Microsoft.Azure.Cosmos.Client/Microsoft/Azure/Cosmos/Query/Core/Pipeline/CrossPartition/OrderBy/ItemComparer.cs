// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy.ItemComparer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Json;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy
{
  internal sealed class ItemComparer : IComparer<
  #nullable disable
  CosmosElement>
  {
    public static readonly ItemComparer Instance = new ItemComparer();
    public static readonly ItemComparer.MinValueItem MinValue = ItemComparer.MinValueItem.Singleton;
    public static readonly ItemComparer.MaxValueItem MaxValue = ItemComparer.MaxValueItem.Singleton;

    public int Compare(CosmosElement element1, CosmosElement element2)
    {
      if ((object) element1 == (object) element2)
        return 0;
      if ((object) element1 == (object) ItemComparer.MinValueItem.Singleton)
        return -1;
      if ((object) element2 == (object) ItemComparer.MinValueItem.Singleton || (object) element1 == (object) ItemComparer.MaxValueItem.Singleton)
        return 1;
      return (object) element2 == (object) ItemComparer.MaxValueItem.Singleton ? -1 : element1.CompareTo(element2);
    }

    public static bool IsMinOrMax(CosmosElement obj) => obj == (CosmosElement) ItemComparer.MinValue || obj == (CosmosElement) ItemComparer.MaxValue;

    public sealed class MinValueItem : CosmosElement
    {
      public static readonly ItemComparer.MinValueItem Singleton = new ItemComparer.MinValueItem();

      private MinValueItem()
      {
      }

      public override void Accept(ICosmosElementVisitor cosmosElementVisitor) => throw new NotImplementedException();

      public override TResult Accept<TResult>(
        ICosmosElementVisitor<TResult> cosmosElementVisitor)
      {
        throw new NotImplementedException();
      }

      public override TResult Accept<TArg, TResult>(
        ICosmosElementVisitor<TArg, TResult> cosmosElementVisitor,
        TArg input)
      {
        throw new NotImplementedException();
      }

      public override bool Equals(CosmosElement cosmosElement) => cosmosElement is ItemComparer.MinValueItem;

      public override int GetHashCode() => 42;

      public override void WriteTo(IJsonWriter jsonWriter) => throw new NotImplementedException();
    }

    public sealed class MaxValueItem : CosmosElement
    {
      public static readonly ItemComparer.MaxValueItem Singleton = new ItemComparer.MaxValueItem();

      private MaxValueItem()
      {
      }

      public override void Accept(ICosmosElementVisitor cosmosElementVisitor) => throw new NotImplementedException();

      public override TResult Accept<TResult>(
        ICosmosElementVisitor<TResult> cosmosElementVisitor)
      {
        throw new NotImplementedException();
      }

      public override TResult Accept<TArg, TResult>(
        ICosmosElementVisitor<TArg, TResult> cosmosElementVisitor,
        TArg input)
      {
        throw new NotImplementedException();
      }

      public override bool Equals(CosmosElement cosmosElement) => cosmosElement is ItemComparer.MaxValueItem;

      public override int GetHashCode() => 1337;

      public override void WriteTo(IJsonWriter jsonWriter) => throw new NotImplementedException();
    }
  }
}
