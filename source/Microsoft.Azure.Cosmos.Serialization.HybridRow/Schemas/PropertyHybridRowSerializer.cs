// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.PropertyHybridRowSerializer
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
  public readonly struct PropertyHybridRowSerializer : IHybridRowSerializer<Property>
  {
    public const int SchemaId = 2147473657;
    public const int Size = 1;
    private static readonly Utf8String PathName = Utf8String.TranscodeUtf16("path");
    private static readonly Utf8String CommentName = Utf8String.TranscodeUtf16("comment");
    private static readonly Utf8String PropertyTypeName = Utf8String.TranscodeUtf16("type");
    private static readonly Utf8String ApiNameName = Utf8String.TranscodeUtf16("apiname");
    private static readonly Utf8String AllowEmptyName = Utf8String.TranscodeUtf16("allowEmpty");
    private static readonly LayoutColumn PathColumn;
    private static readonly LayoutColumn CommentColumn;
    private static readonly LayoutColumn PropertyTypeColumn;
    private static readonly LayoutColumn ApiNameColumn;
    private static readonly LayoutColumn AllowEmptyColumn;
    private static readonly StringToken CommentToken;
    private static readonly StringToken PropertyTypeToken;
    private static readonly StringToken ApiNameToken;
    private static readonly StringToken AllowEmptyToken;

    public IEqualityComparer<Property> Comparer => (IEqualityComparer<Property>) PropertyHybridRowSerializer.PropertyComparer.Default;

    static PropertyHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473657));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PropertyHybridRowSerializer.PathName), out PropertyHybridRowSerializer.PathColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PropertyHybridRowSerializer.CommentName), out PropertyHybridRowSerializer.CommentColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PropertyHybridRowSerializer.PropertyTypeName), out PropertyHybridRowSerializer.PropertyTypeColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PropertyHybridRowSerializer.ApiNameName), out PropertyHybridRowSerializer.ApiNameColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(PropertyHybridRowSerializer.AllowEmptyName), out PropertyHybridRowSerializer.AllowEmptyColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(PropertyHybridRowSerializer.CommentColumn.Path), out PropertyHybridRowSerializer.CommentToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(PropertyHybridRowSerializer.PropertyTypeColumn.Path), out PropertyHybridRowSerializer.PropertyTypeToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(PropertyHybridRowSerializer.ApiNameColumn.Path), out PropertyHybridRowSerializer.ApiNameToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(PropertyHybridRowSerializer.AllowEmptyColumn.Path), out PropertyHybridRowSerializer.AllowEmptyToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      Property value)
    {
      if (isRoot)
        return PropertyHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473657), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = PropertyHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, Property value)
    {
      if (value.Path != null)
      {
        Result result = LayoutType.Utf8.WriteVariable(ref row, ref scope, PropertyHybridRowSerializer.PathColumn, value.Path);
        if (result != Result.Success)
          return result;
      }
      if (value.Comment != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(PropertyHybridRowSerializer.CommentColumn.Path));
        Result result = LayoutType.Utf8.WriteSparse(ref row, ref scope, value.Comment, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.PropertyType != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(PropertyHybridRowSerializer.PropertyTypeColumn.Path));
        Result result = new PropertyTypeHybridRowSerializer().Write(ref row, ref scope, false, PropertyHybridRowSerializer.PropertyTypeColumn.TypeArgs, value.PropertyType);
        if (result != Result.Success)
          return result;
      }
      if (value.ApiName != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(PropertyHybridRowSerializer.ApiNameColumn.Path));
        Result result = LayoutType.Utf8.WriteSparse(ref row, ref scope, value.ApiName, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.AllowEmpty != AllowEmptyKind.None)
      {
        scope.Find(ref row, Utf8String.op_Implicit(PropertyHybridRowSerializer.AllowEmptyColumn.Path));
        Result result = LayoutType.UInt8.WriteSparse(ref row, ref scope, (byte) value.AllowEmpty, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      return Result.Success;
    }

    public Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out Property value)
    {
      if (isRoot)
      {
        value = new Property();
        return PropertyHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (Property) null;
        return result1;
      }
      value = new Property();
      Result result2 = PropertyHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (Property) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref Property value)
    {
      string str1;
      Result result1 = LayoutType.Utf8.ReadVariable(ref row, ref scope, PropertyHybridRowSerializer.PathColumn, out str1);
      switch (result1)
      {
        case Result.Success:
          value.Path = str1;
          goto case Result.NotFound;
        case Result.NotFound:
          while (scope.MoveNext(ref row))
          {
            if ((long) scope.Token == (long) PropertyHybridRowSerializer.CommentToken.Id)
            {
              string str2;
              Result result2 = LayoutType.Utf8.ReadSparse(ref row, ref scope, out str2);
              if (result2 != Result.Success)
                return result2;
              value.Comment = str2;
            }
            else if ((long) scope.Token == (long) PropertyHybridRowSerializer.PropertyTypeToken.Id)
            {
              PropertyType propertyType;
              Result result3 = new PropertyTypeHybridRowSerializer().Read(ref row, ref scope, false, out propertyType);
              if (result3 != Result.Success)
                return result3;
              value.PropertyType = propertyType;
            }
            else if ((long) scope.Token == (long) PropertyHybridRowSerializer.ApiNameToken.Id)
            {
              string str3;
              Result result4 = LayoutType.Utf8.ReadSparse(ref row, ref scope, out str3);
              if (result4 != Result.Success)
                return result4;
              value.ApiName = str3;
            }
            else if ((long) scope.Token == (long) PropertyHybridRowSerializer.AllowEmptyToken.Id)
            {
              byte num;
              Result result5 = LayoutType.UInt8.ReadSparse(ref row, ref scope, out num);
              if (result5 != Result.Success)
                return result5;
              value.AllowEmpty = (AllowEmptyKind) num;
            }
          }
          return Result.Success;
        default:
          return result1;
      }
    }

    public sealed class PropertyComparer : EqualityComparer<Property>
    {
      public static readonly PropertyHybridRowSerializer.PropertyComparer Default = new PropertyHybridRowSerializer.PropertyComparer();

      public override bool Equals(Property x, Property y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<Property>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        Utf8HybridRowSerializer hybridRowSerializer = new Utf8HybridRowSerializer();
        if (hybridRowSerializer.Comparer.Equals(x.Path, y.Path))
        {
          hybridRowSerializer = new Utf8HybridRowSerializer();
          if (hybridRowSerializer.Comparer.Equals(x.Comment, y.Comment) && new PropertyTypeHybridRowSerializer().Comparer.Equals(x.PropertyType, y.PropertyType))
          {
            hybridRowSerializer = new Utf8HybridRowSerializer();
            if (hybridRowSerializer.Comparer.Equals(x.ApiName, y.ApiName))
              return new UInt8HybridRowSerializer().Comparer.Equals((byte) x.AllowEmpty, (byte) y.AllowEmpty);
          }
        }
        return false;
      }

      public override int GetHashCode(Property obj)
      {
        Utf8HybridRowSerializer hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode1 = hybridRowSerializer.Comparer.GetHashCode(obj.Path);
        hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode2 = hybridRowSerializer.Comparer.GetHashCode(obj.Comment);
        int hashCode3 = new PropertyTypeHybridRowSerializer().Comparer.GetHashCode(obj.PropertyType);
        hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode4 = hybridRowSerializer.Comparer.GetHashCode(obj.ApiName);
        int hashCode5 = new UInt8HybridRowSerializer().Comparer.GetHashCode((byte) obj.AllowEmpty);
        return HashCode.Combine<int, int, int, int, int>(hashCode1, hashCode2, hashCode3, hashCode4, hashCode5);
      }
    }
  }
}
