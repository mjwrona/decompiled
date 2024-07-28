// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.Numbers.CosmosInt64
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements.Numbers
{
  internal abstract class CosmosInt64 : 
    CosmosNumber,
    IEquatable<CosmosInt64>,
    IComparable<CosmosInt64>
  {
    public override Number64 Value => (Number64) this.GetValue();

    public abstract long GetValue();

    public override void Accept(ICosmosNumberVisitor cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TResult Accept<TResult>(ICosmosNumberVisitor<TResult> cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TOutput Accept<TArg, TOutput>(
      ICosmosNumberVisitor<TArg, TOutput> cosmosNumberVisitor,
      TArg input)
    {
      return cosmosNumberVisitor.Visit(this, input);
    }

    public override bool Equals(CosmosNumber cosmosNumber) => cosmosNumber is CosmosInt64 cosmosInt64 && this.Equals(cosmosInt64);

    public bool Equals(CosmosInt64 cosmosInt64) => this.GetValue() == cosmosInt64.GetValue();

    public override int GetHashCode() => (int) MurmurHash3.Hash32<long>(this.GetValue(), 2562566505U);

    public int CompareTo(CosmosInt64 cosmosInt64) => this.GetValue().CompareTo(cosmosInt64.GetValue());

    public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteInt64Value(this.GetValue());

    public static CosmosInt64 Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosInt64) new CosmosInt64.LazyCosmosInt64(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosInt64 Create(long number) => (CosmosInt64) new CosmosInt64.EagerCosmosInt64(number);

    private sealed class EagerCosmosInt64 : CosmosInt64
    {
      private readonly long number;

      public EagerCosmosInt64(long number) => this.number = number;

      public override long GetValue() => this.number;
    }

    private sealed class LazyCosmosInt64 : CosmosInt64
    {
      private readonly Lazy<long> lazyNumber;

      public LazyCosmosInt64(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        if (jsonNavigator == null)
          throw new ArgumentNullException(nameof (jsonNavigator));
        JsonNodeType jsonNodeType = jsonNavigatorNode != null ? jsonNavigator.GetNodeType(jsonNavigatorNode) : throw new ArgumentNullException(nameof (jsonNavigatorNode));
        if (jsonNodeType != JsonNodeType.Int64)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be a {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.Int64, (object) jsonNodeType));
        this.lazyNumber = new Lazy<long>((Func<long>) (() => jsonNavigator.GetInt64Value(jsonNavigatorNode)));
      }

      public override long GetValue() => this.lazyNumber.Value;
    }
  }
}
