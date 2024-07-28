// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.StepConverter
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
  internal sealed class StepConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (Step).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      JObject jobject = JObject.Load(reader);
      JToken jtoken;
      if (!jobject.TryGetValue("Type", StringComparison.OrdinalIgnoreCase, out jtoken))
      {
        Step target = !jobject.TryGetValue("Parameters", StringComparison.OrdinalIgnoreCase, out JToken _) ? (Step) new TaskStep() : (Step) new TaskTemplateStep();
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, (object) target);
        return (object) target;
      }
      StepType result;
      if (jtoken.Type == JTokenType.Integer)
        result = (StepType) (int) jtoken;
      else if (jtoken.Type != JTokenType.String || !Enum.TryParse<StepType>((string) jtoken, true, out result))
        return (object) null;
      Step target1 = (Step) null;
      switch (result)
      {
        case StepType.Task:
          target1 = (Step) new TaskStep();
          break;
        case StepType.TaskTemplate:
          target1 = (Step) new TaskTemplateStep();
          break;
        case StepType.Group:
          target1 = (Step) new GroupStep();
          break;
      }
      using (JsonReader reader2 = jobject.CreateReader())
        serializer.Populate(reader2, (object) target1);
      return (object) target1;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
