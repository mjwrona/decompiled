// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.JobRequestMessageJsonConverter
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
  internal sealed class JobRequestMessageJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (JobRequestMessage).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

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
      if (jobject.TryGetValue("MessageType", StringComparison.OrdinalIgnoreCase, out jtoken) && jtoken.Type == JTokenType.String)
      {
        switch ((string) jtoken)
        {
          case "JobRequest":
            target = (object) new AgentJobRequestMessage();
            break;
          case "ServerTaskRequest":
          case "ServerJobRequest":
            target = (object) new ServerTaskRequestMessage();
            break;
        }
      }
      if (target == null && jobject.TryGetValue("RequestId", StringComparison.OrdinalIgnoreCase, out jtoken))
        target = (object) new AgentJobRequestMessage();
      if (target == null)
        return existingValue;
      using (JsonReader reader1 = jobject.CreateReader())
        serializer.Populate(reader1, target);
      return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
  }
}
