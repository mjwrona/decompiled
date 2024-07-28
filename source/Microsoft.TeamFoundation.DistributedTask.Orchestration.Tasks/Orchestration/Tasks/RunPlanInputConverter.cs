// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunPlanInputConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal sealed class RunPlanInputConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (RunPlanInput).IsAssignableFrom(objectType);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

    public override object ReadJson(
      JsonReader reader,
      Type type,
      object value,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      RunPlanInput target = new RunPlanInput();
      JObject jobject = JObject.Load(reader);
      using (JsonReader reader1 = jobject.CreateReader())
        serializer.Populate(reader1, (object) target);
      JToken jtoken;
      if (jobject.TryGetValue("ContinueOnError", StringComparison.OrdinalIgnoreCase, out jtoken) && jtoken.Type == JTokenType.Boolean)
        target.Implementation.ContinueOnError = (bool) jtoken;
      return (object) target;
    }

    public override bool CanRead => true;

    public override bool CanWrite => false;
  }
}
