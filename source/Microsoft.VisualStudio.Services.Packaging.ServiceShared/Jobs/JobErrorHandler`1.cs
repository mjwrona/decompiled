// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.JobErrorHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs
{
  public class JobErrorHandler<TReq> : 
    IAsyncHandler<TReq, JobResult>,
    IHaveInputType<TReq>,
    IHaveOutputType<JobResult>
  {
    private readonly IAsyncHandler<TReq, JobResult> backingHandler;
    private readonly JobId jobId;
    private readonly Func<Exception, bool> expectedExceptionsPredicate;
    private readonly IHandler<JobQueueRequest> jobQueuer;

    public JobErrorHandler(
      IAsyncHandler<TReq, JobResult> backingHandler,
      JobId jobId,
      Func<Exception, bool> expectedExceptionsPredicate,
      IHandler<JobQueueRequest> jobQueuer)
    {
      this.backingHandler = backingHandler;
      this.jobId = jobId;
      this.expectedExceptionsPredicate = expectedExceptionsPredicate;
      this.jobQueuer = jobQueuer;
    }

    public async Task<JobResult> Handle(TReq request)
    {
      try
      {
        JobResult jobResult = await this.backingHandler.Handle(request);
        if (jobResult.Result == TeamFoundationJobExecutionResult.PartiallySucceeded || jobResult.Result == TeamFoundationJobExecutionResult.Stopped)
          this.jobQueuer.Handle(JobQueueRequest.QueueAfter(this.jobId, jobResult.RequeueAfter));
        return jobResult;
      }
      catch (JobFailedException ex) when (this.expectedExceptionsPredicate(ex.InnerException))
      {
        return this.GetJobResult(ex, true);
      }
      catch (JobFailedException ex)
      {
        this.jobQueuer.Handle(new JobQueueRequest(this.jobId));
        return this.GetJobResult(ex, false);
      }
      catch (Exception ex)
      {
        bool flag = this.expectedExceptionsPredicate(ex);
        if (!flag)
          this.jobQueuer.Handle(new JobQueueRequest(this.jobId));
        JobResult jobResult = new JobResult();
        JobTelemetry jobTelemetry = new JobTelemetry();
        jobTelemetry.LogException(ex);
        jobResult.Telemetry = jobTelemetry;
        jobResult.Result = flag ? TeamFoundationJobExecutionResult.Succeeded : TeamFoundationJobExecutionResult.Failed;
        jobResult.Telemetry.Message = string.Format("exception hit. Expected: {0}", (object) flag);
        return jobResult;
      }
    }

    private JobResult GetJobResult(JobFailedException jobFailedException, bool isExpectedException)
    {
      JobResult jobResult = new JobResult();
      jobResult.Result = isExpectedException ? TeamFoundationJobExecutionResult.Succeeded : TeamFoundationJobExecutionResult.Failed;
      jobResult.Telemetry = jobFailedException.Telemetry;
      jobResult.Telemetry.LogException(jobFailedException.InnerException);
      return jobResult;
    }
  }
}
