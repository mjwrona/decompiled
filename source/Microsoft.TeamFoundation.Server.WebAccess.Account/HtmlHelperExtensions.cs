// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.HtmlHelperExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public static class HtmlHelperExtensions
  {
    public static MvcHtmlString IdentityProviderOptions(
      this HtmlHelper htmlHelper,
      IdentityProviderModel model)
    {
      return htmlHelper.JsonIsland((object) new
      {
        providerOptions = model.ToJson()
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString CreateDefaultProjectOptions(
      this HtmlHelper htmlHelper,
      CreateProjectViewModel model)
    {
      return htmlHelper.JsonIsland((object) new
      {
        Scenario = model.Scenario
      }, (object) new
      {
        @class = "options",
        id = "create-default-project-page-data"
      });
    }
  }
}
