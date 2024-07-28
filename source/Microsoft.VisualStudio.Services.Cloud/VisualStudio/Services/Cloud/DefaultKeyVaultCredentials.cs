// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DefaultKeyVaultCredentials
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.KeyVault;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Cloud.Authentication;
using Microsoft.VisualStudio.Services.CloudConfiguration;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class DefaultKeyVaultCredentials : BaseServiceClientCredentials<KeyVaultCredential>
  {
    public DefaultKeyVaultCredentials(bool useManagedIdentity = false, ITFLogger logger = null)
      : base(useManagedIdentity, logger)
    {
    }

    protected override KeyVaultCredential CreateDeploymentAgentCredentials()
    {
      AzureTokenProvider tokenProvider = new AzureTokenProvider("https://login.microsoftonline.com", "Common", "https://management.core.windows.net/", "872cd9fa-d31f-45e0-9eab-6e460a02d1f1", "urn:ietf:wg:oauth:2.0:oob", false, this.Logger);
      return new KeyVaultCredential((KeyVaultClient.AuthenticationCallback) ((authority, resource, scope) => tokenProvider.GetTokenAsync(authority, resource, scope)));
    }

    protected override KeyVaultCredential CreateRuntimeServicePrincipalCredentials() => new KeyVaultCredential(new AzureServicePrincipalProvider(this.Logger).GetKeyVaultAuthCallback());

    protected override KeyVaultCredential CreateManagedIdentityCredentials() => new KeyVaultCredential((KeyVaultClient.AuthenticationCallback) ((authority, resource, scope) => new ManagedIdentityTokenProvider(resource, this.Logger).GetTokenAsync()));
  }
}
