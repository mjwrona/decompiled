// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ApiStartController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Api", NavigationContextLevels.Application | NavigationContextLevels.Collection)]
  [OutputCache(CacheProfile = "NoCache")]
  public class ApiStartController : AdminAreaController
  {
    private const int WebAccessExceptionEaten = 599999;

    [HttpPost]
    [TfsTraceFilter(503080, 503090)]
    public ActionResult RequeueCollectionJob()
    {
      try
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        SqlRegistryService service1 = vssRequestContext.GetService<SqlRegistryService>();
        TeamFoundationServicingService service2 = vssRequestContext.GetService<TeamFoundationServicingService>();
        Guid jobId = service1.GetValue<Guid>(vssRequestContext, (RegistryQuery) "/Account/Configuration/ActivationJobId", false, new Guid());
        Guid hostId = service1.GetValue<Guid>(vssRequestContext, (RegistryQuery) "/Account/Configuration/ActivationHostId", false, new Guid());
        if (jobId == Guid.Empty || hostId == Guid.Empty)
          return (ActionResult) this.RedirectToAction("monitorJobProgress", "job", (object) new
          {
            routeArea = "Api"
          });
        service2.RequeueServicingJob(vssRequestContext, hostId, jobId);
        return (ActionResult) this.RedirectToAction("monitorJobProgress", "job", (object) new
        {
          routeArea = "Api",
          jobId = jobId
        });
      }
      catch (Exception ex)
      {
        this.LogException(ex);
        this.TraceException(599999, ex);
        return (ActionResult) this.RedirectToAction("monitorJobProgress", "job", (object) new
        {
          routeArea = "Api"
        });
      }
    }
  }
}
