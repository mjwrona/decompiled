// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO.RecordIOParser
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO
{
  public struct RecordIOParser
  {
    private RecordIOParser.State state;
    private Segment segment;
    private Record record;

    public bool HaveSegment => this.state >= RecordIOParser.State.NeedHeader;

    public Segment Segment
    {
      get
      {
        Contract.Requires(this.HaveSegment);
        return this.segment;
      }
    }

    [Obsolete("Use ReadOnlyMemory<byte> override instead.")]
    public Result Process(
      ReadOnlyMemory<byte> buffer,
      out RecordIOParser.ProductionType type,
      out Memory<byte> record,
      out int need,
      out int consumed)
    {
      ReadOnlyMemory<byte> record1;
      int num = (int) this.Process(buffer, out type, out record1, out need, out consumed);
      record = MemoryMarshal.AsMemory<byte>(record1);
      return (Result) num;
    }

    public Result Process(
      ReadOnlyMemory<byte> buffer,
      out RecordIOParser.ProductionType type,
      out ReadOnlyMemory<byte> record,
      out int need,
      out int consumed)
    {
      Result result = Result.Failure;
      ReadOnlyMemory<byte> readOnlyMemory = buffer;
      type = RecordIOParser.ProductionType.None;
      record = new ReadOnlyMemory<byte>();
      SegmentHybridRowSerializer hybridRowSerializer;
      switch (this.state)
      {
        case RecordIOParser.State.Start:
          this.state = RecordIOParser.State.NeedSegmentLength;
          goto case RecordIOParser.State.NeedSegmentLength;
        case RecordIOParser.State.NeedSegmentLength:
          int length1 = 10;
          if (readOnlyMemory.Length < length1)
          {
            need = length1;
            consumed = checked (buffer.Length - readOnlyMemory.Length);
            return Result.InsufficientBuffer;
          }
          RowBuffer row1 = new RowBuffer(MemoryMarshal.AsMemory<byte>(readOnlyMemory.Slice(0, length1)).Span, HybridRowVersion.V1, SchemasHrSchema.LayoutResolver);
          RowCursor scope1 = RowCursor.Create(ref row1);
          hybridRowSerializer = new SegmentHybridRowSerializer();
          result = hybridRowSerializer.Read(ref row1, ref scope1, true, out this.segment);
          if (result == Result.Success)
          {
            this.state = RecordIOParser.State.NeedSegment;
            goto case RecordIOParser.State.NeedSegment;
          }
          else
            break;
        case RecordIOParser.State.NeedSegment:
          if (readOnlyMemory.Length < this.segment.Length)
          {
            need = this.segment.Length;
            consumed = checked (buffer.Length - readOnlyMemory.Length);
            return Result.InsufficientBuffer;
          }
          ReadOnlyMemory<byte> memory1 = readOnlyMemory.Slice(0, this.segment.Length);
          RowBuffer row2 = new RowBuffer(MemoryMarshal.AsMemory<byte>(memory1).Span, HybridRowVersion.V1, SchemasHrSchema.LayoutResolver);
          RowCursor scope2 = RowCursor.Create(ref row2);
          hybridRowSerializer = new SegmentHybridRowSerializer();
          result = hybridRowSerializer.Read(ref row2, ref scope2, true, out this.segment);
          if (result == Result.Success)
          {
            record = readOnlyMemory.Slice(0, memory1.Length);
            readOnlyMemory = readOnlyMemory.Slice(memory1.Length);
            need = 0;
            this.state = RecordIOParser.State.NeedHeader;
            consumed = checked (buffer.Length - readOnlyMemory.Length);
            type = RecordIOParser.ProductionType.Segment;
            return Result.Success;
          }
          break;
        case RecordIOParser.State.NeedHeader:
          if (readOnlyMemory.Length < 5)
          {
            need = 5;
            consumed = checked (buffer.Length - readOnlyMemory.Length);
            return Result.InsufficientBuffer;
          }
          HybridRowHeader hybridRowHeader;
          MemoryMarshal.TryRead<HybridRowHeader>(readOnlyMemory.Span, out hybridRowHeader);
          if (hybridRowHeader.Version != HybridRowVersion.V1)
          {
            result = Result.InvalidRow;
            break;
          }
          if (!(hybridRowHeader.SchemaId == (SchemaId) 2147473648))
          {
            if (!(hybridRowHeader.SchemaId == (SchemaId) 2147473649))
            {
              result = Result.InvalidRow;
              break;
            }
            goto case RecordIOParser.State.NeedRecord;
          }
          else
            goto case RecordIOParser.State.NeedSegment;
        case RecordIOParser.State.NeedRecord:
          int length2 = 13;
          if (readOnlyMemory.Length < length2)
          {
            need = length2;
            consumed = checked (buffer.Length - readOnlyMemory.Length);
            return Result.InsufficientBuffer;
          }
          ReadOnlyMemory<byte> memory2 = readOnlyMemory.Slice(0, length2);
          RowBuffer row3 = new RowBuffer(MemoryMarshal.AsMemory<byte>(memory2).Span, HybridRowVersion.V1, SchemasHrSchema.LayoutResolver);
          RowCursor scope3 = RowCursor.Create(ref row3);
          result = new RecordHybridRowSerializer().Read(ref row3, ref scope3, true, out this.record);
          if (result == Result.Success)
          {
            readOnlyMemory = readOnlyMemory.Slice(memory2.Length);
            this.state = RecordIOParser.State.NeedRow;
            goto case RecordIOParser.State.NeedRow;
          }
          else
            break;
        case RecordIOParser.State.NeedRow:
          if (readOnlyMemory.Length < this.record.Length)
          {
            need = this.record.Length;
            consumed = checked (buffer.Length - readOnlyMemory.Length);
            return Result.InsufficientBuffer;
          }
          record = readOnlyMemory.Slice(0, this.record.Length);
          if ((int) Crc32.Update(0U, record.Span) != (int) this.record.Crc32)
          {
            result = Result.InvalidRow;
            break;
          }
          readOnlyMemory = readOnlyMemory.Slice(this.record.Length);
          need = 0;
          this.state = RecordIOParser.State.NeedHeader;
          consumed = checked (buffer.Length - readOnlyMemory.Length);
          type = RecordIOParser.ProductionType.Record;
          return Result.Success;
      }
      this.state = RecordIOParser.State.Error;
      need = 0;
      consumed = checked (buffer.Length - readOnlyMemory.Length);
      return result;
    }

    public enum ProductionType
    {
      None,
      Segment,
      Record,
    }

    private enum State : byte
    {
      Start,
      Error,
      NeedSegmentLength,
      NeedSegment,
      NeedHeader,
      NeedRecord,
      NeedRow,
    }
  }
}
