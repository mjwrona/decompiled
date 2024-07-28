// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointIsReadyConverter`1
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  internal class EndpointIsReadyConverter<T> : JsonConverter
  {
    public override bool CanConvert(Type objectType) => true;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Boolean || reader.TokenType == JsonToken.Integer)
        return (object) serializer.Deserialize<T>(reader);
      if (reader.TokenType != JsonToken.String)
        return (object) true;
      string str = (string) reader.Value;
      return str.Equals("false", StringComparison.OrdinalIgnoreCase) || str.Equals("0", StringComparison.OrdinalIgnoreCase) ? (object) false : (object) true;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => writer.WriteValue((bool) value);
  }
}
