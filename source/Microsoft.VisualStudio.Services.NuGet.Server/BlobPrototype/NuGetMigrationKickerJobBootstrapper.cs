// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetMigrationKickerJobBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetMigrationKickerJobBootstrapper : 
    IBootstrapper<IAsyncHandler<MigrationKickerRequest, JobResult>>
  {
    private readonly IVssRequestContext deploymentContext;
    private readonly Guid jobId;

    public NuGetMigrationKickerJobBootstrapper(IVssRequestContext deploymentContext, Guid jobId)
    {
      this.deploymentContext = deploymentContext;
      this.jobId = jobId;
    }

    public IAsyncHandler<MigrationKickerRequest, JobResult> Bootstrap() => new MigrationKickerJobBootstrapper(this.deploymentContext, this.jobId, new NuGetMigrationDefinitionsProviderBootstrapper(this.deploymentContext).Bootstrap(), (IFactory<CollectionId, IDisposingFeedJobQueuer>) new CollectionContextFactoryFacade<IDisposingFeedJobQueuer>(this.deploymentContext, (Func<IVssRequestContext, IDisposingFeedJobQueuer>) (collectionContext => new DisposingMigrationProcessingJobQueuerBootstrapper(collectionContext, new NuGetMigrationProcessingJobQueuerBootstrapper(collectionContext).Bootstrap()).Bootstrap())), (IProtocol) Protocol.NuGet).Bootstrap();
  }
}
