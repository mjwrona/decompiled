// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.IJsonReader
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using System;

namespace Microsoft.Azure.Cosmos.Json
{
  internal interface IJsonReader
  {
    JsonSerializationFormat SerializationFormat { get; }

    int CurrentDepth { get; }

    JsonTokenType CurrentTokenType { get; }

    bool Read();

    Number64 GetNumberValue();

    UtfAnyString GetStringValue();

    bool TryGetBufferedStringValue(out Utf8Memory value);

    sbyte GetInt8Value();

    short GetInt16Value();

    int GetInt32Value();

    long GetInt64Value();

    uint GetUInt32Value();

    float GetFloat32Value();

    double GetFloat64Value();

    Guid GetGuidValue();

    ReadOnlyMemory<byte> GetBinaryValue();

    void WriteCurrentToken(IJsonWriter writer);

    void WriteAll(IJsonWriter writer);
  }
}
