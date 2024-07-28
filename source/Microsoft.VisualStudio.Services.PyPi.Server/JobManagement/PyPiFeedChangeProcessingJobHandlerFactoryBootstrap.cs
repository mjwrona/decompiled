// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.JobManagement.PyPiFeedChangeProcessingJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
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
using Microsoft.VisualStudio.Services.PyPi.Server.ChangeProcessing;
using Microsoft.VisualStudio.Services.PyPi.Server.CommitLog;
using Microsoft.VisualStudio.Services.PyPi.Server.Constants;
using Microsoft.VisualStudio.Services.PyPi.Server.Migration;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.JobManagement
{
  public class PyPiFeedChangeProcessingJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly bool handleOnlyMigrationCatchup;

    public PyPiFeedChangeProcessingJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      bool handleOnlyMigrationCatchup = false)
    {
      this.requestContext = requestContext;
      this.handleOnlyMigrationCatchup = handleOnlyMigrationCatchup;
    }

    public IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap()
    {
      ICommitLog commitLog = new PyPiCommitLogFacadeBootstrapper(this.requestContext).Bootstrap();
      return new FeedChangeProcessingJobHandlerFactoryBootstrapper(this.requestContext, (ICommitLogReader<CommitLogEntry>) commitLog, (ICommitLogWriter<ICommitLogEntry>) commitLog, Protocol.PyPi.ChangeProcessingBookmarkTokenKey, new PyPiMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), (IShouldMarkFactory) new PyPiExceptionMarkFactory(), (Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference>) null, (Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference>) null, (ICancellationFacade) new CancellationFacade(this.requestContext), PyPiJobConstants.DeleteProcessingJobConstants.FeedDeletedPackageJobCreationInfo, this.handleOnlyMigrationCatchup, Enumerable.Empty<IPduNotificationDetector>()).Bootstrap();
    }
  }
}
