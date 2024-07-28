// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.IJsonWriter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using System;

namespace Microsoft.Azure.Cosmos.Json
{
  internal interface IJsonWriter
  {
    JsonSerializationFormat SerializationFormat { get; }

    long CurrentLength { get; }

    void WriteObjectStart();

    void WriteObjectEnd();

    void WriteArrayStart();

    void WriteArrayEnd();

    void WriteFieldName(string fieldName);

    void WriteFieldName(Utf8Span fieldName);

    void WriteStringValue(string value);

    void WriteStringValue(Utf8Span value);

    void WriteNumber64Value(Number64 value);

    void WriteBoolValue(bool value);

    void WriteNullValue();

    void WriteInt8Value(sbyte value);

    void WriteInt16Value(short value);

    void WriteInt32Value(int value);

    void WriteInt64Value(long value);

    void WriteFloat32Value(float value);

    void WriteFloat64Value(double value);

    void WriteUInt32Value(uint value);

    void WriteGuidValue(Guid value);

    void WriteBinaryValue(ReadOnlySpan<byte> value);

    ReadOnlyMemory<byte> GetResult();
  }
}
