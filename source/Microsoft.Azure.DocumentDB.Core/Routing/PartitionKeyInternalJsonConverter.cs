// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.PartitionKeyInternalJsonConverter
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Documents.Routing
{
  internal sealed class PartitionKeyInternalJsonConverter : JsonConverter
  {
    private const string Type = "type";
    private const string MinNumber = "MinNumber";
    private const string MaxNumber = "MaxNumber";
    private const string MinString = "MinString";
    private const string MaxString = "MaxString";
    private const string Infinity = "Infinity";

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      PartitionKeyInternal partitionKeyInternal = (PartitionKeyInternal) value;
      if (partitionKeyInternal.Equals(PartitionKeyInternal.ExclusiveMaximum))
      {
        writer.WriteValue("Infinity");
      }
      else
      {
        writer.WriteStartArray();
        foreach (IPartitionKeyComponent component in (IEnumerable<IPartitionKeyComponent>) partitionKeyInternal.Components)
          component.JsonEncode(writer);
        writer.WriteEndArray();
      }
    }

    public override object ReadJson(
      JsonReader reader,
      System.Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JToken jtoken1 = JToken.Load(reader);
      if (jtoken1.Type == JTokenType.String && jtoken1.Value<string>() == "Infinity")
        return (object) PartitionKeyInternal.ExclusiveMaximum;
      List<object> values = new List<object>();
      if (jtoken1.Type != JTokenType.Array)
        throw new JsonSerializationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.UnableToDeserializePartitionKeyValue, (object) jtoken1));
      foreach (JToken jtoken2 in (JArray) jtoken1)
      {
        switch (jtoken2)
        {
          case JObject _:
            JObject jobject = (JObject) jtoken2;
            if (!jobject.Properties().Any<JProperty>())
            {
              values.Add((object) Undefined.Value);
              continue;
            }
            bool flag = false;
            JToken jtoken3;
            if (jobject.TryGetValue("type", out jtoken3) && jtoken3.Type == JTokenType.String)
            {
              flag = true;
              if (jtoken3.Value<string>() == "MinNumber")
                values.Add((object) Microsoft.Azure.Documents.Routing.MinNumber.Value);
              else if (jtoken3.Value<string>() == "MaxNumber")
                values.Add((object) Microsoft.Azure.Documents.Routing.MaxNumber.Value);
              else if (jtoken3.Value<string>() == "MinString")
                values.Add((object) Microsoft.Azure.Documents.Routing.MinString.Value);
              else if (jtoken3.Value<string>() == "MaxString")
                values.Add((object) Microsoft.Azure.Documents.Routing.MaxString.Value);
              else
                flag = false;
            }
            if (!flag)
              throw new JsonSerializationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.UnableToDeserializePartitionKeyValue, (object) jtoken1));
            continue;
          case JValue _:
            values.Add(((JValue) jtoken2).Value);
            continue;
          default:
            throw new JsonSerializationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.UnableToDeserializePartitionKeyValue, (object) jtoken1));
        }
      }
      return (object) PartitionKeyInternal.FromObjectArray((IEnumerable<object>) values, true);
    }

    public override bool CanConvert(System.Type objectType) => CustomTypeExtensions.IsAssignableFrom(typeof (PartitionKeyInternal), objectType);

    public static void JsonEncode(MinNumberPartitionKeyComponent component, JsonWriter writer) => PartitionKeyInternalJsonConverter.JsonEncodeLimit(writer, "MinNumber");

    public static void JsonEncode(MaxNumberPartitionKeyComponent component, JsonWriter writer) => PartitionKeyInternalJsonConverter.JsonEncodeLimit(writer, "MaxNumber");

    public static void JsonEncode(MinStringPartitionKeyComponent component, JsonWriter writer) => PartitionKeyInternalJsonConverter.JsonEncodeLimit(writer, "MinString");

    public static void JsonEncode(MaxStringPartitionKeyComponent component, JsonWriter writer) => PartitionKeyInternalJsonConverter.JsonEncodeLimit(writer, "MaxString");

    private static void JsonEncodeLimit(JsonWriter writer, string value)
    {
      writer.WriteStartObject();
      writer.WritePropertyName("type");
      writer.WriteValue(value);
      writer.WriteEndObject();
    }
  }
}
