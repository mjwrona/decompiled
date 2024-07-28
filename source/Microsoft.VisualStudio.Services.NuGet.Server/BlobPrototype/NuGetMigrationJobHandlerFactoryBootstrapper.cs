// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetMigrationJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog;
using Microsoft.VisualStudio.Services.NuGet.Server.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetMigrationJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFactory<int> numberOfBatchesFactory;

    public NuGetMigrationJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      IFactory<int> migrationJobNumberOfBatchesFactory = null)
    {
      this.requestContext = requestContext;
      this.numberOfBatchesFactory = migrationJobNumberOfBatchesFactory;
    }

    public IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap() => new MigrationJobHandlerFactoryBootstrapper(this.requestContext, new NuGetMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), (ICommitLogReader<CommitLogEntry>) new NuGetCommitLogFacadeBootstrapper(this.requestContext).Bootstrap(), new NuGetChangeProcessingJobQueuerBootstrapper(this.requestContext).Bootstrap(), new NuGetMigrationProcessingJobQueuerBootstrapper(this.requestContext).Bootstrap(), Protocol.NuGet.ChangeProcessingBookmarkTokenKey, this.numberOfBatchesFactory).Bootstrap();
  }
}
