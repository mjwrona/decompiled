// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.ServicePrincipalS2SAuthTokenValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public class ServicePrincipalS2SAuthTokenValidator : S2SAuthTokenValidator
  {
    private static readonly Guid s_mmsProvisionerAuthServicePrincipalGuid = Guid.Parse("00000048-0000-8888-8000-000000000000");

    public ServicePrincipalS2SAuthTokenValidator()
    {
    }

    public ServicePrincipalS2SAuthTokenValidator(IJwtTracer jwtTracer)
      : base(jwtTracer)
    {
    }

    protected override AuthenticationMechanism AuthenticationMechanism => AuthenticationMechanism.S2S_ServicePrincipal;

    public override void Initialize(
      IVssRequestContext requestContext,
      IOAuth2SettingsService settings)
    {
      IS2SAuthSettings s2SauthSettings = settings.GetS2SAuthSettings(requestContext);
      this._enabled = s2SauthSettings.Enabled;
      if (this._enabled)
      {
        string issuer = s2SauthSettings.Issuer;
        List<string> stringList = new List<string>();
        if (!string.IsNullOrEmpty(issuer))
          stringList.Add(issuer);
        this._validIssuers = (IEnumerable<string>) stringList;
        this._tenantDomain = s2SauthSettings.TenantDomain;
      }
      else
        this._validIssuers = (IEnumerable<string>) Array.Empty<string>();
    }

    protected override bool ValidateServicePrincipal(string name, out Guid servicePrincipal)
    {
      servicePrincipal = Guid.Empty;
      Guid result;
      if (!Guid.TryParse(name, out result) || !ServicePrincipals.IsInternalServicePrincipalId(result))
        return false;
      servicePrincipal = result;
      return true;
    }

    internal override void TransformIdentityClaims(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity,
      out bool impersonating)
    {
      base.TransformIdentityClaims(requestContext, token, identity, out impersonating);
      ClaimsIdentity claimsIdentity = identity;
      if (token.Actor != null)
        claimsIdentity = identity.Actor;
      Guid servicePrincipal = Guid.Empty;
      this.ValidateServicePrincipal(claimsIdentity.FindFirst("appid").Value, out servicePrincipal);
      if (!(servicePrincipal != ServicePrincipalS2SAuthTokenValidator.s_mmsProvisionerAuthServicePrincipalGuid) || this.IsSystemServicePrincipal(requestContext, servicePrincipal))
        return;
      claimsIdentity.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Service"));
    }

    private bool IsSystemServicePrincipal(IVssRequestContext requestContext, Guid servicePrincipal)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IVssSecuritySubjectService>().GetSecuritySubjectEntry(vssRequestContext, servicePrincipal) != null;
    }
  }
}
