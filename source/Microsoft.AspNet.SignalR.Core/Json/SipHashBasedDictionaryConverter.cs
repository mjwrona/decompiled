// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Json.SipHashBasedDictionaryConverter
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Json
{
  internal class SipHashBasedDictionaryConverter : JsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => objectType == typeof (IDictionary<string, object>);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return this.ReadJsonObject(reader);
    }

    private object ReadJsonObject(JsonReader reader)
    {
      switch (reader.TokenType)
      {
        case JsonToken.StartObject:
          return this.ReadObject(reader);
        case JsonToken.StartArray:
          return this.ReadArray(reader);
        case JsonToken.Integer:
        case JsonToken.Float:
        case JsonToken.String:
        case JsonToken.Boolean:
        case JsonToken.Null:
        case JsonToken.Undefined:
        case JsonToken.Date:
        case JsonToken.Bytes:
          return reader.Value;
        default:
          throw new NotSupportedException();
      }
    }

    private object ReadArray(JsonReader reader)
    {
      List<object> objectList = new List<object>();
      while (reader.Read())
      {
        if (reader.TokenType == JsonToken.EndArray)
          return (object) objectList;
        object obj = this.ReadJsonObject(reader);
        objectList.Add(obj);
      }
      throw new JsonSerializationException(Resources.Error_ParseObjectFailed);
    }

    private object ReadObject(JsonReader reader)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) new SipHashBasedStringEqualityComparer());
      while (reader.Read())
      {
        switch (reader.TokenType)
        {
          case JsonToken.PropertyName:
            string key = reader.Value.ToString();
            object obj = reader.Read() ? this.ReadJsonObject(reader) : throw new JsonSerializationException(Resources.Error_ParseObjectFailed);
            dictionary[key] = obj;
            continue;
          case JsonToken.EndObject:
            return (object) dictionary;
          default:
            throw new JsonSerializationException(Resources.Error_ParseObjectFailed);
        }
      }
      throw new JsonSerializationException(Resources.Error_ParseObjectFailed);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
