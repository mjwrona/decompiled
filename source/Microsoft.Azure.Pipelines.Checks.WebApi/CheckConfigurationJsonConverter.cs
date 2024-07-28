// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.CheckConfigurationJsonConverter
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CheckConfigurationJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (CheckConfiguration).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

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
      JObject jobject = JObject.Load(reader);
      JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("Type");
      JToken jtoken;
      if (closestMatchProperty == null || !jobject.TryGetValue(closestMatchProperty.PropertyName, StringComparison.OrdinalIgnoreCase, out jtoken))
        return existingValue;
      CheckType type = jtoken.ToObject<CheckType>();
      CheckConfiguration target = (CheckConfiguration) null;
      if (type != null)
        target = type.CreateCheckConfigurationObject();
      if (target != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, (object) target);
      }
      return (object) target;
    }
  }
}
