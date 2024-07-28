// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens.AadOnBehalfOfHandler
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Aad;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens
{
  internal class AadOnBehalfOfHandler : IAadOnBehalfOfHandler
  {
    private const string s_OnBehalfOfFlowFeatureFlag = "VisualStudio.Services.AadTokenService.OnBehalfOf";
    private const string s_Area = "AzureActiveDirectory";
    private const string s_Layer = "AadOnBehalfOfHandler";

    public void TryUpdateRefreshTokenOnBehalfOfUser(
      IVssRequestContext requestContext,
      string accessToken,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      ArgumentUtility.CheckStringForNullOrEmpty(accessToken, nameof (accessToken));
      requestContext.TraceEnter(9002200, "AzureActiveDirectory", nameof (AadOnBehalfOfHandler), nameof (TryUpdateRefreshTokenOnBehalfOfUser));
      try
      {
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.AadTokenService.OnBehalfOf"))
          return;
        try
        {
          JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(accessToken);
          Claim appIdClaim = jwtSecurityToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "appid"));
          if (appIdClaim == null)
            requestContext.Trace(9002201, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadOnBehalfOfHandler), "No AppId claim is present, skipping OnBehalfOf flow");
          else if (!requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetService<IOAuth2SettingsService>().GetAADAuthSettings(requestContext).OnBehalfOfAllowedAADAppIds.Any<string>((Func<string, bool>) (x => x.Equals(appIdClaim.Value, StringComparison.OrdinalIgnoreCase))))
          {
            requestContext.Trace(9002201, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadOnBehalfOfHandler), "AppId: {0}, is not allowed, skipping OnBehalfOf flow", (object) appIdClaim.Value);
          }
          else
          {
            string tenantId = jwtSecurityToken.Claims.First<Claim>((Func<Claim, bool>) (x => x.Type == "tid")).Value;
            IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
            IAadTokenService service = vssRequestContext.GetService<IAadTokenService>();
            string defaultResource = service.DefaultResource;
            service.TryUpdateRefreshTokenOnBehalfOfUser(vssRequestContext, accessToken, defaultResource, tenantId, identity);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(9002202, "AzureActiveDirectory", nameof (AadOnBehalfOfHandler), ex);
        }
      }
      finally
      {
        requestContext.TraceLeave(9002209, "AzureActiveDirectory", nameof (AadOnBehalfOfHandler), nameof (TryUpdateRefreshTokenOnBehalfOfUser));
      }
    }

    internal string ExtractTokenAudience(JwtSecurityToken token, string defaultAudience)
    {
      string tokenAudience = defaultAudience;
      if (token != null)
      {
        IEnumerable<string> audiences = token.Audiences;
        bool? nullable = audiences != null ? new bool?(audiences.Any<string>()) : new bool?();
        bool flag = true;
        if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
        {
          foreach (string audience in token.Audiences)
          {
            if (!string.IsNullOrEmpty(audience))
            {
              if (string.Equals(audience, defaultAudience, StringComparison.OrdinalIgnoreCase))
                return audience;
              tokenAudience = audience;
            }
          }
        }
      }
      return tokenAudience;
    }
  }
}
