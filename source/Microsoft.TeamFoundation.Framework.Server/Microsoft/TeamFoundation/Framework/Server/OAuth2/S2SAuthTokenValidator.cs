// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.S2SAuthTokenValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public abstract class S2SAuthTokenValidator : OAuth2TokenValidator
  {
    protected const string TraceLayer = "AuthTokenValidator";
    protected const string TraceArea = "Authentication";
    protected bool _enabled;
    protected IEnumerable<string> _validIssuers;
    protected string _tenantDomain;
    private readonly IJwtTracer _jwtTracer;

    protected abstract AuthenticationMechanism AuthenticationMechanism { get; }

    public S2SAuthTokenValidator()
      : this(JwtTracer.Instance)
    {
    }

    public S2SAuthTokenValidator(IJwtTracer jwtTracer)
      : base("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
    {
      ArgumentUtility.CheckForNull<IJwtTracer>(jwtTracer, nameof (jwtTracer));
      this._jwtTracer = jwtTracer;
    }

    public override IEnumerable<string> ValidIssuers => this._validIssuers;

    public override OAuth2TokenValidators ValidatorType => OAuth2TokenValidators.S2S;

    public override bool CanValidateToken(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      if (this._enabled)
      {
        if (token.Actor != null)
        {
          string[] strArray = token.Issuer.Split('@');
          return strArray.Length == 2 && this.ValidateServicePrincipal(strArray[0], out Guid _) && strArray[1].Equals(this._tenantDomain, StringComparison.OrdinalIgnoreCase);
        }
        string issuer = token.Issuer;
        if (!string.IsNullOrEmpty(issuer) && this._validIssuers.Contains<string>(issuer, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        {
          Claim claim = token.Claims.Where<Claim>((Func<Claim, bool>) (x => x.Type == "appid")).FirstOrDefault<Claim>();
          if (claim != null)
            return this.ValidateServicePrincipal(claim.Value, out Guid _);
        }
      }
      return false;
    }

    internal override bool ValidateIdentity(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      ClaimsIdentity claimsIdentity = identity;
      if (token.Actor != null)
        claimsIdentity = identity.Actor;
      requestContext.Trace(14100005, TraceLevel.Verbose, "Authentication", "AuthTokenValidator", "JWT Dump: {0}", (object) token);
      IEnumerable<Claim> all1 = claimsIdentity.FindAll("appid");
      if (all1.Count<Claim>() != 1)
      {
        if (!all1.Any<Claim>())
        {
          requestContext.Trace(5510400, TraceLevel.Error, "Authentication", "AuthTokenValidator", "Token did not contain appid claim.");
          return false;
        }
        requestContext.Trace(5510400, TraceLevel.Error, "Authentication", "AuthTokenValidator", "Token contains multiple appid claims.");
        return false;
      }
      IEnumerable<Claim> all2 = claimsIdentity.FindAll("http://schemas.microsoft.com/identity/claims/tenantid");
      if (all2.Count<Claim>() != 1)
      {
        if (!all2.Any<Claim>())
        {
          requestContext.Trace(5510400, TraceLevel.Error, "Authentication", "AuthTokenValidator", "Token did not contain tid claim.");
          return false;
        }
        requestContext.Trace(5510400, TraceLevel.Error, "Authentication", "AuthTokenValidator", "Token contains multiple tid claims.");
        return false;
      }
      string name = all1.First<Claim>().Value;
      requestContext.Trace(5510400, TraceLevel.Verbose, "Authentication", "AuthTokenValidator", "S2SAuthTokenValidator servicePrincipalId:{0}, token.Audience: {1}", (object) name, (object) token.Audiences.First<string>());
      Guid servicePrincipal;
      if (!this.ValidateServicePrincipal(name, out servicePrincipal))
      {
        requestContext.Trace(5510400, TraceLevel.Error, "Authentication", "AuthTokenValidator", "Could not parse token service principal id or the authenticated service principal ID was not in the correct format. Id: {0}", (object) servicePrincipal);
        return false;
      }
      AuthenticationHelpers.SetAuthenticationMechanism(requestContext, this.AuthenticationMechanism);
      return true;
    }

    internal override void TransformIdentityClaims(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity,
      out bool impersonating)
    {
      impersonating = identity.Actor != null;
      ClaimsIdentity identity1 = identity;
      if (token.Actor != null)
        identity1 = identity.Actor;
      string str1 = S2SAuthTokenValidator.TransformNameIdClaim(identity1);
      if (string.IsNullOrEmpty(identity1.Name) || !str1.Equals(identity1.Name, StringComparison.Ordinal))
        requestContext.Trace(5510400, TraceLevel.Warning, "Authentication", "AuthTokenValidator", "Identity Name property is not set to the AppId claim.");
      Claim claim = identity1.FindAll("identityprovider").FirstOrDefault<Claim>();
      if (claim != null && !identity1.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider")))
      {
        string str2 = claim.Value;
        if (str2.Contains("@"))
          str2 = str2.Substring(str2.IndexOf("@"));
        identity1.AddClaim(new Claim("http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider", str2));
      }
      identity1.AddClaim(new Claim("IdentityTypeClaim", "System:ServicePrincipal"));
    }

    internal override void TraceTokenDetails(
      IVssRequestContext requestContext,
      JwtSecurityToken token)
    {
      this._jwtTracer.Trace(requestContext, token);
    }

    private static string TransformNameIdClaim(ClaimsIdentity identity)
    {
      string str = identity.FindFirst("appid").Value + "@" + identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
      foreach (Claim claim in identity.FindAll("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))
        identity.RemoveClaim(claim);
      identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", str));
      return str;
    }

    protected abstract bool ValidateServicePrincipal(string name, out Guid servicePrincipal);

    protected override bool SupportNestedToken => true;
  }
}
