// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecureToken.Actions.SymmetricKeySignedJwtTokenValidator
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
  internal class SymmetricKeySignedJwtTokenValidator : ISecureTokenValidator
  {
    protected ISecurityKeyIdentifierParser keyOperator = (ISecurityKeyIdentifierParser) new ColonSeperatedSecurityKeyIdentifierParser();

    public SecureTokenValidationResult ValidateToken(
      string jwtString,
      TokenSigningKey signingKey,
      TokenValidationParameters validationParameters)
    {
      SecurityToken validatedToken = (SecurityToken) null;
      try
      {
        this.keyOperator.Validate(signingKey.SigningKeyNamespace, signingKey.KeyId);
        validationParameters = validationParameters ?? new TokenValidationParameters();
        validationParameters.ValidateIssuerSigningKey = true;
        validationParameters.IssuerSigningKeyResolver = (IssuerSigningKeyResolver) ((token, securityToken, kid, validationArgs) => (IEnumerable<SecurityKey>) new SecurityKey[1]
        {
          (SecurityKey) new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey.KeyData))
        });
        ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(jwtString, validationParameters, out validatedToken);
        return new SecureTokenValidationResult()
        {
          ClaimsPrincipal = claimsPrincipal,
          FailureReason = SecureTokenValidationFailureReason.None,
          validatedJwt = (JwtSecurityToken) validatedToken
        };
      }
      catch (Exception ex)
      {
        return new SecureTokenValidationResult()
        {
          FailureReason = SecureTokenValidationFailureReason.Unknown,
          Failure = ex
        };
      }
    }

    public void ExtractKeyInformation(
      string jwtString,
      out string signingKeyNamespace,
      out int keyId)
    {
      try
      {
        this.keyOperator.Decode(JwtHeader.Base64UrlDeserialize(new JwtSecurityToken(jwtString).EncodedHeader).Kid, out signingKeyNamespace, out keyId);
      }
      catch (Exception ex)
      {
        if (!(ex is SecureTokenServiceException))
          throw new InvalidTokenException("Couldn't extract a valid key from the JWT", ex);
        throw;
      }
    }
  }
}
