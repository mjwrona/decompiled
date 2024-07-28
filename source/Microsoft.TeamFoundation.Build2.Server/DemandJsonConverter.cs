// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DemandJsonConverter
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class DemandJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (Demand).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      Demand demand;
      if (existingValue == null && reader.TokenType == JsonToken.String && Demand.TryParse((string) reader.Value, out demand))
        existingValue = (object) demand;
      return existingValue;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      if (value == null)
        return;
      writer.WriteValue(value.ToString());
    }
  }
}
