// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Internal.Utf8StringJsonConverter
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Internal
{
  internal class Utf8StringJsonConverter : JsonConverter
  {
    public override bool CanWrite => true;

    public override bool CanConvert(Type objectType) => typeof (Utf8String).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      Contract.Requires(reader.TokenType == JsonToken.String);
      return (object) Utf8String.TranscodeUtf16((string) reader.Value);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => writer.WriteValue(((Utf8String) value).ToString());
  }
}
