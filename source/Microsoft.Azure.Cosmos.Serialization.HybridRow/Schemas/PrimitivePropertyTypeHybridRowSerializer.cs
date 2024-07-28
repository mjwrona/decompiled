// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.PrimitivePropertyTypeHybridRowSerializer
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public readonly struct PrimitivePropertyTypeHybridRowSerializer : 
    IHybridRowSerializer<PrimitivePropertyType>
  {
    public const int SchemaId = 2147473659;
    public const int Size = 5;
    private static readonly Utf8String LengthName = Utf8String.TranscodeUtf16("length");
    private static readonly Utf8String StorageName = Utf8String.TranscodeUtf16("storage");
    private static readonly Utf8String EnumName = Utf8String.TranscodeUtf16("enum");
    private static readonly Utf8String RowBufferSizeName = Utf8String.TranscodeUtf16("rowBufferSize");
    private static readonly Utf8String __BaseName = Utf8String.TranscodeUtf16("__base");
    private static readonly LayoutColumn LengthColumn;
    private static readonly LayoutColumn StorageColumn;
    private static readonly LayoutColumn EnumColumn;
    private static readonly LayoutColumn RowBufferSizeColumn;
    private static readonly LayoutColumn __BaseColumn;
    private static readonly StringToken EnumToken;
    private static readonly StringToken RowBufferSizeToken;
    private static readonly StringToken __BaseToken;

    public IEqualityComparer<PrimitivePropertyType> Comparer => (IEqualityComparer<PrimitivePropertyType>) PrimitivePropertyTypeHybridRowSerializer.PrimitivePropertyTypeComparer.Default;

    static PrimitivePropertyTypeHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473659));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PrimitivePropertyTypeHybridRowSerializer.LengthName), out PrimitivePropertyTypeHybridRowSerializer.LengthColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PrimitivePropertyTypeHybridRowSerializer.StorageName), out PrimitivePropertyTypeHybridRowSerializer.StorageColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PrimitivePropertyTypeHybridRowSerializer.EnumName), out PrimitivePropertyTypeHybridRowSerializer.EnumColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PrimitivePropertyTypeHybridRowSerializer.RowBufferSizeName), out PrimitivePropertyTypeHybridRowSerializer.RowBufferSizeColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PrimitivePropertyTypeHybridRowSerializer.__BaseName), out PrimitivePropertyTypeHybridRowSerializer.__BaseColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(PrimitivePropertyTypeHybridRowSerializer.EnumColumn.Path), out PrimitivePropertyTypeHybridRowSerializer.EnumToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(PrimitivePropertyTypeHybridRowSerializer.RowBufferSizeColumn.Path), out PrimitivePropertyTypeHybridRowSerializer.RowBufferSizeToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(PrimitivePropertyTypeHybridRowSerializer.__BaseColumn.Path), out PrimitivePropertyTypeHybridRowSerializer.__BaseToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      PrimitivePropertyType value)
    {
      if (isRoot)
        return PrimitivePropertyTypeHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473659), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = PrimitivePropertyTypeHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      PrimitivePropertyType value)
    {
      if (value.Length != 0)
      {
        Result result = LayoutType.Int32.WriteFixed(ref row, ref scope, PrimitivePropertyTypeHybridRowSerializer.LengthColumn, value.Length);
        if (result != Result.Success)
          return result;
      }
      if (value.Storage != StorageKind.Sparse)
      {
        Result result = LayoutType.UInt8.WriteFixed(ref row, ref scope, PrimitivePropertyTypeHybridRowSerializer.StorageColumn, checked ((byte) (uint) value.Storage));
        if (result != Result.Success)
          return result;
      }
      if (!string.IsNullOrEmpty(value.Enum))
      {
        scope.Find(ref row, Utf8String.op_Implicit(PrimitivePropertyTypeHybridRowSerializer.EnumColumn.Path));
        Result result = LayoutType.Utf8.WriteSparse(ref row, ref scope, value.Enum, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.RowBufferSize)
      {
        scope.Find(ref row, Utf8String.op_Implicit(PrimitivePropertyTypeHybridRowSerializer.RowBufferSizeColumn.Path));
        Result result = LayoutType.Boolean.WriteSparse(ref row, ref scope, value.RowBufferSize, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      scope.Find(ref row, Utf8String.op_Implicit(PrimitivePropertyTypeHybridRowSerializer.__BaseColumn.Path));
      Result result1 = PropertyTypeHybridRowSerializer.WriteBase(ref row, ref scope, (PropertyType) value);
      return result1 != Result.Success ? result1 : Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out PrimitivePropertyType value)
    {
      if (isRoot)
      {
        value = new PrimitivePropertyType();
        return PrimitivePropertyTypeHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (PrimitivePropertyType) null;
        return result1;
      }
      value = new PrimitivePropertyType();
      Result result2 = PrimitivePropertyTypeHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (PrimitivePropertyType) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      ref PrimitivePropertyType value)
    {
      int num1;
      Result result1 = LayoutType.Int32.ReadFixed(ref row, ref scope, PrimitivePropertyTypeHybridRowSerializer.LengthColumn, out num1);
      switch (result1)
      {
        case Result.Success:
          value.Length = num1;
          goto case Result.NotFound;
        case Result.NotFound:
          byte num2;
          Result result2 = LayoutType.UInt8.ReadFixed(ref row, ref scope, PrimitivePropertyTypeHybridRowSerializer.StorageColumn, out num2);
          switch (result2)
          {
            case Result.Success:
              value.Storage = (StorageKind) num2;
              goto case Result.NotFound;
            case Result.NotFound:
              while (scope.MoveNext(ref row))
              {
                if ((long) scope.Token == (long) PrimitivePropertyTypeHybridRowSerializer.EnumToken.Id)
                {
                  string str;
                  Result result3 = LayoutType.Utf8.ReadSparse(ref row, ref scope, out str);
                  if (result3 != Result.Success)
                    return result3;
                  value.Enum = str;
                }
                else if ((long) scope.Token == (long) PrimitivePropertyTypeHybridRowSerializer.RowBufferSizeToken.Id)
                {
                  bool flag;
                  Result result4 = LayoutType.Boolean.ReadSparse(ref row, ref scope, out flag);
                  if (result4 != Result.Success)
                    return result4;
                  value.RowBufferSize = flag;
                }
                else if ((long) scope.Token == (long) PrimitivePropertyTypeHybridRowSerializer.__BaseToken.Id)
                {
                  PropertyType propertyType = (PropertyType) value;
                  Result result5 = PropertyTypeHybridRowSerializer.ReadBase(ref row, ref scope, ref propertyType);
                  if (result5 != Result.Success)
                    return result5;
                }
              }
              return Result.Success;
            default:
              return result2;
          }
        default:
          return result1;
      }
    }

    public sealed class PrimitivePropertyTypeComparer : EqualityComparer<PrimitivePropertyType>
    {
      public static readonly PrimitivePropertyTypeHybridRowSerializer.PrimitivePropertyTypeComparer Default = new PrimitivePropertyTypeHybridRowSerializer.PrimitivePropertyTypeComparer();

      public override bool Equals(PrimitivePropertyType x, PrimitivePropertyType y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<PrimitivePropertyType>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        return PropertyTypeHybridRowSerializer.PropertyTypeComparer.EqualsBase((PropertyType) x, (PropertyType) y) && new Int32HybridRowSerializer().Comparer.Equals(x.Length, y.Length) && new UInt8HybridRowSerializer().Comparer.Equals(checked ((byte) (uint) x.Storage), checked ((byte) (uint) y.Storage)) && new Utf8HybridRowSerializer().Comparer.Equals(x.Enum, y.Enum) && new BooleanHybridRowSerializer().Comparer.Equals(x.RowBufferSize, y.RowBufferSize);
      }

      public override int GetHashCode(PrimitivePropertyType obj) => HashCode.Combine<int, int, int, int, int>(PropertyTypeHybridRowSerializer.PropertyTypeComparer.GetHashCodeBase((PropertyType) obj), new Int32HybridRowSerializer().Comparer.GetHashCode(obj.Length), new UInt8HybridRowSerializer().Comparer.GetHashCode(checked ((byte) (uint) obj.Storage)), new Utf8HybridRowSerializer().Comparer.GetHashCode(obj.Enum), new BooleanHybridRowSerializer().Comparer.GetHashCode(obj.RowBufferSize));
    }
  }
}
