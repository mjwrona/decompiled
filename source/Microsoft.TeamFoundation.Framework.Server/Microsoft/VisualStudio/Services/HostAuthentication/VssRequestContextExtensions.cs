// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostAuthentication.VssRequestContextExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Web;

namespace Microsoft.VisualStudio.Services.HostAuthentication
{
  internal static class VssRequestContextExtensions
  {
    private static readonly string s_area = "HostAuthentication";
    private static readonly string s_layer = "HostAuthenticationCookie";

    internal static bool IsHostAuthenticated(this IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || !requestContext.RootContext.Items.ContainsKey("AuthenticationWithSessionAuth"))
        return true;
      IWebRequestContextInternal requestContextInternal = requestContext as IWebRequestContextInternal;
      ClaimsPrincipal principal = (ClaimsPrincipal) null;
      if (requestContextInternal != null)
      {
        HttpContextBase httpContext = requestContextInternal.HttpContext;
        if (httpContext != null)
          principal = httpContext.User as ClaimsPrincipal;
      }
      if (principal != null && principal.HasClaim((Predicate<Claim>) (x => string.Equals(x.Type, "IsClient", StringComparison.OrdinalIgnoreCase) && x.Value == bool.TrueString)))
      {
        requestContext.Trace(334536, TraceLevel.Info, "HostAuthentication", nameof (IsHostAuthenticated), "Skipping Host Authentication Cookie Validation since request is from Legacy Sign-In Flow");
        return true;
      }
      if (principal != null && !HostAuthenticationCookieRolloutHelper.ShouldUseHostAuthenticationCookie(requestContext, principal))
      {
        requestContext.Trace(334538, TraceLevel.Info, "HostAuthentication", nameof (IsHostAuthenticated), "Skipping Host Authentication Cookie Validation since feature is not rolledout for user");
        return true;
      }
      if (!requestContext.IsDevOpsDomainRequestUsingRootContext() || requestContext.RequestRestrictions() != null && requestContext.RequestRestrictions().RequiredAuthentication < RequiredAuthentication.ValidatedUser)
        return true;
      HostAuthenticationToken authenticationToken = HostAuthenticationCookie.GetHostAuthenticationToken(requestContext);
      bool flag = authenticationToken != null && authenticationToken.IsHostAuthenticated(requestContext.ServiceHost.InstanceId);
      if (!flag)
        requestContext.Trace(334537, TraceLevel.Info, "HostAuthentication", nameof (IsHostAuthenticated), "HostAuthentication JWT token was not valid.");
      return !flag && requestContext.UserAgent != null && !new UserAgentDetails(requestContext.UserAgent).IsBrowser || flag;
    }

    internal static void SetHostAuthenticated(this IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        requestContext.Trace(340041, TraceLevel.Info, VssRequestContextExtensions.s_area, VssRequestContextExtensions.s_layer, "Write of host authentication cookie not enabled at deployment.");
      else if (!requestContext.IsDevOpsDomainRequestUsingRootContext())
      {
        requestContext.Trace(340042, TraceLevel.Info, VssRequestContextExtensions.s_area, VssRequestContextExtensions.s_layer, "Write of host authentication cookie not enabled  if it is a DevOps domain");
      }
      else
      {
        HostAuthenticationToken token = HostAuthenticationCookie.GetHostAuthenticationToken(requestContext) ?? new HostAuthenticationToken();
        token.AddHostAuthentication(requestContext.ServiceHost.InstanceId, requestContext.GetUserIdentity().SubjectDescriptor);
        HostAuthenticationCookie.SetHostAuthenticationToken(requestContext, token);
        requestContext.Trace(340043, TraceLevel.Info, VssRequestContextExtensions.s_area, VssRequestContextExtensions.s_layer, "Wrote host id {0} to host authentication cookie for user.", (object) requestContext.ServiceHost.InstanceId);
      }
    }
  }
}
