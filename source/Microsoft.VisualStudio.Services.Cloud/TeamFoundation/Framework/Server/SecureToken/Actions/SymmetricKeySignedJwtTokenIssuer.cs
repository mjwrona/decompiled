// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecureToken.Actions.SymmetricKeySignedJwtTokenIssuer
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server.SecureToken.Actions
{
  internal class SymmetricKeySignedJwtTokenIssuer : ISecureTokenIssuer
  {
    protected ISecurityKeyIdentifierParser keyOperator = (ISecurityKeyIdentifierParser) new ColonSeperatedSecurityKeyIdentifierParser();
    protected string SignatureAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
    protected string DigestAlgorithm = "http://www.w3.org/2001/04/xmlenc#sha512";
    protected const string HmacSha512Signature = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";

    public JwtSecurityToken IssueToken(
      TokenSigningKey signingKey,
      string audience,
      string issuer,
      IEnumerable<Claim> claims,
      TimeSpan tokenLifetime,
      DateTimeOffset? validFrom)
    {
      DateTimeOffset dateTimeOffset = validFrom ?? DateTimeOffset.UtcNow;
      dateTimeOffset = dateTimeOffset.Add(tokenLifetime);
      DateTime utcDateTime = dateTimeOffset.UtcDateTime;
      this.ValidateSigningKey(signingKey);
      string str = this.keyOperator.Encode(signingKey.SigningKeyNamespace, signingKey.KeyId);
      SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey.KeyData));
      key.KeyId = str;
      SigningCredentials signingCredentials = new SigningCredentials((SecurityKey) key, this.SignatureAlgorithm);
      JwtSecurityTokenHandler securityTokenHandler = new JwtSecurityTokenHandler();
      SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor();
      tokenDescriptor.Issuer = issuer;
      tokenDescriptor.Audience = audience;
      tokenDescriptor.Subject = new ClaimsIdentity(claims);
      tokenDescriptor.Expires = new DateTime?(utcDateTime);
      tokenDescriptor.SigningCredentials = signingCredentials;
      dateTimeOffset = validFrom.GetValueOrDefault(DateTimeOffset.UtcNow);
      tokenDescriptor.NotBefore = new DateTime?(dateTimeOffset.UtcDateTime);
      return (JwtSecurityToken) securityTokenHandler.CreateToken(tokenDescriptor);
    }

    protected void ValidateSigningKey(TokenSigningKey signingKey)
    {
      this.keyOperator.Validate(signingKey.SigningKeyNamespace, signingKey.KeyId);
      if (string.IsNullOrWhiteSpace(signingKey?.KeyData))
        throw new DependencyFailureException("Couldn't retrieve valid key");
    }
  }
}
