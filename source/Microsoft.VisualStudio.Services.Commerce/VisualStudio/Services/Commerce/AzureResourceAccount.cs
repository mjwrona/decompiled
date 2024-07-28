// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureResourceAccount
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureResourceAccount : ICloneable
  {
    public Guid AccountId { get; set; }

    public Guid CollectionId { get; set; }

    public Guid AzureSubscriptionId { get; set; }

    public AccountProviderNamespace ProviderNamespaceId { get; set; }

    public string AzureCloudServiceName { get; set; }

    public string AlternateCloudServiceName { get; set; }

    public string AzureGeoRegion { get; set; }

    public string AzureResourceName { get; set; }

    public string ETag { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastUpdated { get; set; }

    public OperationResult OperationResult { get; set; }

    public string GetArmResourceUri() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/subscriptions/{0}/resourcegroups/{1}/providers/{2}/{3}/{4}", (object) this.AzureSubscriptionId, (object) this.AzureCloudServiceName, (object) this.ProviderNamespaceId, (object) "account", (object) this.AzureResourceName);

    public AzureResourceAccount()
    {
    }

    public AzureResourceAccount(
      string azureResourceName,
      Guid azureSubscriptionId,
      Guid accountId,
      Guid collectionId,
      string azureCloudServiceName,
      string azureGeoRegion,
      string eTag,
      OperationResult operationResult)
    {
      this.AccountId = accountId;
      this.CollectionId = collectionId;
      this.ETag = eTag;
      this.AzureSubscriptionId = azureSubscriptionId;
      this.AzureCloudServiceName = azureCloudServiceName;
      this.AzureResourceName = azureResourceName;
      this.AzureGeoRegion = azureGeoRegion;
      this.Created = DateTime.UtcNow;
      this.LastUpdated = DateTime.UtcNow;
      this.OperationResult = operationResult;
    }

    public object Clone() => (object) new AzureResourceAccount()
    {
      AccountId = this.AccountId,
      AlternateCloudServiceName = this.AlternateCloudServiceName,
      AzureCloudServiceName = this.AzureCloudServiceName,
      AzureGeoRegion = this.AzureGeoRegion,
      AzureResourceName = this.AzureResourceName,
      AzureSubscriptionId = this.AzureSubscriptionId,
      CollectionId = this.CollectionId,
      Created = this.Created,
      ETag = this.ETag,
      LastUpdated = this.LastUpdated,
      OperationResult = this.OperationResult,
      ProviderNamespaceId = this.ProviderNamespaceId
    };
  }
}
