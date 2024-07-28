// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.FirstPartyS2SAuthTokenValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal class FirstPartyS2SAuthTokenValidator : S2SAuthTokenValidator
  {
    private static readonly IReadOnlyList<string> AllowedAppIdACrValues = (IReadOnlyList<string>) new List<string>()
    {
      "1",
      "2"
    };
    private IList<Guid> _firstPartyPrincipals;
    private static IList<Guid> s_firstPartyPrincipals;
    internal static readonly Guid s_firstPartyIntAadApplicationId = new Guid(OAuth2RegistryConstants.FirstPartyIntAADApplicationId);
    internal static readonly Guid s_firstPartyProdAadApplicationId = new Guid(OAuth2RegistryConstants.FirstPartyProdAADApplicationId);

    protected override AuthenticationMechanism AuthenticationMechanism => AuthenticationMechanism.S2S_FirstParty;

    public override void Initialize(
      IVssRequestContext requestContext,
      IOAuth2SettingsService settings)
    {
      IS2SAuthSettings s2SauthSettings = settings.GetS2SAuthSettings(requestContext);
      this._enabled = s2SauthSettings.Enabled;
      if (this._enabled)
      {
        IEnumerable<string> servicePrincipals = s2SauthSettings.FirstPartyServicePrincipals;
        this._firstPartyPrincipals = (IList<Guid>) new List<Guid>();
        foreach (string input in servicePrincipals)
        {
          Guid result;
          if (!Guid.TryParse(input, out result))
            requestContext.Trace(5510401, TraceLevel.Warning, "Authentication", "AuthTokenValidator", "String " + input + " was provided as a FirstParty Service Principal, but it would not parse as a Guid, therefore skipping.");
          else
            this._firstPartyPrincipals.Add(result);
        }
        string firstPartyIssuer = s2SauthSettings.FirstPartyIssuer;
        List<string> stringList = new List<string>();
        if (!string.IsNullOrEmpty(firstPartyIssuer))
          stringList.Add(firstPartyIssuer);
        this._validIssuers = (IEnumerable<string>) stringList;
        this._tenantDomain = s2SauthSettings.FirstPartyTenantDomain;
        FirstPartyS2SAuthTokenValidator.s_firstPartyPrincipals = this._firstPartyPrincipals;
      }
      else
        this._validIssuers = (IEnumerable<string>) Array.Empty<string>();
    }

    public override bool CanValidateToken(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      if (!base.CanValidateToken(requestContext, token))
        return false;
      bool flag = true;
      Claim claim1 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (claim => claim.Type == "idtyp" && claim.Value != "app"));
      if (claim1 != null)
      {
        requestContext.TraceAlways(55107800, TraceLevel.Warning, "Authentication", "AuthTokenValidator", "'{0}' claim is present and set to unexpected value: {1}", (object) "idtyp", (object) claim1.Value);
        flag = false;
      }
      Claim claim2 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (claim => claim.Type == "scp"));
      if (claim2 != null)
      {
        requestContext.TraceAlways(55107801, TraceLevel.Warning, "Authentication", "AuthTokenValidator", "'{0}' claim is present with value: {1}", (object) "scp", (object) claim2.Value);
        flag = false;
      }
      Claim claim3 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (claim => claim.Type == "unique_name"));
      if (claim3 != null)
      {
        requestContext.TraceAlways(55107802, TraceLevel.Warning, "Authentication", "AuthTokenValidator", "'{0}' claim is present with value: {1}", (object) "unique_name", (object) claim3.Value);
        flag = false;
      }
      Claim claim4 = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (claim => claim.Type == "appidacr"));
      if (claim4 == null)
      {
        requestContext.TraceAlways(55107803, TraceLevel.Warning, "Authentication", "AuthTokenValidator", "'{0}' claim was not found", (object) "appidacr");
        flag = false;
      }
      else if (!FirstPartyS2SAuthTokenValidator.AllowedAppIdACrValues.Contains<string>(claim4.Value))
      {
        requestContext.TraceAlways(55107804, TraceLevel.Warning, "Authentication", "AuthTokenValidator", "'{0}' claim is present and set to unexpected value: {1}", (object) "appidacr", (object) claim4.Value);
        flag = false;
      }
      return flag;
    }

    protected override bool ValidateServicePrincipal(string name, out Guid servicePrincipal)
    {
      servicePrincipal = Guid.Empty;
      Guid result;
      if (!Guid.TryParse(name, out result) || !this._firstPartyPrincipals.Contains(result))
        return false;
      servicePrincipal = result;
      return true;
    }

    internal static bool IsFirstPartyServicePrincipal(Guid servicePrincipalId, Guid tenantId)
    {
      if (tenantId != new Guid("72f988bf-86f1-41af-91ab-2d7cd011db47"))
        return false;
      if (FirstPartyS2SAuthTokenValidator.s_firstPartyPrincipals != null)
        return FirstPartyS2SAuthTokenValidator.s_firstPartyPrincipals.Contains(servicePrincipalId);
      return servicePrincipalId == FirstPartyS2SAuthTokenValidator.s_firstPartyProdAadApplicationId || servicePrincipalId == FirstPartyS2SAuthTokenValidator.s_firstPartyIntAadApplicationId;
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
      claimsIdentity.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Service"));
    }
  }
}
