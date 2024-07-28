// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.CargoMigrationJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.CommitLog;
using Microsoft.VisualStudio.Services.Cargo.Server.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations
{
  public class CargoMigrationJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFactory<int>? numberOfBatchesFactory;

    public CargoMigrationJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      IFactory<int>? migrationJobNumberOfBatchesFactory = null)
    {
      this.requestContext = requestContext;
      this.numberOfBatchesFactory = migrationJobNumberOfBatchesFactory;
    }

    public IFactory<JobId?, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap() => new MigrationJobHandlerFactoryBootstrapper(this.requestContext, new CargoMigrationDefinitionsProviderBootstrapper(this.requestContext).Bootstrap(), (ICommitLogReader<CommitLogEntry>) new CargoCommitLogFacadeBootstrapper(this.requestContext).Bootstrap(), new CargoChangeProcessingJobQueuerBootstrapper(this.requestContext).Bootstrap(), new CargoMigrationProcessingJobQueuerBootstrapper(this.requestContext).Bootstrap(), Protocol.Cargo.ChangeProcessingBookmarkTokenKey, this.numberOfBatchesFactory).Bootstrap();
  }
}
