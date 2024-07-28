// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.AddStorageInfoToPackageFileRequestHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public class AddStorageInfoToPackageFileRequestHandler<TPackageId, TMetadataEntry> : 
    IAsyncHandler<
    #nullable disable
    IPackageFileRequest<TPackageId>, IPackageFileRequest<TPackageId, IStorageId>>,
    IHaveInputType<IPackageFileRequest<TPackageId>>,
    IHaveOutputType<IPackageFileRequest<TPackageId, IStorageId>>
    where TPackageId : IPackageIdentity
    where TMetadataEntry : class, IMetadataEntry<TPackageId>, IPackageFiles
  {
    private readonly IMetadataCacheService cacheService;
    private readonly IReadMetadataDocumentService<TPackageId, TMetadataEntry> metadataService;
    private readonly IUpstreamEntriesValidChecker upstreamEntriesStillValidChecker;
    private readonly ITracerService tracer;
    private readonly ICache<string, object> requestContextItems;
    private readonly IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler;
    private readonly IValidator<IPackageRequest> blockedIdentityValidator;

    public AddStorageInfoToPackageFileRequestHandler(
      IMetadataCacheService cacheService,
      IReadMetadataDocumentService<TPackageId, TMetadataEntry> metadataService,
      IUpstreamEntriesValidChecker upstreamEntriesStillValidChecker,
      ITracerService tracer,
      ICache<string, object> requestContextItems,
      IHandler<IFeedRequest, IEnumerable<UpstreamSource>> upstreamsFromFeedHandler,
      IValidator<IPackageRequest> blockedIdentityValidator)
    {
      this.cacheService = cacheService;
      this.metadataService = metadataService;
      this.upstreamEntriesStillValidChecker = upstreamEntriesStillValidChecker;
      this.tracer = tracer;
      this.requestContextItems = requestContextItems;
      this.upstreamsFromFeedHandler = upstreamsFromFeedHandler;
      this.blockedIdentityValidator = blockedIdentityValidator;
    }

    public async Task<IPackageFileRequest<TPackageId, IStorageId>> Handle(
      IPackageFileRequest<TPackageId> request)
    {
      AddStorageInfoToPackageFileRequestHandler<TPackageId, TMetadataEntry> sendInTheThisObject = this;
      IPackageFileRequest<TPackageId, IStorageId> packageFileRequest;
      using (sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        sendInTheThisObject.blockedIdentityValidator.Validate((IPackageRequest) request);
        ICachablePackageMetadata packageMetadata;
        if (!sendInTheThisObject.cacheService.TryGetPackageMetadata((IPackageFileRequest) request, out packageMetadata))
        {
          packageMetadata = await sendInTheThisObject.CalculateStorageMetadata(request);
          sendInTheThisObject.cacheService.SetPackageMetadata((IPackageFileRequest) request, packageMetadata);
        }
        if (!(packageMetadata.StorageId is TryAllUpstreamsStorageId))
          sendInTheThisObject.requestContextItems.AddDownloadTelemetry(packageMetadata);
        sendInTheThisObject.requestContextItems.Set("Packaging.PackageStorageId", (object) packageMetadata.StorageId.NonLegacyValueString);
        packageFileRequest = (IPackageFileRequest<TPackageId, IStorageId>) new PackageFileRequest<TPackageId, IStorageId>(request, packageMetadata.StorageId);
      }
      return packageFileRequest;
    }

    private async Task<ICachablePackageMetadata> CalculateStorageMetadata(
      IPackageFileRequest<TPackageId> request)
    {
      MetadataDocument<TMetadataEntry> doc = await this.metadataService.GetPackageVersionStatesDocumentAsync(request.ToSingleVersionPackageNameQuery<TPackageId, TMetadataEntry>());
      MetadataDocument<TMetadataEntry> metadataDocument = doc;
      TMetadataEntry metadataEntry = metadataDocument != null ? metadataDocument.Entries.FirstOrDefault<TMetadataEntry>() : default (TMetadataEntry);
      if ((object) metadataEntry == null)
        return ProbeUpstreamIfNecessary();
      ICachablePackageMetadata cachedMetadata = metadataEntry.GetCachableMetadata((IPackageFileRequest) request);
      if (cachedMetadata == null)
        throw ExceptionHelper.PackageNotFound(request.Feed, (IPackageIdentity) request.PackageId, request.FilePath);
      if (cachedMetadata.StorageId is UpstreamStorageId)
        cachedMetadata = ProbeUpstreamIfNecessary(cachedMetadata);
      return cachedMetadata;

      ICachablePackageMetadata ProbeUpstreamIfNecessary(ICachablePackageMetadata cachedMetadata = null)
      {
        if (this.UpstreamsAreDisabled((IFeedRequest) request))
          throw ExceptionHelper.PackageNotFound(request.Feed, (IPackageIdentity) request.PackageId, request.FilePath);
        if (doc == null || !this.upstreamEntriesStillValidChecker.IsUpstreamInfoValid((IFeedRequest) request, (IMetadataDocument) doc))
          return CachablePackageMetadata.TryAllUpstreams;
        return cachedMetadata != null ? cachedMetadata : throw ExceptionHelper.PackageNotFound(request.Feed, (IPackageIdentity) request.PackageId, request.FilePath);
      }
    }

    private bool UpstreamsAreDisabled(IFeedRequest feedRequest)
    {
      IEnumerable<UpstreamSource> source = this.upstreamsFromFeedHandler.Handle(feedRequest);
      return source == null || !source.Any<UpstreamSource>();
    }
  }
}
