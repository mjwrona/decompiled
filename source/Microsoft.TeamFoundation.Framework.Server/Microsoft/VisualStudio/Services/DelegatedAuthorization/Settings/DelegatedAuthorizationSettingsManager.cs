// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings.DelegatedAuthorizationSettingsManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings
{
  internal class DelegatedAuthorizationSettingsManager : IDelegatedAuthorizationSettingsManager
  {
    private readonly ServiceFactory<IVssRegistryService> registryServiceFactory;
    private const string DelegatedAuthorizationServiceSettingsRoot = "/Service/DelegatedAuthorization";
    private const string AuthorizationGrantLifetime = "/Service/DelegatedAuthorization/AuthorizationGrantLifetime";
    private const string AccessTokenLifetime = "/Service/DelegatedAuthorization/AccessTokenLifetime";
    private const string RefreshTokenLifetime = "/Service/DelegatedAuthorization/RefreshTokenLifetime";
    private const string AccessTokenMaxLifetimeForMediumTrustClients = "/Service/DelegatedAuthorization/AccessTokenMaxLifetimeForMediumTrustClients";
    private const string BearerTokenMaxLifetimeForMediumTrustClients = "/Service/DelegatedAuthorization/BearerTokenMaxLifetimeForMediumTrustClients";
    private const string RefreshTokenMaxLifetimeForMediumTrustClients = "/Service/DelegatedAuthorization/RefreshTokenMaxLifetimeForMediumTrustClients";
    private const string ClientSecretLifetime = "/Service/DelegatedAuthorization/ClientSecretLifetime";
    private const string SessionTokenLifetime = "/Service/DelegatedAuthorization/SessionTokenLifetime";
    private const string SelfDescribingTokenLifetime = "/Service/DelegatedAuthorization/SelfDescribingTokenLifetime";
    private const string SessionTokenImpersonateLifetime = "/Service/DelegatedAuthorization/SessionTokenImpersonateLifetime";
    private const string SessionTokenMaxLifetime = "/Service/DelegatedAuthorization/SessionTokenMaxLifetime";
    internal const string SigningCertificateDrawerName = "/Service/DelegatedAuthorization/SigningCertificateDrawerName";
    internal const string SigningCertificateThumbprint = "/Service/DelegatedAuthorization/SigningCertificateThumbprint";
    internal const string JWTSigningCertificateThumbprint = "JWTSigningCertificateThumbprint";
    internal const string SecondaryJWTSigningCertificateThumbprint = "SecondaryJWTSigningCertificateThumbprint";
    internal const string TokenJWTSigningCertificateThumbprint = "TokenJWTSigningCertificateThumbprint";
    internal const string JWTSigningCertificateThumbprintDrawerName = "/Service/DelegatedAuthorization/JWTSigningCertificateThumbprint";
    internal const string SecondaryJWTSigningCertificateThumbprintDrawerName = "/Service/DelegatedAuthorization/SecondaryJWTSigningCertificateThumbprint";
    private const string AuthorizationScopeDefinitionsJson = "/Service/DelegatedAuthorization/AuthorizationScopeDefinitionsJson";
    private const string AppSessionTokenLifetime = "/Service/DelegatedAuthorization/AppSessionTokenLifetime";
    private const string PATAccessTokenMaxLifetime = "/Service/DelegatedAuthorization/PATAccessTokenMaxLifetime";
    internal const string DelegatedAuthorizationRegistryNotificationFilter = "/Service/DelegatedAuthorization/...";
    private static readonly TimeSpan DefaultAuthorizationGrantLifetime = TimeSpan.FromMinutes(15.0);
    private static readonly TimeSpan DefaultAccessTokenLifetime = TimeSpan.FromHours(1.0);
    private static readonly TimeSpan DefaultRefreshTokenLifetime = DelegatedAuthorizationSettingsManager.TimeSpanFromYears(1);
    private static readonly TimeSpan DefaultAccessTokenMaxLifetimeForMediumTrustClients = TimeSpan.FromHours(1.0);
    private static readonly TimeSpan DefaultBearerTokenMaxLifetimeForMediumTrustClients = TimeSpan.FromMinutes(5.0);
    private static readonly TimeSpan DefaultClientSecretLifetime = DelegatedAuthorizationSettingsManager.TimeSpanFromYears(5);
    private static readonly TimeSpan DefaultSessionTokenLifetime = DelegatedAuthorizationSettingsManager.TimeSpanFromYears(1);
    private static readonly TimeSpan DefaultSelfDescribingTokenLifetime = TimeSpan.FromDays(4.0);
    private static readonly TimeSpan DefaultSessionTokenImpersonateLifetime = TimeSpan.FromDays(1.0);
    private static readonly TimeSpan DefaultSessionTokenMaxLifetime = DelegatedAuthorizationSettingsManager.TimeSpanFromYears(1);
    private static readonly TimeSpan DefaultAppSessionTokenLifetime = TimeSpan.FromHours(1.0);
    private static readonly TimeSpan DefaultPATAccessTokenMaxLifetime = TimeSpan.FromHours(24.0);
    internal const string DefaultSigningCertificateDrawerName = "DelegatedAuthorizationCertificates";
    internal const string DefaultSigningCertificateThumbprint = "A0EBDCCF933FEE9F878C82A5157CFDDEEFD5D19A";

    public DelegatedAuthorizationSettingsManager()
      : this((ServiceFactory<IVssRegistryService>) (x => (IVssRegistryService) x.GetService<CachedRegistryService>()))
    {
    }

    internal DelegatedAuthorizationSettingsManager(
      ServiceFactory<IVssRegistryService> registryServiceFactory)
    {
      this.registryServiceFactory = registryServiceFactory;
    }

    public DelegatedAuthorizationSettings ReadDelegatedAuthorizationSettings(
      IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = this.registryServiceFactory(requestContext).ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/DelegatedAuthorization/*");
      return new DelegatedAuthorizationSettings()
      {
        AuthorizationGrantLifetime = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DelegatedAuthorization/AuthorizationGrantLifetime", DelegatedAuthorizationSettingsManager.DefaultAuthorizationGrantLifetime),
        AccessTokenLifetime = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DelegatedAuthorization/AccessTokenLifetime", DelegatedAuthorizationSettingsManager.DefaultAccessTokenLifetime),
        RefreshTokenLifetime = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DelegatedAuthorization/RefreshTokenLifetime", DelegatedAuthorizationSettingsManager.DefaultRefreshTokenLifetime),
        ClientSecretLifetime = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DelegatedAuthorization/ClientSecretLifetime", DelegatedAuthorizationSettingsManager.DefaultClientSecretLifetime),
        SessionTokenLifetime = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DelegatedAuthorization/SessionTokenLifetime", DelegatedAuthorizationSettingsManager.DefaultSessionTokenLifetime),
        SelfDescribingTokenLifetime = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DelegatedAuthorization/SelfDescribingTokenLifetime", DelegatedAuthorizationSettingsManager.DefaultSelfDescribingTokenLifetime),
        SessionTokenImpersonateLifetime = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DelegatedAuthorization/SessionTokenImpersonateLifetime", DelegatedAuthorizationSettingsManager.DefaultSessionTokenImpersonateLifetime),
        SessionTokenMaxLifetime = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DelegatedAuthorization/SessionTokenMaxLifetime", DelegatedAuthorizationSettingsManager.DefaultSessionTokenMaxLifetime),
        JWTSigningCertificateThumbprintLookupKey = "TokenJWTSigningCertificateThumbprint",
        PrimaryJWTSigningCertificateThumbprintLookupKey = "JWTSigningCertificateThumbprint",
        SecondaryJWTSigningCertificateThumbprintLookUpKey = "SecondaryJWTSigningCertificateThumbprint",
        SigningCertificateDrawerName = registryEntryCollection.GetValueFromPath<string>("/Service/DelegatedAuthorization/SigningCertificateDrawerName", "DelegatedAuthorizationCertificates"),
        SigningCertificateThumbprint = registryEntryCollection.GetValueFromPath<string>("/Service/DelegatedAuthorization/SigningCertificateThumbprint", "A0EBDCCF933FEE9F878C82A5157CFDDEEFD5D19A"),
        AuthorizationScopeDefinitionsJson = registryEntryCollection.GetValueFromPath<string>("/Service/DelegatedAuthorization/AuthorizationScopeDefinitionsJson", (string) null),
        AppSessionTokenLifetime = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DelegatedAuthorization/AppSessionTokenLifetime", DelegatedAuthorizationSettingsManager.DefaultAppSessionTokenLifetime),
        PATAccessTokenMaxLifetime = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DelegatedAuthorization/PATAccessTokenMaxLifetime", DelegatedAuthorizationSettingsManager.DefaultPATAccessTokenMaxLifetime),
        AccessTokenMaxLifetimeForMediumTrustClients = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DelegatedAuthorization/AccessTokenMaxLifetimeForMediumTrustClients", DelegatedAuthorizationSettingsManager.DefaultAccessTokenMaxLifetimeForMediumTrustClients),
        BearerTokenMaxLifetimeForMediumTrustClients = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DelegatedAuthorization/BearerTokenMaxLifetimeForMediumTrustClients", DelegatedAuthorizationSettingsManager.DefaultBearerTokenMaxLifetimeForMediumTrustClients)
      };
    }

    internal static TimeSpan TimeSpanFromYears(int years)
    {
      DateTime utcNow = DateTime.UtcNow;
      return years <= 0 ? utcNow - utcNow.AddYears(years) : utcNow.AddYears(years) - utcNow;
    }
  }
}
