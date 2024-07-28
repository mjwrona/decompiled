// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.CargoMigrationKickerJobBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations
{
  public class CargoMigrationKickerJobBootstrapper : 
    IBootstrapper<IAsyncHandler<MigrationKickerRequest, JobResult>>
  {
    private readonly IVssRequestContext deploymentContext;
    private readonly Guid jobId;

    public CargoMigrationKickerJobBootstrapper(IVssRequestContext deploymentContext, Guid jobId)
    {
      this.deploymentContext = deploymentContext;
      this.jobId = jobId;
    }

    public IAsyncHandler<MigrationKickerRequest, JobResult> Bootstrap() => new MigrationKickerJobBootstrapper(this.deploymentContext, this.jobId, new CargoMigrationDefinitionsProviderBootstrapper(this.deploymentContext).Bootstrap(), (IFactory<CollectionId, IDisposingFeedJobQueuer>) new CollectionContextFactoryFacade<IDisposingFeedJobQueuer>(this.deploymentContext, (Func<IVssRequestContext, IDisposingFeedJobQueuer>) (collectionContext => new DisposingMigrationProcessingJobQueuerBootstrapper(collectionContext, new CargoMigrationProcessingJobQueuerBootstrapper(collectionContext).Bootstrap()).Bootstrap())), (IProtocol) Protocol.Cargo).Bootstrap();
  }
}
