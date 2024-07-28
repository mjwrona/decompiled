// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.PackageFileRequestToContentResultHandlerBootstrapper`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class PackageFileRequestToContentResultHandlerBootstrapper<TPackageId, TMetadataEntry> : 
    IBootstrapper<IAsyncHandler<IPackageFileRequest<TPackageId>, ContentResult>>
    where TPackageId : class, IPackageIdentity
    where TMetadataEntry : class, IMetadataEntry<TPackageId>, IPackageFiles
  {
    private readonly IVssRequestContext requestContext;
    private readonly IReadMetadataDocumentService<TPackageId, TMetadataEntry> metadataService;
    private readonly IMetadataCacheService metadataCacheService;
    private readonly IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult?>? ingestingHandler;
    private readonly IReadOnlyList<ISpecificStorageContentProvider> extraContentProviders;
    private readonly IReadOnlyList<ISpecificExtractingStorageContentProvider> extraExtractingContentProviders;

    public PackageFileRequestToContentResultHandlerBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<TPackageId, TMetadataEntry> metadataService,
      IMetadataCacheService metadataCacheService,
      IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult?>? ingestingHandler,
      IReadOnlyList<ISpecificStorageContentProvider> extraContentProviders,
      IReadOnlyList<ISpecificExtractingStorageContentProvider> extraExtractingContentProviders)
    {
      this.requestContext = requestContext;
      this.metadataService = metadataService;
      this.metadataCacheService = metadataCacheService;
      this.ingestingHandler = ingestingHandler;
      this.extraContentProviders = extraContentProviders;
      this.extraExtractingContentProviders = extraExtractingContentProviders;
    }

    public IAsyncHandler<IPackageFileRequest<TPackageId>, ContentResult> Bootstrap()
    {
      AddStorageInfoToPackageFileRequestHandler<TPackageId, TMetadataEntry> currentHandler = new AddStorageInfoToPackageFileRequestHandler<TPackageId, TMetadataEntry>(this.metadataCacheService, (IReadMetadataDocumentService<TPackageId, TMetadataEntry>) new ThrowIfDeletedDelegatingMetadataDocumentStore<TPackageId, TMetadataEntry>(this.metadataService), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), this.requestContext.GetTracerFacade(), (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext), (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) new UpstreamsFromFeedHandler(this.requestContext.GetFeatureFlagFacade()), new BlockedPackageIdentityRequestToExceptionConverterBootstrapper(this.requestContext).Bootstrap().AsThrowingValidator<IPackageRequest>());
      List<ISpecificStorageContentProvider> plainProviders = new List<ISpecificStorageContentProvider>()
      {
        new BlobStorageContentProviderBootstrapper(this.requestContext).Bootstrap()
      };
      plainProviders.AddRange((IEnumerable<ISpecificStorageContentProvider>) this.extraContentProviders);
      if (this.ingestingHandler != null)
        plainProviders.Add((ISpecificStorageContentProvider) new AsyncHandlerToStorageContentProviderAdapter<TPackageId>((IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult>) new DownloadPackagePermissionsCheckingDelegatingHandler<TPackageId>(this.ingestingHandler, (IFeedPerms) new FeedPermsFacade(this.requestContext), (IExecutionEnvironment) new ExecutionEnvironmentFacade(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeeds.Bootstrap(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeedsMSFT.Bootstrap(this.requestContext))));
      List<ISpecificExtractingStorageContentProvider> extractingProviders = new List<ISpecificExtractingStorageContentProvider>();
      extractingProviders.AddRange((IEnumerable<ISpecificExtractingStorageContentProvider>) this.extraExtractingContentProviders);
      StorageContentProvider contentProvider = new StorageContentProvider((IReadOnlyList<ISpecificStorageContentProvider>) plainProviders, (IReadOnlyList<ISpecificExtractingStorageContentProvider>) extractingProviders);
      IAsyncHandler<IPackageFileRequest<TPackageId, IStorageId>, ContentResult> handler = ByAsyncFuncAsyncHandler.For<IPackageFileRequest<TPackageId, IStorageId>, ContentResult>((Func<IPackageFileRequest<TPackageId, IStorageId>, Task<ContentResult>>) (async request => await contentProvider.GetContent((IPackageFileRequest) request, request.AdditionalData)));
      return currentHandler.ThenDelegateTo<IPackageFileRequest<TPackageId>, IPackageFileRequest<TPackageId, IStorageId>, ContentResult>(handler);
    }
  }
}
