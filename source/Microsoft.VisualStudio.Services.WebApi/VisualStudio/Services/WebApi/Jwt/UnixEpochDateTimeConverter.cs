// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Jwt.UnixEpochDateTimeConverter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.VisualStudio.Services.WebApi.Jwt
{
  internal class UnixEpochDateTimeConverter : DateTimeConverterBase
  {
    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return (object) (reader.Value is string ? long.Parse((string) reader.Value) : (long) reader.Value).FromUnixEpochTime();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      long unixEpochTime = ((DateTime) value).ToUnixEpochTime();
      writer.WriteValue(unixEpochTime);
    }
  }
}
