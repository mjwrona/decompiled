// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PhaseTargetJsonConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  internal sealed class PhaseTargetJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (PhaseTarget).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      PhaseTargetType? nullable = new PhaseTargetType?();
      JObject jobject = JObject.Load(reader);
      JToken jtoken;
      if (!jobject.TryGetValue("Type", StringComparison.OrdinalIgnoreCase, out jtoken))
        return existingValue;
      if (jtoken.Type == JTokenType.Integer)
      {
        nullable = new PhaseTargetType?((PhaseTargetType) (int) jtoken);
      }
      else
      {
        PhaseTargetType result;
        if (jtoken.Type == JTokenType.String && Enum.TryParse<PhaseTargetType>((string) jtoken, true, out result))
          nullable = new PhaseTargetType?(result);
      }
      if (!nullable.HasValue)
        return existingValue;
      object target = (object) null;
      if (nullable.HasValue)
      {
        switch (nullable.GetValueOrDefault())
        {
          case PhaseTargetType.Queue:
            target = (object) new AgentQueueTarget();
            break;
          case PhaseTargetType.Server:
            target = (object) new ServerTarget();
            break;
          case PhaseTargetType.DeploymentGroup:
            target = (object) new DeploymentGroupTarget();
            break;
          case PhaseTargetType.Pool:
            target = (object) new AgentPoolTarget();
            break;
        }
      }
      if (jobject != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, target);
      }
      return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
