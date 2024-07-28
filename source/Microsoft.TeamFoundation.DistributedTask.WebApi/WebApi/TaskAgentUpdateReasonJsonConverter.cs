// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentUpdateReasonJsonConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal sealed class TaskAgentUpdateReasonJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (TaskAgentUpdateReason).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

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
      JObject jobject = JObject.Load(reader);
      JToken jtoken;
      TaskAgentUpdateReasonType result;
      if (jobject.TryGetValue("Code", StringComparison.OrdinalIgnoreCase, out jtoken) && jtoken.Type == JTokenType.String && Enum.TryParse<TaskAgentUpdateReasonType>((string) jtoken, out result))
      {
        switch (result)
        {
          case TaskAgentUpdateReasonType.Manual:
            target = (object) new TaskAgentManualUpdate();
            break;
          case TaskAgentUpdateReasonType.MinAgentVersionRequired:
            target = (object) new TaskAgentMinAgentVersionRequiredUpdate();
            break;
          case TaskAgentUpdateReasonType.Downgrade:
            target = (object) new TaskAgentDowngrade();
            break;
        }
      }
      if (target == null)
        return existingValue;
      using (JsonReader reader1 = jobject.CreateReader())
        serializer.Populate(reader1, target);
      return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
  }
}
