// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.CosmosNull
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements
{
  internal sealed class CosmosNull : CosmosElement, IEquatable<CosmosNull>, IComparable<CosmosNull>
  {
    private const uint Hash = 448207988;
    private static readonly CosmosNull Singleton = new CosmosNull();

    private CosmosNull()
    {
    }

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

    public override bool Equals(CosmosElement cosmosElement) => cosmosElement is CosmosNull cosmosNull && this.Equals(cosmosNull);

    public bool Equals(CosmosNull cosmosNull) => true;

    public static CosmosNull Create() => CosmosNull.Singleton;

    public override int GetHashCode() => 448207988;

    public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteNullValue();

    public static CosmosNull CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.CreateFromBuffer<CosmosNull>(buffer);

    public static CosmosNull Parse(string json) => CosmosElement.Parse<CosmosNull>(json);

    public static bool TryCreateFromBuffer(ReadOnlyMemory<byte> buffer, out CosmosNull cosmosNull) => CosmosElement.TryCreateFromBuffer<CosmosNull>(buffer, out cosmosNull);

    public static bool TryParse(string json, out CosmosNull cosmosNull) => CosmosElement.TryParse<CosmosNull>(json, out cosmosNull);

    public int CompareTo(CosmosNull other) => 0;

    public new static class Monadic
    {
      public static TryCatch<CosmosNull> CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.Monadic.CreateFromBuffer<CosmosNull>(buffer);

      public static TryCatch<CosmosNull> Parse(string json) => CosmosElement.Monadic.Parse<CosmosNull>(json);
    }
  }
}
