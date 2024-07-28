// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.AzureTfsExtController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  [SupportedRouteArea(NavigationContextLevels.Deployment | NavigationContextLevels.Application | NavigationContextLevels.Collection)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class AzureTfsExtController : TfsController
  {
    private string s_layoutFormat = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <!-- Ibiza XHR for cross-origin -->\r\n    <!-- Allow all pages served off of this controller to interact client-side -->\r\n    <script type=\"text/javascript\">\r\n        var domainParts = document.domain.split('.');\r\n        if (domainParts.length > 2) {{\r\n            // Remove subdomain in order for our cross origin requests (to different accounts) to work. Needed since IE9 doesn't support CORS.\r\n            document.domain = domainParts.slice(domainParts.length - 2, domainParts.length).join('.');\r\n        }}\r\n    </script>\r\n    <title>{0}</title>\r\n</head>\r\n<body>\r\n<h1>{0}</h1>\r\n<div>{1}</div>\r\n</body>\r\n</html>";

    [HttpGet]
    public ActionResult Proxy()
    {
      this.Response.Headers.Remove("X-FRAME-OPTIONS");
      return (ActionResult) this.Content(string.Format(this.s_layoutFormat, (object) "XDM IFrame Extension", (object) string.Empty));
    }

    [HttpGet]
    public ActionResult Token()
    {
      this.Response.Headers.Remove("X-FRAME-OPTIONS");
      return (ActionResult) this.Content(string.Format(this.s_layoutFormat, (object) "AzureTfsExt Tokens", (object) this.CreateHtmlHelper().TfsAntiForgeryToken()));
    }

    private HtmlHelper CreateHtmlHelper()
    {
      ViewContext viewContext = new ViewContext();
      viewContext.Controller = (ControllerBase) this;
      viewContext.RequestContext = this.Request.RequestContext;
      viewContext.HttpContext = this.HttpContext;
      return new HtmlHelper(viewContext, (IViewDataContainer) new ViewPage());
    }
  }
}
