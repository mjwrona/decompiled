// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.JwtTracer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public class JwtTracer : IJwtTracer
  {
    private static readonly IReadOnlyCollection<string> BodyClaims = (IReadOnlyCollection<string>) new string[8]
    {
      "uti",
      "iat",
      "iss",
      "exp",
      "appid",
      "azp",
      "aud",
      "nbf"
    };
    private static readonly IReadOnlyCollection<string> BodyClaimsEUPI = (IReadOnlyCollection<string>) new string[3]
    {
      "puid",
      "oid",
      "sid"
    };
    private StringBuilder _sbPool;
    private const string TraceDisabledFeatureFlag = "VisualStudio.Services.Authentication.TraceJwt.Disable";
    private const string TraceEUPIFeatureFlag = "VisualStudio.Services.Authentication.TraceJwt.EUPI";
    private const char ClaimsSeparator = ' ';
    private const string TraceArea = "Authentication";
    private const string TraceLayer = "JWTTracer";

    public static IJwtTracer Instance { get; } = (IJwtTracer) new JwtTracer();

    public void Trace(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      if (requestContext == null || token == null || requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.TraceJwt.Disable"))
        return;
      StringBuilder sb = Interlocked.Exchange<StringBuilder>(ref this._sbPool, (StringBuilder) null) ?? new StringBuilder();
      sb.Append("JWT Claims:");
      object claimValue;
      if (token.Header.TryGetValue("kid", out claimValue))
        JwtTracer.AppendClaim(sb, "kid", claimValue);
      JwtTracer.AppendClaimsFromList(sb, JwtTracer.BodyClaims, token);
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.TraceJwt.EUPI"))
        JwtTracer.AppendClaimsFromList(sb, JwtTracer.BodyClaimsEUPI, token);
      requestContext.TraceAlways(5511100, TraceLevel.Info, "Authentication", "JWTTracer", sb.ToString());
      this._sbPool = sb.Clear();
    }

    private static void AppendClaimsFromList(
      StringBuilder sb,
      IReadOnlyCollection<string> claimsToTrace,
      JwtSecurityToken token)
    {
      JwtPayload payload = token.Payload;
      foreach (string str in (IEnumerable<string>) claimsToTrace)
      {
        object claimValue;
        if (payload.TryGetValue(str, out claimValue))
          JwtTracer.AppendClaim(sb, str, claimValue);
      }
    }

    private static void AppendClaim(StringBuilder sb, string claimName, object claimValue)
    {
      sb.Append(' ');
      sb.Append(claimName);
      sb.Append('=');
      sb.Append(claimValue);
    }
  }
}
