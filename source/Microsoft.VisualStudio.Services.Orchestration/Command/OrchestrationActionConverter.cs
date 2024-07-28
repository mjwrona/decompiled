// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Command.OrchestrationActionConverter
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Orchestration.Command
{
  internal class OrchestrationActionConverter : JsonCreationConverter<OrchestratorAction>
  {
    protected override OrchestratorAction CreateObject(
      Type objectType,
      JObject jobject,
      JsonSerializer serializer)
    {
      if (!(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract jsonObjectContract))
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to resolve Object contract for type {0}", (object) objectType.FullName));
      JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("OrchestratorActionType");
      if (closestMatchProperty == null)
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to resolve OrchestratorActionType property for type {0}", (object) objectType.FullName));
      JToken jtoken;
      if (!jobject.TryGetValue(closestMatchProperty.PropertyName, out jtoken))
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to resolve OrchestratorActionType value for type {0}", (object) objectType.FullName));
      OrchestratorActionType result;
      if (jtoken.Type == JTokenType.Integer)
        result = (OrchestratorActionType) (int) jtoken;
      else if (jtoken.Type != JTokenType.String || !Enum.TryParse<OrchestratorActionType>((string) jtoken, out result))
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to parse OrchestratorActionType value of type {0} for type {1}", (object) jtoken.Type, (object) objectType.FullName));
      switch (result)
      {
        case OrchestratorActionType.ScheduleOrchestrator:
          return (OrchestratorAction) new ScheduleTaskOrchestratorAction();
        case OrchestratorActionType.CreateSubOrchestration:
          return (OrchestratorAction) new CreateSubOrchestrationAction();
        case OrchestratorActionType.CreateTimer:
          return (OrchestratorAction) new CreateTimerOrchestratorAction();
        case OrchestratorActionType.OrchestrationComplete:
          return (OrchestratorAction) new OrchestrationCompleteOrchestratorAction();
        default:
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unrecognized OrchestratorActionType '{0}'", (object) result));
      }
    }
  }
}
