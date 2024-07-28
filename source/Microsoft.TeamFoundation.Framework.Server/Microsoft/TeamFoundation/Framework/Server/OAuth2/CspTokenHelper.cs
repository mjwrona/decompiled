// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.CspTokenHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  internal static class CspTokenHelper
  {
    private const string TraceLayer = "CspTokenHelper";
    private const string TraceArea = "Authorization";
    private static readonly HashSet<string> cspRelatedClaims = new HashSet<string>()
    {
      "aai",
      "oid",
      "upn",
      "tid",
      "idp",
      "wids",
      "altsecid",
      "home_puid"
    };

    internal static bool TryParseTokenAsCspPartner(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      out IdentityMetaType cspPartnerUserType,
      out string cspPartnerUserPuid,
      out string errorMsg)
    {
      cspPartnerUserPuid = string.Empty;
      errorMsg = string.Empty;
      CspTokenHelper.CspTokenDetails cspToken = CspTokenHelper.ParseCspToken(token);
      if (!CspTokenHelper.TryParseCspPartnerUserType(requestContext, cspToken, out cspPartnerUserType))
      {
        errorMsg = "Token does not contain well-known CSP identifiers in wids claim.";
        return false;
      }
      if (cspToken.ClaimsMap.ContainsKey("oid"))
      {
        errorMsg = "oid claim detected. CSP tokens are not supposed to contain this claim.";
        return false;
      }
      if (cspToken.ClaimsMap.ContainsKey("upn"))
      {
        errorMsg = "upn claim detected. CSP tokens are not supposed to contain this claim.";
        return false;
      }
      if (!CspTokenHelper.ValidateTenantIdsForCspPartnerUser(cspToken, out errorMsg))
        return false;
      if (cspToken.IsGdapToken)
      {
        if (!cspToken.TryGetClaimValue("home_puid", out cspPartnerUserPuid, out errorMsg))
          return false;
      }
      else if (!CspTokenHelper.TryParseAltSecIdForCspPartnerUser(cspToken, out cspPartnerUserPuid, out errorMsg))
        return false;
      CspTokenHelper.TraceGdapDapTokenType(requestContext, cspToken);
      requestContext.TraceAlways(5511000, TraceLevel.Info, "Authorization", nameof (CspTokenHelper), string.Format("Token contains CSP claim identifying the user as {0}. wids:[{1}]", (object) cspPartnerUserType, (object) string.Join(", ", (IEnumerable<string>) cspToken.WidsClaimValues)));
      return true;
    }

    private static CspTokenHelper.CspTokenDetails ParseCspToken(JwtSecurityToken token)
    {
      CspTokenHelper.CspTokenDetails cspToken = new CspTokenHelper.CspTokenDetails();
      foreach (Claim claim in token.Claims.Where<Claim>((Func<Claim, bool>) (x => x != null)).Join<Claim, string, string, Claim>((IEnumerable<string>) CspTokenHelper.cspRelatedClaims, (Func<Claim, string>) (t => t.Type), (Func<string, string>) (r => r), (Func<Claim, string, Claim>) ((t, r) => t), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(claim.Type, "aai"))
          cspToken.IsGdapToken = true;
        else if (StringComparer.OrdinalIgnoreCase.Equals(claim.Type, "wids"))
        {
          cspToken.WidsClaimValues.Add(claim.Value);
        }
        else
        {
          if (cspToken.ClaimsMap.ContainsKey(claim.Type))
            throw new SecurityTokenValidationException("Token contains duplicated claim " + claim.Type + ", which is unexpected.");
          cspToken.ClaimsMap.Add(claim.Type, claim.Value);
        }
      }
      return cspToken;
    }

    private static bool TryParseCspPartnerUserType(
      IVssRequestContext requestContext,
      CspTokenHelper.CspTokenDetails cspTokenDetails,
      out IdentityMetaType cspPartnerUserType)
    {
      cspPartnerUserType = IdentityMetaType.Unknown;
      if (cspTokenDetails.WidsClaimValues.Contains("62e90394-69f5-4237-9190-012177145e10"))
      {
        if (cspTokenDetails.IsGdapToken)
        {
          requestContext.TraceAlways(5511003, TraceLevel.Warning, "Authorization", nameof (CspTokenHelper), "GDAP token contains DAP role (62e90394-69f5-4237-9190-012177145e10) in wids claim");
        }
        else
        {
          cspPartnerUserType = IdentityMetaType.CompanyAdministrator;
          return true;
        }
      }
      if (cspTokenDetails.WidsClaimValues.Contains("729827e3-9c14-49f7-bb1b-9608f156bbb8"))
      {
        if (cspTokenDetails.IsGdapToken)
        {
          requestContext.TraceAlways(5511003, TraceLevel.Warning, "Authorization", nameof (CspTokenHelper), "GDAP token contains DAP role (729827e3-9c14-49f7-bb1b-9608f156bbb8) in wids claim");
        }
        else
        {
          cspPartnerUserType = IdentityMetaType.HelpdeskAdministrator;
          return true;
        }
      }
      if (cspTokenDetails.WidsClaimValues.Contains("08372b87-7d02-482a-9e02-fb03ea5fe193"))
      {
        if (!cspTokenDetails.IsGdapToken)
        {
          requestContext.TraceAlways(5511004, TraceLevel.Warning, "Authorization", nameof (CspTokenHelper), "Only CloudServiceProvider wids claim: 08372b87-7d02-482a-9e02-fb03ea5fe193 is present in the DAP token");
        }
        else
        {
          cspPartnerUserType = IdentityMetaType.CompanyAdministrator;
          return true;
        }
      }
      return false;
    }

    private static bool TryParseAltSecIdForCspPartnerUser(
      CspTokenHelper.CspTokenDetails cspTokenDetails,
      out string puid,
      out string errorMsg)
    {
      puid = string.Empty;
      string str;
      if (!cspTokenDetails.TryGetClaimValue("altsecid", out str, out errorMsg))
        return false;
      if (!str.StartsWith("5::", StringComparison.OrdinalIgnoreCase))
      {
        errorMsg = "altsecid claim should start with 5::";
        return false;
      }
      puid = str.Substring("5::".Length);
      if (puid.Length != 0)
        return true;
      errorMsg = "Failed to get a valid PUID from the altsecid claim: " + str + ".";
      return false;
    }

    private static bool ValidateTenantIdsForCspPartnerUser(
      CspTokenHelper.CspTokenDetails cspTokenDetails,
      out string errorMsg)
    {
      string idp;
      string input;
      if (!cspTokenDetails.TryGetClaimValue("idp", out idp, out errorMsg) || !cspTokenDetails.TryGetClaimValue("tid", out input, out errorMsg))
        return false;
      Guid homeTenantId;
      if (!CspTokenHelper.TryParseHomeTenantIdFromIdpClaim(idp, out homeTenantId))
      {
        errorMsg = "Could not parse home tenant from idp claim";
        return false;
      }
      Guid result;
      if (!Guid.TryParse(input, out result))
      {
        errorMsg = "Could not parse resource tenant from tid claim";
        return false;
      }
      if (!(homeTenantId == result))
        return true;
      errorMsg = "The home tenant should be different than the customer tenant for CSP partner users. The idp claim should not contain the Guid value in tid claim, but found idp: " + idp + ", tid: " + input + ".";
      return false;
    }

    private static bool TryParseHomeTenantIdFromIdpClaim(string idp, out Guid homeTenantId)
    {
      homeTenantId = Guid.Empty;
      if (idp.EndsWith("/"))
        idp = idp.Substring(0, idp.Length - 1);
      int num = idp.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
      return num > 0 && Guid.TryParse(idp.Substring(num + 1), out homeTenantId);
    }

    private static void TraceGdapDapTokenType(
      IVssRequestContext requestContext,
      CspTokenHelper.CspTokenDetails cspTokenDetails)
    {
      int tracepoint = cspTokenDetails.IsGdapToken ? 5511002 : 5511001;
      string str = cspTokenDetails.IsGdapToken ? "GDAP" : "DAP";
      requestContext.TraceAlways(tracepoint, TraceLevel.Info, "Authorization", nameof (CspTokenHelper), "Detected CSP " + str + " token");
    }

    private class CspTokenDetails
    {
      public Dictionary<string, string> ClaimsMap { get; set; }

      public HashSet<string> WidsClaimValues { get; set; }

      public bool IsGdapToken { get; set; }

      public CspTokenDetails()
      {
        this.ClaimsMap = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.WidsClaimValues = new HashSet<string>();
      }

      public bool TryGetClaimValue(string key, out string value, out string errorMsg)
      {
        errorMsg = string.Empty;
        if (!this.ClaimsMap.TryGetValue(key, out value))
        {
          errorMsg = key + " claim is missing";
          return false;
        }
        if (!string.IsNullOrWhiteSpace(value))
          return true;
        errorMsg = key + " claim is null or empty";
        return false;
      }
    }
  }
}
