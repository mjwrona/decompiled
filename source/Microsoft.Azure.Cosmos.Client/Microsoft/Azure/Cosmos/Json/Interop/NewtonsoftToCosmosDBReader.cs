// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.Interop.NewtonsoftToCosmosDBReader
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Azure.Cosmos.Json.Interop
{
  internal sealed class NewtonsoftToCosmosDBReader : Microsoft.Azure.Cosmos.Json.JsonReader
  {
    private readonly Newtonsoft.Json.JsonReader reader;

    private NewtonsoftToCosmosDBReader(Newtonsoft.Json.JsonReader reader) => this.reader = reader ?? throw new ArgumentNullException(nameof (reader));

    public override JsonSerializationFormat SerializationFormat => JsonSerializationFormat.Text;

    public override ReadOnlyMemory<byte> GetBinaryValue() => throw new NotImplementedException();

    public override float GetFloat32Value() => (float) this.reader.Value;

    public override double GetFloat64Value() => (double) this.reader.Value;

    public override Guid GetGuidValue() => (Guid) this.reader.Value;

    public override short GetInt16Value() => (short) this.reader.Value;

    public override int GetInt32Value() => (int) this.reader.Value;

    public override long GetInt64Value() => (long) this.reader.Value;

    public override sbyte GetInt8Value() => (sbyte) this.reader.Value;

    public override Number64 GetNumberValue()
    {
      object obj = this.reader.Value;
      return (Number64) double.Parse(!(obj is double _) ? obj.ToString() : ((double) obj).ToString("R"));
    }

    public override UtfAnyString GetStringValue() => UtfAnyString.op_Implicit(this.reader.Value.ToString());

    public override uint GetUInt32Value() => (uint) this.reader.Value;

    public override bool Read()
    {
      bool flag1 = this.reader.Read();
      if (flag1)
      {
        switch (this.reader.TokenType)
        {
          case JsonToken.StartObject:
            this.JsonObjectState.RegisterToken(JsonTokenType.BeginObject);
            break;
          case JsonToken.StartArray:
            this.JsonObjectState.RegisterToken(JsonTokenType.BeginArray);
            break;
          case JsonToken.PropertyName:
            this.JsonObjectState.RegisterToken(JsonTokenType.FieldName);
            break;
          case JsonToken.Integer:
          case JsonToken.Float:
            this.JsonObjectState.RegisterToken(JsonTokenType.Number);
            break;
          case JsonToken.String:
            this.JsonObjectState.RegisterToken(JsonTokenType.String);
            break;
          case JsonToken.Boolean:
            this.JsonObjectState.RegisterToken(!(this.reader.Value is bool flag2) || !flag2 ? JsonTokenType.False : JsonTokenType.True);
            break;
          case JsonToken.Null:
            this.JsonObjectState.RegisterToken(JsonTokenType.Null);
            break;
          case JsonToken.EndObject:
            this.JsonObjectState.RegisterToken(JsonTokenType.EndObject);
            break;
          case JsonToken.EndArray:
            this.JsonObjectState.RegisterToken(JsonTokenType.EndArray);
            break;
          default:
            throw new ArgumentException("Got an invalid newtonsoft type");
        }
      }
      return flag1;
    }

    public static NewtonsoftToCosmosDBReader CreateFromBuffer(ReadOnlyMemory<byte> buffer)
    {
      ArraySegment<byte> segment;
      Newtonsoft.Json.JsonTextReader reader = new Newtonsoft.Json.JsonTextReader((TextReader) new StreamReader(!MemoryMarshal.TryGetArray<byte>(buffer, out segment) ? (Stream) new MemoryStream(buffer.ToArray()) : (Stream) new MemoryStream(segment.Array, segment.Offset, segment.Count), Encoding.UTF8));
      reader.DateParseHandling = DateParseHandling.None;
      return NewtonsoftToCosmosDBReader.CreateFromReader((Newtonsoft.Json.JsonReader) reader);
    }

    public static NewtonsoftToCosmosDBReader CreateFromString(string json)
    {
      Newtonsoft.Json.JsonTextReader reader = json != null ? new Newtonsoft.Json.JsonTextReader((TextReader) new StringReader(json)) : throw new ArgumentNullException(nameof (json));
      reader.DateParseHandling = DateParseHandling.None;
      return NewtonsoftToCosmosDBReader.CreateFromReader((Newtonsoft.Json.JsonReader) reader);
    }

    public static NewtonsoftToCosmosDBReader CreateFromReader(Newtonsoft.Json.JsonReader reader) => reader != null ? new NewtonsoftToCosmosDBReader(reader) : throw new ArgumentNullException(nameof (reader));

    public override bool TryGetBufferedStringValue(out Utf8Memory bufferedUtf8StringValue)
    {
      bufferedUtf8StringValue = new Utf8Memory();
      return false;
    }
  }
}
