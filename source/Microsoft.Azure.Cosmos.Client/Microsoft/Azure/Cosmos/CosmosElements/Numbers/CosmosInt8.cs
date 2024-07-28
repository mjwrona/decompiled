// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.Numbers.CosmosInt8
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements.Numbers
{
  internal abstract class CosmosInt8 : CosmosNumber, IEquatable<CosmosInt8>, IComparable<CosmosInt8>
  {
    public override Number64 Value => (Number64) (long) this.GetValue();

    public abstract sbyte GetValue();

    public override void Accept(ICosmosNumberVisitor cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TResult Accept<TResult>(ICosmosNumberVisitor<TResult> cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TOutput Accept<TArg, TOutput>(
      ICosmosNumberVisitor<TArg, TOutput> cosmosNumberVisitor,
      TArg input)
    {
      return cosmosNumberVisitor.Visit(this, input);
    }

    public override bool Equals(CosmosNumber cosmosNumber) => cosmosNumber is CosmosInt8 cosmosInt8 && this.Equals(cosmosInt8);

    public bool Equals(CosmosInt8 cosmosInt8) => (int) this.GetValue() == (int) cosmosInt8.GetValue();

    public override int GetHashCode() => (int) MurmurHash3.Hash32<sbyte>(this.GetValue(), 1301790982U);

    public int CompareTo(CosmosInt8 cosmosInt8) => this.GetValue().CompareTo(cosmosInt8.GetValue());

    public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteInt8Value(this.GetValue());

    public static CosmosInt8 Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosInt8) new CosmosInt8.LazyCosmosInt8(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosInt8 Create(sbyte number) => (CosmosInt8) new CosmosInt8.EagerCosmosInt8(number);

    private sealed class EagerCosmosInt8 : CosmosInt8
    {
      private readonly sbyte number;

      public EagerCosmosInt8(sbyte number) => this.number = number;

      public override sbyte GetValue() => this.number;
    }

    private sealed class LazyCosmosInt8 : CosmosInt8
    {
      private readonly Lazy<sbyte> lazyNumber;

      public LazyCosmosInt8(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        if (jsonNavigator == null)
          throw new ArgumentNullException(nameof (jsonNavigator));
        JsonNodeType jsonNodeType = jsonNavigatorNode != null ? jsonNavigator.GetNodeType(jsonNavigatorNode) : throw new ArgumentNullException(nameof (jsonNavigatorNode));
        if (jsonNodeType != JsonNodeType.Int8)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be a {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.Int8, (object) jsonNodeType));
        this.lazyNumber = new Lazy<sbyte>((Func<sbyte>) (() => jsonNavigator.GetInt8Value(jsonNavigatorNode)));
      }

      public override sbyte GetValue() => this.lazyNumber.Value;
    }
  }
}
