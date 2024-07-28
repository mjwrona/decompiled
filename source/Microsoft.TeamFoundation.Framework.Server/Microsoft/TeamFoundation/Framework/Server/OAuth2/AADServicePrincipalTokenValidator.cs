// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.AADServicePrincipalTokenValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal class AADServicePrincipalTokenValidator : S2SAuthTokenValidator
  {
    private const string objectIdClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    private readonly IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper;
    private readonly ISet<string> mandatoryClaims = (ISet<string>) new HashSet<string>()
    {
      "appid",
      "tid",
      "oid",
      "appidacr"
    };
    private readonly ISet<string> invalidClaims = (ISet<string>) new HashSet<string>()
    {
      "scp",
      "unique_name"
    };

    protected override AuthenticationMechanism AuthenticationMechanism => AuthenticationMechanism.AADServicePrincipal;

    public override OAuth2TokenValidators ValidatorType => OAuth2TokenValidators.AADServicePrincipal;

    public AADServicePrincipalTokenValidator()
      : this(AadServicePrincipalConfigurationHelper.Instance)
    {
    }

    public AADServicePrincipalTokenValidator(
      IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper)
    {
      this.aadServicePrincipalConfigurationHelper = aadServicePrincipalConfigurationHelper;
    }

    public override void Initialize(
      IVssRequestContext requestContext,
      IOAuth2SettingsService settings)
    {
      IAADAuthSettings aadAuthSettings = settings.GetAADAuthSettings(requestContext);
      this._enabled = aadAuthSettings.Enabled;
      string[] strArray;
      if (!this._enabled)
        strArray = Array.Empty<string>();
      else
        strArray = new string[1]{ aadAuthSettings.Issuer };
      this._validIssuers = (IEnumerable<string>) strArray;
    }

    public override bool CanValidateToken(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      bool flag = this.aadServicePrincipalConfigurationHelper.IsTokenValidationEnabled(requestContext);
      string input1 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "appid"))?.Value;
      string input2 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "tid"))?.Value;
      Guid result1;
      Guid result2;
      return this._enabled & flag && this.HasValidIssuer(token) && this.HasValidClaims(requestContext, token) && Guid.TryParse(input1, out result1) && Guid.TryParse(input2, out result2) && !ServicePrincipals.IsSystemOrFirstPartyServicePrincipal(requestContext, result1, result2, true);
    }

    internal override bool ValidateIdentity(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      int num = !base.ValidateIdentity(requestContext, token, identity) ? 0 : (this.HasExactlyOneClaimOfType(requestContext, identity, "http://schemas.microsoft.com/identity/claims/objectidentifier") ? 1 : 0);
      if (num == 0)
        return num != 0;
      AuthenticationHelpers.SetOAuthAppId(requestContext, token);
      return num != 0;
    }

    internal override void TransformIdentityClaims(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity,
      out bool impersonating)
    {
      impersonating = identity.Actor != null;
      requestContext.RootContext.Items["AuthenticationByIdentityProvider"] = (object) true;
      this.EditNameIdentifierClaim(identity, token);
      this.EditIdentityTypeClaim(identity);
      this.EditAppIdClaim(identity, token);
    }

    private bool HasValidIssuer(JwtSecurityToken token)
    {
      string issuer = token.Issuer;
      if (!string.IsNullOrEmpty(issuer))
      {
        Uri issuerUri;
        if (Uri.TryCreate(issuer, UriKind.Absolute, out issuerUri))
          return this._validIssuers.Any<string>((Func<string, bool>) (validIssuer => validIssuer.Equals(issuerUri.Host, StringComparison.OrdinalIgnoreCase)));
      }
      return false;
    }

    protected override bool ValidateServicePrincipal(string name, out Guid servicePrincipal) => Guid.TryParse(name, out servicePrincipal);

    private bool HasValidClaims(IVssRequestContext requestContext, JwtSecurityToken token) => this.TokenHasMandatoryClaims(requestContext, token) && this.CheckIfTokenClaimsAreValid(requestContext, token);

    private bool TokenHasMandatoryClaims(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      IEnumerable<string> missingMandatoryClaims = this.mandatoryClaims.Except<string>(token.Claims.Select<Claim, string>((Func<Claim, string>) (claim => claim.Type)));
      if (!missingMandatoryClaims.Any<string>())
        return true;
      Func<string> message = (Func<string>) (() => "AAD service principal is missing the following mandatory claims: " + string.Join(",", missingMandatoryClaims.Select<string, string>((Func<string, string>) (x => "'" + x + "'"))));
      requestContext.TraceConditionally(55107900, TraceLevel.Warning, "Authentication", "AuthTokenValidator", message);
      return false;
    }

    private bool CheckIfTokenClaimsAreValid(
      IVssRequestContext requestContext,
      JwtSecurityToken token)
    {
      IEnumerable<Claim> existingInvalidClaims = token.Claims.Where<Claim>((Func<Claim, bool>) (claim => this.invalidClaims.Contains(claim.Type)));
      if (!existingInvalidClaims.Any<Claim>())
        return true;
      Func<string> message = (Func<string>) (() => "AAD service principal has the following invalid claims present: " + string.Join(",", existingInvalidClaims.Select<Claim, string>((Func<Claim, string>) (x => string.Format("'{0}'", (object) x)))));
      requestContext.TraceConditionally(55107901, TraceLevel.Warning, "Authentication", "AuthTokenValidator", message);
      return false;
    }

    private void EditNameIdentifierClaim(ClaimsIdentity identity, JwtSecurityToken token)
    {
      string str1 = token.Claims.First<Claim>((Func<Claim, bool>) (x => x.Type == "tid")).Value;
      string str2 = token.Claims.First<Claim>((Func<Claim, bool>) (x => x.Type == "oid")).Value;
      this.ReplaceClaim(identity, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", str1 + "\\" + str2);
    }

    private void EditIdentityTypeClaim(ClaimsIdentity identity) => this.ReplaceClaim(identity, "IdentityTypeClaim", "Microsoft.VisualStudio.Services.Claims.AadServicePrincipal");

    private void EditAppIdClaim(ClaimsIdentity identity, JwtSecurityToken token)
    {
      string newClaimValue = token.Claims.First<Claim>((Func<Claim, bool>) (x => x.Type == "appid"))?.Value;
      this.ReplaceClaim(identity, "appid", newClaimValue);
    }

    private void ReplaceClaim(ClaimsIdentity identity, string claimName, string newClaimValue)
    {
      Claim first = identity.FindFirst(claimName);
      if (first != null)
        identity.RemoveClaim(first);
      identity.AddClaim(new Claim(claimName, newClaimValue));
    }

    private bool HasExactlyOneClaimOfType(
      IVssRequestContext requestContext,
      ClaimsIdentity identity,
      string claimName)
    {
      IEnumerable<Claim> all = identity.FindAll(claimName);
      if (all.Count<Claim>() == 1)
        return true;
      requestContext.Trace(5510400, TraceLevel.Error, "Authentication", "AuthTokenValidator", string.Format("Token must contain only one '{0}' claim but it contains {1}.", (object) claimName, (object) all.Count<Claim>()));
      return false;
    }
  }
}
