// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersions.PyPiBatchValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersions
{
  public class PyPiBatchValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData, IPyPiMetadataAggregationAccessor>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiBatchValidatingOpGeneratingHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData> Bootstrap(
      IPyPiMetadataAggregationAccessor metadataAccessor)
    {
      DefaultTimeProvider defaultTimeProvider = new DefaultTimeProvider();
      ViewIdResolver viewResolver = new ViewIdResolver((IFeedService) new FeedServiceFacade(this.requestContext));
      IAsyncHandler<IPackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry> pointQueryHandler = metadataAccessor.ToPointQueryHandler<PyPiPackageIdentity, IPyPiMetadataEntry>(true);
      return UntilNonNullHandler.Create<PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData>(PackagesBatchRequestOpGeneratingHandler.Create<PyPiPackageIdentity, FeedView, ICommitOperationData>(BatchOperationType.Promote, (IConverter<PackagesBatchRequest<PyPiPackageIdentity>, PackagesBatchRequest<PyPiPackageIdentity, FeedView>>) new ViewBatchRequestConverter<PyPiPackageIdentity>((IConverter<FeedViewRequest, FeedView>) viewResolver), (IAsyncHandler<PackageRequest<PyPiPackageIdentity, FeedView>, ICommitOperationData>) new PromoteValidatingHandler<PyPiPackageIdentity, FeedView, IPyPiMetadataEntry>((IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry>) pointQueryHandler, (IConverter<PackageRequest<PyPiPackageIdentity, FeedView>, IViewOperationData>) new ViewOpGeneratingConverter<PyPiPackageIdentity>())), PackagesBatchRequestOpGeneratingHandler.Create<PyPiPackageIdentity, DeleteRequestAdditionalData, IDeleteOperationData>(BatchOperationType.Delete, (IConverter<PackagesBatchRequest<PyPiPackageIdentity>, PackagesBatchRequest<PyPiPackageIdentity, DeleteRequestAdditionalData>>) new DeleteBatchRequestConverter<PyPiPackageIdentity>((ITimeProvider) defaultTimeProvider), (IAsyncHandler<PackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>) PyPiAggregationResolver.Bootstrap(this.requestContext).HandlerFor<IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>((IRequireAggBootstrapper<IAsyncHandler<IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>>) new PyPiDeleteOpGeneratorBootstrapper(this.requestContext))), (IAsyncHandler<PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData>) new ThrowWithBadOperationMessageHandler<PyPiPackageIdentity>());
    }
  }
}
