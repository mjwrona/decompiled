// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.RecordHybridRowSerializer
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
  public readonly struct RecordHybridRowSerializer : IHybridRowSerializer<Record>
  {
    public const int SchemaId = 2147473649;
    public const int Size = 8;
    private static readonly Utf8String LengthName = Utf8String.TranscodeUtf16("length");
    private static readonly Utf8String Crc32Name = Utf8String.TranscodeUtf16("crc32");
    private static readonly LayoutColumn LengthColumn;
    private static readonly LayoutColumn Crc32Column;

    public IEqualityComparer<Record> Comparer => (IEqualityComparer<Record>) RecordHybridRowSerializer.RecordComparer.Default;

    static RecordHybridRowSerializer()
    {
      Layout layout = SchemasHrSchema.LayoutResolver.Resolve(new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473649));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(RecordHybridRowSerializer.LengthName), out RecordHybridRowSerializer.LengthColumn));
      Contract.Invariant(layout.TryFind(Utf8String.op_Implicit(RecordHybridRowSerializer.Crc32Name), out RecordHybridRowSerializer.Crc32Column));
    }

    public Result Write(
      ref RowBuffer row,
      ref RowCursor scope,
      bool isRoot,
      TypeArgumentList typeArgs,
      Record value)
    {
      if (isRoot)
        return RecordHybridRowSerializer.Write(ref row, ref scope, value);
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.WriteScope(ref row, ref scope, (TypeArgumentList) new Microsoft.Azure.Cosmos.Serialization.HybridRow.SchemaId(2147473649), out rowCursor, UpdateOptions.Upsert);
      if (result1 != Result.Success)
        return result1;
      Result result2 = RecordHybridRowSerializer.Write(ref row, ref rowCursor, value);
      if (result2 != Result.Success)
        return result2;
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Write(ref RowBuffer row, ref RowCursor scope, Record value)
    {
      if (value.Length != 0)
      {
        Result result = LayoutType.Int32.WriteFixed(ref row, ref scope, RecordHybridRowSerializer.LengthColumn, value.Length);
        if (result != Result.Success)
          return result;
      }
      if (value.Crc32 != 0U)
      {
        Result result = LayoutType.UInt32.WriteFixed(ref row, ref scope, RecordHybridRowSerializer.Crc32Column, value.Crc32);
        if (result != Result.Success)
          return result;
      }
      return Result.Success;
    }

    public Result Read(ref RowBuffer row, ref RowCursor scope, bool isRoot, out Record value)
    {
      if (isRoot)
      {
        value = new Record();
        return RecordHybridRowSerializer.Read(ref row, ref scope, ref value);
      }
      RowCursor rowCursor;
      Result result1 = LayoutType.UDT.ReadScope(ref row, ref scope, out rowCursor);
      if (result1 != Result.Success)
      {
        value = new Record();
        return result1;
      }
      value = new Record();
      Result result2 = RecordHybridRowSerializer.Read(ref row, ref rowCursor, ref value);
      if (result2 != Result.Success)
      {
        value = new Record();
        return result2;
      }
      scope.Skip(ref row, ref rowCursor);
      return Result.Success;
    }

    private static Result Read(ref RowBuffer row, ref RowCursor scope, ref Record value)
    {
      int num1;
      Result result1 = LayoutType.Int32.ReadFixed(ref row, ref scope, RecordHybridRowSerializer.LengthColumn, out num1);
      switch (result1)
      {
        case Result.Success:
          value.Length = num1;
          goto case Result.NotFound;
        case Result.NotFound:
          uint num2;
          Result result2 = LayoutType.UInt32.ReadFixed(ref row, ref scope, RecordHybridRowSerializer.Crc32Column, out num2);
          switch (result2)
          {
            case Result.Success:
              value.Crc32 = num2;
              goto case Result.NotFound;
            case Result.NotFound:
              return Result.Success;
            default:
              return result2;
          }
        default:
          return result1;
      }
    }

    public sealed class RecordComparer : EqualityComparer<Record>
    {
      public static readonly RecordHybridRowSerializer.RecordComparer Default = new RecordHybridRowSerializer.RecordComparer();

      public override bool Equals(Record x, Record y)
      {
        HybridRowSerializer.EqualityReferenceResult equalityReferenceResult = HybridRowSerializer.EqualityReferenceCheck<Record>(x, y);
        if (equalityReferenceResult != HybridRowSerializer.EqualityReferenceResult.Unknown)
          return equalityReferenceResult == HybridRowSerializer.EqualityReferenceResult.Equal;
        return new Int32HybridRowSerializer().Comparer.Equals(x.Length, y.Length) && new UInt32HybridRowSerializer().Comparer.Equals(x.Crc32, y.Crc32);
      }

      public override int GetHashCode(Record obj) => HashCode.Combine<int, int>(new Int32HybridRowSerializer().Comparer.GetHashCode(obj.Length), new UInt32HybridRowSerializer().Comparer.GetHashCode(obj.Crc32));
    }
  }
}
