// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.LeftMenu
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3690C2EA-1623-4663-B65B-BB4B63BFE368
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.dll

using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.TfsCommon
{
  public class LeftMenu : MenuBar
  {
    public LeftMenu(HtmlHelper htmlHelper)
    {
      this.EnhancementCssClass = (string) null;
      this.CssClass<LeftMenu>("bowtie-menus l1-project-selector l1-menubar icon-only l1-main-menu");
      MenuItem menuItem = this.AddMenuItem().CssClass<MenuItem>("l1-main-menu-icon").Icon("bowtie-icon bowtie-brand-vsts menu-icon").ItemId("teams").AriaLabel(TfsCommonResources.GoToHomePage);
      WebContext webContext = htmlHelper.ViewContext.WebContext();
      menuItem.Href = webContext.Url.Action("index", "home", (object) new
      {
        routeArea = "",
        serviceHost = webContext.TfsRequestContext.ServiceHost,
        project = string.Empty,
        team = string.Empty
      });
      menuItem.ClickOpensSubMenu = new bool?(false);
      menuItem.IdIsAction = new bool?();
    }
  }
}
