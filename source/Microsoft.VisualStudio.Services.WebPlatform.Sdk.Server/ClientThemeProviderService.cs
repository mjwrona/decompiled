// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ClientThemeProviderService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ClientThemeProviderService : IClientThemeProviderService, IVssFrameworkService
  {
    private const string c_sharedDataKey = "_themes";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddTheme(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      string themeId)
    {
      WebSdkMetadataDictionary<string, ClientTheme> sharedDataThemes = this.GetSharedDataThemes(requestContext, sharedData);
      if (sharedDataThemes.ContainsKey(themeId))
        return;
      ClientTheme theme = requestContext.GetService<IThemingService>().GetTheme(requestContext, themeId);
      if (theme == null)
        return;
      sharedDataThemes[themeId] = theme;
    }

    public void AddTheme(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      ClientTheme theme)
    {
      this.GetSharedDataThemes(requestContext, sharedData)[theme.Id] = theme;
    }

    private WebSdkMetadataDictionary<string, ClientTheme> GetSharedDataThemes(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData)
    {
      object obj;
      WebSdkMetadataDictionary<string, ClientTheme> sharedDataThemes;
      if (sharedData.TryGetValue("_themes", out obj) && obj is WebSdkMetadataDictionary<string, ClientTheme>)
      {
        sharedDataThemes = obj as WebSdkMetadataDictionary<string, ClientTheme>;
      }
      else
      {
        sharedDataThemes = new WebSdkMetadataDictionary<string, ClientTheme>();
        sharedData.Add("_themes", (object) sharedDataThemes);
      }
      return sharedDataThemes;
    }
  }
}
