// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.IOAuth2TokenValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  [InheritedExport]
  public interface IOAuth2TokenValidator
  {
    string NameClaimType { get; }

    string RoleClaimType { get; }

    IEnumerable<string> ValidIssuers { get; }

    OAuth2TokenValidators ValidatorType { get; }

    void Initialize(IVssRequestContext requestContext, IOAuth2SettingsService settings);

    bool CanValidateToken(IVssRequestContext requestContext, JwtSecurityToken token);

    void ValidateToken(
      IVssRequestContext requestContext,
      JwtSecurityToken token,
      ClaimsPrincipal principal,
      out bool impersonating,
      out bool validIdentity);

    bool ValidateAudience(
      IVssRequestContext requestContext,
      IEnumerable<string> audiences,
      Microsoft.IdentityModel.Tokens.SecurityToken securityToken,
      TokenValidationParameters validationParameters);

    IEnumerable<IdentityDescriptor> ProcessScopes(
      IVssRequestContext requestContext,
      ClaimsPrincipal claimsPrincipal);
  }
}
