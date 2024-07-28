// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.DownloadAndIngestCrateFileFromUpstreamHandler
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams
{
  public class DownloadAndIngestCrateFileFromUpstreamHandler : 
    IAsyncHandler<IPackageFileRequest<CargoPackageIdentity, IStorageId>, ContentResult?>,
    IHaveInputType<IPackageFileRequest<CargoPackageIdentity, IStorageId>>,
    IHaveOutputType<ContentResult?>
  {
    private readonly IFactory<UpstreamSource, Task<IUpstreamCargoClient>> clientFactory;
    private readonly IAsyncHandler<PackageIngestionRequest<CargoPackageIdentity, CargoIngestionInput>, NullResult> ingestionHandler;
    private readonly ITracerService tracer;
    private readonly IAsyncHandler<FeedRequest<Stream>, FileStream> temporarilyStorePackageHandler;
    private readonly IDisposableRegistrar disposableRegistrar;
    private readonly IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler;
    private readonly IFactory<bool> shouldBlockWriteOperationsFactory;
    private readonly ICache<string, object> requestContextItems;
    private readonly IFeedViewVisibilityValidator upstreamValidator;

    public DownloadAndIngestCrateFileFromUpstreamHandler(
      IFactory<UpstreamSource, Task<IUpstreamCargoClient>> clientFactory,
      IAsyncHandler<PackageIngestionRequest<CargoPackageIdentity, CargoIngestionInput>, NullResult> ingestionHandler,
      ITracerService tracer,
      IAsyncHandler<FeedRequest<Stream>, FileStream> temporarilyStorePackageHandler,
      IDisposableRegistrar disposableRegistrar,
      IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler,
      IFactory<bool> shouldBlockWriteOperationsFactory,
      ICache<string, object> requestContextItems,
      IFeedViewVisibilityValidator upstreamValidator)
    {
      this.clientFactory = clientFactory;
      this.ingestionHandler = ingestionHandler;
      this.tracer = tracer;
      this.temporarilyStorePackageHandler = temporarilyStorePackageHandler;
      this.disposableRegistrar = disposableRegistrar;
      this.upstreamsFromFeedHandler = upstreamsFromFeedHandler;
      this.shouldBlockWriteOperationsFactory = shouldBlockWriteOperationsFactory;
      this.requestContextItems = requestContextItems;
      this.upstreamValidator = upstreamValidator;
    }

    public async Task<ContentResult?> Handle(
      IPackageFileRequest<CargoPackageIdentity, IStorageId> request)
    {
      DownloadAndIngestCrateFileFromUpstreamHandler sendInTheThisObject = this;
      UpstreamStorageId additionalData1 = request.AdditionalData as UpstreamStorageId;
      TryAllUpstreamsStorageId additionalData2 = request.AdditionalData as TryAllUpstreamsStorageId;
      if (additionalData1 == null && additionalData2 == null)
        return (ContentResult) null;
      if (sendInTheThisObject.shouldBlockWriteOperationsFactory.Get())
        throw new PotentiallyDangerousRequestException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_UpstreamIngestion_CannotSkipIngestion());
      IEnumerable<UpstreamSource> upstreamSources = sendInTheThisObject.GetValidUpstreamSources((IFeedRequest) request, additionalData1?.UpstreamContentSource ?? throw new ArgumentException("expected a valid upstream. found none.", "AdditionalData"));
      if (upstreamSources == null || !upstreamSources.Any<UpstreamSource>())
        throw new ArgumentException("expected a valid upstream. found none.", "AdditionalData");
      using (ITracerBlock traceBlock = sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        foreach (UpstreamSource upstreamSource in upstreamSources)
        {
          await sendInTheThisObject.upstreamValidator.Validate((IFeedRequest) request, upstreamSource);
          IUpstreamCargoClient client = await sendInTheThisObject.clientFactory.Get(upstreamSource);
          CargoUpstreamMetadata upstreamMetadata;
          try
          {
            upstreamMetadata = await client.GetUpstreamMetadata(request.PackageId);
          }
          catch (Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException ex)
          {
            continue;
          }
          try
          {
            using (Stream packageStream = await client.GetPackageContentStreamAsync(request.PackageId))
            {
              List<UpstreamSourceInfo> sourceChain = upstreamMetadata.SourceChain.ToList<UpstreamSourceInfo>();
              sourceChain.Insert(0, UpstreamSourceInfoUtils.CreateUpstreamSourceInfo(upstreamSource));
              sendInTheThisObject.requestContextItems.Set("Packaging.Properties.PackageSource", (object) "upstream");
              sendInTheThisObject.requestContextItems.Set("Packaging.Properties.DirectUpstreamSourceId", (object) upstreamSource.Id);
              if (packageStream != null)
              {
                FileStream fs;
                using (traceBlock.CreateTimeToFirstPageExclusionBlock())
                  fs = await sendInTheThisObject.temporarilyStorePackageHandler.Handle(new FeedRequest<Stream>((IFeedRequest) request, packageStream));
                sendInTheThisObject.disposableRegistrar.DisposeAtEndOfRequest((IDisposable) fs);
                PackageIngestionRequest<CargoPackageIdentity, CargoIngestionInput> request1 = new PackageIngestionRequest<CargoPackageIdentity, CargoIngestionInput>((IFeedRequest) request, new CargoIngestionInput(upstreamMetadata.IndexRow, upstreamMetadata.PublishManifest, upstreamMetadata.StorageId, (Stream) fs), "1")
                {
                  SourceChain = (IEnumerable<UpstreamSourceInfo>) sourceChain,
                  ExpectedIdentity = request.PackageId
                };
                NullResult nullResult = await sendInTheThisObject.ingestionHandler.Handle(request1);
                fs.Seek(0L, SeekOrigin.Begin);
                return new ContentResult((Stream) fs, request.FilePath);
              }
              continue;
            }
          }
          catch (Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException ex)
          {
          }
          client = (IUpstreamCargoClient) null;
          upstreamMetadata = (CargoUpstreamMetadata) null;
        }
        throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageNotFoundInUpstreams((object) request.PackageId, (object) string.Join(",", upstreamSources.Select<UpstreamSource, string>((Func<UpstreamSource, string>) (s => s.Location))), (object) request.Feed.FullyQualifiedName));
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
