// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.AccountController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Cloud;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  [SupportedRouteArea(NavigationContextLevels.Deployment)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class AccountController : AccountAreaController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(504091, 504100)]
    public ActionResult Error() => (ActionResult) this.View("WebError");

    [AcceptVerbs(HttpVerbs.Get)]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    [TfsTraceFilter(504020, 504030)]
    public ActionResult Home() => (ActionResult) this.Redirect(PortalAndTfsRedirectHelper.GetRedirectUrl(this.TfsRequestContext));
  }
}
