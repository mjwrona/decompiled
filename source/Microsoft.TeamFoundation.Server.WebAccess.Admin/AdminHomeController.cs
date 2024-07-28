// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminHomeController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Admin", NavigationContextLevels.ApplicationAll)]
  [OutputCache(CacheProfile = "NoCache")]
  [DemandFeature("65AC9DB3-BB0A-42fe-B584-A690FB0D817B", true)]
  public class AdminHomeController : AdminAreaController
  {
    public AdminHomeController() => this.m_executeContributedRequestHandlers = true;

    [TfsTraceFilter(502000, 502010)]
    [HttpGet]
    public ActionResult Index(bool launchNewTeamDialog = false)
    {
      if (this.TfsWebContext.FeatureContext.AreStandardFeaturesAvailable)
      {
        switch (this.NavigationContext.TopMostLevel)
        {
          case NavigationContextLevels.Application:
            return (ActionResult) this.View("ControlPanel", (object) new BrowseControlModel()
            {
              ShowTasks = true,
              UseHashNavigation = true,
              Collections = this.TfsWebContext.GetAllCollections(),
              ShowStoppedCollections = true
            });
          case NavigationContextLevels.Collection:
            this.CheckCollectionReadPermission();
            return (ActionResult) this.View("CollectionOverview", (object) new CollectionViewModel(this.TfsWebContext)
            {
              CanCreateProjects = this.CanCreateProjects
            });
          default:
            return (ActionResult) new EmptyResult();
        }
      }
      else
      {
        this.CheckManageLicensesPermission();
        return (ActionResult) this.RedirectToAction("index", "licenses", (object) new
        {
          routeArea = "Admin",
          serviceHost = this.TfsRequestContext.ServiceHost.OrganizationServiceHost,
          project = "",
          team = ""
        });
      }
    }

    [TfsTraceFilter(502011, 502020)]
    [HttpGet]
    public ActionResult Settings()
    {
      if (this.NavigationContext.TopMostLevel == NavigationContextLevels.Application || this.NavigationContext.TopMostLevel == NavigationContextLevels.Collection)
        return (ActionResult) this.View(nameof (Settings));
      throw new HttpException(404, WACommonResources.PageNotFound);
    }

    private void CheckCollectionReadPermission()
    {
      try
      {
        this.TfsRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(this.TfsRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);
      }
      catch (AccessCheckException ex)
      {
        throw new HttpException(401, ex.Message, (Exception) ex);
      }
    }
  }
}
