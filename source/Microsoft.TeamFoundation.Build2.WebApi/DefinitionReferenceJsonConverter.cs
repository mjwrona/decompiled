// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.DefinitionReferenceJsonConverter
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.TeamFoundation.Build.WebApi.Internals;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  internal sealed class DefinitionReferenceJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (DefinitionReference).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override bool CanWrite => false;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      object target = (object) null;
      if (objectType == typeof (BuildDefinition))
        target = (object) new BuildDefinition();
      else if (objectType == typeof (BuildDefinition3_2))
        target = (object) new BuildDefinition3_2();
      else if (objectType == typeof (XamlBuildDefinition))
        target = (object) new XamlBuildDefinition();
      else if (objectType == typeof (BuildDefinitionReference))
        target = (object) new BuildDefinitionReference();
      JObject jobject = JObject.Load(reader);
      if (target == null)
      {
        if (!(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract jsonObjectContract))
          return existingValue;
        JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("Type");
        if (closestMatchProperty == null)
          return existingValue;
        DefinitionType result = DefinitionType.Build;
        JToken jtoken;
        if (jobject.TryGetValue(closestMatchProperty.PropertyName, out jtoken))
        {
          if (jtoken.Type == JTokenType.Integer)
            result = (DefinitionType) (int) jtoken;
          else if (jtoken.Type != JTokenType.String || !Enum.TryParse<DefinitionType>((string) jtoken, true, out result))
            result = DefinitionType.Build;
        }
        if (result != DefinitionType.Xaml)
        {
          if (result == DefinitionType.Build)
            ;
          target = (object) new BuildDefinition();
        }
        else
          target = (object) new XamlBuildDefinition();
      }
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
