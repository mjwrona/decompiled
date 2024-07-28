// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackagingServerConstants
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using System;
using System.ComponentModel;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class PackagingServerConstants
  {
    public const string PackageGracePeriodHoursRegistryPath = "/Configuration/Packaging/DeletedPackagesProcessing/PackageGracePeriodHours";
    public static readonly TimeSpan DefaultPackageGracePeriod = TimeSpan.FromDays(28.0);
    public const string PackageEdgeCachingRootPath = "/Configuration/Packaging/EdgeCaching";
    public const string PackageEdgeCachingEnabledPath = "/Configuration/Packaging/EdgeCaching/Enabled";
    public const string DownloadContentDispositionUsesFilenamesPerProtocolFormat = "/Configuration/Packaging/{0}/Downloads/ContentDispositionUsesFileName/Enabled";
    public const string PrimaryEdgeCachingUrlSigningKeySettingName = "PrimaryEdgeCacheUrlSigningKey";
    public const string SecondaryEdgeCachingUrlSigningKeySettingName = "SecondaryEdgeCacheUrlSigningKey";
    public const string StagingEdgeCachingUrlSigningKeySettingName = "StagingEdgeCacheUrlSigningKey";
    internal const string PackagingServiceRegistryRootPath = "/Configuration/Packaging";
    public const string BookmarkFeedContainerTypeName = "feed";
    public const string BookmarkDeploymentContainerTypeName = "deployment";
    public const string ChangeProcessingBookmarkTokenName = "searchIndexLastUpdatedBookmarkToken";
    public const string DeletedPackageJobBookmarkTokenName = "DeletedPackageLastUpdatedBookmarkToken";
    public const string ContentVerificationBookmarkTokenName = "contentVerificationLastUpdatedBookmarkToken";
    public const string CollectionDeletedFeedsProcessingBookmarkTokenContainerName = "collection";
    public const string CollectionDeletedFeedsProcessingBookmarkTokenName = "LastDeletedFeedsBookmark";
    public const string BlockWriteOperationOnGetRequestCacheKey = "Packaging.BlockWriteOperationOnGetRequest";
    public const string ProgressReportIntervalRegistryKey = "/Configuration/Packaging/PushProgressReportIntervalBytes";
    public const int MinimumLengthForZipArchive = 22;
    public const string UpstreamMetadataCacheRefreshTimeoutSecondsRegistryPathFormat = "/Configuration/Packaging/{0}/UpstreamMetadataCache/RefreshFeedJobTimeoutSeconds";
    public const string UpstreamMetadataCacheRefreshSpreadSecondsRegistryPathFormat = "/Configuration/Packaging/{0}/UpstreamMetadataCache/RefreshFeedJobSpreadSeconds";
    public const int UpstreamMetadataCacheRefreshTimeoutSeconds = 10200;
    public const string CollectionLevelFeedJobQueueingSpreadSecondsRegistryPathFormat = "/Configuration/Packaging/CollectionLevelFeedJobQueueing/{0}/JobSpreadSeconds";
    public const string NumParallelRequestsToTpinDuringRefresh = "/Configuration/Packaging/UpstreamMetadataCache/NumParallelRequestsToTpin";
    public const string MaxNumVersionsPerPkgToSendToTpin = "/Configuration/Packaging/UpstreamMetadataCache/MaxNumVersionsPerPkgToSendToTpin";
    public const string MaxNumSecondsToSpendOnTpin = "/Configuration/Packaging/UpstreamMetadataCache/MaxTimeToSpendOnRequestsToTpin";
    public const string UpstreamMetadataCacheRefreshMaxNumberOfJobsPerFeedRegistryPath = "/Configuration/Packaging/UpstreamMetadataCache/MaxNumberOfJobsPerFeed";
    public const int UpstreamMetadataCacheRefreshMaxNumberOfJobsPerFeedDefault = 3;
    public static readonly RegistryFrotocolLevelPackagingSettingDefinition<int> MaxJobCount = new RegistryFrotocolLevelPackagingSettingDefinition<int>((Func<IProtocol, RegistryQuery>) (protocol => (RegistryQuery) ("/Configuration/Packaging/" + protocol.CorrectlyCasedName + "/UpstreamMetadataCache/MaxNumberOfJobsPerFeed")), (Func<IProtocol, FeedIdentity, RegistryQuery>) ((protocol, feed) => (RegistryQuery) string.Format("/Configuration/Packaging/{0}/UpstreamMetadataCache/MaxNumberOfJobsPerFeed/{1}", (object) protocol.CorrectlyCasedName, (object) feed.Id)), 3, new RegistryQuery?((RegistryQuery) "/Configuration/Packaging/UpstreamMetadataCache/MaxNumberOfJobsPerFeed"));
    public const string UpstreamMetadataCacheRefreshHigherLimitOfWorkPerJobRegistryPath = "/Configuration/packaging/UpstreamMetadataCache/HigherLimitOfWorkPerFeed";
    public const int UpstreamMetadataCacheRefreshHigherLimitOfWorkPerJobDefault = 120000;
    public static readonly RegistryFrotocolLevelPackagingSettingDefinition<int> HigherLimitOfWork = new RegistryFrotocolLevelPackagingSettingDefinition<int>((Func<IProtocol, RegistryQuery>) (protocol => (RegistryQuery) ("/Configuration/Packaging/" + protocol.CorrectlyCasedName + "/UpstreamMetadataCache/HigherLimitOfWorkPerFeed")), (Func<IProtocol, FeedIdentity, RegistryQuery>) ((protocol, feed) => (RegistryQuery) string.Format("/Configuration/Packaging/{0}/UpstreamMetadataCache/HigherLimitOfWorkPerFeed/{1}", (object) protocol.CorrectlyCasedName, (object) feed.Id)), 120000, new RegistryQuery?((RegistryQuery) "/Configuration/packaging/UpstreamMetadataCache/HigherLimitOfWorkPerFeed"));
    public const string NpmjsUnitOfWorkMultiplierRegistryPath = "/Configuration/Packaging/npm/UpstreamMetadataCache/UnitOfWorkMultiplier/npmjs";
    public static readonly OrgLevelRegistrySettingDefinition<string?> TerrapinApiAuthResourceSetting = new OrgLevelRegistrySettingDefinition<string>((RegistryQuery) "/Configuration/Packaging/Terrapin/Auth/AadResource", (string) null);
    public static readonly OrgLevelRegistrySettingDefinition<string?> TerrapinApiAuthAadTenantSetting = new OrgLevelRegistrySettingDefinition<string>((RegistryQuery) "/Configuration/Packaging/Terrapin/Auth/AadTenant", (string) null);
    public static readonly OrgLevelRegistrySettingDefinition<Uri?> TerrapinApiV1BaseUriSetting = new OrgLevelRegistrySettingDefinition<Uri>((RegistryQuery) "/Configuration/Packaging/Terrapin/BaseUri", (Uri) null);
    public static readonly OrgLevelRegistrySettingDefinition<Uri?> TerrapinApiV2BaseUriSetting = new OrgLevelRegistrySettingDefinition<Uri>((RegistryQuery) "/Configuration/Packaging/TerrapinV2/BaseUri", (Uri) null);
    public const int MaxConcurrentRequestsPerProtocol = 50;
    public static readonly Guid MicrosoftTenantId = Guid.Parse("72f988bf-86f1-41af-91ab-2d7cd011db47");
    public static readonly Guid MicrosoftPMETenantId = Guid.Parse("975f013f-7f24-47e8-a7d3-abc4752bf346");
    public static readonly Guid ServiceIdentifier = Guid.Parse("00000030-0000-8888-8000-000000000000");
    public static readonly OrgLevelRegistrySettingDefinition<TimeSpan> ProblemPackageRecordingIntervalSetting = new OrgLevelRegistrySettingDefinition<TimeSpan>((RegistryQuery) "/Configuration/Packaging/ProblemPackageRecordingService/ThrottleInterval", TimeSpan.FromHours(1.0));
    public static readonly FeatureFlagPackagingSettingDefinition AllowMultipleFeedUpstreamRefreshJobRetriesSetting = new FeatureFlagPackagingSettingDefinition("Packaging.AllowMultipleFURJRetries");
    public static readonly RegistryWildcardListFrotocolLevelPackagingSettingDefinition<string> BannedCustomUpstreamHostsSetting = new RegistryWildcardListFrotocolLevelPackagingSettingDefinition<string>((Func<IProtocol, RegistryQuery>) (protocol => (RegistryQuery) ("/Configuration/Packaging/" + protocol.CorrectlyCasedName + "/BannedCustomUpstreamHosts/*")), (Func<IProtocol, FeedIdentity, RegistryQuery>) ((protocol, feed) => (RegistryQuery) string.Format("/Configuration/Packaging/{0}/BannedCustomUpstreamHostsByFeed/{1}/*", (object) protocol.CorrectlyCasedName, (object) feed.Id)));
    public const int DefaultPublishRequestTimeoutIntervalInSeconds = 1200;
    public static readonly OrgLevelRegistrySettingDefinition<int> MaxStoredTraces = new OrgLevelRegistrySettingDefinition<int>((RegistryQuery) "/Configuration/Packaging/MaxStoredTraces", 150);
    public static readonly FeatureFlagPackagingSettingDefinition ConsolidateConcurrentInlineUpstreamRefreshes = new FeatureFlagPackagingSettingDefinition("Packaging.ConsolidateConcurrentUpstreamRefreshes");
    public static readonly FeatureFlagPackagingSettingDefinition OffloadInlineUpstreamRefreshesToTasks = new FeatureFlagPackagingSettingDefinition("Packaging.OffloadInlineUpstreamRefreshesToTasks");
    public static readonly OrgLevelRegistrySettingDefinition<int> PublicUpstreamFollowerMaxInvalidations = new OrgLevelRegistrySettingDefinition<int>((RegistryQuery) "/Configuration/Packaging/PublicUpstreamFollower/MaxInvalidations", 500);
  }
}
