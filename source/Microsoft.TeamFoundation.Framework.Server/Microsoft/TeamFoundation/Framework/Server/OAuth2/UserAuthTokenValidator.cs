// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.UserAuthTokenValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.IdentityModel.Tokens;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal class UserAuthTokenValidator : OAuth2TokenValidator
  {
    private const string TraceArea = "Authentication";
    private const string TraceLayer = "UserAuthTokenValidator";
    private IEnumerable<string> m_trustedIssuers;
    private string m_audience;
    private bool m_enabled;
    private readonly UserAuthenticationSessionTokenProvider m_userAuthenticationSessionTokenProvider;

    public override IEnumerable<string> ValidIssuers => this.m_trustedIssuers;

    public override OAuth2TokenValidators ValidatorType => OAuth2TokenValidators.UserAuthentication;

    public UserAuthTokenValidator()
      : this(new UserAuthenticationSessionTokenProvider())
    {
    }

    public UserAuthTokenValidator(
      UserAuthenticationSessionTokenProvider userAuthenticationSessionTokenProvider)
    {
      this.m_userAuthenticationSessionTokenProvider = userAuthenticationSessionTokenProvider;
    }

    public override void Initialize(
      IVssRequestContext requestContext,
      IOAuth2SettingsService settings)
    {
      IUserAuthSettings userAuthSettings = settings.GetUserAuthSettings(requestContext);
      this.m_enabled = userAuthSettings.Enabled;
      this.m_audience = userAuthSettings.Audience;
      this.m_trustedIssuers = userAuthSettings.TrustedIssuers;
    }

    public override bool CanValidateToken(IVssRequestContext requestContext, JwtSecurityToken token) => this.m_enabled && this.ContainsValidAudience(token.Audiences) && !string.IsNullOrEmpty(token.Issuer) && this.m_trustedIssuers.Contains<string>(token.Issuer, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public override bool ValidateAudience(
      IVssRequestContext requestContext,
      IEnumerable<string> audiences,
      SecurityToken securityToken,
      TokenValidationParameters validationParameters)
    {
      return this.ContainsValidAudience(audiences);
    }

    private bool ContainsValidAudience(IEnumerable<string> audiences) => audiences != null && audiences.Count<string>() == 1 && StringComparer.OrdinalIgnoreCase.Equals(audiences.FirstOrDefault<string>(), this.m_audience);

    internal override bool ValidateIdentity(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.UserAuthToken);
      AuthenticationHelpers.SetCanIssueUserAuthenticationToken(requestContext, true);
      requestContext.RootContext.Items["AuthenticationWithSessionAuth"] = (object) true;
      return true;
    }

    internal override void TransformIdentityClaims(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity,
      out bool impersonating)
    {
      Claim claim1 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "tid"));
      if (claim1 != null)
      {
        string str = claim1.Value;
        identity.AddClaim(new Claim("http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider", str));
        identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "uri:WindowsLiveId"));
      }
      else
      {
        identity.AddClaim(new Claim("http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider", "Windows Live ID"));
        identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "Windows Live ID"));
      }
      foreach (Claim claim2 in identity.Claims)
      {
        string validLongClaimType = UserAuthenticationSessionTokenHandler.ClaimTypeMapping.GetValidLongClaimType(claim2.Type);
        if (validLongClaimType != null)
        {
          identity.RemoveClaim(claim2);
          identity.AddClaim(new Claim(validLongClaimType, claim2.Value));
        }
      }
      impersonating = false;
    }

    internal override bool PostProcessValidatedToken(
      IVssRequestContext requestContext,
      JwtSecurityToken token)
    {
      if (!this.m_userAuthenticationSessionTokenProvider.IsUserAuthenticationDisabled(requestContext, token))
        return true;
      requestContext.RootContext.Items["$UserAuthenticationSkipped"] = (object) true;
      requestContext.Trace(5510202, TraceLevel.Warning, "Authentication", nameof (UserAuthTokenValidator), "Token validation failed for issuer {0}. The token was validated, but UserAuthentication is disabled.", (object) token.Issuer);
      return false;
    }
  }
}
