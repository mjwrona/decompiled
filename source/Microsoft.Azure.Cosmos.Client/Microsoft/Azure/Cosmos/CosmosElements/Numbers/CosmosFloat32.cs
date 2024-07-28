// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.Numbers.CosmosFloat32
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements.Numbers
{
  internal abstract class CosmosFloat32 : 
    CosmosNumber,
    IEquatable<CosmosFloat32>,
    IComparable<CosmosFloat32>
  {
    public override Number64 Value => (Number64) (double) this.GetValue();

    public abstract float GetValue();

    public override void Accept(ICosmosNumberVisitor cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TResult Accept<TResult>(ICosmosNumberVisitor<TResult> cosmosNumberVisitor) => cosmosNumberVisitor.Visit(this);

    public override TOutput Accept<TArg, TOutput>(
      ICosmosNumberVisitor<TArg, TOutput> cosmosNumberVisitor,
      TArg input)
    {
      return cosmosNumberVisitor.Visit(this, input);
    }

    public override bool Equals(CosmosNumber cosmosNumber) => cosmosNumber is CosmosFloat32 cosmosFloat32 && this.Equals(cosmosFloat32);

    public bool Equals(CosmosFloat32 cosmosFloat32) => (double) this.GetValue() == (double) cosmosFloat32.GetValue();

    public override int GetHashCode() => (int) MurmurHash3.Hash32<float>(this.GetValue(), 495253708U);

    public int CompareTo(CosmosFloat32 cosmosFloat32) => this.GetValue().CompareTo(cosmosFloat32.GetValue());

    public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteFloat32Value(this.GetValue());

    public static CosmosFloat32 Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosFloat32) new CosmosFloat32.LazyCosmosFloat32(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosFloat32 Create(float number) => (CosmosFloat32) new CosmosFloat32.EagerCosmosFloat32(number);

    private sealed class EagerCosmosFloat32 : CosmosFloat32
    {
      private readonly float number;

      public EagerCosmosFloat32(float number) => this.number = number;

      public override float GetValue() => this.number;
    }

    private sealed class LazyCosmosFloat32 : CosmosFloat32
    {
      private readonly Lazy<float> lazyNumber;

      public LazyCosmosFloat32(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        if (jsonNavigator == null)
          throw new ArgumentNullException(nameof (jsonNavigator));
        JsonNodeType jsonNodeType = jsonNavigatorNode != null ? jsonNavigator.GetNodeType(jsonNavigatorNode) : throw new ArgumentNullException(nameof (jsonNavigatorNode));
        if (jsonNodeType != JsonNodeType.Float32)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be a {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.Float32, (object) jsonNodeType));
        this.lazyNumber = new Lazy<float>((Func<float>) (() => jsonNavigator.GetFloat32Value(jsonNavigatorNode)));
      }

      public override float GetValue() => this.lazyNumber.Value;
    }
  }
}
