// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.UdtPropertyTypeHybridRowSerializer
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
  public readonly struct UdtPropertyTypeHybridRowSerializer : IHybridRowSerializer<UdtPropertyType>
  {
    public const int SchemaId = 2147473663;
    public const int Size = 5;
    private static readonly Utf8String NameName = Utf8String.TranscodeUtf16("name");
    private static readonly Utf8String SchemaIdName = Utf8String.TranscodeUtf16("id");
    private static readonly Utf8String __BaseName = Utf8String.TranscodeUtf16("__base");
    private static readonly LayoutColumn NameColumn;
    private static readonly LayoutColumn SchemaIdColumn;
    private static readonly LayoutColumn __BaseColumn;
    private static readonly StringToken __BaseToken;

    public IEqualityComparer<UdtPropertyType> Comparer => (IEqualityComparer<UdtPropertyType>) UdtPropertyTypeHybridRowSerializer.UdtPropertyTypeComparer.Default;

    static UdtPropertyTypeHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473663));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(UdtPropertyTypeHybridRowSerializer.NameName), out UdtPropertyTypeHybridRowSerializer.NameColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(UdtPropertyTypeHybridRowSerializer.SchemaIdName), out UdtPropertyTypeHybridRowSerializer.SchemaIdColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(UdtPropertyTypeHybridRowSerializer.__BaseName), out UdtPropertyTypeHybridRowSerializer.__BaseColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(UdtPropertyTypeHybridRowSerializer.__BaseColumn.Path), out UdtPropertyTypeHybridRowSerializer.__BaseToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      UdtPropertyType value)
    {
      if (isRoot)
        return UdtPropertyTypeHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473663), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = UdtPropertyTypeHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, UdtPropertyType value)
    {
      if (value.SchemaId != new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId())
      {
        Result result = LayoutType.Int32.WriteFixed(ref row, ref scope, UdtPropertyTypeHybridRowSerializer.SchemaIdColumn, (int) value.SchemaId);
        if (result != Result.Success)
          return result;
      }
      if (value.Name != null)
      {
        Result result = LayoutType.Utf8.WriteVariable(ref row, ref scope, UdtPropertyTypeHybridRowSerializer.NameColumn, value.Name);
        if (result != Result.Success)
          return result;
      }
      scope.Find(ref row, Utf8String.op_Implicit(UdtPropertyTypeHybridRowSerializer.__BaseColumn.Path));
      Result result1 = ScopePropertyTypeHybridRowSerializer.WriteBase(ref row, ref scope, (ScopePropertyType) value);
      return result1 != Result.Success ? result1 : Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out UdtPropertyType value)
    {
      if (isRoot)
      {
        value = new UdtPropertyType();
        return UdtPropertyTypeHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (UdtPropertyType) null;
        return result1;
      }
      value = new UdtPropertyType();
      Result result2 = UdtPropertyTypeHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (UdtPropertyType) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref UdtPropertyType value)
    {
      int num;
      Result result1 = LayoutType.Int32.ReadFixed(ref row, ref scope, UdtPropertyTypeHybridRowSerializer.SchemaIdColumn, out num);
      switch (result1)
      {
        case Result.Success:
          value.SchemaId = (Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId) num;
          goto case Result.NotFound;
        case Result.NotFound:
          string str;
          Result result2 = LayoutType.Utf8.ReadVariable(ref row, ref scope, UdtPropertyTypeHybridRowSerializer.NameColumn, out str);
          switch (result2)
          {
            case Result.Success:
              value.Name = str;
              goto case Result.NotFound;
            case Result.NotFound:
              while (scope.MoveNext(ref row))
              {
                if ((long) scope.Token == (long) UdtPropertyTypeHybridRowSerializer.__BaseToken.Id)
                {
                  ScopePropertyType scopePropertyType = (ScopePropertyType) value;
                  Result result3 = ScopePropertyTypeHybridRowSerializer.ReadBase(ref row, ref scope, ref scopePropertyType);
                  if (result3 != Result.Success)
                    return result3;
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

    public sealed class UdtPropertyTypeComparer : EqualityComparer<UdtPropertyType>
    {
      public static readonly UdtPropertyTypeHybridRowSerializer.UdtPropertyTypeComparer Default = new UdtPropertyTypeHybridRowSerializer.UdtPropertyTypeComparer();

      public override bool Equals(UdtPropertyType x, UdtPropertyType y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<UdtPropertyType>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        return ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer.EqualsBase((ScopePropertyType) x, (ScopePropertyType) y) && new Utf8HybridRowSerializer().Comparer.Equals(x.Name, y.Name) && new Int32HybridRowSerializer().Comparer.Equals((int) x.SchemaId, (int) y.SchemaId);
      }

      public override int GetHashCode(UdtPropertyType obj) => HashCode.Combine<int, int, int>(ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer.GetHashCodeBase((ScopePropertyType) obj), new Utf8HybridRowSerializer().Comparer.GetHashCode(obj.Name), new Int32HybridRowSerializer().Comparer.GetHashCode((int) obj.SchemaId));
    }
  }
}
