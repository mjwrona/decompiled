// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.EnumValueHybridRowSerializer
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
  public readonly struct EnumValueHybridRowSerializer : IHybridRowSerializer<EnumValue>
  {
    public const int SchemaId = 2147473669;
    public const int Size = 9;
    private static readonly Utf8String NameName = Utf8String.TranscodeUtf16("name");
    private static readonly Utf8String ValueName = Utf8String.TranscodeUtf16("value");
    private static readonly Utf8String CommentName = Utf8String.TranscodeUtf16("comment");
    private static readonly LayoutColumn NameColumn;
    private static readonly LayoutColumn ValueColumn;
    private static readonly LayoutColumn CommentColumn;
    private static readonly StringToken CommentToken;

    public IEqualityComparer<EnumValue> Comparer => (IEqualityComparer<EnumValue>) EnumValueHybridRowSerializer.EnumValueComparer.Default;

    static EnumValueHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473669));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(EnumValueHybridRowSerializer.NameName), out EnumValueHybridRowSerializer.NameColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(EnumValueHybridRowSerializer.ValueName), out EnumValueHybridRowSerializer.ValueColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(EnumValueHybridRowSerializer.CommentName), out EnumValueHybridRowSerializer.CommentColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(EnumValueHybridRowSerializer.CommentColumn.Path), out EnumValueHybridRowSerializer.CommentToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      EnumValue value)
    {
      if (isRoot)
        return EnumValueHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473669), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = EnumValueHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, EnumValue value)
    {
      if (value.Value != 0L)
      {
        Result result = LayoutType.Int64.WriteFixed(ref row, ref scope, EnumValueHybridRowSerializer.ValueColumn, value.Value);
        if (result != Result.Success)
          return result;
      }
      if (value.Name != null)
      {
        Result result = LayoutType.Utf8.WriteVariable(ref row, ref scope, EnumValueHybridRowSerializer.NameColumn, value.Name);
        if (result != Result.Success)
          return result;
      }
      if (value.Comment != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(EnumValueHybridRowSerializer.CommentColumn.Path));
        Result result = LayoutType.Utf8.WriteSparse(ref row, ref scope, value.Comment, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      return Result.Success;
    }

    public Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out EnumValue value)
    {
      if (isRoot)
      {
        value = new EnumValue();
        return EnumValueHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = (EnumValue) null;
        return result1;
      }
      value = new EnumValue();
      Result result2 = EnumValueHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = (EnumValue) null;
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref EnumValue value)
    {
      long num;
      Result result1 = LayoutType.Int64.ReadFixed(ref row, ref scope, EnumValueHybridRowSerializer.ValueColumn, out num);
      switch (result1)
      {
        case Result.Success:
          value.Value = num;
          goto case Result.NotFound;
        case Result.NotFound:
          string str1;
          Result result2 = LayoutType.Utf8.ReadVariable(ref row, ref scope, EnumValueHybridRowSerializer.NameColumn, out str1);
          switch (result2)
          {
            case Result.Success:
              value.Name = str1;
              goto case Result.NotFound;
            case Result.NotFound:
              while (scope.MoveNext(ref row))
              {
                if ((long) scope.Token == (long) EnumValueHybridRowSerializer.CommentToken.Id)
                {
                  string str2;
                  Result result3 = LayoutType.Utf8.ReadSparse(ref row, ref scope, out str2);
                  if (result3 != Result.Success)
                    return result3;
                  value.Comment = str2;
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

    public sealed class EnumValueComparer : EqualityComparer<EnumValue>
    {
      public static readonly EnumValueHybridRowSerializer.EnumValueComparer Default = new EnumValueHybridRowSerializer.EnumValueComparer();

      public override bool Equals(EnumValue x, EnumValue y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<EnumValue>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        Utf8HybridRowSerializer hybridRowSerializer = new Utf8HybridRowSerializer();
        if (!hybridRowSerializer.Comparer.Equals(x.Name, y.Name) || !new Int64HybridRowSerializer().Comparer.Equals(x.Value, y.Value))
          return false;
        hybridRowSerializer = new Utf8HybridRowSerializer();
        return hybridRowSerializer.Comparer.Equals(x.Comment, y.Comment);
      }

      public override int GetHashCode(EnumValue obj)
      {
        Utf8HybridRowSerializer hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode1 = hybridRowSerializer.Comparer.GetHashCode(obj.Name);
        int hashCode2 = new Int64HybridRowSerializer().Comparer.GetHashCode(obj.Value);
        hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode3 = hybridRowSerializer.Comparer.GetHashCode(obj.Comment);
        return HashCode.Combine<int, int, int>(hashCode1, hashCode2, hashCode3);
      }
    }
  }
}
