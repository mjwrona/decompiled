// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.OAuth2TokenValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public abstract class OAuth2TokenValidator : IOAuth2TokenValidator
  {
    private const string TraceArea = "Authentication";
    private const string TraceLayer = "OAuth2TokenValidator";

    protected OAuth2TokenValidator()
      : this("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
    {
    }

    public virtual bool ValidateAudience(
      IVssRequestContext requestContext,
      IEnumerable<string> audiences,
      Microsoft.IdentityModel.Tokens.SecurityToken securityToken,
      TokenValidationParameters validationParameters)
    {
      if (audiences == null || !audiences.Any<string>())
        return false;
      if (!(securityToken is JwtSecurityToken jwtSecurityToken) || jwtSecurityToken.Audiences == null || !jwtSecurityToken.Audiences.Any<string>() || validationParameters == null || validationParameters.ValidAudiences == null || !validationParameters.ValidAudiences.Any<string>() || !validationParameters.ValidateAudience)
        return true;
      foreach (string audience in audiences)
      {
        if (!string.IsNullOrWhiteSpace(audience) && !this.ValidateServiceAudience(requestContext, audience, validationParameters))
          return false;
      }
      return true;
    }

    public virtual IEnumerable<IdentityDescriptor> ProcessScopes(
      IVssRequestContext requestContext,
      ClaimsPrincipal claimsPrincipal)
    {
      return (IEnumerable<IdentityDescriptor>) null;
    }

    protected bool ValidateServiceAudience(
      IVssRequestContext requestContext,
      string audience,
      TokenValidationParameters tokenValidationParameters)
    {
      if (tokenValidationParameters.ValidAudiences.Contains<string>(audience, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        return true;
      if (tokenValidationParameters.ValidAudiences.Contains<string>("*", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        Uri result;
        if (Uri.TryCreate(audience, UriKind.Absolute, out result))
          audience = result.Host;
        Uri requestUri = this.GetRequestUri(requestContext);
        if (requestUri != (Uri) null)
          return string.Equals(audience, requestUri.Host, StringComparison.OrdinalIgnoreCase);
      }
      return false;
    }

    protected virtual Uri GetRequestUri(IVssRequestContext requestContext) => requestContext == null ? (Uri) null : requestContext.RequestUri();

    protected OAuth2TokenValidator(string nameClaimType)
      : this(nameClaimType, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
    {
    }

    protected OAuth2TokenValidator(string nameClaimType, string roleClaimType)
    {
      this.NameClaimType = nameClaimType;
      this.RoleClaimType = roleClaimType;
    }

    public string NameClaimType { get; private set; }

    public string RoleClaimType { get; private set; }

    public abstract IEnumerable<string> ValidIssuers { get; }

    public abstract OAuth2TokenValidators ValidatorType { get; }

    public abstract void Initialize(
      IVssRequestContext requestContext,
      IOAuth2SettingsService settings);

    public abstract bool CanValidateToken(IVssRequestContext requestContext, JwtSecurityToken token);

    public void ValidateToken(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsPrincipal principal,
      out bool impersonating,
      out bool validIdentity)
    {
      impersonating = false;
      validIdentity = false;
      requestContext.TraceEnter(5510200, "Authentication", nameof (OAuth2TokenValidator), nameof (ValidateToken));
      try
      {
        this.TraceTokenDetails(requestContext, token);
        if (!this.SupportNestedToken && token != null && token.Actor != null)
          requestContext.Trace(5510201, TraceLevel.Info, "Authentication", nameof (OAuth2TokenValidator), "This token validator does not support nested tokens. Skipping token validaton and returning null");
        else if (!this.CanValidateToken(requestContext, token))
          requestContext.Trace(5510201, TraceLevel.Info, "Authentication", nameof (OAuth2TokenValidator), "CanValidateToken returned false. Skipping token validaton and returning null");
        else if (!this.ValidateIdentity(requestContext, token, (ClaimsIdentity) principal.Identity))
          requestContext.Trace(5510202, TraceLevel.Warning, "Authentication", nameof (OAuth2TokenValidator), "Token validation failed for issuer {0}. The token was validated, but the identity was not authorized for access.", (object) token.Issuer);
        else if (!this.ValidateScopes(requestContext, token))
        {
          requestContext.Trace(5510202, TraceLevel.Warning, "Authentication", nameof (OAuth2TokenValidator), "Token validation failed for issuer {0}. The token was validated, but the scopes were not valid for the request.", (object) token.Issuer);
        }
        else
        {
          if (!this.PostProcessValidatedToken(requestContext, token))
            return;
          this.TransformIdentityClaims(requestContext, token, (ClaimsIdentity) principal.Identity, out impersonating);
          validIdentity = true;
        }
      }
      finally
      {
        requestContext.TraceLeave(5510203, "Authentication", nameof (OAuth2TokenValidator), nameof (ValidateToken));
      }
    }

    internal abstract bool ValidateIdentity(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity);

    internal virtual bool ValidateScopes(IVssRequestContext requestContext, JwtSecurityToken token) => true;

    internal virtual bool PostProcessValidatedToken(
      IVssRequestContext requestContext,
      JwtSecurityToken token)
    {
      return true;
    }

    internal virtual void TraceTokenDetails(
      IVssRequestContext requestContext,
      JwtSecurityToken token)
    {
    }

    internal virtual void TransformIdentityClaims(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity,
      out bool impersonating)
    {
      impersonating = false;
    }

    protected virtual bool SupportNestedToken => false;
  }
}
