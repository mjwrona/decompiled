// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.Controllers.UserProfileController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account.Controllers
{
  [SupportedRouteArea(NavigationContextLevels.ApplicationAll)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class UserProfileController : AccountAreaController
  {
    [HttpGet]
    public ActionResult Information() => (ActionResult) this.View();

    [HttpGet]
    public ActionResult Preferences() => (ActionResult) this.View((object) new UserProfilePreferencesModel(this.TfsWebContext));

    [HttpGet]
    public ActionResult GetUserProfile()
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      return (ActionResult) this.Json((object) new UserProfileInformationModel(this.TfsWebContext), JsonRequestBehavior.AllowGet);
    }
  }
}
