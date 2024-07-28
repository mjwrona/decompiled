// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.JobSettings
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class JobSettings
  {
    public JobSettings()
    {
    }

    public JobSettings(IVssRequestContext requestContext)
    {
      this.BatchCountForReindexing = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BatchCountForReindexing");
      this.TriggerAndMonitorReindexingJobDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/TriggerAndMonitorReindexingJobDelayInSec");
      this.MaxTimeIntervalForAccountReindexingInDays = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxTimeIntervalForAccountReindexingInDays");
      this.RepositoryHealerBatchSize = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/RepositoryHealerBatchSize");
      this.RepositoryMetaDataCrawlJobRetryDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/RepositoryMetaDataCrawlJobRetryDelayInSec");
      this.MaxIndexingRetryCount = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxIndexingRetryCount");
      this.PatchInMemoryThresholdForMaxDocs = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PatchInMemoryThresholdForMaxDocs");
      this.LargeRepositoryThresholdCountToUseFileMetadataStoreLookup = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/LargeRepositoryThresholdCountToUseFileMetadataStoreLookup");
      this.AccountFaultInJobDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/AccountFaultInJobDelayInSec");
      this.AccountFaultInJobMaxRetries = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/AccountFaultInJobMaxRetries");
      this.AccountHealthStatusJobMaxRetries = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/AccountHealthStatusJobMaxRetries");
      this.SettingsIndexUpdateJobMaxRetries = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/SettingsIndexUpdateJobMaxRetries");
      this.PeriodicWorkItemRefreshJobDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PeriodicWorkItemRefreshJobDelayInSec");
      this.PeriodicWikiCatchUpJobDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PeriodicWikiCatchUpJobDelayInSec");
      this.PeriodicMaintenanceJobDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PeriodicMaintenanceJobDelayInSec");
      this.PeriodicCatchUpJobDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PeriodicCatchUpJobDelayInSec");
      this.ProjectIndexerJobDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/ProjectIndexerJobDelayInSec");
      this.PeriodicCleanUpOfOlderEventsInHours = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PeriodicCleanUpOfOlderEventsInHours");
      this.ElasticSearchMonitorBackupJobDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/ElasticSearchMonitorBackupJobDelayInSec");
      this.SettingsIndexUpdateJobDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/SettingsIndexUpdateJobDelayInSec");
      this.AzureContainerForBackup = requestContext.GetConfigValue("/Service/ALMSearch/Settings/AzureContainerForBackup");
      this.BasePathInContainer = requestContext.GetConfigValue("/Service/ALMSearch/Settings/BasePathInContainer");
      this.ESBackupRepositoryName = requestContext.GetConfigValue("/Service/ALMSearch/Settings/ESBackupRepositoryName");
      this.ESBackupJobPeriodicityInDays = requestContext.GetConfigValue("/Service/ALMSearch/Settings/ESBackupJobPeriodicity");
      this.BatchCountForCustomRepoProcessing = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BatchCountForCustomRepoProcessing");
      this.MaxAllowedOnPremiseJobExecutionTimeInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxAllowedOnPremiseJobExecutionTimeInSec");
      this.FileLevelFailureAlertThreshold = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FileLevelFailureAlertThreshold");
      this.FailedItemsMaxRetryCount = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FailedItemsMaxRetryCount");
      this.FailedItemsPatchOperationDelayInSeconds = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FailedItemsPatchOperationDelayInSeconds");
      this.TFSGetMetaDataMaxThresholdForDocs = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/TFSGetMetaDataMaxThresholdForDocs");
      this.TFSGetMetaDataMinThresholdForDocs = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/TFSGetMetaDataMinThresholdForDocs");
      this.MaxNumberOfFailedItemsToProcess = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxNumberOfFailedItemsToProcess");
      if (this.MaxNumberOfFailedItemsToProcess <= 0)
        this.MaxNumberOfFailedItemsToProcess = 500;
      if (this.FailedItemsPatchOperationDelayInSeconds <= 0)
        this.FailedItemsPatchOperationDelayInSeconds = 1800;
      if (this.LargeRepositoryThresholdCountToUseFileMetadataStoreLookup <= 0)
        this.LargeRepositoryThresholdCountToUseFileMetadataStoreLookup = 1000;
      if (this.TFSGetMetaDataMaxThresholdForDocs <= 0)
        this.TFSGetMetaDataMaxThresholdForDocs = 10000;
      if (this.TFSGetMetaDataMinThresholdForDocs <= 0)
        this.TFSGetMetaDataMinThresholdForDocs = 100;
      if (this.AccountFaultInJobMaxRetries <= 0)
        this.AccountFaultInJobMaxRetries = 3;
      if (this.AccountHealthStatusJobMaxRetries <= 0)
        this.AccountHealthStatusJobMaxRetries = 2;
      if (this.SettingsIndexUpdateJobMaxRetries <= 0)
        this.SettingsIndexUpdateJobMaxRetries = 3;
      if (this.AccountFaultInJobDelayInSec < 0)
        this.AccountFaultInJobDelayInSec = 120;
      this.TfvcBIEventDelayInMinutes = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/TfvcBIEventDelayInMinutes", 0);
    }

    public int BatchCountForReindexing { get; set; }

    public int TriggerAndMonitorReindexingJobDelayInSec { get; set; }

    public int MaxTimeIntervalForAccountReindexingInDays { get; set; }

    public int RepositoryHealerBatchSize { get; set; }

    public int RepositoryMetaDataCrawlJobRetryDelayInSec { get; set; }

    public int MaxIndexingRetryCount { get; set; }

    public int PatchInMemoryThresholdForMaxDocs { get; set; }

    public int LargeRepositoryThresholdCountToUseFileMetadataStoreLookup { get; set; }

    public int AccountFaultInJobDelayInSec { get; set; }

    public int AccountFaultInJobMaxRetries { get; set; }

    public int AccountHealthStatusJobMaxRetries { get; set; }

    public int SettingsIndexUpdateJobMaxRetries { get; set; }

    public int PeriodicWorkItemRefreshJobDelayInSec { get; set; }

    public int PeriodicWikiCatchUpJobDelayInSec { get; set; }

    public int PeriodicMaintenanceJobDelayInSec { get; set; }

    public int PeriodicCatchUpJobDelayInSec { get; set; }

    public int ProjectIndexerJobDelayInSec { get; set; }

    public int PeriodicCleanUpOfOlderEventsInHours { get; set; }

    public int ElasticSearchMonitorBackupJobDelayInSec { get; set; }

    public int SettingsIndexUpdateJobDelayInSec { get; set; }

    public string AzureContainerForBackup { get; set; }

    public string BasePathInContainer { get; set; }

    public string ESBackupRepositoryName { get; set; }

    public string ESBackupJobPeriodicityInDays { get; set; }

    public int BatchCountForCustomRepoProcessing { get; set; }

    public int MaxAllowedOnPremiseJobExecutionTimeInSec { get; set; }

    public int FileLevelFailureAlertThreshold { get; set; }

    public int FailedItemsMaxRetryCount { get; set; }

    public int FailedItemsPatchOperationDelayInSeconds { get; set; }

    public int TfvcBIEventDelayInMinutes { get; set; }

    public int TFSGetMetaDataMaxThresholdForDocs { get; set; }

    public int TFSGetMetaDataMinThresholdForDocs { get; set; }

    public int MaxNumberOfFailedItemsToProcess { get; set; }
  }
}
