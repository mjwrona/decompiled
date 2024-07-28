// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.ConfigurationTypeJsonConverter`1
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Microsoft.Azure.Pipelines.WebApi
{
  public abstract class ConfigurationTypeJsonConverter<T> : VssSecureJsonConverter where T : class
  {
    public override bool CanConvert(Type objectType) => typeof (T).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override bool CanWrite => false;

    protected abstract T Create(Type objectType);

    protected abstract T Create(ConfigurationType type);

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
      T target = this.Create(objectType);
      JObject jobject = JObject.Load(reader);
      if ((object) target == null)
      {
        JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("Type");
        JToken jtoken;
        if (closestMatchProperty == null || !jobject.TryGetValue(closestMatchProperty.PropertyName, StringComparison.OrdinalIgnoreCase, out jtoken))
          return existingValue;
        target = this.Create(UnknownEnum.Parse<ConfigurationType>(jtoken.ToString()));
      }
      if ((object) target != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, (object) target);
      }
      return (object) target;
    }
  }
}
