// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.DemandJsonConverter
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  internal sealed class DemandJsonConverter : VssSecureJsonConverter
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
