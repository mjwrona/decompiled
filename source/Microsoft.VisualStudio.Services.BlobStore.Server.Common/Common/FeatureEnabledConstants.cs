// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.FeatureEnabledConstants
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public static class FeatureEnabledConstants
  {
    public const string DedupStoreAzureFrontDoor = "BlobStore.Features.AzureBlobDedupProviderAzureFrontDoor";
    public const string BlobStoreAzureFrontDoor = "BlobStore.Features.BlobStoreAzureFrontDoor";
    public const string ClientToolAzureFrontDoor = "BlobStore.Features.ClientToolAzureFrontDoor";
    public const string DedupStoreDebug = "BlobStore.Features.DedupStoreDebug";
    public const string BlobStoreShardMap = "BlobStore.Features.ShardMap";
    public const string StorageVolumeQuotaCap = "BlobStore.Features.StorageVolumeQuotaCap";
    public const string BucketKeepUntil = "BlobStore.Features.BucketKeepUntilByDay";
    public const string MultiDomainOperations = "Blobstore.Features.MultiDomainOperations";
    public const string EnableMultiDomainBilling = "BlobStore.Features.EnableMultiDomainBilling";
    public const string EnableFutureKeepUntilMarking = "BlobStore.Features.MarkWithFutureKeepUntil";
    public const string AlwaysMarkKeepUntil = "BlobStore.Features.AlwaysMarkKeepUntil";
    public const string AllowDeletionOfDanglingPARoots = "BlobStore.Features.AllowDeletionOfDanglingPARoots";
    public const string EnableGcOperationTimeout = "BlobStore.Features.EnableGcOperationTimeout";
    public const string AzureBlobTelemetry = "Blobstore.Features.AzureBlobTelemetry";
    public const string EnablePackageSizeBreakdownInBillingUI = "BlobStore.Features.EnablePackageSizeBreakdownInBillingUI";
    public const string EnableSimpleDedupGCJob = "BlobStore.Features.EnableSimpleDedupGCJob";
    public const string EnableChunkDedupGCCheckpoints = "BlobStore.Features.EnableChunkDedupGCCheckpoints";
    public const string EnableSweepInSimplifiedDedupGC = "BlobStore.Features.EnableSweepInSimplifiedDedupGC";
    public const string EnableCargoBilling = "BlobStore.Features.EnableCargoBilling";
    public const string EnableStorageMetricsTransactionPopulatorJob = "BlobStore.Features.EnableStorageMetricsTransactionPopulatorJob";
    public const string EnableSMTPopulatorTelemetryPublishing = "BlobStore.Features.EnableSMTPopulatorTelemetryPublishing";
    public const string EnableCopyViaJobAgent = "BlobStore.Features.EnableCopyViaJobAgent";
    public const string ServerDedup = "Blobstore.Features.ServerDedup";
    public const string ServerDedupAllowDirectCalls = "Blobstore.Features.ServerDedupAllowDirectCalls";
    public const string EnableContentStitcher = "Blobstore.Features.EnableContentStitcher";
    public const string ProjectDomains = "Blobstore.Features.ProjectDomains";
    public const string ThrowOnContentLengthMismatch = "Blobstore.Features.ThrowOnContentLengthMismatch";
    public const string BypassExceptionIfHashRetryMatches = "Blobstore.Features.BypassExceptionIfHashRetryMatches";

    public static class PermanentFeatures
    {
      public const string PackagingDataExport = "BlobStore.Features.DataExport.Packaging";
      public const string SymbolDataExport = "BlobStore.Features.DataExport.Symbols";
    }
  }
}
