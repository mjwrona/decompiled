// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TimeZoneInfoConverter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TimeZoneInfoConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (TimeZoneInfo).IsAssignableFrom(objectType);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      writer.WriteValue(((TimeZoneInfo) value).Id);
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return reader.Value == null ? (object) null : (object) TimeZoneInfo.FindSystemTimeZoneById((string) reader.Value);
    }
  }
}
