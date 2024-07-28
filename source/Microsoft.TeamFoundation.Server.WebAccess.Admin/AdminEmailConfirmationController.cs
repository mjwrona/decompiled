// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminEmailConfirmationController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Admin", NavigationContextLevels.Deployment)]
  public class AdminEmailConfirmationController : AdminAreaController
  {
    [HttpGet]
    [TfsTraceFilter(500660, 500670)]
    [ValidateInput(false)]
    public ActionResult Index(string hash, Guid tfId, string preferredEmailAddress)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      return (ActionResult) this.View((object) new EmailConfirmationModel()
      {
        EmailAddress = preferredEmailAddress,
        ErrorMessage = AdminServerResources.EmailConfirmationNotSupported
      });
    }
  }
}
