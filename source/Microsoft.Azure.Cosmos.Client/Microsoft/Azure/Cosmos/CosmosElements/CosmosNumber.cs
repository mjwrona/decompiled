// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosElements.CosmosNumber
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements.Numbers;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;


#nullable enable
namespace Microsoft.Azure.Cosmos.CosmosElements
{
  internal abstract class CosmosNumber : 
    CosmosElement,
    IEquatable<CosmosNumber>,
    IComparable<CosmosNumber>
  {
    public abstract Number64 Value { get; }

    public abstract void Accept(ICosmosNumberVisitor cosmosNumberVisitor);

    public abstract TResult Accept<TResult>(ICosmosNumberVisitor<TResult> cosmosNumberVisitor);

    public abstract TOutput Accept<TArg, TOutput>(
      ICosmosNumberVisitor<TArg, TOutput> cosmosNumberVisitor,
      TArg input);

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

    public override bool Equals(CosmosElement cosmosElement) => cosmosElement is CosmosNumber cosmosNumber && this.Equals(cosmosNumber);

    public abstract bool Equals(CosmosNumber cosmosNumber);

    public int CompareTo(CosmosNumber other)
    {
      int num1 = this.Accept<int>((ICosmosNumberVisitor<int>) CosmosNumber.CosmosNumberToTypeOrder.Singleton);
      int num2 = other.Accept<int>((ICosmosNumberVisitor<int>) CosmosNumber.CosmosNumberToTypeOrder.Singleton);
      return num1 != num2 ? num1.CompareTo(num2) : this.Accept<CosmosNumber, int>((ICosmosNumberVisitor<CosmosNumber, int>) CosmosNumber.CosmosNumberWithinTypeComparer.Singleton, other);
    }

    public static CosmosNumber CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.CreateFromBuffer<CosmosNumber>(buffer);

    public static CosmosNumber Parse(string json) => CosmosElement.Parse<CosmosNumber>(json);

    public static bool TryCreateFromBuffer(
      ReadOnlyMemory<byte> buffer,
      out CosmosNumber cosmosNumber)
    {
      return CosmosElement.TryCreateFromBuffer<CosmosNumber>(buffer, out cosmosNumber);
    }

    public static bool TryParse(string json, out CosmosNumber cosmosNumber) => CosmosElement.TryParse<CosmosNumber>(json, out cosmosNumber);

    public new static class Monadic
    {
      public static TryCatch<CosmosNumber> CreateFromBuffer(ReadOnlyMemory<byte> buffer) => CosmosElement.Monadic.CreateFromBuffer<CosmosNumber>(buffer);

      public static TryCatch<CosmosNumber> Parse(string json) => CosmosElement.Monadic.Parse<CosmosNumber>(json);
    }

    private sealed class CosmosNumberToTypeOrder : ICosmosNumberVisitor<int>
    {
      public static readonly CosmosNumber.CosmosNumberToTypeOrder Singleton = new CosmosNumber.CosmosNumberToTypeOrder();

      private CosmosNumberToTypeOrder()
      {
      }

      public int Visit(CosmosNumber64 cosmosNumber64) => 0;

      public int Visit(CosmosInt8 cosmosInt8) => 1;

      public int Visit(CosmosInt16 cosmosInt16) => 2;

      public int Visit(CosmosInt32 cosmosInt32) => 3;

      public int Visit(CosmosInt64 cosmosInt64) => 4;

      public int Visit(CosmosUInt32 cosmosUInt32) => 5;

      public int Visit(CosmosFloat32 cosmosFloat32) => 6;

      public int Visit(CosmosFloat64 cosmosFloat64) => 7;
    }

    private sealed class CosmosNumberWithinTypeComparer : ICosmosNumberVisitor<CosmosNumber, int>
    {
      public static readonly CosmosNumber.CosmosNumberWithinTypeComparer Singleton = new CosmosNumber.CosmosNumberWithinTypeComparer();

      private CosmosNumberWithinTypeComparer()
      {
      }

      public int Visit(CosmosNumber64 cosmosNumber64, CosmosNumber input) => cosmosNumber64.CompareTo((CosmosNumber64) input);

      public int Visit(CosmosInt8 cosmosInt8, CosmosNumber input) => cosmosInt8.CompareTo((CosmosInt8) input);

      public int Visit(CosmosInt16 cosmosInt16, CosmosNumber input) => cosmosInt16.CompareTo((CosmosInt16) input);

      public int Visit(CosmosInt32 cosmosInt32, CosmosNumber input) => cosmosInt32.CompareTo((CosmosInt32) input);

      public int Visit(CosmosInt64 cosmosInt64, CosmosNumber input) => cosmosInt64.CompareTo((CosmosInt64) input);

      public int Visit(CosmosUInt32 cosmosUInt32, CosmosNumber input) => cosmosUInt32.CompareTo((CosmosUInt32) input);

      public int Visit(CosmosFloat32 cosmosFloat32, CosmosNumber input) => cosmosFloat32.CompareTo((CosmosFloat32) input);

      public int Visit(CosmosFloat64 cosmosFloat64, CosmosNumber input) => cosmosFloat64.CompareTo((CosmosFloat64) input);
    }
  }
}
