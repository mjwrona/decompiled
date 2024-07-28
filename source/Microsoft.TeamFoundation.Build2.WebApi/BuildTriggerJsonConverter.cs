// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildTriggerJsonConverter
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
  internal sealed class BuildTriggerJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (BuildTrigger).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override bool CanRead => true;

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
      JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("TriggerType");
      if (closestMatchProperty == null)
        return existingValue;
      JObject jobject = JObject.Load(reader);
      JToken jtoken;
      if (!jobject.TryGetValue(closestMatchProperty.PropertyName, StringComparison.OrdinalIgnoreCase, out jtoken))
        return existingValue;
      DefinitionTriggerType result;
      if (jtoken.Type == JTokenType.Integer)
        result = (DefinitionTriggerType) (int) jtoken;
      else if (jtoken.Type != JTokenType.String || !Enum.TryParse<DefinitionTriggerType>((string) jtoken, true, out result))
        return existingValue;
      object target = (object) null;
      switch (result)
      {
        case DefinitionTriggerType.ContinuousIntegration:
          target = (object) new ContinuousIntegrationTrigger();
          break;
        case DefinitionTriggerType.Schedule:
          target = (object) new ScheduleTrigger();
          break;
        case DefinitionTriggerType.GatedCheckIn:
          target = (object) new GatedCheckInTrigger();
          break;
        case DefinitionTriggerType.PullRequest:
          target = (object) new PullRequestTrigger();
          break;
        case DefinitionTriggerType.BuildCompletion:
          target = (object) new BuildCompletionTrigger();
          break;
      }
      if (jobject != null && target != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, target);
      }
      return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
