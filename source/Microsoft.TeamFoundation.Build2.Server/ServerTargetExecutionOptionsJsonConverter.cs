// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ServerTargetExecutionOptionsJsonConverter
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class ServerTargetExecutionOptionsJsonConverter : 
    TypePropertyJsonConverter<ServerTargetExecutionOptions>
  {
    protected override ServerTargetExecutionOptions GetInstance(Type objectType)
    {
      if (objectType == typeof (ServerTargetExecutionOptions))
        return new ServerTargetExecutionOptions();
      return objectType == typeof (VariableMultipliersServerExecutionOptions) ? (ServerTargetExecutionOptions) new VariableMultipliersServerExecutionOptions() : base.GetInstance(objectType);
    }

    protected override ServerTargetExecutionOptions GetInstance(int targetType, JObject value)
    {
      if (targetType == 0)
        return new ServerTargetExecutionOptions();
      return targetType == 1 ? (ServerTargetExecutionOptions) new VariableMultipliersServerExecutionOptions() : (ServerTargetExecutionOptions) null;
    }

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
      ServerTargetExecutionOptions target = this.GetInstance(objectType);
      if (target == null)
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
        target = this.GetInstance(type, jobject);
      }
      if (!(target is VariableMultipliersServerExecutionOptions) && target.Type == 0)
        target = !jobject.TryGetValue("maxConcurrency", StringComparison.OrdinalIgnoreCase, out JToken _) ? this.GetInstance(0, jobject) : this.GetInstance(1, jobject);
      if (jobject != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, (object) target);
      }
      return (object) target;
    }
  }
}
