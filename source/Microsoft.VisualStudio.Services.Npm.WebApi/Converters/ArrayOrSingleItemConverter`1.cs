// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.ArrayOrSingleItemConverter`1
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public class ArrayOrSingleItemConverter<T> : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => objectType == typeof (T);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.StartArray)
        return serializer.Deserialize(reader, objectType);
      if (string.IsNullOrEmpty(reader.Value.ToString()))
        return (object) null;
      return (object) new T[1]
      {
        serializer.Deserialize<T>(reader)
      };
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
