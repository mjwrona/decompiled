// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.JobManagement.MavenFeedChangeProcessingJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Maven.Server.ChangeProcessing;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
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
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.JobManagement
{
  public class MavenFeedChangeProcessingJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly bool handleOnlyMigrationCatchup;

    public MavenFeedChangeProcessingJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      bool handleOnlyMigrationCatchup = false)
    {
      this.requestContext = requestContext;
      this.handleOnlyMigrationCatchup = handleOnlyMigrationCatchup;
    }

    public IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap()
    {
      ICommitLog commitLog = new MavenCommitLogFacadeBootstrapper(this.requestContext).Bootstrap();
      return new FeedChangeProcessingJobHandlerFactoryBootstrapper(this.requestContext, (ICommitLogReader<CommitLogEntry>) commitLog, (ICommitLogWriter<ICommitLogEntry>) commitLog, Protocol.Maven.ChangeProcessingBookmarkTokenKey, new MavenMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), (IShouldMarkFactory) new MavenExceptionMarkerFactory(), (Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference>) null, (Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference>) null, (ICancellationFacade) new CancellationFacade(this.requestContext), MavenJobConstants.FeedDeletedPackageJobCreationInfo, this.handleOnlyMigrationCatchup, Enumerable.Empty<IPduNotificationDetector>()).Bootstrap();
    }
  }
}
