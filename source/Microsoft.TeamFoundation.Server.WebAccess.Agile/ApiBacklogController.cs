// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ApiBacklogController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.Utils.Performance;
using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [SupportedRouteArea("Api", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [OutputCache(CacheProfile = "NoCache")]
  public class ApiBacklogController : BacklogsController
  {
    private ApiBacklogControllerHelper m_apiBacklogControllerHelper;

    public ApiBacklogControllerHelper ApiBacklogControllerHelper => this.m_apiBacklogControllerHelper ?? (this.m_apiBacklogControllerHelper = new ApiBacklogControllerHelper((BacklogsController) this));

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult IterationBacklogQuery(string[] fields, Guid iterationId) => (ActionResult) this.Json((object) this.ApiBacklogControllerHelper.GetIterationBacklogQuery(fields, iterationId), JsonRequestBehavior.AllowGet);

    [HttpGet]
    [SamplePerformanceData]
    public JsonResult GetBoardModel(string hubCategoryReferenceName) => this.Json((object) this.ApiBacklogControllerHelper.GetBoardModelJsObject(hubCategoryReferenceName), JsonRequestBehavior.AllowGet);
  }
}
