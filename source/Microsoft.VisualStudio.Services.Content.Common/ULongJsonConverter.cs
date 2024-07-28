// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ULongJsonConverter
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class ULongJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (ulong);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      Newtonsoft.Json.JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      if (reader.TokenType != JsonToken.Integer)
        throw new JsonReaderException(string.Format("Unexected token type {0}. Expected an (unsigned) integer.", (object) reader.TokenType));
      try
      {
        return (long) reader.Value >= 0L ? (object) (ulong) (long) reader.Value : throw new JsonReaderException(string.Format("Unexected value {0}. Expected a non-negative integer.", reader.Value));
      }
      catch (Exception ex)
      {
        throw new JsonReaderException("Failed to read " + objectType.Name, ex);
      }
    }

    public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer) => writer.WriteValue((ulong) value);
  }
}
