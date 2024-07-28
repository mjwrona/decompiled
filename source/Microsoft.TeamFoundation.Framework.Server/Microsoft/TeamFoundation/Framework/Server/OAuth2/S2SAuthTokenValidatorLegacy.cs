// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.S2SAuthTokenValidatorLegacy
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal class S2SAuthTokenValidatorLegacy : OAuth2TokenValidator
  {
    private const string TraceLayer = "AuthTokenValidator";
    private const string TraceArea = "Authentication";
    private const string AADWellKnownServicePrincipal = "00000001-0000-0000-c000-000000000000";
    private bool _enabled;
    private string _issuer;
    private IEnumerable<string> _validIssuers;

    public S2SAuthTokenValidatorLegacy()
      : base("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
    {
    }

    public override IEnumerable<string> ValidIssuers => this._validIssuers;

    public override OAuth2TokenValidators ValidatorType => OAuth2TokenValidators.S2S;

    public override void Initialize(
      IVssRequestContext requestContext,
      IOAuth2SettingsService settings)
    {
      IS2SAuthSettings s2SauthSettings = settings.GetS2SAuthSettings(requestContext);
      this._enabled = s2SauthSettings.Enabled;
      if (this._enabled)
      {
        this._issuer = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}", (object) "00000001-0000-0000-c000-000000000000", (object) s2SauthSettings.TenantId);
        this._validIssuers = (IEnumerable<string>) new string[1]
        {
          this._issuer
        };
      }
      else
        this._validIssuers = (IEnumerable<string>) Array.Empty<string>();
    }

    public override bool CanValidateToken(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      if (token.Actor != null || !this._enabled)
        return false;
      string issuer = token.Issuer;
      return !string.IsNullOrEmpty(issuer) && issuer.Equals(this._issuer, StringComparison.OrdinalIgnoreCase);
    }

    internal override bool ValidateIdentity(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      if (token.Actor != null)
      {
        string jwtEncodedString = token.Actor;
        if (token.Actor.StartsWith("\""))
          jwtEncodedString = jwtEncodedString.Remove(0, 1);
        if (token.Actor.EndsWith("\""))
          jwtEncodedString = jwtEncodedString.Remove(jwtEncodedString.Length - 1, 1);
        S2SAuthTokenValidatorLegacy.TransformNameIdClaim(new JwtSecurityToken(jwtEncodedString), identity.Actor);
      }
      else
        S2SAuthTokenValidatorLegacy.TransformNameIdClaim(token, identity);
      string name = token.Actor != null ? identity.Actor.Name : identity.Name;
      requestContext.Trace(5510400, TraceLevel.Verbose, "Authentication", "AuthTokenValidator", "S2SAuthTokenValidator servicePrincipalId:{0}, token.Audience: {1}", (object) name, (object) token.Audiences.First<string>());
      Guid servicePrincipal = this.ParseServicePrincipal(name);
      if (servicePrincipal == Guid.Empty)
      {
        requestContext.Trace(5510400, TraceLevel.Error, "Authentication", "AuthTokenValidator", "Could not parse token service principal id");
        return false;
      }
      if (!ServicePrincipals.IsInternalServicePrincipalId(servicePrincipal))
      {
        requestContext.Trace(5510400, TraceLevel.Error, "Authentication", "AuthTokenValidator", "The authenticated service principal ID was not in the correct format. Id: {0}", (object) servicePrincipal);
        return false;
      }
      AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.S2S_Legacy);
      return true;
    }

    private Guid ParseServicePrincipal(string name)
    {
      Guid result;
      return !Guid.TryParse(name.Split('@')[0], out result) ? Guid.Empty : result;
    }

    internal override void TransformIdentityClaims(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity,
      out bool impersonating)
    {
      impersonating = identity.Actor != null;
      requestContext.To(TeamFoundationHostType.Deployment).Items[RequestContextItemsKeys.AlternateAuthCredentialsContextKey] = (object) true;
      requestContext.To(TeamFoundationHostType.Deployment).Items[RequestContextItemsKeys.AlternateAuthCredentialsIdentityCreatorContextKey] = (object) true;
      S2SAuthTokenValidatorLegacy.TransformNameIdClaim(token, identity);
      if (impersonating)
      {
        requestContext.Trace(5510410, TraceLevel.Info, "Authentication", "AuthTokenValidator", "Recieved an impersonating token from issuer {0}", (object) token.Issuer);
        string jwtEncodedString = token.Actor;
        if (token.Actor.StartsWith("\""))
          jwtEncodedString = jwtEncodedString.Remove(0, 1);
        if (token.Actor.EndsWith("\""))
          jwtEncodedString = jwtEncodedString.Remove(jwtEncodedString.Length - 1, 1);
        S2SAuthTokenValidatorLegacy.TransformNameIdClaim(new JwtSecurityToken(jwtEncodedString), identity.Actor);
      }
      ClaimsIdentity claimsIdentity = impersonating ? identity.Actor : identity;
      Claim claim = claimsIdentity.FindAll("identityprovider").FirstOrDefault<Claim>();
      if (claim != null && !claimsIdentity.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider")))
      {
        string str = claim.Value;
        if (str.Contains("@"))
          str = str.Substring(str.IndexOf("@"));
        claimsIdentity.AddClaim(new Claim("http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider", str));
      }
      claimsIdentity.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Service"));
    }

    private static void TransformNameIdClaim(JwtSecurityToken token, ClaimsIdentity identity)
    {
      Claim claim1 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "nameid"));
      if (claim1 == null)
        return;
      foreach (Claim claim2 in identity.FindAll((Predicate<Claim>) (x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")))
      {
        if (!claim2.Value.Equals(claim1.Value))
          identity.RemoveClaim(claim2);
      }
    }
  }
}
