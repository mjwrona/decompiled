// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.IO.RowReader
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.IO
{
  public ref struct RowReader
  {
    private readonly int schematizedCount;
    private readonly Layout.ColumnView columns;
    private RowBuffer row;
    private RowReader.States state;
    private int columnIndex;
    private RowCursor cursor;

    private RowReader(ref RowBuffer row, in RowCursor scope)
    {
      this.cursor = scope;
      this.row = row;
      this.columns = this.cursor.layout.Columns;
      this.schematizedCount = checked (this.cursor.layout.NumFixed + this.cursor.layout.NumVariable);
      this.state = RowReader.States.None;
      this.columnIndex = -1;
    }

    public RowReader(ref RowBuffer row)
      : this(ref row, RowCursor.Create(ref row))
    {
    }

    public RowReader(
      ReadOnlyMemory<byte> buffer,
      HybridRowVersion version,
      LayoutResolver resolver)
    {
      this.row = new RowBuffer(MemoryMarshal.AsMemory<byte>(buffer).Span, version, resolver);
      this.cursor = RowCursor.Create(ref this.row);
      this.columns = this.cursor.layout.Columns;
      this.schematizedCount = checked (this.cursor.layout.NumFixed + this.cursor.layout.NumVariable);
      this.state = RowReader.States.None;
      this.columnIndex = -1;
    }

    public RowReader(
      ReadOnlyMemory<byte> buffer,
      HybridRowVersion version,
      LayoutResolver resolver,
      in RowReader.Checkpoint checkpoint)
    {
      this.row = new RowBuffer(MemoryMarshal.AsMemory<byte>(buffer).Span, version, resolver);
      this.columns = checkpoint.Cursor.layout.Columns;
      this.schematizedCount = checked (checkpoint.Cursor.layout.NumFixed + checkpoint.Cursor.layout.NumVariable);
      this.state = checkpoint.State;
      this.cursor = checkpoint.Cursor;
      this.columnIndex = checkpoint.ColumnIndex;
    }

    public RowReader(ref RowBuffer row, in RowReader.Checkpoint checkpoint)
    {
      this.row = row;
      this.columns = checkpoint.Cursor.layout.Columns;
      this.schematizedCount = checked (checkpoint.Cursor.layout.NumFixed + checkpoint.Cursor.layout.NumVariable);
      this.state = checkpoint.State;
      this.cursor = checkpoint.Cursor;
      this.columnIndex = checkpoint.ColumnIndex;
    }

    public int Length => this.row.Length;

    public HybridRowHeader Header => this.row.Header;

    public StorageKind Storage
    {
      get
      {
        switch (this.state)
        {
          case RowReader.States.Schematized:
            return this.columns[this.columnIndex].Storage;
          case RowReader.States.Sparse:
            return StorageKind.Sparse;
          default:
            return StorageKind.Sparse;
        }
      }
    }

    public LayoutType Type
    {
      get
      {
        switch (this.state)
        {
          case RowReader.States.Schematized:
            return this.columns[this.columnIndex].Type;
          case RowReader.States.Sparse:
            return this.cursor.cellType;
          default:
            return (LayoutType) null;
        }
      }
    }

    public TypeArgumentList TypeArgs
    {
      get
      {
        switch (this.state)
        {
          case RowReader.States.Schematized:
            return this.columns[this.columnIndex].TypeArgs;
          case RowReader.States.Sparse:
            return this.cursor.cellTypeArgs;
          default:
            return TypeArgumentList.Empty;
        }
      }
    }

    public bool HasValue
    {
      get
      {
        switch (this.state)
        {
          case RowReader.States.Schematized:
            return true;
          case RowReader.States.Sparse:
            if (!(this.cursor.cellType is LayoutNullable))
              return true;
            RowCursor scope = this.row.SparseIteratorReadScope(ref this.cursor, true);
            return LayoutNullable.HasValue(ref this.row, ref scope) == Result.Success;
          default:
            return false;
        }
      }
    }

    public UtfAnyString Path
    {
      get
      {
        switch (this.state)
        {
          case RowReader.States.Schematized:
            return Utf8String.op_Implicit(this.columns[this.columnIndex].Path);
          case RowReader.States.Sparse:
            return this.cursor.pathOffset == 0 ? new UtfAnyString() : Utf8String.op_Implicit(Utf8String.CopyFrom(this.row.ReadSparsePath(ref this.cursor)));
          default:
            return new UtfAnyString();
        }
      }
    }

    public Utf8Span PathSpan
    {
      get
      {
        switch (this.state)
        {
          case RowReader.States.Schematized:
            return this.columns[this.columnIndex].Path.Span;
          case RowReader.States.Sparse:
            return this.row.ReadSparsePath(ref this.cursor);
          default:
            return new Utf8Span();
        }
      }
    }

    public int Index
    {
      get
      {
        switch (this.state)
        {
          case RowReader.States.Schematized:
            return 0;
          case RowReader.States.Sparse:
            return this.cursor.index;
          default:
            return 0;
        }
      }
    }

    public RowReader.Checkpoint SaveCheckpoint() => new RowReader.Checkpoint(this.state, this.columnIndex, this.cursor);

    public bool Read()
    {
      switch (this.state)
      {
        case RowReader.States.None:
          if (this.cursor.scopeType is LayoutUDT)
          {
            this.state = RowReader.States.Schematized;
            goto case RowReader.States.Schematized;
          }
          else
          {
            this.state = RowReader.States.Sparse;
            goto case RowReader.States.Sparse;
          }
        case RowReader.States.Schematized:
          do
          {
            checked { ++this.columnIndex; }
            if (this.columnIndex >= this.schematizedCount)
            {
              this.state = RowReader.States.Sparse;
              goto case RowReader.States.Sparse;
            }
          }
          while (!this.row.ReadBit(this.cursor.start, this.columns[this.columnIndex].NullBit));
          return true;
        case RowReader.States.Sparse:
          if (this.cursor.MoveNext(ref this.row))
            return true;
          this.state = RowReader.States.Done;
          goto case RowReader.States.Done;
        case RowReader.States.Done:
          return false;
        default:
          return false;
      }
    }

    public Result ReadBool(out bool value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<bool>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutBoolean))
          {
            value = false;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseBool(ref this.cursor);
          return Result.Success;
        default:
          value = false;
          return Result.Failure;
      }
    }

    public Result ReadNull(out NullValue value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<NullValue>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutNull))
          {
            value = new NullValue();
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseNull(ref this.cursor);
          return Result.Success;
        default:
          value = new NullValue();
          return Result.Failure;
      }
    }

    public Result ReadInt8(out sbyte value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<sbyte>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutInt8))
          {
            value = (sbyte) 0;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseInt8(ref this.cursor);
          return Result.Success;
        default:
          value = (sbyte) 0;
          return Result.Failure;
      }
    }

    public Result ReadInt16(out short value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<short>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutInt16))
          {
            value = (short) 0;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseInt16(ref this.cursor);
          return Result.Success;
        default:
          value = (short) 0;
          return Result.Failure;
      }
    }

    public Result ReadInt32(out int value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<int>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutInt32))
          {
            value = 0;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseInt32(ref this.cursor);
          return Result.Success;
        default:
          value = 0;
          return Result.Failure;
      }
    }

    public Result ReadInt64(out long value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<long>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutInt64))
          {
            value = 0L;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseInt64(ref this.cursor);
          return Result.Success;
        default:
          value = 0L;
          return Result.Failure;
      }
    }

    public Result ReadUInt8(out byte value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<byte>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutUInt8))
          {
            value = (byte) 0;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseUInt8(ref this.cursor);
          return Result.Success;
        default:
          value = (byte) 0;
          return Result.Failure;
      }
    }

    public Result ReadUInt16(out ushort value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<ushort>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutUInt16))
          {
            value = (ushort) 0;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseUInt16(ref this.cursor);
          return Result.Success;
        default:
          value = (ushort) 0;
          return Result.Failure;
      }
    }

    public Result ReadUInt32(out uint value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<uint>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutUInt32))
          {
            value = 0U;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseUInt32(ref this.cursor);
          return Result.Success;
        default:
          value = 0U;
          return Result.Failure;
      }
    }

    public Result ReadUInt64(out ulong value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<ulong>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutUInt64))
          {
            value = 0UL;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseUInt64(ref this.cursor);
          return Result.Success;
        default:
          value = 0UL;
          return Result.Failure;
      }
    }

    public Result ReadVarInt(out long value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<long>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutVarInt))
          {
            value = 0L;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseVarInt(ref this.cursor);
          return Result.Success;
        default:
          value = 0L;
          return Result.Failure;
      }
    }

    public Result ReadVarUInt(out ulong value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<ulong>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutVarUInt))
          {
            value = 0UL;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseVarUInt(ref this.cursor);
          return Result.Success;
        default:
          value = 0UL;
          return Result.Failure;
      }
    }

    public Result ReadFloat32(out float value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<float>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutFloat32))
          {
            value = 0.0f;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseFloat32(ref this.cursor);
          return Result.Success;
        default:
          value = 0.0f;
          return Result.Failure;
      }
    }

    public Result ReadFloat64(out double value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<double>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutFloat64))
          {
            value = 0.0;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseFloat64(ref this.cursor);
          return Result.Success;
        default:
          value = 0.0;
          return Result.Failure;
      }
    }

    public Result ReadFloat128(out Float128 value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<Float128>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutFloat128))
          {
            value = new Float128();
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseFloat128(ref this.cursor);
          return Result.Success;
        default:
          value = new Float128();
          return Result.Failure;
      }
    }

    public Result ReadDecimal(out Decimal value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<Decimal>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutDecimal))
          {
            value = 0M;
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseDecimal(ref this.cursor);
          return Result.Success;
        default:
          value = 0M;
          return Result.Failure;
      }
    }

    public Result ReadDateTime(out DateTime value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<DateTime>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutDateTime))
          {
            value = new DateTime();
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseDateTime(ref this.cursor);
          return Result.Success;
        default:
          value = new DateTime();
          return Result.Failure;
      }
    }

    public Result ReadUnixDateTime(out UnixDateTime value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<UnixDateTime>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutUnixDateTime))
          {
            value = new UnixDateTime();
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseUnixDateTime(ref this.cursor);
          return Result.Success;
        default:
          value = new UnixDateTime();
          return Result.Failure;
      }
    }

    public Result ReadGuid(out Guid value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<Guid>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutGuid))
          {
            value = new Guid();
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseGuid(ref this.cursor);
          return Result.Success;
        default:
          value = new Guid();
          return Result.Failure;
      }
    }

    public Result ReadMongoDbObjectId(out MongoDbObjectId value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<MongoDbObjectId>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutMongoDbObjectId))
          {
            value = new MongoDbObjectId();
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseMongoDbObjectId(ref this.cursor);
          return Result.Success;
        default:
          value = new MongoDbObjectId();
          return Result.Failure;
      }
    }

    public Result ReadString(out string value)
    {
      Utf8Span utf8Span;
      Result result = this.ReadString(out utf8Span);
      value = result == Result.Success ? utf8Span.ToString() : (string) null;
      return result;
    }

    public Result ReadString(out Utf8String value)
    {
      Utf8Span utf8Span;
      Result result = this.ReadString(out utf8Span);
      value = result == Result.Success ? Utf8String.CopyFrom(utf8Span) : (Utf8String) null;
      return result;
    }

    public Result ReadString(out Utf8Span value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutUtf8))
          {
            value = new Utf8Span();
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseString(ref this.cursor);
          return Result.Success;
        default:
          value = new Utf8Span();
          return Result.Failure;
      }
    }

    public Result ReadBinary(out byte[] value)
    {
      ReadOnlySpan<byte> readOnlySpan;
      Result result = this.ReadBinary(out readOnlySpan);
      value = result == Result.Success ? readOnlySpan.ToArray() : (byte[]) null;
      return result;
    }

    public Result ReadBinary(out ReadOnlySpan<byte> value)
    {
      switch (this.state)
      {
        case RowReader.States.Schematized:
          return this.ReadPrimitiveValue<byte>(out value);
        case RowReader.States.Sparse:
          if (!(this.cursor.cellType is LayoutBinary))
          {
            value = new ReadOnlySpan<byte>();
            return Result.TypeMismatch;
          }
          value = this.row.ReadSparseBinary(ref this.cursor);
          return Result.Success;
        default:
          value = new ReadOnlySpan<byte>();
          return Result.Failure;
      }
    }

    public RowReader ReadScope()
    {
      RowCursor scope = this.row.SparseIteratorReadScope(ref this.cursor, true);
      return new RowReader(ref this.row, in scope);
    }

    public Result ReadScope<T, TSerializer>(out T value) where TSerializer : struct, IHybridRowSerializer<T> => default (TSerializer).Read(ref this.row, ref this.cursor, false, out value);

    public Result ReadScope<TContext>(TContext context, RowReader.ReaderFunc<TContext> func)
    {
      RowCursor scope = this.row.SparseIteratorReadScope(ref this.cursor, true);
      RowReader reader = new RowReader(ref this.row, in scope);
      Result result = func != null ? func(ref reader, context) : Result.Success;
      if (result != Result.Success)
        return result;
      this.cursor.Skip(ref this.row, ref reader.cursor);
      return Result.Success;
    }

    public Result SkipScope(ref RowReader nestedReader)
    {
      if (nestedReader.cursor.start != this.cursor.valueOffset)
        return Result.Failure;
      this.cursor.Skip(ref this.row, ref nestedReader.cursor);
      return Result.Success;
    }

    private Result ReadPrimitiveValue<TValue>(out TValue value)
    {
      LayoutColumn column = this.columns[this.columnIndex];
      LayoutType type = this.columns[this.columnIndex].Type;
      if (!(type is LayoutType<TValue>))
      {
        value = default (TValue);
        return Result.TypeMismatch;
      }
      StorageKind? storage = column?.Storage;
      if (storage.HasValue)
      {
        switch (storage.GetValueOrDefault())
        {
          case StorageKind.Fixed:
            return type.TypeAs<LayoutType<TValue>>().ReadFixed(ref this.row, ref this.cursor, column, out value);
          case StorageKind.Variable:
            return type.TypeAs<LayoutType<TValue>>().ReadVariable(ref this.row, ref this.cursor, column, out value);
        }
      }
      value = default (TValue);
      return Result.Failure;
    }

    private Result ReadPrimitiveValue(out Utf8Span value)
    {
      LayoutColumn column = this.columns[this.columnIndex];
      LayoutType type = this.columns[this.columnIndex].Type;
      if (!(type is ILayoutUtf8SpanReadable))
      {
        value = new Utf8Span();
        return Result.TypeMismatch;
      }
      StorageKind? storage = column?.Storage;
      if (storage.HasValue)
      {
        switch (storage.GetValueOrDefault())
        {
          case StorageKind.Fixed:
            return type.TypeAs<ILayoutUtf8SpanReadable>().ReadFixed(ref this.row, ref this.cursor, column, out value);
          case StorageKind.Variable:
            return type.TypeAs<ILayoutUtf8SpanReadable>().ReadVariable(ref this.row, ref this.cursor, column, out value);
        }
      }
      value = new Utf8Span();
      return Result.Failure;
    }

    private Result ReadPrimitiveValue<TElement>(out ReadOnlySpan<TElement> value)
    {
      LayoutColumn column = this.columns[this.columnIndex];
      LayoutType type = this.columns[this.columnIndex].Type;
      if (!(type is ILayoutSpanReadable<TElement>))
      {
        value = new ReadOnlySpan<TElement>();
        return Result.TypeMismatch;
      }
      StorageKind? storage = column?.Storage;
      if (storage.HasValue)
      {
        switch (storage.GetValueOrDefault())
        {
          case StorageKind.Fixed:
            return type.TypeAs<ILayoutSpanReadable<TElement>>().ReadFixed(ref this.row, ref this.cursor, column, out value);
          case StorageKind.Variable:
            return type.TypeAs<ILayoutSpanReadable<TElement>>().ReadVariable(ref this.row, ref this.cursor, column, out value);
        }
      }
      value = new ReadOnlySpan<TElement>();
      return Result.Failure;
    }

    internal enum States : byte
    {
      None,
      Schematized,
      Sparse,
      Done,
    }

    public delegate Result ReaderFunc<in TContext>(ref RowReader reader, TContext context);

    public readonly struct Checkpoint
    {
      internal readonly RowReader.States State;
      internal readonly int ColumnIndex;
      internal readonly RowCursor Cursor;

      internal Checkpoint(RowReader.States state, int columnIndex, RowCursor cursor)
      {
        this.State = state;
        this.ColumnIndex = columnIndex;
        this.Cursor = cursor;
      }
    }
  }
}
