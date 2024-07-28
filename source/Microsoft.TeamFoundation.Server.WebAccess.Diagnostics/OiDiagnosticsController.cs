// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.OiDiagnosticsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6A4F2FF9-BE93-434B-9864-FE0D09D21D75
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.Controllers;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Diagnostics
{
  [SupportedRouteArea("Oi", NavigationContextLevels.All)]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [OutputCache(CacheProfile = "NoCache")]
  public class OiDiagnosticsController : DiagnosticsAreaController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Deployment | NavigationContextLevels.Application)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult ActivityLog() => (ActionResult) this.View();

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Deployment)]
    public ActionResult Tracing()
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).Elevate();
      TeamFoundationIdentityService service = vssRequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity readIdentity = service.ReadIdentities(vssRequestContext, new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      })[0];
      return !service.IsMember(vssRequestContext, readIdentity.Descriptor, this.TfsRequestContext.UserContext) && !this.TfsRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, FrameworkSecurity.DiagnosticNamespaceId).HasPermission(this.TfsRequestContext, FrameworkSecurity.DiagnosticNamespaceToken, 4) ? (ActionResult) new RedirectResult("~/") : (ActionResult) this.View();
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Deployment)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(false)]
    public ActionResult AlertPublishingTest(
      int eventId,
      EventLogEntryType eventType,
      string message,
      params object[] eventValues)
    {
      if (!this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(this.TfsRequestContext, "VisualStudio.FrameworkService.AlertPublishing"))
        return (ActionResult) this.Content("Error: AlertPublishing is not enabled.");
      TeamFoundationEventLog.Default.Log(this.TfsRequestContext, message, eventId, eventType, eventValues);
      return (ActionResult) this.Content(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Success. Event logged at {0}. Event may or may not be treated as an alert, depending on alerting configuration.", (object) DateTime.UtcNow));
    }
  }
}
