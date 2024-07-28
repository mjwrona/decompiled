// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Migration.PyPiMigrationKickerJobBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.JobManagement;
using System;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Migration
{
  public class PyPiMigrationKickerJobBootstrapper : 
    IBootstrapper<IAsyncHandler<MigrationKickerRequest, JobResult>>
  {
    private readonly IVssRequestContext deploymentContext;
    private readonly Guid jobId;

    public PyPiMigrationKickerJobBootstrapper(IVssRequestContext deploymentContext, Guid jobId)
    {
      this.deploymentContext = deploymentContext;
      this.jobId = jobId;
    }

    public IAsyncHandler<MigrationKickerRequest, JobResult> Bootstrap() => new MigrationKickerJobBootstrapper(this.deploymentContext, this.jobId, new PyPiMigrationDefinitionsProviderBootstrapper(this.deploymentContext).Bootstrap(), (IFactory<CollectionId, IDisposingFeedJobQueuer>) new CollectionContextFactoryFacade<IDisposingFeedJobQueuer>(this.deploymentContext, (Func<IVssRequestContext, IDisposingFeedJobQueuer>) (collectionContext => new DisposingMigrationProcessingJobQueuerBootstrapper(collectionContext, new PyPiMigrationProcessingJobQueuerBootstrapper(collectionContext).Bootstrap()).Bootstrap())), (IProtocol) Protocol.PyPi).Bootstrap();
  }
}
