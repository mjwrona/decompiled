// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy.LegacyJsonConverter`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy
{
  internal class LegacyJsonConverter<T> : VssSecureJsonConverter where T : new()
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => objectType.Equals(typeof (T));

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      T target = new T();
      serializer.Populate(reader, (object) target);
      return (object) target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
  }
}
