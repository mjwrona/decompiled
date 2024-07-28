// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Routing.TfsRouteHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Routing
{
  public static class TfsRouteHelpers
  {
    public static string GetControllerActionRouteName(TfsWebContext tfsWebContext, string area = "") => TfsRouteHelpers.GetControllerActionRouteName(tfsWebContext.NavigationContext.TopMostLevel, area);

    public static string GetControllerActionRouteName(
      NavigationContextLevels navigationLevel,
      string area = "",
      bool withParameters = false)
    {
      string str;
      switch (navigationLevel)
      {
        case NavigationContextLevels.Project:
          str = "ProjectControllerAction";
          break;
        case NavigationContextLevels.Team:
          str = "TeamControllerAction";
          break;
        default:
          str = "ServiceHostControllerAction";
          break;
      }
      string controllerActionRouteName = str;
      if (!string.IsNullOrEmpty(area))
        controllerActionRouteName = area + controllerActionRouteName;
      if (withParameters)
        controllerActionRouteName += "WithParameters";
      return controllerActionRouteName;
    }

    public static string GetLevelRouteName(TfsWebContext tfsWebContext, string area = "") => TfsRouteHelpers.GetLevelRouteName(tfsWebContext.NavigationContext.TopMostLevel, area);

    public static string GetLevelRouteName(MRUNavigationContextEntry mruEntry, string area = "")
    {
      string levelRouteName = !string.IsNullOrEmpty(mruEntry.Team) ? "Team" : (!string.IsNullOrEmpty(mruEntry.Project) ? "Project" : "ServiceHost");
      if (!string.IsNullOrEmpty(area))
        levelRouteName = area + levelRouteName;
      return levelRouteName;
    }

    public static string GetLevelRouteName(NavigationContextLevels navigationLevel, string area = "")
    {
      string str;
      switch (navigationLevel)
      {
        case NavigationContextLevels.Project:
          str = "Project";
          break;
        case NavigationContextLevels.Team:
          str = "Team";
          break;
        default:
          str = "ServiceHost";
          break;
      }
      string levelRouteName = str;
      if (!string.IsNullOrEmpty(area))
        levelRouteName = area + levelRouteName;
      return levelRouteName;
    }
  }
}
