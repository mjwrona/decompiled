// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.JobManagement.NpmFeedChangeProcessingJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Npm.Server.ChangeProcessing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Constants;
using Microsoft.VisualStudio.Services.Npm.Server.Migration;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.JobManagement
{
  public class NpmFeedChangeProcessingJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly bool handleOnlyMigrationCatchup;

    public NpmFeedChangeProcessingJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      bool handleOnlyMigrationCatchup = false)
    {
      this.requestContext = requestContext;
      this.handleOnlyMigrationCatchup = handleOnlyMigrationCatchup;
    }

    public IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap()
    {
      ICommitLog commitLog = new NpmCommitLogFacadeBootstrapper(this.requestContext).Bootstrap();
      return new FeedChangeProcessingJobHandlerFactoryBootstrapper(this.requestContext, (ICommitLogReader<CommitLogEntry>) commitLog, (ICommitLogWriter<ICommitLogEntry>) commitLog, Protocol.npm.ChangeProcessingBookmarkTokenKey, new NpmMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), (IShouldMarkFactory) new NpmExceptionMarkerFactory(), new Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference>(this.GetBlobReference), (Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference>) null, (ICancellationFacade) new CancellationFacade(this.requestContext), NpmJobConstants.FeedDeletedPackageJobCreationInfo, (this.handleOnlyMigrationCatchup ? 1 : 0) != 0, (IEnumerable<IPduNotificationDetector>) new NpmMetadataDiffCommitPduNotificationDetector[1]
      {
        new NpmMetadataDiffCommitPduNotificationDetector()
      }).Bootstrap();
    }

    public IdBlobReference GetBlobReference(IStorageDeletionRequest<BlobStorageId> request)
    {
      NpmPackageIdentity npmPackageIdentity = new NpmPackageIdentity(new NpmPackageName(request.PackageIdentity.Name.NormalizedName), SemanticVersion.Parse(request.PackageIdentity.Version.NormalizedVersion));
      return NpmBlobUtils.GetPackageBlobReference(request.FeedId, npmPackageIdentity.Name, npmPackageIdentity.Version);
    }
  }
}
