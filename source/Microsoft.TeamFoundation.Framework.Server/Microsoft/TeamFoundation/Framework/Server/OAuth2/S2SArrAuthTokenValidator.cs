// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.S2SArrAuthTokenValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public class S2SArrAuthTokenValidator : S2SAuthTokenValidator
  {
    private static readonly string s_metaArrSPAudience = "00000073-0000-8888-8000-000000000000/visualstudio.com";

    public override OAuth2TokenValidators ValidatorType => OAuth2TokenValidators.S2S_ARR;

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

    public override bool ValidateAudience(
      IVssRequestContext requestContext,
      IEnumerable<string> audiences,
      SecurityToken securityToken,
      TokenValidationParameters validationParameters)
    {
      return audiences != null && audiences.Count<string>() == 1 && StringComparer.OrdinalIgnoreCase.Equals(audiences.FirstOrDefault<string>(), S2SArrAuthTokenValidator.s_metaArrSPAudience);
    }
  }
}
