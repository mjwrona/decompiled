// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO.RecordIOStream
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO
{
  public static class RecordIOStream
  {
    [Obsolete("Use ReadOnlyMemory<byte> override instead.")]
    public static Task<Result> ReadRecordIOAsync(
      this Stream stm,
      Func<Memory<byte>, Result> visitRecord,
      Func<Memory<byte>, Result> visitSegment = null,
      MemorySpanResizer<byte> resizer = null)
    {
      return stm.ReadRecordIOAsync((Func<ReadOnlyMemory<byte>, Result>) (rom => visitRecord(MemoryMarshal.AsMemory<byte>(rom))), visitSegment != null ? (Func<ReadOnlyMemory<byte>, Result>) (rom => visitSegment(MemoryMarshal.AsMemory<byte>(rom))) : (Func<ReadOnlyMemory<byte>, Result>) null, resizer);
    }

    public static Task<Result> ReadRecordIOAsync(
      this Stream stm,
      Func<ReadOnlyMemory<byte>, Result> visitRecord,
      Func<ReadOnlyMemory<byte>, Result> visitSegment = null,
      MemorySpanResizer<byte> resizer = null)
    {
      return stm.ReadRecordIOAsync((Func<ReadOnlyMemory<byte>, ValueTask<Result>>) (rom => new ValueTask<Result>(visitRecord(rom))), visitSegment != null ? (Func<ReadOnlyMemory<byte>, ValueTask<Result>>) (rom => new ValueTask<Result>(visitSegment(rom))) : (Func<ReadOnlyMemory<byte>, ValueTask<Result>>) null, resizer);
    }

    public static async Task<Result> ReadRecordIOAsync(
      this Stream stm,
      Func<ReadOnlyMemory<byte>, ValueTask<Result>> visitRecord,
      Func<ReadOnlyMemory<byte>, ValueTask<Result>> visitSegment = null,
      MemorySpanResizer<byte> resizer = null)
    {
      Contract.Requires(stm != null);
      Contract.Requires(visitRecord != null);
      resizer = resizer ?? new MemorySpanResizer<byte>();
      RecordIOParser parser = new RecordIOParser();
      int need = 0;
      Memory<byte> active = resizer.Memory;
      Memory<byte> avail = new Memory<byte>();
label_1:
      do
      {
        int num = await SpanExtensions.ReadAsync(stm, active.Slice(avail.Length), new CancellationToken());
        if (num != 0)
          avail = active.Slice(0, checked (avail.Length + num));
        else
          goto label_19;
      }
      while (avail.Length < need);
      while (avail.Length > 0)
      {
        RecordIOParser.ProductionType prodType;
        ReadOnlyMemory<byte> record;
        int consumed;
        Result result1 = parser.Process((ReadOnlyMemory<byte>) avail, out prodType, out record, out need, out consumed);
        switch (result1)
        {
          case Result.Success:
          case Result.InsufficientBuffer:
            active = active.Slice(consumed);
            avail = avail.Slice(consumed);
            if (avail.IsEmpty)
              active = resizer.Memory;
            if (result1 == Result.InsufficientBuffer)
            {
              if (need > active.Length)
              {
                resizer.Resize(need, avail.Span);
                active = resizer.Memory;
                avail = resizer.Memory.Slice(0, avail.Length);
                goto label_1;
              }
              else
                goto label_1;
            }
            else
            {
              if (prodType == RecordIOParser.ProductionType.Segment && visitSegment != null)
              {
                Result result2 = await visitSegment(record);
                if (result2 != Result.Success)
                  return result2;
              }
              if (prodType == RecordIOParser.ProductionType.Record)
              {
                Result result3 = await visitRecord(record);
                if (result3 != Result.Success)
                  return result3;
              }
              record = new ReadOnlyMemory<byte>();
              continue;
            }
          default:
            return result1;
        }
      }
      goto label_1;
label_19:
      return Result.Success;
    }

    public static Task<Result> WriteRecordIOAsync(
      this Stream stm,
      Segment segment,
      RecordIOStream.ProduceFunc produce,
      MemorySpanResizer<byte> resizer = null)
    {
      ReadOnlyMemory<byte> buffer;
      return stm.WriteRecordIOAsync(segment, (RecordIOStream.ProduceFuncAsync) (index => new ValueTask<(Result, ReadOnlyMemory<byte>)>((produce(index, out buffer), buffer))), resizer);
    }

    public static async Task<Result> WriteRecordIOAsync(
      this Stream stm,
      Segment segment,
      RecordIOStream.ProduceFuncAsync produce,
      MemorySpanResizer<byte> resizer = null)
    {
      resizer = resizer ?? new MemorySpanResizer<byte>();
      Memory<byte> block;
      Result result1 = RecordIOStream.FormatSegment(segment, resizer, out block);
      if (result1 != Result.Success)
        return result1;
      await SpanExtensions.WriteAsync(stm, (ReadOnlyMemory<byte>) block, new CancellationToken());
      long index = 0;
      Result result2;
      Result result3;
      while (true)
      {
        ReadOnlyMemory<byte> body;
        (result2, body) = await produce(checked (index++));
        if (result2 == Result.Success)
        {
          if (!body.IsEmpty)
          {
            result3 = RecordIOStream.FormatRow(body, resizer, out block);
            if (result3 == Result.Success)
            {
              await SpanExtensions.WriteAsync(stm, (ReadOnlyMemory<byte>) block, new CancellationToken());
              await SpanExtensions.WriteAsync(stm, body, new CancellationToken());
              body = new ReadOnlyMemory<byte>();
            }
            else
              goto label_8;
          }
          else
            goto label_11;
        }
        else
          break;
      }
      return result2;
label_8:
      return result3;
label_11:
      return Result.Success;
    }

    private static Result FormatSegment(
      Segment segment,
      MemorySpanResizer<byte> resizer,
      out Memory<byte> block)
    {
      RowBuffer row;
      Result result = RecordIOFormatter.FormatSegment(segment, out row, (ISpanResizer<byte>) resizer);
      if (result != Result.Success)
      {
        block = new Memory<byte>();
        return result;
      }
      block = resizer.Memory.Slice(0, row.Length);
      return Result.Success;
    }

    private static Result FormatRow(
      ReadOnlyMemory<byte> body,
      MemorySpanResizer<byte> resizer,
      out Memory<byte> block)
    {
      RowBuffer row;
      Result result = RecordIOFormatter.FormatRecord(body, out row, (ISpanResizer<byte>) resizer);
      if (result != Result.Success)
      {
        block = new Memory<byte>();
        return result;
      }
      block = resizer.Memory.Slice(0, row.Length);
      return Result.Success;
    }

    public delegate Result ProduceFunc(long index, out ReadOnlyMemory<byte> buffer);

    public delegate ValueTask<(Result Result, ReadOnlyMemory<byte> Buffer)> ProduceFuncAsync(
      long index);
  }
}
