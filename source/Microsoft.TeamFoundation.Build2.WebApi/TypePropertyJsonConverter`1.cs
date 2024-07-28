// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.TypePropertyJsonConverter`1
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  internal abstract class TypePropertyJsonConverter<TInstance> : VssSecureJsonConverter where TInstance : class
  {
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
      JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("Type");
      if (closestMatchProperty == null)
        return existingValue;
      JObject jobject = JObject.Load(reader);
      TInstance instance = this.GetInstance(objectType);
      if ((object) instance == null)
      {
        JToken jtoken;
        int type;
        if (!jobject.TryGetValue(closestMatchProperty.PropertyName, StringComparison.OrdinalIgnoreCase, out jtoken))
        {
          if (!this.TryInferType(jobject, out type))
            return existingValue;
        }
        else
        {
          if (jtoken.Type != JTokenType.Integer)
            return existingValue;
          type = (int) jtoken;
        }
        instance = this.GetInstance(type);
      }
      if (jobject != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, (object) instance);
      }
      return (object) instance;
    }

    protected abstract TInstance GetInstance(int targetType);

    protected virtual TInstance GetInstance(Type objectType) => default (TInstance);

    protected virtual bool TryInferType(JObject value, out int type)
    {
      type = 0;
      return false;
    }

    public override bool CanConvert(Type objectType) => typeof (TInstance).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
  }
}
