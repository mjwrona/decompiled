// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.FullScreenHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.VisualStudio.Services.Common;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  public static class FullScreenHelper
  {
    private const string c_ViewDataOverrideKey = "__fullScreenOverride";

    public static bool IsFullScreen(TfsWebContext webContext) => GeneralHtmlExtensions.IsFullscreen(webContext.RequestContext.HttpContext);

    public static void FullScreenBody(this HtmlHelper htmlHelper, TfsWebContext webContext)
    {
      bool flag = false;
      if (!FullScreenHelper.IsFullScreen(webContext) && !(htmlHelper.ViewData.TryGetValue<bool>("__fullScreenOverride", out flag) & flag))
        return;
      GeneralHtmlExtensions.AddFullscreenClasses(webContext.RequestContext.HttpContext);
    }

    public static void OverrideFullscreenForCurrentAction(
      ViewDataDictionary viewData,
      bool isFullscreen)
    {
      viewData["__fullScreenOverride"] = (object) isFullscreen;
    }
  }
}
