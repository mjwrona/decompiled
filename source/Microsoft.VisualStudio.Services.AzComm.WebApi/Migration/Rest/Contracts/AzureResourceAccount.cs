// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount
// Assembly: Microsoft.VisualStudio.Services.AzComm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1B69FBB-72A0-4C7F-B8FC-E0B0311A8184
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.AzComm.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts
{
  public class AzureResourceAccount
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
  }
}
