// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.EventsConfigJsonConverter
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
  internal sealed class EventsConfigJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (EventsConfig).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JObject jobject = JObject.Load(reader);
      EventsConfig target = (EventsConfig) null;
      JToken jtoken;
      if (jobject.TryGetValue("JobAssigned", StringComparison.OrdinalIgnoreCase, out jtoken) || jobject.TryGetValue("JobStarted", StringComparison.OrdinalIgnoreCase, out jtoken) || jobject.TryGetValue("JobCompleted", StringComparison.OrdinalIgnoreCase, out jtoken))
        target = (EventsConfig) new JobEventsConfig();
      else if (jobject.TryGetValue("TaskAssigned", StringComparison.OrdinalIgnoreCase, out jtoken) || jobject.TryGetValue("TaskStarted", StringComparison.OrdinalIgnoreCase, out jtoken) || jobject.TryGetValue("TaskCompleted", StringComparison.OrdinalIgnoreCase, out jtoken))
        target = (EventsConfig) new TaskEventsConfig();
      if (target == null)
        return existingValue;
      using (JsonReader reader1 = jobject.CreateReader())
        serializer.Populate(reader1, (object) target);
      return (object) target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
