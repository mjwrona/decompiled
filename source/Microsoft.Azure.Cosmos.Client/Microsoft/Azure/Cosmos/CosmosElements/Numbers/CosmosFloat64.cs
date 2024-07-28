// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.Numbers.CosmosFloat64
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements.Numbers
{
  internal abstract class CosmosFloat64 : 
    CosmosNumber,
    IEquatable<CosmosFloat64>,
    IComparable<CosmosFloat64>
  {
    public override Number64 Value => (Number64) this.GetValue();

    public abstract double GetValue();

    public override void Accept(ICosmosNumberVisitor cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TResult Accept<TResult>(ICosmosNumberVisitor<TResult> cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TOutput Accept<TArg, TOutput>(
      ICosmosNumberVisitor<TArg, TOutput> cosmosNumberVisitor,
      TArg input)
    {
      return cosmosNumberVisitor.Visit(this, input);
    }

    public override bool Equals(CosmosNumber cosmosNumber) => cosmosNumber is CosmosFloat64 cosmosFloat64 && this.Equals(cosmosFloat64);

    public bool Equals(CosmosFloat64 cosmosFloat64) => this.GetValue() == cosmosFloat64.GetValue();

    public override int GetHashCode() => (int) MurmurHash3.Hash32<double>(this.GetValue(), 470975939U);

    public int CompareTo(CosmosFloat64 cosmosFloat64) => this.GetValue().CompareTo(cosmosFloat64.GetValue());

    public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteFloat64Value(this.GetValue());

    public static CosmosFloat64 Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosFloat64) new CosmosFloat64.LazyCosmosFloat64(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosFloat64 Create(double number) => (CosmosFloat64) new CosmosFloat64.EagerCosmosFloat64(number);

    private sealed class EagerCosmosFloat64 : CosmosFloat64
    {
      private readonly double number;

      public EagerCosmosFloat64(double number) => this.number = number;

      public override double GetValue() => this.number;
    }

    private sealed class LazyCosmosFloat64 : CosmosFloat64
    {
      private readonly Lazy<double> lazyNumber;

      public LazyCosmosFloat64(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        if (jsonNavigator == null)
          throw new ArgumentNullException(nameof (jsonNavigator));
        JsonNodeType jsonNodeType = jsonNavigatorNode != null ? jsonNavigator.GetNodeType(jsonNavigatorNode) : throw new ArgumentNullException(nameof (jsonNavigatorNode));
        if (jsonNodeType != JsonNodeType.Float64)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be a {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.Float64, (object) jsonNodeType));
        this.lazyNumber = new Lazy<double>((Func<double>) (() => jsonNavigator.GetFloat64Value(jsonNavigatorNode)));
      }

      public override double GetValue() => this.lazyNumber.Value;
    }
  }
}
