// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedRangeCompositeContinuationConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class FeedRangeCompositeContinuationConverter : JsonConverter
  {
    private const string VersionPropertyName = "V";
    private const string RidPropertyName = "Rid";
    private const string ContinuationPropertyName = "Continuation";

    public override bool CanConvert(Type objectType) => objectType == typeof (FeedRangeCompositeContinuation);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      JObject jObject = reader.TokenType == JsonToken.StartObject ? JObject.Load(reader) : throw new JsonReaderException();
      JToken jtoken1;
      if (!jObject.TryGetValue("Continuation", out jtoken1))
        throw new JsonReaderException();
      string containerRid = (string) null;
      JToken jtoken2;
      if (jObject.TryGetValue("Rid", out jtoken2))
        containerRid = jtoken2.Value<string>();
      List<CompositeContinuationToken> deserializedTokens = serializer.Deserialize<List<CompositeContinuationToken>>(jtoken1.CreateReader());
      FeedRangeInternal feedRange = FeedRangeInternalConverter.ReadJObject(jObject, serializer);
      return (object) new FeedRangeCompositeContinuation(containerRid, feedRange, (IReadOnlyList<CompositeContinuationToken>) deserializedTokens);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (!(value is FeedRangeCompositeContinuation compositeContinuation))
        throw new JsonSerializationException(ClientResources.FeedToken_UnrecognizedFeedToken);
      writer.WriteStartObject();
      writer.WritePropertyName("V");
      writer.WriteValue((object) FeedRangeContinuationVersion.V1);
      writer.WritePropertyName("Rid");
      writer.WriteValue(compositeContinuation.ContainerRid);
      writer.WritePropertyName("Continuation");
      serializer.Serialize(writer, (object) compositeContinuation.CompositeContinuationTokens.ToArray());
      FeedRangeInternalConverter.WriteJObject(writer, (object) compositeContinuation.FeedRange, serializer);
      writer.WriteEndObject();
    }
  }
}
