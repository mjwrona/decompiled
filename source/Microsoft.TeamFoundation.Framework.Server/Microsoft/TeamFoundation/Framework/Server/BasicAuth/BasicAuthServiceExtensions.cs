// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BasicAuth.BasicAuthServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.BasicAuth
{
  public static class BasicAuthServiceExtensions
  {
    public static string GetCurrentUserBasicAuthUsername(this IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Application).GetUserIdentity()?.GetProperty<string>("Alias", string.Empty);

    public static void SetCurrentUserBasicAuthUsername(
      this IVssRequestContext requestContext,
      string newAlias)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = vssRequestContext.GetUserIdentity();
      userIdentity.SetProperty("Alias", (object) newAlias);
      IVssRequestContext requestContext1 = vssRequestContext;
      Microsoft.VisualStudio.Services.Identity.Identity[] identities = new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        userIdentity
      };
      service.UpdateIdentities(requestContext1, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities);
    }

    public static bool CurrentUserHasAlternateCredential(
      this ITeamFoundationBasicAuthService basicAuthService,
      IVssRequestContext requestContext)
    {
      IVssRequestContext basicAuthContext = requestContext.GetScopedBasicAuthContext();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = basicAuthContext.GetUserIdentity();
      return basicAuthService.HasBasicCredential(basicAuthContext, userIdentity);
    }

    public static bool IsBasicAuthDisabledForCurrentUser(
      this ITeamFoundationBasicAuthService basicAuthService,
      IVssRequestContext requestContext)
    {
      IVssRequestContext basicAuthContext = requestContext.GetScopedBasicAuthContext();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = basicAuthContext.GetUserIdentity();
      return basicAuthService.IsBasicAuthDisabled(basicAuthContext, userIdentity);
    }

    public static IVssRequestContext GetScopedBasicAuthContext(
      this IVssRequestContext requestContext)
    {
      return !requestContext.IsFeatureEnabled("VisualStudio.Services.BasicAuthorization.EnterpriseStore") ? requestContext.To(TeamFoundationHostType.Deployment) : requestContext.To(TeamFoundationHostType.Application);
    }
  }
}
