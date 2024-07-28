// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.RangeJsonConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal sealed class RangeJsonConverter : JsonConverter
  {
    private static readonly string MinProperty = "min";
    private static readonly string MaxProperty = "max";

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      try
      {
        Range<string> range = (Range<string>) value;
        writer.WriteStartObject();
        writer.WritePropertyName(RangeJsonConverter.MinProperty);
        serializer.Serialize(writer, (object) range.Min);
        writer.WritePropertyName(RangeJsonConverter.MaxProperty);
        serializer.Serialize(writer, (object) range.Max);
        writer.WriteEndObject();
      }
      catch (Exception ex)
      {
        throw new JsonSerializationException(string.Empty, ex);
      }
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      try
      {
        JObject jobject = JObject.Load(reader);
        return (object) new Range<string>(jobject[RangeJsonConverter.MinProperty].Value<string>(), jobject[RangeJsonConverter.MaxProperty].Value<string>(), true, false);
      }
      catch (Exception ex)
      {
        throw new JsonSerializationException(string.Empty, ex);
      }
    }

    public override bool CanConvert(Type objectType) => typeof (PartitionKeyRange).IsAssignableFrom(objectType);
  }
}
