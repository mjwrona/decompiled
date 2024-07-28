// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.OrderByQueryResult
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class OrderByQueryResult
  {
    public OrderByQueryResult(string rid, QueryItem[] orderByItems, object payload)
    {
      if (string.IsNullOrEmpty(rid))
        throw new ArgumentNullException("rid can not be null or empty.");
      if (orderByItems == null)
        throw new ArgumentNullException("orderByItems can not be null.");
      if (orderByItems.Length == 0)
        throw new ArgumentException("orderByItems can not be empty.");
      this.Rid = rid;
      this.OrderByItems = orderByItems;
      this.Payload = payload;
    }

    [JsonProperty("_rid")]
    public string Rid { get; }

    [JsonProperty("orderByItems")]
    public QueryItem[] OrderByItems { get; }

    [JsonProperty("payload")]
    [JsonConverter(typeof (OrderByQueryResult.PayloadConverter))]
    public object Payload { get; }

    private sealed class PayloadConverter : JsonConverter
    {
      public override bool CanConvert(Type objectType) => (object) objectType == (object) typeof (object);

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        JToken jObject = JToken.Load(reader);
        return jObject.Type == JTokenType.Object || jObject.Type == JTokenType.Array ? (object) new QueryResult((JContainer) jObject, (string) null, serializer) : (object) jObject;
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => serializer.Serialize(writer, value);
    }
  }
}
