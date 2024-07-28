// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RuntimeKeyVaultProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class RuntimeKeyVaultProvider : SecretProvider
  {
    public RuntimeKeyVaultProvider(
      IVssRequestContext requestContext,
      ITFLogger logger,
      AzureServicePrincipalProvider servicePrincipalProvider,
      bool readOnly = true)
      : this(requestContext.ExecutionEnvironment.IsDevFabricDeployment, logger, servicePrincipalProvider, readOnly)
    {
    }

    public RuntimeKeyVaultProvider(
      bool isDevFabric,
      ITFLogger logger,
      AzureServicePrincipalProvider servicePrincipalProvider,
      bool readOnly = true)
      : base(SecretProviderFactory.CreateStorageProvider(isDevFabric, (KeyVaultSecretStorageProvider) new RuntimeKeyVaultSecretStorageProvider(servicePrincipalProvider, logger, readOnly), logger), logger, readOnly)
    {
    }
  }
}
