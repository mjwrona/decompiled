// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.CustomizationExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class CustomizationExtensions
  {
    public static void Area(this ControllerContext controllerContext, string area) => controllerContext.Controller.ViewData["__currentArea"] = (object) area;

    public static string Area(this HtmlHelper htmlHelper) => htmlHelper.ViewData["__currentArea"] is string str ? str : "";

    public static void NavigationLevel(
      this ControllerContext controllerContext,
      NavigationContextLevels level)
    {
      controllerContext.Controller.ViewData["__currentLevel"] = (object) level;
    }

    public static NavigationContextLevels NavigationLevel(this HtmlHelper htmlHelper)
    {
      NavigationContextLevels? nullable = htmlHelper.ViewData["__currentLevel"] as NavigationContextLevels?;
      return !nullable.HasValue ? NavigationContextLevels.Deployment : nullable.Value;
    }
  }
}
