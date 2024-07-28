// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ProvDataClaimHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class ProvDataClaimHelper
  {
    private const string Area = "Authentication";
    private const string Layer = "ProvDataClaimHelper";

    public static bool HasGitHubClaim(ProviderDataClaim[] providerDataClaims) => providerDataClaims != null && providerDataClaims.Length != 0 && ((IEnumerable<ProviderDataClaim>) providerDataClaims).Where<ProviderDataClaim>((Func<ProviderDataClaim, bool>) (providerDataClaim => providerDataClaim.ProviderName == "github.com")).Any<ProviderDataClaim>();

    public static bool HasGitHubClaim(ProviderDataClaim providerDataClaim) => providerDataClaim.ProviderName == "github.com";

    public static bool HasGitHubClaim(
      IVssRequestContext context,
      ClaimsIdentity claimsIdentityFromToken)
    {
      if (claimsIdentityFromToken == null)
        return false;
      Claim[] provDataClaims = ProvDataClaimHelper.GetProvDataClaims(context, claimsIdentityFromToken);
      return provDataClaims != null && provDataClaims.Length != 0 && ProvDataClaimHelper.HasGitHubClaim(((IEnumerable<Claim>) provDataClaims).Select<Claim, ProviderDataClaim>((Func<Claim, ProviderDataClaim>) (claim => ProvDataClaimHelper.ToProviderDataStub(context, claim))).Where<ProviderDataClaim>((Func<ProviderDataClaim, bool>) (stub => stub != null)).ToArray<ProviderDataClaim>());
    }

    public static Claim[] GetProvDataClaims(
      IVssRequestContext context,
      ClaimsIdentity claimsIdentityFromToken)
    {
      if (claimsIdentityFromToken == null)
        return (Claim[]) null;
      Lazy<object[]> lazilyTraceableIdTokenClaims = new Lazy<object[]>((Func<object[]>) (() => claimsIdentityFromToken.Claims.Traceable()));
      context.TraceDataConditionally(61521302, TraceLevel.Verbose, "Authentication", nameof (ProvDataClaimHelper), "Input", (Func<object>) (() => (object) new
      {
        idTokenClaims = lazilyTraceableIdTokenClaims.Value
      }), nameof (GetProvDataClaims));
      Claim[] array = claimsIdentityFromToken.Claims.Where<Claim>((Func<Claim, bool>) (claim => claim.Type == "prov_data")).ToArray<Claim>();
      if (array.Length != 0)
        return array;
      context.TraceDataConditionally(61521303, TraceLevel.Verbose, "Authentication", nameof (ProvDataClaimHelper), "No op since there are no provider data claims", (Func<object>) (() => (object) new
      {
        idTokenClaims = lazilyTraceableIdTokenClaims.Value
      }), nameof (GetProvDataClaims));
      return (Claim[]) null;
    }

    public static ProviderDataClaim ToProviderDataStub(
      IVssRequestContext context,
      Claim providerDataClaim)
    {
      try
      {
        ProviderDataClaim providerDataStub = JsonUtilities.Deserialize<ProviderDataClaim>(providerDataClaim.Value);
        context.TraceDataConditionally(61521321, TraceLevel.Verbose, "Authentication", nameof (ProvDataClaimHelper), "Deserialized provider data claim as stub", (Func<object>) (() => (object) new
        {
          providerDataClaim = providerDataClaim.Traceable(),
          providerDataStub = providerDataStub
        }), nameof (ToProviderDataStub));
        return providerDataStub;
      }
      catch (Exception ex)
      {
        context.TraceException(61521329, "Authentication", nameof (ProvDataClaimHelper), ex);
        return (ProviderDataClaim) null;
      }
    }
  }
}
