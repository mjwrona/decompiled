// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.CargoManualUpstreamIngestionBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams
{
  public class CargoManualUpstreamIngestionBootstrapper : 
    IBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>>
  {
    private readonly IVssRequestContext requestContext;

    public CargoManualUpstreamIngestionBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<IRawPackageRequest, NullResult> Bootstrap()
    {
      IConverter<IRawPackageRequest, IPackageFileRequest<CargoPackageIdentity>> converter = ((IConverter<IRawPackageRequest, IPackageRequest<CargoPackageIdentity>>) new RawPackageRequestConverter<CargoPackageIdentity>(CargoIdentityResolver.Instance.GetRawPackageRequestToIdentityConverter<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, SimplePackageFileName>(this.requestContext))).ThenConvertBy<IRawPackageRequest, IPackageRequest<CargoPackageIdentity>, IPackageFileRequest<CargoPackageIdentity>>(ByFuncConverter.Create<IPackageRequest<CargoPackageIdentity>, IPackageFileRequest<CargoPackageIdentity>>((Func<IPackageRequest<CargoPackageIdentity>, IPackageFileRequest<CargoPackageIdentity>>) (request => request.WithFile<CargoPackageIdentity>(request.PackageId.GetCanonicalCrateFileName()))));
      IAsyncHandler<IPackageFileRequest<CargoPackageIdentity>, IPackageFileRequest<CargoPackageIdentity, IStorageId>> asyncHandler = CargoAggregationResolver.Bootstrap(this.requestContext).HandlerFor<IPackageFileRequest<CargoPackageIdentity>, IPackageFileRequest<CargoPackageIdentity, IStorageId>, ICargoPackageMetadataAggregationAccessor, IMetadataCacheService>((Func<ICargoPackageMetadataAggregationAccessor, IMetadataCacheService, IAsyncHandler<IPackageFileRequest<CargoPackageIdentity>, IPackageFileRequest<CargoPackageIdentity, IStorageId>>>) ((metadataService, metadataCacheService) => new AddStorageInfoToPackageFileRequestHandler<CargoPackageIdentity, ICargoMetadataEntry>(metadataCacheService, (IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>) new ThrowIfDeletedDelegatingMetadataDocumentStore<CargoPackageIdentity, ICargoMetadataEntry>((IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>) metadataService), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), this.requestContext.GetTracerFacade(), (ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext), (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) new UpstreamsFromFeedHandler(this.requestContext.GetFeatureFlagFacade()), new BlockedPackageIdentityRequestToExceptionConverterBootstrapper(this.requestContext).Bootstrap().AsThrowingValidator<IPackageRequest>()).AsIAsyncHandler<IPackageFileRequest<CargoPackageIdentity>, IPackageFileRequest<CargoPackageIdentity, IStorageId>>()));
      IAsyncHandler<IPackageFileRequest<CargoPackageIdentity, IStorageId>, ContentResult> handler1 = new IngestPackageIfNotAlreadyIngestedBootstrapper(this.requestContext).Bootstrap();
      IAsyncHandler<IPackageFileRequest<CargoPackageIdentity>, IPackageFileRequest<CargoPackageIdentity, IStorageId>> handler2 = asyncHandler;
      return (IAsyncHandler<IRawPackageRequest, NullResult>) converter.ThenDelegateTo<IRawPackageRequest, IPackageFileRequest<CargoPackageIdentity>, IPackageFileRequest<CargoPackageIdentity, IStorageId>>(handler2).ThenDelegateTo<IRawPackageRequest, IPackageFileRequest<CargoPackageIdentity, IStorageId>, ContentResult>(handler1).ThenReturnNullResult<IRawPackageRequest, ContentResult>();
    }
  }
}
