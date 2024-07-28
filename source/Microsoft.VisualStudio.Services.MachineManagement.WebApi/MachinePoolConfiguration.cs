// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolConfiguration
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public class MachinePoolConfiguration
  {
    public string Name { get; set; }

    public string ImageLabel { get; set; }

    public string[] SupportedImageLabels { get; set; }

    public string ImageName { get; set; }

    public string ImagePublisher { get; set; }

    public string ImageOffer { get; set; }

    public string ImageSku { get; set; }

    public string ImageVersion { get; set; }

    public int? FactoryDiskSizeGB { get; set; }

    public int? FactoryScriptTimeout { get; set; }

    public int? CreateFactoryDiskMachineProvisionedEventTimeout { get; set; }

    public bool UseNestedVirtualization { get; set; }

    public short InstanceParallelism { get; set; }

    public int MachineCount { get; set; }

    public string OperatingSystem { get; set; }

    public string SubscriptionId { get; set; }

    public string ResourceProviderType { get; set; }

    public string Region { get; set; }

    public string RoleSize { get; set; }

    public string BuildDirectory { get; set; }

    public string AzureRegion { get; set; }

    public string PerformanceTier { get; set; }

    public bool IncludeKeyVault { get; set; }

    public string KeyVaultObjectId { get; set; }

    public string KeyVaultTenantId { get; set; }

    public string[] KeyVaultGetAccessObjectIds { get; set; }

    public string[] KeyVaultAllAccessObjectIds { get; set; }

    public string ImageRepoSubscriptionId { get; set; }

    public string ImageRepoResourceGroup { get; set; }

    public string ImageRepoStorageAccount { get; set; }

    public string ImageRepoSasToken { get; set; }

    public bool SharedFactorySnapshotEnabled { get; set; }

    public string SharedFactorySnapshotSubscriptionId { get; set; }

    public string SharedFactorySnapshotResourceGroup { get; set; }

    public string SharedFactorySnapshotName { get; set; }
  }
}
