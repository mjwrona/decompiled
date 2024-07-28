// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolProperties
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public static class MachinePoolProperties
  {
    public const string AgentSpecSize = "AgentSpecSize";
    public const string AutoResizingEnabled = "AutoResizingEnabled";
    public const string BlockedIPs = "BlockedIPs";
    public const string BuildDirectory = "BuildDirectory";
    public const string CleanupPolicy = "CleanupPolicy";
    public const string CreateFactoryDiskMachineProvisionedEventTimeout = "CreateFactoryDiskMachineProvisionedEventTimeout";
    public const string CreateFactoryDisksOrchestrationId = "CreateFactoryDisksOrchestrationId";
    public const string CurrentMachinePoolImageFactoryDiskLabel = "CurrentMachinePoolImageFactoryDiskLabel";
    public const string DataDiskCachingType = "DataDiskCachingType";
    public const string DataDiskCount = "DataDiskCount";
    public const string DataDiskSizeGB = "DataDiskSizeGB";
    public const string DebugMode = "DebugMode";
    public const string DebugStateDelayDurationDays = "DebugStateDelayDurationDays";
    public const string DisableGalleryImageUpdates = "DisableGalleryImageUpdates";
    public const string DisableMachineWarmUp = "DisableMachineWarmUp";
    public const string DisableSecureTimeSeeding = "DisableSecureTimeSeeding";
    public const string DisconnectTimeout = "DisconnectTimeout";
    public const string EnableHostsFileMonitoring = "EnableHostsFileMonitoring";
    public const string EnableNestedInlineImageUpdates = "EnableNestedInlineImageUpdates";
    public const string EnableProvisionerServerTracing = "EnableProvisionerServerTracing";
    public const string ErrorStateDelayDurationMinutes = "ErrorStateDelayDurationMinutes";
    public const string FactoryDiskId = "FactoryDiskId";
    public const string FactoryDiskSizeGB = "FactoryDiskSizeGB";
    public const string FactoryImageReplicaCount = "FactoryImageReplicaCount";
    public const string FactoryImageStorageAccountType = "FactoryImageStorageAccountType";
    public const string FactoryScriptTimeout = "FactoryScriptTimeout";
    public const string FoldAtHomeEnabled = "FoldAtHomeEnabled";
    public const string HostsBlacklist = "HostsBlacklist";
    public const string KeepErrorMachinesAllocated = "KeepErrorMachinesAllocated";
    public const string MachineProvisionerVersion = "MachineProvisionerVersion";
    public const string MachineReimageLaterTimeoutMinutes = "MachineReimageLaterTimeoutMinutes";
    public const string MachineReimageLaterStaggerRangeMinutes = "MachineReimageLaterStaggerRangeMinutes";
    public const string MachinesDisabledForInvestigation = "MachinesDisabledForInvestigation";
    public const string MaxNumberOfMachinesToDisableForInvestigation = "MaxNumberOfMachinesToDisableForInvestigation";
    public const string MinNumberOfMachinesInPool = "MinNumberOfMachinesInPool";
    public const string NetworkSecurityGroup = "NetworkSecurityGroup";
    public const string NotStartedTimeout = "NotStartedTimeout";
    public const string NumberOfRequestWaitingAlertThreshold = "NumberOfRequestWaitingAlertThreshold";
    public const string PoolScopedVnetName = "PoolScopedVnetName";
    public const string PoolScopedVnetResourceGroupName = "PoolScopedVnetResourceGroupName";
    public const string PoolScopedVnetSubscriptionId = "PoolScopedVnetSubscriptionId";
    public const string PreviousMachinePoolImageFactoryDiskLabel = "PreviousMachinePoolImageFactoryDiskLabel";
    public const string RamDiskSizeGB = "RamDiskSizeGB";
    public const string ReimageDurationAlertThreshold = "ReimageDurationAlertThreshold";
    public const string RequestAssignmentAttemptsLimit = "RequestAssignmentAttemptsLimit";
    public const string SharedFactorySnapshotEnabled = "SharedFactorySnapshotEnabled";
    public const string SharedFactorySnapshotName = "SharedFactorySnapshotName";
    public const string SharedFactorySnapshotResourceGroup = "SharedFactorySnapshotResourceGroup";
    public const string SharedFactorySnapshotSubscriptionId = "SharedFactorySnapshotSubscriptionId";
    public const string SupportedImageLabels = "SupportedImageLabels";
    public const string DiskFormatExtension = "DiskFormatExtension";
    public const string UpdateMachinePoolImageFactoryDiskLabel = "UpdateMachinePoolImageFactoryDiskLabel";
    public const string UpdateMachinePoolImageOrchestrationId = "UpdateMachinePoolImageOrchestrationId";
    public const string UpdateMachinePoolImageTargetName = "UpdateMachinePoolImageTargetName";
    public const string UpdateMachinePoolImageTargetVersion = "UpdateMachinePoolImageTargetVersion";
    public const string UseRamDisks = "UseRamDisks";
    public const string VirtualPoolForceTargetRealm = "VirtualPoolForceTargetRealm";
    public const string VirtualPoolTargetInstances = "VirtualPoolTargetInstances";
    public const string VirtualPoolHealthRequestCheckIntervalSeconds = "VirtualPoolHealthRequestCheckIntervalSeconds";
    public const string VirtualPoolHealthRequestMaxTimeoutSeconds = "VirtualPoolHealthRequestMaxTimeoutSeconds";
    public const string VirtualPoolMonitorRequestIntervalSeconds = "VirtualPoolMonitorRequestIntervalSeconds";
    public const string VirtualPoolWaitForFinishTimeoutSeconds = "VirtualPoolWaitForFinishTimeoutSeconds";
    public const string UseEphemeralDisk = "UseEphemeralDisk";
    public const string UseFactoryGalleryImageVersion = "UseFactoryGalleryImageVersion";
    public const string UsePoolScopedVnet = "UsePoolScopedVnet";

    public static string GetUpdateMachinePoolImageOrchestrationId(this MachinePool pool) => pool.Properties.GetValue<string>("UpdateMachinePoolImageOrchestrationId", (string) null);

    public static string GetUpdateMachinePoolTargetImageName(this MachinePool pool) => pool.Properties.GetValue<string>("UpdateMachinePoolImageTargetName", (string) null);

    public static string GetUpdateMachinePoolTargetImageVersion(this MachinePool pool) => pool.Properties.GetValue<string>("UpdateMachinePoolImageTargetVersion", (string) null);

    public static void SetUpdateMachinePoolImageInfo(
      this MachinePool pool,
      string orchestrationId,
      string imageName,
      string imageVersion)
    {
      pool.Properties["UpdateMachinePoolImageOrchestrationId"] = (object) orchestrationId;
      pool.Properties["UpdateMachinePoolImageTargetName"] = (object) imageName;
      pool.Properties["UpdateMachinePoolImageTargetVersion"] = (object) imageVersion;
    }

    public static bool HasPoolScopedVnet(this MachinePool pool) => pool.Properties.GetValue<bool>("UsePoolScopedVnet", false);
  }
}
