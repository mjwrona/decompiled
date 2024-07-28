// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.Numbers.CosmosInt32
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements.Numbers
{
  internal abstract class CosmosInt32 : 
    CosmosNumber,
    IEquatable<CosmosInt32>,
    IComparable<CosmosInt32>
  {
    public override Number64 Value => (Number64) (long) this.GetValue();

    public abstract int GetValue();

    public override void Accept(ICosmosNumberVisitor cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TResult Accept<TResult>(ICosmosNumberVisitor<TResult> cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TOutput Accept<TArg, TOutput>(
      ICosmosNumberVisitor<TArg, TOutput> cosmosNumberVisitor,
      TArg input)
    {
      return cosmosNumberVisitor.Visit(this, input);
    }

    public override bool Equals(CosmosNumber cosmosNumber) => cosmosNumber is CosmosInt32 cosmosInt32 && this.Equals(cosmosInt32);

    public bool Equals(CosmosInt32 cosmosInt32) => this.GetValue() == cosmosInt32.GetValue();

    public override int GetHashCode() => (int) MurmurHash3.Hash32<int>(this.GetValue(), 1791401667U);

    public int CompareTo(CosmosInt32 cosmosInt32) => this.GetValue().CompareTo(cosmosInt32.GetValue());

    public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteInt32Value(this.GetValue());

    public static CosmosInt32 Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosInt32) new CosmosInt32.LazyCosmosInt32(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosInt32 Create(int number) => (CosmosInt32) new CosmosInt32.EagerCosmosInt32(number);

    private sealed class EagerCosmosInt32 : CosmosInt32
    {
      private readonly int number;

      public EagerCosmosInt32(int number) => this.number = number;

      public override int GetValue() => this.number;
    }

    private sealed class LazyCosmosInt32 : CosmosInt32
    {
      private readonly Lazy<int> lazyNumber;

      public LazyCosmosInt32(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        if (jsonNavigator == null)
          throw new ArgumentNullException(nameof (jsonNavigator));
        JsonNodeType jsonNodeType = jsonNavigatorNode != null ? jsonNavigator.GetNodeType(jsonNavigatorNode) : throw new ArgumentNullException(nameof (jsonNavigatorNode));
        if (jsonNodeType != JsonNodeType.Int32)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be a {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.Int32, (object) jsonNodeType));
        this.lazyNumber = new Lazy<int>((Func<int>) (() => jsonNavigator.GetInt32Value(jsonNavigatorNode)));
      }

      public override int GetValue() => this.lazyNumber.Value;
    }
  }
}
