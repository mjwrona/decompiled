// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.RowBuffer
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow
{
  public ref struct RowBuffer
  {
    private readonly ISpanResizer<byte> resizer;
    private Span<byte> buffer;
    private LayoutResolver resolver;
    private int length;

    public RowBuffer(int capacity, ISpanResizer<byte> resizer = null)
    {
      this.resizer = resizer ?? (ISpanResizer<byte>) DefaultSpanResizer<byte>.Default;
      this.buffer = this.resizer.Resize(capacity);
      this.length = 0;
      this.resolver = (LayoutResolver) null;
    }

    public RowBuffer(
      Span<byte> buffer,
      HybridRowVersion version,
      LayoutResolver resolver,
      ISpanResizer<byte> resizer = null)
    {
      Contract.Requires(buffer.Length >= 5);
      this.resizer = resizer ?? (ISpanResizer<byte>) DefaultSpanResizer<byte>.Default;
      this.length = buffer.Length;
      this.buffer = buffer;
      this.resolver = resolver;
      HybridRowHeader hybridRowHeader = this.ReadHeader(0);
      Contract.Invariant(hybridRowHeader.Version == version);
      Contract.Invariant(checked (5 + resolver.Resolve(hybridRowHeader.SchemaId).Size) <= this.length);
    }

    public HybridRowHeader Header => this.ReadHeader(0);

    public int Length => this.length;

    public byte[] ToArray() => this.AsSpan().ToArray();

    public ReadOnlySpan<byte> AsSpan() => (ReadOnlySpan<byte>) this.buffer.Slice(0, this.length);

    public LayoutResolver Resolver => this.resolver;

    public void Reset()
    {
      this.length = 0;
      this.resolver = (LayoutResolver) null;
    }

    public void WriteTo(Stream stream) => SpanExtensions.Write(stream, (ReadOnlySpan<byte>) this.buffer.Slice(0, this.length));

    public bool ReadFrom(
      Stream inputStream,
      int bytesCount,
      HybridRowVersion rowVersion,
      LayoutResolver resolver)
    {
      Contract.Requires(inputStream != null);
      this.Reset();
      this.resolver = resolver;
      this.Ensure(bytesCount);
      this.length = bytesCount;
      Span<byte> span = this.buffer.Slice(0, bytesCount);
      int start;
      do
      {
        start = SpanExtensions.Read(inputStream, span);
        span = span.Slice(start);
      }
      while (start != 0);
      return span.Length == 0 && this.InitReadFrom(rowVersion);
    }

    public bool ReadFrom(
      ReadOnlySpan<byte> input,
      HybridRowVersion rowVersion,
      LayoutResolver resolver)
    {
      int length = input.Length;
      this.Reset();
      this.resolver = resolver;
      this.Ensure(length);
      input.CopyTo(this.buffer);
      this.length = length;
      return this.InitReadFrom(rowVersion);
    }

    public void InitLayout(HybridRowVersion version, Layout layout, LayoutResolver resolver)
    {
      Contract.Requires(layout != null);
      this.resolver = resolver;
      this.Ensure(checked (5 + layout.Size));
      this.length = checked (5 + layout.Size);
      this.buffer.Slice(5, layout.Size).Fill((byte) 0);
      this.WriteHeader(0, new HybridRowHeader(version, layout.SchemaId));
    }

    internal void WriteHeader(int offset, HybridRowHeader value) => MemoryMarshal.Write<HybridRowHeader>(this.buffer.Slice(offset), ref value);

    internal HybridRowHeader ReadHeader(int offset) => MemoryMarshal.Read<HybridRowHeader>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void WriteSchemaId(int offset, SchemaId value) => this.WriteInt32(offset, value.Id);

    internal SchemaId ReadSchemaId(int offset) => new SchemaId(this.ReadInt32(offset));

    internal void SetBit(int offset, LayoutBit bit)
    {
      if (bit.IsInvalid)
        return;
      this.buffer[bit.GetOffset(offset)] |= (byte) (1 << bit.GetBit());
    }

    internal void UnsetBit(int offset, LayoutBit bit) => this.buffer[bit.GetOffset(offset)] &= (byte) ~(1 << bit.GetBit());

    internal bool ReadBit(int offset, LayoutBit bit) => bit.IsInvalid || ((uint) this.buffer[bit.GetOffset(offset)] & (uint) (byte) (1 << bit.GetBit())) > 0U;

    internal void DeleteVariable(int offset, bool isVarint)
    {
      int lenInBytes;
      ulong num = this.Read7BitEncodedUInt(offset, out lenInBytes);
      if (!isVarint)
        checked { lenInBytes += (int) num; }
      this.buffer.Slice(checked (offset + lenInBytes), checked (this.length - offset + lenInBytes)).CopyTo(this.buffer.Slice(offset));
      checked { this.length -= lenInBytes; }
    }

    internal void WriteInt8(int offset, sbyte value) => this.buffer[offset] = (byte) value;

    internal sbyte ReadInt8(int offset) => (sbyte) this.buffer[offset];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void WriteUInt8(int offset, byte value) => this.buffer[offset] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal byte ReadUInt8(int offset) => this.buffer[offset];

    internal void WriteInt16(int offset, short value) => MemoryMarshal.Write<short>(this.buffer.Slice(offset), ref value);

    internal short ReadInt16(int offset) => MemoryMarshal.Read<short>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void WriteUInt16(int offset, ushort value) => MemoryMarshal.Write<ushort>(this.buffer.Slice(offset), ref value);

    internal ushort ReadUInt16(int offset) => MemoryMarshal.Read<ushort>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void WriteInt32(int offset, int value) => MemoryMarshal.Write<int>(this.buffer.Slice(offset), ref value);

    internal int ReadInt32(int offset) => MemoryMarshal.Read<int>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void IncrementUInt32(int offset, uint increment) => (MemoryMarshal.Cast<byte, uint>(this.buffer.Slice(offset))[0]) += (increment);

    internal void DecrementUInt32(int offset, uint decrement) => (MemoryMarshal.Cast<byte, uint>(this.buffer.Slice(offset))[0]) -= (decrement);

    internal void WriteUInt32(int offset, uint value) => MemoryMarshal.Write<uint>(this.buffer.Slice(offset), ref value);

    internal uint ReadUInt32(int offset) => MemoryMarshal.Read<uint>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void WriteInt64(int offset, long value) => MemoryMarshal.Write<long>(this.buffer.Slice(offset), ref value);

    internal long ReadInt64(int offset) => MemoryMarshal.Read<long>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void WriteUInt64(int offset, ulong value) => MemoryMarshal.Write<ulong>(this.buffer.Slice(offset), ref value);

    internal ulong ReadUInt64(int offset) => MemoryMarshal.Read<ulong>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal int Write7BitEncodedUInt(int offset, ulong value)
    {
      int num1 = 0;
      for (; value >= 128UL; value >>= 7)
        this.WriteUInt8(checked (offset + num1++), (byte) (value | 128UL));
      int num2 = offset;
      int num3 = num1;
      int num4 = checked (num3 + 1);
      this.WriteUInt8(checked (num2 + num3), checked ((byte) value));
      return num4;
    }

    internal ulong Read7BitEncodedUInt(int offset, out int lenInBytes)
    {
      ulong num1 = (ulong) this.buffer[offset];
      if (num1 < 128UL)
      {
        lenInBytes = 1;
        return num1;
      }
      ulong num2 = num1 & (ulong) sbyte.MaxValue;
      int num3 = 7;
      ulong num4;
      do
      {
        num4 = (ulong) this.buffer[checked (++offset)];
        num2 |= (ulong) (((long) num4 & (long) sbyte.MaxValue) << num3);
        checked { num3 += 7; }
      }
      while (num4 >= 128UL);
      lenInBytes = num3 / 7;
      return num2;
    }

    internal int Write7BitEncodedInt(int offset, long value) => this.Write7BitEncodedUInt(offset, RowBuffer.RotateSignToLsb(value));

    internal long Read7BitEncodedInt(int offset, out int lenInBytes) => RowBuffer.RotateSignToMsb(this.Read7BitEncodedUInt(offset, out lenInBytes));

    internal void WriteFloat32(int offset, float value) => MemoryMarshal.Write<float>(this.buffer.Slice(offset), ref value);

    internal float ReadFloat32(int offset) => MemoryMarshal.Read<float>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void WriteFloat64(int offset, double value) => MemoryMarshal.Write<double>(this.buffer.Slice(offset), ref value);

    internal double ReadFloat64(int offset) => MemoryMarshal.Read<double>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void WriteFloat128(int offset, Float128 value) => MemoryMarshal.Write<Float128>(this.buffer.Slice(offset), ref value);

    internal Float128 ReadFloat128(int offset) => MemoryMarshal.Read<Float128>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void WriteDecimal(int offset, Decimal value) => MemoryMarshal.Write<Decimal>(this.buffer.Slice(offset), ref value);

    internal Decimal ReadDecimal(int offset) => MemoryMarshal.Read<Decimal>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void WriteDateTime(int offset, DateTime value) => MemoryMarshal.Write<DateTime>(this.buffer.Slice(offset), ref value);

    internal DateTime ReadDateTime(int offset) => MemoryMarshal.Read<DateTime>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void WriteUnixDateTime(int offset, UnixDateTime value) => MemoryMarshal.Write<UnixDateTime>(this.buffer.Slice(offset), ref value);

    internal UnixDateTime ReadUnixDateTime(int offset) => MemoryMarshal.Read<UnixDateTime>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void WriteGuid(int offset, Guid value) => MemoryMarshal.Write<Guid>(this.buffer.Slice(offset), ref value);

    internal Guid ReadGuid(int offset) => MemoryMarshal.Read<Guid>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal void WriteMongoDbObjectId(int offset, MongoDbObjectId value) => MemoryMarshal.Write<MongoDbObjectId>(this.buffer.Slice(offset), ref value);

    internal MongoDbObjectId ReadMongoDbObjectId(int offset) => MemoryMarshal.Read<MongoDbObjectId>((ReadOnlySpan<byte>) this.buffer.Slice(offset));

    internal Utf8Span ReadFixedString(int offset, int len) => Utf8Span.UnsafeFromUtf8BytesNoValidation((ReadOnlySpan<byte>) this.buffer.Slice(offset, len));

    internal void WriteFixedString(int offset, Utf8Span value) => ((Utf8Span) ref value).Span.CopyTo(this.buffer.Slice(offset));

    internal ReadOnlySpan<byte> ReadFixedBinary(int offset, int len) => (ReadOnlySpan<byte>) this.buffer.Slice(offset, len);

    internal void WriteFixedBinary(int offset, ReadOnlySpan<byte> value, int len)
    {
      value.CopyTo(this.buffer.Slice(offset, len));
      if (value.Length >= len)
        return;
      this.buffer.Slice(checked (offset + value.Length), checked (len - value.Length)).Fill((byte) 0);
    }

    internal void WriteFixedBinary(int offset, ReadOnlySequence<byte> value, int len)
    {
      value.CopyTo<byte>(this.buffer.Slice(offset, len));
      if (value.Length >= (long) len)
        return;
      this.buffer.Slice(checked (offset + (int) value.Length), checked (len - (int) value.Length)).Fill((byte) 0);
    }

    internal Utf8Span ReadVariableString(int offset) => this.ReadString(offset, out int _);

    internal void WriteVariableString(int offset, Utf8Span value, bool exists, out int shift)
    {
      int length = ((Utf8Span) ref value).Length;
      this.EnsureVariable(offset, false, length, exists, out int _, out shift);
      this.WriteString(offset, value);
      checked { this.length += shift; }
    }

    internal ReadOnlySpan<byte> ReadVariableBinary(int offset) => this.ReadBinary(offset, out int _);

    internal void WriteVariableBinary(
      int offset,
      ReadOnlySpan<byte> value,
      bool exists,
      out int shift)
    {
      int length = value.Length;
      this.EnsureVariable(offset, false, length, exists, out int _, out shift);
      this.WriteBinary(offset, value);
      checked { this.length += shift; }
    }

    internal void WriteVariableBinary(
      int offset,
      ReadOnlySequence<byte> value,
      bool exists,
      out int shift)
    {
      int length = checked ((int) value.Length);
      this.EnsureVariable(offset, false, length, exists, out int _, out shift);
      this.WriteBinary(offset, value);
      checked { this.length += shift; }
    }

    internal long ReadVariableInt(int offset) => this.Read7BitEncodedInt(offset, out int _);

    internal void WriteVariableInt(int offset, long value, bool exists, out int shift)
    {
      int numBytes = RowBuffer.Count7BitEncodedInt(value);
      this.EnsureVariable(offset, true, numBytes, exists, out int _, out shift);
      this.Write7BitEncodedInt(offset, value);
      checked { this.length += shift; }
    }

    internal ulong ReadVariableUInt(int offset) => this.Read7BitEncodedUInt(offset, out int _);

    internal void WriteVariableUInt(int offset, ulong value, bool exists, out int shift)
    {
      int numBytes = RowBuffer.Count7BitEncodedUInt(value);
      this.EnsureVariable(offset, true, numBytes, exists, out int _, out shift);
      this.Write7BitEncodedUInt(offset, value);
      checked { this.length += shift; }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal LayoutType ReadSparseTypeCode(int offset) => LayoutType.FromCode((LayoutCode) this.ReadUInt8(offset));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void WriteSparseTypeCode(int offset, LayoutCode code) => this.WriteUInt8(offset, (byte) code);

    internal sbyte ReadSparseInt8(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 1);
      return this.ReadInt8(edit.valueOffset);
    }

    internal void WriteSparseInt8(ref RowCursor edit, sbyte value, UpdateOptions options)
    {
      int numBytes = 1;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Int8, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Int8, TypeArgumentList.Empty, metaBytes);
      this.WriteInt8(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal short ReadSparseInt16(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 2);
      return this.ReadInt16(edit.valueOffset);
    }

    internal void WriteSparseInt16(ref RowCursor edit, short value, UpdateOptions options)
    {
      int numBytes = 2;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Int16, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Int16, TypeArgumentList.Empty, metaBytes);
      this.WriteInt16(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal int ReadSparseInt32(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 4);
      return this.ReadInt32(edit.valueOffset);
    }

    internal void WriteSparseInt32(ref RowCursor edit, int value, UpdateOptions options)
    {
      int numBytes = 4;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Int32, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Int32, TypeArgumentList.Empty, metaBytes);
      this.WriteInt32(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal long ReadSparseInt64(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 8);
      return this.ReadInt64(edit.valueOffset);
    }

    internal void WriteSparseInt64(ref RowCursor edit, long value, UpdateOptions options)
    {
      int numBytes = 8;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Int64, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Int64, TypeArgumentList.Empty, metaBytes);
      this.WriteInt64(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal byte ReadSparseUInt8(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 1);
      return this.ReadUInt8(edit.valueOffset);
    }

    internal void WriteSparseUInt8(ref RowCursor edit, byte value, UpdateOptions options)
    {
      int numBytes = 1;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.UInt8, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.UInt8, TypeArgumentList.Empty, metaBytes);
      this.WriteUInt8(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal ushort ReadSparseUInt16(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 2);
      return this.ReadUInt16(edit.valueOffset);
    }

    internal void WriteSparseUInt16(ref RowCursor edit, ushort value, UpdateOptions options)
    {
      int numBytes = 2;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.UInt16, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.UInt16, TypeArgumentList.Empty, metaBytes);
      this.WriteUInt16(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal uint ReadSparseUInt32(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 4);
      return this.ReadUInt32(edit.valueOffset);
    }

    internal void WriteSparseUInt32(ref RowCursor edit, uint value, UpdateOptions options)
    {
      int numBytes = 4;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.UInt32, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.UInt32, TypeArgumentList.Empty, metaBytes);
      this.WriteUInt32(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal ulong ReadSparseUInt64(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 8);
      return this.ReadUInt64(edit.valueOffset);
    }

    internal void WriteSparseUInt64(ref RowCursor edit, ulong value, UpdateOptions options)
    {
      int numBytes = 8;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.UInt64, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.UInt64, TypeArgumentList.Empty, metaBytes);
      this.WriteUInt64(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal long ReadSparseVarInt(ref RowCursor edit)
    {
      int lenInBytes;
      long num = this.Read7BitEncodedInt(edit.valueOffset, out lenInBytes);
      edit.endOffset = checked (edit.valueOffset + lenInBytes);
      return num;
    }

    internal void WriteSparseVarInt(ref RowCursor edit, long value, UpdateOptions options)
    {
      int numBytes = RowBuffer.Count7BitEncodedInt(value);
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.VarInt, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.VarInt, TypeArgumentList.Empty, metaBytes);
      this.Write7BitEncodedInt(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal ulong ReadSparseVarUInt(ref RowCursor edit)
    {
      int lenInBytes;
      long num = (long) this.Read7BitEncodedUInt(edit.valueOffset, out lenInBytes);
      edit.endOffset = checked (edit.valueOffset + lenInBytes);
      return (ulong) num;
    }

    internal void WriteSparseVarUInt(ref RowCursor edit, ulong value, UpdateOptions options)
    {
      int numBytes = RowBuffer.Count7BitEncodedUInt(value);
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.VarUInt, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.VarUInt, TypeArgumentList.Empty, metaBytes);
      this.Write7BitEncodedUInt(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal float ReadSparseFloat32(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 4);
      return this.ReadFloat32(edit.valueOffset);
    }

    internal void WriteSparseFloat32(ref RowCursor edit, float value, UpdateOptions options)
    {
      int numBytes = 4;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Float32, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Float32, TypeArgumentList.Empty, metaBytes);
      this.WriteFloat32(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal double ReadSparseFloat64(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 8);
      return this.ReadFloat64(edit.valueOffset);
    }

    internal void WriteSparseFloat64(ref RowCursor edit, double value, UpdateOptions options)
    {
      int numBytes = 8;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Float64, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Float64, TypeArgumentList.Empty, metaBytes);
      this.WriteFloat64(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal Float128 ReadSparseFloat128(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 16);
      return this.ReadFloat128(edit.valueOffset);
    }

    internal void WriteSparseFloat128(ref RowCursor edit, Float128 value, UpdateOptions options)
    {
      int numBytes = 16;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Float128, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Float128, TypeArgumentList.Empty, metaBytes);
      this.WriteFloat128(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal Decimal ReadSparseDecimal(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 16);
      return this.ReadDecimal(edit.valueOffset);
    }

    internal void WriteSparseDecimal(ref RowCursor edit, Decimal value, UpdateOptions options)
    {
      int numBytes = 16;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Decimal, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Decimal, TypeArgumentList.Empty, metaBytes);
      this.WriteDecimal(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal DateTime ReadSparseDateTime(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 8);
      return this.ReadDateTime(edit.valueOffset);
    }

    internal void WriteSparseDateTime(ref RowCursor edit, DateTime value, UpdateOptions options)
    {
      int numBytes = 8;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.DateTime, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.DateTime, TypeArgumentList.Empty, metaBytes);
      this.WriteDateTime(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal UnixDateTime ReadSparseUnixDateTime(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 8);
      return this.ReadUnixDateTime(edit.valueOffset);
    }

    internal void WriteSparseUnixDateTime(
      ref RowCursor edit,
      UnixDateTime value,
      UpdateOptions options)
    {
      int numBytes = 8;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.UnixDateTime, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.UnixDateTime, TypeArgumentList.Empty, metaBytes);
      this.WriteUnixDateTime(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal Guid ReadSparseGuid(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 16);
      return this.ReadGuid(edit.valueOffset);
    }

    internal void WriteSparseGuid(ref RowCursor edit, Guid value, UpdateOptions options)
    {
      int numBytes = 16;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Guid, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Guid, TypeArgumentList.Empty, metaBytes);
      this.WriteGuid(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal MongoDbObjectId ReadSparseMongoDbObjectId(ref RowCursor edit)
    {
      edit.endOffset = checked (edit.valueOffset + 12);
      return this.ReadMongoDbObjectId(edit.valueOffset);
    }

    internal void WriteSparseMongoDbObjectId(
      ref RowCursor edit,
      MongoDbObjectId value,
      UpdateOptions options)
    {
      int numBytes = 12;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.MongoDbObjectId, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.MongoDbObjectId, TypeArgumentList.Empty, metaBytes);
      this.WriteMongoDbObjectId(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal NullValue ReadSparseNull(ref RowCursor edit)
    {
      edit.endOffset = edit.valueOffset;
      return NullValue.Default;
    }

    internal void WriteSparseNull(ref RowCursor edit, NullValue value, UpdateOptions options)
    {
      int numBytes = 0;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Null, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Null, TypeArgumentList.Empty, metaBytes);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal bool ReadSparseBool(ref RowCursor edit)
    {
      edit.endOffset = edit.valueOffset;
      return edit.cellType == LayoutType.Boolean;
    }

    internal void WriteSparseBool(ref RowCursor edit, bool value, UpdateOptions options)
    {
      int numBytes = 0;
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, value ? (LayoutType) LayoutType.Boolean : (LayoutType) LayoutType.BooleanFalse, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, value ? (LayoutType) LayoutType.Boolean : (LayoutType) LayoutType.BooleanFalse, TypeArgumentList.Empty, metaBytes);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal Utf8Span ReadSparseString(ref RowCursor edit)
    {
      int sizeLenInBytes;
      Utf8Span utf8Span = this.ReadString(edit.valueOffset, out sizeLenInBytes);
      edit.endOffset = checked (edit.valueOffset + sizeLenInBytes + ((Utf8Span) ref utf8Span).Length);
      return utf8Span;
    }

    internal void WriteSparseString(ref RowCursor edit, Utf8Span value, UpdateOptions options)
    {
      int length;
      int num = RowBuffer.Count7BitEncodedUInt((ulong) (length = ((Utf8Span) ref value).Length));
      int numBytes = checked (length + num);
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Utf8, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Utf8, TypeArgumentList.Empty, metaBytes);
      this.WriteString(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal ReadOnlySpan<byte> ReadSparseBinary(ref RowCursor edit)
    {
      int sizeLenInBytes;
      ReadOnlySpan<byte> readOnlySpan = this.ReadBinary(edit.valueOffset, out sizeLenInBytes);
      edit.endOffset = checked (edit.valueOffset + sizeLenInBytes + readOnlySpan.Length);
      return readOnlySpan;
    }

    internal void WriteSparseBinary(
      ref RowCursor edit,
      ReadOnlySpan<byte> value,
      UpdateOptions options)
    {
      int length;
      int num = RowBuffer.Count7BitEncodedUInt((ulong) (length = value.Length));
      int numBytes = checked (length + num);
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Binary, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Binary, TypeArgumentList.Empty, metaBytes);
      this.WriteBinary(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal void WriteSparseBinary(
      ref RowCursor edit,
      ReadOnlySequence<byte> value,
      UpdateOptions options)
    {
      int length;
      int num = RowBuffer.Count7BitEncodedUInt((ulong) (length = checked ((int) value.Length)));
      int numBytes = checked (length + num);
      int metaBytes;
      int spaceNeeded;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) LayoutType.Binary, TypeArgumentList.Empty, numBytes, options, out metaBytes, out spaceNeeded, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) LayoutType.Binary, TypeArgumentList.Empty, metaBytes);
      this.WriteBinary(edit.valueOffset, value);
      edit.endOffset = checked (edit.metaOffset + spaceNeeded);
      checked { this.length += shift; }
    }

    internal void WriteSparseObject(
      ref RowCursor edit,
      LayoutScope scopeType,
      UpdateOptions options,
      out RowCursor newScope)
    {
      int numBytes = 1;
      TypeArgumentList empty = TypeArgumentList.Empty;
      int metaBytes;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) scopeType, empty, numBytes, options, out metaBytes, out int _, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) scopeType, TypeArgumentList.Empty, metaBytes);
      this.WriteSparseTypeCode(edit.valueOffset, LayoutCode.EndScope);
      newScope = new RowCursor()
      {
        scopeType = scopeType,
        scopeTypeArgs = TypeArgumentList.Empty,
        start = edit.valueOffset,
        valueOffset = edit.valueOffset,
        metaOffset = edit.valueOffset,
        layout = edit.layout
      };
      checked { this.length += shift; }
    }

    internal void WriteSparseArray(
      ref RowCursor edit,
      LayoutScope scopeType,
      UpdateOptions options,
      out RowCursor newScope)
    {
      int numBytes = 1;
      TypeArgumentList empty = TypeArgumentList.Empty;
      int metaBytes;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) scopeType, empty, numBytes, options, out metaBytes, out int _, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) scopeType, empty, metaBytes);
      this.WriteSparseTypeCode(edit.valueOffset, LayoutCode.EndScope);
      newScope = new RowCursor()
      {
        scopeType = scopeType,
        scopeTypeArgs = empty,
        start = edit.valueOffset,
        valueOffset = edit.valueOffset,
        metaOffset = edit.valueOffset,
        layout = edit.layout
      };
      checked { this.length += shift; }
    }

    internal void WriteTypedArray(
      ref RowCursor edit,
      LayoutScope scopeType,
      TypeArgumentList typeArgs,
      UpdateOptions options,
      out RowCursor newScope)
    {
      int numBytes = 4;
      int metaBytes;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) scopeType, typeArgs, numBytes, options, out metaBytes, out int _, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) scopeType, typeArgs, metaBytes);
      this.WriteUInt32(edit.valueOffset, 0U);
      int num = checked (edit.valueOffset + 4);
      newScope = new RowCursor()
      {
        scopeType = scopeType,
        scopeTypeArgs = typeArgs,
        start = edit.valueOffset,
        valueOffset = num,
        metaOffset = num,
        layout = edit.layout
      };
      checked { this.length += shift; }
    }

    internal void WriteTypedSet(
      ref RowCursor edit,
      LayoutScope scopeType,
      TypeArgumentList typeArgs,
      UpdateOptions options,
      out RowCursor newScope)
    {
      int numBytes = 4;
      int metaBytes;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) scopeType, typeArgs, numBytes, options, out metaBytes, out int _, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) scopeType, typeArgs, metaBytes);
      this.WriteUInt32(edit.valueOffset, 0U);
      int num = checked (edit.valueOffset + 4);
      newScope = new RowCursor()
      {
        scopeType = scopeType,
        scopeTypeArgs = typeArgs,
        start = edit.valueOffset,
        valueOffset = num,
        metaOffset = num,
        layout = edit.layout
      };
      checked { this.length += shift; }
    }

    internal void WriteTypedMap(
      ref RowCursor edit,
      LayoutScope scopeType,
      TypeArgumentList typeArgs,
      UpdateOptions options,
      out RowCursor newScope)
    {
      int numBytes = 4;
      int metaBytes;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) scopeType, typeArgs, numBytes, options, out metaBytes, out int _, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) scopeType, typeArgs, metaBytes);
      this.WriteUInt32(edit.valueOffset, 0U);
      int num = checked (edit.valueOffset + 4);
      newScope = new RowCursor()
      {
        scopeType = scopeType,
        scopeTypeArgs = typeArgs,
        start = edit.valueOffset,
        valueOffset = num,
        metaOffset = num,
        layout = edit.layout
      };
      checked { this.length += shift; }
    }

    internal void WriteSparseTuple(
      ref RowCursor edit,
      LayoutScope scopeType,
      TypeArgumentList typeArgs,
      UpdateOptions options,
      out RowCursor newScope)
    {
      int numBytes = checked (1 + typeArgs.Count);
      int metaBytes;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) scopeType, typeArgs, numBytes, options, out metaBytes, out int _, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) scopeType, typeArgs, metaBytes);
      int valueOffset = edit.valueOffset;
      int num = 0;
      while (num < typeArgs.Count)
      {
        this.WriteSparseTypeCode(valueOffset, LayoutCode.Null);
        checked { ++valueOffset; }
        checked { ++num; }
      }
      this.WriteSparseTypeCode(valueOffset, LayoutCode.EndScope);
      newScope = new RowCursor()
      {
        scopeType = scopeType,
        scopeTypeArgs = typeArgs,
        start = edit.valueOffset,
        valueOffset = edit.valueOffset,
        metaOffset = edit.valueOffset,
        layout = edit.layout,
        count = typeArgs.Count
      };
      checked { this.length += shift; }
    }

    internal void WriteTypedTuple(
      ref RowCursor edit,
      LayoutScope scopeType,
      TypeArgumentList typeArgs,
      UpdateOptions options,
      out RowCursor newScope)
    {
      int numBytes = this.CountDefaultValue((LayoutType) scopeType, typeArgs);
      int metaBytes;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) scopeType, typeArgs, numBytes, options, out metaBytes, out int _, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) scopeType, typeArgs, metaBytes);
      this.WriteDefaultValue(edit.valueOffset, (LayoutType) scopeType, typeArgs);
      newScope = new RowCursor()
      {
        scopeType = scopeType,
        scopeTypeArgs = typeArgs,
        start = edit.valueOffset,
        valueOffset = edit.valueOffset,
        metaOffset = edit.valueOffset,
        layout = edit.layout,
        count = typeArgs.Count
      };
      checked { this.length += shift; }
      newScope.MoveNext(ref this);
    }

    internal void WriteNullable(
      ref RowCursor edit,
      LayoutScope scopeType,
      TypeArgumentList typeArgs,
      UpdateOptions options,
      bool hasValue,
      out RowCursor newScope)
    {
      int numBytes = this.CountDefaultValue((LayoutType) scopeType, typeArgs);
      int metaBytes;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) scopeType, typeArgs, numBytes, options, out metaBytes, out int _, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) scopeType, typeArgs, metaBytes);
      this.WriteDefaultValue(edit.valueOffset, (LayoutType) scopeType, typeArgs);
      if (hasValue)
        this.WriteInt8(edit.valueOffset, (sbyte) 1);
      int num = checked (edit.valueOffset + 1);
      newScope = new RowCursor()
      {
        scopeType = scopeType,
        scopeTypeArgs = typeArgs,
        start = edit.valueOffset,
        valueOffset = num,
        metaOffset = num,
        layout = edit.layout,
        count = 2,
        index = 1
      };
      checked { this.length += shift; }
      newScope.MoveNext(ref this);
    }

    internal void WriteSparseUDT(
      ref RowCursor edit,
      LayoutScope scopeType,
      Layout udt,
      UpdateOptions options,
      out RowCursor newScope)
    {
      TypeArgumentList typeArgs = new TypeArgumentList(udt.SchemaId);
      int numBytes = checked (udt.Size + 1);
      int metaBytes;
      int shift;
      this.EnsureSparse(ref edit, (LayoutType) scopeType, typeArgs, numBytes, options, out metaBytes, out int _, out shift);
      this.WriteSparseMetadata(ref edit, (LayoutType) scopeType, typeArgs, metaBytes);
      this.buffer.Slice(edit.valueOffset, udt.Size).Fill((byte) 0);
      int offset = checked (edit.valueOffset + udt.Size);
      this.WriteSparseTypeCode(offset, LayoutCode.EndScope);
      newScope = new RowCursor()
      {
        scopeType = scopeType,
        scopeTypeArgs = typeArgs,
        start = edit.valueOffset,
        valueOffset = offset,
        metaOffset = offset,
        layout = udt
      };
      checked { this.length += shift; }
    }

    internal void DeleteSparse(ref RowCursor edit)
    {
      if (!edit.exists)
        return;
      int numBytes = 0;
      int shift;
      this.EnsureSparse(ref edit, edit.cellType, edit.cellTypeArgs, numBytes, RowOptions.Delete, out int _, out int _, out shift);
      checked { this.length += shift; }
    }

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:DocumentationTextMustEndWithAPeriod", Justification = "Colon.")]
    internal static ulong RotateSignToLsb(long value)
    {
      int num1 = value < 0L ? 1 : 0;
      ulong num2 = (ulong) value;
      return num1 != 0 ? (ulong) ((~(long) num2 + 1L << 1) + 1L) : num2 << 1;
    }

    internal static long RotateSignToMsb(ulong uvalue) => uvalue % 2UL <= 0UL ? (long) (uvalue >> 1) : ~(long) (uvalue >> 1) + 1L | long.MinValue;

    internal int ComputeVariableValueOffset(Layout layout, int scopeOffset, int varIndex)
    {
      if (layout == null)
        return scopeOffset;
      int num1 = checked (layout.NumFixed + varIndex);
      Layout.ColumnView columns = layout.Columns;
      int offset = checked (scopeOffset + layout.Size);
      int numFixed = layout.NumFixed;
      while (numFixed < num1)
      {
        LayoutColumn layoutColumn = columns[numFixed];
        if (this.ReadBit(scopeOffset, layoutColumn.NullBit))
        {
          int lenInBytes;
          ulong num2 = this.Read7BitEncodedUInt(offset, out lenInBytes);
          if (layoutColumn.Type.IsVarint)
            checked { offset += lenInBytes; }
          else
            checked { offset += (int) num2 + lenInBytes; }
        }
        checked { ++numFixed; }
      }
      return offset;
    }

    internal bool SparseIteratorMoveNext(ref RowCursor edit)
    {
      if (edit.cellType != null)
      {
        if (edit.scopeType.IsIndexedScope)
          checked { ++edit.index; }
        if (edit.endOffset != 0)
        {
          edit.metaOffset = edit.endOffset;
          edit.endOffset = 0;
        }
        else
          checked { edit.metaOffset += this.SparseComputeSize(ref edit); }
      }
      if (edit.metaOffset < this.length && (!edit.scopeType.IsSizedScope || edit.index != edit.count))
      {
        this.ReadSparseMetadata(ref edit);
        if (!(edit.cellType is LayoutEndScope))
        {
          edit.exists = true;
          return true;
        }
      }
      edit.cellType = (LayoutType) LayoutType.EndScope;
      edit.exists = false;
      edit.valueOffset = edit.metaOffset;
      return false;
    }

    internal RowCursor SparseIteratorReadScope(ref RowCursor edit, bool immutable)
    {
      LayoutScope cellType = edit.cellType as LayoutScope;
      switch (cellType)
      {
        case LayoutObject _:
        case LayoutArray _:
          return new RowCursor()
          {
            scopeType = cellType,
            scopeTypeArgs = edit.cellTypeArgs,
            start = edit.valueOffset,
            valueOffset = edit.valueOffset,
            metaOffset = edit.valueOffset,
            layout = edit.layout,
            immutable = immutable
          };
        case LayoutTypedArray _:
        case LayoutTypedSet _:
        case LayoutTypedMap _:
          int num1 = checked (edit.valueOffset + 4);
          return new RowCursor()
          {
            scopeType = cellType,
            scopeTypeArgs = edit.cellTypeArgs,
            start = edit.valueOffset,
            valueOffset = num1,
            metaOffset = num1,
            layout = edit.layout,
            immutable = immutable,
            count = checked ((int) this.ReadUInt32(edit.valueOffset))
          };
        case LayoutTypedTuple _:
        case LayoutTuple _:
        case LayoutTagged _:
        case LayoutTagged2 _:
          return new RowCursor()
          {
            scopeType = cellType,
            scopeTypeArgs = edit.cellTypeArgs,
            start = edit.valueOffset,
            valueOffset = edit.valueOffset,
            metaOffset = edit.valueOffset,
            layout = edit.layout,
            immutable = immutable,
            count = edit.cellTypeArgs.Count
          };
        case LayoutNullable _:
          if (this.ReadInt8(edit.valueOffset) != (sbyte) 0)
          {
            int num2 = checked (edit.valueOffset + 1);
            return new RowCursor()
            {
              scopeType = cellType,
              scopeTypeArgs = edit.cellTypeArgs,
              start = edit.valueOffset,
              valueOffset = num2,
              metaOffset = num2,
              layout = edit.layout,
              immutable = immutable,
              count = 2,
              index = 1
            };
          }
          TypeArgument cellTypeArg = edit.cellTypeArgs[0];
          int num3 = checked (edit.valueOffset + 1 + this.CountDefaultValue(cellTypeArg.Type, cellTypeArg.TypeArgs));
          return new RowCursor()
          {
            scopeType = cellType,
            scopeTypeArgs = edit.cellTypeArgs,
            start = edit.valueOffset,
            valueOffset = num3,
            metaOffset = num3,
            layout = edit.layout,
            immutable = immutable,
            count = 2,
            index = 2
          };
        case LayoutUDT _:
          Layout layout = this.resolver.Resolve(edit.cellTypeArgs.SchemaId);
          int variableValueOffset = this.ComputeVariableValueOffset(layout, edit.valueOffset, layout.NumVariable);
          return new RowCursor()
          {
            scopeType = cellType,
            scopeTypeArgs = edit.cellTypeArgs,
            start = edit.valueOffset,
            valueOffset = variableValueOffset,
            metaOffset = variableValueOffset,
            layout = layout,
            immutable = immutable
          };
        default:
          Contract.Fail("Not a scope type.");
          return new RowCursor();
      }
    }

    internal RowCursor PrepareSparseMove(ref RowCursor scope, ref RowCursor srcEdit)
    {
      Contract.Requires(scope.scopeType.IsUniqueScope);
      Contract.Requires(scope.index == 0);
      RowCursor dest;
      scope.Clone(out dest);
      dest.metaOffset = scope.valueOffset;
      int leftLen = checked (this.SparseComputeSize(ref srcEdit) - srcEdit.valueOffset - srcEdit.metaOffset);
      while (dest.index < dest.count)
      {
        this.ReadSparseMetadata(ref dest);
        int num1 = -1;
        int num2;
        if (scope.scopeType is LayoutTypedMap)
        {
          num2 = this.CompareKeyValueFieldValue(srcEdit, dest);
        }
        else
        {
          num1 = this.SparseComputeSize(ref dest);
          int rightLen = checked (num1 - dest.valueOffset - dest.metaOffset);
          num2 = this.CompareFieldValue(srcEdit, leftLen, dest, rightLen);
        }
        if (num2 <= 0)
        {
          dest.exists = num2 == 0;
          return dest;
        }
        int num3 = num1 == -1 ? this.SparseComputeSize(ref dest) : num1;
        checked { ++dest.index; }
        checked { dest.metaOffset += num3; }
      }
      dest.exists = false;
      dest.cellType = (LayoutType) LayoutType.EndScope;
      dest.valueOffset = dest.metaOffset;
      return dest;
    }

    internal void TypedCollectionMoveField(
      ref RowCursor dstEdit,
      ref RowCursor srcEdit,
      RowOptions options)
    {
      int num = checked (this.SparseComputeSize(ref srcEdit) - srcEdit.valueOffset - srcEdit.metaOffset);
      int metaBytes;
      int spaceNeeded;
      int shift1;
      this.EnsureSparse(ref dstEdit, srcEdit.cellType, srcEdit.cellTypeArgs, num, options, out metaBytes, out spaceNeeded, out shift1);
      this.WriteSparseMetadata(ref dstEdit, srcEdit.cellType, srcEdit.cellTypeArgs, metaBytes);
      if (srcEdit.metaOffset >= dstEdit.metaOffset)
      {
        checked { srcEdit.metaOffset += shift1; }
        checked { srcEdit.valueOffset += shift1; }
      }
      this.buffer.Slice(srcEdit.valueOffset, num).CopyTo(this.buffer.Slice(dstEdit.valueOffset));
      checked { this.length += shift1; }
      int shift2;
      this.EnsureSparse(ref srcEdit, srcEdit.cellType, srcEdit.cellTypeArgs, num, RowOptions.Delete, out metaBytes, out spaceNeeded, out shift2);
      checked { this.length += shift2; }
    }

    internal unsafe Result TypedCollectionUniqueIndexRebuild(ref RowCursor scope)
    {
      Contract.Requires(scope.scopeType.IsUniqueScope);
      Contract.Requires(scope.index == 0);
      RowCursor dest;
      scope.Clone(out dest);
      if (dest.count <= 1)
        return Result.Success;
      Span<RowBuffer.UniqueIndexItem> span1;
      if (dest.count < 100)
      {
        int count = dest.count;
        // ISSUE: untyped stack allocation
        span1 = new Span<RowBuffer.UniqueIndexItem>((void*) __untypedstackalloc(checked (unchecked ((IntPtr) (uint) count) * sizeof (RowBuffer.UniqueIndexItem))), count);
      }
      else
        span1 = (Span<RowBuffer.UniqueIndexItem>) new RowBuffer.UniqueIndexItem[dest.count];
      Span<RowBuffer.UniqueIndexItem> span2 = span1;
      dest.metaOffset = scope.valueOffset;
      while (dest.index < dest.count)
      {
        this.ReadSparseMetadata(ref dest);
        int size = this.SparseComputeSize(ref dest);
        span2[dest.index] = new RowBuffer.UniqueIndexItem()
        {
          Code = dest.cellType.LayoutCode,
          MetaOffset = dest.metaOffset,
          ValueOffset = dest.valueOffset,
          Size = size
        };
        checked { dest.metaOffset += size; }
        checked { ++dest.index; }
      }
      int num = checked (dest.metaOffset - scope.valueOffset);
      Span<RowBuffer.UniqueIndexItem> uniqueIndex = new Span<RowBuffer.UniqueIndexItem>(Unsafe.AsPointer<RowBuffer.UniqueIndexItem>(ref span2.GetPinnableReference()), span2.Length);
      if (!this.InsertionSort(ref scope, ref dest, uniqueIndex))
        return Result.Exists;
      int valueOffset = scope.valueOffset;
      this.Ensure(checked (this.length + num));
      Span<byte> span3 = this.buffer.Slice(valueOffset, checked (this.length - valueOffset));
      span3.CopyTo(this.buffer.Slice(checked (valueOffset + num)));
      Span<RowBuffer.UniqueIndexItem> span4 = span2;
      for (int index = 0; index < span4.Length; ++index)
      {
        RowBuffer.UniqueIndexItem uniqueIndexItem = span4[index];
        span3 = this.buffer.Slice(checked (uniqueIndexItem.MetaOffset + num), uniqueIndexItem.Size);
        span3.CopyTo(this.buffer.Slice(valueOffset));
        checked { valueOffset += uniqueIndexItem.Size; }
      }
      if (valueOffset != this.length)
      {
        span3 = this.buffer.Slice(checked (valueOffset + num), checked (this.length - valueOffset));
        span3.CopyTo(this.buffer.Slice(valueOffset));
      }
      return Result.Success;
    }

    private static int CountSparsePath(ref RowCursor edit)
    {
      if (!edit.writePathToken.IsNull || edit.layout.Tokenizer.TryFindToken(edit.writePath, out edit.writePathToken))
        return edit.writePathToken.Varint.Length;
      int length = ((UtfAnyString) ref edit.writePath).ToUtf8String().Length;
      return checked (RowBuffer.Count7BitEncodedUInt((ulong) (edit.layout.Tokenizer.Count + length)) + length);
    }

    internal static int Count7BitEncodedUInt(ulong value)
    {
      int num = 0;
      for (; value >= 128UL; value >>= 7)
        checked { ++num; }
      return checked (num + 1);
    }

    private static int Count7BitEncodedInt(long value) => RowBuffer.Count7BitEncodedUInt(RowBuffer.RotateSignToLsb(value));

    private bool InitReadFrom(HybridRowVersion rowVersion)
    {
      HybridRowHeader hybridRowHeader = this.ReadHeader(0);
      Layout layout = this.resolver.Resolve(hybridRowHeader.SchemaId);
      return hybridRowHeader.Version == rowVersion && checked (5 + layout.Size) <= this.length;
    }

    private int SkipScope(ref RowCursor edit)
    {
      do
        ;
      while (this.SparseIteratorMoveNext(ref edit));
      if (!edit.scopeType.IsSizedScope)
        checked { ++edit.metaOffset; }
      return edit.metaOffset;
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201", Justification = "Logical Grouping.")]
    private int CompareFieldValue(RowCursor left, int leftLen, RowCursor right, int rightLen)
    {
      if (left.cellType.LayoutCode < right.cellType.LayoutCode)
        return -1;
      if (left.cellType == right.cellType)
      {
        if (leftLen < rightLen)
          return -1;
        if (leftLen == rightLen)
          return this.buffer.Slice(left.valueOffset, leftLen).SequenceCompareTo<byte>((ReadOnlySpan<byte>) this.buffer.Slice(right.valueOffset, rightLen));
      }
      return 1;
    }

    private int CompareKeyValueFieldValue(RowCursor left, RowCursor right)
    {
      LayoutTypedTuple cellType1 = left.cellType as LayoutTypedTuple;
      LayoutTypedTuple cellType2 = right.cellType as LayoutTypedTuple;
      Contract.Requires(cellType1 != null);
      Contract.Requires(cellType2 != null);
      Contract.Requires(left.cellTypeArgs.Count == 2);
      Contract.Requires(left.cellTypeArgs.Equals(right.cellTypeArgs));
      RowCursor edit1 = new RowCursor()
      {
        layout = left.layout,
        scopeType = (LayoutScope) cellType1,
        scopeTypeArgs = left.cellTypeArgs,
        start = left.valueOffset,
        metaOffset = left.valueOffset,
        index = 0
      };
      this.ReadSparseMetadata(ref edit1);
      int leftLen = checked (this.SparseComputeSize(ref edit1) - edit1.valueOffset - edit1.metaOffset);
      RowCursor edit2 = new RowCursor()
      {
        layout = right.layout,
        scopeType = (LayoutScope) cellType2,
        scopeTypeArgs = right.cellTypeArgs,
        start = right.valueOffset,
        metaOffset = right.valueOffset,
        index = 0
      };
      this.ReadSparseMetadata(ref edit2);
      int rightLen = checked (this.SparseComputeSize(ref edit2) - edit2.valueOffset - edit2.metaOffset);
      return this.CompareFieldValue(edit1, leftLen, edit2, rightLen);
    }

    private bool InsertionSort(
      ref RowCursor scope,
      ref RowCursor dstEdit,
      Span<RowBuffer.UniqueIndexItem> uniqueIndex)
    {
      RowCursor left = dstEdit;
      RowCursor right = dstEdit;
      int index1 = 1;
      while (index1 < uniqueIndex.Length)
      {
        RowBuffer.UniqueIndexItem uniqueIndexItem1 = uniqueIndex[index1];
        left.cellType = LayoutType.FromCode(uniqueIndexItem1.Code);
        left.metaOffset = uniqueIndexItem1.MetaOffset;
        left.valueOffset = uniqueIndexItem1.ValueOffset;
        int leftLen = checked (uniqueIndexItem1.Size - uniqueIndexItem1.ValueOffset - uniqueIndexItem1.MetaOffset);
        int index2 = checked (index1 - 1);
        while (index2 >= 0)
        {
          RowBuffer.UniqueIndexItem uniqueIndexItem2 = uniqueIndex[index2];
          right.cellType = LayoutType.FromCode(uniqueIndexItem2.Code);
          right.metaOffset = uniqueIndexItem2.MetaOffset;
          right.valueOffset = uniqueIndexItem2.ValueOffset;
          int num;
          if (scope.scopeType is LayoutTypedMap)
          {
            num = this.CompareKeyValueFieldValue(left, right);
          }
          else
          {
            int rightLen = checked (uniqueIndexItem2.Size - uniqueIndexItem2.ValueOffset - uniqueIndexItem2.MetaOffset);
            num = this.CompareFieldValue(left, leftLen, right, rightLen);
          }
          if (num == 0)
            return false;
          if (num <= 0)
          {
            uniqueIndex[checked (index2 + 1)] = uniqueIndex[index2];
            checked { --index2; }
          }
          else
            break;
        }
        uniqueIndex[checked (index2 + 1)] = uniqueIndexItem1;
        checked { ++index1; }
      }
      return true;
    }

    internal int ReadSparsePathLen(
      Layout layout,
      int offset,
      out int pathLenInBytes,
      out int pathOffset)
    {
      int lenInBytes;
      int num1 = checked ((int) this.Read7BitEncodedUInt(offset, out lenInBytes));
      if (num1 < layout.Tokenizer.Count)
      {
        pathLenInBytes = lenInBytes;
        pathOffset = offset;
        return num1;
      }
      int num2 = checked (num1 - layout.Tokenizer.Count);
      pathLenInBytes = checked (num2 + lenInBytes);
      pathOffset = checked (offset + lenInBytes);
      return num1;
    }

    internal Utf8Span ReadSparsePath(ref RowCursor edit)
    {
      Utf8String path;
      if (edit.layout.Tokenizer.TryFindString(checked ((ulong) edit.pathToken), out path))
        return path.Span;
      int length = checked (edit.pathToken - edit.layout.Tokenizer.Count);
      return Utf8Span.UnsafeFromUtf8BytesNoValidation((ReadOnlySpan<byte>) this.buffer.Slice(edit.pathOffset, length));
    }

    private void WriteSparsePath(ref RowCursor edit, int offset)
    {
      if (edit.scopeType.IsIndexedScope)
      {
        edit.pathToken = 0;
        edit.pathOffset = 0;
      }
      else if (!edit.writePathToken.IsNull)
      {
        edit.writePathToken.Varint.CopyTo<byte>(this.buffer.Slice(offset));
        edit.pathToken = checked ((int) edit.writePathToken.Id);
        edit.pathOffset = offset;
      }
      else
      {
        Utf8Span utf8Span = Utf8String.op_Implicit(((UtfAnyString) ref edit.writePath).ToUtf8String());
        edit.pathToken = checked (edit.layout.Tokenizer.Count + ((Utf8Span) ref utf8Span).Length);
        int num = this.Write7BitEncodedUInt(offset, checked ((ulong) edit.pathToken));
        edit.pathOffset = checked (offset + num);
        ((Utf8Span) ref utf8Span).Span.CopyTo(this.buffer.Slice(checked (offset + num)));
      }
    }

    private Utf8Span ReadString(int offset, out int sizeLenInBytes)
    {
      int length = checked ((int) this.Read7BitEncodedUInt(offset, out sizeLenInBytes));
      return Utf8Span.UnsafeFromUtf8BytesNoValidation((ReadOnlySpan<byte>) this.buffer.Slice(checked (offset + sizeLenInBytes), length));
    }

    private int WriteString(int offset, Utf8Span value)
    {
      int num = this.Write7BitEncodedUInt(offset, checked ((ulong) ((Utf8Span) ref value).Length));
      ((Utf8Span) ref value).Span.CopyTo(this.buffer.Slice(checked (offset + num)));
      return num;
    }

    private ReadOnlySpan<byte> ReadBinary(int offset, out int sizeLenInBytes)
    {
      int length = checked ((int) this.Read7BitEncodedUInt(offset, out sizeLenInBytes));
      return (ReadOnlySpan<byte>) this.buffer.Slice(checked (offset + sizeLenInBytes), length);
    }

    private int WriteBinary(int offset, ReadOnlySpan<byte> value)
    {
      int num = this.Write7BitEncodedUInt(offset, checked ((ulong) value.Length));
      value.CopyTo(this.buffer.Slice(checked (offset + num)));
      return num;
    }

    private int WriteBinary(int offset, ReadOnlySequence<byte> value)
    {
      int num = this.Write7BitEncodedUInt(offset, checked ((ulong) value.Length));
      value.CopyTo<byte>(this.buffer.Slice(checked (offset + num)));
      return num;
    }

    private void Ensure(int size)
    {
      if (this.buffer.Length >= size)
        return;
      this.buffer = this.resizer.Resize(size, this.buffer);
    }

    private void EnsureVariable(
      int offset,
      bool isVarint,
      int numBytes,
      bool exists,
      out int spaceNeeded,
      out int shift)
    {
      int lenInBytes = 0;
      ulong num = 0;
      if (exists)
        num = this.Read7BitEncodedUInt(offset, out lenInBytes);
      if (isVarint)
      {
        spaceNeeded = numBytes;
      }
      else
      {
        checked { lenInBytes += (int) num; }
        spaceNeeded = checked (numBytes + RowBuffer.Count7BitEncodedUInt((ulong) numBytes));
      }
      shift = checked (spaceNeeded - lenInBytes);
      if (shift > 0)
      {
        this.Ensure(checked (this.length + shift));
        this.buffer.Slice(checked (offset + lenInBytes), checked (this.length - offset + lenInBytes)).CopyTo(this.buffer.Slice(checked (offset + spaceNeeded)));
      }
      else
      {
        if (shift >= 0)
          return;
        this.buffer.Slice(checked (offset + lenInBytes), checked (this.length - offset + lenInBytes)).CopyTo(this.buffer.Slice(checked (offset + spaceNeeded)));
      }
    }

    [Conditional("DEBUG")]
    private void ValidateSparsePrimitiveTypeCode(ref RowCursor edit, LayoutType code)
    {
      if (edit.scopeType.HasImplicitTypeCode(ref edit))
      {
        if (edit.scopeType is LayoutNullable || !edit.scopeType.IsFixedArity)
          ;
      }
      else if (code == LayoutType.Boolean)
        code = this.ReadSparseTypeCode(edit.metaOffset);
      if (edit.scopeType.IsIndexedScope)
        return;
      this.ReadSparsePathLen(edit.layout, checked (edit.metaOffset + 1), out int _, out int _);
    }

    private void WriteSparseMetadata(
      ref RowCursor edit,
      LayoutType cellType,
      TypeArgumentList typeArgs,
      int metaBytes)
    {
      int metaOffset = edit.metaOffset;
      if (!edit.scopeType.HasImplicitTypeCode(ref edit))
        checked { metaOffset += cellType.WriteTypeArgument(ref this, metaOffset, typeArgs); }
      this.WriteSparsePath(ref edit, metaOffset);
      edit.valueOffset = checked (edit.metaOffset + metaBytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureSparse(
      ref RowCursor edit,
      LayoutType cellType,
      TypeArgumentList typeArgs,
      int numBytes,
      UpdateOptions options,
      out int metaBytes,
      out int spaceNeeded,
      out int shift)
    {
      this.EnsureSparse(ref edit, cellType, typeArgs, numBytes, (RowOptions) options, out metaBytes, out spaceNeeded, out shift);
    }

    private void EnsureSparse(
      ref RowCursor edit,
      LayoutType cellType,
      TypeArgumentList typeArgs,
      int numBytes,
      RowOptions options,
      out int metaBytes,
      out int spaceNeeded,
      out int shift)
    {
      int metaOffset = edit.metaOffset;
      int num1 = 0;
      metaBytes = !edit.scopeType.HasImplicitTypeCode(ref edit) ? cellType.CountTypeArgument(typeArgs) : 0;
      if (!edit.scopeType.IsIndexedScope)
      {
        int num2 = RowBuffer.CountSparsePath(ref edit);
        checked { metaBytes += num2; }
      }
      if (edit.exists)
        num1 = this.SparseComputeSize(ref edit);
      spaceNeeded = options == RowOptions.Delete ? 0 : checked (metaBytes + numBytes);
      shift = checked (spaceNeeded - num1);
      if (shift > 0)
        this.Ensure(checked (this.length + shift));
      this.buffer.Slice(checked (metaOffset + num1), checked (this.length - metaOffset + num1)).CopyTo(this.buffer.Slice(checked (metaOffset + spaceNeeded)));
      if (edit.scopeType.IsSizedScope && !edit.scopeType.IsFixedArity)
      {
        if (options == RowOptions.Insert || options == RowOptions.InsertAt || options == RowOptions.Upsert && !edit.exists)
        {
          this.IncrementUInt32(edit.start, 1U);
          checked { ++edit.count; }
        }
        else if (options == RowOptions.Delete && edit.exists)
        {
          this.DecrementUInt32(edit.start, 1U);
          checked { --edit.count; }
        }
      }
      if (options == RowOptions.Delete)
      {
        edit.cellType = (LayoutType) null;
        edit.cellTypeArgs = new TypeArgumentList();
        edit.exists = false;
      }
      else
      {
        edit.cellType = cellType;
        edit.cellTypeArgs = typeArgs;
        edit.exists = true;
      }
    }

    private void ReadSparseMetadata(ref RowCursor edit)
    {
      if (edit.scopeType.HasImplicitTypeCode(ref edit))
      {
        edit.scopeType.SetImplicitTypeCode(ref edit);
        edit.valueOffset = edit.metaOffset;
      }
      else
      {
        edit.cellType = this.ReadSparseTypeCode(edit.metaOffset);
        edit.valueOffset = checked (edit.metaOffset + 1);
        edit.cellTypeArgs = TypeArgumentList.Empty;
        if (edit.cellType is LayoutEndScope)
        {
          edit.pathToken = 0;
          edit.pathOffset = 0;
          edit.valueOffset = edit.metaOffset;
          return;
        }
        int lenInBytes;
        edit.cellTypeArgs = edit.cellType.ReadTypeArgumentList(ref this, edit.valueOffset, out lenInBytes);
        checked { edit.valueOffset += lenInBytes; }
      }
      edit.scopeType.ReadSparsePath(ref this, ref edit);
    }

    private int SparseComputeSize(ref RowCursor edit)
    {
      if (!(edit.cellType is LayoutScope))
        return this.SparseComputePrimitiveSize(edit.cellType, edit.metaOffset, edit.valueOffset);
      RowCursor edit1 = this.SparseIteratorReadScope(ref edit, true);
      return checked (this.SkipScope(ref edit1) - edit.metaOffset);
    }

    private int SparseComputePrimitiveSize(LayoutType cellType, int metaOffset, int valueOffset)
    {
      int primitiveSize = checked (valueOffset - metaOffset);
      LayoutCode layoutCode = cellType.LayoutCode;
      switch (layoutCode)
      {
        case LayoutCode.Null:
          return primitiveSize;
        case LayoutCode.BooleanFalse:
        case LayoutCode.Boolean:
          return primitiveSize;
        case LayoutCode.Int8:
          return checked (primitiveSize + LayoutType.Int8.Size);
        case LayoutCode.Int16:
          return checked (primitiveSize + LayoutType.Int16.Size);
        case LayoutCode.Int32:
          return checked (primitiveSize + LayoutType.Int32.Size);
        case LayoutCode.Int64:
          return checked (primitiveSize + LayoutType.Int64.Size);
        case LayoutCode.UInt8:
          return checked (primitiveSize + LayoutType.UInt8.Size);
        case LayoutCode.UInt16:
          return checked (primitiveSize + LayoutType.UInt16.Size);
        case LayoutCode.UInt32:
          return checked (primitiveSize + LayoutType.UInt32.Size);
        case LayoutCode.UInt64:
          return checked (primitiveSize + LayoutType.UInt64.Size);
        case LayoutCode.VarInt:
        case LayoutCode.VarUInt:
          int lenInBytes1;
          long num1 = (long) this.Read7BitEncodedUInt(checked (metaOffset + primitiveSize), out lenInBytes1);
          return checked (primitiveSize + lenInBytes1);
        case LayoutCode.Float32:
          return checked (primitiveSize + LayoutType.Float32.Size);
        case LayoutCode.Float64:
          return checked (primitiveSize + LayoutType.Float64.Size);
        case LayoutCode.Decimal:
          return checked (primitiveSize + LayoutType.Decimal.Size);
        case LayoutCode.DateTime:
          return checked (primitiveSize + LayoutType.DateTime.Size);
        case LayoutCode.Guid:
          return checked (primitiveSize + LayoutType.Guid.Size);
        case LayoutCode.Utf8:
        case LayoutCode.Binary:
          int lenInBytes2;
          int num2 = checked ((int) this.Read7BitEncodedUInt(metaOffset + primitiveSize, out lenInBytes2));
          return checked (primitiveSize + lenInBytes2 + num2);
        case LayoutCode.Float128:
          return checked (primitiveSize + LayoutType.Float128.Size);
        case LayoutCode.UnixDateTime:
          return checked (primitiveSize + LayoutType.UnixDateTime.Size);
        case LayoutCode.MongoDbObjectId:
          return checked (primitiveSize + LayoutType.MongoDbObjectId.Size);
        default:
          Contract.Fail(string.Format("Not Implemented: {0}", (object) layoutCode));
          return 0;
      }
    }

    private int CountDefaultValue(LayoutType code, TypeArgumentList typeArgs)
    {
      // ISSUE: unable to decompile the method.
    }

    private int WriteDefaultValue(int offset, LayoutType code, TypeArgumentList typeArgs)
    {
      // ISSUE: unable to decompile the method.
    }

    [DebuggerDisplay("{MetaOffset}/{ValueOffset}")]
    private struct UniqueIndexItem
    {
      public LayoutCode Code;
      public int MetaOffset;
      public int ValueOffset;
      public int Size;
    }
  }
}
