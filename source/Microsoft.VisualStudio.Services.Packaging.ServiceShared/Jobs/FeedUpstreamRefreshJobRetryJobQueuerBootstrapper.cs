// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.FeedUpstreamRefreshJobRetryJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs
{
  public class FeedUpstreamRefreshJobRetryJobQueuerBootstrapper : IBootstrapper<IDoer>
  {
    private readonly IVssRequestContext requestContext;
    private readonly JobId jobId;

    public FeedUpstreamRefreshJobRetryJobQueuerBootstrapper(
      IVssRequestContext requestContext,
      JobId jobId)
    {
      this.requestContext = requestContext;
      this.jobId = jobId;
    }

    public IDoer Bootstrap()
    {
      bool flag = PackagingServerConstants.AllowMultipleFeedUpstreamRefreshJobRetriesSetting.Bootstrap(this.requestContext).Get();
      JobServiceFacade jobServiceFacade = new JobServiceFacade(this.requestContext, this.requestContext.GetService<ITeamFoundationJobService>());
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      IHandler<JobQueueRequest> retryJobQueuingHandler = flag ? (IHandler<JobQueueRequest>) new QueueJobWithDelayBasedOnPreviousResultHandler((IJobQueuer) jobServiceFacade, tracerFacade, new JobRunSettings()) : (IHandler<JobQueueRequest>) new OneTimeJobOneShotRetryQueueingHandler((IJobQueuer) jobServiceFacade, tracerFacade, new JobRunSettings());
      return (IDoer) new ByActionDoer((Action) (() => retryJobQueuingHandler.Handle(new JobQueueRequest(this.jobId))));
    }
  }
}
