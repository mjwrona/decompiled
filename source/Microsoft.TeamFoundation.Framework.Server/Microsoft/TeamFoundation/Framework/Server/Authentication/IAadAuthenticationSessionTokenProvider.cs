// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.IAadAuthenticationSessionTokenProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.IdentityModel.Tokens.Jwt;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public interface IAadAuthenticationSessionTokenProvider
  {
    IAadAuthenticationSessionTokenConfiguration Configuration { get; }

    bool TryGetAadAuthenticationSessionCookie(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      out UserAuthenticationSessionToken aadAccessToken);

    void DeleteSessionToken(IVssRequestContext requestContext, HttpContextBase httpContext);

    void IssueSessionToken(IVssRequestContext requestContext, HttpContextBase httpContext);

    UserAuthenticationSessionToken ReadSessionToken(
      IVssRequestContext requestContext,
      HttpContextBase context);

    void UpdateSessionToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      JwtSecurityToken token);

    void WriteTokenToCookie(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      JwtSecurityToken token);

    AuthOptions GetAuthOptions(IVssRequestContext requestContext);
  }
}
