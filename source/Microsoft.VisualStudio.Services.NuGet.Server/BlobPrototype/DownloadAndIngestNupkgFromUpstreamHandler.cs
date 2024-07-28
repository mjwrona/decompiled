// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.DownloadAndIngestNupkgFromUpstreamHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class DownloadAndIngestNupkgFromUpstreamHandler : 
    IAsyncHandler<
    #nullable disable
    IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, ContentResult>,
    IHaveInputType<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>>,
    IHaveOutputType<ContentResult>
  {
    private readonly IFactory<UpstreamSource, Task<IUpstreamNuGetClient>> nuGetClientFactory;
    private readonly INuGetPackageIngestionService nuGetPackageIngestionService;
    private readonly IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, IStorageId>, Stream> byBlobIdStreamingHandler;
    private readonly ITracerService tracer;
    private readonly IAsyncHandler<FeedRequest<Stream>, FileStream> temporarilyStorePackageHandler;
    private readonly IDisposableRegistrar disposableRegistrar;
    private readonly IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler;
    private readonly ICache<string, object> requestContextItems;
    private readonly IFeedViewVisibilityValidator upstreamValidator;
    private readonly IOrgLevelPackagingSetting<bool> propagateDelistSetting;

    public DownloadAndIngestNupkgFromUpstreamHandler(
      IFactory<UpstreamSource, Task<IUpstreamNuGetClient>> nuGetClientFactory,
      INuGetPackageIngestionService nuGetPackageIngestionService,
      IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, IStorageId>, Stream> byBlobIdStreamingHandler,
      ITracerService tracer,
      IAsyncHandler<FeedRequest<Stream>, FileStream> temporarilyStorePackageHandler,
      IDisposableRegistrar disposableRegistrar,
      IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler,
      ICache<string, object> requestContextItems,
      IFeedViewVisibilityValidator upstreamValidator,
      IOrgLevelPackagingSetting<bool> propagateDelistSetting)
    {
      this.nuGetClientFactory = nuGetClientFactory;
      this.nuGetPackageIngestionService = nuGetPackageIngestionService;
      this.byBlobIdStreamingHandler = byBlobIdStreamingHandler;
      this.tracer = tracer;
      this.temporarilyStorePackageHandler = temporarilyStorePackageHandler;
      this.disposableRegistrar = disposableRegistrar;
      this.upstreamsFromFeedHandler = upstreamsFromFeedHandler;
      this.requestContextItems = requestContextItems;
      this.upstreamValidator = upstreamValidator;
      this.propagateDelistSetting = propagateDelistSetting;
    }

    public async Task<ContentResult> Handle(
      IPackageFileRequest<VssNuGetPackageIdentity, IStorageId> request)
    {
      DownloadAndIngestNupkgFromUpstreamHandler sendInTheThisObject = this;
      UpstreamStorageId additionalData1 = request.AdditionalData as UpstreamStorageId;
      TryAllUpstreamsStorageId additionalData2 = request.AdditionalData as TryAllUpstreamsStorageId;
      if (additionalData1 == null && additionalData2 == null)
        return (ContentResult) null;
      UpstreamSourceInfo upstreamContentSource = additionalData1?.UpstreamContentSource;
      IEnumerable<UpstreamSource> upstreamSources = sendInTheThisObject.GetValidUpstreamSources((IFeedRequest) request, upstreamContentSource);
      if (upstreamSources == null || !upstreamSources.Any<UpstreamSource>())
        throw new ArgumentException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_UpstreamsEmpty(), "AdditionalData");
      using (ITracerBlock traceBlock = sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        foreach (UpstreamSource upstreamSource in upstreamSources)
        {
          await sendInTheThisObject.upstreamValidator.Validate((IFeedRequest) request, upstreamSource);
          IUpstreamNuGetClient client = await sendInTheThisObject.nuGetClientFactory.Get(upstreamSource);
          try
          {
            NuGetUpstreamMetadata upstreamMetadata = await client.GetUpstreamMetadata((IFeedRequest) request, request.PackageId);
            List<UpstreamSourceInfo> sourceChain = new List<UpstreamSourceInfo>((IEnumerable<UpstreamSourceInfo>) upstreamMetadata.SourceChain ?? Enumerable.Empty<UpstreamSourceInfo>());
            sourceChain.Insert(0, UpstreamSourceInfoUtils.CreateUpstreamSourceInfo(upstreamSource));
            sendInTheThisObject.requestContextItems.Set("Packaging.Properties.PackageSource", (object) "upstream");
            sendInTheThisObject.requestContextItems.Set("Packaging.Properties.DirectUpstreamSourceId", (object) upstreamSource.Id);
            bool addAsDelisted = sendInTheThisObject.propagateDelistSetting.Get() && !upstreamMetadata.Listed;
            if (upstreamMetadata.StorageId is BlobStorageId blobStorageId)
            {
              await sendInTheThisObject.nuGetPackageIngestionService.AddPackageFromBlobAsync((IFeedRequest) request, new BlobIdentifierWithOrWithoutBlocks(blobStorageId.BlobId), "v2", (IEnumerable<UpstreamSourceInfo>) sourceChain, addAsDelisted, request.PackageId);
              return new ContentResult(await sendInTheThisObject.byBlobIdStreamingHandler.Handle(new PackageRequest<VssNuGetPackageIdentity, IStorageId>((IPackageRequest<VssNuGetPackageIdentity>) request, (IStorageId) blobStorageId)), request.FilePath);
            }
            using (Stream stream = await client.GetNupkg((IFeedRequest) request, request.PackageId))
            {
              FileStream fs;
              using (traceBlock.CreateTimeToFirstPageExclusionBlock())
                fs = await sendInTheThisObject.temporarilyStorePackageHandler.Handle(new FeedRequest<Stream>((IFeedRequest) request, stream));
              sendInTheThisObject.disposableRegistrar.DisposeAtEndOfRequest((IDisposable) fs);
              await sendInTheThisObject.nuGetPackageIngestionService.AddPackageFromStreamAsync((IFeedRequest) request, (Stream) fs, "v2", (IEnumerable<UpstreamSourceInfo>) sourceChain, addAsDelisted, request.PackageId);
              fs.Seek(0L, SeekOrigin.Begin);
              return new ContentResult((Stream) fs, request.FilePath);
            }
          }
          catch (Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException ex)
          {
          }
          client = (IUpstreamNuGetClient) null;
        }
        throw new Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageNotFoundInUpstreams((object) request.PackageId.Name.NormalizedName, (object) string.Join(",", upstreamSources.Select<UpstreamSource, string>((Func<UpstreamSource, string>) (s => s.Location))), (object) request.Feed.FullyQualifiedName));
      }
    }

    private IEnumerable<UpstreamSource> GetValidUpstreamSources(
      IFeedRequest feedRequest,
      UpstreamSourceInfo source)
    {
      IEnumerable<UpstreamSource> source1 = this.upstreamsFromFeedHandler.Handle(feedRequest);
      if (source == null)
        return source1;
      UpstreamSource upstreamSource = source1.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (s => s.Id == source.Id));
      if (upstreamSource == null)
        return source1;
      return (IEnumerable<UpstreamSource>) new UpstreamSource[1]
      {
        upstreamSource
      };
    }
  }
}
