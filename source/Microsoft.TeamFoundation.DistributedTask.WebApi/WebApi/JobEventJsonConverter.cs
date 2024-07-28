// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.JobEventJsonConverter
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
  internal sealed class JobEventJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (JobEvent).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JObject jobject = JObject.Load(reader);
      JobEvent target = (JobEvent) null;
      JToken jtoken;
      if (jobject.TryGetValue("Name", StringComparison.OrdinalIgnoreCase, out jtoken) && jtoken.Type == JTokenType.String)
      {
        string a = (string) jtoken;
        if (string.Equals(a, "JobAssigned", StringComparison.Ordinal))
          target = (JobEvent) new JobAssignedEvent();
        else if (string.Equals(a, "JobCanceled", StringComparison.Ordinal))
          target = (JobEvent) new JobCanceledEvent();
        else if (string.Equals(a, "JobCompleted", StringComparison.Ordinal))
          target = (JobEvent) new JobCompletedEvent();
        else if (string.Equals(a, "JobStarted", StringComparison.Ordinal))
          target = (JobEvent) new JobStartedEvent();
        else if (string.Equals(a, "JobMetadataUpdate", StringComparison.Ordinal))
          target = (JobEvent) new JobMetadataEvent();
        else if (string.Equals(a, "TaskAssigned", StringComparison.Ordinal))
          target = (JobEvent) new TaskAssignedEvent();
        else if (string.Equals(a, "TaskStarted", StringComparison.Ordinal))
          target = (JobEvent) new TaskStartedEvent();
        else if (string.Equals(a, "TaskCompleted", StringComparison.Ordinal))
          target = (JobEvent) new TaskCompletedEvent();
      }
      if (target == null)
      {
        if (jobject.TryGetValue("Request", StringComparison.OrdinalIgnoreCase, out jtoken))
          target = (JobEvent) new JobAssignedEvent();
        else if (jobject.TryGetValue("Result", StringComparison.OrdinalIgnoreCase, out jtoken))
          target = (JobEvent) new JobCompletedEvent();
      }
      if (target == null)
        return existingValue;
      using (JsonReader reader1 = jobject.CreateReader())
        serializer.Populate(reader1, (object) target);
      return (object) target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
