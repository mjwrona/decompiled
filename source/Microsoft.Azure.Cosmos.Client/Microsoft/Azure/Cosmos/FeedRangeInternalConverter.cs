// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedRangeInternalConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class FeedRangeInternalConverter : JsonConverter
  {
    private const string PartitionKeyNoneValue = "None";
    private const string RangePropertyName = "Range";
    private const string PartitionKeyPropertyName = "PK";
    private const string PartitionKeyRangeIdPropertyName = "PKRangeId";
    private static readonly RangeJsonConverter rangeJsonConverter = new RangeJsonConverter();

    public override bool CanConvert(Type objectType) => objectType == typeof (FeedRangeEpk) || objectType == typeof (FeedRangePartitionKey) || objectType == typeof (FeedRangePartitionKeyRange);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      return reader.TokenType == JsonToken.StartObject ? (object) FeedRangeInternalConverter.ReadJObject(JObject.Load(reader), serializer) : throw new JsonReaderException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      writer.WriteStartObject();
      FeedRangeInternalConverter.WriteJObject(writer, value, serializer);
      writer.WriteEndObject();
    }

    public static FeedRangeInternal ReadJObject(JObject jObject, JsonSerializer serializer)
    {
      JToken jtoken1;
      if (jObject.TryGetValue("Range", out jtoken1))
      {
        try
        {
          return (FeedRangeInternal) new FeedRangeEpk((Range<string>) FeedRangeInternalConverter.rangeJsonConverter.ReadJson(jtoken1.CreateReader(), typeof (Range<string>), (object) null, serializer));
        }
        catch (JsonSerializationException ex)
        {
          throw new JsonReaderException();
        }
      }
      else
      {
        JToken jtoken2;
        if (jObject.TryGetValue("PK", out jtoken2))
        {
          string partitionKeyString = jtoken2.Value<string>();
          if ("None".Equals(partitionKeyString, StringComparison.OrdinalIgnoreCase))
            return (FeedRangeInternal) new FeedRangePartitionKey(PartitionKey.None);
          PartitionKey partitionKey;
          if (!PartitionKey.TryParseJsonString(partitionKeyString, out partitionKey))
            throw new JsonReaderException();
          return (FeedRangeInternal) new FeedRangePartitionKey(partitionKey);
        }
        JToken jtoken3;
        if (jObject.TryGetValue("PKRangeId", out jtoken3))
          return (FeedRangeInternal) new FeedRangePartitionKeyRange(jtoken3.Value<string>());
        throw new JsonReaderException();
      }
    }

    public static void WriteJObject(JsonWriter writer, object value, JsonSerializer serializer)
    {
      switch (value)
      {
        case FeedRangeEpk feedRangeEpk:
          writer.WritePropertyName("Range");
          FeedRangeInternalConverter.rangeJsonConverter.WriteJson(writer, (object) feedRangeEpk.Range, serializer);
          break;
        case FeedRangePartitionKey rangePartitionKey:
          writer.WritePropertyName("PK");
          if (rangePartitionKey.PartitionKey.IsNone)
          {
            writer.WriteValue("None");
            break;
          }
          writer.WriteValue(rangePartitionKey.PartitionKey.ToJsonString());
          break;
        case FeedRangePartitionKeyRange partitionKeyRange:
          writer.WritePropertyName("PKRangeId");
          writer.WriteValue(partitionKeyRange.PartitionKeyRangeId);
          break;
        default:
          throw new JsonSerializationException(ClientResources.FeedToken_UnrecognizedFeedToken);
      }
    }
  }
}
