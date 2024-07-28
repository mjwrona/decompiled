// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO.RecordIOFormatter
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO
{
  public static class RecordIOFormatter
  {
    private static readonly Layout SegmentLayout = SchemasHrSchema.LayoutResolver.Resolve((SchemaId) 2147473648);
    private static readonly Layout RecordLayout = SchemasHrSchema.LayoutResolver.Resolve((SchemaId) 2147473649);

    public static Result FormatSegment(
      Segment segment,
      out RowBuffer row,
      ISpanResizer<byte> resizer = null)
    {
      resizer = resizer ?? (ISpanResizer<byte>) DefaultSpanResizer<byte>.Default;
      int num1 = checked (5 + RecordIOFormatter.SegmentLayout.Size);
      int? length1 = segment.Comment?.Length;
      int? nullable = length1.HasValue ? new int?(checked (num1 + length1.GetValueOrDefault())) : new int?();
      int num2;
      if (!nullable.HasValue)
      {
        int? length2 = segment.SDL?.Length;
        num2 = (length2.HasValue ? new int?(length2.GetValueOrDefault()) : new int?()) ?? 20;
      }
      else
        num2 = nullable.GetValueOrDefault();
      int initialCapacity = num2;
      return RecordIOFormatter.FormatObject<Segment, SegmentHybridRowSerializer>(resizer, initialCapacity, RecordIOFormatter.SegmentLayout, segment, out row);
    }

    public static Result FormatRecord(
      ReadOnlyMemory<byte> body,
      out RowBuffer row,
      ISpanResizer<byte> resizer = null)
    {
      resizer = resizer ?? (ISpanResizer<byte>) DefaultSpanResizer<byte>.Default;
      int initialCapacity = checked (5 + RecordIOFormatter.RecordLayout.Size + body.Length);
      uint crc32 = Crc32.Update(0U, body.Span);
      Record record = new Record(body.Length, crc32);
      return RecordIOFormatter.FormatObject<Record, RecordHybridRowSerializer>(resizer, initialCapacity, RecordIOFormatter.RecordLayout, record, out row);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Result FormatObject<T, TSerializer>(
      ISpanResizer<byte> resizer,
      int initialCapacity,
      Layout layout,
      T obj,
      out RowBuffer row)
      where TSerializer : struct, IHybridRowSerializer<T>
    {
      row = new RowBuffer(initialCapacity, resizer);
      row.InitLayout(HybridRowVersion.V1, layout, SystemSchema.LayoutResolver);
      RowCursor scope = RowCursor.Create(ref row);
      Result result = default (TSerializer).Write(ref row, ref scope, true, new TypeArgumentList(), obj);
      if (result == Result.Success)
        return Result.Success;
      row = new RowBuffer();
      return result;
    }
  }
}
