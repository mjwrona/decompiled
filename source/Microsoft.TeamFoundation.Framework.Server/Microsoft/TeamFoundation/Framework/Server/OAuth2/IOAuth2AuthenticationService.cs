// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.IOAuth2AuthenticationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  [DefaultServiceImplementation(typeof (OAuth2AuthenticationService))]
  public interface IOAuth2AuthenticationService : IVssFrameworkService
  {
    ClaimsPrincipal ValidateToken(
      IVssRequestContext requestContext,
      string token,
      OAuth2TokenValidators allowedValidators,
      out JwtSecurityToken jwtToken,
      out bool impersonating,
      out bool validIdentity,
      out OAuth2TokenValidators selectedValidator);

    IEnumerable<IdentityDescriptor> ProcessScopes(
      IVssRequestContext requestContext,
      ClaimsPrincipal claimsPrincipal);
  }
}
