// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersions.NpmBatchValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersion;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.WebApi;
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
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersions
{
  public class NpmBatchValidatingOpGeneratingHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<PackagesBatchRequest<NpmPackageIdentity>, BatchCommitOperationData>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmBatchValidatingOpGeneratingHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<PackagesBatchRequest<NpmPackageIdentity>, BatchCommitOperationData> Bootstrap()
    {
      DefaultTimeProvider defaultTimeProvider = new DefaultTimeProvider();
      ViewIdResolver viewResolver = new ViewIdResolver((IFeedService) new FeedServiceFacade(this.requestContext));
      IAsyncHandler<IPackageRequest<NpmPackageIdentity>, INpmMetadataEntry> metadataHandler = new NpmMetadataHandlerBootstrapper(this.requestContext).Bootstrap();
      ViewOpGeneratingConverter<NpmPackageIdentity> requestToOpConverter = new ViewOpGeneratingConverter<NpmPackageIdentity>();
      return UntilNonNullHandler.Create<PackagesBatchRequest<NpmPackageIdentity>, BatchCommitOperationData>(PackagesBatchRequestOpGeneratingHandler.Create<NpmPackageIdentity, FeedView, ICommitOperationData>(BatchOperationType.Promote, (IConverter<PackagesBatchRequest<NpmPackageIdentity>, PackagesBatchRequest<NpmPackageIdentity, FeedView>>) new ViewBatchRequestConverter<NpmPackageIdentity>((IConverter<FeedViewRequest, FeedView>) viewResolver), (IAsyncHandler<PackageRequest<NpmPackageIdentity, FeedView>, ICommitOperationData>) new PromoteValidatingHandler<NpmPackageIdentity, FeedView, INpmMetadataEntry>((IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry>) metadataHandler, (IConverter<PackageRequest<NpmPackageIdentity, FeedView>, IViewOperationData>) requestToOpConverter)), PackagesBatchRequestOpGeneratingHandler.Create<NpmPackageIdentity, INpmDeprecateData, ICommitOperationData>((IBatchOperationType) NpmPackagesBatchRequest.Deprecate, ConvertFrom.InputTypeOf<PackagesBatchRequest<NpmPackageIdentity>>((IBootstrapper<IHaveInputType<PackagesBatchRequest<NpmPackageIdentity>>>) this).By<PackagesBatchRequest<NpmPackageIdentity>, PackagesBatchRequest<NpmPackageIdentity, INpmDeprecateData>>((Func<PackagesBatchRequest<NpmPackageIdentity>, PackagesBatchRequest<NpmPackageIdentity, INpmDeprecateData>>) (r => new PackagesBatchRequest<NpmPackageIdentity, INpmDeprecateData>(r, (INpmDeprecateData) r.BatchOperationData))), (IAsyncHandler<PackageRequest<NpmPackageIdentity, INpmDeprecateData>, ICommitOperationData>) new DeprecateValidatingHandler<INpmDeprecateData>((IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry>) metadataHandler)), PackagesBatchRequestOpGeneratingHandler.Create<NpmPackageIdentity, DeleteRequestAdditionalData, IDeleteOperationData>(BatchOperationType.Delete, (IConverter<PackagesBatchRequest<NpmPackageIdentity>, PackagesBatchRequest<NpmPackageIdentity, DeleteRequestAdditionalData>>) new DeleteBatchRequestConverter<NpmPackageIdentity>((ITimeProvider) defaultTimeProvider), (IAsyncHandler<PackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>) NpmAggregationResolver.Bootstrap(this.requestContext).HandlerFor<IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>((IRequireAggBootstrapper<IAsyncHandler<IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>>) new NpmDeleteOpGeneratorBootstrapper(this.requestContext))), (IAsyncHandler<PackagesBatchRequest<NpmPackageIdentity>, BatchCommitOperationData>) new ThrowWithBadOperationMessageHandler<NpmPackageIdentity>());
    }
  }
}
