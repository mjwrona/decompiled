// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.Msal.MsalPopupController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers.Msal
{
  [SupportedRouteArea("Public", NavigationContextLevels.All)]
  public class MsalPopupController : WebPlatformController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public ActionResult Index() => (ActionResult) this.View("MsalPopup");
  }
}
