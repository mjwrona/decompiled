// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Migration.MavenMigrationKickerJobBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server.Migration
{
  public class MavenMigrationKickerJobBootstrapper : 
    IBootstrapper<IAsyncHandler<MigrationKickerRequest, JobResult>>
  {
    private readonly IVssRequestContext deploymentContext;
    private readonly Guid jobId;

    public MavenMigrationKickerJobBootstrapper(IVssRequestContext deploymentContext, Guid jobId)
    {
      this.deploymentContext = deploymentContext;
      this.jobId = jobId;
    }

    public IAsyncHandler<MigrationKickerRequest, JobResult> Bootstrap() => new MigrationKickerJobBootstrapper(this.deploymentContext, this.jobId, new MavenMigrationDefinitionsProviderBootstrapper(this.deploymentContext).Bootstrap(), (IFactory<CollectionId, IDisposingFeedJobQueuer>) new CollectionContextFactoryFacade<IDisposingFeedJobQueuer>(this.deploymentContext, (Func<IVssRequestContext, IDisposingFeedJobQueuer>) (collectionContext => new DisposingMigrationProcessingJobQueuerBootstrapper(collectionContext, new MavenMigrationProcessingJobQueuerBootstrapper(collectionContext).Bootstrap()).Bootstrap())), (IProtocol) Protocol.Maven).Bootstrap();
  }
}
