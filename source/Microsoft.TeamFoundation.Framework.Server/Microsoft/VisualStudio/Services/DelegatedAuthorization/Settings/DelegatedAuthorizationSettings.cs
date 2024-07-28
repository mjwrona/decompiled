// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings.DelegatedAuthorizationSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings
{
  public class DelegatedAuthorizationSettings
  {
    public TimeSpan AuthorizationGrantLifetime { get; set; }

    public TimeSpan AccessTokenLifetime { get; set; }

    public TimeSpan RefreshTokenLifetime { get; set; }

    public TimeSpan ClientSecretLifetime { get; set; }

    public string SigningCertificateDrawerName { get; set; }

    public string SigningCertificateThumbprint { get; set; }

    public string PrimaryJWTSigningCertificateThumbprintLookupKey { get; set; }

    public string SecondaryJWTSigningCertificateThumbprintLookUpKey { get; set; }

    public string JWTSigningCertificateThumbprintLookupKey { get; set; }

    public string AuthorizationScopeDefinitionsJson { get; set; }

    public TimeSpan SessionTokenLifetime { get; set; }

    public TimeSpan SelfDescribingTokenLifetime { get; set; }

    public TimeSpan SessionTokenImpersonateLifetime { get; set; }

    public TimeSpan SessionTokenMaxLifetime { get; set; }

    public TimeSpan AppSessionTokenLifetime { get; set; }

    public TimeSpan AccessTokenMaxLifetimeForMediumTrustClients { get; set; }

    public TimeSpan BearerTokenMaxLifetimeForMediumTrustClients { get; set; }

    public TimeSpan PATAccessTokenMaxLifetime { get; set; }
  }
}
