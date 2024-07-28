// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySettingsPath
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentitySettingsPath
  {
    internal const string c_registryHive = "/Service/Integration/Settings/";
    public const string FullSync = "/Service/Integration/Settings/IdentitySyncFull";
    public const string SyncCycleStart = "/Service/Integration/Settings/IdentitySyncCycleStart";
    public const string SyncCycleDuration = "/Service/Integration/Settings/IdentitySyncCycleDuration";
    public const string ReadBatchSizeLimit = "/Service/Integration/Settings/ReadBatchSizeLimit";
    public const string IdentitySyncTimeout = "/Service/Integration/Settings/IdentitySyncTimeout";
    public const string IdentityHostCacheSize = "/Service/Integration/Settings/IdentityHostCacheSize";
    public const string IdentitySyncErrorLimit = "/Service/Integration/Settings/IdentitySyncErrorLimit";
    public const string IdentitySyncServicingErrorLimit = "/Service/Integration/Settings/IdentitySyncServicingErrorLimit";
    public const string UseAccountDisplayMode = "/Service/Integration/Settings/useAccountDisplayMode";
    public const string UpdateLastUserAccessInterval = "/Service/Integration/Settings/UpdateLastUserAccessInterval";
    public const string BlockRemovingSelfFromAdminGroup = "/Service/Integration/Settings/BlockRemovingSelfFromAdminGroup";
    public const string SendIdentityChangedEvents = "/Service/Integration/Settings/SendIdentityChangedEvents";
    public const string IdentityEvictionOperationIntervalInHours = "/Service/Integration/Settings/IdentityEvictionOperationIntervalInHours";
    public const string IdentityEvictionEnabled = "/Service/Integration/Settings/IdentityEvictionEnabled";
    public const string IdentityInactivityIntervalInHours = "/Service/Integration/Settings/IdentityInactivityIntervalInHours";
    public const string IdentityExpiryIntervalInHours = "/Service/Integration/Settings/IdentityExpiryIntervalInHours";
    public const string PropertyEvictionOperationIntervalInHours = "/Service/Integration/Settings/PropertyEvictionOperationIntervalInHours";
    public const string PropertyEvictionEnabled = "/Service/Integration/Settings/PropertyEvictionEnabled";
    public const string PropertyInactivityIntervalInHours = "/Service/Integration/Settings/PropertyInactivityIntervalInHours";
    public const string PropertyCacheSize = "/Service/Integration/Settings/PropertyCacheSize";
    internal const string s_registryRoot = "/Service/Identity/Settings/";
    public const string DeploymentCacheSize = "/Service/Identity/Settings/DeploymentCacheSize";
    public const string HostCacheSize = "/Service/Identity/Settings/HostCacheSize";
    public const string IdentityChangePublisherJobQueueDelay = "/Service/Identity/Settings/IdentityChangePublisherJobQueueDelayInSeconds";
    public const string IdentityChangePublisherJobQueuePriority = "/Service/Identity/Settings/IdentityChangePublisherJobQueuePriority";
    public const string IdentityChangePublisherJobSequenceId = "/Service/Identity/Settings/IdentityChangePublisherJobSequenceId";
    public const string IdentityChangePublisherJobIdentitySequenceId = "/Service/Identity/Settings/IdentityChangePublisherJobIdentitySequenceId";
    public const string IdentityChangePublisherJobGroupSequenceId = "/Service/Identity/Settings/IdentityChangePublisherJobGroupSequenceId";
    public const string OrganizationIdentityChangePublisherJobGroupSequenceId = "/Service/Identity/Settings/IdentityChangePublisherJobOrganizationIdentitySequenceId";
    public const string IdentityChangeQueueCleanupBefore = "/Service/Identity/Settings/IdentityChangeQueueCleanupBeforeDays";
    public const string IdentityChangePublisherMessageBufferSize = "/Service/Identity/Settings/IdentityChangePublisherMessageBufferSize";
    public const string IdentityMegaTenant = "/Service/Identity/Settings/IdentityMegaTenant";
    public const string IdentityMegaTenantSize = "/Service/Identity/Settings/IdentityMegaTenantSize";
    public const string IdentityAuditFirstSequenceIdFormattable = "/Service/Identity/Settings/IdentityAuditFirstSequenceId/{0}";
    public const string IdentityMinimumResourceVersion = "/Service/Identity/Settings/IdentityMinimumResourceVersion";
    public const string ReadIdentitiesAadMembershipExtensionReadGroupsBatchSizePath = "/Service/Identity/Settings/ReadIdentitiesAadMembershipExtension/ReadGroupsBatchSize";
    public const string IdentityStoreReadGroupsChunkSizePath = "/Service/Identity/Settings/IdentityStore/ReadGroupsChunkSize";
    public const string ReadReplicaRegistryPath = "/Service/Identity/Settings/ReadReplica";
    public const string PercentageCallsToRouteToReadReplicaRegistryPath = "/Service/Identity/Settings/ReadReplica/PercentageCallsToRouteToReadReplica";
    public const string IdentityChangePublisherTaskRegistryPath = "/Service/Identity/Settings/IdentityChangePublisherTask";
    public const string MembershipChangeMaxTaskThresholdRegisteryPath = "/Service/Identity/Settings/MembershipChangeMaxTaskThreshold";
    public const string IdentityChangePublisherTaskQueueDelayRegistryPath = "/Service/Identity/Settings/IdentityChangePublisherTask/QueueDelayInSeconds";
    public static readonly RegistryQuery IdentitySettingsQuery = new RegistryQuery("/Service/Identity/Settings/...");
    public const string InactiveMemberLifeSpan = "/Service/Framework/Settings/DataAge/GroupMembership";
    public const string IdentityIdentifierCacheEvictionEnabled = "/Service/Integration/Settings/IdentityIdentifierCacheEvictionEnabled";
    public const string IdentityIdentifierCacheInactivityIntervalInHours = "/Service/Integration/Settings/IdentityIdentifierCacheInactivityIntervalInHours";
    public const string IdentityIdentifierHostCacheSize = "/Service/Identity/Settings/IdentityIdentifierHostCacheSize";
    public const string IdentityIdentifierDeploymentCacheSize = "/Service/Identity/Settings/IdentityIdentifierDeploymentCacheSize";
    public const string IdentityIdentifierCacheEvictionOperationIntervalInHours = "/Service/Integration/Settings/IdentityIdentifierCacheEvictionOperationIntervalInHours";
    public const string PlatformIdentityServiceUseReadAncestorMemberships = "VisualStudio.Services.Identity.PlatformIdentityService.UseReadAncestorMemberships";
    public const string AadGroupIntegrationUseAadGroupsAncestorsCache = "VisualStudio.Services.Identity.AadGroupIntegration.UseAadGroupsAncestorsCache";
  }
}
