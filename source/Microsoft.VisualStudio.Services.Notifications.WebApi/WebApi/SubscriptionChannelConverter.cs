// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionChannelConverter
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  public class SubscriptionChannelConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => SubscriptionChannelMapping.SupportedChannels.Values.Contains<Type>(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      object obj = (object) null;
      if (reader.TokenType == JsonToken.StartObject)
      {
        string key = (string) null;
        JObject jobject = JObject.Load(reader);
        JToken jtoken = jobject["type"];
        if (jtoken != null)
        {
          key = jtoken.Value<string>();
          Type objectType1;
          if (!string.IsNullOrEmpty(key) && SubscriptionChannelMapping.SupportedChannels.TryGetValue(key, out objectType1))
            obj = jobject.ToObject(objectType1);
        }
        if (obj == null)
          obj = (object) new UnsupportedSubscriptionChannel(!string.IsNullOrEmpty(key) ? key : "Invalid");
      }
      return obj;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      JToken.FromObject(value, serializer).WriteTo(writer);
    }
  }
}
