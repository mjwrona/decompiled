// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.CosmosBinary
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements
{
  internal abstract class CosmosBinary : 
    CosmosElement,
    IEquatable<CosmosBinary>,
    IComparable<CosmosBinary>
  {
    private const uint HashSeed = 1577818695;

    public abstract ReadOnlyMemory<byte> Value { get; }

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

    public override bool Equals(CosmosElement cosmosElement) => cosmosElement is CosmosBinary cosmosBinary && this.Equals(cosmosBinary);

    public bool Equals(CosmosBinary cosmosBinary)
    {
      ReadOnlyMemory<byte> readOnlyMemory = this.Value;
      ReadOnlySpan<byte> span1 = readOnlyMemory.Span;
      readOnlyMemory = cosmosBinary.Value;
      ReadOnlySpan<byte> span2 = readOnlyMemory.Span;
      return span1.SequenceEqual<byte>(span2);
    }

    public override int GetHashCode() => (int) MurmurHash3.Hash32(this.Value.Span, 1577818695U);

    public int CompareTo(CosmosBinary cosmosBinary)
    {
      ReadOnlyMemory<byte> readOnlyMemory = this.Value;
      ReadOnlySpan<byte> span1 = readOnlyMemory.Span;
      readOnlyMemory = cosmosBinary.Value;
      ReadOnlySpan<byte> span2 = readOnlyMemory.Span;
      return span1.SequenceCompareTo<byte>(span2);
    }

    public static CosmosBinary Create(
      IJsonNavigator jsonNavigator,
      IJsonNavigatorNode jsonNavigatorNode)
    {
      return (CosmosBinary) new CosmosBinary.LazyCosmosBinary(jsonNavigator, jsonNavigatorNode);
    }

    public static CosmosBinary Create(ReadOnlyMemory<byte> value) => (CosmosBinary) new CosmosBinary.EagerCosmosBinary(value);

    public static CosmosBinary CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.CreateFromBuffer<CosmosBinary>(buffer);

    public static CosmosBinary Parse(string json) => CosmosElement.Parse<CosmosBinary>(json);

    public static bool TryCreateFromBuffer(
      ReadOnlyMemory<byte> buffer,
      out CosmosBinary cosmosBinary)
    {
      return CosmosElement.TryCreateFromBuffer<CosmosBinary>(buffer, out cosmosBinary);
    }

    public static bool TryParse(string json, out CosmosBinary cosmosBinary) => CosmosElement.TryParse<CosmosBinary>(json, out cosmosBinary);

    public new static class Monadic
    {
      public static TryCatch<CosmosBinary> CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.Monadic.CreateFromBuffer<CosmosBinary>(buffer);

      public static TryCatch<CosmosBinary> Parse(string json) => CosmosElement.Monadic.Parse<CosmosBinary>(json);
    }

    private sealed class EagerCosmosBinary : CosmosBinary
    {
      public EagerCosmosBinary(ReadOnlyMemory<byte> value) => this.Value = value;

      public override ReadOnlyMemory<byte> Value { get; }

      public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteBinaryValue(this.Value.Span);
    }

    private sealed class LazyCosmosBinary : CosmosBinary
    {
      private readonly IJsonNavigator jsonNavigator;
      private readonly IJsonNavigatorNode jsonNavigatorNode;
      private readonly Lazy<ReadOnlyMemory<byte>> lazyBytes;

      public LazyCosmosBinary(IJsonNavigator jsonNavigator, IJsonNavigatorNode jsonNavigatorNode)
      {
        JsonNodeType nodeType = jsonNavigator.GetNodeType(jsonNavigatorNode);
        if (nodeType != JsonNodeType.Binary)
          throw new ArgumentOutOfRangeException(string.Format("{0} must be a {1} node. Got {2} instead.", (object) nameof (jsonNavigatorNode), (object) JsonNodeType.Binary, (object) nodeType));
        this.jsonNavigator = jsonNavigator;
        this.jsonNavigatorNode = jsonNavigatorNode;
        this.lazyBytes = new Lazy<ReadOnlyMemory<byte>>((Func<ReadOnlyMemory<byte>>) (() =>
        {
          ReadOnlyMemory<byte> bufferedBinaryValue;
          if (!this.jsonNavigator.TryGetBufferedBinaryValue(this.jsonNavigatorNode, out bufferedBinaryValue))
            bufferedBinaryValue = this.jsonNavigator.GetBinaryValue(this.jsonNavigatorNode);
          return bufferedBinaryValue;
        }));
      }

      public override ReadOnlyMemory<byte> Value => this.lazyBytes.Value;

      public override void WriteTo(IJsonWriter jsonWriter) => this.jsonNavigator.WriteNode(this.jsonNavigatorNode, jsonWriter);
    }
  }
}
