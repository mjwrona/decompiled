// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.AADCookieTokenValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal class AADCookieTokenValidator : AADAuthTokenValidator
  {
    internal const string ValidatedTokenKey = "AadCookieTokenValidator_ValidatedToken";

    public override OAuth2TokenValidators ValidatorType => OAuth2TokenValidators.AADCookie;

    protected override void ProcessAadTokens(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
    }

    protected override void SetAadAuthFlow(IVssRequestContext requestContext)
    {
    }

    protected override void SetAuthenticationMechanism(
      IVssRequestContext requestContext,
      JwtSecurityToken token)
    {
      AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.AAD_Cookie);
    }

    internal override bool PostProcessValidatedToken(
      IVssRequestContext requestContext,
      JwtSecurityToken token)
    {
      requestContext.RootContext.Items["AadCookieTokenValidator_ValidatedToken"] = (object) token;
      return base.PostProcessValidatedToken(requestContext, token);
    }
  }
}
