// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DemandJsonConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  public class DemandJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (Demand).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof (reader));
      Demand demand;
      if (existingValue == null && reader.TokenType == JsonToken.String && Demand.TryParse((string) reader.Value, out demand))
        existingValue = (object) demand;
      return existingValue;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      if (value == null)
        return;
      writer.WriteValue(value.ToString());
    }
  }
}
