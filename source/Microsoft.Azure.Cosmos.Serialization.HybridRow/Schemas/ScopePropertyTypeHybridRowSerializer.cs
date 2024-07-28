// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.ScopePropertyTypeHybridRowSerializer
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public readonly struct ScopePropertyTypeHybridRowSerializer : 
    IHybridRowSerializer<ScopePropertyType>
  {
    public const int SchemaId = 2147473660;
    public const int Size = 1;
    private static readonly Utf8String ImmutableName = Utf8String.TranscodeUtf16("immutable");
    private static readonly Utf8String __BaseName = Utf8String.TranscodeUtf16("__base");
    private static readonly LayoutColumn ImmutableColumn;
    private static readonly LayoutColumn __BaseColumn;
    private static readonly StringToken __BaseToken;

    public IEqualityComparer<ScopePropertyType> Comparer => (IEqualityComparer<ScopePropertyType>) ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer.Default;

    static ScopePropertyTypeHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473660));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(ScopePropertyTypeHybridRowSerializer.ImmutableName), out ScopePropertyTypeHybridRowSerializer.ImmutableColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(ScopePropertyTypeHybridRowSerializer.__BaseName), out ScopePropertyTypeHybridRowSerializer.__BaseColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(ScopePropertyTypeHybridRowSerializer.__BaseColumn.Path), out ScopePropertyTypeHybridRowSerializer.__BaseToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      ScopePropertyType value)
    {
      switch (value)
      {
        case ArrayPropertyType arrayPropertyType:
          return new ArrayPropertyTypeHybridRowSerializer().Write(ref row, ref scope, isRoot, typeArgs, arrayPropertyType);
        case ObjectPropertyType objectPropertyType:
          return new ObjectPropertyTypeHybridRowSerializer().Write(ref row, ref scope, isRoot, typeArgs, objectPropertyType);
        case UdtPropertyType udtPropertyType:
          return new UdtPropertyTypeHybridRowSerializer().Write(ref row, ref scope, isRoot, typeArgs, udtPropertyType);
        case SetPropertyType setPropertyType:
          return new SetPropertyTypeHybridRowSerializer().Write(ref row, ref scope, isRoot, typeArgs, setPropertyType);
        case MapPropertyType mapPropertyType:
          return new MapPropertyTypeHybridRowSerializer().Write(ref row, ref scope, isRoot, typeArgs, mapPropertyType);
        case TuplePropertyType tuplePropertyType:
          return new TuplePropertyTypeHybridRowSerializer().Write(ref row, ref scope, isRoot, typeArgs, tuplePropertyType);
        case TaggedPropertyType taggedPropertyType:
          return new TaggedPropertyTypeHybridRowSerializer().Write(ref row, ref scope, isRoot, typeArgs, taggedPropertyType);
        default:
          Contract.Fail("Type is abstract.");
          return Result.Failure;
      }
    }

    public static Result WriteBase(ref RowBuffer row, ref RowCursor scope, ScopePropertyType value)
    {
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473660), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = ScopePropertyTypeHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, ScopePropertyType value)
    {
      if (value.Immutable)
      {
        Result result = LayoutType.Boolean.WriteFixed(ref row, ref scope, ScopePropertyTypeHybridRowSerializer.ImmutableColumn, value.Immutable);
        if (result != Result.Success)
          return result;
      }
      scope.Find(ref row, Utf8String.op_Implicit(ScopePropertyTypeHybridRowSerializer.__BaseColumn.Path));
      Result result1 = PropertyTypeHybridRowSerializer.WriteBase(ref row, ref scope, (PropertyType) value);
      return result1 != Result.Success ? result1 : Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out ScopePropertyType value)
    {
      if (!(scope.TypeArg.Type is LayoutUDT))
      {
        value = (ScopePropertyType) null;
        return Result.TypeMismatch;
      }
      switch (scope.TypeArg.TypeArgs.SchemaId.Id)
      {
        case 2147473661:
          ArrayPropertyType arrayPropertyType;
          int num1 = (int) new ArrayPropertyTypeHybridRowSerializer().Read(ref row, ref scope, false, out arrayPropertyType);
          value = (ScopePropertyType) arrayPropertyType;
          return (Result) num1;
        case 2147473662:
          ObjectPropertyType objectPropertyType;
          int num2 = (int) new ObjectPropertyTypeHybridRowSerializer().Read(ref row, ref scope, false, out objectPropertyType);
          value = (ScopePropertyType) objectPropertyType;
          return (Result) num2;
        case 2147473663:
          UdtPropertyType udtPropertyType;
          int num3 = (int) new UdtPropertyTypeHybridRowSerializer().Read(ref row, ref scope, false, out udtPropertyType);
          value = (ScopePropertyType) udtPropertyType;
          return (Result) num3;
        case 2147473664:
          SetPropertyType setPropertyType;
          int num4 = (int) new SetPropertyTypeHybridRowSerializer().Read(ref row, ref scope, false, out setPropertyType);
          value = (ScopePropertyType) setPropertyType;
          return (Result) num4;
        case 2147473665:
          MapPropertyType mapPropertyType;
          int num5 = (int) new MapPropertyTypeHybridRowSerializer().Read(ref row, ref scope, false, out mapPropertyType);
          value = (ScopePropertyType) mapPropertyType;
          return (Result) num5;
        case 2147473666:
          TuplePropertyType tuplePropertyType;
          int num6 = (int) new TuplePropertyTypeHybridRowSerializer().Read(ref row, ref scope, false, out tuplePropertyType);
          value = (ScopePropertyType) tuplePropertyType;
          return (Result) num6;
        case 2147473667:
          TaggedPropertyType taggedPropertyType;
          int num7 = (int) new TaggedPropertyTypeHybridRowSerializer().Read(ref row, ref scope, false, out taggedPropertyType);
          value = (ScopePropertyType) taggedPropertyType;
          return (Result) num7;
        default:
          Contract.Fail("Type is abstract.");
          value = (ScopePropertyType) null;
          return Result.Failure;
      }
    }

    public static Result ReadBase(
      ref RowBuffer row,
      ref RowCursor scope,
      ref ScopePropertyType value)
    {
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
        return result1;
      Result result2 = ScopePropertyTypeHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref ScopePropertyType value)
    {
      bool flag;
      Result result1 = LayoutType.Boolean.ReadFixed(ref row, ref scope, ScopePropertyTypeHybridRowSerializer.ImmutableColumn, out flag);
      switch (result1)
      {
        case Result.Success:
          value.Immutable = flag;
          goto case Result.NotFound;
        case Result.NotFound:
          while (scope.MoveNext(ref row))
          {
            if ((long) scope.Token == (long) ScopePropertyTypeHybridRowSerializer.__BaseToken.Id)
            {
              PropertyType propertyType = (PropertyType) value;
              Result result2 = PropertyTypeHybridRowSerializer.ReadBase(ref row, ref scope, ref propertyType);
              if (result2 != Result.Success)
                return result2;
            }
          }
          return Result.Success;
        default:
          return result1;
      }
    }

    public sealed class ScopePropertyTypeComparer : EqualityComparer<ScopePropertyType>
    {
      public static readonly ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer Default = new ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer();

      public override bool Equals(ScopePropertyType x, ScopePropertyType y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<ScopePropertyType>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        switch (x)
        {
          case ArrayPropertyType x1:
            return new ArrayPropertyTypeHybridRowSerializer().Comparer.Equals(x1, (ArrayPropertyType) y);
          case ObjectPropertyType x2:
            return new ObjectPropertyTypeHybridRowSerializer().Comparer.Equals(x2, (ObjectPropertyType) y);
          case UdtPropertyType x3:
            return new UdtPropertyTypeHybridRowSerializer().Comparer.Equals(x3, (UdtPropertyType) y);
          case SetPropertyType x4:
            return new SetPropertyTypeHybridRowSerializer().Comparer.Equals(x4, (SetPropertyType) y);
          case MapPropertyType x5:
            return new MapPropertyTypeHybridRowSerializer().Comparer.Equals(x5, (MapPropertyType) y);
          case TuplePropertyType x6:
            return new TuplePropertyTypeHybridRowSerializer().Comparer.Equals(x6, (TuplePropertyType) y);
          case TaggedPropertyType x7:
            return new TaggedPropertyTypeHybridRowSerializer().Comparer.Equals(x7, (TaggedPropertyType) y);
          default:
            Contract.Fail("Type is abstract.");
            return false;
        }
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      internal static bool EqualsBase(ScopePropertyType x, ScopePropertyType y) => PropertyTypeHybridRowSerializer.PropertyTypeComparer.EqualsBase((PropertyType) x, (PropertyType) y) && new BooleanHybridRowSerializer().Comparer.Equals(x.Immutable, y.Immutable);

      public override int GetHashCode(ScopePropertyType obj)
      {
        switch (obj)
        {
          case ArrayPropertyType arrayPropertyType:
            return new ArrayPropertyTypeHybridRowSerializer().Comparer.GetHashCode(arrayPropertyType);
          case ObjectPropertyType objectPropertyType:
            return new ObjectPropertyTypeHybridRowSerializer().Comparer.GetHashCode(objectPropertyType);
          case UdtPropertyType udtPropertyType:
            return new UdtPropertyTypeHybridRowSerializer().Comparer.GetHashCode(udtPropertyType);
          case SetPropertyType setPropertyType:
            return new SetPropertyTypeHybridRowSerializer().Comparer.GetHashCode(setPropertyType);
          case MapPropertyType mapPropertyType:
            return new MapPropertyTypeHybridRowSerializer().Comparer.GetHashCode(mapPropertyType);
          case TuplePropertyType tuplePropertyType:
            return new TuplePropertyTypeHybridRowSerializer().Comparer.GetHashCode(tuplePropertyType);
          case TaggedPropertyType taggedPropertyType:
            return new TaggedPropertyTypeHybridRowSerializer().Comparer.GetHashCode(taggedPropertyType);
          default:
            Contract.Fail("Type is abstract.");
            return 0;
        }
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      internal static int GetHashCodeBase(ScopePropertyType obj) => HashCode.Combine<int, int>(PropertyTypeHybridRowSerializer.PropertyTypeComparer.GetHashCodeBase((PropertyType) obj), new BooleanHybridRowSerializer().Comparer.GetHashCode(obj.Immutable));
    }
  }
}
