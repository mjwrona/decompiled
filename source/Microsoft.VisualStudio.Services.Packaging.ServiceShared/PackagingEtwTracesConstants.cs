// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackagingEtwTracesConstants
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public static class PackagingEtwTracesConstants
  {
    public const string Protocol = "Packaging.Protocol";
    public const string Feed = "Packaging.Feed";
    public const string ClientSessionId = "Packaging.ClientSessionId";
    public const string PackageName = "Packaging.PackageName";
    public const string PackageIdentity = "Packaging.PackageIdentity";
    public const string PackageStorageId = "Packaging.PackageStorageId";
    public const string DataCurrentVersion = "Packaging.DataCurrentVersion";
    public const string DataDestinationVersion = "Packaging.DataDestinationVersion";
    public const string DataMigrationState = "Packaging.DataMigrationState";
    public const string ConsultedFeatureFlags = "Packaging.ConsultedFeatureFlags";
    public const string Properties = "Packaging.Properties";
    public const string SubmittedCommitBookmarkProperty = "Packaging.Properties.SubmittedCommitBookmark";
    public const string CommitBatchSizeProperty = "Packaging.Properties.CommitBatchSize";
    public const string CommitOperationTypeProperty = "Packaging.Properties.CommitOperationType";
    public const string CommitCountProperty = "Packaging.Properties.CommitCount";
    public const string OperationCountProperty = "Packaging.Properties.OperationCount";
    public const string BatchCommitVersionedPackageNamesProperty = "Packaging.Properties.BatchCommitVersionedPackageNames";
    public const string MetadataReadsCountProperty = "Packaging.Properties.MetadataReadsCount";
    public const string MetadataReadZipProperty = "Packaging.Properties.MetadataReadZip";
    public const string MetadataLengthSampleProperty = "Packaging.Properties.MetadataLengthSample";
    public const string MetadataWriteZipProperty = "Packaging.Properties.MetadataWriteZip";
    public const string MetadataWritesCountProperty = "Packaging.Properties.MetadataWritesCount";
    public const string MetadataLastBlobSizeProperty = "Packaging.Properties.MetadataLastBlobSize";
    public const string UpstreamMetadataLastBlobSizeProperty = "Packaging.Properties.UpstreamMetadataLastBlobSize";
    public const string VersionCountReadProperty = "Packaging.Properties.VersionsRead";
    public const string VersionCountSeenProperty = "Packaging.Properties.VersionsSeen";
    public const string VersionCountMatchedProperty = "Packaging.Properties.VersionsMatched";
    public const string VersionCountWrittenProperty = "Packaging.Properties.VersionsWritten";
    public const string LocalVersionCountReadProperty = "Packaging.Properties.LocalVersionsRead";
    public const string LocalVersionCountMatchedProperty = "Packaging.Properties.LocalVersionsMatched";
    public const string FilesCountProperty = "Packaging.Properties.FilesCount";
    public const string PackageSourceProperty = "Packaging.Properties.PackageSource";
    public const string DirectUpstreamSourceIdProperty = "Packaging.Properties.DirectUpstreamSourceId";
    public const string OriginUpstreamSourceTypeProperty = "Packaging.Properties.OriginUpstreamSourceType";
    public const string ForcedMavenCentralRetrieval = "Packaging.Properties.ForcedMavenCentralRetrieval";
    public const string PackageStorageTypeProperty = "Packaging.Properties.PackageStorageType";
    public const string UpstreamRefreshCount = "UpstreamRefreshCount";
    public const string UpstreamVersionRefreshCount = "UpstreamVersionRefreshCount";
    public const string DataMigrationIsDeprecated = "Packaging.Properties.DataMigrationIsDeprecated";
    public const string CommitDidNotApply = "Packaging.Properties.CommitDidNotApply";
    public const string ProtocolAndFeedTracePointTemplate = "Packaging.FeedTracePoint.{0}.{1}";
    public const string FeedId = "Packaging.FeedId";
  }
}
