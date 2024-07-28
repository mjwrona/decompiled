// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.AdminTestManagementController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [SupportedRouteArea("Admin", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
  public class AdminTestManagementController : TfsAreaController
  {
    public override string AreaName => "TestManagement";

    public AdminTestManagementController() => this.m_executeContributedRequestHandlers = true;

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index() => (ActionResult) this.View(nameof (Index));
  }
}
