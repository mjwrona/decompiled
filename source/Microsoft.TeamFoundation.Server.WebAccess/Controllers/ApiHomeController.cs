// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.ApiHomeController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.ApplicationAll)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiHomeController : TfsController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index() => throw new HttpException(404, WACommonResources.PageNotFound);
  }
}
