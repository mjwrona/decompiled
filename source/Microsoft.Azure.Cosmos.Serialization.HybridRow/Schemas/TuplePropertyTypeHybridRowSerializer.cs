// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.TuplePropertyTypeHybridRowSerializer
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
  public readonly struct TuplePropertyTypeHybridRowSerializer : 
    IHybridRowSerializer<TuplePropertyType>
  {
    public const int SchemaId = 2147473666;
    public const int Size = 0;
    private static readonly Utf8String ItemsName = Utf8String.TranscodeUtf16("items");
    private static readonly Utf8String __BaseName = Utf8String.TranscodeUtf16("__base");
    private static readonly LayoutColumn ItemsColumn;
    private static readonly LayoutColumn __BaseColumn;
    private static readonly StringToken ItemsToken;
    private static readonly StringToken __BaseToken;

    public IEqualityComparer<TuplePropertyType> Comparer => (IEqualityComparer<TuplePropertyType>) TuplePropertyTypeHybridRowSerializer.TuplePropertyTypeComparer.Default;

    static TuplePropertyTypeHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473666));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(TuplePropertyTypeHybridRowSerializer.ItemsName), out TuplePropertyTypeHybridRowSerializer.ItemsColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(TuplePropertyTypeHybridRowSerializer.__BaseName), out TuplePropertyTypeHybridRowSerializer.__BaseColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(TuplePropertyTypeHybridRowSerializer.ItemsColumn.Path), out TuplePropertyTypeHybridRowSerializer.ItemsToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(TuplePropertyTypeHybridRowSerializer.__BaseColumn.Path), out TuplePropertyTypeHybridRowSerializer.__BaseToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      TuplePropertyType value)
    {
      if (isRoot)
        return TuplePropertyTypeHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473666), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = TuplePropertyTypeHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, TuplePropertyType value)
    {
      if (value.Items != null && value.Items.Count > 0)
      {
        scope.Find(ref row, Utf8String.op_Implicit(TuplePropertyTypeHybridRowSerializer.ItemsColumn.Path));
        Result result = new ArrayHybridRowSerializer<PropertyType, PropertyTypeHybridRowSerializer>().Write(ref row, ref scope, false, TuplePropertyTypeHybridRowSerializer.ItemsColumn.TypeArgs, value.Items);
        if (result != Result.Success)
          return result;
      }
      scope.Find(ref row, Utf8String.op_Implicit(TuplePropertyTypeHybridRowSerializer.__BaseColumn.Path));
      Result result1 = ScopePropertyTypeHybridRowSerializer.WriteBase(ref row, ref scope, (ScopePropertyType) value);
      return result1 != Result.Success ? result1 : Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out TuplePropertyType value)
    {
      if (isRoot)
      {
        value = new TuplePropertyType();
        return TuplePropertyTypeHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (TuplePropertyType) null;
        return result1;
      }
      value = new TuplePropertyType();
      Result result2 = TuplePropertyTypeHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (TuplePropertyType) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref TuplePropertyType value)
    {
      while (scope.MoveNext(ref row))
      {
        if ((long) scope.Token == (long) TuplePropertyTypeHybridRowSerializer.ItemsToken.Id)
        {
          List<PropertyType> propertyTypeList;
          Result result = new ArrayHybridRowSerializer<PropertyType, PropertyTypeHybridRowSerializer>().Read(ref row, ref scope, false, out propertyTypeList);
          if (result != Result.Success)
            return result;
          value.Items = propertyTypeList;
        }
        else if ((long) scope.Token == (long) TuplePropertyTypeHybridRowSerializer.__BaseToken.Id)
        {
          ScopePropertyType scopePropertyType = (ScopePropertyType) value;
          Result result = ScopePropertyTypeHybridRowSerializer.ReadBase(ref row, ref scope, ref scopePropertyType);
          if (result != Result.Success)
            return result;
        }
      }
      return Result.Success;
    }

    public sealed class TuplePropertyTypeComparer : EqualityComparer<TuplePropertyType>
    {
      public static readonly TuplePropertyTypeHybridRowSerializer.TuplePropertyTypeComparer Default = new TuplePropertyTypeHybridRowSerializer.TuplePropertyTypeComparer();

      public override bool Equals(TuplePropertyType x, TuplePropertyType y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<TuplePropertyType>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        return ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer.EqualsBase((ScopePropertyType) x, (ScopePropertyType) y) && new TypedArrayHybridRowSerializer<PropertyType, PropertyTypeHybridRowSerializer>().Comparer.Equals(x.Items, y.Items);
      }

      public override int GetHashCode(TuplePropertyType obj) => HashCode.Combine<int, int>(ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer.GetHashCodeBase((ScopePropertyType) obj), new TypedArrayHybridRowSerializer<PropertyType, PropertyTypeHybridRowSerializer>().Comparer.GetHashCode(obj.Items));
    }
  }
}
