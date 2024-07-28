// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ChangeProcessing.CollectionDeletedFeedsProcessingJobHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ChangeProcessing
{
  public class CollectionDeletedFeedsProcessingJobHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<TeamFoundationJobDefinition, JobResult>>
  {
    private const int FeedsDeletedCodeOnlyBookmarkVersion = 2;
    private readonly IVssRequestContext requestContext;
    private readonly Guid jobId;
    private readonly IProtocol protocol;
    private readonly BookmarkTokenKey deletedFeedsBookmarkKey;
    private readonly ICommitLog commitLogFacade;
    private readonly IMigrationDefinitionsProvider migrationDefinitionsProvider;
    private readonly Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference> getBlobReferenceFunc;
    private readonly Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference> getDedupReferenceFunc;
    private readonly IAsyncHandler<FeedRequest<IAddOperationData>, IStorageDeletionRequest> storageDeletionRequestCreator;

    public CollectionDeletedFeedsProcessingJobHandlerBootstrapper(
      IVssRequestContext requestContext,
      Guid jobId,
      IProtocol protocol,
      BookmarkTokenKey deletedFeedsBookmarkKey,
      ICommitLog commitLogFactory,
      IMigrationDefinitionsProvider migrationDefinitionsProvider,
      Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference> getBlobReferenceFunc,
      Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference> getDedupReferenceFunc,
      IAsyncHandler<FeedRequest<IAddOperationData>, IStorageDeletionRequest> storageDeletionRequestCreator = null)
    {
      this.requestContext = requestContext;
      this.jobId = jobId;
      this.protocol = protocol;
      this.deletedFeedsBookmarkKey = deletedFeedsBookmarkKey;
      this.commitLogFacade = commitLogFactory;
      this.migrationDefinitionsProvider = migrationDefinitionsProvider;
      this.getBlobReferenceFunc = getBlobReferenceFunc;
      this.getDedupReferenceFunc = getDedupReferenceFunc;
      this.storageDeletionRequestCreator = storageDeletionRequestCreator ?? (IAsyncHandler<FeedRequest<IAddOperationData>, IStorageDeletionRequest>) new ByFuncAsyncHandler<FeedRequest<IAddOperationData>, IStorageDeletionRequest>((Func<FeedRequest<IAddOperationData>, IStorageDeletionRequest>) (r => StorageDeletionRequest.Create(r.Feed.Id, r.AdditionalData.Identity, r.AdditionalData.PackageStorageId, (IEnumerable<BlobReferenceIdentifier>) null)));
    }

    public IAsyncHandler<TeamFoundationJobDefinition, JobResult> Bootstrap()
    {
      ReturnSameInstanceFactory<ICommitLog> commitLogService = new ReturnSameInstanceFactory<ICommitLog>(this.commitLogFacade);
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      IExecutionEnvironment environmentFacade = this.requestContext.GetExecutionEnvironmentFacade();
      CollectionDeletedFeedsProcessingJobHandler jobHandler = new CollectionDeletedFeedsProcessingJobHandler((ITimeProvider) new DefaultTimeProvider(), featureFlagFacade, environmentFacade, (IRegistryService) new RegistryServiceFacade(this.requestContext), FeedChangesBookmarkTokenProvider.Bootstrap(this.requestContext, this.deletedFeedsBookmarkKey), (IFeedChangesService) new FeedChangesServiceFacade(this.requestContext, this.requestContext.GetService<IFeedChangeClientService>()), (IMigrationTransitioner) new NoCachingMigrationTransitionerBootstrapper(this.requestContext, this.migrationDefinitionsProvider).Bootstrap(), (IFactory<ICommitLog>) commitLogService, this.protocol, this.requestContext.GetTracerFacade(), new StorageDeleterHandlerBootstrapper(this.requestContext, this.getBlobReferenceFunc, this.getDedupReferenceFunc).Bootstrap(), this.storageDeletionRequestCreator, (ICommitOperationData) new PackageBlobsDereferencedMarkerOperationData());
      return UntilNonNullHandler.Create<TeamFoundationJobDefinition, JobResult>((IAsyncHandler<TeamFoundationJobDefinition, JobResult>) new FeatureFlagCheckingJobHandler<TeamFoundationJobDefinition>(featureFlagFacade, this.protocol.ReadOnlyFeatureFlagName, true), new JobErrorHandlerBootstrapper<TeamFoundationJobDefinition>(this.requestContext, (IAsyncHandler<TeamFoundationJobDefinition, JobResult>) jobHandler, (JobId) this.jobId).Bootstrap());
    }
  }
}
