// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.JsonFormatter
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public static class JsonFormatter
  {
    private static readonly Lazy<JsonSerializerSettings> s_serializerSettings = new Lazy<JsonSerializerSettings>((Func<JsonSerializerSettings>) (() => new VssJsonMediaTypeFormatter().SerializerSettings));
    private static readonly Lazy<JsonSerializerSettings> s_indentSettings = new Lazy<JsonSerializerSettings>((Func<JsonSerializerSettings>) (() =>
    {
      JsonSerializerSettings serializerSettings = new VssJsonMediaTypeFormatter().SerializerSettings;
      serializerSettings.Formatting = Formatting.Indented;
      return serializerSettings;
    }));
    private static Lazy<JsonSerializer> jsonSerializer = new Lazy<JsonSerializer>((Func<JsonSerializer>) (() => JsonSerializer.Create(JsonFormatter.s_indentSettings.Value)));

    public static byte[] Serialize(object toSerialize)
    {
      if (toSerialize == null)
        return (byte[]) null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) memoryStream))
        {
          using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) streamWriter))
            JsonFormatter.jsonSerializer.Value.Serialize((JsonWriter) jsonTextWriter, toSerialize);
        }
        return memoryStream.ToArray();
      }
    }
  }
}
