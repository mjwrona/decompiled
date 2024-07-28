// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.NamespaceHybridRowSerializer
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
  public readonly struct NamespaceHybridRowSerializer : IHybridRowSerializer<Namespace>
  {
    public const int SchemaId = 2147473651;
    public const int Size = 2;
    private static readonly Utf8String VersionName = Utf8String.TranscodeUtf16("version");
    private static readonly Utf8String NameName = Utf8String.TranscodeUtf16("name");
    private static readonly Utf8String CommentName = Utf8String.TranscodeUtf16("comment");
    private static readonly Utf8String SchemasName = Utf8String.TranscodeUtf16("schemas");
    private static readonly Utf8String EnumsName = Utf8String.TranscodeUtf16("enums");
    private static readonly Utf8String CppNamespaceName = Utf8String.TranscodeUtf16("cppNamespace");
    private static readonly LayoutColumn VersionColumn;
    private static readonly LayoutColumn NameColumn;
    private static readonly LayoutColumn CommentColumn;
    private static readonly LayoutColumn SchemasColumn;
    private static readonly LayoutColumn EnumsColumn;
    private static readonly LayoutColumn CppNamespaceColumn;
    private static readonly StringToken CommentToken;
    private static readonly StringToken SchemasToken;
    private static readonly StringToken EnumsToken;
    private static readonly StringToken CppNamespaceToken;

    public IEqualityComparer<Namespace> Comparer => (IEqualityComparer<Namespace>) NamespaceHybridRowSerializer.NamespaceComparer.Default;

    static NamespaceHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473651));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(NamespaceHybridRowSerializer.VersionName), out NamespaceHybridRowSerializer.VersionColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(NamespaceHybridRowSerializer.NameName), out NamespaceHybridRowSerializer.NameColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(NamespaceHybridRowSerializer.CommentName), out NamespaceHybridRowSerializer.CommentColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(NamespaceHybridRowSerializer.SchemasName), out NamespaceHybridRowSerializer.SchemasColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(NamespaceHybridRowSerializer.EnumsName), out NamespaceHybridRowSerializer.EnumsColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(NamespaceHybridRowSerializer.CppNamespaceName), out NamespaceHybridRowSerializer.CppNamespaceColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(NamespaceHybridRowSerializer.CommentColumn.Path), out NamespaceHybridRowSerializer.CommentToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(NamespaceHybridRowSerializer.SchemasColumn.Path), out NamespaceHybridRowSerializer.SchemasToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(NamespaceHybridRowSerializer.EnumsColumn.Path), out NamespaceHybridRowSerializer.EnumsToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(NamespaceHybridRowSerializer.CppNamespaceColumn.Path), out NamespaceHybridRowSerializer.CppNamespaceToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      Namespace value)
    {
      if (isRoot)
        return NamespaceHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473651), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = NamespaceHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, Namespace value)
    {
      if (value.Version != SchemaLanguageVersion.V1)
      {
        Result result = LayoutType.UInt8.WriteFixed(ref row, ref scope, NamespaceHybridRowSerializer.VersionColumn, (byte) value.Version);
        if (result != Result.Success)
          return result;
      }
      if (value.Name != null)
      {
        Result result = LayoutType.Utf8.WriteVariable(ref row, ref scope, NamespaceHybridRowSerializer.NameColumn, value.Name);
        if (result != Result.Success)
          return result;
      }
      if (value.Comment != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(NamespaceHybridRowSerializer.CommentColumn.Path));
        Result result = LayoutType.Utf8.WriteSparse(ref row, ref scope, value.Comment, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.Schemas != null && value.Schemas.Count > 0)
      {
        scope.Find(ref row, Utf8String.op_Implicit(NamespaceHybridRowSerializer.SchemasColumn.Path));
        Result result = new TypedArrayHybridRowSerializer<Schema, SchemaHybridRowSerializer>().Write(ref row, ref scope, false, NamespaceHybridRowSerializer.SchemasColumn.TypeArgs, value.Schemas);
        if (result != Result.Success)
          return result;
      }
      if (value.Enums != null && value.Enums.Count > 0)
      {
        scope.Find(ref row, Utf8String.op_Implicit(NamespaceHybridRowSerializer.EnumsColumn.Path));
        Result result = new TypedArrayHybridRowSerializer<EnumSchema, EnumSchemaHybridRowSerializer>().Write(ref row, ref scope, false, NamespaceHybridRowSerializer.EnumsColumn.TypeArgs, value.Enums);
        if (result != Result.Success)
          return result;
      }
      if (value.CppNamespace != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(NamespaceHybridRowSerializer.CppNamespaceColumn.Path));
        Result result = LayoutType.Utf8.WriteSparse(ref row, ref scope, value.CppNamespace, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      return Result.Success;
    }

    public Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out Namespace value)
    {
      if (isRoot)
      {
        value = new Namespace();
        return NamespaceHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (Namespace) null;
        return result1;
      }
      value = new Namespace();
      Result result2 = NamespaceHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (Namespace) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref Namespace value)
    {
      byte num;
      Result result1 = LayoutType.UInt8.ReadFixed(ref row, ref scope, NamespaceHybridRowSerializer.VersionColumn, out num);
      switch (result1)
      {
        case Result.Success:
          value.Version = (SchemaLanguageVersion) num;
          goto case Result.NotFound;
        case Result.NotFound:
          string str1;
          Result result2 = LayoutType.Utf8.ReadVariable(ref row, ref scope, NamespaceHybridRowSerializer.NameColumn, out str1);
          switch (result2)
          {
            case Result.Success:
              value.Name = str1;
              goto case Result.NotFound;
            case Result.NotFound:
              while (scope.MoveNext(ref row))
              {
                if ((long) scope.Token == (long) NamespaceHybridRowSerializer.CommentToken.Id)
                {
                  string str2;
                  Result result3 = LayoutType.Utf8.ReadSparse(ref row, ref scope, out str2);
                  if (result3 != Result.Success)
                    return result3;
                  value.Comment = str2;
                }
                else if ((long) scope.Token == (long) NamespaceHybridRowSerializer.SchemasToken.Id)
                {
                  List<Schema> schemaList;
                  Result result4 = new TypedArrayHybridRowSerializer<Schema, SchemaHybridRowSerializer>().Read(ref row, ref scope, false, out schemaList);
                  if (result4 != Result.Success)
                    return result4;
                  value.Schemas = schemaList;
                }
                else if ((long) scope.Token == (long) NamespaceHybridRowSerializer.EnumsToken.Id)
                {
                  List<EnumSchema> enumSchemaList;
                  Result result5 = new TypedArrayHybridRowSerializer<EnumSchema, EnumSchemaHybridRowSerializer>().Read(ref row, ref scope, false, out enumSchemaList);
                  if (result5 != Result.Success)
                    return result5;
                  value.Enums = enumSchemaList;
                }
                else if ((long) scope.Token == (long) NamespaceHybridRowSerializer.CppNamespaceToken.Id)
                {
                  string str3;
                  Result result6 = LayoutType.Utf8.ReadSparse(ref row, ref scope, out str3);
                  if (result6 != Result.Success)
                    return result6;
                  value.CppNamespace = str3;
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

    public sealed class NamespaceComparer : EqualityComparer<Namespace>
    {
      public static readonly NamespaceHybridRowSerializer.NamespaceComparer Default = new NamespaceHybridRowSerializer.NamespaceComparer();

      public override bool Equals(Namespace x, Namespace y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<Namespace>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        if (new UInt8HybridRowSerializer().Comparer.Equals((byte) x.Version, (byte) y.Version))
        {
          Utf8HybridRowSerializer hybridRowSerializer = new Utf8HybridRowSerializer();
          if (hybridRowSerializer.Comparer.Equals(x.Name, y.Name))
          {
            hybridRowSerializer = new Utf8HybridRowSerializer();
            if (hybridRowSerializer.Comparer.Equals(x.Comment, y.Comment) && new TypedArrayHybridRowSerializer<Schema, SchemaHybridRowSerializer>().Comparer.Equals(x.Schemas, y.Schemas) && new TypedArrayHybridRowSerializer<EnumSchema, EnumSchemaHybridRowSerializer>().Comparer.Equals(x.Enums, y.Enums))
            {
              hybridRowSerializer = new Utf8HybridRowSerializer();
              return hybridRowSerializer.Comparer.Equals(x.CppNamespace, y.CppNamespace);
            }
          }
        }
        return false;
      }

      public override int GetHashCode(Namespace obj)
      {
        int hashCode1 = new UInt8HybridRowSerializer().Comparer.GetHashCode((byte) obj.Version);
        Utf8HybridRowSerializer hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode2 = hybridRowSerializer.Comparer.GetHashCode(obj.Name);
        hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode3 = hybridRowSerializer.Comparer.GetHashCode(obj.Comment);
        int hashCode4 = new TypedArrayHybridRowSerializer<Schema, SchemaHybridRowSerializer>().Comparer.GetHashCode(obj.Schemas);
        int hashCode5 = new TypedArrayHybridRowSerializer<EnumSchema, EnumSchemaHybridRowSerializer>().Comparer.GetHashCode(obj.Enums);
        hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode6 = hybridRowSerializer.Comparer.GetHashCode(obj.CppNamespace);
        return HashCode.Combine<int, int, int, int, int, int>(hashCode1, hashCode2, hashCode3, hashCode4, hashCode5, hashCode6);
      }
    }
  }
}
