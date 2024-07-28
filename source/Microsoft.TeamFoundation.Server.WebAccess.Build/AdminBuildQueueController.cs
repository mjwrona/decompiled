// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.AdminBuildQueueController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  [SupportedRouteArea("Admin", NavigationContextLevels.Collection | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  public class AdminBuildQueueController : BuildAreaController
  {
    public AdminBuildQueueController() => this.m_executeContributedRequestHandlers = true;

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(510998, 510999)]
    public ActionResult Index() => (ActionResult) this.View((object) BuildSettingsAdminViewModel.Create(this.TfsWebContext));
  }
}
