// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobNameConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class JobNameConstants
  {
    internal static readonly int MaximumJobNameLength = 128;
    internal static readonly string AssignHostingAccountFormat = "Assign hosting account {0} ({1})";
    internal static readonly string AssignTfsAccountFormat = "Assign tfs account {0} ({1})";
    internal static readonly string CreateTfsAccountFormat = "Create Tfs account host {0} ({1})";
    internal static readonly string CreateHostingAccountFormat = "Create hosting account {0} ({1})";
    internal static readonly string CreateHostingOrganizationFormat = "Create hosting organization {0} ({1})";
    internal static readonly string CreateCollectionFormat = "Create collection {0} ({1})";
    internal static readonly string CreateSchemaFormat = "Create schema {0}";
    public static readonly string PrecreateHostingAccountFormat = "Pre-create hosting account {0}";
    public static readonly string PrecreateHostingOrganizationFormat = "Pre-create hosting organization {0}";
    internal static readonly string ProvisionCreateCollectionFormat = "Provision Create Team Project collection (Name: {0}, CollectionId: {1})";
    internal static readonly string ProvisionCreateCollectionShortFormat = "Provision Create Collection (CollectionId: {0})";
    internal static readonly string HostingAccountExpiration = "Hosting account expiration job";
    internal static readonly string HostedBuildResultLogging = "Hosted Build Result Logging";
    internal static readonly string AccountPreCreation = "Account pre-creation job";
    internal static readonly string JobHistoryCleanup = "Job History Cleanup Job";
    internal static readonly string RuntimeTelemetryDataCollector = "Runtime Telemetry Data Collector Job";
    internal static readonly string SecurityIdentityCleanup = "Security Identity Cleanup Job";
    internal static readonly string TeamoundationServerActivityLoggingAdministration = "Team Foundation Server Activity Logging Administration";
    internal static readonly string TeamFoundationServerApplicationTierMaintanence = "Team Foundation Server Application Tier Maintanence Job";
    internal static readonly string TeamFoundationServerDataMaintanence = "Team Foundation Server Data Maintanence";
    internal static readonly string TeamFoundationServerDatabaseOptimization = "Team Foundation Server Database Optimization";
    internal static readonly string TeamFoundationServerImageCleanup = "Team Foundation Server Image Cleanup";
    internal static readonly string TeamFoundationServerOnDemandIdentitySynchronization = "Team Foundation Server On Demand Identity Synchronization";
    internal static readonly string TeamFoundationServerPeriodicIdentitySynchronization = "Team Foundation Server Periodic Identity Synchronization";
    internal static readonly string TeamFoundationServerSendEmailConfirmation = "Team Foundation Server Send Email Confirmation Job";
    internal static readonly string BuildInformationCleanup = "Build Information Cleanup Job";
    internal static readonly string CleanupDiscussionDatabase = "Cleanup Discussion Database";
    internal static readonly string CleanupServicingTables = "Cleanup servicing tables job";
    internal static readonly string CleanupTestManagementDatabase = "Cleanup TestManagement Database";
    internal static readonly string CleanupTestImpactDatabase = "Cleanup TestImpact Database";
    internal static readonly string SoftDeleteLegacyTestImpactDatabase = "Soft Delete Legacy TestImpact Database";
    internal static readonly string CleanupLegacyTestImpactDatabase = "Cleanup Legacy TestImpact Database";
    internal static readonly string CleanupDistributedTestRunsJob = "Cleanup Distributed Test Runs Job";
    internal static readonly string LabManagerOperationCleanup = "LabManager Operation Cleanup Job";
    internal static readonly string LabManagerOperationRecovery = "LabManager Operation Recovery Job";
    internal static readonly string MessageQueueCleanup = "Message Queue Cleanup Job";
    internal static readonly string RepopulateDynamicSuites = "Repopulate Dynamic Suites";
    internal static readonly string SynchronizeTestCases = "Synchronize Test Cases";
    internal static readonly string TeamFoundationServerActivityLoggingAdministration = "Team Foundation Server Activity Logging Administration";
    internal static readonly string TeamFoundationServerCoverageAnalysis = "Team Foundation Server Coverage Analysis";
    internal static readonly string TeamFoundationServerSendMailJob = "Team Foundation Server Send Mail Job";
    internal static readonly string VersionControlAdministration = "Version Control Administration";
    internal static readonly string VersionControlCodeChurn = "Version Control Code Churn";
    internal static readonly string VersionControlDeltaProcessing = "Version Control Delta Processing";
    internal static readonly string VersionControlStatisticsUpdate = "Version Control Statistics Update";
    internal static readonly string WorkItemTrackingAdministration = "Work Item Tracking Administration";
    internal static readonly string WorkItemTrackingIdentityProcessing = "Work Item Tracking Identity Processing";
    internal static readonly string WorkItemTrackingIntegrationSynchronization = "Work Item Tracking Integration Synchronization";
    internal static readonly string WorkItemTrackingReferencedIdentitiesUpdate = "Work Item Tracking Referenced Identities Update";
    internal static readonly string WorkItemTrackingRemoveunusedconstants = "Work Item Tracking Remove unused constants";
    internal static readonly string WorkItemTrackingRemoveOrphanAttachments = "Work Item Tracking Remove Orphan Attachments";
    internal static readonly string TeamFoundationServerCleanupJob = "Team Foundation Server Cleanup";
    internal static readonly string TeamFoundationServerEventProcessing = "Team Foundation Server Event Processing";
    internal static readonly string TeamFoundationServerFrameworkFileContainerCleanup = "Team Foundation Server Framework File Container Cleanup";
    internal static readonly string TeamFoundationServerFrameworkFileServiceCleanup = "Team Foundation Server Framework File Service Cleanup";
    internal static readonly string TeamFoundationServerUserActivityJob = "Team Foundation Server User Activity Job";
    internal static readonly string ValidateJobAgentPluginsJob = "Validate Job Agent Plugins";
  }
}
