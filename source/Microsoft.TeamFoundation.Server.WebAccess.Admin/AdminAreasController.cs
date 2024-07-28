// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminAreasController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Admin", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  public class AdminAreasController : AdminBaseClassificationController
  {
    public const string FILTERNAME_AREAS_SHOW = "Areas.ShowFilter";

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(210401, 210409)]
    public ActionResult Index() => (ActionResult) this.RedirectToAction("", "AdminWork", (object) new
    {
      _a = "areas"
    });

    [HttpPost]
    [TfsTraceFilter(210411, 210419)]
    public ActionResult UpdateAreasData([ModelBinder(typeof (JsonModelBinder))] TeamFieldData saveData)
    {
      ArgumentUtility.CheckForNull<TeamFieldData>(saveData, nameof (saveData));
      this.TfsRequestContext.GetService<TeamConfigurationService>().SaveTeamFields(this.TfsRequestContext, this.Team, (ITeamFieldValue[]) saveData.TeamFieldValues, saveData.DefaultValueIndex);
      return (ActionResult) this.Json((object) new
      {
        success = true
      }, JsonRequestBehavior.AllowGet);
    }
  }
}
