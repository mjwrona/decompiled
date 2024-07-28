// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.JobErrorHandlerBootstrapper`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs
{
  public class JobErrorHandlerBootstrapper<TRequest> : 
    IBootstrapper<IAsyncHandler<TRequest, JobResult>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IAsyncHandler<TRequest, JobResult> jobHandler;
    private readonly JobId currentRunningJobId;
    private static readonly IEnumerable<Type> KnownExceptionTypes = (IEnumerable<Type>) new List<Type>()
    {
      typeof (FeedIdNotFoundException)
    };

    public JobErrorHandlerBootstrapper(
      IVssRequestContext requestContext,
      IAsyncHandler<TRequest, JobResult> jobHandler,
      JobId currentRunningJobId)
    {
      this.requestContext = requestContext;
      this.jobHandler = jobHandler;
      this.currentRunningJobId = currentRunningJobId;
    }

    public IAsyncHandler<TRequest, JobResult> Bootstrap()
    {
      JobRunSettings jobRunSettings = new JobRunSettings();
      return (IAsyncHandler<TRequest, JobResult>) new JobErrorHandler<TRequest>(this.jobHandler, this.currentRunningJobId, (Func<Exception, bool>) (e => JobErrorHandlerBootstrapper<TRequest>.KnownExceptionTypes.Contains<Type>(e?.GetType())), (IHandler<JobQueueRequest>) new QueueJobWithDelayBasedOnPreviousResultHandler((IJobQueuer) new JobServiceFacade(this.requestContext, this.requestContext.GetService<ITeamFoundationJobService>()), this.requestContext.GetTracerFacade(), jobRunSettings));
    }
  }
}
