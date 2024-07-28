// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.IOAuth2AuthenticationServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public static class IOAuth2AuthenticationServiceExtensions
  {
    public static ClaimsPrincipal ValidateToken(
      this IOAuth2AuthenticationService oauth2AuthenticationService,
      IVssRequestContext requestContext,
      string token,
      OAuth2TokenValidators allowedValidators,
      out JwtSecurityToken jwtToken,
      out bool impersonating,
      out bool validIdentity)
    {
      return oauth2AuthenticationService.ValidateToken(requestContext, token, allowedValidators, out jwtToken, out impersonating, out validIdentity, out OAuth2TokenValidators _);
    }
  }
}
