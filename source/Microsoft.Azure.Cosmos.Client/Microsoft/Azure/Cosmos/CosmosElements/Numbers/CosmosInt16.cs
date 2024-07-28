// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.Numbers.CosmosInt16
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements.Numbers
{
  internal abstract class CosmosInt16 : 
    CosmosNumber,
    IEquatable<CosmosInt16>,
    IComparable<CosmosInt16>
  {
    public override Number64 Value => (Number64) (long) this.GetValue();

    public abstract short GetValue();

    public override void Accept(ICosmosNumberVisitor cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TResult Accept<TResult>(ICosmosNumberVisitor<TResult> cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TOutput Accept<TArg, TOutput>(
      ICosmosNumberVisitor<TArg, TOutput> cosmosNumberVisitor,
      TArg input)
    {
      return cosmosNumberVisitor.Visit(this, input);
    }

    public override bool Equals(CosmosNumber cosmosNumber) => cosmosNumber is CosmosInt16 cosmosInt16 && this.Equals(cosmosInt16);

    public bool Equals(CosmosInt16 cosmosInt16) => (int) this.GetValue() == (int) cosmosInt16.GetValue();

    public override int GetHashCode() => (int) MurmurHash3.Hash32<short>(this.GetValue(), 1176550641U);

    public int CompareTo(CosmosInt16 cosmosInt16) => this.GetValue().CompareTo(cosmosInt16.GetValue());

    public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteInt16Value(this.GetValue());

    public static CosmosInt16 Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosInt16) new CosmosInt16.LazyCosmosInt16(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosInt16 Create(short number) => (CosmosInt16) new CosmosInt16.EagerCosmosInt16(number);

    private sealed class EagerCosmosInt16 : CosmosInt16
    {
      private readonly short number;

      public EagerCosmosInt16(short number) => this.number = number;

      public override short GetValue() => this.number;
    }

    private sealed class LazyCosmosInt16 : CosmosInt16
    {
      private readonly Lazy<short> lazyNumber;

      public LazyCosmosInt16(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        if (jsonNavigator == null)
          throw new ArgumentNullException(nameof (jsonNavigator));
        JsonNodeType jsonNodeType = jsonNavigatorNode != null ? jsonNavigator.GetNodeType(jsonNavigatorNode) : throw new ArgumentNullException(nameof (jsonNavigatorNode));
        if (jsonNodeType != JsonNodeType.Int16)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be a {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.Int16, (object) jsonNodeType));
        this.lazyNumber = new Lazy<short>((Func<short>) (() => jsonNavigator.GetInt16Value(jsonNavigatorNode)));
      }

      public override short GetValue() => this.lazyNumber.Value;
    }
  }
}
