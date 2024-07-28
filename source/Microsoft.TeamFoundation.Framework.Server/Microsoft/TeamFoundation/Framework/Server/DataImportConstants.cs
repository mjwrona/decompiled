// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataImportConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class DataImportConstants
  {
    public const string Area = "DataImport";
    public const string DataImportRegistryPath = "/Configuration/DataImport";
    public const string DataImportId = "/Configuration/DataImport/DataImportId";
    public const string DataImportMaxBlobCopies = "/Configuration/DataImport/MaxBlobCopies";
    public const string DataImportFileBatchSize = "/Configuration/DataImport/FileBatchSize";
    public const string DataImportMaxBlobCopyMultiplier = "/Configuration/DataImport/MaxBlobCopyMultiplier";
    public static readonly string DataImportJobRetryThreshold = ServicingOrchestrationConstants.JobRetryThreshold("DataImport");
    public static readonly string DataImportJobRetryInterval = ServicingOrchestrationConstants.JobRetryInterval("DataImport");
    public static readonly string DataImportJobMinWait = ServicingOrchestrationConstants.JobRetryMinWait("DataImport");
    public static readonly string DataImportServicingLogsRetryThreshold = ServicingOrchestrationConstants.ServicingLogsRetryThreshold("DataImport");
    public static readonly string DataImportServicingLogsRetryInterval = ServicingOrchestrationConstants.ServicingLogsRetryInterval("DataImport");
    public const string DataImportOptOutKey = "/Configuration/DataImport/OptOut";
    public const string DataImportAllowCustomTargetServiceObjective = "/Configuration/DataImport/AllowCustomTargetServiceObjective";
    public const string DataImportUnsupportedCollations = "/Configuration/DataImport/UnsupportedCollations";
    public const string DataImportSupportedMilestones = "/Configuration/DataImport/SupportedMilestones";
    public const string DataImportStretchMilestones = "/Configuration/DataImport/StretchMilestones";
    public const string DataImportSupportedServices = "/Configuration/DataImport/SupportedServices";
    public const string DataImportSourceOnPremServerInstanceId = "/Configuration/DataImport/SourceOnPremServerInstanceId";
    public const string DataImportGlobalCleanupThreshold = "/Configuration/DataImport/CleanupThreshold";
    public const string DataImportBlockOnInProgressDatabaseUpgrade = "/Configuration/DataImport/BlockOnInProgressDatabaseUpgrade";
    public const string WaitForPreviouslySelectedDatabaseMin = "/Configuration/DataImport/WaitForPreviouslySelectedDatabaseMin";
    public const string AssumeHoldAbandonedAfterMin = "/Configuration/DataImport/AssumeHoldAbandonedAfterMin";
    public const string DataImportEngTeamDL = "/Configuration/DataImport/EngTeamDL";
    public const string DataImportIdentityMappingSizeLimitInBytes = "/Configuration/DataImport/IdentityMappingSizeLimitInBytes";
    public const string DataImportIdentityIdsToSkipDuringValidation = "/Configuration/DataImport/IdentityIdsToSkipDuringValidation";
    public const string DataImportMinimumReturnToServiceObjective = "/Configuration/DataImport/MinimumReturnToServiceObjective";
    public const string DataImportMinimumPromotedServiceObjective = "/Configuration/DataImport/MinimumPromotedServiceObjective";
    public const string DataImportUpgradeStandardGeneralPopulationServiceObjective = "/Configuration/DataImport/UpgradeStandardGeneralPopulationServiceObjective";
    public const string DataImportUpgradePremiumGeneralPopulationServiceObjective = "/Configuration/DataImport/UpgradePremiumGeneralPopulationServiceObjective";
    public const string DataImportLargeImportRegistryPath = "/Configuration/DataImport/LargeImport";
    public const string DataImportLargeImportDatabaseServiceObjective = "/Configuration/DataImport/LargeImport/ServiceObjective";
    public const string DataImportLargeImportImportThresholdInMb = "/Configuration/DataImport/LargeImport/ImportThresholdInMB";
    public const string DataImportCollectionImportDbSizeInGb = "/Configuration/DataImport/CollectionImportDbSizeInGB";
    public const string DatabaseIdsToIgnoreWhenSelectingGeneralPopulation = "/Configuration/DataImport/DatabaseIdsToIgnoreWhenSelectingGeneralPopulation";
    public const string DataImportDatabaseSizePercentThreshold = "/Configuration/DataImport/DatabaseSizePercentThreshold";
    public const string DataImportDefaultImportVersion = "/Configuration/DataImport/DefaultImportVersion";
    public const string DataImportWaitForHostStopTimeout = "/Configuration/DataImport/WaitForHostStopTimeout";
    public const string DataImportWaitForConnectionTimeout = "/Configuration/DataImport/WaitForConnectionTimeout";
    public const string DataImportCompletedHistoryWindow = "/Configuration/DataImport/CompletedHistoryWindow";
    public const string DataImportUncompletedHistoryWindow = "/Configuration/DataImport/UncompletedHistoryWindow";
    public const string DataImportDefaultRestRetryDelay = "/Configuration/DataImport/DefaultRestRetryDelay";
    public const string DataImportDefaultRestRetryCount = "/Configuration/DataImport/DefaultRestRetryCount";
    public const string FileImportHelperLongRunningTasksFeatureName = "VisualStudio.Services.DataImport.FileImportHelperLongRunningTasks";
    public const string DataImportMaxConcurrentDacpacDeploys = "/Configuration/DataImport/MaxConcurrentDacpacDeploys";
    public const string DataImportDacServicesQueryTimeoutInSeconds = "/Configuration/DataImport/DacServicesTimeoutInSeconds";
    public const string DataImportDacServicesDeploymentTimeoutInSeconds = "/Configuration/DataImport/DacServicesDeploymentTimeoutInSeconds";
    public const string DataImportDacServicesMaxDeploymentAttempts = "/Configuration/DataImport/DacServicesMaxDeploymentAttempts";
    public const string DataImportDacServicesStuckDeploymentThresholdInSeconds = "/Configuration/DataImport/DacServicesStuckDeploymentThresholdInSeconds";
    public const string DataImportDacServicesThreadWaitTimeInMs = "/Configuration/DataImport/DacServicesThreadWaitTimeInMs";
    public const string DataImportHostName = "/Configuration/DataImport/HostName";
    public const string DataImportSpsUrl = "/Configuration/DataImport/SpsUrlForOrgApi";
    public const string DataImportMinimumPartitionDBs = "/DataImport/MinimumPartitionDBs";
    public const string DataImportCurrentlyPromotingDBs = "/DataImport/CurrentlyPromotingDBs";
    public const string DataImportPromotingPartitionDB = "/DataImport/CurrentlyPromotingDBs/{0}";
    public const string DataImportOwnDatabaseSizeThresholdMB = "/Configuration/DataImport/OwnDatabaseSizeThresholdMB/{0}";
    public const string DataImportOwnDatabaseLicenseCount = "/Configuration/DataImport/OwnDatabaseLicenseCount/{0}";
    public const string DataImportShortWaitLicenseCount = "/Configuration/DataImport/ShortWaitLicenseCount/{0}";
    public const string DataImportShortWaitLengthMin = "/Configuration/DataImport/ShortWaitLengthMin/{0}";
    public const string DataImportLongWaitLengthMin = "/Configuration/DataImport/LongWaitLengthMin/{0}";
    public const string DataImportAll = "/Configuration/DataImport/{0}/**";
    public const string DataImportTargetDatabaseId = "/Configuration/DataImport/{0}/TargetDatabaseId";
    public const string DataImportGoldenDatabaseId = "/Configuration/DataImport/{0}/GoldenDatabaseId";
    public const string DataImportSourceDatabaseId = "/Configuration/DataImport/{0}/SourceDatabaseId";
    public const string DataImportFinalDestinationDatabaseId = "/Configuration/DataImport/{0}/FinalDestinationDatabaseId";
    public const string DataImportFileWatermark = "/Configuration/DataImport/{0}/FileWatermark";
    public const string DataImportTotalBytesWritten = "/Configuration/DataImport/{0}/TotalBytesWritten";
    public const string DataImportPreviousDatabaseServiceLevel = "/Configuration/DataImport/{0}/PreviousDatabaseServiceLevel";
    public const string DataImportCompletedPreHostMoveChecks = "/Configuration/DataImport/{0}/CompletedPreHostMoveChecks{1}";
    public const string DataImportDatabaseUpgradeJobId = "/Configuration/DataImport/{0}/DatabaseUpgradeJobId";
    public const string DataImportHostUpgradeJobId = "/Configuration/DataImport/{0}/HostUpgradeJobId";
    public const string DataImportTargetHostId = "/Configuration/DataImport/{0}/TargetHostId";
    public const string DataImportDehydrationStatus = "/Configuration/DataImport/{0}/DataImportDehydrationStatus";
    public const string DataImportDatabaseUpgradeJobBucket = "/Configuration/DataImport/{0}/DatabaseUpgradeJobs";
    public const string DataImportDatabaseUpgradeRequiredCollection = "/Configuration/DataImport/{0}/DatabaseUpgradeRequiredCollection";
    public const string DataImportPromotionPool = "/Configuration/DataImport/{0}/PromotionPool";
    public const string DataImportSealPromotedDatabase = "/Configuration/DataImport/{0}/SealPromotedDatabase";
    public const string DataImportResourceIncludedQuantity = "/Configuration/DataImport/{0}/ResourceIncludedQuantity";
    public const string DataImportResourceMaxQuantity = "/Configuration/DataImport/{0}/ResourceMaxQuantity";
    public const string DataImportEmailSent = "/Configuration/DataImport/{0}/EmailSent/{1}";
    public const string DataImportStartWaitTimeSelectDatabase = "/Configuration/DataImport/{0}/StartWaitTime/SelectDatabase";
    public const string DataImportStartWaitTimeForGivenDatabase = "/Configuration/DataImport/{0}/StartWaitTime/ForGivenDatabase";
    public const string DataImportWaitStepIdentifiers = "/Configuration/DataImport/{0}/WaitStepIdentifiers/";
    public const string DataImportDryRunExpiryDate = "/Configuration/DataImport/{0}/DryRunExpiryDate";
    public const string DataImportDryRunLifeSpan = "/Configuration/DataImport/{0}/DryRunLifeSpan";
    public const string DataImportServicesToImport = "/Configuration/DataImport/{0}/Services";
    public const string DataImportHostUpgradeJobIds = "/Configuration/DataImport/{0}/HostUpgradeJobIds";
    public const string DataImportInstanceData = "/Configuration/DataImport/{0}/InstanceData";
    public const string DataImportStatus = "/Configuration/DataImport/{0}/Status";
    public const string FileCopyParameters = "/Configuration/DataImport/{0}/CopyParameters";
    public const string DataImportFileCopyRequestId = "/Configuration/DataImport/{0}/FileCopyRequestId";
    public const string DataImportJoinedFileCopy = "/Configuration/DataImport/{0}/JoinedFileCopy";
    public const string DataImportExpiryNotificationSent = "/Configuration/DataImport/{0}/ExpiryNotificationSent";
    public const string DataImportExpiryNotificationDaysInAdvance = "/Configuration/DataImport/ExpiryNotificationDaysInAdvance";
    public const string DataImportCleanupJobForceExecution = "/Configuration/DataImport/CleanupJobForceExecution";
    public const string DataImportCiPath = "/Configuration/DataImport/{0}/Ci";
    public const string DataImportCiAll = "/Configuration/DataImport/{0}/Ci/**";
    public const string DataImportStartTime = "/Configuration/DataImport/{0}/Ci/StartTime";
    public const string DataImportFinishTime = "/Configuration/DataImport/{0}/Ci/FinishTime";
    public const string DataImportFailureArea = "/Configuration/DataImport/{0}/Ci/FailureArea";
    public const string DataImportNumberofMappedUsers = "/Configuration/DataImport/{0}/Ci/NumberOfMappedUsers";
    public const string DataImportWitData = "/Configuration/DataImport/{0}/Ci/WorkitemTrackingData";
    public const string DataImportLicenseCount = "/Configuration/DataImport/{0}/Ci/NonStakeholderCount";
    public const string DataImportUserErrorMessage = "/Configuration/DataImport/{0}/Ci/UserErrorMessage";
    public const string DataImportServiceErrorMessage = "/Configuration/DataImport/{0}/Ci/ServiceErrorMessage";
    public const string DataImportDryRunHostExpiryDate = "/Configuration/DataImport/DataImportDryRunHostExpiryDate";
    public const string DataImportResponsePropertiesPath = "/Configuration/DataImport/{0}/ResultProperties";
    public const string DataImportResponsePropertiesAll = "/Configuration/DataImport/{0}/ResultProperties/**";
    public const string DataImportResponseImportServiceInstances = "/Configuration/DataImport/{0}/ResultProperties/ImportServiceInstances";
    public const string DataImportResponseDryRunAccountLifeSpan = "/Configuration/DataImport/{0}/ResultProperties/DryRunAccountLifeSpan";
    public const string DataImportIdAttributeName = "VSS_DATAIMPORT_ID";
    public const string DataImportReadyToDropAttributeName = "VSS_DATAIMPORT_READY_TO_DROP";
    public const int DataImportMinimumPartitionDBsDefault = 2;
    public const string DataImportMinimumPartitionDBsLease = "DataImportMinimumPartitionDBsLease";
    public const string DefaultCollation = "SQL_Latin1_General_CP1_CI_AS;Latin1_General_CI_AS";
    public const string DataImportDefaultHostName = "dataimport.dev.azure.com";
    public const int ImportVersionUnknown = -1;
    public const int ImportVersionInitial = 1;
    public const int ImportVersionDelayHostUpgrade = 2;
    public const int ImportVersionSupportS2SDuringHostUpgrade = 3;
    public const int ImportVersionSupportDelayedHold = 4;
    public const int ImportVersionMaxSupported = 4;
    public const char IgnoreDatabaseIdDelim = ',';
    public const char UpgradeJobDelimiter = ';';
    public const string DefaultServicesToImport = "00025394-6065-48CA-87D9-7F5672854EF7";
    public const string UnsupportedCollationSeparator = ";";
    public const string SupportedRegionsSeparator = ";";
    public static readonly string DataImportCollectionIdExtendedProperty = "TFS_DATAIMPORT_COLLECTIONID";
    public const string DataImportOrganizationPrefix = "DataImport-";
    public const string OrgLeaveOrganizationPrefix = "OrgLeave-";
    public const string DataImportRunType = "/DataImport/RunType";
    public const string DataImportWorkFlowToken = "IsDataImportWorkFlow";
    public static Guid DataImportServiceInstanceType = Guid.Parse("0000003E-0000-8888-8000-000000000000");
    public const string DataImportServiceInstanceTypeName = "DataImport";

    public static string L2Key(string key) => "/L2Test" + key;
  }
}
