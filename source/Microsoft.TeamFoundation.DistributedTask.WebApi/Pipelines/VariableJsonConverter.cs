// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableJsonConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  internal class VariableJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (IVariable).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      JObject jobject = JObject.Load(reader);
      VariableType variableType = VariableType.Inline;
      JToken jtoken1;
      if (jobject.TryGetValue("type", StringComparison.OrdinalIgnoreCase, out jtoken1))
      {
        if (jtoken1.Type == JTokenType.Integer)
          variableType = (VariableType) (int) jtoken1;
        if (jtoken1.Type == JTokenType.String)
          variableType = (VariableType) Enum.Parse(typeof (VariableType), (string) jtoken1, true);
      }
      else
      {
        JToken jtoken2;
        if (jobject.TryGetValue("id", StringComparison.OrdinalIgnoreCase, out jtoken2) || jobject.TryGetValue("groupType", StringComparison.OrdinalIgnoreCase, out jtoken2) || jobject.TryGetValue("secretStore", StringComparison.OrdinalIgnoreCase, out jtoken2))
          variableType = VariableType.Group;
      }
      IVariable target = variableType != VariableType.Group ? (IVariable) new Variable() : (IVariable) new VariableGroupReference();
      using (JsonReader reader1 = jobject.CreateReader())
        serializer.Populate(reader1, (object) target);
      return (object) target;
    }
  }
}
