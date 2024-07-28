// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.ArrayPropertyTypeHybridRowSerializer
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
  public readonly struct ArrayPropertyTypeHybridRowSerializer : 
    IHybridRowSerializer<ArrayPropertyType>
  {
    public const int SchemaId = 2147473661;
    public const int Size = 0;
    private static readonly Utf8String ItemsName = Utf8String.TranscodeUtf16("items");
    private static readonly Utf8String __BaseName = Utf8String.TranscodeUtf16("__base");
    private static readonly LayoutColumn ItemsColumn;
    private static readonly LayoutColumn __BaseColumn;
    private static readonly StringToken ItemsToken;
    private static readonly StringToken __BaseToken;

    public IEqualityComparer<ArrayPropertyType> Comparer => (IEqualityComparer<ArrayPropertyType>) ArrayPropertyTypeHybridRowSerializer.ArrayPropertyTypeComparer.Default;

    static ArrayPropertyTypeHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473661));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(ArrayPropertyTypeHybridRowSerializer.ItemsName), out ArrayPropertyTypeHybridRowSerializer.ItemsColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(ArrayPropertyTypeHybridRowSerializer.__BaseName), out ArrayPropertyTypeHybridRowSerializer.__BaseColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(ArrayPropertyTypeHybridRowSerializer.ItemsColumn.Path), out ArrayPropertyTypeHybridRowSerializer.ItemsToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(ArrayPropertyTypeHybridRowSerializer.__BaseColumn.Path), out ArrayPropertyTypeHybridRowSerializer.__BaseToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      ArrayPropertyType value)
    {
      if (isRoot)
        return ArrayPropertyTypeHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473661), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = ArrayPropertyTypeHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, ArrayPropertyType value)
    {
      if (value.Items != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(ArrayPropertyTypeHybridRowSerializer.ItemsColumn.Path));
        Result result = new PropertyTypeHybridRowSerializer().Write(ref row, ref scope, false, ArrayPropertyTypeHybridRowSerializer.ItemsColumn.TypeArgs, value.Items);
        if (result != Result.Success)
          return result;
      }
      scope.Find(ref row, Utf8String.op_Implicit(ArrayPropertyTypeHybridRowSerializer.__BaseColumn.Path));
      Result result1 = ScopePropertyTypeHybridRowSerializer.WriteBase(ref row, ref scope, (ScopePropertyType) value);
      return result1 != Result.Success ? result1 : Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out ArrayPropertyType value)
    {
      if (isRoot)
      {
        value = new ArrayPropertyType();
        return ArrayPropertyTypeHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (ArrayPropertyType) null;
        return result1;
      }
      value = new ArrayPropertyType();
      Result result2 = ArrayPropertyTypeHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (ArrayPropertyType) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref ArrayPropertyType value)
    {
      while (scope.MoveNext(ref row))
      {
        if ((long) scope.Token == (long) ArrayPropertyTypeHybridRowSerializer.ItemsToken.Id)
        {
          PropertyType propertyType;
          Result result = new PropertyTypeHybridRowSerializer().Read(ref row, ref scope, false, out propertyType);
          if (result != Result.Success)
            return result;
          value.Items = propertyType;
        }
        else if ((long) scope.Token == (long) ArrayPropertyTypeHybridRowSerializer.__BaseToken.Id)
        {
          ScopePropertyType scopePropertyType = (ScopePropertyType) value;
          Result result = ScopePropertyTypeHybridRowSerializer.ReadBase(ref row, ref scope, ref scopePropertyType);
          if (result != Result.Success)
            return result;
        }
      }
      return Result.Success;
    }

    public sealed class ArrayPropertyTypeComparer : EqualityComparer<ArrayPropertyType>
    {
      public static readonly ArrayPropertyTypeHybridRowSerializer.ArrayPropertyTypeComparer Default = new ArrayPropertyTypeHybridRowSerializer.ArrayPropertyTypeComparer();

      public override bool Equals(ArrayPropertyType x, ArrayPropertyType y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<ArrayPropertyType>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        return ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer.EqualsBase((ScopePropertyType) x, (ScopePropertyType) y) && new PropertyTypeHybridRowSerializer().Comparer.Equals(x.Items, y.Items);
      }

      public override int GetHashCode(ArrayPropertyType obj) => HashCode.Combine<int, int>(ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer.GetHashCodeBase((ScopePropertyType) obj), new PropertyTypeHybridRowSerializer().Comparer.GetHashCode(obj.Items));
    }
  }
}
