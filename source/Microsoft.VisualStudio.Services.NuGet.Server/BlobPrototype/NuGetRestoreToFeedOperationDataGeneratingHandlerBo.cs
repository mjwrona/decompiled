// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetRestoreToFeedOperationDataGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.OperationsData;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetRestoreToFeedOperationDataGeneratingHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, NuGetRestoreToFeedOperationData>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry> metadataHandler;

    public NuGetRestoreToFeedOperationDataGeneratingHandlerBootstrapper(
      IVssRequestContext requestContext,
      IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry> metadataHandler)
    {
      this.requestContext = requestContext;
      this.metadataHandler = metadataHandler;
    }

    public IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, NuGetRestoreToFeedOperationData> Bootstrap()
    {
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      IContentBlobStore contentBlobStore = new ContentBlobStoreFacadeBootstrapper(this.requestContext).Bootstrap();
      return (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, NuGetRestoreToFeedOperationData>) new NuGetRestoreToFeedOperationDataGeneratingHandler((IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, byte[]>) new ReadNuspecFromContentProviderHandler(this.metadataHandler, tracerFacade, new GetSeekableBlobHttpStreamHandlerBootstrapper(this.requestContext).Bootstrap(), new GetNuspecBlobIdFromDropHandler((IDropHttpClient) new DropClientFacade(this.requestContext)).ThenDelegateTo<string, BlobIdentifier, byte[]>((IAsyncHandler<BlobIdentifier, byte[]>) new GetBytesFromBlobIdHandler(contentBlobStore))));
    }
  }
}
