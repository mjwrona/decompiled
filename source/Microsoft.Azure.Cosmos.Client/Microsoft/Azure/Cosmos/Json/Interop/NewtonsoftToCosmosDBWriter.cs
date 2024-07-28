// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.Interop.NewtonsoftToCosmosDBWriter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using System;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Cosmos.Json.Interop
{
  internal sealed class NewtonsoftToCosmosDBWriter : Microsoft.Azure.Cosmos.Json.JsonWriter
  {
    private readonly Newtonsoft.Json.JsonWriter writer;
    private readonly Func<byte[]> getResultCallback;

    private NewtonsoftToCosmosDBWriter(Newtonsoft.Json.JsonWriter writer, Func<byte[]> getResultCallback)
    {
      this.writer = writer ?? throw new ArgumentNullException(nameof (writer));
      this.getResultCallback = getResultCallback ?? throw new ArgumentNullException(nameof (getResultCallback));
    }

    public override long CurrentLength => throw new NotImplementedException();

    public override JsonSerializationFormat SerializationFormat => JsonSerializationFormat.Text;

    public override ReadOnlyMemory<byte> GetResult() => (ReadOnlyMemory<byte>) this.getResultCallback();

    public override void WriteArrayEnd() => this.writer.WriteEndArray();

    public override void WriteArrayStart() => this.writer.WriteStartArray();

    public override void WriteBinaryValue(ReadOnlySpan<byte> value) => throw new NotImplementedException();

    public override void WriteBoolValue(bool value) => this.writer.WriteValue(value);

    public override void WriteFieldName(string fieldName) => this.writer.WritePropertyName(fieldName);

    public override void WriteFloat32Value(float value) => this.writer.WriteValue(value);

    public override void WriteFloat64Value(double value) => this.writer.WriteValue(value);

    public override void WriteGuidValue(Guid value) => this.writer.WriteValue(value);

    public override void WriteInt16Value(short value) => this.writer.WriteValue(value);

    public override void WriteInt32Value(int value) => this.writer.WriteValue(value);

    public override void WriteInt64Value(long value) => this.writer.WriteValue(value);

    public override void WriteInt8Value(sbyte value) => this.writer.WriteValue(value);

    public override void WriteNullValue() => this.writer.WriteNull();

    public override void WriteNumber64Value(Number64 value)
    {
      if (value.IsInteger)
        this.writer.WriteValue(Number64.ToLong(value));
      else
        this.writer.WriteValue(Number64.ToDouble(value));
    }

    public override void WriteObjectEnd() => this.writer.WriteEndObject();

    public override void WriteObjectStart() => this.writer.WriteStartObject();

    public override void WriteStringValue(string value) => this.writer.WriteValue(value);

    public override void WriteUInt32Value(uint value) => this.writer.WriteValue(value);

    public static NewtonsoftToCosmosDBWriter CreateTextWriter()
    {
      StringWriter stringWriter = new StringWriter();
      return new NewtonsoftToCosmosDBWriter((Newtonsoft.Json.JsonWriter) new Newtonsoft.Json.JsonTextWriter((TextWriter) stringWriter), (Func<byte[]>) (() => Encoding.UTF8.GetBytes(stringWriter.ToString())));
    }

    public static NewtonsoftToCosmosDBWriter CreateFromWriter(Newtonsoft.Json.JsonWriter writer) => writer != null ? new NewtonsoftToCosmosDBWriter(writer, (Func<byte[]>) (() =>
    {
      throw new NotSupportedException();
    })) : throw new ArgumentNullException(nameof (writer));

    public override void WriteFieldName(Utf8Span utf8FieldName) => this.WriteFieldName(Encoding.UTF8.GetString(((Utf8Span) ref utf8FieldName).Span));

    public override void WriteStringValue(Utf8Span utf8StringValue) => this.WriteStringValue(Encoding.UTF8.GetString(((Utf8Span) ref utf8StringValue).Span));
  }
}
