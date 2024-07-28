// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Migration.PyPiMigrationJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.CommitLog;
using Microsoft.VisualStudio.Services.PyPi.Server.JobManagement;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Migration
{
  public class PyPiMigrationJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFactory<int> numberOfBatchesFactory;

    public PyPiMigrationJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      IFactory<int> migrationJobNumberOfBatchesFactory = null)
    {
      this.requestContext = requestContext;
      this.numberOfBatchesFactory = migrationJobNumberOfBatchesFactory;
    }

    public IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap() => new MigrationJobHandlerFactoryBootstrapper(this.requestContext, new PyPiMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), (ICommitLogReader<CommitLogEntry>) new PyPiCommitLogFacadeBootstrapper(this.requestContext).Bootstrap(), new PyPiChangeProcessingJobQueuerBootstrapper(this.requestContext).Bootstrap(), new PyPiMigrationProcessingJobQueuerBootstrapper(this.requestContext).Bootstrap(), Protocol.PyPi.ChangeProcessingBookmarkTokenKey, this.numberOfBatchesFactory).Bootstrap();
  }
}
