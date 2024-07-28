// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.StrictIntegerConverter
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  internal sealed class StrictIntegerConverter : JsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => StrictIntegerConverter.IsIntegerType(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Integer)
        return serializer.Deserialize(reader, objectType);
      throw new JsonSerializationException(string.Format("Token \"{0}\" of type {1} was not a JSON integer", reader.Value, (object) reader.TokenType));
    }

    [ExcludeFromCodeCoverage]
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
    }

    private static bool IsIntegerType(Type type) => type == typeof (long) || type == typeof (ulong) || type == typeof (int) || type == typeof (uint) || type == typeof (short) || type == typeof (ushort) || type == typeof (byte) || type == typeof (sbyte) || type == typeof (BigInteger);
  }
}
