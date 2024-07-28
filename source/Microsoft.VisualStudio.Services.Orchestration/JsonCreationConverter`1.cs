// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.JsonCreationConverter`1
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  internal abstract class JsonCreationConverter<T> : VssSecureJsonConverter where T : class
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (T).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      JObject jobject = JObject.Load(reader);
      T target = this.CreateObject(objectType, jobject, serializer);
      serializer.Populate(jobject.CreateReader(), (object) target);
      return (object) target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

    protected abstract T CreateObject(Type objectType, JObject jobject, JsonSerializer serializer);
  }
}
