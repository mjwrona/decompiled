// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecureToken.Actions.ISecureTokenIssuer
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microsoft.TeamFoundation.Framework.Server.SecureToken.Actions
{
  public interface ISecureTokenIssuer
  {
    JwtSecurityToken IssueToken(
      TokenSigningKey signingKey,
      string audience,
      string issuer,
      IEnumerable<Claim> claims,
      TimeSpan tokenLifetime,
      DateTimeOffset? validFrom);
  }
}
