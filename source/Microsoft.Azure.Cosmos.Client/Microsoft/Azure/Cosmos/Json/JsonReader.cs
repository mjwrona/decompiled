// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.JsonReader
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.Json
{
  internal abstract class JsonReader : IJsonReader
  {
    internal readonly JsonObjectState JsonObjectState;
    private static readonly ImmutableArray<JsonTokenType> TypeMarkerToTokenType = ((IEnumerable<JsonTokenType>) new JsonTokenType[256]
    {
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.String,
      JsonTokenType.NotStarted,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Number,
      JsonTokenType.Float32,
      JsonTokenType.Float64,
      JsonTokenType.NotStarted,
      JsonTokenType.Null,
      JsonTokenType.False,
      JsonTokenType.True,
      JsonTokenType.Guid,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.Int8,
      JsonTokenType.Int16,
      JsonTokenType.Int32,
      JsonTokenType.Int64,
      JsonTokenType.UInt32,
      JsonTokenType.Binary,
      JsonTokenType.Binary,
      JsonTokenType.Binary,
      JsonTokenType.BeginArray,
      JsonTokenType.BeginArray,
      JsonTokenType.BeginArray,
      JsonTokenType.BeginArray,
      JsonTokenType.BeginArray,
      JsonTokenType.BeginArray,
      JsonTokenType.BeginArray,
      JsonTokenType.BeginArray,
      JsonTokenType.BeginObject,
      JsonTokenType.BeginObject,
      JsonTokenType.BeginObject,
      JsonTokenType.BeginObject,
      JsonTokenType.BeginObject,
      JsonTokenType.BeginObject,
      JsonTokenType.BeginObject,
      JsonTokenType.BeginObject,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted,
      JsonTokenType.NotStarted
    }).ToImmutableArray<JsonTokenType>();

    protected JsonReader() => this.JsonObjectState = new JsonObjectState(true);

    public abstract JsonSerializationFormat SerializationFormat { get; }

    public int CurrentDepth => this.JsonObjectState.CurrentDepth;

    public JsonTokenType CurrentTokenType => this.JsonObjectState.CurrentTokenType;

    public static IJsonReader Create(ReadOnlyMemory<byte> buffer) => !buffer.IsEmpty ? JsonReader.Create(buffer.Span[0] == (byte) 128 ? JsonSerializationFormat.Binary : JsonSerializationFormat.Text, buffer) : throw new ArgumentOutOfRangeException("buffer can not be empty.");

    public static IJsonReader Create(
      JsonSerializationFormat jsonSerializationFormat,
      ReadOnlyMemory<byte> buffer)
    {
      if (buffer.IsEmpty)
        throw new ArgumentOutOfRangeException("buffer can not be empty.");
      if (jsonSerializationFormat == JsonSerializationFormat.Text)
        return (IJsonReader) new JsonReader.JsonTextReader(buffer);
      if (jsonSerializationFormat == JsonSerializationFormat.Binary)
        return (IJsonReader) new JsonReader.JsonBinaryReader(buffer);
      throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}.", (object) "JsonSerializationFormat", (object) jsonSerializationFormat));
    }

    internal static IJsonReader CreateBinaryFromOffset(ReadOnlyMemory<byte> buffer, int offset) => (IJsonReader) new JsonReader.JsonBinaryReader(buffer, new int?(offset));

    public abstract bool Read();

    public abstract Number64 GetNumberValue();

    public abstract UtfAnyString GetStringValue();

    public abstract bool TryGetBufferedStringValue(out Utf8Memory value);

    public abstract sbyte GetInt8Value();

    public abstract short GetInt16Value();

    public abstract int GetInt32Value();

    public abstract long GetInt64Value();

    public abstract uint GetUInt32Value();

    public abstract float GetFloat32Value();

    public abstract double GetFloat64Value();

    public abstract Guid GetGuidValue();

    public abstract ReadOnlyMemory<byte> GetBinaryValue();

    public virtual void WriteCurrentToken(IJsonWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      JsonTokenType currentTokenType = this.CurrentTokenType;
      switch (currentTokenType)
      {
        case JsonTokenType.NotStarted:
          break;
        case JsonTokenType.BeginArray:
          writer.WriteArrayStart();
          break;
        case JsonTokenType.EndArray:
          writer.WriteArrayEnd();
          break;
        case JsonTokenType.BeginObject:
          writer.WriteObjectStart();
          break;
        case JsonTokenType.EndObject:
          writer.WriteObjectEnd();
          break;
        case JsonTokenType.String:
        case JsonTokenType.FieldName:
          bool flag = currentTokenType == JsonTokenType.FieldName;
          Utf8Memory utf8Memory;
          if (this.TryGetBufferedStringValue(out utf8Memory))
          {
            if (flag)
            {
              writer.WriteFieldName(utf8Memory.Span);
              break;
            }
            writer.WriteStringValue(utf8Memory.Span);
            break;
          }
          string fieldName = UtfAnyString.op_Implicit(this.GetStringValue());
          if (flag)
          {
            writer.WriteFieldName(fieldName);
            break;
          }
          writer.WriteStringValue(fieldName);
          break;
        case JsonTokenType.Number:
          Number64 numberValue = this.GetNumberValue();
          writer.WriteNumber64Value(numberValue);
          break;
        case JsonTokenType.True:
          writer.WriteBoolValue(true);
          break;
        case JsonTokenType.False:
          writer.WriteBoolValue(false);
          break;
        case JsonTokenType.Null:
          writer.WriteNullValue();
          break;
        case JsonTokenType.Int8:
          sbyte int8Value = this.GetInt8Value();
          writer.WriteInt8Value(int8Value);
          break;
        case JsonTokenType.Int16:
          short int16Value = this.GetInt16Value();
          writer.WriteInt16Value(int16Value);
          break;
        case JsonTokenType.Int32:
          int int32Value = this.GetInt32Value();
          writer.WriteInt32Value(int32Value);
          break;
        case JsonTokenType.Int64:
          long int64Value = this.GetInt64Value();
          writer.WriteInt64Value(int64Value);
          break;
        case JsonTokenType.UInt32:
          uint uint32Value = this.GetUInt32Value();
          writer.WriteUInt32Value(uint32Value);
          break;
        case JsonTokenType.Float32:
          float float32Value = this.GetFloat32Value();
          writer.WriteFloat32Value(float32Value);
          break;
        case JsonTokenType.Float64:
          double float64Value = this.GetFloat64Value();
          writer.WriteFloat64Value(float64Value);
          break;
        case JsonTokenType.Guid:
          Guid guidValue = this.GetGuidValue();
          writer.WriteGuidValue(guidValue);
          break;
        case JsonTokenType.Binary:
          ReadOnlyMemory<byte> binaryValue = this.GetBinaryValue();
          writer.WriteBinaryValue(binaryValue.Span);
          break;
        default:
          throw new InvalidOperationException(string.Format("Unknown enum type: {0}.", (object) currentTokenType));
      }
    }

    public virtual void WriteAll(IJsonWriter writer)
    {
      while (this.Read())
        this.WriteCurrentToken(writer);
    }

    private sealed class JsonBinaryReader : JsonReader, ITypedJsonReader, IJsonReader
    {
      private readonly JsonReader.JsonBinaryReader.JsonBinaryMemoryReader jsonBinaryBuffer;
      private readonly Stack<int> arrayAndObjectEndStack;
      private readonly ReadOnlyMemory<byte> rootBuffer;
      private int currentTokenPosition;

      public JsonBinaryReader(ReadOnlyMemory<byte> buffer)
        : this(buffer, new int?())
      {
      }

      internal JsonBinaryReader(ReadOnlyMemory<byte> rootBuffer, int? indexToStartFrom = null)
      {
        this.rootBuffer = !rootBuffer.IsEmpty ? rootBuffer : throw new ArgumentException("rootBuffer must not be empty.");
        ReadOnlyMemory<byte> rootBuffer1 = this.rootBuffer;
        ReadOnlyMemory<byte> buffer = !indexToStartFrom.HasValue ? rootBuffer1.Slice(1) : rootBuffer1.Slice(indexToStartFrom.Value);
        int valueLength = JsonBinaryEncoding.GetValueLength(buffer.Span);
        if (buffer.Length < valueLength)
          throw new ArgumentException("buffer is shorter than the length prefix.");
        buffer = buffer.Slice(0, valueLength);
        this.jsonBinaryBuffer = new JsonReader.JsonBinaryReader.JsonBinaryMemoryReader(buffer);
        this.arrayAndObjectEndStack = new Stack<int>();
      }

      public override JsonSerializationFormat SerializationFormat => JsonSerializationFormat.Binary;

      public override bool Read()
      {
        if (!this.arrayAndObjectEndStack.Empty<int>() && this.arrayAndObjectEndStack.Peek() == this.jsonBinaryBuffer.Position)
        {
          if (this.JsonObjectState.InArrayContext)
          {
            this.JsonObjectState.RegisterEndArray();
          }
          else
          {
            if (!this.JsonObjectState.InObjectContext)
              throw new JsonInvalidTokenException();
            this.JsonObjectState.RegisterEndObject();
          }
          this.currentTokenPosition = this.jsonBinaryBuffer.Position;
          this.arrayAndObjectEndStack.Pop();
        }
        else
        {
          if (this.jsonBinaryBuffer.IsEof)
          {
            if (this.JsonObjectState.CurrentDepth == 0)
              return false;
            if (this.JsonObjectState.InObjectContext)
              throw new JsonMissingEndObjectException();
            if (this.JsonObjectState.InArrayContext)
              throw new JsonMissingEndArrayException();
            throw new InvalidOperationException("Expected to be in either array or object context");
          }
          if (this.JsonObjectState.CurrentDepth == 0 && this.CurrentTokenType != JsonTokenType.NotStarted)
            throw new JsonUnexpectedTokenException();
          ReadOnlySpan<byte> span = this.jsonBinaryBuffer.GetBufferedRawJsonToken().Span;
          int offset = JsonBinaryEncoding.GetValueLength(span);
          byte typeMarker = span[0];
          JsonTokenType jsonTokenType = JsonReader.JsonBinaryReader.GetJsonTokenType(typeMarker);
          switch (jsonTokenType)
          {
            case JsonTokenType.BeginArray:
            case JsonTokenType.BeginObject:
              this.arrayAndObjectEndStack.Push(this.jsonBinaryBuffer.Position + offset);
              offset = JsonReader.JsonBinaryReader.GetArrayOrObjectPrefixLength(typeMarker);
              break;
            case JsonTokenType.String:
              if (this.JsonObjectState.IsPropertyExpected)
              {
                jsonTokenType = JsonTokenType.FieldName;
                break;
              }
              break;
          }
          this.JsonObjectState.RegisterToken(jsonTokenType);
          this.currentTokenPosition = this.jsonBinaryBuffer.Position;
          this.jsonBinaryBuffer.SkipBytes(offset);
        }
        return true;
      }

      public override Number64 GetNumberValue()
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.Number)
          throw new JsonNotNumberTokenException();
        return JsonBinaryEncoding.GetNumberValue(this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition).Span);
      }

      public override UtfAnyString GetStringValue()
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.String && this.JsonObjectState.CurrentTokenType != JsonTokenType.FieldName)
          throw new JsonInvalidTokenException();
        return Utf8String.op_Implicit(JsonBinaryEncoding.GetUtf8StringValue(this.rootBuffer, this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition)));
      }

      public override bool TryGetBufferedStringValue(out Utf8Memory bufferedUtf8StringValue)
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.String && this.JsonObjectState.CurrentTokenType != JsonTokenType.FieldName)
          throw new JsonInvalidTokenException();
        return JsonBinaryEncoding.TryGetBufferedStringValue(this.rootBuffer, this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition), out bufferedUtf8StringValue);
      }

      public override sbyte GetInt8Value()
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.Int8)
          throw new JsonNotNumberTokenException();
        return JsonBinaryEncoding.GetInt8Value(this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition).Span);
      }

      public override short GetInt16Value()
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.Int16)
          throw new JsonNotNumberTokenException();
        return JsonBinaryEncoding.GetInt16Value(this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition).Span);
      }

      public override int GetInt32Value()
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.Int32)
          throw new JsonNotNumberTokenException();
        return JsonBinaryEncoding.GetInt32Value(this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition).Span);
      }

      public override long GetInt64Value()
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.Int64)
          throw new JsonNotNumberTokenException();
        return JsonBinaryEncoding.GetInt64Value(this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition).Span);
      }

      public override uint GetUInt32Value()
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.UInt32)
          throw new JsonNotNumberTokenException();
        return JsonBinaryEncoding.GetUInt32Value(this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition).Span);
      }

      public override float GetFloat32Value()
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.Float32)
          throw new JsonNotNumberTokenException();
        return JsonBinaryEncoding.GetFloat32Value(this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition).Span);
      }

      public override double GetFloat64Value()
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.Float64)
          throw new JsonNotNumberTokenException();
        return JsonBinaryEncoding.GetFloat64Value(this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition).Span);
      }

      public bool TryReadTypedJsonValueWrapper(out int typeCode)
      {
        int position = this.jsonBinaryBuffer.Position;
        if (this.CurrentTokenType != JsonTokenType.BeginObject || !this.JsonObjectState.IsPropertyExpected || this.arrayAndObjectEndStack.Peek() - position <= 3)
        {
          typeCode = 0;
          return false;
        }
        ReadOnlySpan<byte> span = this.jsonBinaryBuffer.GetBufferedRawJsonToken(position, position + 3).Span;
        if (span[0] == (byte) 33 && JsonBinaryEncoding.TypeMarker.IsEncodedNumberLiteral((long) span[1]) && span[2] == (byte) 34)
        {
          this.JsonObjectState.RegisterFieldName();
          this.jsonBinaryBuffer.SkipBytes(3);
          this.currentTokenPosition = position;
          typeCode = (int) span[1];
          return true;
        }
        typeCode = 0;
        return false;
      }

      public override Guid GetGuidValue()
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.Guid)
          throw new JsonNotNumberTokenException();
        return JsonBinaryEncoding.GetGuidValue(this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition).Span);
      }

      public override ReadOnlyMemory<byte> GetBinaryValue()
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.Binary)
          throw new JsonNotNumberTokenException();
        return JsonBinaryEncoding.GetBinaryValue(this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition));
      }

      public Utf8Span GetUtf8SpanValue()
      {
        if (this.JsonObjectState.CurrentTokenType != JsonTokenType.String && this.JsonObjectState.CurrentTokenType != JsonTokenType.FieldName)
          throw new JsonInvalidTokenException();
        return JsonBinaryEncoding.GetUtf8SpanValue(this.rootBuffer, this.jsonBinaryBuffer.GetBufferedRawJsonToken(this.currentTokenPosition));
      }

      private static JsonTokenType GetJsonTokenType(byte typeMarker)
      {
        int num = (int) JsonReader.TypeMarkerToTokenType[(int) typeMarker];
        return num != 0 ? (JsonTokenType) num : throw new JsonInvalidTokenException();
      }

      private static int GetArrayOrObjectPrefixLength(byte typeMarker)
      {
        switch (typeMarker)
        {
          case 224:
          case 225:
            return 1;
          case 226:
            return 2;
          case 227:
            return 3;
          case 228:
            return 5;
          case 229:
            return 3;
          case 230:
            return 5;
          case 231:
            return 9;
          case 232:
          case 233:
            return 1;
          case 234:
            return 2;
          case 235:
            return 3;
          case 236:
            return 5;
          case 237:
            return 3;
          case 238:
            return 5;
          case 239:
            return 9;
          default:
            throw new ArgumentException(string.Format("Unknown typemarker: {0}", (object) typeMarker));
        }
      }

      private sealed class JsonBinaryMemoryReader : JsonMemoryReader
      {
        public JsonBinaryMemoryReader(ReadOnlyMemory<byte> buffer)
          : base(buffer)
        {
        }

        public void SkipBytes(int offset) => this.position += offset;
      }
    }

    private sealed class JsonTextReader : 
      JsonReader,
      IJsonTextReaderPrivateImplementation,
      IJsonReader
    {
      private const char Int8TokenPrefix = 'I';
      private const char Int16TokenPrefix = 'H';
      private const char Int32TokenPrefix = 'L';
      private const char UnsignedTokenPrefix = 'U';
      private const char FloatTokenPrefix = 'S';
      private const char DoubleTokenPrefix = 'D';
      private const char GuidTokenPrefix = 'G';
      private const char BinaryTokenPrefix = 'B';
      private static readonly HashSet<char> EscapeCharacters = new HashSet<char>()
      {
        'b',
        'f',
        'n',
        'r',
        't',
        '\\',
        '"',
        '/',
        'u'
      };
      private readonly JsonReader.JsonTextReader.JsonTextMemoryReader jsonTextBuffer;
      private JsonReader.JsonTextReader.TokenState token;
      private bool hasSeperator;

      public JsonTextReader(ReadOnlyMemory<byte> buffer) => this.jsonTextBuffer = new JsonReader.JsonTextReader.JsonTextMemoryReader(buffer);

      public override JsonSerializationFormat SerializationFormat => JsonSerializationFormat.Text;

      public override bool Read()
      {
        this.jsonTextBuffer.AdvanceWhileWhitespace();
        if (this.jsonTextBuffer.IsEof)
        {
          if (this.JsonObjectState.CurrentDepth == 0)
            return false;
          if (this.JsonObjectState.InObjectContext)
            throw new JsonMissingEndObjectException();
          if (this.JsonObjectState.InArrayContext)
            throw new JsonMissingEndArrayException();
          throw new JsonNotCompleteException();
        }
        this.token.Start = this.jsonTextBuffer.Position;
        switch (this.jsonTextBuffer.PeekCharacter())
        {
          case '"':
            this.ProcessString();
            break;
          case ',':
            this.ProcessValueSeparator();
            break;
          case '-':
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
            this.ProcessNumber();
            break;
          case ':':
            this.ProcessNameSeparator();
            break;
          case 'B':
            this.ProcessBinary();
            break;
          case 'D':
            this.ProcessFloat64();
            break;
          case 'G':
            this.ProcessGuid();
            break;
          case 'H':
            this.ProcessInt16();
            break;
          case 'I':
            this.ProcessInt8();
            break;
          case 'L':
            this.ProcessInt32OrInt64();
            break;
          case 'S':
            this.ProcessFloat32();
            break;
          case 'U':
            this.ProcessUInt32();
            break;
          case '[':
            this.ProcessSingleByteToken(JsonReader.JsonTextReader.JsonTextTokenType.BeginArray);
            break;
          case ']':
            this.ProcessSingleByteToken(JsonReader.JsonTextReader.JsonTextTokenType.EndArray);
            break;
          case 'f':
            this.ProcessFalse();
            break;
          case 'n':
            this.ProcessNull();
            break;
          case 't':
            this.ProcessTrue();
            break;
          case '{':
            this.ProcessSingleByteToken(JsonReader.JsonTextReader.JsonTextTokenType.BeginObject);
            break;
          case '}':
            this.ProcessSingleByteToken(JsonReader.JsonTextReader.JsonTextTokenType.EndObject);
            break;
          default:
            throw new JsonUnexpectedTokenException();
        }
        this.token.End = this.jsonTextBuffer.Position;
        return true;
      }

      public override Number64 GetNumberValue() => JsonTextParser.GetNumberValue(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start, this.token.End).Span);

      public override UtfAnyString GetStringValue()
      {
        Utf8Memory utf8Memory;
        return this.TryGetBufferedStringValue(out utf8Memory) ? Utf8String.op_Implicit(Utf8String.UnsafeFromUtf8BytesNoValidation(utf8Memory.Memory)) : Utf8String.op_Implicit(JsonTextParser.GetStringValue(Utf8Memory.UnsafeCreateNoValidation(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start, this.token.End))));
      }

      public override bool TryGetBufferedStringValue(out Utf8Memory value)
      {
        if (this.token.JsonTextTokenType.HasFlag((Enum) JsonReader.JsonTextReader.JsonTextTokenType.EscapedFlag))
        {
          value = new Utf8Memory();
          return false;
        }
        value = Utf8Memory.UnsafeCreateNoValidation(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start + 1, this.token.End - 1));
        return true;
      }

      public override sbyte GetInt8Value() => JsonTextParser.GetInt8Value(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start, this.token.End).Span);

      public override short GetInt16Value() => JsonTextParser.GetInt16Value(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start, this.token.End).Span);

      public override int GetInt32Value() => JsonTextParser.GetInt32Value(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start, this.token.End).Span);

      public override long GetInt64Value() => JsonTextParser.GetInt64Value(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start, this.token.End).Span);

      public override uint GetUInt32Value() => JsonTextParser.GetUInt32Value(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start, this.token.End).Span);

      public override float GetFloat32Value() => JsonTextParser.GetFloat32Value(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start, this.token.End).Span);

      public override double GetFloat64Value() => JsonTextParser.GetFloat64Value(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start, this.token.End).Span);

      public override Guid GetGuidValue() => JsonTextParser.GetGuidValue(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start, this.token.End).Span);

      public override ReadOnlyMemory<byte> GetBinaryValue() => JsonTextParser.GetBinaryValue(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start, this.token.End).Span);

      Utf8Memory IJsonTextReaderPrivateImplementation.GetBufferedJsonToken() => Utf8Memory.UnsafeCreateNoValidation(this.jsonTextBuffer.GetBufferedRawJsonToken(this.token.Start, this.token.End));

      private static JsonTokenType JsonTextToJsonTokenType(
        JsonReader.JsonTextReader.JsonTextTokenType jsonTextTokenType)
      {
        return (JsonTokenType) (jsonTextTokenType & (JsonReader.JsonTextReader.JsonTextTokenType) 65535);
      }

      private void ProcessSingleByteToken(
        JsonReader.JsonTextReader.JsonTextTokenType jsonTextTokenType)
      {
        this.token.JsonTextTokenType = jsonTextTokenType;
        int num = (int) this.jsonTextBuffer.ReadCharacter();
        this.RegisterToken();
      }

      private void ProcessTrue()
      {
        this.token.JsonTextTokenType = JsonReader.JsonTextReader.JsonTextTokenType.True;
        if (!this.jsonTextBuffer.TryReadTrueToken())
          throw new JsonInvalidTokenException();
        this.RegisterToken();
      }

      private void ProcessFalse()
      {
        this.token.JsonTextTokenType = JsonReader.JsonTextReader.JsonTextTokenType.False;
        if (!this.jsonTextBuffer.TryReadFalseToken())
          throw new JsonInvalidTokenException();
        this.RegisterToken();
      }

      private void ProcessNull()
      {
        this.token.JsonTextTokenType = JsonReader.JsonTextReader.JsonTextTokenType.Null;
        if (!this.jsonTextBuffer.TryReadNullToken())
          throw new JsonInvalidTokenException();
        this.RegisterToken();
      }

      private void ProcessNumber()
      {
        this.ProcessNumberValueToken();
        this.token.JsonTextTokenType = JsonReader.JsonTextReader.JsonTextTokenType.Number;
        this.RegisterToken();
      }

      private void ProcessNumberValueToken()
      {
        this.token.JsonTextTokenType = JsonReader.JsonTextReader.JsonTextTokenType.Number;
        if (this.jsonTextBuffer.PeekCharacter() == '-')
        {
          int num1 = (int) this.jsonTextBuffer.ReadCharacter();
        }
        if (!char.IsDigit(this.jsonTextBuffer.PeekCharacter()))
          throw new JsonInvalidNumberException();
        if (this.jsonTextBuffer.PeekCharacter() == '0')
        {
          int num2 = (int) this.jsonTextBuffer.ReadCharacter();
          if (this.jsonTextBuffer.PeekCharacter() == '0')
            throw new JsonInvalidNumberException();
        }
        else
        {
          while (char.IsDigit(this.jsonTextBuffer.PeekCharacter()))
          {
            int num3 = (int) this.jsonTextBuffer.ReadCharacter();
          }
        }
        if (this.jsonTextBuffer.PeekCharacter() == '.')
        {
          this.token.JsonTextTokenType = JsonReader.JsonTextReader.JsonTextTokenType.Number;
          int num4 = (int) this.jsonTextBuffer.ReadCharacter();
          if (!char.IsDigit(this.jsonTextBuffer.PeekCharacter()))
            throw new JsonInvalidNumberException();
          while (char.IsDigit(this.jsonTextBuffer.PeekCharacter()))
          {
            int num5 = (int) this.jsonTextBuffer.ReadCharacter();
          }
        }
        if (this.jsonTextBuffer.PeekCharacter() == 'e' || this.jsonTextBuffer.PeekCharacter() == 'E')
        {
          this.token.JsonTextTokenType = JsonReader.JsonTextReader.JsonTextTokenType.Number;
          int num6 = (int) this.jsonTextBuffer.ReadCharacter();
          if (this.jsonTextBuffer.PeekCharacter() == '+' || this.jsonTextBuffer.PeekCharacter() == '-')
          {
            int num7 = (int) this.jsonTextBuffer.ReadCharacter();
          }
          if (!char.IsDigit(this.jsonTextBuffer.PeekCharacter()))
            throw new JsonInvalidNumberException();
          while (char.IsDigit(this.jsonTextBuffer.PeekCharacter()))
          {
            int num8 = (int) this.jsonTextBuffer.ReadCharacter();
          }
        }
        char ch = this.jsonTextBuffer.PeekCharacter();
        if (!this.jsonTextBuffer.IsEof && !JsonReader.JsonTextReader.JsonTextMemoryReader.IsWhitespace(ch) && ch != '}' && ch != ',' && ch != ']')
          throw new JsonInvalidNumberException();
      }

      private void ProcessInt8()
      {
        if (this.jsonTextBuffer.ReadCharacter() != 'I')
          throw new JsonInvalidTokenException();
        this.ProcessIntegerToken(JsonReader.JsonTextReader.JsonTextTokenType.Int8);
      }

      private void ProcessInt16()
      {
        if (this.jsonTextBuffer.ReadCharacter() != 'H')
          throw new JsonInvalidTokenException();
        this.ProcessIntegerToken(JsonReader.JsonTextReader.JsonTextTokenType.Int16);
      }

      private void ProcessInt32OrInt64()
      {
        if (this.jsonTextBuffer.ReadCharacter() != 'L')
          throw new JsonInvalidTokenException();
        if (this.jsonTextBuffer.PeekCharacter() == 'L')
        {
          if (this.jsonTextBuffer.ReadCharacter() != 'L')
            throw new JsonInvalidTokenException();
          this.ProcessIntegerToken(JsonReader.JsonTextReader.JsonTextTokenType.Int64);
        }
        else
          this.ProcessIntegerToken(JsonReader.JsonTextReader.JsonTextTokenType.Int32);
      }

      private void ProcessUInt32()
      {
        if (this.jsonTextBuffer.ReadCharacter() != 'U')
          throw new JsonInvalidTokenException();
        if (this.jsonTextBuffer.ReadCharacter() != 'L')
          throw new JsonInvalidTokenException();
        if (!char.IsDigit(this.jsonTextBuffer.PeekCharacter()))
          throw new JsonInvalidNumberException();
        this.ProcessIntegerToken(JsonReader.JsonTextReader.JsonTextTokenType.UInt32);
      }

      private void ProcessIntegerToken(
        JsonReader.JsonTextReader.JsonTextTokenType jsonTextTokenType)
      {
        if (this.jsonTextBuffer.PeekCharacter() == '-')
        {
          int num1 = (int) this.jsonTextBuffer.ReadCharacter();
        }
        if (!char.IsDigit(this.jsonTextBuffer.PeekCharacter()))
          throw new JsonInvalidNumberException();
        while (char.IsDigit(this.jsonTextBuffer.PeekCharacter()))
        {
          int num2 = (int) this.jsonTextBuffer.ReadCharacter();
        }
        this.token.JsonTextTokenType = jsonTextTokenType;
        this.RegisterToken();
      }

      private void ProcessFloat32()
      {
        if (this.jsonTextBuffer.ReadCharacter() != 'S')
          throw new JsonInvalidTokenException();
        this.ProcessNumberValueToken();
        this.token.JsonTextTokenType = JsonReader.JsonTextReader.JsonTextTokenType.Float32;
        this.RegisterToken();
      }

      private void ProcessFloat64()
      {
        if (this.jsonTextBuffer.ReadCharacter() != 'D')
          throw new JsonInvalidTokenException();
        this.ProcessNumberValueToken();
        this.token.JsonTextTokenType = JsonReader.JsonTextReader.JsonTextTokenType.Float64;
        this.RegisterToken();
      }

      private void ProcessGuid()
      {
        if (this.jsonTextBuffer.ReadCharacter() != 'G')
          throw new JsonInvalidTokenException();
        int num1 = 0;
        while (char.IsLetterOrDigit(this.jsonTextBuffer.PeekCharacter()) || this.jsonTextBuffer.PeekCharacter() == '-')
        {
          int num2 = (int) this.jsonTextBuffer.ReadCharacter();
          ++num1;
        }
        if (num1 != 36)
          throw new JsonInvalidTokenException();
        this.token.JsonTextTokenType = JsonReader.JsonTextReader.JsonTextTokenType.Guid;
        this.RegisterToken();
      }

      private void ProcessBinary()
      {
        if (this.jsonTextBuffer.ReadCharacter() != 'B')
          throw new JsonInvalidTokenException();
        for (char c = this.jsonTextBuffer.PeekCharacter(); char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '='; c = this.jsonTextBuffer.PeekCharacter())
        {
          int num = (int) this.jsonTextBuffer.ReadCharacter();
        }
        this.token.JsonTextTokenType = JsonReader.JsonTextReader.JsonTextTokenType.Binary;
        this.RegisterToken();
      }

      private void ProcessString()
      {
        this.token.JsonTextTokenType = this.JsonObjectState.IsPropertyExpected ? JsonReader.JsonTextReader.JsonTextTokenType.UnescapedFieldName : JsonReader.JsonTextReader.JsonTextTokenType.UnescapedString;
        if (this.jsonTextBuffer.ReadCharacter() != '"')
          throw new JsonUnexpectedTokenException();
        bool flag = false;
        while (!flag)
        {
          switch (this.jsonTextBuffer.ReadCharacter())
          {
            case char.MinValue:
              if (this.jsonTextBuffer.IsEof)
                throw new JsonMissingClosingQuoteException();
              continue;
            case '"':
              this.RegisterToken();
              flag = true;
              continue;
            case '\\':
              this.token.JsonTextTokenType |= JsonReader.JsonTextReader.JsonTextTokenType.EscapedFlag;
              char ch1 = this.jsonTextBuffer.ReadCharacter();
              if (ch1 == 'u')
              {
                for (int index = 0; index < 4; ++index)
                {
                  char ch2 = this.jsonTextBuffer.ReadCharacter();
                  if ((ch2 < '0' || ch2 > '9') && (ch2 < 'a' || ch2 > 'f') && (ch2 < 'A' || ch2 > 'F'))
                    throw new JsonInvalidEscapedCharacterException();
                }
                continue;
              }
              if (!JsonReader.JsonTextReader.EscapeCharacters.Contains(ch1))
                throw new JsonInvalidEscapedCharacterException();
              continue;
            default:
              continue;
          }
        }
      }

      private void ProcessNameSeparator()
      {
        if (this.hasSeperator || this.JsonObjectState.CurrentTokenType != JsonTokenType.FieldName)
          throw new JsonUnexpectedNameSeparatorException();
        int num = (int) this.jsonTextBuffer.ReadCharacter();
        this.hasSeperator = true;
        this.Read();
      }

      private void ProcessValueSeparator()
      {
        if (this.hasSeperator)
          throw new JsonUnexpectedValueSeparatorException();
        switch (this.JsonObjectState.CurrentTokenType)
        {
          case JsonTokenType.EndArray:
          case JsonTokenType.EndObject:
          case JsonTokenType.String:
          case JsonTokenType.Number:
          case JsonTokenType.True:
          case JsonTokenType.False:
          case JsonTokenType.Null:
          case JsonTokenType.Int8:
          case JsonTokenType.Int16:
          case JsonTokenType.Int32:
          case JsonTokenType.Int64:
          case JsonTokenType.UInt32:
          case JsonTokenType.Float32:
          case JsonTokenType.Float64:
          case JsonTokenType.Guid:
          case JsonTokenType.Binary:
            this.hasSeperator = true;
            int num = (int) this.jsonTextBuffer.ReadCharacter();
            this.Read();
            break;
          default:
            throw new JsonUnexpectedValueSeparatorException();
        }
      }

      private void RegisterToken()
      {
        JsonTokenType jsonTokenType = JsonReader.JsonTextReader.JsonTextToJsonTokenType(this.token.JsonTextTokenType);
        JsonTokenType currentTokenType = this.JsonObjectState.CurrentTokenType;
        this.JsonObjectState.RegisterToken(jsonTokenType);
        switch (jsonTokenType)
        {
          case JsonTokenType.EndArray:
            if (!this.hasSeperator)
              break;
            throw new JsonUnexpectedEndArrayException();
          case JsonTokenType.EndObject:
            if (!this.hasSeperator)
              break;
            throw new JsonUnexpectedEndObjectException();
          default:
            switch (currentTokenType)
            {
              case JsonTokenType.NotStarted:
              case JsonTokenType.BeginArray:
              case JsonTokenType.BeginObject:
                this.hasSeperator = false;
                return;
              case JsonTokenType.FieldName:
                if (!this.hasSeperator)
                  throw new JsonMissingNameSeparatorException();
                goto case JsonTokenType.NotStarted;
              default:
                if (!this.hasSeperator)
                  throw new JsonUnexpectedTokenException();
                goto case JsonTokenType.NotStarted;
            }
        }
      }

      private enum JsonTextTokenType
      {
        NotStarted = 0,
        BeginArray = 1,
        EndArray = 2,
        BeginObject = 3,
        EndObject = 4,
        UnescapedString = 5,
        Number = 6,
        True = 7,
        False = 8,
        Null = 9,
        UnescapedFieldName = 10, // 0x0000000A
        Int8 = 11, // 0x0000000B
        Int16 = 12, // 0x0000000C
        Int32 = 13, // 0x0000000D
        Int64 = 14, // 0x0000000E
        UInt32 = 15, // 0x0000000F
        Float32 = 16, // 0x00000010
        Float64 = 17, // 0x00000011
        Guid = 18, // 0x00000012
        Binary = 19, // 0x00000013
        EscapedFlag = 65536, // 0x00010000
        FloatFlag = 65536, // 0x00010000
        EscapedString = 65541, // 0x00010005
        EscapedFieldName = 65546, // 0x0001000A
      }

      private sealed class JsonTextMemoryReader : JsonMemoryReader
      {
        private static readonly ReadOnlyMemory<byte> TrueMemory = (ReadOnlyMemory<byte>) new byte[4]
        {
          (byte) 116,
          (byte) 114,
          (byte) 117,
          (byte) 101
        };
        private static readonly ReadOnlyMemory<byte> FalseMemory = (ReadOnlyMemory<byte>) new byte[5]
        {
          (byte) 102,
          (byte) 97,
          (byte) 108,
          (byte) 115,
          (byte) 101
        };
        private static readonly ReadOnlyMemory<byte> NullMemory = (ReadOnlyMemory<byte>) new byte[4]
        {
          (byte) 110,
          (byte) 117,
          (byte) 108,
          (byte) 108
        };

        public JsonTextMemoryReader(ReadOnlyMemory<byte> buffer)
          : base(buffer)
        {
        }

        public char ReadCharacter() => (char) this.Read();

        public char PeekCharacter() => (char) this.Peek();

        public void AdvanceWhileWhitespace()
        {
          while (JsonReader.JsonTextReader.JsonTextMemoryReader.IsWhitespace(this.PeekCharacter()))
          {
            int num = (int) this.ReadCharacter();
          }
        }

        public bool TryReadTrueToken() => this.TryReadToken(JsonReader.JsonTextReader.JsonTextMemoryReader.TrueMemory.Span);

        public bool TryReadFalseToken() => this.TryReadToken(JsonReader.JsonTextReader.JsonTextMemoryReader.FalseMemory.Span);

        public bool TryReadNullToken() => this.TryReadToken(JsonReader.JsonTextReader.JsonTextMemoryReader.NullMemory.Span);

        private bool TryReadToken(ReadOnlySpan<byte> token)
        {
          if (this.position + token.Length > this.buffer.Length)
            return false;
          int num = this.buffer.Slice(this.position, token.Length).Span.SequenceEqual<byte>(token) ? 1 : 0;
          this.position += token.Length;
          return num != 0;
        }

        public static bool IsWhitespace(char value) => value == ' ' || value == '\t' || value == '\r' || value == '\n';
      }

      private struct TokenState
      {
        public JsonReader.JsonTextReader.JsonTextTokenType JsonTextTokenType { get; set; }

        public int Start { get; set; }

        public int End { get; set; }
      }
    }
  }
}
