// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.SkypeTeamTabsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  [SupportedRouteArea(NavigationContextLevels.Collection)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [TfsHandleFeatureFlag("WebAccess.SkypeTeams.TabConfiguration", null)]
  public class SkypeTeamTabsController : TfsController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult BoardTab()
    {
      if (!this.TfsWebContext.IsHosted)
        throw new HttpException(404, WACommonResources.PageNotFound);
      this.EnsureCurrentUserHasGlobalReadAccess();
      if (this.TfsWebContext.TfsRequestContext.IsFeatureEnabled("WebAccess.SkypeTeams.TabFrameAncestorsEnabled"))
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        string policyValue = vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, (RegistryQuery) "/Configuration/WebPlatform/TeamsApp/ContentSecurityPolicy/FrameAncestors", "*.teams.microsoft.com teams.microsoft.com skype.com");
        vssRequestContext.GetService<IContentSecurityPolicyHeaderManagementService>().AddAdditionalPolicyForDirective(this.TfsRequestContext, this.HttpContext, "frame-ancestors", policyValue);
      }
      return (ActionResult) this.View();
    }

    private void EnsureCurrentUserHasGlobalReadAccess()
    {
      if (!this.TfsWebContext.CurrentUserHasGlobalReadAccess)
      {
        InvalidAccessException invalidAccessException = new InvalidAccessException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, PresentationServerResources.ErrorApplicationLevelAccessDenied, (object) this.TfsRequestContext.AuthenticatedUserName));
        invalidAccessException.ReportException = false;
        throw invalidAccessException;
      }
    }
  }
}
