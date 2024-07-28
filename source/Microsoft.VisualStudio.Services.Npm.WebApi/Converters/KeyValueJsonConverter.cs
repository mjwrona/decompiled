// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.KeyValueJsonConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public class KeyValueJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        throw new InvalidPackageJsonException("Unexpected Format.");
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      while (reader.Read() && JsonToken.EndObject != reader.TokenType)
      {
        if (JsonToken.PropertyName != reader.TokenType)
          throw new InvalidPackageJsonException("Unexpected Format.");
        if (reader.Value is string key)
        {
          reader.Read();
          object obj = serializer.Deserialize(reader);
          dictionary.Add(key, obj.ToString());
        }
      }
      return (object) dictionary;
    }

    public override bool CanConvert(Type objectType) => objectType == typeof (Dictionary<string, string>);
  }
}
