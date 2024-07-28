// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.CustomRepositoryPropertiesJsonConverter
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  internal sealed class CustomRepositoryPropertiesJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (CustomRepositoryProperties).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override bool CanWrite => false;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      if (!(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract jsonObjectContract))
        return existingValue;
      JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("propertiesType");
      if (closestMatchProperty == null)
        return existingValue;
      JObject jobject = JObject.Load(reader);
      JToken jtoken;
      if (!jobject.TryGetValue(closestMatchProperty.PropertyName, out jtoken))
        return existingValue;
      CustomRepositoryPropertiesType result;
      if (jtoken.Type == JTokenType.Integer)
        result = (CustomRepositoryPropertiesType) (int) jtoken;
      else if (jtoken.Type != JTokenType.String || !Enum.TryParse<CustomRepositoryPropertiesType>((string) jtoken, true, out result))
        return existingValue;
      object target = (object) null;
      if (result == CustomRepositoryPropertiesType.SourceDepot)
        target = (object) new SDRepositoryProperties();
      if (jobject != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, target);
      }
      return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
  }
}
