// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicenseJsonConverter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal sealed class LicenseJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (License).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.String)
        return (object) License.Parse(reader.Value.ToString(), true);
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      throw new JsonSerializationException();
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
        License license = (License) value;
        writer.WriteValue(license.ToString());
      }
    }
  }
}
