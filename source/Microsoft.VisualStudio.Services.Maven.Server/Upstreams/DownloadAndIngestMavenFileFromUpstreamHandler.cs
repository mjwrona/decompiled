// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.DownloadAndIngestMavenFileFromUpstreamHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.Internal;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
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

namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class DownloadAndIngestMavenFileFromUpstreamHandler : 
    IAsyncHandler<IPackageFileRequest<MavenPackageIdentity, IStorageId>, ContentResult>,
    IHaveInputType<IPackageFileRequest<MavenPackageIdentity, IStorageId>>,
    IHaveOutputType<ContentResult>
  {
    private readonly IFactory<UpstreamSource, Task<IUpstreamMavenClient>> clientFactory;
    private readonly IAsyncHandler<PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>> ingestionHandler;
    private readonly ITracerService tracer;
    private readonly IAsyncHandler<FeedRequest<Stream>, FileStream> temporarilyStorePackageHandler;
    private readonly IDisposableRegistrar disposableRegistrar;
    private readonly IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler;
    private readonly IFactory<bool> shouldBlockWriteOperationsFactory;
    private readonly ICache<string, object> requestContextItems;
    private readonly IFeedViewVisibilityValidator upstreamValidator;

    public DownloadAndIngestMavenFileFromUpstreamHandler(
      IFactory<UpstreamSource, Task<IUpstreamMavenClient>> clientFactory,
      IAsyncHandler<PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>> ingestionHandler,
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
      IPackageFileRequest<MavenPackageIdentity, IStorageId> request)
    {
      DownloadAndIngestMavenFileFromUpstreamHandler sendInTheThisObject = this;
      UpstreamStorageId additionalData1 = request.AdditionalData as UpstreamStorageId;
      TryAllUpstreamsStorageId additionalData2 = request.AdditionalData as TryAllUpstreamsStorageId;
      if (additionalData1 == null && additionalData2 == null)
        return (ContentResult) null;
      if (sendInTheThisObject.shouldBlockWriteOperationsFactory.Get())
        throw new PotentiallyDangerousRequestException(Microsoft.VisualStudio.Services.Maven.Server.Resources.Error_UpstreamIngestion_CannotSkipIngestion());
      UpstreamSourceInfo upstreamContentSource = additionalData1?.UpstreamContentSource;
      IEnumerable<UpstreamSource> upstreamSources = sendInTheThisObject.GetValidUpstreamSources((IFeedRequest) request, upstreamContentSource);
      if (upstreamSources == null || !upstreamSources.Any<UpstreamSource>())
        throw new ArgumentException("expected a valid upstream. found none.", "AdditionalData");
      using (ITracerBlock traceBlock = sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        foreach (UpstreamSource upstreamSource in upstreamSources)
        {
          await sendInTheThisObject.upstreamValidator.Validate((IFeedRequest) request, upstreamSource);
          IUpstreamMavenClient client = await sendInTheThisObject.clientFactory.Get(upstreamSource);
          try
          {
            IMavenArtifactFilePath filePath = (IMavenArtifactFilePath) new MavenArtifactFilePath(request.PackageId.Name.GroupId, request.PackageId.Name.ArtifactId, request.PackageId.Version.DisplayVersion, request.FilePath);
            using (Stream packageStream = await client.GetFileAsync(filePath))
            {
              FileStream packageFileStream;
              using (traceBlock.CreateTimeToFirstPageExclusionBlock())
              {
                packageFileStream = await sendInTheThisObject.temporarilyStorePackageHandler.Handle(new FeedRequest<Stream>((IFeedRequest) request, packageStream));
                sendInTheThisObject.disposableRegistrar.DisposeAtEndOfRequest((IDisposable) packageFileStream);
              }
              sendInTheThisObject.requestContextItems.Set("Packaging.Properties.PackageSource", (object) "upstream");
              sendInTheThisObject.requestContextItems.Set("Packaging.Properties.DirectUpstreamSourceId", (object) upstreamSource.Id);
              List<UpstreamSourceInfo> list = (await client.GetSourceChainAsync((IMavenFullyQualifiedFilePath) filePath)).ToList<UpstreamSourceInfo>();
              list.Insert(0, UpstreamSourceInfoUtils.CreateUpstreamSourceInfo(upstreamSource));
              PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo> request1 = new PackageIngestionRequest<MavenPackageIdentity, MavenPackageFileInfo>((IFeedRequest) request, new MavenPackageFileInfo((Stream) packageFileStream, filePath), Protocol.Maven.V1)
              {
                SourceChain = (IEnumerable<UpstreamSourceInfo>) list,
                ExpectedIdentity = request.PackageId
              };
              NullResult nullResult = await sendInTheThisObject.ingestionHandler.Handle(request1);
              packageFileStream.Seek(0L, SeekOrigin.Begin);
              return new ContentResult((Stream) packageFileStream, filePath.FileName);
            }
          }
          catch (Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException ex)
          {
          }
          client = (IUpstreamMavenClient) null;
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
