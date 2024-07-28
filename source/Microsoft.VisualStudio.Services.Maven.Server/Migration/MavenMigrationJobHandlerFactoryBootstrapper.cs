// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Migration.MavenMigrationJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Maven.Server.Migration
{
  public class MavenMigrationJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFactory<int> numberOfBatchesFactory;

    public MavenMigrationJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      IFactory<int> migrationJobNumberOfBatchesFactory = null)
    {
      this.requestContext = requestContext;
      this.numberOfBatchesFactory = migrationJobNumberOfBatchesFactory;
    }

    public IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap() => new MigrationJobHandlerFactoryBootstrapper(this.requestContext, new MavenMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), (ICommitLogReader<CommitLogEntry>) new MavenCommitLogFacadeBootstrapper(this.requestContext).Bootstrap(), new MavenChangeProcessingJobQueuerBootstrapper(this.requestContext).Bootstrap(), new MavenMigrationProcessingJobQueuerBootstrapper(this.requestContext).Bootstrap(), Protocol.Maven.ChangeProcessingBookmarkTokenKey, this.numberOfBatchesFactory).Bootstrap();
  }
}
