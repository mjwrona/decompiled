// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.OAuth2EndpointController
// Assembly: Microsoft.TeamFoundation.ConnectedService.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEB400C-7A81-4197-B897-D0116BC50257
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.ContentSecurityPolicy;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.ConnectedService.Server
{
  [IncludeCspHeader]
  [SupportedRouteArea("", NavigationContextLevels.Collection)]
  public class OAuth2EndpointController : WebPlatformAreaController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    [HttpGet]
    public ActionResult Callback(string code, string state)
    {
      bool redirect;
      string str = this.TfsRequestContext.GetService<ConnectedServiceProviderService>().GetProvider(this.TfsRequestContext, "oauth2").CompleteCallback(this.TfsRequestContext, code, state, this.CreateHtmlHelper().GenerateNonce(), out redirect);
      return redirect ? (ActionResult) this.Redirect(str) : (ActionResult) this.Content(str);
    }

    private HtmlHelper CreateHtmlHelper()
    {
      ViewContext viewContext = new ViewContext();
      viewContext.Controller = (ControllerBase) this;
      viewContext.RequestContext = this.Request.RequestContext;
      viewContext.HttpContext = this.HttpContext;
      return new HtmlHelper(viewContext, (IViewDataContainer) new ViewPage());
    }

    public override string AreaName => "OAuth2";
  }
}
