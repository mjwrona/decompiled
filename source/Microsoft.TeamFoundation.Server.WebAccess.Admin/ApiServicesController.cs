// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ApiServicesController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Api", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiServicesController : AdminAreaController
  {
    [HttpPost]
    [ValidateInput(false)]
    public ActionResult Disconnect(string name)
    {
      this.CheckManageServicesPermission();
      TeamFoundationConnectedServicesService service = this.TfsRequestContext.GetService<TeamFoundationConnectedServicesService>();
      if (service.GetConnectedService(this.TfsRequestContext, name, this.TfsWebContext.Project.Name) != null)
        service.DeleteConnectedService(this.TfsRequestContext, name, this.TfsWebContext.Project.Name);
      return (ActionResult) new EmptyResult();
    }
  }
}
