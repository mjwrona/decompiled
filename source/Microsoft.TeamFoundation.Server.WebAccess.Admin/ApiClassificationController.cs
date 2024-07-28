// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ApiClassificationController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Api", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiClassificationController : AdminAreaController
  {
    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.All)]
    public ActionResult GetIterations() => (ActionResult) this.Json(BoardsAdminDataSource.GetIterations(this.TfsRequestContext, this.TfsWebContext.ProjectContext.Name), JsonRequestBehavior.AllowGet);

    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.All)]
    public ActionResult HasPermissions(
      Guid nodeId,
      bool genericReadAccess = false,
      bool genericWriteAccess = false,
      bool createChildrenAccess = false,
      bool deleteAccess = false)
    {
      return (ActionResult) this.Json((object) BoardsAdminDataSource.HasPermissions(this.TfsRequestContext, nodeId, genericReadAccess, genericWriteAccess, createChildrenAccess, deleteAccess), JsonRequestBehavior.AllowGet);
    }
  }
}
