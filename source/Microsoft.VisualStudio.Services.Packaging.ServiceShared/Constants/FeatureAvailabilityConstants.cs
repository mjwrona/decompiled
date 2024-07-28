// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.FeatureAvailabilityConstants
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants
{
  public static class FeatureAvailabilityConstants
  {
    public const string ArtifactsPackageSizeBreakdown = "Packaging.ArtifactsPackageSizeBreakdown";
    public const string CacheVersionListWithSizeFileBetweenCommits = "Packaging.CacheVersionListWithSizeFileBetweenCommits";
    public const string DeflateVersionListWithSizeFile = "Packaging.DeflateVersionListWithSizeFile";
    public const string DeletePackageContentForDeletedFeeds = "Packaging.DeletedFeeds.DeletePackageContent";
    public const string DeletedFeedsProcessingJobEnabled = "Packaging.DeletedFeeds.EnableProcessingJob";
    public const string IgnoreDeleteDropAccessDeniedException = "Packaging.IgnoreDeleteDropAccessDeniedException";
    public const string PackagingActivityLogEnableTimingTrace = "Packaging.PackagingActivityLog.EnableTimingTrace";
    public const string InternalUpstreams = "Packaging.InternalUpstreams";
    public const string SkipCSRFValidation = "Packaging.SkipCSRFValidation";
    public const string PackageMetricsEnabled = "Packaging.PackageMetrics";
    public const string PackageMetricsWriteToFeed = "Packaging.PackageMetricsWriteToFeed";
    public const string PackageContentVerificationEnabled = "Packaging.ContentVerification";
    public const string PackageGdprCleanupJobEnabled = "Packaging.GdprCleanupJobEnabled";
    public const string UpdateUpstreamJobSchedule = "Packaging.UpdateUpstreamJobSchedule";
    public const string UpstreamStatusAT = "Packaging.UpstreamStatus.AT";
    public const string UseCachedUpstreamVersionLists = "Packaging.UseCachedUpstreamVersionLists";
    public const string WriteCachedUpstreamVersionLists = "Packaging.WriteCachedUpstreamVersionLists";
    public const string SkipFeedLevelRefreshes = "Packaging.SkipFeedLevelRefreshes";
    public const string UseExponentialBackoffForAzureBlobRetries = "Packaging.UseExponentialBackoffForAzureBlobRetries";
    public const string SBOMSigningApi = "Packaging.SBOMSigningApi";
    public const string SBOMUseDigestSigning = "Packaging.SBOMUseDigestSigning";
    public const string SBOMTelemetryApi = "Packaging.SBOMTelemetryApi";
    public const string EBOMIdBsiApi = "Packaging.EBOMIdBsiApi";
    public const string SscTaskManagementApi = "Packaging.SscTaskManagementApi";
    public const string SscServiceRepoDataJob = "Packaging.SscServiceRepoDataJob";
    public const string UseMigrationStateCache = "Packaging.UseMigrationStateCache";
    public const string UseMigrationStateDocumentPerOrgCache = "Packaging.UseMigrationStateDocumentPerOrgCache";
    public static readonly FeatureFlagPackagingSettingDefinition CallTerrapinIngestionApi = new FeatureFlagPackagingSettingDefinition("Packaging.SkipCallingTerrapinIngestionApi", invert: true);
    public static readonly FeatureFlagPackagingSettingDefinition EnforceTerrapinIngestionResults = new FeatureFlagPackagingSettingDefinition("Packaging.DontEnforceTerrapinIngestionResults", invert: true);
    public static readonly FeatureFlagPackagingSettingDefinition CallTerrapinOnRefresh = new FeatureFlagPackagingSettingDefinition("Packaging.CallTerrapinOnUpstreamRefresh");
    public const string UPackCrossOrgUpstreams = "Packaging.UPackCrossOrgUpstreams";
    public static readonly FeatureFlagPackagingSettingDefinition CompressUpstreamVersionListFeature = new FeatureFlagPackagingSettingDefinition("Packaging.CompressUpstreamVersionListDocument");
    public static readonly FeatureFlagPackagingSettingDefinition CompressProblemPackagesDocumentFeature = new FeatureFlagPackagingSettingDefinition("Packaging.CompressProblemPackagesDocument");
    public static readonly FeatureFlagPackagingSettingDefinition UpstreamsAllowedForPublicFeeds = new FeatureFlagPackagingSettingDefinition("Packaging.UpstreamsAllowedForPublicFeeds");
    public static readonly FeatureFlagPackagingSettingDefinition UpstreamsAllowedForPublicFeedsMSFT = new FeatureFlagPackagingSettingDefinition("Packaging.UpstreamsAllowedForPublicFeeds.MSFT");
    public static readonly FeatureFlagPackagingSettingDefinition PackageIngestionUsingTerrapinApiV2 = new FeatureFlagPackagingSettingDefinition("Packaging.PackageIngestionUsingTerrapinApiV2");
    public static readonly FeatureFlagPackagingSettingDefinition SyncAsyncInvokerIsPassThru = new FeatureFlagPackagingSettingDefinition("Packaging.SyncAsyncInvokerIsPassThru");
    public static readonly FeatureFlagPackagingSettingDefinition AllowStoredTraces = new FeatureFlagPackagingSettingDefinition("Packaging.AllowStoredTraces");
    public static readonly FeatureFlagPackagingSettingDefinition PushDrivenUpstreamsEnabled = new FeatureFlagPackagingSettingDefinition("Packaging.PushDrivenUpstreams.Disable", invert: true);
    public static readonly FeatureFlagPackagingSettingDefinition PublicUpstreamFollowerJobQueueingEnabled = new FeatureFlagPackagingSettingDefinition("Packaging.PublicUpstreamFollowerJobQueueing");
    public static readonly FeatureFlagPackagingSettingDefinition EnableEdgeCaching = new FeatureFlagPackagingSettingDefinition("Packaging.EdgeCaching.EnableCaching", true);
  }
}
