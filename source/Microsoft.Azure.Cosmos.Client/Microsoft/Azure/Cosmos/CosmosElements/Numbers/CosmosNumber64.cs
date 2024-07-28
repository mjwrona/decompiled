// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.Numbers.CosmosNumber64
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements.Numbers
{
  internal abstract class CosmosNumber64 : 
    CosmosNumber,
    IEquatable<CosmosNumber64>,
    IComparable<CosmosNumber64>
  {
    public override Number64 Value => this.GetValue();

    public abstract Number64 GetValue();

    public override void Accept(ICosmosNumberVisitor cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TResult Accept<TResult>(ICosmosNumberVisitor<TResult> cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TOutput Accept<TArg, TOutput>(
      ICosmosNumberVisitor<TArg, TOutput> cosmosNumberVisitor,
      TArg input)
    {
      return cosmosNumberVisitor.Visit(this, input);
    }

    public override bool Equals(CosmosNumber cosmosNumber) => cosmosNumber is CosmosNumber64 cosmosNumber64 && this.Equals(cosmosNumber64);

    public bool Equals(CosmosNumber64 cosmosNumber64) => this.GetValue() == cosmosNumber64.GetValue();

    public override int GetHashCode() => (int) MurmurHash3.Hash32<Number64.DoubleEx>(Number64.ToDoubleEx(this.GetValue()), 1943952435U);

    public int CompareTo(CosmosNumber64 cosmosNumber64) => this.GetValue().CompareTo(cosmosNumber64.GetValue());

    public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteNumber64Value(this.GetValue());

    public static CosmosNumber64 Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosNumber64) new CosmosNumber64.LazyCosmosNumber64(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosNumber64 Create(Number64 number) => (CosmosNumber64) new CosmosNumber64.EagerCosmosNumber64(number);

    private sealed class EagerCosmosNumber64 : CosmosNumber64
    {
      private readonly Number64 number;

      public EagerCosmosNumber64(Number64 number) => this.number = number;

      public override Number64 GetValue() => this.number;
    }

    private sealed class LazyCosmosNumber64 : CosmosNumber64
    {
      private readonly Lazy<Number64> lazyNumber;

      public LazyCosmosNumber64(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        if (jsonNavigator == null)
          throw new ArgumentNullException(nameof (jsonNavigator));
        JsonNodeType jsonNodeType = jsonNavigatorNode != null ? jsonNavigator.GetNodeType(jsonNavigatorNode) : throw new ArgumentNullException(nameof (jsonNavigatorNode));
        if (jsonNodeType != JsonNodeType.Number64)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be a {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.Number64, (object) jsonNodeType));
        this.lazyNumber = new Lazy<Number64>((Func<Number64>) (() => jsonNavigator.GetNumber64Value(jsonNavigatorNode)));
      }

      public override Number64 GetValue() => this.lazyNumber.Value;
    }
  }
}
