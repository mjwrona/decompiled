// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetFeedChangeProcessingJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.ChangeProcessing;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.Migration;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
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

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetFeedChangeProcessingJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly bool handleOnlyMigrationCatchup;

    public NuGetFeedChangeProcessingJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      bool handleOnlyMigrationCatchup = false)
    {
      this.requestContext = requestContext;
      this.handleOnlyMigrationCatchup = handleOnlyMigrationCatchup;
    }

    public IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap()
    {
      ICommitLog commitLog = new NuGetCommitLogFacadeBootstrapper(this.requestContext).Bootstrap();
      return new FeedChangeProcessingJobHandlerFactoryBootstrapper(this.requestContext, (ICommitLogReader<CommitLogEntry>) commitLog, (ICommitLogWriter<ICommitLogEntry>) commitLog, Protocol.NuGet.ChangeProcessingBookmarkTokenKey, new NuGetMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), (IShouldMarkFactory) new NuGetExceptionMarkFactory(), new Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference>(this.GetBlobReference), (Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference>) null, (ICancellationFacade) new CancellationFacade(this.requestContext), NuGetServerConstants.FeedDeletedPackageJobCreationInfo, this.handleOnlyMigrationCatchup, Enumerable.Empty<IPduNotificationDetector>()).Bootstrap();
    }

    public IdBlobReference GetBlobReference(IStorageDeletionRequest<BlobStorageId> request) => new NuGetServerUtils().GetIdBlobReference(request.FeedId, (VssNuGetPackageIdentity) request.PackageIdentity);
  }
}
