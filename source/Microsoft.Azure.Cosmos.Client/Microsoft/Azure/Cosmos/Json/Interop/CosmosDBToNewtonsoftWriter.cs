// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.Interop.CosmosDBToNewtonsoftWriter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Text;

namespace Microsoft.Azure.Cosmos.Json.Interop
{
  internal sealed class CosmosDBToNewtonsoftWriter : Newtonsoft.Json.JsonWriter
  {
    private readonly IJsonWriter jsonWriter;

    public CosmosDBToNewtonsoftWriter(JsonSerializationFormat jsonSerializationFormat) => this.jsonWriter = Microsoft.Azure.Cosmos.Json.JsonWriter.Create(jsonSerializationFormat);

    public override void Flush() => this.jsonWriter.GetResult();

    public override void WriteComment(string text) => throw new NotSupportedException("Cannot write JSON comment.");

    public override void WriteEndArray()
    {
      base.WriteEndArray();
      this.jsonWriter.WriteArrayEnd();
    }

    public override void WriteEndConstructor() => throw new NotSupportedException("Cannot write end constructor.");

    public override void WriteEndObject()
    {
      base.WriteEndObject();
      this.jsonWriter.WriteObjectEnd();
    }

    public override void WriteNull()
    {
      base.WriteNull();
      this.jsonWriter.WriteNullValue();
    }

    public override void WritePropertyName(string name) => this.WritePropertyName(name, false);

    public override void WritePropertyName(string name, bool escape)
    {
      base.WritePropertyName(name);
      this.jsonWriter.WriteFieldName(name);
    }

    public override void WriteStartConstructor(string name) => throw new NotSupportedException("Cannot write Start constructor.");

    public override void WriteRaw(string json) => Microsoft.Azure.Cosmos.Json.JsonReader.Create((ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(json)).WriteAll(this.jsonWriter);

    public override void WriteRawValue(string json) => Microsoft.Azure.Cosmos.Json.JsonReader.Create((ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(json)).WriteAll(this.jsonWriter);

    public override void WriteStartArray()
    {
      base.WriteStartArray();
      this.jsonWriter.WriteArrayStart();
    }

    public override void WriteStartObject()
    {
      base.WriteStartObject();
      this.jsonWriter.WriteObjectStart();
    }

    public override void WriteUndefined() => throw new NotSupportedException("Can not write undefined");

    public override void WriteValue(object value)
    {
      if (value is string str)
        this.WriteValue(str);
      else
        this.WriteValue((double) value);
    }

    public override void WriteValue(string value)
    {
      base.WriteValue(value);
      this.jsonWriter.WriteStringValue(value);
    }

    public override void WriteValue(int value) => this.WriteValue((long) value);

    public override void WriteValue(uint value) => this.WriteValue((long) value);

    public override void WriteValue(long value)
    {
      base.WriteValue(value);
      this.jsonWriter.WriteNumber64Value((Number64) value);
    }

    public override void WriteValue(ulong value)
    {
      if (value <= (ulong) long.MaxValue)
        this.WriteValue((long) value);
      else
        this.WriteValue((double) value);
    }

    public override void WriteValue(float value) => this.WriteValue((double) value);

    public override void WriteValue(double value)
    {
      base.WriteValue(value);
      this.jsonWriter.WriteNumber64Value((Number64) value);
    }

    public override void WriteValue(bool value)
    {
      base.WriteValue(value);
      this.jsonWriter.WriteBoolValue(value);
    }

    public override void WriteValue(short value) => base.WriteValue((long) value);

    public override void WriteValue(ushort value) => this.WriteValue((long) value);

    public override void WriteValue(char value)
    {
      base.WriteValue(value);
      this.jsonWriter.WriteStringValue(value.ToString());
    }

    public override void WriteValue(byte value) => this.WriteValue((long) value);

    public override void WriteValue(sbyte value) => this.WriteValue((long) value);

    public override void WriteValue(Decimal value) => this.WriteValue((double) value);

    public override void WriteValue(DateTime value) => this.WriteValue(value.ToString());

    public override void WriteValue(byte[] value) => throw new NotSupportedException("Can not write byte arrays");

    public override void WriteValue(Guid value) => this.WriteValue(value.ToString());

    public override void WriteValue(TimeSpan value) => this.WriteValue(value.ToString());

    public override void WriteValue(Uri value)
    {
      if (value == (Uri) null)
        this.WriteNull();
      else
        this.WriteValue(value.ToString());
    }

    public ReadOnlyMemory<byte> GetResult() => this.jsonWriter.GetResult();
  }
}
