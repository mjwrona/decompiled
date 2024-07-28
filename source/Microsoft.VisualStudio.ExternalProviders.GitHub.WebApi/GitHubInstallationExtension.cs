// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GitHubInstallationExtension
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  public static class GitHubInstallationExtension
  {
    public const string c_claim_installationId = "installationId";
    public const int c_maxTokenLength = 2024;
    public const string c_issuer = "vsts";
    public const string c_audience = "vsts";

    public static bool IsValid(this GitHubData.InstallationAccessToken token) => !string.IsNullOrEmpty(token.Token) && DateTime.UtcNow.AddMinutes(2.0) < token.Expires_at;

    public static bool IsValidForMinutes(
      this GitHubData.InstallationAccessToken token,
      int isTokenValidForXMinutes)
    {
      return !string.IsNullOrEmpty(token.Token) && DateTime.UtcNow.AddMinutes((double) isTokenValidForXMinutes) <= token.Expires_at;
    }

    public static string ToJwtToken(this string installationId, byte[] secret) => installationId.ToJwtToken(nameof (installationId), secret);

    public static string ToJwtToken(
      this string content,
      string claimType,
      byte[] secret,
      string issuer = null)
    {
      ArgumentUtility.CheckForNull<string>(content, nameof (content));
      ArgumentUtility.CheckForNull<byte[]>(secret, nameof (secret));
      Claim[] additionalClaims = new Claim[1]
      {
        new Claim(claimType, content)
      };
      DateTime dateTime = new DateTime(1, 1, 1, 1, 1, 1, DateTimeKind.Utc);
      string encodedToken = JsonWebToken.Create(issuer ?? "vsts", "vsts", dateTime, dateTime, (IEnumerable<Claim>) additionalClaims, VssSigningCredentials.Create(secret)).EncodedToken;
      return encodedToken.Length <= 2024 ? encodedToken : throw new InvalidTokenException("The token length exceeded maximum length.");
    }
  }
}
