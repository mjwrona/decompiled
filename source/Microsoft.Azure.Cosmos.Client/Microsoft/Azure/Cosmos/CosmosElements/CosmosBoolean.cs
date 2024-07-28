// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.CosmosBoolean
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements
{
  internal sealed class CosmosBoolean : 
    CosmosElement,
    IEquatable<CosmosBoolean>,
    IComparable<CosmosBoolean>
  {
    private const int TrueHash = 1071096595;
    private const int FalseHash = 1031304189;
    private static readonly CosmosBoolean True = new CosmosBoolean(true);
    private static readonly CosmosBoolean False = new CosmosBoolean(false);

    private CosmosBoolean(bool value) => this.Value = value;

    public bool Value { get; }

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

    public override bool Equals(CosmosElement cosmosElement) => cosmosElement is CosmosBoolean cosmosBoolean && this.Equals(cosmosBoolean);

    public bool Equals(CosmosBoolean cosmosBoolean) => this.Value == cosmosBoolean.Value;

    public override int GetHashCode() => !this.Value ? 1031304189 : 1071096595;

    public int CompareTo(CosmosBoolean cosmosBoolean) => this.Value.CompareTo(cosmosBoolean.Value);

    public static CosmosBoolean Create(bool boolean) => !boolean ? CosmosBoolean.False : CosmosBoolean.True;

    public override void WriteTo(IJsonWriter jsonWriter) => jsonWriter.WriteBoolValue(this.Value);

    public static CosmosBoolean CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.CreateFromBuffer<CosmosBoolean>(buffer);

    public static CosmosBoolean Parse(string json) => CosmosElement.Parse<CosmosBoolean>(json);

    public static bool TryCreateFromBuffer(
      ReadOnlyMemory<byte> buffer,
      out CosmosBoolean cosmosBoolean)
    {
      return CosmosElement.TryCreateFromBuffer<CosmosBoolean>(buffer, out cosmosBoolean);
    }

    public static bool TryParse(string json, out CosmosBoolean cosmosBoolean) => CosmosElement.TryParse<CosmosBoolean>(json, out cosmosBoolean);

    public new static class Monadic
    {
      public static TryCatch<CosmosBoolean> CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.Monadic.CreateFromBuffer<CosmosBoolean>(buffer);

      public static TryCatch<CosmosBoolean> Parse(string json) => CosmosElement.Monadic.Parse<CosmosBoolean>(json);
    }
  }
}
