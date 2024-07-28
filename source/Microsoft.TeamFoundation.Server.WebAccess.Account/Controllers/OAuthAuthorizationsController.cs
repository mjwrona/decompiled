// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.Controllers.OAuthAuthorizationsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.IO;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account.Controllers
{
  [SupportedRouteArea(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class OAuthAuthorizationsController : AccountAreaController
  {
    private const string DisplayDateFormat = "M/d/yyyy";

    [HttpGet]
    [TfsTraceFilter(505101, 505110)]
    public ActionResult Index()
    {
      this.ConfigureLeftHubSplitter(AccountServerResources.SecurityDetailsNavigationSplitter, toggleButtonCollapsedTooltip: AccountServerResources.SecurityDetailsNavigationSplitterCollapsed, toggleButtonExpandedTooltip: AccountServerResources.SecurityDetailsNavigationSplitterExpanded);
      return (ActionResult) this.View();
    }

    [HttpGet]
    public ActionResult GetApplicationImage(int id)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      Stream fileStream = vssRequestContext.GetService<ITeamFoundationFileService>().RetrieveFile(vssRequestContext, (long) id, out CompressionType _);
      return fileStream == null ? (ActionResult) new HttpStatusCodeResult(404) : (ActionResult) new FileStreamResult(fileStream, "image/jpeg");
    }

    [HttpGet]
    [TfsTraceFilter(505111, 505120)]
    public ActionResult List() => (ActionResult) this.Json((object) new System.Collections.Generic.List<OAuthAuthorizationsModel>(), JsonRequestBehavior.AllowGet);

    [HttpDelete]
    [TfsBypassAntiForgeryValidation]
    [TfsTraceFilter(505121, 505130)]
    public ActionResult Revoke(string clientId) => (ActionResult) new HttpStatusCodeResult(200);

    [HttpDelete]
    [TfsBypassAntiForgeryValidation]
    [TfsTraceFilter(505071, 505080)]
    public ActionResult RevokeAll() => (ActionResult) new HttpStatusCodeResult(200);
  }
}
