// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TimeSpanJsonConverter
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using Newtonsoft.Json;
using System;
using System.Xml;

namespace Microsoft.AzureAd.Icm.Types
{
  public class TimeSpanJsonConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      string str = XmlConvert.ToString((TimeSpan) value);
      serializer.Serialize(writer, (object) str);
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return reader.TokenType == JsonToken.Null ? (object) null : (object) XmlConvert.ToTimeSpan(serializer.Deserialize<string>(reader));
    }

    public override bool CanConvert(Type objectType) => objectType == typeof (TimeSpan) || objectType == typeof (TimeSpan?);
  }
}
