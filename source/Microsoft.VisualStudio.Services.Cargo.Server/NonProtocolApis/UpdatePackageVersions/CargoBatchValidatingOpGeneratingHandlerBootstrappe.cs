// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.UpdatePackageVersions.CargoBatchValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.UpdatePackageVersions
{
  public class CargoBatchValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackagesBatchRequest<CargoPackageIdentity>, BatchCommitOperationData, ICargoPackageMetadataAggregationAccessor>
  {
    private readonly IVssRequestContext requestContext;

    public CargoBatchValidatingOpGeneratingHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<PackagesBatchRequest<CargoPackageIdentity>, BatchCommitOperationData> Bootstrap(
      ICargoPackageMetadataAggregationAccessor metadataAccessor)
    {
      DefaultTimeProvider defaultTimeProvider = new DefaultTimeProvider();
      ViewIdResolver viewResolver = new ViewIdResolver((IFeedService) new FeedServiceFacade(this.requestContext));
      IAsyncHandler<IPackageRequest<CargoPackageIdentity>, ICargoMetadataEntry> pointQueryHandler = metadataAccessor.ToPointQueryHandler<CargoPackageIdentity, ICargoMetadataEntry>(true);
      return UntilNonNullHandler.Create<PackagesBatchRequest<CargoPackageIdentity>, BatchCommitOperationData>(PackagesBatchRequestOpGeneratingHandler.Create<CargoPackageIdentity, FeedView, ICommitOperationData>(BatchOperationType.Promote, (IConverter<PackagesBatchRequest<CargoPackageIdentity>, PackagesBatchRequest<CargoPackageIdentity, FeedView>>) new ViewBatchRequestConverter<CargoPackageIdentity>((IConverter<FeedViewRequest, FeedView>) viewResolver), (IAsyncHandler<PackageRequest<CargoPackageIdentity, FeedView>, ICommitOperationData>) new PromoteValidatingHandler<CargoPackageIdentity, FeedView, ICargoMetadataEntry>((IAsyncHandler<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry>) pointQueryHandler, (IConverter<PackageRequest<CargoPackageIdentity, FeedView>, IViewOperationData>) new ViewOpGeneratingConverter<CargoPackageIdentity>())), PackagesBatchRequestOpGeneratingHandler.Create<CargoPackageIdentity, ListingOperationRequestAdditionalData, IListingStateChangeOperationData>((IBatchOperationType) CargoPackagesBatchRequest.Yank, (IConverter<PackagesBatchRequest<CargoPackageIdentity>, PackagesBatchRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>>) new PackagesBatchListingOperationRequestConverter<CargoPackageIdentity, ListingOperationRequestAdditionalData>(), (IAsyncHandler<PackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>) new ListValidatingHandler<ListingOperationRequestAdditionalData, CargoPackageIdentity, ICargoMetadataEntry>(pointQueryHandler, (IConverter<IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>) new ListingOperationDataGeneratingConverter())), PackagesBatchRequestOpGeneratingHandler.Create<CargoPackageIdentity, DeleteRequestAdditionalData, IDeleteOperationData>(BatchOperationType.Delete, (IConverter<PackagesBatchRequest<CargoPackageIdentity>, PackagesBatchRequest<CargoPackageIdentity, DeleteRequestAdditionalData>>) new DeleteBatchRequestConverter<CargoPackageIdentity>((ITimeProvider) defaultTimeProvider), (IAsyncHandler<PackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>) CargoAggregationResolver.Bootstrap(this.requestContext).HandlerFor<IPackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>((IRequireAggBootstrapper<IAsyncHandler<IPackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>>) new CargoDeleteOpGeneratorBootstrapper(this.requestContext))), (IAsyncHandler<PackagesBatchRequest<CargoPackageIdentity>, BatchCommitOperationData>) new ThrowWithBadOperationMessageHandler<CargoPackageIdentity>());
    }
  }
}
