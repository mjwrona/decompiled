// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminStartController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Admin", NavigationContextLevels.Application | NavigationContextLevels.Collection)]
  [OutputCache(CacheProfile = "NoCache")]
  public class AdminStartController : AdminAreaController
  {
    [HttpGet]
    [TfsTraceFilter(503000, 503010)]
    public ActionResult Index()
    {
      this.CheckIsHosted(true);
      return (ActionResult) this.RedirectToAction("GettingStarted");
    }

    [HttpGet]
    [TfsTraceFilter(503020, 503030)]
    public ActionResult GettingStarted(bool? createTeamProject, Guid? collectionId)
    {
      this.CheckIsHosted(true);
      return (ActionResult) this.RedirectToAction("HostedApplicationIndex", "Home", (object) new
      {
        createTeamProject = createTeamProject,
        collectionId = collectionId
      });
    }
  }
}
