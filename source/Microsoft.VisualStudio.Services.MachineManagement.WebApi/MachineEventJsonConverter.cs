// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineEventJsonConverter
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  internal sealed class MachineEventJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (MachineOrchestrationEvent).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JObject jobject = JObject.Load(reader);
      MachineOrchestrationEvent target = (MachineOrchestrationEvent) null;
      JToken a;
      if (jobject.TryGetValue("Name", StringComparison.OrdinalIgnoreCase, out a) && a.Type == JTokenType.String && string.Equals((string) a, "MachineRegistered", StringComparison.Ordinal))
        target = (MachineOrchestrationEvent) new MachineCreatedEvent();
      if (target == null)
        return existingValue;
      using (JsonReader reader1 = jobject.CreateReader())
        serializer.Populate(reader1, (object) target);
      return (object) target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
