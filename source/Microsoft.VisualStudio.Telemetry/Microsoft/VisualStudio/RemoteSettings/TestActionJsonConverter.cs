// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.TestActionJsonConverter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class TestActionJsonConverter : JsonConverter
  {
    public override bool CanConvert(System.Type objectType) => throw new NotImplementedException("Should only be used on property");

    public override object ReadJson(
      JsonReader reader,
      System.Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return reader.TokenType == JsonToken.String ? (object) (reader.Value as string) : (object) JObject.Load(reader).ToString();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException("Cannot use this class to write");
  }
}
