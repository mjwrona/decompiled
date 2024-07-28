// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.LongToStringConverter
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.AzureAd.Icm.Types
{
  public class LongToStringConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (long).Equals(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return (object) JToken.ReadFrom(reader).Value<long>();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => serializer.Serialize(writer, (object) value.ToString());
  }
}
