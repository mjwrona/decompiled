// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.CodeSenseSerializationSettings
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public static class CodeSenseSerializationSettings
  {
    private static readonly ThreadLocal<JsonSerializerSettings> JsonSerializerSettingsInstance = new ThreadLocal<JsonSerializerSettings>((Func<JsonSerializerSettings>) (() => new JsonSerializerSettings()
    {
      ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
      ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
    }));

    public static JsonSerializerSettings JsonSerializerSettings => CodeSenseSerializationSettings.JsonSerializerSettingsInstance.Value;
  }
}
