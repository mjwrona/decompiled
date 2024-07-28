// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunJobInputConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal sealed class RunJobInputConverter : VssSecureJsonConverter
  {
    public override bool CanRead => true;

    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (RunJobInput).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      RunJobInput target = new RunJobInput();
      serializer.Populate(reader, (object) target);
      return (object) target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
