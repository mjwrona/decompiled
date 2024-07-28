// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.RangeJsonConverter
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Azure.Documents.Routing
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

    public override bool CanConvert(Type objectType) => CustomTypeExtensions.IsAssignableFrom(typeof (PartitionKeyRange), objectType);
  }
}
