// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.HostAuthenticationCookieRolloutHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  internal static class HostAuthenticationCookieRolloutHelper
  {
    private static readonly int DefaultRolloutPercentage = 0;
    private static readonly RegistryQuery RolloutPercentageQuery = new RegistryQuery("/Configuration/HostAuthenticationCookieRolloutHelper/RolloutPercentage");
    private static readonly string s_area = nameof (HostAuthenticationCookieRolloutHelper);

    internal static bool ShouldUseHostAuthenticationCookie(
      IVssRequestContext requestContext,
      ClaimsPrincipal principal)
    {
      Claim nameIdClaim = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
      int rolloutPercentage = HostAuthenticationCookieRolloutHelper.GetRolloutPercentage(requestContext);
      int absoluteHash = nameIdClaim != null ? Math.Abs(nameIdClaim.Value.GetHashCode()) : 0;
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
      {
        requestContext.TraceDataConditionally(218569, TraceLevel.Info, HostAuthenticationCookieRolloutHelper.s_area, nameof (ShouldUseHostAuthenticationCookie), "Always enabled on DevFabric", methodName: nameof (ShouldUseHostAuthenticationCookie));
        return true;
      }
      if (nameIdClaim != null && absoluteHash % 100 < rolloutPercentage)
      {
        requestContext.TraceDataConditionally(218570, TraceLevel.Info, HostAuthenticationCookieRolloutHelper.s_area, nameof (ShouldUseHostAuthenticationCookie), "Reading HostAuthenticationCookie.", (Func<object>) (() => (object) new
        {
          nameIdClaim = nameIdClaim,
          absoluteHash = absoluteHash,
          rolloutPercentage = rolloutPercentage
        }), nameof (ShouldUseHostAuthenticationCookie));
        return true;
      }
      requestContext.TraceDataConditionally(218570, TraceLevel.Info, HostAuthenticationCookieRolloutHelper.s_area, nameof (ShouldUseHostAuthenticationCookie), "Not reading HostAuthenticationCookie", (Func<object>) (() => (object) new
      {
        nameIdClaim = nameIdClaim,
        absoluteHash = absoluteHash,
        rolloutPercentage = rolloutPercentage
      }), nameof (ShouldUseHostAuthenticationCookie));
      return false;
    }

    private static int GetRolloutPercentage(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, in HostAuthenticationCookieRolloutHelper.RolloutPercentageQuery, HostAuthenticationCookieRolloutHelper.DefaultRolloutPercentage);
    }
  }
}
