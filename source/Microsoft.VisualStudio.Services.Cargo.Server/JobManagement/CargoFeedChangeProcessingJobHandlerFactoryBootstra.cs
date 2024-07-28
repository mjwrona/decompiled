// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.JobManagement.CargoFeedChangeProcessingJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.ChangeProcessing;
using Microsoft.VisualStudio.Services.Cargo.Server.CommitLog;
using Microsoft.VisualStudio.Services.Cargo.Server.Constants;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.Upload;
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


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.JobManagement
{
  public class CargoFeedChangeProcessingJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly bool handleOnlyMigrationCatchup;

    public CargoFeedChangeProcessingJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      bool handleOnlyMigrationCatchup = false)
    {
      this.requestContext = requestContext;
      this.handleOnlyMigrationCatchup = handleOnlyMigrationCatchup;
    }

    public IFactory<JobId?, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap()
    {
      ICommitLog commitLog = new CargoCommitLogFacadeBootstrapper(this.requestContext).Bootstrap();
      return new FeedChangeProcessingJobHandlerFactoryBootstrapper(this.requestContext, (ICommitLogReader<CommitLogEntry>) commitLog, (ICommitLogWriter<ICommitLogEntry>) commitLog, Protocol.Cargo.ChangeProcessingBookmarkTokenKey, new CargoMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), (IShouldMarkFactory) new CargoExceptionMarkFactory(), (Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference>) (r => CargoBlobRefGenerator.ForCrate(r.FeedId, (CargoPackageIdentity) r.PackageIdentity)), (Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference>) null, (ICancellationFacade) new CancellationFacade(this.requestContext), CargoJobConstants.DeleteProcessingJobConstants.FeedDeletedPackageJobCreationInfo, this.handleOnlyMigrationCatchup, Enumerable.Empty<IPduNotificationDetector>()).Bootstrap();
    }
  }
}
