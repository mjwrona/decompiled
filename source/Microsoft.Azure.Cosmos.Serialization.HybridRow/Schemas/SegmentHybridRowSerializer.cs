// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.SegmentHybridRowSerializer
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public readonly struct SegmentHybridRowSerializer : IHybridRowSerializer<Segment>
  {
    public const int SchemaId = 2147473648;
    public const int Size = 5;
    private static readonly Utf8String LengthName = Utf8String.TranscodeUtf16("length");
    private static readonly Utf8String CommentName = Utf8String.TranscodeUtf16("comment");
    private static readonly Utf8String SDLName = Utf8String.TranscodeUtf16("sdl");
    private static readonly Utf8String SchemaName = Utf8String.TranscodeUtf16("schema");
    private static readonly LayoutColumn LengthColumn;
    private static readonly LayoutColumn CommentColumn;
    private static readonly LayoutColumn SDLColumn;
    private static readonly LayoutColumn SchemaColumn;
    private static readonly StringToken CommentToken;
    private static readonly StringToken SDLToken;
    private static readonly StringToken SchemaToken;

    public IEqualityComparer<Segment> Comparer => (IEqualityComparer<Segment>) SegmentHybridRowSerializer.SegmentComparer.Default;

    static SegmentHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473648));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SegmentHybridRowSerializer.LengthName), out SegmentHybridRowSerializer.LengthColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SegmentHybridRowSerializer.CommentName), out SegmentHybridRowSerializer.CommentColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SegmentHybridRowSerializer.SDLName), out SegmentHybridRowSerializer.SDLColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(SegmentHybridRowSerializer.SchemaName), out SegmentHybridRowSerializer.SchemaColumn));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SegmentHybridRowSerializer.CommentColumn.Path), out SegmentHybridRowSerializer.CommentToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SegmentHybridRowSerializer.SDLColumn.Path), out SegmentHybridRowSerializer.SDLToken));
      Contract.Invariant(layout.Tokenizer.TryFindToken(Utf8String.op_Implicit(SegmentHybridRowSerializer.SchemaColumn.Path), out SegmentHybridRowSerializer.SchemaToken));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      Segment value)
    {
      if (isRoot)
        return SegmentHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473648), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = SegmentHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, Segment value)
    {
      if (value.Comment != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SegmentHybridRowSerializer.CommentColumn.Path));
        Result result = LayoutType.Utf8.WriteSparse(ref row, ref scope, value.Comment, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.SDL != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SegmentHybridRowSerializer.SDLColumn.Path));
        Result result = LayoutType.Utf8.WriteSparse(ref row, ref scope, value.SDL, UpdateOptions.Upsert);
        if (result != Result.Success)
          return result;
      }
      if (value.Schema != null)
      {
        scope.Find(ref row, Utf8String.op_Implicit(SegmentHybridRowSerializer.SchemaColumn.Path));
        Result result = new NamespaceHybridRowSerializer().Write(ref row, ref scope, false, SegmentHybridRowSerializer.SchemaColumn.TypeArgs, value.Schema);
        if (result != Result.Success)
          return result;
      }
      Result result1 = LayoutType.Int32.WriteFixed(ref row, ref scope, SegmentHybridRowSerializer.LengthColumn, row.Length);
      return result1 != Result.Success ? result1 : Result.Success;
    }

    public Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out Segment value)
    {
      if (isRoot)
      {
        value = new Segment();
        return SegmentHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = new Segment();
        return result1;
      }
      value = new Segment();
      Result result2 = SegmentHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = new Segment();
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref Segment value)
    {
      int num;
      Result result1 = LayoutType.Int32.ReadFixed(ref row, ref scope, SegmentHybridRowSerializer.LengthColumn, out num);
      switch (result1)
      {
        case Result.Success:
          value.Length = num;
          goto case Result.NotFound;
        case Result.NotFound:
          while (scope.MoveNext(ref row))
          {
            if ((long) scope.Token == (long) SegmentHybridRowSerializer.CommentToken.Id)
            {
              string str;
              Result result2 = LayoutType.Utf8.ReadSparse(ref row, ref scope, out str);
              if (result2 != Result.Success)
                return result2;
              value.Comment = str;
            }
            else if ((long) scope.Token == (long) SegmentHybridRowSerializer.SDLToken.Id)
            {
              string str;
              Result result3 = LayoutType.Utf8.ReadSparse(ref row, ref scope, out str);
              if (result3 != Result.Success)
                return result3;
              value.SDL = str;
            }
            else if ((long) scope.Token == (long) SegmentHybridRowSerializer.SchemaToken.Id)
            {
              Namespace @namespace;
              Result result4 = new NamespaceHybridRowSerializer().Read(ref row, ref scope, false, out @namespace);
              if (result4 != Result.Success)
                return result4;
              value.Schema = @namespace;
            }
          }
          return Result.Success;
        default:
          return result1;
      }
    }

    public sealed class SegmentComparer : EqualityComparer<Segment>
    {
      public static readonly SegmentHybridRowSerializer.SegmentComparer Default = new SegmentHybridRowSerializer.SegmentComparer();

      public override bool Equals(Segment x, Segment y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<Segment>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        if (new Int32HybridRowSerializer().Comparer.Equals(x.Length, y.Length))
        {
          Utf8HybridRowSerializer hybridRowSerializer = new Utf8HybridRowSerializer();
          if (hybridRowSerializer.Comparer.Equals(x.Comment, y.Comment))
          {
            hybridRowSerializer = new Utf8HybridRowSerializer();
            if (hybridRowSerializer.Comparer.Equals(x.SDL, y.SDL))
              return new NamespaceHybridRowSerializer().Comparer.Equals(x.Schema, y.Schema);
          }
        }
        return false;
      }

      public override int GetHashCode(Segment obj)
      {
        int hashCode1 = new Int32HybridRowSerializer().Comparer.GetHashCode(obj.Length);
        Utf8HybridRowSerializer hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode2 = hybridRowSerializer.Comparer.GetHashCode(obj.Comment);
        hybridRowSerializer = new Utf8HybridRowSerializer();
        int hashCode3 = hybridRowSerializer.Comparer.GetHashCode(obj.SDL);
        int hashCode4 = new NamespaceHybridRowSerializer().Comparer.GetHashCode(obj.Schema);
        return HashCode.Combine<int, int, int, int>(hashCode1, hashCode2, hashCode3, hashCode4);
      }
    }
  }
}
