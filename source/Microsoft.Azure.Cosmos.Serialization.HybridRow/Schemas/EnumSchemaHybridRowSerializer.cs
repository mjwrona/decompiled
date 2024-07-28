// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.EnumSchemaHybridRowSerializer
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
  public readonly struct EnumSchemaHybridRowSerializer : IHybridRowSerializer<EnumSchema>
  {
    public const int SchemaId = 2147473668;
    public const int Size = 2;
    private static readonly Utf8String TypeName = Utf8String.TranscodeUtf16("type");
    private static readonly Utf8String NameName = Utf8String.TranscodeUtf16("name");
    private static readonly Utf8String CommentName = Utf8String.TranscodeUtf16("comment");
    private static readonly Utf8String ApiTypeName = Utf8String.TranscodeUtf16("apitype");
    private static readonly Utf8String ValuesName = Utf8String.TranscodeUtf16("values");
    private static readonly LayoutColumn TypeColumn;
    private static readonly LayoutColumn NameColumn;
    private static readonly LayoutColumn CommentColumn;
    private static readonly LayoutColumn ApiTypeColumn;
    private static readonly LayoutColumn ValuesColumn;
    private static readonly StringToken CommentToken;
    private static readonly StringToken ApiTypeToken;
    private static readonly StringToken ValuesToken;

    public IEqualityComparer<EnumSchema> Comparer => (IEqualityComparer<EnumSchema>) EnumSchemaHybridRowSerializer.EnumSchemaComparer.Default;

    static EnumSchemaHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473668));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(EnumSchemaHybridRowSerializer.TypeName), out EnumSchemaHybridRowSerializer.TypeColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(EnumSchemaHybridRowSerializer.NameName), out EnumSchemaHybridRowSerializer.NameColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(EnumSchemaHybridRowSerializer.CommentName), out EnumSchemaHybridRowSerializer.CommentColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(EnumSchemaHybridRowSerializer.ApiTypeName), out EnumSchemaHybridRowSerializer.ApiTypeColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(EnumSchemaHybridRowSerializer.ValuesName), out EnumSchemaHybridRowSerializer.ValuesColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(EnumSchemaHybridRowSerializer.CommentColumn.Path), out EnumSchemaHybridRowSerializer.CommentToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(EnumSchemaHybridRowSerializer.ApiTypeColumn.Path), out EnumSchemaHybridRowSerializer.ApiTypeToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(EnumSchemaHybridRowSerializer.ValuesColumn.Path), out EnumSchemaHybridRowSerializer.ValuesToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      EnumSchema value)
    {
      if (isRoot)
        return EnumSchemaHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473668), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = EnumSchemaHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, EnumSchema value)
    {
      if (value.Type != TypeKind.Invalid)
      {
        Result result = LayoutType.UInt8.WriteFixed(ref row, ref scope, EnumSchemaHybridRowSerializer.TypeColumn, (byte) value.Type);
        if (result != Result.Success)
          return result;
      }
      if (value.Name != null)
      {
        Result result = LayoutType.Utf8.WriteVariable(ref row, ref scope, EnumSchemaHybridRowSerializer.NameColumn, value.Name);
        if (result != Result.Success)
          return result;
      }
      if (value.Comment != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(EnumSchemaHybridRowSerializer.CommentColumn.Path));
        Result result = LayoutType.Utf8.WriteSparse(ref row, ref scope, value.Comment, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.ApiType != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(EnumSchemaHybridRowSerializer.ApiTypeColumn.Path));
        Result result = LayoutType.Utf8.WriteSparse(ref row, ref scope, value.ApiType, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.Values != null && value.Values.Count > 0)
      {
        scope.Find(ref row, Utf8String.op_Implicit(EnumSchemaHybridRowSerializer.ValuesColumn.Path));
        Result result = new TypedArrayHybridRowSerializer<EnumValue, EnumValueHybridRowSerializer>().Write(ref row, ref scope, false, EnumSchemaHybridRowSerializer.ValuesColumn.TypeArgs, value.Values);
        if (result != Result.Success)
          return result;
      }
      return Result.Success;
    }

    public Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out EnumSchema value)
    {
      if (isRoot)
      {
        value = new EnumSchema();
        return EnumSchemaHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (EnumSchema) null;
        return result1;
      }
      value = new EnumSchema();
      Result result2 = EnumSchemaHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (EnumSchema) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref EnumSchema value)
    {
      byte num;
      Result result1 = LayoutType.UInt8.ReadFixed(ref row, ref scope, EnumSchemaHybridRowSerializer.TypeColumn, out num);
      switch (result1)
      {
        case Result.Success:
          value.Type = (TypeKind) num;
          goto case Result.NotFound;
        case Result.NotFound:
          string str1;
          Result result2 = LayoutType.Utf8.ReadVariable(ref row, ref scope, EnumSchemaHybridRowSerializer.NameColumn, out str1);
          switch (result2)
          {
            case Result.Success:
              value.Name = str1;
              goto case Result.NotFound;
            case Result.NotFound:
              while (scope.MoveNext(ref row))
              {
                if ((long) scope.Token == (long) EnumSchemaHybridRowSerializer.CommentToken.Id)
                {
                  string str2;
                  Result result3 = LayoutType.Utf8.ReadSparse(ref row, ref scope, out str2);
                  if (result3 != Result.Success)
                    return result3;
                  value.Comment = str2;
                }
                else if ((long) scope.Token == (long) EnumSchemaHybridRowSerializer.ApiTypeToken.Id)
                {
                  string str3;
                  Result result4 = LayoutType.Utf8.ReadSparse(ref row, ref scope, out str3);
                  if (result4 != Result.Success)
                    return result4;
                  value.ApiType = str3;
                }
                else if ((long) scope.Token == (long) EnumSchemaHybridRowSerializer.ValuesToken.Id)
                {
                  List<EnumValue> enumValueList;
                  Result result5 = new TypedArrayHybridRowSerializer<EnumValue, EnumValueHybridRowSerializer>().Read(ref row, ref scope, false, out enumValueList);
                  if (result5 != Result.Success)
                    return result5;
                  value.Values = enumValueList;
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

    public sealed class EnumSchemaComparer : EqualityComparer<EnumSchema>
    {
      public static readonly EnumSchemaHybridRowSerializer.EnumSchemaComparer Default = new EnumSchemaHybridRowSerializer.EnumSchemaComparer();

      public override bool Equals(EnumSchema x, EnumSchema y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<EnumSchema>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        if (new UInt8HybridRowSerializer().Comparer.Equals((byte) x.Type, (byte) y.Type))
        {
          Utf8HybridRowSerializer hybridRowSerializer = new Utf8HybridRowSerializer();
          if (hybridRowSerializer.Comparer.Equals(x.Name, y.Name))
          {
            hybridRowSerializer = new Utf8HybridRowSerializer();
            if (hybridRowSerializer.Comparer.Equals(x.Comment, y.Comment))
            {
              hybridRowSerializer = new Utf8HybridRowSerializer();
              if (hybridRowSerializer.Comparer.Equals(x.ApiType, y.ApiType))
                return new TypedArrayHybridRowSerializer<EnumValue, EnumValueHybridRowSerializer>().Comparer.Equals(x.Values, y.Values);
            }
          }
        }
        return false;
      }

      public override int GetHashCode(EnumSchema obj)
      {
        int hashCode1 = new UInt8HybridRowSerializer().Comparer.GetHashCode((byte) obj.Type);
        Utf8HybridRowSerializer hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode2 = hybridRowSerializer.Comparer.GetHashCode(obj.Name);
        hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode3 = hybridRowSerializer.Comparer.GetHashCode(obj.Comment);
        hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode4 = hybridRowSerializer.Comparer.GetHashCode(obj.ApiType);
        int hashCode5 = new TypedArrayHybridRowSerializer<EnumValue, EnumValueHybridRowSerializer>().Comparer.GetHashCode(obj.Values);
        return HashCode.Combine<int, int, int, int, int>(hashCode1, hashCode2, hashCode3, hashCode4, hashCode5);
      }
    }
  }
}
