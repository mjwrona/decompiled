// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.ObjectPropertyTypeHybridRowSerializer
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
  public readonly struct ObjectPropertyTypeHybridRowSerializer : 
    IHybridRowSerializer<ObjectPropertyType>
  {
    public const int SchemaId = 2147473662;
    public const int Size = 0;
    private static readonly Utf8String PropertiesName = Utf8String.TranscodeUtf16("properties");
    private static readonly Utf8String __BaseName = Utf8String.TranscodeUtf16("__base");
    private static readonly LayoutColumn PropertiesColumn;
    private static readonly LayoutColumn __BaseColumn;
    private static readonly StringToken PropertiesToken;
    private static readonly StringToken __BaseToken;

    public IEqualityComparer<ObjectPropertyType> Comparer => (IEqualityComparer<ObjectPropertyType>) ObjectPropertyTypeHybridRowSerializer.ObjectPropertyTypeComparer.Default;

    static ObjectPropertyTypeHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473662));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(ObjectPropertyTypeHybridRowSerializer.PropertiesName), out ObjectPropertyTypeHybridRowSerializer.PropertiesColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(ObjectPropertyTypeHybridRowSerializer.__BaseName), out ObjectPropertyTypeHybridRowSerializer.__BaseColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(ObjectPropertyTypeHybridRowSerializer.PropertiesColumn.Path), out ObjectPropertyTypeHybridRowSerializer.PropertiesToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(ObjectPropertyTypeHybridRowSerializer.__BaseColumn.Path), out ObjectPropertyTypeHybridRowSerializer.__BaseToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      ObjectPropertyType value)
    {
      if (isRoot)
        return ObjectPropertyTypeHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473662), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = ObjectPropertyTypeHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, ObjectPropertyType value)
    {
      if (value.Properties != null && value.Properties.Count > 0)
      {
        scope.Find(ref row, Utf8String.op_Implicit(ObjectPropertyTypeHybridRowSerializer.PropertiesColumn.Path));
        Result result = new TypedArrayHybridRowSerializer<Property, PropertyHybridRowSerializer>().Write(ref row, ref scope, false, ObjectPropertyTypeHybridRowSerializer.PropertiesColumn.TypeArgs, value.Properties);
        if (result != Result.Success)
          return result;
      }
      scope.Find(ref row, Utf8String.op_Implicit(ObjectPropertyTypeHybridRowSerializer.__BaseColumn.Path));
      Result result1 = ScopePropertyTypeHybridRowSerializer.WriteBase(ref row, ref scope, (ScopePropertyType) value);
      return result1 != Result.Success ? result1 : Result.Success;
    }

    public Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      out ObjectPropertyType value)
    {
      if (isRoot)
      {
        value = new ObjectPropertyType();
        return ObjectPropertyTypeHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (ObjectPropertyType) null;
        return result1;
      }
      value = new ObjectPropertyType();
      Result result2 = ObjectPropertyTypeHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (ObjectPropertyType) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(
      ref RowBuffer row,
      ref RowCursor scope,
      ref ObjectPropertyType value)
    {
      while (scope.MoveNext(ref row))
      {
        if ((long) scope.Token == (long) ObjectPropertyTypeHybridRowSerializer.PropertiesToken.Id)
        {
          List<Property> propertyList;
          Result result = new TypedArrayHybridRowSerializer<Property, PropertyHybridRowSerializer>().Read(ref row, ref scope, false, out propertyList);
          if (result != Result.Success)
            return result;
          value.Properties = propertyList;
        }
        else if ((long) scope.Token == (long) ObjectPropertyTypeHybridRowSerializer.__BaseToken.Id)
        {
          ScopePropertyType scopePropertyType = (ScopePropertyType) value;
          Result result = ScopePropertyTypeHybridRowSerializer.ReadBase(ref row, ref scope, ref scopePropertyType);
          if (result != Result.Success)
            return result;
        }
      }
      return Result.Success;
    }

    public sealed class ObjectPropertyTypeComparer : EqualityComparer<ObjectPropertyType>
    {
      public static readonly ObjectPropertyTypeHybridRowSerializer.ObjectPropertyTypeComparer Default = new ObjectPropertyTypeHybridRowSerializer.ObjectPropertyTypeComparer();

      public override bool Equals(ObjectPropertyType x, ObjectPropertyType y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<ObjectPropertyType>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        return ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer.EqualsBase((ScopePropertyType) x, (ScopePropertyType) y) && new TypedArrayHybridRowSerializer<Property, PropertyHybridRowSerializer>().Comparer.Equals(x.Properties, y.Properties);
      }

      public override int GetHashCode(ObjectPropertyType obj) => HashCode.Combine<int, int>(ScopePropertyTypeHybridRowSerializer.ScopePropertyTypeComparer.GetHashCodeBase((ScopePropertyType) obj), new TypedArrayHybridRowSerializer<Property, PropertyHybridRowSerializer>().Comparer.GetHashCode(obj.Properties));
    }
  }
}
