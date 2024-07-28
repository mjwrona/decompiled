// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.PropertyTypeHybridRowSerializer
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
  public readonly struct PropertyTypeHybridRowSerializer : IHybridRowSerializer<PropertyType>
  {
    public const int SchemaId = 2147473658;
    public const int Size = 2;
    private static readonly Utf8String ApiTypeName = Utf8String.TranscodeUtf16("apitype");
    private static readonly Utf8String TypeName = Utf8String.TranscodeUtf16("type");
    private static readonly Utf8String NullableName = Utf8String.TranscodeUtf16("nullable");
    private static readonly LayoutColumn ApiTypeColumn;
    private static readonly LayoutColumn TypeColumn;
    private static readonly LayoutColumn NullableColumn;

    public IEqualityComparer<PropertyType> Comparer => (IEqualityComparer<PropertyType>) PropertyTypeHybridRowSerializer.PropertyTypeComparer.Default;

    static PropertyTypeHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473658));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PropertyTypeHybridRowSerializer.ApiTypeName), out PropertyTypeHybridRowSerializer.ApiTypeColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PropertyTypeHybridRowSerializer.TypeName), out PropertyTypeHybridRowSerializer.TypeColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PropertyTypeHybridRowSerializer.NullableName), out PropertyTypeHybridRowSerializer.NullableColumn));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      PropertyType value)
    {
      switch (value)
      {
        case PrimitivePropertyType primitivePropertyType:
          return new PrimitivePropertyTypeHybridRowSerializer().Write(ref row, ref scope, isRoot, typeArgs, primitivePropertyType);
        case ScopePropertyType scopePropertyType:
          return new ScopePropertyTypeHybridRowSerializer().Write(ref row, ref scope, isRoot, typeArgs, scopePropertyType);
        default:
          Contract.Fail("Type is abstract.");
          return Result.Failure;
      }
    }

    public static Result WriteBase(ref RowBuffer row, ref RowCursor scope, PropertyType value)
    {
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473658), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = PropertyTypeHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, PropertyType value)
    {
      if (value.Type != TypeKind.Invalid)
      {
        Result result = LayoutType.UInt8.WriteFixed(ref row, ref scope, PropertyTypeHybridRowSerializer.TypeColumn, (byte) value.Type);
        if (result != Result.Success)
          return result;
      }
      if (value.Nullable)
      {
        Result result = LayoutType.Boolean.WriteFixed(ref row, ref scope, PropertyTypeHybridRowSerializer.NullableColumn, value.Nullable);
        if (result != Result.Success)
          return result;
      }
      if (value.ApiType != null)
      {
        Result result = LayoutType.Utf8.WriteVariable(ref row, ref scope, PropertyTypeHybridRowSerializer.ApiTypeColumn, value.ApiType);
        if (result != Result.Success)
          return result;
      }
      return Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out PropertyType value)
    {
      if (!(scope.TypeArg.Type is LayoutUDT))
      {
        value = (PropertyType) null;
        return Result.TypeMismatch;
      }
      switch (scope.TypeArg.TypeArgs.SchemaId.Id)
      {
        case 2147473659:
          PrimitivePropertyType primitivePropertyType;
          int num1 = (int) new PrimitivePropertyTypeHybridRowSerializer().Read(ref row, ref scope, false, out primitivePropertyType);
          value = (PropertyType) primitivePropertyType;
          return (Result) num1;
        case 2147473660:
        case 2147473661:
        case 2147473662:
        case 2147473663:
        case 2147473664:
        case 2147473665:
        case 2147473666:
        case 2147473667:
          ScopePropertyType scopePropertyType;
          int num2 = (int) new ScopePropertyTypeHybridRowSerializer().Read(ref row, ref scope, false, out scopePropertyType);
          value = (PropertyType) scopePropertyType;
          return (Result) num2;
        default:
          Contract.Fail("Type is abstract.");
          value = (PropertyType) null;
          return Result.Failure;
      }
    }

    public static Result ReadBase(ref RowBuffer row, ref RowCursor scope, ref PropertyType value)
    {
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
        return result1;
      Result result2 = PropertyTypeHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref PropertyType value)
    {
      byte num;
      Result result1 = LayoutType.UInt8.ReadFixed(ref row, ref scope, PropertyTypeHybridRowSerializer.TypeColumn, out num);
      switch (result1)
      {
        case Result.Success:
          value.Type = (TypeKind) num;
          goto case Result.NotFound;
        case Result.NotFound:
          bool flag;
          Result result2 = LayoutType.Boolean.ReadFixed(ref row, ref scope, PropertyTypeHybridRowSerializer.NullableColumn, out flag);
          switch (result2)
          {
            case Result.Success:
              value.Nullable = flag;
              goto case Result.NotFound;
            case Result.NotFound:
              string str;
              Result result3 = LayoutType.Utf8.ReadVariable(ref row, ref scope, PropertyTypeHybridRowSerializer.ApiTypeColumn, out str);
              switch (result3)
              {
                case Result.Success:
                  value.ApiType = str;
                  goto case Result.NotFound;
                case Result.NotFound:
                  return Result.Success;
                default:
                  return result3;
              }
            default:
              return result2;
          }
        default:
          return result1;
      }
    }

    public sealed class PropertyTypeComparer : EqualityComparer<PropertyType>
    {
      public static readonly PropertyTypeHybridRowSerializer.PropertyTypeComparer Default = new PropertyTypeHybridRowSerializer.PropertyTypeComparer();

      public override bool Equals(PropertyType x, PropertyType y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<PropertyType>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        switch (x)
        {
          case PrimitivePropertyType x1:
            return new PrimitivePropertyTypeHybridRowSerializer().Comparer.Equals(x1, (PrimitivePropertyType) y);
          case ScopePropertyType x2:
            return new ScopePropertyTypeHybridRowSerializer().Comparer.Equals(x2, (ScopePropertyType) y);
          default:
            Contract.Fail("Type is abstract.");
            return false;
        }
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      internal static bool EqualsBase(PropertyType x, PropertyType y) => new Utf8HybridRowSerializer().Comparer.Equals(x.ApiType, y.ApiType) && new UInt8HybridRowSerializer().Comparer.Equals((byte) x.Type, (byte) y.Type) && new BooleanHybridRowSerializer().Comparer.Equals(x.Nullable, y.Nullable);

      public override int GetHashCode(PropertyType obj)
      {
        switch (obj)
        {
          case PrimitivePropertyType primitivePropertyType:
            return new PrimitivePropertyTypeHybridRowSerializer().Comparer.GetHashCode(primitivePropertyType);
          case ScopePropertyType scopePropertyType:
            return new ScopePropertyTypeHybridRowSerializer().Comparer.GetHashCode(scopePropertyType);
          default:
            Contract.Fail("Type is abstract.");
            return 0;
        }
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      internal static int GetHashCodeBase(PropertyType obj) => HashCode.Combine<int, int, int>(new Utf8HybridRowSerializer().Comparer.GetHashCode(obj.ApiType), new UInt8HybridRowSerializer().Comparer.GetHashCode((byte) obj.Type), new BooleanHybridRowSerializer().Comparer.GetHashCode(obj.Nullable));
    }
  }
}
