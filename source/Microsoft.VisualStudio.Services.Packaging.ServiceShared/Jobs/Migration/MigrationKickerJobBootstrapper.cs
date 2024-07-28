// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration.MigrationKickerJobBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration
{
  public class MigrationKickerJobBootstrapper : 
    IBootstrapper<IAsyncHandler<MigrationKickerRequest, JobResult>>
  {
    private readonly IVssRequestContext deploymentContext;
    private readonly Guid jobId;
    private readonly IMigrationDefinitionsProvider migrationDefinitionsProvider;
    private readonly IFactory<CollectionId, IDisposingFeedJobQueuer> jobQueuerFactory;
    private readonly IProtocol protocol;

    public MigrationKickerJobBootstrapper(
      IVssRequestContext deploymentContext,
      Guid jobId,
      IMigrationDefinitionsProvider migrationDefinitionsProvider,
      IFactory<CollectionId, IDisposingFeedJobQueuer> jobQueuerFactory,
      IProtocol protocol)
    {
      this.deploymentContext = deploymentContext;
      this.jobId = jobId;
      this.migrationDefinitionsProvider = migrationDefinitionsProvider;
      this.jobQueuerFactory = jobQueuerFactory;
      this.protocol = protocol;
    }

    public IAsyncHandler<MigrationKickerRequest, JobResult> Bootstrap() => (IAsyncHandler<MigrationKickerRequest, JobResult>) new MigrationKickerJobHandler((IFactory<IMigrationTransitionerInternal>) new ByFuncFactory<IMigrationTransitionerInternal>((Func<IMigrationTransitionerInternal>) (() => new NoCachingMigrationTransitionerBootstrapper(this.deploymentContext, this.migrationDefinitionsProvider).Bootstrap())), this.jobQueuerFactory, this.protocol, this.deploymentContext.GetTracerFacade());
  }
}
