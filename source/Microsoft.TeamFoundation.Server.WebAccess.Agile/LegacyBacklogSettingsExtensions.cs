// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.LegacyBacklogSettingsExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Newtonsoft.Json;
using System;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  internal static class LegacyBacklogSettingsExtensions
  {
    public static SavedNavigationSettings GetLegacySavedNavigationSettings(
      this IVssRequestContext requestContext)
    {
      WebPageDataProviderPageSource pageSource = WebPageDataProviderUtil.GetPageSource(requestContext);
      string setting = WebSettings.GetWebSettings(requestContext, pageSource.Project.Id, (WebApiTeam) null, WebSettingsScope.User).GetSetting<string>(LegacyBacklogSettingsExtensions.GetMruHubRegistryKey(pageSource.Project.Id), (string) null);
      SavedNavigationSettings navigationSettings = (SavedNavigationSettings) null;
      try
      {
        if (setting != null)
          navigationSettings = !requestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? new JavaScriptSerializer().Deserialize<SavedNavigationSettings>(setting) : JsonConvert.DeserializeObject<SavedNavigationSettings>(setting);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(59999, "Agile", "BacklogsController", ex);
      }
      return navigationSettings;
    }

    private static string GetMruHubRegistryKey(Guid projectId) => string.Format("/Backlogs/Navigation/mruHub/{0}", (object) projectId);
  }
}
