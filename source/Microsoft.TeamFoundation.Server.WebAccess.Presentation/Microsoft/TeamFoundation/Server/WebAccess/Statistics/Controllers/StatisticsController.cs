// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Statistics.Controllers.StatisticsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Statistics.Controllers
{
  [SupportedRouteArea(NavigationContextLevels.All)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class StatisticsController : TfsController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index() => (ActionResult) this.View();
  }
}
