// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ISecureTokenService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (SecureTokenService))]
  public interface ISecureTokenService : IVssFrameworkService
  {
    JwtSecurityToken IssueToken(
      IVssRequestContext requestContext,
      string audience,
      string issuer,
      IEnumerable<Claim> claims,
      string keyNamespace,
      TimeSpan tokenLifetime,
      DateTimeOffset? validFrom = null);

    SecureTokenValidationResult ValidateToken(
      IVssRequestContext requestContext,
      string jwtString,
      TokenValidationParameters validationParameters);
  }
}
