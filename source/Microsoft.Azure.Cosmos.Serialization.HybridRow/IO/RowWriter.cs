// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.IO.RowWriter
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using System;
using System.Buffers;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.IO
{
  public ref struct RowWriter
  {
    private RowBuffer row;
    private RowCursor cursor;

    private RowWriter(ref RowBuffer row, ref RowCursor scope)
    {
      this.row = row;
      this.cursor = scope;
    }

    public LayoutResolver Resolver => this.row.Resolver;

    public int Length => this.row.Length;

    public Layout Layout => this.cursor.layout;

    public static Result WriteBuffer<TContext>(
      ref RowBuffer row,
      TContext context,
      RowWriter.WriterFunc<TContext> func)
    {
      RowCursor scope = RowCursor.Create(ref row);
      RowWriter writer = new RowWriter(ref row, ref scope);
      TypeArgument typeArg = new TypeArgument((LayoutType) LayoutType.UDT, new TypeArgumentList(scope.layout.SchemaId));
      int num = (int) func(ref writer, typeArg, context);
      row = writer.row;
      return (Result) num;
    }

    public Result WriteBool(UtfAnyString path, bool value) => this.WritePrimitive<bool>(path, value, (LayoutType<bool>) LayoutType.Boolean, (RowWriter.AccessMethod<bool>) ((ref RowWriter w, bool v) => w.row.WriteSparseBool(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteNull(UtfAnyString path) => this.WritePrimitive<NullValue>(path, NullValue.Default, (LayoutType<NullValue>) LayoutType.Null, (RowWriter.AccessMethod<NullValue>) ((ref RowWriter w, NullValue v) => w.row.WriteSparseNull(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteInt8(UtfAnyString path, sbyte value) => this.WritePrimitive<sbyte>(path, value, (LayoutType<sbyte>) LayoutType.Int8, (RowWriter.AccessMethod<sbyte>) ((ref RowWriter w, sbyte v) => w.row.WriteSparseInt8(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteInt16(UtfAnyString path, short value) => this.WritePrimitive<short>(path, value, (LayoutType<short>) LayoutType.Int16, (RowWriter.AccessMethod<short>) ((ref RowWriter w, short v) => w.row.WriteSparseInt16(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteInt32(UtfAnyString path, int value) => this.WritePrimitive<int>(path, value, (LayoutType<int>) LayoutType.Int32, (RowWriter.AccessMethod<int>) ((ref RowWriter w, int v) => w.row.WriteSparseInt32(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteInt64(UtfAnyString path, long value) => this.WritePrimitive<long>(path, value, (LayoutType<long>) LayoutType.Int64, (RowWriter.AccessMethod<long>) ((ref RowWriter w, long v) => w.row.WriteSparseInt64(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteUInt8(UtfAnyString path, byte value) => this.WritePrimitive<byte>(path, value, (LayoutType<byte>) LayoutType.UInt8, (RowWriter.AccessMethod<byte>) ((ref RowWriter w, byte v) => w.row.WriteSparseUInt8(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteUInt16(UtfAnyString path, ushort value) => this.WritePrimitive<ushort>(path, value, (LayoutType<ushort>) LayoutType.UInt16, (RowWriter.AccessMethod<ushort>) ((ref RowWriter w, ushort v) => w.row.WriteSparseUInt16(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteUInt32(UtfAnyString path, uint value) => this.WritePrimitive<uint>(path, value, (LayoutType<uint>) LayoutType.UInt32, (RowWriter.AccessMethod<uint>) ((ref RowWriter w, uint v) => w.row.WriteSparseUInt32(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteUInt64(UtfAnyString path, ulong value) => this.WritePrimitive<ulong>(path, value, (LayoutType<ulong>) LayoutType.UInt64, (RowWriter.AccessMethod<ulong>) ((ref RowWriter w, ulong v) => w.row.WriteSparseUInt64(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteVarInt(UtfAnyString path, long value) => this.WritePrimitive<long>(path, value, (LayoutType<long>) LayoutType.VarInt, (RowWriter.AccessMethod<long>) ((ref RowWriter w, long v) => w.row.WriteSparseVarInt(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteVarUInt(UtfAnyString path, ulong value) => this.WritePrimitive<ulong>(path, value, (LayoutType<ulong>) LayoutType.VarUInt, (RowWriter.AccessMethod<ulong>) ((ref RowWriter w, ulong v) => w.row.WriteSparseVarUInt(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteFloat32(UtfAnyString path, float value) => this.WritePrimitive<float>(path, value, (LayoutType<float>) LayoutType.Float32, (RowWriter.AccessMethod<float>) ((ref RowWriter w, float v) => w.row.WriteSparseFloat32(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteFloat64(UtfAnyString path, double value) => this.WritePrimitive<double>(path, value, (LayoutType<double>) LayoutType.Float64, (RowWriter.AccessMethod<double>) ((ref RowWriter w, double v) => w.row.WriteSparseFloat64(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteFloat128(UtfAnyString path, Float128 value) => this.WritePrimitive<Float128>(path, value, (LayoutType<Float128>) LayoutType.Float128, (RowWriter.AccessMethod<Float128>) ((ref RowWriter w, Float128 v) => w.row.WriteSparseFloat128(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteDecimal(UtfAnyString path, Decimal value) => this.WritePrimitive<Decimal>(path, value, (LayoutType<Decimal>) LayoutType.Decimal, (RowWriter.AccessMethod<Decimal>) ((ref RowWriter w, Decimal v) => w.row.WriteSparseDecimal(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteDateTime(UtfAnyString path, DateTime value) => this.WritePrimitive<DateTime>(path, value, (LayoutType<DateTime>) LayoutType.DateTime, (RowWriter.AccessMethod<DateTime>) ((ref RowWriter w, DateTime v) => w.row.WriteSparseDateTime(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteUnixDateTime(UtfAnyString path, UnixDateTime value) => this.WritePrimitive<UnixDateTime>(path, value, (LayoutType<UnixDateTime>) LayoutType.UnixDateTime, (RowWriter.AccessMethod<UnixDateTime>) ((ref RowWriter w, UnixDateTime v) => w.row.WriteSparseUnixDateTime(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteGuid(UtfAnyString path, Guid value) => this.WritePrimitive<Guid>(path, value, (LayoutType<Guid>) LayoutType.Guid, (RowWriter.AccessMethod<Guid>) ((ref RowWriter w, Guid v) => w.row.WriteSparseGuid(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteMongoDbObjectId(UtfAnyString path, MongoDbObjectId value) => this.WritePrimitive<MongoDbObjectId>(path, value, (LayoutType<MongoDbObjectId>) LayoutType.MongoDbObjectId, (RowWriter.AccessMethod<MongoDbObjectId>) ((ref RowWriter w, MongoDbObjectId v) => w.row.WriteSparseMongoDbObjectId(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteString(UtfAnyString path, string value) => this.WritePrimitive<string>(path, value, (LayoutType<string>) LayoutType.Utf8, (RowWriter.AccessMethod<string>) ((ref RowWriter w, string v) => w.row.WriteSparseString(ref w.cursor, Utf8Span.TranscodeUtf16(v), UpdateOptions.Upsert)));

    public Result WriteString(UtfAnyString path, Utf8Span value) => this.WritePrimitive<LayoutUtf8>(path, value, LayoutType.Utf8, (RowWriter.AccessUtf8SpanMethod) ((ref RowWriter w, Utf8Span v) => w.row.WriteSparseString(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteBinary(UtfAnyString path, byte[] value) => this.WritePrimitive<byte[]>(path, value, (LayoutType<byte[]>) LayoutType.Binary, (RowWriter.AccessMethod<byte[]>) ((ref RowWriter w, byte[] v) => w.row.WriteSparseBinary(ref w.cursor, (ReadOnlySpan<byte>) v, UpdateOptions.Upsert)));

    public Result WriteBinary(UtfAnyString path, ReadOnlySpan<byte> value) => this.WritePrimitive<LayoutBinary, byte>(path, value, LayoutType.Binary, (RowWriter.AccessReadOnlySpanMethod<byte>) ((ref RowWriter w, ReadOnlySpan<byte> v) => w.row.WriteSparseBinary(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteBinary(UtfAnyString path, ReadOnlySequence<byte> value) => this.WritePrimitive<LayoutBinary, byte>(path, value, LayoutType.Binary, (RowWriter.AccessMethod<ReadOnlySequence<byte>>) ((ref RowWriter w, ReadOnlySequence<byte> v) => w.row.WriteSparseBinary(ref w.cursor, v, UpdateOptions.Upsert)));

    public Result WriteScope<T, TSerializer>(UtfAnyString path, TypeArgument typeArg, T value) where TSerializer : struct, IHybridRowSerializer<T>
    {
      Result result1 = this.PrepareSparseWrite(path, typeArg);
      if (result1 != Result.Success)
        return result1;
      Result result2 = default (TSerializer).Write(ref this.row, ref this.cursor, false, typeArg.TypeArgs, value);
      if (result2 != Result.Success)
        return result2;
      this.cursor.MoveNext(ref this.row);
      return Result.Success;
    }

    public Result WriteScope<TContext>(
      UtfAnyString path,
      TypeArgument typeArg,
      TContext context,
      RowWriter.WriterFunc<TContext> func)
    {
      LayoutType type = typeArg.Type;
      Result result1 = this.PrepareSparseWrite(path, typeArg);
      if (result1 != Result.Success)
        return result1;
      RowCursor newScope;
      switch (type)
      {
        case LayoutObject scopeType1:
          this.row.WriteSparseObject(ref this.cursor, (LayoutScope) scopeType1, UpdateOptions.Upsert, out newScope);
          break;
        case LayoutArray scopeType2:
          this.row.WriteSparseArray(ref this.cursor, (LayoutScope) scopeType2, UpdateOptions.Upsert, out newScope);
          break;
        case LayoutTypedArray scopeType3:
          this.row.WriteTypedArray(ref this.cursor, (LayoutScope) scopeType3, typeArg.TypeArgs, UpdateOptions.Upsert, out newScope);
          break;
        case LayoutTuple scopeType4:
          this.row.WriteSparseTuple(ref this.cursor, (LayoutScope) scopeType4, typeArg.TypeArgs, UpdateOptions.Upsert, out newScope);
          break;
        case LayoutTypedTuple scopeType5:
          this.row.WriteTypedTuple(ref this.cursor, (LayoutScope) scopeType5, typeArg.TypeArgs, UpdateOptions.Upsert, out newScope);
          break;
        case LayoutTagged scopeType6:
          this.row.WriteTypedTuple(ref this.cursor, (LayoutScope) scopeType6, typeArg.TypeArgs, UpdateOptions.Upsert, out newScope);
          break;
        case LayoutTagged2 scopeType7:
          this.row.WriteTypedTuple(ref this.cursor, (LayoutScope) scopeType7, typeArg.TypeArgs, UpdateOptions.Upsert, out newScope);
          break;
        case LayoutNullable scopeType8:
          this.row.WriteNullable(ref this.cursor, (LayoutScope) scopeType8, typeArg.TypeArgs, UpdateOptions.Upsert, func != null, out newScope);
          break;
        case LayoutUDT scopeType9:
          Layout udt = this.row.Resolver.Resolve(typeArg.TypeArgs.SchemaId);
          this.row.WriteSparseUDT(ref this.cursor, (LayoutScope) scopeType9, udt, UpdateOptions.Upsert, out newScope);
          break;
        case LayoutTypedSet scopeType10:
          this.row.WriteTypedSet(ref this.cursor, (LayoutScope) scopeType10, typeArg.TypeArgs, UpdateOptions.Upsert, out newScope);
          break;
        case LayoutTypedMap scopeType11:
          this.row.WriteTypedMap(ref this.cursor, (LayoutScope) scopeType11, typeArg.TypeArgs, UpdateOptions.Upsert, out newScope);
          break;
        default:
          return Result.Failure;
      }
      RowWriter writer = new RowWriter(ref this.row, ref newScope);
      Result result2 = func != null ? func(ref writer, typeArg, context) : Result.Success;
      this.row = writer.row;
      newScope.count = writer.cursor.count;
      if (result2 != Result.Success)
        return result2;
      if (type is LayoutUniqueScope)
      {
        Result result3 = this.row.TypedCollectionUniqueIndexRebuild(ref newScope);
        if (result3 != Result.Success)
          return result3;
      }
      this.cursor.MoveNext(ref this.row, ref writer.cursor);
      return Result.Success;
    }

    private Result WritePrimitive<TValue>(
      UtfAnyString path,
      TValue value,
      LayoutType<TValue> type,
      RowWriter.AccessMethod<TValue> sparse)
    {
      Result result = Result.NotFound;
      if (this.cursor.scopeType is LayoutUDT)
        result = this.WriteSchematizedValue<TValue>(path, value);
      if (result == Result.NotFound)
      {
        result = this.PrepareSparseWrite(path, type.TypeArg);
        if (result != Result.Success)
          return result;
        sparse(ref this, value);
        this.cursor.MoveNext(ref this.row);
      }
      return result;
    }

    private Result WritePrimitive<TLayoutType>(
      UtfAnyString path,
      Utf8Span value,
      TLayoutType type,
      RowWriter.AccessUtf8SpanMethod sparse)
      where TLayoutType : LayoutType<string>, ILayoutUtf8SpanWritable
    {
      Result result = Result.NotFound;
      if (this.cursor.scopeType is LayoutUDT)
        result = this.WriteSchematizedValue(path, value);
      if (result == Result.NotFound)
      {
        result = this.PrepareSparseWrite(path, type.TypeArg);
        if (result != Result.Success)
          return result;
        sparse(ref this, value);
        this.cursor.MoveNext(ref this.row);
      }
      return result;
    }

    private Result WritePrimitive<TLayoutType, TElement>(
      UtfAnyString path,
      ReadOnlySpan<TElement> value,
      TLayoutType type,
      RowWriter.AccessReadOnlySpanMethod<TElement> sparse)
      where TLayoutType : LayoutType<TElement[]>, ILayoutSpanWritable<TElement>
    {
      Result result = Result.NotFound;
      if (this.cursor.scopeType is LayoutUDT)
        result = this.WriteSchematizedValue<TElement>(path, value);
      if (result == Result.NotFound)
      {
        result = this.PrepareSparseWrite(path, type.TypeArg);
        if (result != Result.Success)
          return result;
        sparse(ref this, value);
        this.cursor.MoveNext(ref this.row);
      }
      return result;
    }

    private Result WritePrimitive<TLayoutType, TElement>(
      UtfAnyString path,
      ReadOnlySequence<TElement> value,
      TLayoutType type,
      RowWriter.AccessMethod<ReadOnlySequence<TElement>> sparse)
      where TLayoutType : LayoutType<TElement[]>, ILayoutSequenceWritable<TElement>
    {
      Result result = Result.NotFound;
      if (this.cursor.scopeType is LayoutUDT)
        result = this.WriteSchematizedValue<TElement>(path, value);
      if (result == Result.NotFound)
      {
        result = this.PrepareSparseWrite(path, type.TypeArg);
        if (result != Result.Success)
          return result;
        sparse(ref this, value);
        this.cursor.MoveNext(ref this.row);
      }
      return result;
    }

    private Result PrepareSparseWrite(UtfAnyString path, TypeArgument typeArg)
    {
      if (this.cursor.scopeType.IsFixedArity && !(this.cursor.scopeType is LayoutNullable))
      {
        if (this.cursor.index < this.cursor.scopeTypeArgs.Count && !typeArg.Equals(this.cursor.scopeTypeArgs[this.cursor.index]))
          return Result.TypeConstraint;
      }
      else if (this.cursor.scopeType is LayoutTypedMap)
      {
        if (!typeArg.Equals(this.cursor.scopeType.TypeAs<LayoutUniqueScope>().FieldType(ref this.cursor)))
          return Result.TypeConstraint;
      }
      else if (this.cursor.scopeType.IsTypedScope && !typeArg.Equals(this.cursor.scopeTypeArgs[0]))
        return Result.TypeConstraint;
      this.cursor.writePath = path;
      return Result.Success;
    }

    private Result WriteSchematizedValue<TValue>(UtfAnyString path, TValue value)
    {
      LayoutColumn column;
      if (!this.cursor.layout.TryFind(path, out column) || !(column.Type is LayoutType<TValue> type))
        return Result.NotFound;
      switch (column.Storage)
      {
        case StorageKind.Fixed:
          return type.WriteFixed(ref this.row, ref this.cursor, column, value);
        case StorageKind.Variable:
          return type.WriteVariable(ref this.row, ref this.cursor, column, value);
        default:
          return Result.NotFound;
      }
    }

    private Result WriteSchematizedValue(UtfAnyString path, Utf8Span value)
    {
      LayoutColumn column;
      if (!this.cursor.layout.TryFind(path, out column))
        return Result.NotFound;
      LayoutType type = column.Type;
      if (!(type is ILayoutUtf8SpanWritable))
        return Result.NotFound;
      switch (column.Storage)
      {
        case StorageKind.Fixed:
          return type.TypeAs<ILayoutUtf8SpanWritable>().WriteFixed(ref this.row, ref this.cursor, column, value);
        case StorageKind.Variable:
          return type.TypeAs<ILayoutUtf8SpanWritable>().WriteVariable(ref this.row, ref this.cursor, column, value);
        default:
          return Result.NotFound;
      }
    }

    private Result WriteSchematizedValue<TElement>(UtfAnyString path, ReadOnlySpan<TElement> value)
    {
      LayoutColumn column;
      if (!this.cursor.layout.TryFind(path, out column))
        return Result.NotFound;
      LayoutType type = column.Type;
      if (!(type is ILayoutSpanWritable<TElement>))
        return Result.NotFound;
      switch (column.Storage)
      {
        case StorageKind.Fixed:
          return type.TypeAs<ILayoutSpanWritable<TElement>>().WriteFixed(ref this.row, ref this.cursor, column, value);
        case StorageKind.Variable:
          return type.TypeAs<ILayoutSpanWritable<TElement>>().WriteVariable(ref this.row, ref this.cursor, column, value);
        default:
          return Result.NotFound;
      }
    }

    private Result WriteSchematizedValue<TElement>(
      UtfAnyString path,
      ReadOnlySequence<TElement> value)
    {
      LayoutColumn column;
      if (!this.cursor.layout.TryFind(path, out column))
        return Result.NotFound;
      LayoutType type = column.Type;
      if (!(type is ILayoutSequenceWritable<TElement>))
        return Result.NotFound;
      switch (column.Storage)
      {
        case StorageKind.Fixed:
          return type.TypeAs<ILayoutSequenceWritable<TElement>>().WriteFixed(ref this.row, ref this.cursor, column, value);
        case StorageKind.Variable:
          return type.TypeAs<ILayoutSequenceWritable<TElement>>().WriteVariable(ref this.row, ref this.cursor, column, value);
        default:
          return Result.NotFound;
      }
    }

    public delegate Result WriterFunc<in TContext>(
      ref RowWriter writer,
      TypeArgument typeArg,
      TContext context);

    private delegate void AccessMethod<in TValue>(ref RowWriter writer, TValue value);

    private delegate void AccessReadOnlySpanMethod<T>(ref RowWriter writer, ReadOnlySpan<T> value);

    private delegate void AccessUtf8SpanMethod(ref RowWriter writer, Utf8Span value);
  }
}
