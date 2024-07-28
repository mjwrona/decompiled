// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Welcome.WelcomeExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Welcome, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B024A61-082C-4505-8523-CF030F6A8A5A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Welcome.dll

using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Welcome
{
  public static class WelcomeExtensions
  {
    public static MvcHtmlString WelcomeViewOptions(
      this HtmlHelper htmlHelper,
      WelcomeViewModel model)
    {
      return htmlHelper.RestApiJsonIsland((object) model, (object) new
      {
        @class = "options"
      });
    }
  }
}
