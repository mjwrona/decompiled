// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Authentication.ConfidentialClientApplicationProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Cloud.Authentication
{
  public class ConfidentialClientApplicationProvider
  {
    private readonly Lazy<IMsiAccessTokenProvider> m_msiAccessTokenProvider;
    private static readonly Lazy<HttpClient> s_httpClient = new Lazy<HttpClient>((Func<HttpClient>) (() => new HttpClient()));

    public ConfidentialClientApplicationProvider() => this.m_msiAccessTokenProvider = new Lazy<IMsiAccessTokenProvider>((Func<IMsiAccessTokenProvider>) (() => (IMsiAccessTokenProvider) new MsiAccessTokenProvider(MsiTokenCache.SharedCache, (IAzureInstanceMetadataProvider) new AzureInstanceMetadataProvider(ConfidentialClientApplicationProvider.s_httpClient.Value))));

    public ConfidentialClientApplicationProvider(IMsiAccessTokenProvider msiAccessTokenProvider) => this.m_msiAccessTokenProvider = msiAccessTokenProvider != null ? new Lazy<IMsiAccessTokenProvider>((Func<IMsiAccessTokenProvider>) (() => msiAccessTokenProvider)) : throw new ArgumentNullException(nameof (msiAccessTokenProvider));

    public IConfidentialClientApplication GetApplication(
      IVssRequestContext requestContext,
      ConfidentialClientApplicationAuthData authData)
    {
      if (authData == null)
        throw new ArgumentNullException(nameof (authData));
      if (string.IsNullOrEmpty(authData.ClientId))
        throw new ArgumentException("ClientId is empty", nameof (authData));
      if ((object) authData.TenantAuthority == null)
        throw new ArgumentException("TenantAuthority is empty", nameof (authData));
      IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      ConfidentialClientApplicationBuilder applicationBuilder;
      switch (authData.CredentialType)
      {
        case ConfidentialClientApplicationCredentialType.StrongBoxCertificate:
          applicationBuilder = this.GetBuilderWithStrongBoxCertificateCredential(deploymentContext, authData.ClientId, authData.TenantAuthority, authData.StrongBoxLookupKey);
          break;
        case ConfidentialClientApplicationCredentialType.StrongBoxSecret:
          applicationBuilder = this.GetBuilderWithStrongBoxSecretCredential(deploymentContext, authData.ClientId, authData.TenantAuthority, authData.StrongBoxLookupKey);
          break;
        case ConfidentialClientApplicationCredentialType.ManagedIdentity:
          applicationBuilder = this.GetBuilderWithManagedIdentityCredential(authData.ClientId, authData.TenantAuthority);
          break;
        default:
          throw new NotSupportedException(string.Format("{0} value not supported: {1}", (object) "ConfidentialClientApplicationCredentialType", (object) authData.CredentialType));
      }
      return applicationBuilder.Build();
    }

    private ConfidentialClientApplicationBuilder GetBuilderWithStrongBoxCertificateCredential(
      IVssRequestContext deploymentContext,
      string clientId,
      Uri tenantAuthority,
      string strongBoxLookupKey)
    {
      if (string.IsNullOrEmpty(strongBoxLookupKey))
        throw new ArgumentException(nameof (strongBoxLookupKey));
      ITeamFoundationStrongBoxService service = deploymentContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(deploymentContext, "ConfigurationSecrets", true);
      return ConfidentialClientApplicationBuilder.Create(clientId).WithAuthority(tenantAuthority).WithCertificate(service.RetrieveFileAsCertificate(deploymentContext, drawerId, strongBoxLookupKey), true);
    }

    private ConfidentialClientApplicationBuilder GetBuilderWithStrongBoxSecretCredential(
      IVssRequestContext deploymentContext,
      string clientId,
      Uri tenantAuthority,
      string strongBoxLookupKey)
    {
      if (string.IsNullOrEmpty(strongBoxLookupKey))
        throw new ArgumentException(nameof (strongBoxLookupKey));
      ITeamFoundationStrongBoxService service = deploymentContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(deploymentContext, "ConfigurationSecrets", true);
      return ConfidentialClientApplicationBuilder.Create(clientId).WithAuthority(tenantAuthority).WithClientSecret(service.GetString(deploymentContext, drawerId, strongBoxLookupKey));
    }

    private ConfidentialClientApplicationBuilder GetBuilderWithManagedIdentityCredential(
      string clientId,
      Uri tenantAuthority)
    {
      string accessToken = this.m_msiAccessTokenProvider.Value.GetAccessToken("api://AzureADTokenExchange");
      return ConfidentialClientApplicationBuilder.Create(clientId).WithAuthority(tenantAuthority).WithClientAssertion(accessToken);
    }
  }
}
