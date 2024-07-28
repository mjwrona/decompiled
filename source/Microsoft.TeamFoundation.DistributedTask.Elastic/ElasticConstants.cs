// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  public static class ElasticConstants
  {
    public const string RegistryRoot = "/Service/DistributedTask/ElasticPool/";
    public const string SizingJobPeriodPath = "/Service/DistributedTask/ElasticPool/SizingJobPeriodMinutes";
    public const string ConfigurationJobPeriodPath = "/Service/DistributedTask/ElasticPool/ConfigurationJobPeriodMinutes";
    public const string ScaleUpPercentagePath = "/Service/DistributedTask/ElasticPool/ScaleUpPercentage";
    public const string SingleUseScaleUpPercentagePath = "/Service/DistributedTask/ElasticPool/SingleUseScaleUpPercentage";
    public const string StartTimePath = "/Service/DistributedTask/ElasticPool/StartTimeMinutes";
    public const string LostTimePath = "/Service/DistributedTask/ElasticPool/LostTimeMinutes";
    public const string ReimageTimePath = "/Service/DistributedTask/ElasticPool/ReimageTimeMinutes";
    public const string OfflineTimePath = "/Service/DistributedTask/ElasticPool/OfflineTimeMinutes";
    public const string DeletingTimePath = "/Service/DistributedTask/ElasticPool/DeletingTimeMinutes";
    public const string CreatingTimePath = "/Service/DistributedTask/ElasticPool/CreatingTimeMinutes";
    public const string AzureVMSSClientApiVersion = "/Service/DistributedTask/ElasticPool/AzureVMSSClientApiVersion";
    public const string AzureVMClientApiVersion = "/Service/DistributedTask/ElasticPool/AzureVMClientApiVersion";
    public const string AzureMonitorClientApiVersion = "/Service/DistributedTask/ElasticPool/AzureMonitorClientApiVersion";
    public const string VMExpirationInHours = "/Service/DistributedTask/ElasticPool/VMExpirationInHours";
    public const string ScaleUpPercentagePathFlexible = "/Service/DistributedTask/ElasticPool/ScaleUpPercentageFlexible";
    public const string SingleUseScaleUpPercentagePathFlexible = "/Service/DistributedTask/ElasticPool/SingleUseScaleUpPercentageFlexible";
    public const string StartTimeForUnhealthyVm = "/Service/DistributedTask/ElasticPool/StartTimeForUnhealthyVm";
    public const string OfflineScalesetHoursPath = "/Service/DistributedTask/ElasticPool/OfflineScalesetHours";
    public const string AttemptsBeforeBackoffPath = "/Service/DistributedTask/ElasticPool/AttemptsBeforeBackoff";
    public const string DaysOfLogsToKeepPath = "/Service/DistributedTask/ElasticPool/DaysOfLogsToKeep";
    public const string CleanupAttemptsPath = "/Service/DistributedTask/ElasticPool/CleanupAttempts";
    public const string PrioritizeReimageFactorPath = "/Service/DistributedTask/ElasticPool/PrioritizeReimageFactor";
    public const string PrioritizeReimagePostponeDivisorPath = "/Service/DistributedTask/ElasticPool/PrioritizeReimagePostponeDivisorPath";
    public const string DaysOffProsessFlexiblePools = "/Service/DistributedTask/ElasticPool/DaysOffProsessFlexiblePools";
    public const string ExtensionPublisherPath = "/Service/DistributedTask/ElasticPool/ExtensionPublisher";
    public const string ExtensionPublisherPoolPath = "/Service/DistributedTask/ElasticPool/ExtensionPublisher/{0}";
    public const string WindowsScriptUrlPath = "/Service/DistributedTask/ElasticPool/WindowsScriptUrl";
    public const string WindowsExtensionVersionPath = "/Service/DistributedTask/ElasticPool/WindowsExtensionVersion";
    public const string WindowsScriptVersionPath = "/Service/DistributedTask/ElasticPool/WindowsScriptVersion";
    public const string WindowsAgentUrlPath = "/Service/DistributedTask/ElasticPool/WindowsAgentUrl";
    public const string LinuxScriptUrlPath = "/Service/DistributedTask/ElasticPool/LinuxScriptUrl";
    public const string LinuxExtensionVersionPath = "/Service/DistributedTask/ElasticPool/LinuxExtensionVersion";
    public const string LinuxScriptVersionPath = "/Service/DistributedTask/ElasticPool/LinuxScriptVersion";
    public const string LinuxAgentUrlPath = "/Service/DistributedTask/ElasticPool/LinuxAgentUrl";
    public const string WindowsScriptUrlPoolPath = "/Service/DistributedTask/ElasticPool/WindowsScriptUrl/{0}";
    public const string WindowsExtensionVersionPoolPath = "/Service/DistributedTask/ElasticPool/WindowsExtensionVersion/{0}";
    public const string WindowsScriptVersionPoolPath = "/Service/DistributedTask/ElasticPool/WindowsScriptVersion/{0}";
    public const string WindowsAgentUrlPoolPath = "/Service/DistributedTask/ElasticPool/WindowsAgentUrl/{0}";
    public const string LinuxScriptUrlPoolPath = "/Service/DistributedTask/ElasticPool/LinuxScriptUrl/{0}";
    public const string LinuxExtensionVersionPoolPath = "/Service/DistributedTask/ElasticPool/LinuxExtensionVersion/{0}";
    public const string LinuxScriptVersionPoolPath = "/Service/DistributedTask/ElasticPool/LinuxScriptVersion/{0}";
    public const string LinuxAgentUrlPoolPath = "/Service/DistributedTask/ElasticPool/LinuxAgentUrl/{0}";
    public const int DefaultSizingJobPeriodMinutes = 5;
    public const int DefaultConfigurationJobPeriodMinutes = 120;
    public const int DefaultScaleUpPercentage = 25;
    public const int DefaultSingleUseScaleUpPercentage = 35;
    public const int DefaultScaleUpPercentageFlexible = 20;
    public const int DefaultSingleUseScaleUpPercentageFlexible = 25;
    public const int DefaultStartTimeMinutes = 10;
    public const int DefaultReimageTimeMinutes = 60;
    public const int DefaultIdleTimeMinutes = 30;
    public const int DefaultOfflineTimeMinutes = 20;
    public const int DefaultLostTimeMinutes = 20;
    public const int DefaultDeletingTimeMinutes = 30;
    public const int DefaultOfflineScalesetHours = 10;
    public const int DefaultAttemptsBeforeBackoff = 2;
    public const int DefaultDaysOfLogsToKeep = 30;
    public const int DefaultCleanupAttempts = 2;
    public const int DefaultPrioritizeReimageFactor = 50;
    public const int DefaultCreatingTimeMinutes = 45;
    public const int DefaultPrioritizeReimagePostponeDivisor = 5;
    public const int DefaultVMExpirationInHours = 46;
    public const int DefaultStartTimeForUnhealthyVmMinutes = 10;
    public const string VmssTagKey = "__AzureDevOpsElasticPool";
    public const string VmssTimeStampKey = "__AzureDevOpsElasticPoolTimeStamp";
    public const string ExtensionName = "Microsoft.Azure.DevOps.Pipelines.Agent";
    public const string DevFabricRelayExtensionName = "DevFabricRelay";
    public const string ExtensionPublisher = "Microsoft.VisualStudio.Services";
    public const string WindowsExtensionVersion = "1.31";
    public const string WindowsScriptUrl = "https://vstsagenttools.blob.core.windows.net/tools/ElasticPools/Windows/{0}/enableagent.ps1";
    public const string WindowsScriptVersion = "17";
    public const string LinuxExtensionVersion = "1.23";
    public const string LinuxScriptUrl = "https://vstsagenttools.blob.core.windows.net/tools/ElasticPools/Linux/{0}/enableagent.sh";
    public const string LinuxScriptVersion = "15";
  }
}
