// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RuntimeKeyVaultSecretStorageProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class RuntimeKeyVaultSecretStorageProvider : KeyVaultSecretStorageProvider
  {
    private readonly AzureServicePrincipalProvider m_servicePrincipalProvider;

    public RuntimeKeyVaultSecretStorageProvider(
      AzureServicePrincipalProvider servicePrincipalProvider,
      ITFLogger logger,
      bool readOnly = true)
      : base(KeyVaultClientAdapterFactory.GetKeyVaultClientAdapter(servicePrincipalProvider.GetKeyVaultAuthCallback(), readOnly, logger), logger)
    {
      this.m_servicePrincipalProvider = servicePrincipalProvider;
    }

    protected virtual Exception CreateGetSecretException(
      string secretName,
      string vaultUrl,
      Exception ex)
    {
      return (Exception) new SecretProviderException(string.Format("Error retrieving secret {0} from key vault {1} using runtime service principal credentials. \r\nSee https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki?pageId=1636&anchor=vault-reads-using-runtime-service-principal-credentials for tips on diagnosing this error.\r\n\r\nError: {2}\r\n\r\nService Principal ClientId/AppId:  {3}\r\nService Principal TenantId:        {4}\r\nService Principal Cert Thumbprint: {5}\r\nService Principal Cert Lifetime:   {6} - {7}\r\n", (object) secretName, (object) vaultUrl, (object) ex.Message, (object) this.m_servicePrincipalProvider.RuntimeServicePrincipalClientId, (object) this.m_servicePrincipalProvider.RuntimeServicePrincipalApplicationTenantId, (object) this.m_servicePrincipalProvider.RuntimeServicePrincipalCertificateThumbprint, (object) this.m_servicePrincipalProvider.RuntimeServicePrincipalCertificate.NotBefore.ToUniversalTime(), (object) this.m_servicePrincipalProvider.RuntimeServicePrincipalCertificate.NotAfter.ToUniversalTime()), ex);
    }
  }
}
