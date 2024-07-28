// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.DataProviderContextExtensions
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  public static class DataProviderContextExtensions
  {
    public static T GetPropertiesAs<T>(this DataProviderContext context)
    {
      string str = JsonConvert.SerializeObject((object) context.Properties);
      return !string.IsNullOrEmpty(str) ? JsonConvert.DeserializeObject<T>(str) : default (T);
    }
  }
}
