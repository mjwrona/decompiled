// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Migration.NpmMigrationKickerJobBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.Migration
{
  public class NpmMigrationKickerJobBootstrapper : 
    IBootstrapper<IAsyncHandler<MigrationKickerRequest, JobResult>>
  {
    private readonly IVssRequestContext deploymentContext;
    private readonly Guid jobId;

    public NpmMigrationKickerJobBootstrapper(IVssRequestContext deploymentContext, Guid jobId)
    {
      this.deploymentContext = deploymentContext;
      this.jobId = jobId;
    }

    public IAsyncHandler<MigrationKickerRequest, JobResult> Bootstrap() => new MigrationKickerJobBootstrapper(this.deploymentContext, this.jobId, new NpmMigrationDefinitionsProviderBootstrapper(this.deploymentContext).Bootstrap(), (IFactory<CollectionId, IDisposingFeedJobQueuer>) new CollectionContextFactoryFacade<IDisposingFeedJobQueuer>(this.deploymentContext, (Func<IVssRequestContext, IDisposingFeedJobQueuer>) (collectionContext => new DisposingMigrationProcessingJobQueuerBootstrapper(collectionContext, new NpmMigrationProcessingJobQueuerBootstrapper(collectionContext).Bootstrap()).Bootstrap())), (IProtocol) Protocol.npm).Bootstrap();
  }
}
