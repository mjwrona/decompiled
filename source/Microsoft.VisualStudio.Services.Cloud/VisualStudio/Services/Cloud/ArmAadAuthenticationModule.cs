// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ArmAadAuthenticationModule
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ArmAadAuthenticationModule : VssfAuthenticationHttpModuleBase
  {
    private const string s_Area = "ArmAadAuthenticationModule";
    private const string s_Layer = "Module";
    private const string ArmAtAdoServicePrincipalId = "00000007-0000-8888-8000-000000000000@2c895908-04e0-4952-89fd-54b0046d6288";
    private const string AuthorizationHeader = "Authorization";

    public override void OnAuthenticateRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      requestContext.TraceEnter(90003100, nameof (ArmAadAuthenticationModule), "Module", "ArmAadAuthenticationModule.OnAuthenticateRequest");
      try
      {
        string str = httpContext.Request.Headers.Get("Authorization");
        if ((str != null ? (str.StartsWith("pop", StringComparison.OrdinalIgnoreCase) ? 1 : 0) : 0) == 0)
          return;
        this.TraceToken(requestContext, str);
        if (this.IsPopTokenValid(requestContext, str))
        {
          ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal((IIdentity) new ClaimsIdentity((IEnumerable<Claim>) new Claim[1]
          {
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "00000007-0000-8888-8000-000000000000@2c895908-04e0-4952-89fd-54b0046d6288")
          }, "X509"));
          httpContext.User = (IPrincipal) claimsPrincipal;
          AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.AAD_PoP);
          requestContext.TraceAlways(90003104, TraceLevel.Info, nameof (ArmAadAuthenticationModule), "Module", "PoP validated, assigned identity.");
        }
        else
          requestContext.TraceAlways(90003103, TraceLevel.Info, nameof (ArmAadAuthenticationModule), "Module", "Invalid PoP token.");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(90003101, nameof (ArmAadAuthenticationModule), "Module", ex);
      }
    }

    internal bool IsPopTokenValid(IVssRequestContext requestContext, string popToken) => requestContext.To(TeamFoundationHostType.Deployment).GetService<IArmAadValidatorService>().ApiProtectedWithProofOfPossession(requestContext.HttpMethod(), requestContext.RequestUri(), popToken, requestContext.ActivityId, requestContext.CancellationToken);

    private void TraceToken(IVssRequestContext requestContext, string authHeader)
    {
      try
      {
        Claim claim = new JwtSecurityToken(authHeader.Substring("PoP ".Length).Trim()).Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "at"));
        if (claim != null)
        {
          JwtSecurityToken token = new JwtSecurityToken(claim.Value);
          JwtTracer.Instance.Trace(requestContext, token);
        }
        else
          requestContext.TraceAlways(5511101, TraceLevel.Info, nameof (ArmAadAuthenticationModule), "Module", "at claim not found in the PoP token.");
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(90003105, TraceLevel.Error, nameof (ArmAadAuthenticationModule), "Module", ex.ToString());
      }
    }
  }
}
