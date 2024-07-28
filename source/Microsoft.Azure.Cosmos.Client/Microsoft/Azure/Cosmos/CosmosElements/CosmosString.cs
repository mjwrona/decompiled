// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.CosmosString
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements
{
  internal abstract class CosmosString : 
    CosmosElement,
    IEquatable<CosmosString>,
    IComparable<CosmosString>
  {
    public static CosmosString Empty = (CosmosString) new CosmosString.EagerCosmosString(UtfAnyString.op_Implicit(string.Empty));
    private const uint HashSeed = 3163568842;

    public abstract UtfAnyString Value { get; }

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

    public override bool Equals(CosmosElement cosmosElement) => cosmosElement is CosmosString cosmosString && this.Equals(cosmosString);

    public bool Equals(CosmosString cosmosString) => UtfAnyString.op_Equality(this.Value, cosmosString.Value);

    public override int GetHashCode()
    {
      uint seed = 3163568842;
      return (int) MurmurHash3.Hash32(UtfAnyString.op_Implicit(this.Value), seed);
    }

    public int CompareTo(CosmosString cosmosString) => string.CompareOrdinal(UtfAnyString.op_Implicit(this.Value), UtfAnyString.op_Implicit(cosmosString.Value));

    public static CosmosString Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosString) new CosmosString.LazyCosmosString(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosString Create(string value)
    {
      switch (value)
      {
        case null:
          throw new ArgumentNullException(nameof (value));
        case "":
          return CosmosString.Empty;
        default:
          return (CosmosString) new CosmosString.EagerCosmosString(UtfAnyString.op_Implicit(value));
      }
    }

    public static CosmosString CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.CreateFromBuffer<CosmosString>(buffer);

    public static CosmosString Parse(string json) => CosmosElement.Parse<CosmosString>(json);

    public static bool TryCreateFromBuffer(
      ReadOnlyMemory<byte> buffer,
      out CosmosString cosmosString)
    {
      return CosmosElement.TryCreateFromBuffer<CosmosString>(buffer, out cosmosString);
    }

    public static bool TryParse(string json, out CosmosString cosmosString) => CosmosElement.TryParse<CosmosString>(json, out cosmosString);

    public new static class Monadic
    {
      public static TryCatch<CosmosString> CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.Monadic.CreateFromBuffer<CosmosString>(buffer);

      public static TryCatch<CosmosString> Parse(string json) => CosmosElement.Monadic.Parse<CosmosString>(json);
    }

    private sealed class EagerCosmosString : CosmosString
    {
      public EagerCosmosString(UtfAnyString value) => this.Value = value;

      public override UtfAnyString Value { get; }

      public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteStringValue(UtfAnyString.op_Implicit(this.Value));
    }

    private sealed class LazyCosmosString : CosmosString
    {
      private readonly IJsonNavigator jsonNavigator;
      private readonly IJsonNavigatorNode jsonNavigatorNode;
      private readonly Lazy<UtfAnyString> lazyString;

      public LazyCosmosString(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        JsonNodeType nodeType = jsonNavigator.GetNodeType(jsonNavigatorNode);
        if (nodeType != JsonNodeType.String)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be a {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.String, (object) nodeType));
        this.jsonNavigator = jsonNavigator;
        this.jsonNavigatorNode = jsonNavigatorNode;
        this.lazyString = new Lazy<UtfAnyString>((Func<UtfAnyString>) (() => this.jsonNavigator.GetStringValue(this.jsonNavigatorNode)));
      }

      public override UtfAnyString Value => this.lazyString.Value;

      public override void WriteTo(IJsonWriter jsonWriter) => this.jsonNavigator.WriteNode(this.jsonNavigatorNode, jsonWriter);
    }
  }
}
