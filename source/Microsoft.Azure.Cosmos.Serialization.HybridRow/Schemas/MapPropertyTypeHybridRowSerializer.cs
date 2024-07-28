// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.MapPropertyTypeHybridRowSerializer
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
  public readonly struct MapPropertyTypeHybridRowSerializer : IHybridRowSerializer<MapPropertyType>
  {
    public const int SchemaId = 2147473665;
    public const int Size = 0;
    private static readonly Utf8String KeysName = Utf8String.TranscodeUtf16("keys");
    private static readonly Utf8String ValuesName = Utf8String.TranscodeUtf16("values");
    private static readonly Utf8String __BaseName = Utf8String.TranscodeUtf16("__base");
    private static readonly LayoutColumn KeysColumn;
    private static readonly LayoutColumn ValuesColumn;
    private static readonly LayoutColumn __BaseColumn;
    private static readonly StringToken KeysToken;
    private static readonly StringToken ValuesToken;
    private static readonly StringToken __BaseToken;

    public IEqualityComparer<MapPropertyType> Comparer => (IEqualityComparer<MapPropertyType>) MapPropertyTypeHybridRowSerializer.MapPropertyTypeComparer.Default;

    static MapPropertyTypeHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473665));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(MapPropertyTypeHybridRowSerializer.KeysName), out MapPropertyTypeHybridRowSerializer.KeysColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(MapPropertyTypeHybridRowSerializer.ValuesName), out MapPropertyTypeHybridRowSerializer.ValuesColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(MapPropertyTypeHybridRowSerializer.__BaseName), out MapPropertyTypeHybridRowSerializer.__BaseColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(MapPropertyTypeHybridRowSerializer.KeysColumn.Path), out MapPropertyTypeHybridRowSerializer.KeysToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(MapPropertyTypeHybridRowSerializer.ValuesColumn.Path), out MapPropertyTypeHybridRowSerializer.ValuesToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(MapPropertyTypeHybridRowSerializer.__BaseColumn.Path), out MapPropertyTypeHybridRowSerializer.__BaseToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      MapPropertyType value)
    {
      if (isRoot)
        return MapPropertyTypeHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473665), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = MapPropertyTypeHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, MapPropertyType value)
    {
      PropertyTypeHybridRowSerializer hybridRowSerializer;
      if (value.Keys != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(MapPropertyTypeHybridRowSerializer.KeysColumn.Path));
        hybridRowSerializer = new PropertyTypeHybridRowSerializer();
        Result result = hybridRowSerializer.Write(ref row, ref scope, false, MapPropertyTypeHybridRowSerializer.KeysColumn.TypeArgs, value.Keys);
        if (result != Result.Success)
          return result;
      }
      if (value.Values != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(MapPropertyTypeHybridRowSerializer.ValuesColumn.Path));
        hybridRowSerializer = new PropertyTypeHybridRowSerializer();
        Result result = hybridRowSerializer.Write(ref row, ref scope, false, MapPropertyTypeHybridRowSerializer.ValuesColumn.TypeArgs, value.Values);
        if (result != Result.Success)
          return result;
      }
      scope.Find(ref row, Utf8String.op_Implicit(MapPropertyTypeHybridRowSerializer.__BaseColumn.Path));
      Result result1 = ScopePropertyTypeHybridRowSerializer.WriteBase(ref row, ref scope, (ScopePropertyType) value);
      return result1 != Result.Success ? result1 : Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out MapPropertyType value)
    {
      if (isRoot)
      {
        value = new MapPropertyType();
        return MapPropertyTypeHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (MapPropertyType) null;
        return result1;
      }
      value = new MapPropertyType();
      Result result2 = MapPropertyTypeHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (MapPropertyType) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref MapPropertyType value)
    {
      PropertyTypeHybridRowSerializer hybridRowSerializer;
      while (scope.MoveNext(ref row))
      {
        if ((long) scope.Token == (long) MapPropertyTypeHybridRowSerializer.KeysToken.Id)
        {
          hybridRowSerializer = new PropertyTypeHybridRowSerializer();
          PropertyType propertyType;
          Result result = hybridRowSerializer.Read(ref row, ref scope, false, out propertyType);
          if (result != Result.Success)
            return result;
          value.Keys = propertyType;
        }
        else if ((long) scope.Token == (long) MapPropertyTypeHybridRowSerializer.ValuesToken.Id)
        {
          hybridRowSerializer = new PropertyTypeHybridRowSerializer();
          PropertyType propertyType;
          Result result = hybridRowSerializer.Read(ref row, ref scope, false, out propertyType);
          if (result != Result.Success)
            return result;
          value.Values = propertyType;
        }
        else if ((long) scope.Token == (long) MapPropertyTypeHybridRowSerializer.__BaseToken.Id)
        {
          ScopePropertyType scopePropertyType = (ScopePropertyType) value;
          Result result = ScopePropertyTypeHybridRowSerializer.ReadBase(ref row, ref scope, ref scopePropertyType);
          if (result != Result.Success)
            return result;
        }
      }
      return Result.Success;
    }

    public sealed class MapPropertyTypeComparer : EqualityComparer<MapPropertyType>
    {
      public static readonly MapPropertyTypeHybridRowSerializer.MapPropertyTypeComparer Default = new MapPropertyTypeHybridRowSerializer.MapPropertyTypeComparer();

      public override bool Equals(MapPropertyType x, MapPropertyType y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<MapPropertyType>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        if (ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer.EqualsBase((ScopePropertyType) x, (ScopePropertyType) y))
        {
          PropertyTypeHybridRowSerializer hybridRowSerializer = new PropertyTypeHybridRowSerializer();
          if (hybridRowSerializer.Comparer.Equals(x.Keys, y.Keys))
          {
            hybridRowSerializer = new PropertyTypeHybridRowSerializer();
            return hybridRowSerializer.Comparer.Equals(x.Values, y.Values);
          }
        }
        return false;
      }

      public override int GetHashCode(MapPropertyType obj)
      {
        int hashCodeBase = ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer.GetHashCodeBase((ScopePropertyType) obj);
        PropertyTypeHybridRowSerializer hybridRowSerializer = new PropertyTypeHybridRowSerializer();
        int hashCode1 = hybridRowSerializer.Comparer.GetHashCode(obj.Keys);
        hybridRowSerializer = new PropertyTypeHybridRowSerializer();
        int hashCode2 = hybridRowSerializer.Comparer.GetHashCode(obj.Values);
        return HashCode.Combine<int, int, int>(hashCodeBase, hashCode1, hashCode2);
      }
    }
  }
}
