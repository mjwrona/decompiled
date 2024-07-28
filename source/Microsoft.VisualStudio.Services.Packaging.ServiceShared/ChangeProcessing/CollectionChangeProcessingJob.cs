// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.CollectionChangeProcessingJob
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing
{
  public abstract class CollectionChangeProcessingJob : VssAsyncJobExtension
  {
    internal static readonly int MaxTryCount = 2;
    private static readonly TraceData TraceData = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.CollectionChangeProcessingJob.TraceData;

    public abstract string ExtensionName { get; }

    internal static TimeSpan MaximumWaitBeforeRetry { get; set; } = TimeSpan.FromSeconds(30.0);

    public abstract IList<TeamFoundationJobDefinition> GetFeedChangeJobDefinitions(
      IVssRequestContext requestContext);

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, CollectionChangeProcessingJob.TraceData, 5724600, nameof (RunAsync)))
      {
        CollectionChangeProcessingJobTelemetryData jobTelemetryData = new CollectionChangeProcessingJobTelemetryData();
        jobTelemetryData.MaxTryCount = CollectionChangeProcessingJob.MaxTryCount;
        CollectionChangeProcessingJobTelemetryData telemetryData = jobTelemetryData;
        try
        {
          int num = await new RetryHelper(requestContext, CollectionChangeProcessingJob.MaxTryCount - 1, CollectionChangeProcessingJob.MaximumWaitBeforeRetry, (Func<Exception, bool>) (exception => true)).Invoke<bool>((Func<Task<bool>>) (() => Task.FromResult<bool>(this.RunInternal(requestContext, jobDefinition, queueTime, telemetryData)))) ? 1 : 0;
          return JobResult.Succeeded((JobTelemetry) telemetryData).ToVssJobResult();
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          FeedJobQueuer.TryRetryFailedJob(requestContext, jobDefinition);
          telemetryData.LogException(ex);
          return JobResult.Failed((JobTelemetry) telemetryData).ToVssJobResult();
        }
      }
    }

    private bool RunInternal(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      CollectionChangeProcessingJobTelemetryData telemetryData)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, CollectionChangeProcessingJob.TraceData, 5724620, nameof (RunInternal)))
      {
        ++telemetryData.ExecutedTryCount;
        int num = 0;
        IList<TeamFoundationJobDefinition> changeJobDefinitions = this.GetFeedChangeJobDefinitions(requestContext);
        IRegistryWriterService registryFacade = requestContext.GetRegistryFacade();
        DefaultRandomProvider rng = new DefaultRandomProvider();
        foreach (TeamFoundationJobDefinition jobDefinition1 in (IEnumerable<TeamFoundationJobDefinition>) changeJobDefinitions)
        {
          if (!this.QueueChangeProcessingJob(requestContext, (IRegistryService) registryFacade, (IRandomProvider) rng, jobDefinition1))
          {
            telemetryData.LogErrorMessage("Failed to queue the job #" + jobDefinition1.Name, nameof (RunInternal), 83);
          }
          else
          {
            tracer.TraceInfo("Successfully queued the job #" + jobDefinition1.Name);
            ++num;
          }
        }
        telemetryData.FeedJobsCount = changeJobDefinitions.Count;
        telemetryData.ScheduledFeedJobsCount = num;
        if (num < changeJobDefinitions.Count)
          throw new ChangeProcessingJobFailedException(jobDefinition.JobId, "Failed to queue at least one job");
        return true;
      }
    }

    private bool QueueChangeProcessingJob(
      IVssRequestContext requestContext,
      IRegistryService registryService,
      IRandomProvider rng,
      TeamFoundationJobDefinition jobDefinition)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, CollectionChangeProcessingJob.TraceData, 5724610, nameof (QueueChangeProcessingJob)))
      {
        try
        {
          FeedJobQueuer.QueueFeedProcessingJob(requestContext, jobDefinition, maxDelaySeconds: rng.Next(0, JobQueueingUtils.GetFeedJobSpreadSeconds(registryService, jobDefinition.ExtensionName)));
          return true;
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          return false;
        }
      }
    }
  }
}
