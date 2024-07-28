// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.CosmosGuid
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements
{
  internal abstract class CosmosGuid : CosmosElement, IEquatable<CosmosGuid>, IComparable<CosmosGuid>
  {
    private const uint HashSeed = 527095639;

    public abstract Guid Value { get; }

    public override void Accept(ICosmosElementVisitor cosmosElementVisitor) => cosmosElementVisitor.Visit(this);

    public override TResult Accept<TResult>(
      ICosmosElementVisitor<TResult> cosmosElementVisitor)
    {
      return cosmosElementVisitor.Visit(this);
    }

    public override TResult Accept<TArg, TResult>(
      ICosmosElementVisitor<TArg, TResult> cosmosElementVisitor,
      TArg input)
    {
      return cosmosElementVisitor.Visit(this, input);
    }

    public override bool Equals(CosmosElement cosmosElement) => cosmosElement is CosmosGuid cosmosGuid && this.Equals(cosmosGuid);

    public bool Equals(CosmosGuid cosmosGuid) => this.Value == cosmosGuid.Value;

    public override int GetHashCode() => (int) MurmurHash3.Hash32<Guid>(this.Value, 527095639U);

    public int CompareTo(CosmosGuid cosmosGuid) => this.Value.CompareTo(cosmosGuid.Value);

    public static CosmosGuid Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosGuid) new CosmosGuid.LazyCosmosGuid(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosGuid Create(Guid value) => (CosmosGuid) new CosmosGuid.EagerCosmosGuid(value);

    public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteGuidValue(this.Value);

    public static CosmosGuid CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.CreateFromBuffer<CosmosGuid>(buffer);

    public static CosmosGuid Parse(string json) => CosmosElement.Parse<CosmosGuid>(json);

    public static bool TryCreateFromBuffer(ReadOnlyMemory<byte> buffer, out CosmosGuid cosmosGuid) => CosmosElement.TryCreateFromBuffer<CosmosGuid>(buffer, out cosmosGuid);

    public static bool TryParse(string json, out CosmosGuid cosmosGuid) => CosmosElement.TryParse<CosmosGuid>(json, out cosmosGuid);

    public new static class Monadic
    {
      public static TryCatch<CosmosGuid> CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.Monadic.CreateFromBuffer<CosmosGuid>(buffer);

      public static TryCatch<CosmosGuid> Parse(string json) => CosmosElement.Monadic.Parse<CosmosGuid>(json);
    }

    private sealed class EagerCosmosGuid : CosmosGuid
    {
      public EagerCosmosGuid(Guid value) => this.Value = value;

      public override Guid Value { get; }
    }

    private sealed class LazyCosmosGuid : CosmosGuid
    {
      private readonly Lazy<Guid> lazyGuid;

      public LazyCosmosGuid(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        JsonNodeType nodeType = jsonNavigator.GetNodeType(jsonNavigatorNode);
        if (nodeType != JsonNodeType.Guid)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be a {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.Guid, (object) nodeType));
        this.lazyGuid = new Lazy<Guid>((Func<Guid>) (() => jsonNavigator.GetGuidValue(jsonNavigatorNode)));
      }

      public override Guid Value => this.lazyGuid.Value;
    }
  }
}
