// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement.DocumentServiceLeaseConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
  internal sealed class DocumentServiceLeaseConverter : JsonConverter
  {
    private static readonly string VersionPropertyName = "version";

    public override bool CanConvert(Type objectType) => objectType == typeof (DocumentServiceLeaseCore) || objectType == typeof (DocumentServiceLeaseCoreEpk);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      JObject jobject = reader.TokenType == JsonToken.StartObject ? JObject.Load(reader) : throw new JsonReaderException();
      JToken jtoken;
      if (jobject.TryGetValue(DocumentServiceLeaseConverter.VersionPropertyName, out jtoken))
      {
        int num = jtoken.Value<int>();
        serializer.ContractResolver.ResolveContract(typeof (DocumentServiceLeaseCoreEpk)).Converter = (JsonConverter) null;
        if (num == 1)
          return serializer.Deserialize(jobject.CreateReader(), typeof (DocumentServiceLeaseCoreEpk));
      }
      serializer.ContractResolver.ResolveContract(typeof (DocumentServiceLeaseCore)).Converter = (JsonConverter) null;
      return serializer.Deserialize(jobject.CreateReader(), typeof (DocumentServiceLeaseCore));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (value is DocumentServiceLeaseCore serviceLeaseCore)
      {
        serializer.ContractResolver.ResolveContract(typeof (DocumentServiceLeaseCore)).Converter = (JsonConverter) null;
        serializer.Serialize(writer, (object) serviceLeaseCore, typeof (DocumentServiceLeaseCore));
      }
      if (!(value is DocumentServiceLeaseCoreEpk serviceLeaseCoreEpk))
        return;
      serializer.ContractResolver.ResolveContract(typeof (DocumentServiceLeaseCoreEpk)).Converter = (JsonConverter) null;
      serializer.Serialize(writer, (object) serviceLeaseCoreEpk, typeof (DocumentServiceLeaseCoreEpk));
    }
  }
}
