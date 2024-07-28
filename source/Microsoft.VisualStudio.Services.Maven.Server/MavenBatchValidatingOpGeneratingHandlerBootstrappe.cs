// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenBatchValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Aggregations;
using Microsoft.VisualStudio.Services.Maven.Server.Converters;
using Microsoft.VisualStudio.Services.Maven.Server.DeleteProcessingJob;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenBatchValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackagesBatchRequest<MavenPackageIdentity>, BatchCommitOperationData, IMavenMetadataAggregationAccessor>
  {
    private readonly IVssRequestContext requestContext;

    public MavenBatchValidatingOpGeneratingHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<PackagesBatchRequest<MavenPackageIdentity>, BatchCommitOperationData> Bootstrap(
      IMavenMetadataAggregationAccessor metadataAccessor)
    {
      IAsyncHandler<IPackageRequest<MavenPackageIdentity>, IMavenMetadataEntry> pointQueryHandler = metadataAccessor.ToPointQueryHandler<MavenPackageIdentity, IMavenMetadataEntry>(true);
      return UntilNonNullHandler.Create<PackagesBatchRequest<MavenPackageIdentity>, BatchCommitOperationData>(this.MakePromoteHandler((IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry>) pointQueryHandler), MavenBatchValidatingOpGeneratingHandlerBootstrapper.MakeRestoreToFeedHandler((IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry>) pointQueryHandler), MavenBatchValidatingOpGeneratingHandlerBootstrapper.MakePermanentDeleteHandler((IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry>) pointQueryHandler), this.MakeDeleteHandler(), (IAsyncHandler<PackagesBatchRequest<MavenPackageIdentity>, BatchCommitOperationData>) new ThrowWithBadOperationMessageHandler<MavenPackageIdentity>());
    }

    private IAsyncHandler<PackagesBatchRequest<MavenPackageIdentity>, BatchCommitOperationData> MakePromoteHandler(
      IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry> metadataHandler)
    {
      ViewIdResolver viewResolver = new ViewIdResolver((IFeedService) new FeedServiceFacade(this.requestContext));
      ViewOpGeneratingConverter<MavenPackageIdentity> requestToOpConverter = new ViewOpGeneratingConverter<MavenPackageIdentity>();
      return PackagesBatchRequestOpGeneratingHandler.Create<MavenPackageIdentity, FeedView, ICommitOperationData>(BatchOperationType.Promote, (IConverter<PackagesBatchRequest<MavenPackageIdentity>, PackagesBatchRequest<MavenPackageIdentity, FeedView>>) new ViewBatchRequestConverter<MavenPackageIdentity>((IConverter<FeedViewRequest, FeedView>) viewResolver), (IAsyncHandler<PackageRequest<MavenPackageIdentity, FeedView>, ICommitOperationData>) new PromoteValidatingHandler<MavenPackageIdentity, FeedView, IMavenMetadataEntry>(metadataHandler, (IConverter<PackageRequest<MavenPackageIdentity, FeedView>, IViewOperationData>) requestToOpConverter));
    }

    private static IAsyncHandler<PackagesBatchRequest<MavenPackageIdentity>, BatchCommitOperationData> MakeRestoreToFeedHandler(
      IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry> metadataHandler)
    {
      MavenRecycleBinPackageVersionDetails recycleBinVersionDetails = new MavenRecycleBinPackageVersionDetails()
      {
        Deleted = new bool?(false)
      };
      ByFuncConverter<PackagesBatchRequest<MavenPackageIdentity>, PackagesBatchRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>> requestWithDataConverter = new ByFuncConverter<PackagesBatchRequest<MavenPackageIdentity>, PackagesBatchRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>>((Func<PackagesBatchRequest<MavenPackageIdentity>, PackagesBatchRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>>) (batchRequest => new PackagesBatchRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>(batchRequest, (IRecycleBinPackageVersionDetails) recycleBinVersionDetails)));
      return PackagesBatchRequestOpGeneratingHandler.Create<MavenPackageIdentity, IRecycleBinPackageVersionDetails, IRestoreToFeedOperationData>(BatchOperationType.RestoreToFeed, (IConverter<PackagesBatchRequest<MavenPackageIdentity>, PackagesBatchRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>>) requestWithDataConverter, (IAsyncHandler<PackageRequest<MavenPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) new RestoreToFeedValidatingOpGeneratingHandler<MavenPackageIdentity, IMavenMetadataEntry, IRestoreToFeedOperationData>(metadataHandler, (IAsyncHandler<PackageRequest<MavenPackageIdentity>, IRestoreToFeedOperationData>) new MavenRestoreToFeedOpGeneratingHandler()));
    }

    private IAsyncHandler<PackagesBatchRequest<MavenPackageIdentity>, BatchCommitOperationData> MakeDeleteHandler() => PackagesBatchRequestOpGeneratingHandler.Create<MavenPackageIdentity, DeleteRequestAdditionalData, IDeleteOperationData>(BatchOperationType.Delete, (IConverter<PackagesBatchRequest<MavenPackageIdentity>, PackagesBatchRequest<MavenPackageIdentity, DeleteRequestAdditionalData>>) new DeleteBatchRequestConverter<MavenPackageIdentity>((ITimeProvider) new DefaultTimeProvider()), (IAsyncHandler<PackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>) MavenAggregationResolver.Bootstrap(this.requestContext).HandlerFor<IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>((IRequireAggBootstrapper<IAsyncHandler<IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>>) new MavenDeleteOpGeneratorBootstrapper(this.requestContext)));

    private static IAsyncHandler<PackagesBatchRequest<MavenPackageIdentity>, BatchCommitOperationData> MakePermanentDeleteHandler(
      IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry> metadataHandler)
    {
      return PackagesBatchRequestOpGeneratingHandler.Create<MavenPackageIdentity, IPermanentDeleteOperationData>(BatchOperationType.PermanentDelete, (IAsyncHandler<PackageRequest<MavenPackageIdentity>, IPermanentDeleteOperationData>) new MavenPermanentDeleteValidatingOpGeneratingHandler(metadataHandler, (IConverter<MavenPackageRequestWithMetadata, IEnumerable<BlobReferenceIdentifier>>) new MavenGetExtraAssetsBlobRefIdsHandler((IConverter<IPackageNameRequest<MavenPackageName, IEnumerable<MavenPackageFileNew>>, IEnumerable<BlobReferenceIdentifier>>) new MavenPackageFilesToBlobReferenceIdentifiersConverter())));
    }
  }
}
