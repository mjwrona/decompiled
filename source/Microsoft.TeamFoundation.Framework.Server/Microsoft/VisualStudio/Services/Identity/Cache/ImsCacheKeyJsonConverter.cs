// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheKeyJsonConverter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal sealed class ImsCacheKeyJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (ImsCacheKey).IsAssignableFrom(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      string input = reader.TokenType == JsonToken.String ? reader.Value.ToString() : throw new JsonSerializationException();
      ImsCacheIdKey result1;
      if (ImsCacheIdKey.TryParse(input, out result1))
        return (object) result1;
      ImsCacheDescriptorKey result2;
      if (ImsCacheDescriptorKey.TryParse(input, out result2))
        return (object) result2;
      ImsCacheScopedIdKey result3;
      if (ImsCacheScopedIdKey.TryParse(input, out result3))
        return (object) result3;
      ImsCacheScopedNameKey result4;
      if (ImsCacheScopedNameKey.TryParse(input, out result4))
        return (object) result4;
      ImsCacheStringKey result5;
      if (ImsCacheStringKey.TryParse(input, out result5))
        return (object) result5;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        ImsCacheKey imsCacheKey = (ImsCacheKey) value;
        writer.WriteValue(imsCacheKey.Serialize());
      }
    }
  }
}
