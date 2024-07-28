// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetBatchValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types;
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

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetBatchValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData, INuGetMetadataService>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetBatchValidatingOpGeneratingHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData> Bootstrap(
      INuGetMetadataService metadataAccessor)
    {
      DefaultTimeProvider defaultTimeProvider = new DefaultTimeProvider();
      ViewIdResolver viewResolver = new ViewIdResolver((IFeedService) new FeedServiceFacade(this.requestContext));
      IAsyncHandler<IPackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry> pointQueryHandler = metadataAccessor.ToPointQueryHandler<VssNuGetPackageIdentity, INuGetMetadataEntry>(true);
      return UntilNonNullHandler.Create<PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData>(PackagesBatchRequestOpGeneratingHandler.Create<VssNuGetPackageIdentity, FeedView, ICommitOperationData>(BatchOperationType.Promote, (IConverter<PackagesBatchRequest<VssNuGetPackageIdentity>, PackagesBatchRequest<VssNuGetPackageIdentity, FeedView>>) new ViewBatchRequestConverter<VssNuGetPackageIdentity>((IConverter<FeedViewRequest, FeedView>) viewResolver), (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, FeedView>, ICommitOperationData>) new PromoteValidatingHandler<VssNuGetPackageIdentity, FeedView, INuGetMetadataEntry>((IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry>) pointQueryHandler, (IConverter<PackageRequest<VssNuGetPackageIdentity, FeedView>, IViewOperationData>) new ViewOpGeneratingConverter<VssNuGetPackageIdentity>())), PackagesBatchRequestOpGeneratingHandler.Create<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData, IListingStateChangeOperationData>((IBatchOperationType) NuGetPackagesBatchRequest.List, (IConverter<PackagesBatchRequest<VssNuGetPackageIdentity>, PackagesBatchRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>>) new NuGetPackagesBatchListingOperationRequestConverter(), (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>) new ListValidatingHandler<ListingOperationRequestAdditionalData, VssNuGetPackageIdentity, INuGetMetadataEntry>(pointQueryHandler, (IConverter<IPackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>) new ListingOperationDataGeneratingConverter())), PackagesBatchRequestOpGeneratingHandler.Create<VssNuGetPackageIdentity, DeleteRequestAdditionalData, IDeleteOperationData>(BatchOperationType.Delete, (IConverter<PackagesBatchRequest<VssNuGetPackageIdentity>, PackagesBatchRequest<VssNuGetPackageIdentity, DeleteRequestAdditionalData>>) new DeleteBatchRequestConverter<VssNuGetPackageIdentity>((ITimeProvider) defaultTimeProvider), (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>) NuGetAggregationResolver.Bootstrap(this.requestContext).HandlerFor<IPackageRequest<VssNuGetPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>((IRequireAggBootstrapper<IAsyncHandler<IPackageRequest<VssNuGetPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>>) new NuGetDeleteOpGeneratorBootstrapper(this.requestContext))), (IAsyncHandler<PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData>) new ThrowWithBadOperationMessageHandler<VssNuGetPackageIdentity>());
    }
  }
}
