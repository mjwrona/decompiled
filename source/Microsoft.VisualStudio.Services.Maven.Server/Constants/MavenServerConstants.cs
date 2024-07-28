// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Constants.MavenServerConstants
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server.Constants
{
  public static class MavenServerConstants
  {
    public const string ProtocolName = "maven";
    public const string FeedIndexProtocolType = "maven";
    public const string ProtocolVersion = "1.0";
    public const int SchemaVersion = 1;
    public const string FeedChangeProcessingJobExtensionName = "Microsoft.VisualStudio.Services.Maven.Server.Plugins.ChangeProcessing.MavenFeedChangeProcessingJob";
    public const string FeedContentVerificationJobExtensionName = "Microsoft.VisualStudio.Services.Maven.Server.Plugins.ContentVerification.MavenFeedContentVerificationJob";
    public const string FeedSnapshotCleanupJobExtensionName = "Microsoft.VisualStudio.Services.Maven.Server.Plugins.SnapshotCleanup.MavenFeedSnapshotCleanupJob";
    public const string AlternateMetadataKey = "/Configuration/Packaging/Maven/AlternateMetadata";
    public const string SnapshotCleanupLogContainerPath = "maven-snapshot-cleanup-log";
    public const string ProcessedSnapshotCleanupLogContainerPath = "snapshotcleanupcompleted";
    public const string SnapshotRetentionRotationTargetCountPath = "/Configuration/Packaging/Maven/SnapshotRetentionRotationTargetCount";
    public const int DefaultSnapshotRotationTargetCount = 30;
    public const long MaximumPomSizeDefault = 524288;
    public static readonly Guid CollectionChangeProcessingJobId = Guid.Parse("981C120A-E2E4-4E15-B956-3C42E20F14A5");
    public const string MaxPomSizeRegistryPath = "/Configuration/Packaging/Maven/Ingestion/Pom/MaxSize";
    public static readonly BookmarkTokenKey SnapshotCleanupBookmarkTokenKey = new BookmarkTokenKey("maven", "feed", "SnapshotCleanupLastUpdatedBookmarkToken", 1);
    private const string SnapshotCleanupBookmarkTokenName = "SnapshotCleanupLastUpdatedBookmarkToken";
    private const int SnapshotCleanupBookmarkTokenVersion = 1;
    internal const string MetadataLastUpdatedFormat = "yyyyMMddHHmmss";

    public static class ChangeProcessingJobConstants
    {
      public static readonly JobCreationInfo MavenChangeProcessingJobCreationInfo = new JobCreationInfo("FeedChangeProcessingJob", "Microsoft.VisualStudio.Services.Maven.Server.Plugins.ChangeProcessing.MavenFeedChangeProcessingJob", TeamFoundationHostType.ProjectCollection);
    }

    public static class RecoveryJobConstants
    {
      public static readonly Guid RecoveryMasterJobId = Guid.Parse("DEBC9A99-70CB-4049-9D81-4E4742263341");
      public static readonly JobCreationInfo RecoveryWorkerJobCreationInfo = new JobCreationInfo("MavenRecoveryWorkerJob", "Microsoft.VisualStudio.Services.Maven.Server.Plugins.RecoveryJobs.MavenRecoveryWorkerJob", TeamFoundationHostType.ProjectCollection, JobPriorityClass.High);
    }

    public static class ContentVerificationJobConstants
    {
      public static readonly JobCreationInfo MavenContentVerificationCreationInfo = new JobCreationInfo("MavenFeedContentVerificationJob", "Microsoft.VisualStudio.Services.Maven.Server.Plugins.ContentVerification.MavenFeedContentVerificationJob", TeamFoundationHostType.ProjectCollection);
      public static readonly Guid CollectionContentVerificationJobId = Guid.Parse("6D01E16C-8AFD-4101-964C-2FED9F08B2E2");
    }
  }
}
