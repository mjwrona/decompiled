// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.JsonWriter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Documents;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Azure.Cosmos.Json
{
  internal abstract class JsonWriter : IJsonWriter
  {
    private const int MaxStackAlloc = 4096;
    protected readonly JsonObjectState JsonObjectState;

    protected JsonWriter() => this.JsonObjectState = new JsonObjectState(false);

    public abstract JsonSerializationFormat SerializationFormat { get; }

    public abstract long CurrentLength { get; }

    public static IJsonWriter Create(
      JsonSerializationFormat jsonSerializationFormat,
      int initalCapacity = 256,
      bool enableEncodedStrings = true)
    {
      if (jsonSerializationFormat == JsonSerializationFormat.Text)
        return (IJsonWriter) new JsonWriter.JsonTextWriter(initalCapacity);
      if (jsonSerializationFormat == JsonSerializationFormat.Binary)
        return (IJsonWriter) new JsonWriter.JsonBinaryWriter(initalCapacity, false, enableEncodedStrings);
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, RMResources.UnexpectedJsonSerializationFormat, (object) jsonSerializationFormat));
    }

    public abstract void WriteObjectStart();

    public abstract void WriteObjectEnd();

    public abstract void WriteArrayStart();

    public abstract void WriteArrayEnd();

    public virtual unsafe void WriteFieldName(string fieldName)
    {
      int byteCount = Encoding.UTF8.GetByteCount(fieldName);
      Span<byte> span;
      if (byteCount < 4096)
      {
        int length = byteCount;
        // ISSUE: untyped stack allocation
        span = new Span<byte>((void*) __untypedstackalloc((IntPtr) (uint) length), length);
      }
      else
        span = (Span<byte>) new byte[byteCount];
      Span<byte> dest = span;
      Encoding.UTF8.GetBytes(fieldName, dest);
      this.WriteFieldName(Utf8Span.UnsafeFromUtf8BytesNoValidation((ReadOnlySpan<byte>) dest));
    }

    public abstract void WriteFieldName(Utf8Span fieldName);

    public virtual unsafe void WriteStringValue(string value)
    {
      int byteCount = Encoding.UTF8.GetByteCount(value);
      Span<byte> span;
      if (byteCount < 4096)
      {
        int length = byteCount;
        // ISSUE: untyped stack allocation
        span = new Span<byte>((void*) __untypedstackalloc((IntPtr) (uint) length), length);
      }
      else
        span = (Span<byte>) new byte[byteCount];
      Span<byte> dest = span;
      Encoding.UTF8.GetBytes(value, dest);
      this.WriteStringValue(Utf8Span.UnsafeFromUtf8BytesNoValidation((ReadOnlySpan<byte>) dest));
    }

    public abstract void WriteStringValue(Utf8Span value);

    public abstract void WriteNumber64Value(Number64 value);

    public abstract void WriteBoolValue(bool value);

    public abstract void WriteNullValue();

    public abstract void WriteInt8Value(sbyte value);

    public abstract void WriteInt16Value(short value);

    public abstract void WriteInt32Value(int value);

    public abstract void WriteInt64Value(long value);

    public abstract void WriteFloat32Value(float value);

    public abstract void WriteFloat64Value(double value);

    public abstract void WriteUInt32Value(uint value);

    public abstract void WriteGuidValue(Guid value);

    public abstract void WriteBinaryValue(ReadOnlySpan<byte> value);

    public abstract ReadOnlyMemory<byte> GetResult();

    internal bool EnableEncodedStrings { get; private set; }

    public static PreblittedBinaryJsonScope CapturePreblittedBinaryJsonScope(
      Action<ITypedBinaryJsonWriter> scopeWriter)
    {
      JsonWriter.JsonBinaryWriter jsonBinaryWriter = new JsonWriter.JsonBinaryWriter(256, false, false);
      Contract.Requires(!jsonBinaryWriter.JsonObjectState.InArrayContext);
      Contract.Requires(!jsonBinaryWriter.JsonObjectState.InObjectContext);
      Contract.Requires(!jsonBinaryWriter.JsonObjectState.IsPropertyExpected);
      jsonBinaryWriter.WriteObjectStart();
      jsonBinaryWriter.WriteFieldName("someFieldName");
      int currentLength = (int) jsonBinaryWriter.CurrentLength;
      scopeWriter((ITypedBinaryJsonWriter) jsonBinaryWriter);
      return jsonBinaryWriter.CapturePreblittedBinaryJsonScope(currentLength);
    }

    private sealed class JsonBinaryWriter : 
      JsonWriter,
      IJsonBinaryWriterExtensions,
      IJsonWriter,
      ITypedBinaryJsonWriter
    {
      private const int MaxStackAllocSize = 4096;
      private const int MinReferenceStringLength = 2;
      private const int MaxReferenceStringLength = 88;
      private static readonly ImmutableArray<byte> RawValueTypes = ((IEnumerable<JsonWriter.JsonBinaryWriter.RawValueType>) new JsonWriter.JsonBinaryWriter.RawValueType[256]
      {
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.StrUsr,
        JsonWriter.JsonBinaryWriter.RawValueType.StrUsr,
        JsonWriter.JsonBinaryWriter.RawValueType.StrUsr,
        JsonWriter.JsonBinaryWriter.RawValueType.StrUsr,
        JsonWriter.JsonBinaryWriter.RawValueType.StrUsr,
        JsonWriter.JsonBinaryWriter.RawValueType.StrUsr,
        JsonWriter.JsonBinaryWriter.RawValueType.StrUsr,
        JsonWriter.JsonBinaryWriter.RawValueType.StrUsr,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen,
        JsonWriter.JsonBinaryWriter.RawValueType.StrL1,
        JsonWriter.JsonBinaryWriter.RawValueType.StrL2,
        JsonWriter.JsonBinaryWriter.RawValueType.StrL4,
        JsonWriter.JsonBinaryWriter.RawValueType.StrR1,
        JsonWriter.JsonBinaryWriter.RawValueType.StrR2,
        JsonWriter.JsonBinaryWriter.RawValueType.StrR3,
        JsonWriter.JsonBinaryWriter.RawValueType.StrR4,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Arr1,
        JsonWriter.JsonBinaryWriter.RawValueType.Arr,
        JsonWriter.JsonBinaryWriter.RawValueType.Arr,
        JsonWriter.JsonBinaryWriter.RawValueType.Arr,
        JsonWriter.JsonBinaryWriter.RawValueType.Arr,
        JsonWriter.JsonBinaryWriter.RawValueType.Arr,
        JsonWriter.JsonBinaryWriter.RawValueType.Arr,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Obj1,
        JsonWriter.JsonBinaryWriter.RawValueType.Obj,
        JsonWriter.JsonBinaryWriter.RawValueType.Obj,
        JsonWriter.JsonBinaryWriter.RawValueType.Obj,
        JsonWriter.JsonBinaryWriter.RawValueType.Obj,
        JsonWriter.JsonBinaryWriter.RawValueType.Obj,
        JsonWriter.JsonBinaryWriter.RawValueType.Obj,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token,
        JsonWriter.JsonBinaryWriter.RawValueType.Token
      }).Select<JsonWriter.JsonBinaryWriter.RawValueType, byte>((Func<JsonWriter.JsonBinaryWriter.RawValueType, byte>) (x => (byte) x)).ToImmutableArray<byte>();
      private static readonly byte DollarTSystemString = (byte) (32 + JsonBinaryEncoding.SystemStrings.GetSystemStringId(Utf8Span.TranscodeUtf16("$t")).Value);
      private static readonly byte DollarVSystemString = (byte) (32 + JsonBinaryEncoding.SystemStrings.GetSystemStringId(Utf8Span.TranscodeUtf16("$v")).Value);
      private readonly JsonWriter.JsonBinaryWriter.JsonBinaryMemoryWriter binaryWriter;
      private readonly Stack<JsonWriter.JsonBinaryWriter.ArrayAndObjectInfo> bufferedContexts;
      private readonly bool serializeCount;
      private readonly int reservationSize;
      private readonly List<JsonWriter.JsonBinaryWriter.SharedStringValue> sharedStrings;
      private readonly JsonWriter.JsonBinaryWriter.ReferenceStringDictionary sharedStringIndexes;
      private readonly List<int> stringReferenceOffsets;

      public JsonBinaryWriter(int initialCapacity, bool serializeCount, bool enableEncodedStrings)
      {
        this.EnableEncodedStrings = enableEncodedStrings;
        this.binaryWriter = new JsonWriter.JsonBinaryWriter.JsonBinaryMemoryWriter(initialCapacity);
        this.bufferedContexts = new Stack<JsonWriter.JsonBinaryWriter.ArrayAndObjectInfo>();
        this.serializeCount = serializeCount;
        this.reservationSize = 2 + (this.serializeCount ? 1 : 0);
        this.sharedStrings = new List<JsonWriter.JsonBinaryWriter.SharedStringValue>();
        this.sharedStringIndexes = new JsonWriter.JsonBinaryWriter.ReferenceStringDictionary();
        this.stringReferenceOffsets = new List<int>();
        this.binaryWriter.Write((byte) 128);
        this.bufferedContexts.Push(new JsonWriter.JsonBinaryWriter.ArrayAndObjectInfo(this.CurrentLength, 0, 0L, 0));
      }

      public override JsonSerializationFormat SerializationFormat => JsonSerializationFormat.Binary;

      public override long CurrentLength => (long) this.binaryWriter.Position;

      public override void WriteObjectStart() => this.WriterArrayOrObjectStart(false);

      public override void WriteObjectEnd() => this.WriteArrayOrObjectEnd(false);

      public override void WriteArrayStart() => this.WriterArrayOrObjectStart(true);

      public override void WriteArrayEnd() => this.WriteArrayOrObjectEnd(true);

      public override void WriteFieldName(Utf8Span fieldName) => this.WriteFieldNameOrString(true, fieldName);

      public override void WriteStringValue(Utf8Span value) => this.WriteFieldNameOrString(false, value);

      public override void WriteNumber64Value(Number64 value)
      {
        if (value.IsInteger)
          this.WriteIntegerInternal(Number64.ToLong(value));
        else
          this.WriteDoubleInternal(Number64.ToDouble(value));
        ++this.bufferedContexts.Peek().Count;
      }

      public override void WriteBoolValue(bool value)
      {
        this.JsonObjectState.RegisterToken(value ? JsonTokenType.True : JsonTokenType.False);
        this.binaryWriter.Write(value ? (byte) 210 : (byte) 209);
        ++this.bufferedContexts.Peek().Count;
      }

      public override void WriteNullValue()
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Null);
        this.binaryWriter.Write((byte) 208);
        ++this.bufferedContexts.Peek().Count;
      }

      public override void WriteInt8Value(sbyte value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Int8);
        this.binaryWriter.Write((byte) 216);
        this.binaryWriter.Write(value);
        ++this.bufferedContexts.Peek().Count;
      }

      public override void WriteInt16Value(short value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Int16);
        this.binaryWriter.Write((byte) 217);
        this.binaryWriter.Write(value);
        ++this.bufferedContexts.Peek().Count;
      }

      public override void WriteInt32Value(int value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Int32);
        this.binaryWriter.Write((byte) 218);
        this.binaryWriter.Write(value);
        ++this.bufferedContexts.Peek().Count;
      }

      public override void WriteInt64Value(long value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Int64);
        this.binaryWriter.Write((byte) 219);
        this.binaryWriter.Write(value);
        ++this.bufferedContexts.Peek().Count;
      }

      public override void WriteFloat32Value(float value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Float32);
        this.binaryWriter.Write((byte) 205);
        this.binaryWriter.Write(value);
        ++this.bufferedContexts.Peek().Count;
      }

      public override void WriteFloat64Value(double value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Float64);
        this.binaryWriter.Write((byte) 206);
        this.binaryWriter.Write(value);
        ++this.bufferedContexts.Peek().Count;
      }

      public override void WriteUInt32Value(uint value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.UInt32);
        this.binaryWriter.Write((byte) 220);
        this.binaryWriter.Write(value);
        ++this.bufferedContexts.Peek().Count;
      }

      public override void WriteGuidValue(Guid value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Guid);
        this.binaryWriter.Write((byte) 211);
        this.binaryWriter.Write((ReadOnlySpan<byte>) value.ToByteArray());
        ++this.bufferedContexts.Peek().Count;
      }

      public override void WriteBinaryValue(ReadOnlySpan<byte> value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Binary);
        long length = (long) value.Length;
        if ((length & -256L) == 0L)
        {
          this.binaryWriter.Write((byte) 221);
          this.binaryWriter.Write((byte) length);
        }
        else if ((length & -65536L) == 0L)
        {
          this.binaryWriter.Write((byte) 222);
          this.binaryWriter.Write((ushort) length);
        }
        else
        {
          if ((length & -4294967296L) != 0L)
            throw new ArgumentOutOfRangeException("Binary length was too large.");
          this.binaryWriter.Write((byte) 223);
          this.binaryWriter.Write((float) (ulong) length);
        }
        this.binaryWriter.Write(value);
        ++this.bufferedContexts.Peek().Count;
      }

      public override ReadOnlyMemory<byte> GetResult()
      {
        if (this.bufferedContexts.Count > 1)
          throw new JsonNotCompleteException();
        return this.binaryWriter.Position == 1 ? ReadOnlyMemory<byte>.Empty : this.binaryWriter.BufferAsMemory.Slice(0, this.binaryWriter.Position);
      }

      public void WriteDollarTBsonTypeDollarV(byte cosmosBsonTypeByte)
      {
        this.binaryWriter.EnsureRemainingBufferSpace(5);
        this.RegisterArrayOrObjectStart(false, (long) this.binaryWriter.Position, 1);
        this.JsonObjectState.RegisterFieldName();
        Span<byte> span = this.binaryWriter.Cursor.Slice(2);
        span[0] = JsonWriter.JsonBinaryWriter.DollarTSystemString;
        span[1] = cosmosBsonTypeByte;
        span[2] = JsonWriter.JsonBinaryWriter.DollarVSystemString;
        this.binaryWriter.Position += 5;
      }

      public void WriteDollarTBsonTypeDollarVNestedScope(
        bool isNestedArray,
        byte cosmosBsonTypeByte)
      {
        this.binaryWriter.EnsureRemainingBufferSpace(7);
        this.RegisterArrayOrObjectStart(false, (long) this.binaryWriter.Position, 1);
        this.JsonObjectState.RegisterFieldName();
        this.RegisterArrayOrObjectStart(isNestedArray, (long) (this.binaryWriter.Position + 5), 0);
        Span<byte> span = this.binaryWriter.Cursor.Slice(2);
        span[0] = JsonWriter.JsonBinaryWriter.DollarTSystemString;
        span[1] = cosmosBsonTypeByte;
        span[2] = JsonWriter.JsonBinaryWriter.DollarVSystemString;
        this.binaryWriter.Position += 7;
      }

      public void Write(PreblittedBinaryJsonScope scope)
      {
        this.binaryWriter.Write(scope.Bytes.Span);
        this.JsonObjectState.RegisterToken(JsonTokenType.Null);
        ++this.bufferedContexts.Peek().Count;
      }

      internal PreblittedBinaryJsonScope CapturePreblittedBinaryJsonScope(int startPosition)
      {
        Span<byte> span = this.binaryWriter.BufferAsSpan;
        span = span.Slice(startPosition, this.binaryWriter.Position - startPosition);
        return new PreblittedBinaryJsonScope((ReadOnlyMemory<byte>) span.ToArray());
      }

      private void WriterArrayOrObjectStart(bool isArray)
      {
        this.RegisterArrayOrObjectStart(isArray, (long) this.binaryWriter.Position, 0);
        this.binaryWriter.Write((byte) 0);
        this.binaryWriter.Write((byte) 0);
        if (!this.serializeCount)
          return;
        this.binaryWriter.Write((byte) 0);
      }

      private void RegisterArrayOrObjectStart(bool isArray, long offset, int valueCount)
      {
        this.JsonObjectState.RegisterToken(isArray ? JsonTokenType.BeginArray : JsonTokenType.BeginObject);
        this.bufferedContexts.Push(new JsonWriter.JsonBinaryWriter.ArrayAndObjectInfo(offset, this.sharedStrings.Count, (long) this.stringReferenceOffsets.Count, valueCount));
      }

      private void WriteArrayOrObjectEnd(bool isArray)
      {
        this.JsonObjectState.RegisterToken(isArray ? JsonTokenType.EndArray : JsonTokenType.EndObject);
        JsonWriter.JsonBinaryWriter.ArrayAndObjectInfo arrayAndObjectInfo = this.bufferedContexts.Pop();
        int offset = (int) arrayAndObjectInfo.Offset;
        int payloadIndex = offset + this.reservationSize;
        int payloadLength = (int) this.CurrentLength - payloadIndex;
        int count = (int) arrayAndObjectInfo.Count;
        int stringStartIndex = (int) arrayAndObjectInfo.StringStartIndex;
        int referenceStartIndex = (int) arrayAndObjectInfo.StringReferenceStartIndex;
        switch (count)
        {
          case 0:
            this.binaryWriter.Position = offset;
            this.binaryWriter.Write(isArray ? (byte) 224 : (byte) 232);
            break;
          case 1:
            Span<byte> bufferAsSpan1 = this.binaryWriter.BufferAsSpan;
            int bytesToWrite1 = 1;
            this.MoveBuffer(bufferAsSpan1, payloadIndex, payloadLength, offset, bytesToWrite1, stringStartIndex, referenceStartIndex);
            this.binaryWriter.Position = offset;
            this.binaryWriter.Write(isArray ? (byte) 225 : (byte) 233);
            this.binaryWriter.Position = offset + 1 + payloadLength;
            break;
          default:
            if (payloadLength <= (int) byte.MaxValue)
            {
              int num = 2 + (this.serializeCount ? 1 : 0);
              this.binaryWriter.Position = offset;
              if (this.serializeCount)
              {
                this.binaryWriter.Write(isArray ? (byte) 229 : (byte) 237);
                this.binaryWriter.Write((byte) payloadLength);
                this.binaryWriter.Write((byte) count);
              }
              else
              {
                this.binaryWriter.Write(isArray ? (byte) 226 : (byte) 234);
                this.binaryWriter.Write((byte) payloadLength);
              }
              this.binaryWriter.Position = offset + num + payloadLength;
              break;
            }
            if (payloadLength <= (int) ushort.MaxValue)
            {
              this.binaryWriter.Write((byte) 0);
              if (this.serializeCount)
                this.binaryWriter.Write((byte) 0);
              Span<byte> bufferAsSpan2 = this.binaryWriter.BufferAsSpan;
              int bytesToWrite2 = 3 + (this.serializeCount ? 2 : 0);
              this.MoveBuffer(bufferAsSpan2, payloadIndex, payloadLength, offset, bytesToWrite2, stringStartIndex, referenceStartIndex);
              this.binaryWriter.Position = offset;
              if (this.serializeCount)
              {
                this.binaryWriter.Write(isArray ? (byte) 230 : (byte) 238);
                this.binaryWriter.Write((ushort) payloadLength);
                this.binaryWriter.Write((ushort) count);
              }
              else
              {
                this.binaryWriter.Write(isArray ? (byte) 227 : (byte) 235);
                this.binaryWriter.Write((ushort) payloadLength);
              }
              this.binaryWriter.Position = offset + bytesToWrite2 + payloadLength;
              break;
            }
            this.binaryWriter.Write((byte) 0);
            this.binaryWriter.Write((ushort) 0);
            if (this.serializeCount)
            {
              this.binaryWriter.Write((byte) 0);
              this.binaryWriter.Write((ushort) 0);
            }
            Span<byte> bufferAsSpan3 = this.binaryWriter.BufferAsSpan;
            int bytesToWrite3 = 5 + (this.serializeCount ? 4 : 0);
            this.MoveBuffer(bufferAsSpan3, payloadIndex, payloadLength, offset, bytesToWrite3, stringStartIndex, referenceStartIndex);
            this.binaryWriter.Position = offset;
            if (this.serializeCount)
            {
              this.binaryWriter.Write(isArray ? (byte) 231 : (byte) 239);
              this.binaryWriter.Write((uint) payloadLength);
              this.binaryWriter.Write((uint) count);
            }
            else
            {
              this.binaryWriter.Write(isArray ? (byte) 228 : (byte) 236);
              this.binaryWriter.Write((uint) payloadLength);
            }
            this.binaryWriter.Position = offset + bytesToWrite3 + payloadLength;
            break;
        }
        ++this.bufferedContexts.Peek().Count;
        if (offset != 1 || this.sharedStrings.Count <= 0)
          return;
        this.FixReferenceStringOffsets(this.binaryWriter.BufferAsSpan);
      }

      private void MoveBuffer(
        Span<byte> buffer,
        int payloadIndex,
        int payloadLength,
        int typeMarkerIndex,
        int bytesToWrite,
        int stringStartIndex,
        int stringReferenceOffsetLow)
      {
        Span<byte> span = buffer.Slice(payloadIndex, payloadLength);
        int start = typeMarkerIndex + bytesToWrite;
        Span<byte> destination = buffer.Slice(start);
        span.CopyTo(destination);
        int num = start - payloadIndex;
        for (int index = stringStartIndex; index < this.sharedStrings.Count; ++index)
        {
          JsonWriter.JsonBinaryWriter.SharedStringValue sharedString = this.sharedStrings[index];
          this.sharedStrings[index] = new JsonWriter.JsonBinaryWriter.SharedStringValue(sharedString.Offset + num, sharedString.MaxOffset);
        }
        for (int index = stringReferenceOffsetLow; index < this.stringReferenceOffsets.Count; ++index)
          this.stringReferenceOffsets[index] += num;
      }

      private void FixReferenceStringOffsets(Span<byte> binaryWriterRawBuffer)
      {
        foreach (int stringReferenceOffset in this.stringReferenceOffsets)
        {
          byte index = binaryWriterRawBuffer[stringReferenceOffset];
          JsonNodeType jsonNodeType = JsonBinaryEncoding.NodeTypes.Lookup[(int) index];
          switch (jsonNodeType)
          {
            case JsonNodeType.String:
            case JsonNodeType.FieldName:
              Span<byte> buffer = binaryWriterRawBuffer.Slice(stringReferenceOffset + 1);
              switch (index)
              {
                case 195:
                  JsonWriter.JsonBinaryWriter.SharedStringValue sharedString1 = this.sharedStrings[(int) buffer[0]];
                  JsonBinaryEncoding.SetFixedSizedValue<byte>(buffer, (byte) sharedString1.Offset);
                  continue;
                case 196:
                  JsonWriter.JsonBinaryWriter.SharedStringValue sharedString2 = this.sharedStrings[(int) JsonBinaryEncoding.GetFixedSizedValue<ushort>((ReadOnlySpan<byte>) buffer)];
                  JsonBinaryEncoding.SetFixedSizedValue<ushort>(buffer, (ushort) sharedString2.Offset);
                  continue;
                case 197:
                  JsonWriter.JsonBinaryWriter.SharedStringValue sharedString3 = this.sharedStrings[(int) JsonBinaryEncoding.GetFixedSizedValue<JsonBinaryEncoding.UInt24>((ReadOnlySpan<byte>) buffer)];
                  JsonBinaryEncoding.SetFixedSizedValue<JsonBinaryEncoding.UInt24>(buffer, (JsonBinaryEncoding.UInt24) sharedString3.Offset);
                  continue;
                case 198:
                  JsonWriter.JsonBinaryWriter.SharedStringValue sharedString4 = this.sharedStrings[JsonBinaryEncoding.GetFixedSizedValue<int>((ReadOnlySpan<byte>) buffer)];
                  JsonBinaryEncoding.SetFixedSizedValue<int>(buffer, sharedString4.Offset);
                  continue;
                default:
                  continue;
              }
            default:
              throw new InvalidOperationException(string.Format("Unknown {0}: {1}.", (object) "nodeType", (object) jsonNodeType));
          }
        }
      }

      private void WriteFieldNameOrString(bool isFieldName, Utf8Span utf8Span)
      {
        this.binaryWriter.EnsureRemainingBufferSpace(5 + ((Utf8Span) ref utf8Span).Length);
        this.JsonObjectState.RegisterToken(isFieldName ? JsonTokenType.FieldName : JsonTokenType.String);
        JsonBinaryEncoding.MultiByteTypeMarker multiByteTypeMarker;
        if (JsonBinaryEncoding.TryGetEncodedStringTypeMarker(utf8Span, out multiByteTypeMarker))
        {
          switch (multiByteTypeMarker.Length)
          {
            case 1:
              this.binaryWriter.Write(multiByteTypeMarker.One);
              break;
            case 2:
              this.binaryWriter.Write(multiByteTypeMarker.One);
              this.binaryWriter.Write(multiByteTypeMarker.Two);
              break;
            default:
              throw new InvalidOperationException(string.Format("Unable to serialize a {0} of length: {1}", (object) "MultiByteTypeMarker", (object) multiByteTypeMarker.Length));
          }
        }
        else if (!(this.EnableEncodedStrings & isFieldName) || ((Utf8Span) ref utf8Span).Length < 2 || !this.TryRegisterStringValue(utf8Span))
        {
          if (this.EnableEncodedStrings && !isFieldName && ((Utf8Span) ref utf8Span).Length == 36 && JsonBinaryEncoding.TryEncodeGuidString(((Utf8Span) ref utf8Span).Span, this.binaryWriter.Cursor))
            this.binaryWriter.Position += 17;
          else if (this.EnableEncodedStrings && !isFieldName && ((Utf8Span) ref utf8Span).Length == 38 && ((Utf8Span) ref utf8Span).Span[0] == (byte) 34 && ((Utf8Span) ref utf8Span).Span[37] == (byte) 34 && JsonBinaryEncoding.TryEncodeGuidString(((Utf8Span) ref utf8Span).Span.Slice(1), this.binaryWriter.Cursor) && this.binaryWriter.Cursor[0] == (byte) 117)
          {
            this.binaryWriter.Cursor[0] = (byte) 119;
            this.binaryWriter.Position += 17;
          }
          else
          {
            int bytesWritten;
            if (this.EnableEncodedStrings && !isFieldName && JsonBinaryEncoding.TryEncodeCompressedString(((Utf8Span) ref utf8Span).Span, this.binaryWriter.Cursor, out bytesWritten))
              this.binaryWriter.Position += bytesWritten;
            else if (!this.EnableEncodedStrings || isFieldName || ((Utf8Span) ref utf8Span).Length < 2 || ((Utf8Span) ref utf8Span).Length > 88 || !this.TryRegisterStringValue(utf8Span))
            {
              byte typeMarker;
              if (JsonBinaryEncoding.TypeMarker.TryGetEncodedStringLengthTypeMarker((long) ((Utf8Span) ref utf8Span).Length, out typeMarker))
              {
                this.binaryWriter.Write(typeMarker);
                this.binaryWriter.Write(((Utf8Span) ref utf8Span).Span);
              }
              else if (((Utf8Span) ref utf8Span).Length < (int) byte.MaxValue)
              {
                this.binaryWriter.Write((byte) 192);
                this.binaryWriter.Write((byte) ((Utf8Span) ref utf8Span).Length);
                this.binaryWriter.Write(((Utf8Span) ref utf8Span).Span);
              }
              else if (((Utf8Span) ref utf8Span).Length < (int) ushort.MaxValue)
              {
                this.binaryWriter.Write((byte) 193);
                this.binaryWriter.Write((ushort) ((Utf8Span) ref utf8Span).Length);
                this.binaryWriter.Write(((Utf8Span) ref utf8Span).Span);
              }
              else
              {
                this.binaryWriter.Write((byte) 194);
                this.binaryWriter.Write((uint) ((Utf8Span) ref utf8Span).Length);
                this.binaryWriter.Write(((Utf8Span) ref utf8Span).Span);
              }
            }
          }
        }
        if (isFieldName)
          return;
        ++this.bufferedContexts.Peek().Count;
      }

      private bool TryRegisterStringValue(Utf8Span utf8Span)
      {
        (Microsoft.Azure.Cosmos.UInt128 key, ulong value) keyValue;
        if (!this.sharedStringIndexes.TryGetValue(((Utf8Span) ref utf8Span).Span, out keyValue))
        {
          int maxOffset = this.JsonObjectState.CurrentDepth * 3 + (int) this.CurrentLength;
          if ((((Utf8Span) ref utf8Span).Length >= 5 || maxOffset <= (int) byte.MaxValue && ((Utf8Span) ref utf8Span).Length >= 2 || maxOffset <= (int) ushort.MaxValue && ((Utf8Span) ref utf8Span).Length >= 3 ? 1 : (maxOffset > (int) JsonBinaryEncoding.UInt24.MaxValue ? 0 : (((Utf8Span) ref utf8Span).Length >= 4 ? 1 : 0))) != 0)
          {
            this.sharedStrings.Add(new JsonWriter.JsonBinaryWriter.SharedStringValue((int) this.CurrentLength, maxOffset));
            this.sharedStringIndexes.Add(((Utf8Span) ref utf8Span).Length, keyValue.key, (ulong) (this.sharedStrings.Count - 1));
          }
          return false;
        }
        JsonWriter.JsonBinaryWriter.SharedStringValue sharedString = this.sharedStrings[(int) keyValue.value];
        this.stringReferenceOffsets.Add(this.binaryWriter.Position);
        if (sharedString.MaxOffset <= (int) byte.MaxValue)
        {
          this.binaryWriter.Write((byte) 195);
          this.binaryWriter.Write((byte) keyValue.value);
        }
        else if (sharedString.MaxOffset <= (int) ushort.MaxValue)
        {
          this.binaryWriter.Write((byte) 196);
          this.binaryWriter.Write((ushort) keyValue.value);
        }
        else if (sharedString.MaxOffset <= (int) JsonBinaryEncoding.UInt24.MaxValue)
        {
          this.binaryWriter.Write((byte) 197);
          this.binaryWriter.Write((JsonBinaryEncoding.UInt24) (int) keyValue.value);
        }
        else
        {
          if (sharedString.MaxOffset > int.MaxValue)
            return false;
          this.binaryWriter.Write((byte) 198);
          this.binaryWriter.Write((int) keyValue.value);
        }
        return true;
      }

      private void WriteIntegerInternal(long value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Number);
        if (JsonBinaryEncoding.TypeMarker.IsEncodedNumberLiteral(value))
          this.binaryWriter.Write((byte) value);
        else if (value >= 0L)
        {
          if (value <= (long) byte.MaxValue)
          {
            this.binaryWriter.Write((byte) 200);
            this.binaryWriter.Write((byte) value);
          }
          else if (value <= (long) short.MaxValue)
          {
            this.binaryWriter.Write((byte) 201);
            this.binaryWriter.Write((short) value);
          }
          else if (value <= (long) int.MaxValue)
          {
            this.binaryWriter.Write((byte) 202);
            this.binaryWriter.Write((int) value);
          }
          else
          {
            this.binaryWriter.Write((byte) 203);
            this.binaryWriter.Write(value);
          }
        }
        else if (value < (long) int.MinValue)
        {
          this.binaryWriter.Write((byte) 203);
          this.binaryWriter.Write(value);
        }
        else if (value < (long) short.MinValue)
        {
          this.binaryWriter.Write((byte) 202);
          this.binaryWriter.Write((int) value);
        }
        else
        {
          this.binaryWriter.Write((byte) 201);
          this.binaryWriter.Write((short) value);
        }
      }

      private void WriteDoubleInternal(double value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Number);
        this.binaryWriter.Write((byte) 204);
        this.binaryWriter.Write(value);
      }

      public void WriteRawJsonValue(
        ReadOnlyMemory<byte> rootBuffer,
        ReadOnlyMemory<byte> rawJsonValue,
        bool isRootNode,
        bool isFieldName)
      {
        if (isRootNode && this.binaryWriter.Position == 1)
        {
          this.JsonObjectState.RegisterToken(isFieldName ? JsonTokenType.FieldName : JsonTokenType.String);
          this.binaryWriter.Write(rawJsonValue.Span);
          if (isFieldName)
            return;
          ++this.bufferedContexts.Peek().Count;
        }
        else
          this.ForceRewriteRawJsonValue(rootBuffer, rawJsonValue, isFieldName);
      }

      private void ForceRewriteRawJsonValue(
        ReadOnlyMemory<byte> rootBuffer,
        ReadOnlyMemory<byte> rawJsonValue,
        bool isFieldName)
      {
        byte index = rawJsonValue.Span[0];
        JsonWriter.JsonBinaryWriter.RawValueType rawValueType = (JsonWriter.JsonBinaryWriter.RawValueType) JsonWriter.JsonBinaryWriter.RawValueTypes[(int) index];
        switch (rawValueType)
        {
          case JsonWriter.JsonBinaryWriter.RawValueType.Token:
            int valueLength1 = JsonBinaryEncoding.GetValueLength(rawJsonValue.Span);
            rawJsonValue = rawJsonValue.Slice(0, valueLength1);
            this.JsonObjectState.RegisterToken(isFieldName ? JsonTokenType.FieldName : JsonTokenType.String);
            this.binaryWriter.Write(rawJsonValue.Span);
            if (isFieldName)
              break;
            ++this.bufferedContexts.Peek().Count;
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.StrUsr:
          case JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen:
          case JsonWriter.JsonBinaryWriter.RawValueType.StrL1:
          case JsonWriter.JsonBinaryWriter.RawValueType.StrL2:
          case JsonWriter.JsonBinaryWriter.RawValueType.StrL4:
            this.WriteRawStringValue(rawValueType, rawJsonValue, isFieldName);
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.StrR1:
            this.ForceRewriteRawJsonValue(rootBuffer, rootBuffer.Slice((int) JsonBinaryEncoding.GetFixedSizedValue<byte>(rawJsonValue.Slice(1).Span)), isFieldName);
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.StrR2:
            this.ForceRewriteRawJsonValue(rootBuffer, rootBuffer.Slice((int) JsonBinaryEncoding.GetFixedSizedValue<ushort>(rawJsonValue.Slice(1).Span)), isFieldName);
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.StrR3:
            this.ForceRewriteRawJsonValue(rootBuffer, rootBuffer.Slice((int) JsonBinaryEncoding.GetFixedSizedValue<JsonBinaryEncoding.UInt24>(rawJsonValue.Slice(1).Span)), isFieldName);
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.StrR4:
            this.ForceRewriteRawJsonValue(rootBuffer, rootBuffer.Slice(JsonBinaryEncoding.GetFixedSizedValue<int>(rawJsonValue.Slice(1).Span)), isFieldName);
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.Arr1:
            this.JsonObjectState.RegisterToken(JsonTokenType.BeginArray);
            this.binaryWriter.Write(index);
            this.ForceRewriteRawJsonValue(rootBuffer, rawJsonValue.Slice(1), false);
            this.JsonObjectState.RegisterToken(JsonTokenType.EndArray);
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.Obj1:
            this.JsonObjectState.RegisterToken(JsonTokenType.BeginObject);
            this.binaryWriter.Write(index);
            this.ForceRewriteRawJsonValue(rootBuffer, rawJsonValue.Slice(1), true);
            int valueLength2 = JsonBinaryEncoding.GetValueLength(rawJsonValue.Slice(1).Span);
            this.ForceRewriteRawJsonValue(rootBuffer, rawJsonValue.Slice(1 + valueLength2), false);
            this.JsonObjectState.RegisterToken(JsonTokenType.EndObject);
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.Arr:
            this.WriteArrayStart();
            foreach (ReadOnlyMemory<byte> arrayItem in JsonBinaryEncoding.Enumerator.GetArrayItems(rawJsonValue))
              this.ForceRewriteRawJsonValue(rootBuffer, arrayItem, isFieldName);
            this.WriteArrayEnd();
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.Obj:
            this.WriteObjectStart();
            foreach (JsonBinaryEncoding.Enumerator.ObjectProperty objectProperty in JsonBinaryEncoding.Enumerator.GetObjectProperties(rawJsonValue))
            {
              this.ForceRewriteRawJsonValue(rootBuffer, objectProperty.Name, true);
              this.ForceRewriteRawJsonValue(rootBuffer, objectProperty.Value, false);
            }
            this.WriteObjectEnd();
            break;
          default:
            throw new InvalidOperationException(string.Format("Unknown {0} {1}.", (object) "RawValueType", (object) rawValueType));
        }
      }

      private void WriteRawStringValue(
        JsonWriter.JsonBinaryWriter.RawValueType rawValueType,
        ReadOnlyMemory<byte> buffer,
        bool isFieldName)
      {
        Utf8Span utf8Span;
        switch (rawValueType)
        {
          case JsonWriter.JsonBinaryWriter.RawValueType.StrUsr:
            UtfAllString utfAllString;
            if (!JsonBinaryEncoding.TryGetDictionaryEncodedStringValue(buffer.Span, out utfAllString))
              throw new InvalidOperationException("Failed to get dictionary encoded string value");
            utf8Span = utfAllString.Utf8String.Span;
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.StrEncLen:
            long encodedStringLength = JsonBinaryEncoding.TypeMarker.GetEncodedStringLength(buffer.Span[0]);
            if (encodedStringLength > (long) int.MaxValue)
              throw new InvalidOperationException("string is too long.");
            utf8Span = Utf8Span.UnsafeFromUtf8BytesNoValidation(buffer.Slice(1, (int) encodedStringLength).Span);
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.StrL1:
            ReadOnlyMemory<byte> readOnlyMemory1 = buffer.Slice(1);
            byte fixedSizedValue1 = JsonBinaryEncoding.GetFixedSizedValue<byte>(readOnlyMemory1.Span);
            readOnlyMemory1 = buffer.Slice(2, (int) fixedSizedValue1);
            utf8Span = Utf8Span.UnsafeFromUtf8BytesNoValidation(readOnlyMemory1.Span);
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.StrL2:
            ReadOnlyMemory<byte> readOnlyMemory2 = buffer.Slice(1);
            ushort fixedSizedValue2 = JsonBinaryEncoding.GetFixedSizedValue<ushort>(readOnlyMemory2.Span);
            readOnlyMemory2 = buffer.Slice(3, (int) fixedSizedValue2);
            utf8Span = Utf8Span.UnsafeFromUtf8BytesNoValidation(readOnlyMemory2.Span);
            break;
          case JsonWriter.JsonBinaryWriter.RawValueType.StrL4:
            ReadOnlyMemory<byte> readOnlyMemory3 = buffer.Slice(1);
            uint fixedSizedValue3 = JsonBinaryEncoding.GetFixedSizedValue<uint>(readOnlyMemory3.Span);
            readOnlyMemory3 = fixedSizedValue3 <= (uint) int.MaxValue ? buffer.Slice(5, (int) fixedSizedValue3) : throw new InvalidOperationException("string is too long.");
            utf8Span = Utf8Span.UnsafeFromUtf8BytesNoValidation(readOnlyMemory3.Span);
            break;
          default:
            throw new InvalidOperationException(string.Format("Unknown {0}: {1}.", (object) nameof (rawValueType), (object) rawValueType));
        }
        this.WriteFieldNameOrString(isFieldName, utf8Span);
      }

      private enum RawValueType : byte
      {
        Token,
        StrUsr,
        StrEncLen,
        StrL1,
        StrL2,
        StrL4,
        StrR1,
        StrR2,
        StrR3,
        StrR4,
        Arr1,
        Obj1,
        Arr,
        Obj,
      }

      private sealed class ArrayAndObjectInfo
      {
        public ArrayAndObjectInfo(
          long offset,
          int stringStartIndex,
          long stringReferenceStartIndex,
          int valueCount)
        {
          this.Offset = offset;
          this.Count = (long) valueCount;
          this.StringStartIndex = (long) stringStartIndex;
          this.StringReferenceStartIndex = stringReferenceStartIndex;
        }

        public long Offset { get; }

        public long Count { get; set; }

        public long StringStartIndex { get; }

        public long StringReferenceStartIndex { get; }
      }

      private sealed class JsonBinaryMemoryWriter : JsonMemoryWriter
      {
        public JsonBinaryMemoryWriter(int initialCapacity = 256)
          : base(initialCapacity)
        {
        }

        public void Write(byte value)
        {
          this.EnsureRemainingBufferSpace(1);
          this.buffer[this.Position] = value;
          ++this.Position;
        }

        public void Write(sbyte value) => this.Write((byte) value);

        public void Write(short value)
        {
          this.EnsureRemainingBufferSpace(2);
          BinaryPrimitives.WriteInt16LittleEndian(this.Cursor, value);
          this.Position += 2;
        }

        public void Write(ushort value)
        {
          this.EnsureRemainingBufferSpace(2);
          BinaryPrimitives.WriteUInt16LittleEndian(this.Cursor, value);
          this.Position += 2;
        }

        public void Write(JsonBinaryEncoding.UInt24 value)
        {
          this.EnsureRemainingBufferSpace(3);
          this.Write(value.Byte1);
          this.Write(value.Byte2);
          this.Write(value.Byte3);
        }

        public void Write(int value)
        {
          this.EnsureRemainingBufferSpace(4);
          BinaryPrimitives.WriteInt32LittleEndian(this.Cursor, value);
          this.Position += 4;
        }

        public void Write(uint value)
        {
          this.EnsureRemainingBufferSpace(4);
          BinaryPrimitives.WriteUInt32LittleEndian(this.Cursor, value);
          this.Position += 4;
        }

        public void Write(long value)
        {
          this.EnsureRemainingBufferSpace(8);
          BinaryPrimitives.WriteInt64LittleEndian(this.Cursor, value);
          this.Position += 8;
        }

        public void Write(float value)
        {
          this.EnsureRemainingBufferSpace(4);
          MemoryMarshal.Write<float>(this.Cursor, ref value);
          this.Position += 4;
        }

        public void Write(double value)
        {
          this.EnsureRemainingBufferSpace(8);
          MemoryMarshal.Write<double>(this.Cursor, ref value);
          this.Position += 8;
        }

        public void Write(Guid value)
        {
          int size = Marshal.SizeOf<Guid>(Guid.Empty);
          this.EnsureRemainingBufferSpace(size);
          MemoryMarshal.Write<Guid>(this.Cursor, ref value);
          this.Position += size;
        }
      }

      private sealed class ReferenceStringDictionary
      {
        private readonly Dictionary<Microsoft.Azure.Cosmos.UInt128, ulong> stringLiteralDictionary;
        private readonly Dictionary<Microsoft.Azure.Cosmos.UInt128, ulong> stringHashDictionary;

        public ReferenceStringDictionary()
        {
          this.stringLiteralDictionary = new Dictionary<Microsoft.Azure.Cosmos.UInt128, ulong>();
          this.stringHashDictionary = new Dictionary<Microsoft.Azure.Cosmos.UInt128, ulong>();
        }

        public unsafe bool TryGetValue(
          ReadOnlySpan<byte> stringValue,
          out (Microsoft.Azure.Cosmos.UInt128 key, ulong value) keyValue)
        {
          if (stringValue.Length < 16)
          {
            // ISSUE: untyped stack allocation
            Span<byte> span = new Span<byte>((void*) __untypedstackalloc(new IntPtr(16)), 16);
            span[0] = (byte) stringValue.Length;
            stringValue.CopyTo(span.Slice(1));
            Microsoft.Azure.Cosmos.UInt128 key = MemoryMarshal.Cast<byte, Microsoft.Azure.Cosmos.UInt128>(span)[0];
            ulong num;
            if (!this.stringLiteralDictionary.TryGetValue(key, out num))
            {
              keyValue = (key, num);
              return false;
            }
            keyValue = (key, num);
            return true;
          }
          Microsoft.Azure.Cosmos.UInt128 key1 = MurmurHash3.Hash128(stringValue, Microsoft.Azure.Cosmos.UInt128.Create((ulong) stringValue.Length, (ulong) stringValue.Length));
          ulong num1;
          if (!this.stringHashDictionary.TryGetValue(key1, out num1))
          {
            keyValue = (key1, num1);
            return false;
          }
          keyValue = (key1, num1);
          return true;
        }

        public void Add(int stringLength, Microsoft.Azure.Cosmos.UInt128 key, ulong value)
        {
          if (stringLength < 16)
            this.stringLiteralDictionary.Add(key, value);
          else
            this.stringHashDictionary.Add(key, value);
        }
      }

      private readonly struct SharedStringValue
      {
        public SharedStringValue(int offset, int maxOffset)
        {
          this.Offset = offset;
          this.MaxOffset = maxOffset;
        }

        public int Offset { get; }

        public int MaxOffset { get; }
      }
    }

    private sealed class JsonTextWriter : JsonWriter, IJsonTextWriterExtensions, IJsonWriter
    {
      private const byte ValueSeperatorToken = 58;
      private const byte MemberSeperatorToken = 44;
      private const byte ObjectStartToken = 123;
      private const byte ObjectEndToken = 125;
      private const byte ArrayStartToken = 91;
      private const byte ArrayEndToken = 93;
      private const byte PropertyStartToken = 34;
      private const byte PropertyEndToken = 34;
      private const byte StringStartToken = 34;
      private const byte StringEndToken = 34;
      private const byte Int8TokenPrefix = 73;
      private const byte Int16TokenPrefix = 72;
      private const byte Int32TokenPrefix = 76;
      private const byte UnsignedTokenPrefix = 85;
      private const byte FloatTokenPrefix = 83;
      private const byte DoubleTokenPrefix = 68;
      private const byte GuidTokenPrefix = 71;
      private const byte BinaryTokenPrefix = 66;
      private const byte DoubleQuote = 34;
      private const byte ReverseSolidus = 92;
      private const byte Space = 32;
      private static readonly ReadOnlyMemory<byte> NotANumber = (ReadOnlyMemory<byte>) new byte[3]
      {
        (byte) 78,
        (byte) 97,
        (byte) 78
      };
      private static readonly ReadOnlyMemory<byte> PositiveInfinity = (ReadOnlyMemory<byte>) new byte[8]
      {
        (byte) 73,
        (byte) 110,
        (byte) 102,
        (byte) 105,
        (byte) 110,
        (byte) 105,
        (byte) 116,
        (byte) 121
      };
      private static readonly ReadOnlyMemory<byte> NegativeInfinity = (ReadOnlyMemory<byte>) new byte[9]
      {
        (byte) 45,
        (byte) 73,
        (byte) 110,
        (byte) 102,
        (byte) 105,
        (byte) 110,
        (byte) 105,
        (byte) 116,
        (byte) 121
      };
      private static readonly ReadOnlyMemory<byte> TrueString = (ReadOnlyMemory<byte>) new byte[4]
      {
        (byte) 116,
        (byte) 114,
        (byte) 117,
        (byte) 101
      };
      private static readonly ReadOnlyMemory<byte> FalseString = (ReadOnlyMemory<byte>) new byte[5]
      {
        (byte) 102,
        (byte) 97,
        (byte) 108,
        (byte) 115,
        (byte) 101
      };
      private static readonly ReadOnlyMemory<byte> NullString = (ReadOnlyMemory<byte>) new byte[4]
      {
        (byte) 110,
        (byte) 117,
        (byte) 108,
        (byte) 108
      };
      private static readonly Vector<byte> DoubleQuoteVector = new Vector<byte>((byte) 34);
      private static readonly Vector<byte> ReverseSolidusVector = new Vector<byte>((byte) 92);
      private static readonly Vector<byte> SpaceVector = new Vector<byte>((byte) 32);
      private readonly JsonWriter.JsonTextWriter.JsonTextMemoryWriter jsonTextMemoryWriter;
      private bool firstValue;

      public JsonTextWriter(int initialCapacity = 256)
      {
        this.firstValue = true;
        this.jsonTextMemoryWriter = new JsonWriter.JsonTextWriter.JsonTextMemoryWriter(initialCapacity);
      }

      public override JsonSerializationFormat SerializationFormat => JsonSerializationFormat.Text;

      public override long CurrentLength => (long) this.jsonTextMemoryWriter.Position;

      public override void WriteObjectStart()
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.BeginObject);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write((byte) 123);
        this.firstValue = true;
      }

      public override void WriteObjectEnd()
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.EndObject);
        this.jsonTextMemoryWriter.Write((byte) 125);
        this.firstValue = false;
      }

      public override void WriteArrayStart()
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.BeginArray);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write((byte) 91);
        this.firstValue = true;
      }

      public override void WriteArrayEnd()
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.EndArray);
        this.jsonTextMemoryWriter.Write((byte) 93);
        this.firstValue = false;
      }

      public override void WriteFieldName(Utf8Span fieldName)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.FieldName);
        this.PrefixMemberSeparator();
        this.firstValue = true;
        this.jsonTextMemoryWriter.Write((byte) 34);
        this.WriteEscapedString(fieldName);
        this.jsonTextMemoryWriter.Write((byte) 34);
        this.jsonTextMemoryWriter.Write((byte) 58);
      }

      public override void WriteStringValue(Utf8Span value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.String);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write((byte) 34);
        this.WriteEscapedString(value);
        this.jsonTextMemoryWriter.Write((byte) 34);
      }

      public override void WriteNumber64Value(Number64 value)
      {
        if (value.IsInteger)
          this.WriteIntegerInternal(Number64.ToLong(value));
        else
          this.WriteDoubleInternal(Number64.ToDouble(value));
      }

      public override void WriteBoolValue(bool value)
      {
        this.JsonObjectState.RegisterToken(value ? JsonTokenType.True : JsonTokenType.False);
        this.PrefixMemberSeparator();
        if (value)
          this.jsonTextMemoryWriter.Write(JsonWriter.JsonTextWriter.TrueString.Span);
        else
          this.jsonTextMemoryWriter.Write(JsonWriter.JsonTextWriter.FalseString.Span);
      }

      public override void WriteNullValue()
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Null);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write(JsonWriter.JsonTextWriter.NullString.Span);
      }

      public override void WriteInt8Value(sbyte value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Int8);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write((byte) 73);
        this.jsonTextMemoryWriter.Write(value);
      }

      public override void WriteInt16Value(short value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Int16);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write((byte) 72);
        this.jsonTextMemoryWriter.Write(value);
      }

      public override void WriteInt32Value(int value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Int32);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write((byte) 76);
        this.jsonTextMemoryWriter.Write(value);
      }

      public override void WriteInt64Value(long value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Int64);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write((byte) 76);
        this.jsonTextMemoryWriter.Write((byte) 76);
        this.jsonTextMemoryWriter.Write(value);
      }

      public override void WriteFloat32Value(float value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Float32);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write((byte) 83);
        this.jsonTextMemoryWriter.Write(value);
      }

      public override void WriteFloat64Value(double value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Float64);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write((byte) 68);
        this.jsonTextMemoryWriter.Write(value);
      }

      public override void WriteUInt32Value(uint value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.UInt32);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write((byte) 85);
        this.jsonTextMemoryWriter.Write((byte) 76);
        this.jsonTextMemoryWriter.Write(value);
      }

      public override void WriteGuidValue(Guid value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Guid);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write((byte) 71);
        this.jsonTextMemoryWriter.Write(value);
      }

      public override void WriteBinaryValue(ReadOnlySpan<byte> value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Binary);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write((byte) 66);
        this.jsonTextMemoryWriter.WriteBinaryAsBase64(value);
      }

      public void WriteRawJsonValue(ReadOnlyMemory<byte> buffer, bool isFieldName)
      {
        this.JsonObjectState.RegisterToken(isFieldName ? JsonTokenType.FieldName : JsonTokenType.String);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write(buffer.Span);
        if (!isFieldName)
          return;
        this.firstValue = true;
        this.jsonTextMemoryWriter.Write((byte) 58);
      }

      public override ReadOnlyMemory<byte> GetResult() => this.jsonTextMemoryWriter.BufferAsMemory.Slice(0, this.jsonTextMemoryWriter.Position);

      private void WriteIntegerInternal(long value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Number);
        this.PrefixMemberSeparator();
        this.jsonTextMemoryWriter.Write(value);
      }

      private void WriteDoubleInternal(double value)
      {
        this.JsonObjectState.RegisterToken(JsonTokenType.Number);
        this.PrefixMemberSeparator();
        if (double.IsNaN(value))
        {
          this.jsonTextMemoryWriter.Write((byte) 34);
          this.jsonTextMemoryWriter.Write(JsonWriter.JsonTextWriter.NotANumber.Span);
          this.jsonTextMemoryWriter.Write((byte) 34);
        }
        else if (double.IsNegativeInfinity(value))
        {
          this.jsonTextMemoryWriter.Write((byte) 34);
          this.jsonTextMemoryWriter.Write(JsonWriter.JsonTextWriter.NegativeInfinity.Span);
          this.jsonTextMemoryWriter.Write((byte) 34);
        }
        else if (double.IsPositiveInfinity(value))
        {
          this.jsonTextMemoryWriter.Write((byte) 34);
          this.jsonTextMemoryWriter.Write(JsonWriter.JsonTextWriter.PositiveInfinity.Span);
          this.jsonTextMemoryWriter.Write((byte) 34);
        }
        else
          this.jsonTextMemoryWriter.Write(value);
      }

      private void PrefixMemberSeparator()
      {
        if (!this.firstValue)
          this.jsonTextMemoryWriter.Write((byte) 44);
        this.firstValue = false;
      }

      private void WriteEscapedString(Utf8Span unescapedString)
      {
        while (!((Utf8Span) ref unescapedString).IsEmpty)
        {
          int? nullable = JsonWriter.JsonTextWriter.IndexOfCharacterThatNeedsEscaping(unescapedString);
          if (!nullable.HasValue)
            nullable = new int?(((Utf8Span) ref unescapedString).Length);
          JsonWriter.JsonTextWriter.JsonTextMemoryWriter textMemoryWriter = this.jsonTextMemoryWriter;
          ReadOnlySpan<byte> span = ((Utf8Span) ref unescapedString).Span;
          ReadOnlySpan<byte> readOnlySpan = span.Slice(0, nullable.Value);
          textMemoryWriter.Write(readOnlySpan);
          span = ((Utf8Span) ref unescapedString).Span;
          unescapedString = Utf8Span.UnsafeFromUtf8BytesNoValidation(span.Slice(nullable.Value));
          if (!((Utf8Span) ref unescapedString).IsEmpty)
          {
            span = ((Utf8Span) ref unescapedString).Span;
            byte num = span[0];
            span = ((Utf8Span) ref unescapedString).Span;
            unescapedString = Utf8Span.UnsafeFromUtf8BytesNoValidation(span.Slice(1));
            switch (num)
            {
              case 8:
                this.jsonTextMemoryWriter.Write((byte) 92);
                this.jsonTextMemoryWriter.Write((byte) 98);
                continue;
              case 9:
                this.jsonTextMemoryWriter.Write((byte) 92);
                this.jsonTextMemoryWriter.Write((byte) 116);
                continue;
              case 10:
                this.jsonTextMemoryWriter.Write((byte) 92);
                this.jsonTextMemoryWriter.Write((byte) 110);
                continue;
              case 12:
                this.jsonTextMemoryWriter.Write((byte) 92);
                this.jsonTextMemoryWriter.Write((byte) 102);
                continue;
              case 13:
                this.jsonTextMemoryWriter.Write((byte) 92);
                this.jsonTextMemoryWriter.Write((byte) 114);
                continue;
              case 34:
                this.jsonTextMemoryWriter.Write((byte) 92);
                this.jsonTextMemoryWriter.Write((byte) 34);
                continue;
              case 47:
                this.jsonTextMemoryWriter.Write((byte) 92);
                this.jsonTextMemoryWriter.Write((byte) 47);
                continue;
              case 92:
                this.jsonTextMemoryWriter.Write((byte) 92);
                this.jsonTextMemoryWriter.Write((byte) 92);
                continue;
              default:
                char ch = (char) num;
                this.jsonTextMemoryWriter.Write((byte) 92);
                this.jsonTextMemoryWriter.Write((byte) 117);
                this.jsonTextMemoryWriter.Write(JsonWriter.JsonTextWriter.GetHexDigit((int) ch >> 12 & 15));
                this.jsonTextMemoryWriter.Write(JsonWriter.JsonTextWriter.GetHexDigit((int) ch >> 8 & 15));
                this.jsonTextMemoryWriter.Write(JsonWriter.JsonTextWriter.GetHexDigit((int) ch >> 4 & 15));
                this.jsonTextMemoryWriter.Write(JsonWriter.JsonTextWriter.GetHexDigit((int) ch & 15));
                continue;
            }
          }
        }
      }

      private static unsafe int? IndexOfCharacterThatNeedsEscaping(Utf8Span utf8Span)
      {
        int count = Vector<byte>.Count;
        int index1 = 0;
        ReadOnlySpan<byte> span;
        if (Vector.IsHardwareAccelerated)
        {
          for (int index2 = ((Utf8Span) ref utf8Span).Length / count * count; index1 < index2; index1 += count)
          {
            span = ((Utf8Span) ref utf8Span).Span;
            Vector<byte> vector;
            fixed (byte* numPtr = &span.GetPinnableReference())
              vector = Unsafe.Read<Vector<byte>>((void*) (numPtr + index1));
            if (JsonWriter.JsonTextWriter.HasCharacterThatNeedsEscaping(vector))
            {
              for (; index1 < ((Utf8Span) ref utf8Span).Length; ++index1)
              {
                span = ((Utf8Span) ref utf8Span).Span;
                if (JsonWriter.JsonTextWriter.NeedsEscaping(span[index1]))
                  return new int?(index1);
              }
            }
          }
        }
        for (; index1 < ((Utf8Span) ref utf8Span).Length; ++index1)
        {
          span = ((Utf8Span) ref utf8Span).Span;
          if (JsonWriter.JsonTextWriter.NeedsEscaping(span[index1]))
            return new int?(index1);
        }
        return new int?();
      }

      private static bool HasCharacterThatNeedsEscaping(Vector<byte> vector) => Vector.EqualsAny<byte>(vector, JsonWriter.JsonTextWriter.ReverseSolidusVector) || Vector.EqualsAny<byte>(vector, JsonWriter.JsonTextWriter.DoubleQuoteVector) || Vector.LessThanAny<byte>(vector, JsonWriter.JsonTextWriter.SpaceVector);

      private static bool NeedsEscaping(byte value) => value == (byte) 92 || value == (byte) 34 || value < (byte) 32;

      private static byte GetHexDigit(int value) => value < 10 ? (byte) (48 + value) : (byte) (65 + value - 10);

      private sealed class JsonTextMemoryWriter : JsonMemoryWriter
      {
        private static readonly StandardFormat floatFormat = new StandardFormat('R');
        private static readonly StandardFormat doubleFormat = new StandardFormat('R');

        public JsonTextMemoryWriter(int initialCapacity = 256)
          : base(initialCapacity)
        {
        }

        public void Write(bool value)
        {
          this.EnsureRemainingBufferSpace(5);
          int bytesWritten;
          if (!Utf8Formatter.TryFormat(value, this.Cursor, out bytesWritten))
            throw new InvalidOperationException(string.Format("Failed to {0}({1}{2})", (object) nameof (Write), (object) typeof (bool).FullName, (object) value));
          this.Position += bytesWritten;
        }

        public void Write(byte value)
        {
          this.EnsureRemainingBufferSpace(1);
          this.buffer[this.Position] = value;
          ++this.Position;
        }

        public void Write(sbyte value)
        {
          this.EnsureRemainingBufferSpace(4);
          int bytesWritten;
          if (!Utf8Formatter.TryFormat(value, this.Cursor, out bytesWritten, new StandardFormat()))
            throw new InvalidOperationException(string.Format("Failed to {0}({1}{2})", (object) nameof (Write), (object) typeof (sbyte).FullName, (object) value));
          this.Position += bytesWritten;
        }

        public void Write(short value)
        {
          this.EnsureRemainingBufferSpace(6);
          int bytesWritten;
          if (!Utf8Formatter.TryFormat(value, this.Cursor, out bytesWritten, new StandardFormat()))
            throw new InvalidOperationException(string.Format("Failed to {0}({1}{2})", (object) nameof (Write), (object) typeof (short).FullName, (object) value));
          this.Position += bytesWritten;
        }

        public void Write(int value)
        {
          this.EnsureRemainingBufferSpace(11);
          int bytesWritten;
          if (!Utf8Formatter.TryFormat(value, this.Cursor, out bytesWritten, new StandardFormat()))
            throw new InvalidOperationException(string.Format("Failed to {0}({1}{2})", (object) nameof (Write), (object) typeof (int).FullName, (object) value));
          this.Position += bytesWritten;
        }

        public void Write(uint value)
        {
          this.EnsureRemainingBufferSpace(11);
          int bytesWritten;
          if (!Utf8Formatter.TryFormat(value, this.Cursor, out bytesWritten, new StandardFormat()))
            throw new InvalidOperationException(string.Format("Failed to {0}({1}{2})", (object) nameof (Write), (object) typeof (int).FullName, (object) value));
          this.Position += bytesWritten;
        }

        public void Write(long value)
        {
          this.EnsureRemainingBufferSpace(20);
          int bytesWritten;
          if (!Utf8Formatter.TryFormat(value, this.Cursor, out bytesWritten, new StandardFormat()))
            throw new InvalidOperationException(string.Format("Failed to {0}({1}{2})", (object) nameof (Write), (object) typeof (long).FullName, (object) value));
          this.Position += bytesWritten;
        }

        public void Write(float value)
        {
          this.EnsureRemainingBufferSpace(32);
          foreach (byte num in value.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture))
          {
            this.buffer[this.Position] = num;
            ++this.Position;
          }
        }

        public void Write(double value)
        {
          this.EnsureRemainingBufferSpace(32);
          foreach (byte num in value.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture))
          {
            this.buffer[this.Position] = num;
            ++this.Position;
          }
        }

        public void Write(Guid value)
        {
          this.EnsureRemainingBufferSpace(38);
          int bytesWritten;
          if (!Utf8Formatter.TryFormat(value, this.Cursor, out bytesWritten))
            throw new InvalidOperationException(string.Format("Failed to {0}({1}{2})", (object) nameof (Write), (object) typeof (double).FullName, (object) value));
          this.Position += bytesWritten;
        }

        public void WriteBinaryAsBase64(ReadOnlySpan<byte> binary)
        {
          this.EnsureRemainingBufferSpace(Base64.GetMaxEncodedToUtf8Length(binary.Length));
          int bytesWritten;
          int utf8 = (int) Base64.EncodeToUtf8(binary, this.Cursor, out int _, out bytesWritten);
          this.Position += bytesWritten;
        }
      }
    }
  }
}
