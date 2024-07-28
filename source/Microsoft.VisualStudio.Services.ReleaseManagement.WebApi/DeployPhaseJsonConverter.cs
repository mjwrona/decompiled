// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseJsonConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  public class DeployPhaseJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (DeployPhase).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override bool CanRead => true;

    public override bool CanWrite => false;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      if (!(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract jsonObjectContract))
        return existingValue;
      JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("PhaseType");
      if (closestMatchProperty == null)
        return existingValue;
      JObject jobject = JObject.Load(reader);
      JToken jtoken;
      if (!jobject.TryGetValue(closestMatchProperty.PropertyName, StringComparison.OrdinalIgnoreCase, out jtoken))
        return existingValue;
      DeployPhaseTypes result;
      if (jtoken.Type == JTokenType.Integer)
        result = (DeployPhaseTypes) (int) jtoken;
      else if (jtoken.Type != JTokenType.String || !Enum.TryParse<DeployPhaseTypes>((string) jtoken, true, out result))
        return existingValue;
      object target = (object) null;
      switch (result)
      {
        case DeployPhaseTypes.AgentBasedDeployment:
          target = (object) new AgentBasedDeployPhase();
          break;
        case DeployPhaseTypes.RunOnServer:
          target = (object) new RunOnServerDeployPhase();
          break;
        case DeployPhaseTypes.MachineGroupBasedDeployment:
          target = (object) new MachineGroupBasedDeployPhase();
          break;
        case DeployPhaseTypes.DeploymentGates:
          target = (object) new GatesDeployPhase();
          break;
      }
      if (target != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, target);
      }
      return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
