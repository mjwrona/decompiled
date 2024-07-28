// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.OiJobMonitoringController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Monitoring, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2931506-B8BC-4923-B99C-2CD8E1087ABB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Monitoring.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Monitoring
{
  [SupportedRouteArea("Oi", NavigationContextLevels.Deployment | NavigationContextLevels.Application)]
  public class OiJobMonitoringController : JobMonitoringAreaController
  {
    private const int JobReportingStart = 1003000;
    private const int JobReportingEnd = 1003999;

    [AcceptVerbs(HttpVerbs.Get)]
    [RequireDeploymentAdmin(true)]
    [TfsTraceFilter(1003000, 1003999)]
    public ActionResult Index() => (ActionResult) this.View();
  }
}
