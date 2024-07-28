// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.MyWork.WITWidgetController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.MyWork, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8442996D-DF5E-4B6F-9622-CCF23EF07ED1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.MyWork.dll

using Microsoft.TeamFoundation.Server.WebAccess.MyWork.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.MyWork
{
  [SupportedRouteArea(NavigationContextLevels.Deployment | NavigationContextLevels.Application)]
  [OutputCache(CacheProfile = "NoCache")]
  public class WITWidgetController : MyWorkAreaController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index() => (ActionResult) this.View((object) new WITWidgetViewModel());
  }
}
