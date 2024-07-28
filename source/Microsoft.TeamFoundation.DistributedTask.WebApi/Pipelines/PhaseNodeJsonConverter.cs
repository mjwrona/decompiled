// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PhaseNodeJsonConverter
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
  internal sealed class PhaseNodeJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (PhaseNode).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      PhaseType? nullable = new PhaseType?();
      JObject jobject = JObject.Load(reader);
      JToken jtoken;
      if (!jobject.TryGetValue("Type", StringComparison.OrdinalIgnoreCase, out jtoken))
        nullable = new PhaseType?(PhaseType.Phase);
      else if (jtoken.Type == JTokenType.Integer)
      {
        nullable = new PhaseType?((PhaseType) (int) jtoken);
      }
      else
      {
        PhaseType result;
        if (jtoken.Type == JTokenType.String && Enum.TryParse<PhaseType>((string) jtoken, true, out result))
          nullable = new PhaseType?(result);
      }
      if (!nullable.HasValue)
        return existingValue;
      object target = (object) null;
      if (nullable.HasValue)
      {
        switch (nullable.GetValueOrDefault())
        {
          case PhaseType.Phase:
            target = (object) new Phase();
            break;
          case PhaseType.Provider:
            target = (object) new ProviderPhase();
            break;
        }
      }
      using (JsonReader reader1 = jobject.CreateReader())
        serializer.Populate(reader1, target);
      return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
