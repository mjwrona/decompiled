// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FederationConfigurationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Selectors;
using System.IdentityModel.Services.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FederationConfigurationService : IVssFrameworkService
  {
    private volatile FederationConfiguration m_federationConfiguration;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), FederatedAuthRegistryConstants.Root + "/...", HighTrustIdentitiesRegistryConstants.Root + "/...");
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      service.RegisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[2]
      {
        ServicingTokenConstants.FedAuthCookieSigningCertificateThumbprint,
        ServicingTokenConstants.SecondaryFedAuthCookieSigningCertificateThumbprint
      });
      service.RegisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged), FederatedAuthRegistryConstants.SigningCertDrawName, (IEnumerable<string>) new string[1]
      {
        FederatedAuthRegistryConstants.ServiceIdentitySigningKeyLookupKey
      });
      Interlocked.CompareExchange<FederationConfiguration>(ref this.m_federationConfiguration, this.GetConfiguration(requestContext), (FederationConfiguration) null);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      requestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged));
    }

    public FederationConfiguration GetFederationConfiguration(IVssRequestContext requestContext) => this.m_federationConfiguration;

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.m_federationConfiguration = this.GetConfiguration(requestContext);
    }

    private void OnStrongBoxChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      this.m_federationConfiguration = this.GetConfiguration(requestContext);
    }

    private FederationConfiguration GetConfiguration(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) (FederatedAuthRegistryConstants.Root + "/**"));
      if (!registryEntryCollection.GetValueFromPath<bool>(FederatedAuthRegistryConstants.Enabled, false))
        return (FederationConfiguration) null;
      FederationConfiguration configuration = new FederationConfiguration(false);
      IdentityConfiguration identityConfiguration = configuration.IdentityConfiguration;
      identityConfiguration.CertificateValidationMode = registryEntryCollection.GetValueFromPath<X509CertificateValidationMode>(FederatedAuthRegistryConstants.CertificateValidationMode, X509CertificateValidationMode.None);
      TeamFoundationTrace.Verbose("IdentityConfiguration: CertificateValidationMode = {0}", (object) identityConfiguration.CertificateValidationMode);
      identityConfiguration.MaxClockSkew = TimeSpan.FromSeconds(registryEntryCollection.GetValueFromPath<double>(FederatedAuthRegistryConstants.MaxClockSkew, identityConfiguration.MaxClockSkew.TotalSeconds));
      TeamFoundationTrace.Verbose("IdentityConfiguration: MaxClockSkew = {0}", (object) identityConfiguration.MaxClockSkew);
      identityConfiguration.TrustedStoreLocation = registryEntryCollection.GetValueFromPath<StoreLocation>(FederatedAuthRegistryConstants.TrustedStoreLocation, identityConfiguration.TrustedStoreLocation);
      TeamFoundationTrace.Verbose("IdentityConfiguration: TrustedStoreLocation = {0}", (object) identityConfiguration.TrustedStoreLocation);
      identityConfiguration.AudienceRestriction.AudienceMode = AudienceUriMode.Never;
      if (identityConfiguration.IssuerNameRegistry is ConfigurationBasedIssuerNameRegistry)
      {
        ConfigurationBasedIssuerNameRegistry issuerNameRegistry = identityConfiguration.IssuerNameRegistry as ConfigurationBasedIssuerNameRegistry;
        foreach (RegistryEntry registryEntry in registryEntryCollection)
        {
          if (registryEntry.Path.StartsWith(FederatedAuthRegistryConstants.TrustedIssuer, StringComparison.OrdinalIgnoreCase) && !issuerNameRegistry.ConfiguredTrustedIssuers.ContainsKey(registryEntry.Name))
          {
            issuerNameRegistry.AddTrustedIssuer(registryEntry.Name, registryEntry.Value);
            TeamFoundationTrace.Verbose("IdentityConfiguration: TrustedIssuer = {0} : {1}", (object) registryEntry.Name, (object) registryEntry.Value);
          }
        }
      }
      else
        TeamFoundationTrace.Verbose("Custom IssuerNameRegistry type configured, ignoring internal settings");
      X509Certificate2 x509Certificate2_1 = (X509Certificate2) null;
      X509Certificate2 x509Certificate2_2 = (X509Certificate2) null;
      TeamFoundationStrongBoxService service = requestContext.GetService<TeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(requestContext, "ConfigurationSecrets", true);
      StrongBoxItemInfo itemInfo1 = service.GetItemInfo(requestContext, drawerId, ServicingTokenConstants.FedAuthCookieSigningCertificateThumbprint, false);
      StrongBoxItemInfo itemInfo2 = service.GetItemInfo(requestContext, drawerId, ServicingTokenConstants.SecondaryFedAuthCookieSigningCertificateThumbprint, false);
      if (itemInfo1 != null)
      {
        x509Certificate2_1 = service.RetrieveFileAsCertificate(requestContext, itemInfo1, expectPrivateKey: true);
        if (itemInfo2 != null)
          x509Certificate2_2 = service.RetrieveFileAsCertificate(requestContext, itemInfo2);
      }
      if (x509Certificate2_1 != null)
      {
        configuration.ServiceCertificate = x509Certificate2_1;
        SessionSecurityTokenHandler handler = new SessionSecurityTokenHandler(new ReadOnlyCollection<CookieTransform>((IList<CookieTransform>) new List<CookieTransform>()
        {
          (CookieTransform) new DeflateCookieTransform(),
          (CookieTransform) new FederationConfigurationService.RsaEncryptionRolloverCookieTransform(configuration.ServiceCertificate, x509Certificate2_2),
          (CookieTransform) new RsaSignatureValidatorRolloverCookieTransform(configuration.ServiceCertificate, x509Certificate2_2, registryEntryCollection.GetValueFromPath<bool>(FederatedAuthRegistryConstants.IssueTokenSignature, true), registryEntryCollection.GetValueFromPath<bool>(FederatedAuthRegistryConstants.TokenSignatureRequired, true))
        }));
        identityConfiguration.SecurityTokenHandlers.AddOrReplace((SecurityTokenHandler) handler);
      }
      configuration.Initialize();
      return configuration;
    }

    private class RsaEncryptionRolloverCookieTransform : RsaEncryptionCookieTransform
    {
      public RsaEncryptionRolloverCookieTransform(
        X509Certificate2 certificate,
        X509Certificate2 secondaryDecryptionKey)
        : base(certificate)
      {
        if (secondaryDecryptionKey != null && !string.Equals(certificate.Thumbprint, secondaryDecryptionKey.Thumbprint, StringComparison.OrdinalIgnoreCase))
          this.DecryptionKeys = new ReadOnlyCollection<RSA>((IList<RSA>) new RSA[2]
          {
            (RSA) certificate.PrivateKey,
            (RSA) secondaryDecryptionKey.PrivateKey
          });
        else
          this.DecryptionKeys = new ReadOnlyCollection<RSA>((IList<RSA>) new RSA[1]
          {
            (RSA) certificate.PrivateKey
          });
      }

      protected override ReadOnlyCollection<RSA> DecryptionKeys { get; }
    }
  }
}
