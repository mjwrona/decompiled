// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.Converters.CrsJsonConverter
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Azure.Documents.Spatial.Converters
{
  internal sealed class CrsJsonConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      Crs crs = (Crs) value;
      switch (crs.Type)
      {
        case CrsType.Named:
          NamedCrs namedCrs = (NamedCrs) crs;
          writer.WriteStartObject();
          writer.WritePropertyName("type");
          writer.WriteValue("name");
          writer.WritePropertyName("properties");
          writer.WriteStartObject();
          writer.WritePropertyName("name");
          writer.WriteValue(namedCrs.Name);
          writer.WriteEndObject();
          writer.WriteEndObject();
          break;
        case CrsType.Linked:
          LinkedCrs linkedCrs = (LinkedCrs) crs;
          writer.WriteStartObject();
          writer.WritePropertyName("type");
          writer.WriteValue("link");
          writer.WritePropertyName("properties");
          writer.WriteStartObject();
          writer.WritePropertyName("href");
          writer.WriteValue(linkedCrs.Href);
          if (linkedCrs.HrefType != null)
          {
            writer.WritePropertyName("type");
            writer.WriteValue(linkedCrs.HrefType);
          }
          writer.WriteEndObject();
          writer.WriteEndObject();
          break;
        case CrsType.Unspecified:
          writer.WriteNull();
          break;
      }
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JToken jtoken1 = JToken.Load(reader);
      if (jtoken1.Type == JTokenType.Null)
        return (object) Crs.Unspecified;
      JToken jtoken2 = jtoken1.Type == JTokenType.Object ? jtoken1[(object) "properties"] : throw new JsonSerializationException(RMResources.SpatialFailedToDeserializeCrs);
      if (jtoken2 == null || jtoken2.Type != JTokenType.Object)
        throw new JsonSerializationException(RMResources.SpatialFailedToDeserializeCrs);
      JToken jtoken3 = jtoken1[(object) "type"];
      if (jtoken3 == null || jtoken3.Type != JTokenType.String)
        throw new JsonSerializationException(RMResources.SpatialFailedToDeserializeCrs);
      switch (jtoken3.Value<string>())
      {
        case "name":
          JToken jtoken4 = jtoken2[(object) "name"];
          return jtoken4 != null && jtoken4.Type == JTokenType.String ? (object) new NamedCrs(jtoken4.Value<string>()) : throw new JsonSerializationException(RMResources.SpatialFailedToDeserializeCrs);
        case "link":
          JToken jtoken5 = jtoken2[(object) "href"];
          JToken jtoken6 = jtoken2[(object) "type"];
          if (jtoken5 == null || jtoken5.Type != JTokenType.String || jtoken6 != null && jtoken5.Type != JTokenType.String)
            throw new JsonSerializationException(RMResources.SpatialFailedToDeserializeCrs);
          return (object) new LinkedCrs(jtoken5.Value<string>(), jtoken6.Value<string>());
        default:
          throw new JsonSerializationException(RMResources.SpatialFailedToDeserializeCrs);
      }
    }

    public override bool CanConvert(Type objectType) => CustomTypeExtensions.IsAssignableFrom(typeof (Crs), objectType);
  }
}
