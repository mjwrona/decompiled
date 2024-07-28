// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.FeedbackResponseController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [SupportedRouteArea(NavigationContextLevels.Team)]
  public class FeedbackResponseController : TestManagementAreaController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525100, 525200)]
    public ActionResult Index() => (ActionResult) this.View("FeedbackResponse");
  }
}
