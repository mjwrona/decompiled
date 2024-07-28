// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Migration.NpmMigrationJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Npm.Server.Migration
{
  public class NpmMigrationJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFactory<int> numberOfBatchesFactory;

    public NpmMigrationJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      IFactory<int> migrationJobNumberOfBatchesFactory = null)
    {
      this.requestContext = requestContext;
      this.numberOfBatchesFactory = migrationJobNumberOfBatchesFactory;
    }

    public IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap() => new MigrationJobHandlerFactoryBootstrapper(this.requestContext, new NpmMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), (ICommitLogReader<CommitLogEntry>) new NpmCommitLogFacadeBootstrapper(this.requestContext).Bootstrap(), new NpmChangeProcessingJobQueuerBootstrapper(this.requestContext).Bootstrap(), new NpmMigrationProcessingJobQueuerBootstrapper(this.requestContext).Bootstrap(), Protocol.npm.ChangeProcessingBookmarkTokenKey, this.numberOfBatchesFactory).Bootstrap();
  }
}
