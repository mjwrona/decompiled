// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.JsonSerializerFactory
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade
{
  public static class JsonSerializerFactory
  {
    public static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings();

    static JsonSerializerFactory()
    {
      JsonSerializerFactory.DefaultSerializerSettings.Converters.Add((JsonConverter) new IsoDateTimeConverter());
      JsonSerializerFactory.DefaultSerializerSettings.Converters.Add((JsonConverter) new StringEnumConverter());
    }

    public static JsonSerializer New() => JsonSerializer.Create(JsonSerializerFactory.DefaultSerializerSettings);
  }
}
