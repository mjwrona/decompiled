// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.OrchestrationEnvironmentJsonConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal sealed class OrchestrationEnvironmentJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (IOrchestrationEnvironment).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      JObject jobject = JObject.Load(reader);
      IOrchestrationEnvironment target = (IOrchestrationEnvironment) null;
      OrchestrationProcessType result = OrchestrationProcessType.Container;
      JToken jtoken;
      if (jobject.TryGetValue("ProcessType", StringComparison.OrdinalIgnoreCase, out jtoken))
      {
        if (jtoken.Type == JTokenType.Integer)
          result = (OrchestrationProcessType) (int) jtoken;
        else if (jtoken.Type != JTokenType.String || !Enum.TryParse<OrchestrationProcessType>((string) jtoken, true, out result))
          return (object) null;
      }
      switch (result)
      {
        case OrchestrationProcessType.Container:
          target = (IOrchestrationEnvironment) new PlanEnvironment();
          break;
        case OrchestrationProcessType.Pipeline:
          target = (IOrchestrationEnvironment) new PipelineEnvironment();
          break;
      }
      if (target != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, (object) target);
      }
      return (object) target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
