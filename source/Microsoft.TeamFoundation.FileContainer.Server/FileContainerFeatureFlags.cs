// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerFeatureFlags
// Assembly: Microsoft.TeamFoundation.FileContainer.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D69E508F-D51F-4EF2-B979-7E96E61DA19E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.FileContainer.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class FileContainerFeatureFlags
  {
    internal const string BrowseItemsFeature = "VisualStudio.FileContainerService.BrowseItems";
    internal const string EnableLoggingFileContainerUsage = "VisualStudio.FileContainerService.EnableLoggingFileContainerUsage";
    public const string EnableBlobReferenceBlobStoreCleanup = "VisualStudio.FileContainerService.EnableBlobReferenceBlobStoreCleanup";
    public const string EnableFileContainerUploadToBlobStore = "VisualStudio.FileContainerService.EnableFileContainerUploadToBlobStore";
    internal const string UploadBuildArtifactsToBlob = "DistributedTask.Agent.UploadBuildArtifactsToBlob";
    internal const string GZipBlobStreamsForDownload = "VisualStudio.FileContainerService.GZipBlobStreamsForDownload";
    internal const string UseLegacyBlobStitcherLibrary = "VisualStudio.FileContainerService.NewBlobStitcherLibrary";
    internal const string UseReadReplicaForBlobReferenceCleanup = "VisualStudio.FileContainerService.UseReadReplicaForBlobReferenceCleanup";
    public const string RedirectToContentStitcher = "FileContainer.RedirectToContentStitcher";
    internal const string DontSendFileIds = "FileContainer.DontSendFileIds";
    internal const string DontSendLongFileIds = "FileContainer.DontSendLongFileIds";
  }
}
