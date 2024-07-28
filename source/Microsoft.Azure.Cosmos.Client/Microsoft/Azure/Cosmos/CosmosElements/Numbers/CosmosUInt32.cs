// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.Numbers.CosmosUInt32
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements.Numbers
{
  internal abstract class CosmosUInt32 : 
    CosmosNumber,
    IEquatable<CosmosUInt32>,
    IComparable<CosmosUInt32>
  {
    public override Number64 Value => (Number64) (long) this.GetValue();

    public abstract uint GetValue();

    public override void Accept(ICosmosNumberVisitor cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TResult Accept<TResult>(ICosmosNumberVisitor<TResult> cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TOutput Accept<TArg, TOutput>(
      ICosmosNumberVisitor<TArg, TOutput> cosmosNumberVisitor,
      TArg input)
    {
      return cosmosNumberVisitor.Visit(this, input);
    }

    public override bool Equals(CosmosNumber cosmosNumber) => cosmosNumber is CosmosUInt32 cosmosUInt32 && this.Equals(cosmosUInt32);

    public bool Equals(CosmosUInt32 cosmosUInt32) => (int) this.GetValue() == (int) cosmosUInt32.GetValue();

    public override int GetHashCode() => (int) MurmurHash3.Hash32<uint>(this.GetValue(), 3771427877U);

    public int CompareTo(CosmosUInt32 cosmosUInt32) => this.GetValue().CompareTo(cosmosUInt32.GetValue());

    public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteUInt32Value(this.GetValue());

    public static CosmosUInt32 Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosUInt32) new CosmosUInt32.LazyCosmosUInt32(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosUInt32 Create(uint number) => (CosmosUInt32) new CosmosUInt32.EagerCosmosUInt32(number);

    private sealed class EagerCosmosUInt32 : CosmosUInt32
    {
      private readonly uint number;

      public EagerCosmosUInt32(uint number) => this.number = number;

      public override uint GetValue() => this.number;
    }

    private sealed class LazyCosmosUInt32 : CosmosUInt32
    {
      private readonly Lazy<uint> lazyNumber;

      public LazyCosmosUInt32(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        if (jsonNavigator == null)
          throw new ArgumentNullException(nameof (jsonNavigator));
        JsonNodeType jsonNodeType = jsonNavigatorNode != null ? jsonNavigator.GetNodeType(jsonNavigatorNode) : throw new ArgumentNullException(nameof (jsonNavigatorNode));
        if (jsonNodeType != JsonNodeType.UInt32)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be a {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.UInt32, (object) jsonNodeType));
        this.lazyNumber = new Lazy<uint>((Func<uint>) (() => jsonNavigator.GetUInt32Value(jsonNavigatorNode)));
      }

      public override uint GetValue() => this.lazyNumber.Value;
    }
  }
}
