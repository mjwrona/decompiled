// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.StrictBooleanConverter
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  internal sealed class StrictBooleanConverter : JsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => objectType == typeof (bool);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Boolean)
        return serializer.Deserialize(reader, objectType);
      throw new JsonSerializationException(string.Format("Token \"{0}\" of type {1} was not a JSON bool", reader.Value, (object) reader.TokenType));
    }

    [ExcludeFromCodeCoverage]
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
    }
  }
}
