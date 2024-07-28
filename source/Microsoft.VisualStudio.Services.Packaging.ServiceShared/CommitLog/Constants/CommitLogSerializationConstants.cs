// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Constants.CommitLogSerializationConstants
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Constants
{
  public static class CommitLogSerializationConstants
  {
    public const string PackageNameKey = "name";
    public const string PackageVersionKey = "version";
    public const string ViewKey = "view";
    public const string MetadataSuboperationKey = "metadataSuboperation";
    public const string ProtocolOperationKey = "protocolOperation";
    public const string UpstreamRevisionToken = "UpstreamRevision";
    public const string UpstreamSource = "UpstreamSource";
    public const string Operations = "operations";
    public const string PermanentDeleteOperationName = "PermanentDelete";
    public const string CommitBookmarkCacheKey = "Packaging.OperationId";
    public const string CommitBookmarkHeaderName = "X-Packaging-OperationId";
    public const string PackageContentKey = "packageContent";
    public const string PackageSizeKey = "packageSize";
    public const string DescriptionKey = "description";
    public const string DeletedDateKey = "deletedDate";
    public const string ScheduledPermanentDeleteDateKey = "scheduledPermanentDeleteDate";
    public const string ExtraAssetsBlobReferences = "extraAssetsBlobReferences";
    public const string Provenance = "provenance";
    public const string SourceChain = "sourceChain";
    public const string PackageViews = "packageViews";
    public const string PackageBlobsDereferencedMarkerName = "packageBlobsHaveBeenDereferenced";
    public const string ProblemPackageReasons = "reasons";
    public const string StorageId = "storageId";
    public const string HashesKey = "hashes";
    public const string InnerFilesKey = "innerFiles";
    public const string AddAsDelistedKey = "addAsDelisted";
  }
}
