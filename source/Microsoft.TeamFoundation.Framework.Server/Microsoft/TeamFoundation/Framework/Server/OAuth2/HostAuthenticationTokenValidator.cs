// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.HostAuthenticationTokenValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal class HostAuthenticationTokenValidator : OAuth2TokenValidator
  {
    private IEnumerable<string> m_trustedIssuers;
    private bool m_enabled;

    public override IEnumerable<string> ValidIssuers => this.m_trustedIssuers;

    public override OAuth2TokenValidators ValidatorType => OAuth2TokenValidators.HostAuthentication;

    public override bool CanValidateToken(IVssRequestContext requestContext, JwtSecurityToken token) => this.m_enabled && token != null && token.Payload != null && token.Payload.ContainsKey("host_auth");

    public override void Initialize(
      IVssRequestContext requestContext,
      IOAuth2SettingsService settings)
    {
      IDelegatedAuthSettings delegatedAuthSettings = settings.GetDelegatedAuthSettings(requestContext);
      this.m_enabled = delegatedAuthSettings.Enabled;
      this.m_trustedIssuers = delegatedAuthSettings.TrustedIssuers;
    }

    internal override bool ValidateIdentity(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsIdentity identity)
    {
      Claim first = identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
      return first != null && first.Value == "HostAuthentication";
    }
  }
}
