// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.DownloadAndIngestPyPiFileFromUpstreamHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
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
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class DownloadAndIngestPyPiFileFromUpstreamHandler : 
    IAsyncHandler<
    #nullable disable
    IPackageFileRequest<PyPiPackageIdentity, IStorageId>, ContentResult>,
    IHaveInputType<IPackageFileRequest<PyPiPackageIdentity, IStorageId>>,
    IHaveOutputType<ContentResult>
  {
    private readonly IFactory<UpstreamSource, Task<IUpstreamPyPiClient>> clientFactory;
    private readonly IAsyncHandler<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, NullResult> ingestionHandler;
    private readonly ITracerService tracer;
    private readonly IAsyncHandler<FeedRequest<Stream>, FileStream> temporarilyStorePackageHandler;
    private readonly IDisposableRegistrar disposableRegistrar;
    private readonly IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler;
    private readonly IFactory<bool> shouldBlockWriteOperationsFactory;
    private readonly ICache<string, object> requestContextItems;
    private readonly IFeedViewVisibilityValidator upstreamValidator;

    public DownloadAndIngestPyPiFileFromUpstreamHandler(
      IFactory<UpstreamSource, Task<IUpstreamPyPiClient>> clientFactory,
      IAsyncHandler<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, NullResult> ingestionHandler,
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

    public async Task<ContentResult> Handle(
      IPackageFileRequest<PyPiPackageIdentity, IStorageId> request)
    {
      DownloadAndIngestPyPiFileFromUpstreamHandler sendInTheThisObject = this;
      UpstreamStorageId additionalData1 = request.AdditionalData as UpstreamStorageId;
      TryAllUpstreamsStorageId additionalData2 = request.AdditionalData as TryAllUpstreamsStorageId;
      if (additionalData1 == null && additionalData2 == null)
        return (ContentResult) null;
      if (sendInTheThisObject.shouldBlockWriteOperationsFactory.Get())
        throw new PotentiallyDangerousRequestException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_UpstreamIngestion_CannotSkipIngestion());
      UpstreamSourceInfo upstreamContentSource = additionalData1?.UpstreamContentSource;
      IEnumerable<UpstreamSource> upstreamSources = sendInTheThisObject.GetValidUpstreamSources((IFeedRequest) request, upstreamContentSource);
      if (upstreamSources == null || !upstreamSources.Any<UpstreamSource>())
        throw new ArgumentException("expected a valid upstream. found none.", "AdditionalData");
      using (ITracerBlock traceBlock = sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        foreach (UpstreamSource upstreamSource in upstreamSources)
        {
          await sendInTheThisObject.upstreamValidator.Validate((IFeedRequest) request, upstreamSource);
          IUpstreamPyPiClient client = await sendInTheThisObject.clientFactory.Get(upstreamSource);
          PyPiUpstreamMetadata upstreamMetadata;
          try
          {
            upstreamMetadata = await client.GetUpstreamMetadata((IFeedRequest) request, request.PackageId, request.FilePath);
          }
          catch (Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException ex)
          {
            continue;
          }
          if (upstreamMetadata.RawFileMetadata == null)
            throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageFileNotFoundInUpstreams((object) request.FilePath, (object) request.PackageId, (object) upstreamSource.Location, (object) request.Feed.FullyQualifiedName));
          try
          {
            using (Stream packageStream = await client.GetFile((IFeedRequest) request, request.PackageId, request.FilePath))
            {
              using (Stream gpgStream = await client.GetGpgSignatureForFile((IFeedRequest) request, request.PackageId, request.FilePath))
              {
                FileStream gpgFileStream = (FileStream) null;
                FileStream packageFileStream;
                using (traceBlock.CreateTimeToFirstPageExclusionBlock())
                {
                  packageFileStream = await sendInTheThisObject.temporarilyStorePackageHandler.Handle(new FeedRequest<Stream>((IFeedRequest) request, packageStream));
                  sendInTheThisObject.disposableRegistrar.DisposeAtEndOfRequest((IDisposable) packageFileStream);
                  if (gpgStream != null)
                  {
                    gpgFileStream = await sendInTheThisObject.temporarilyStorePackageHandler.Handle(new FeedRequest<Stream>((IFeedRequest) request, gpgStream));
                    if (gpgFileStream.Length == 0L)
                    {
                      gpgFileStream.Dispose();
                      gpgFileStream = (FileStream) null;
                    }
                    else
                      sendInTheThisObject.disposableRegistrar.DisposeAtEndOfRequest((IDisposable) gpgFileStream);
                  }
                }
                ImmutableArray<UpstreamSourceInfo> immutableArray = upstreamMetadata.SourceChain.Insert(0, UpstreamSourceInfoUtils.CreateUpstreamSourceInfo(upstreamSource));
                sendInTheThisObject.requestContextItems.Set("Packaging.Properties.PackageSource", (object) "upstream");
                sendInTheThisObject.requestContextItems.Set("Packaging.Properties.DirectUpstreamSourceId", (object) upstreamSource.Id);
                PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData> request1 = new PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>((IFeedRequest) request, new PackageIngestionFormData(upstreamMetadata.RawFileMetadata, new PackageFileStream(request.FilePath, packageFileStream.Length, (Stream) packageFileStream, (BlobIdentifier) null), gpgFileStream != null ? new PackageFileStream(request.FilePath + ".asc", gpgFileStream.Length, (Stream) gpgFileStream, (BlobIdentifier) null) : (PackageFileStream) null), "1")
                {
                  SourceChain = (IEnumerable<UpstreamSourceInfo>) immutableArray,
                  ExpectedIdentity = request.PackageId
                };
                NullResult nullResult = await sendInTheThisObject.ingestionHandler.Handle(request1);
                packageFileStream.Seek(0L, SeekOrigin.Begin);
                return new ContentResult((Stream) packageFileStream, request.FilePath);
              }
            }
          }
          catch (Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException ex)
          {
          }
          client = (IUpstreamPyPiClient) null;
          upstreamMetadata = (PyPiUpstreamMetadata) null;
        }
        throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageFileNotFoundInUpstreams((object) request.FilePath, (object) request.PackageId, (object) string.Join(",", upstreamSources.Select<UpstreamSource, string>((Func<UpstreamSource, string>) (s => s.Location))), (object) request.Feed.FullyQualifiedName));
      }
    }

    private IEnumerable<UpstreamSource> GetValidUpstreamSources(
      IFeedRequest feedRequest,
      UpstreamSourceInfo source)
    {
      IEnumerable<UpstreamSource> source1 = this.upstreamsFromFeedHandler.Handle(feedRequest);
      if (source == null)
        return source1;
      UpstreamSource upstreamSource = source1 != null ? source1.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (s => s.Id == source.Id)) : (UpstreamSource) null;
      if (upstreamSource == null)
        return source1;
      return (IEnumerable<UpstreamSource>) new UpstreamSource[1]
      {
        upstreamSource
      };
    }
  }
}
