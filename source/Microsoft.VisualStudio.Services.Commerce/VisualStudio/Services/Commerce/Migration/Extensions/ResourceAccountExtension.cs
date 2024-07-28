// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Migration.Extensions.ResourceAccountExtension
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts;

namespace Microsoft.VisualStudio.Services.Commerce.Migration.Extensions
{
  internal static class ResourceAccountExtension
  {
    public static Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount ToMigrationResourceAccount(
      this Microsoft.VisualStudio.Services.Commerce.AzureResourceAccount resourceAccount)
    {
      Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount migrationResourceAccount = (Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount) null;
      if (resourceAccount != null)
        migrationResourceAccount = new Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount()
        {
          AccountId = resourceAccount.AccountId,
          AlternateCloudServiceName = resourceAccount.AlternateCloudServiceName,
          AzureCloudServiceName = resourceAccount.AzureCloudServiceName,
          AzureGeoRegion = resourceAccount.AzureGeoRegion,
          AzureResourceName = resourceAccount.AzureResourceName,
          AzureSubscriptionId = resourceAccount.AzureSubscriptionId,
          CollectionId = resourceAccount.CollectionId,
          Created = resourceAccount.Created,
          ETag = resourceAccount.ETag,
          LastUpdated = resourceAccount.LastUpdated,
          OperationResult = (OperationResult) resourceAccount.OperationResult,
          ProviderNamespaceId = (Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AccountProviderNamespace) resourceAccount.ProviderNamespaceId
        };
      return migrationResourceAccount;
    }
  }
}
