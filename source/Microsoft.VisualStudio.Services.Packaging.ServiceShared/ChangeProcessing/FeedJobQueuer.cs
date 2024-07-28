// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.FeedJobQueuer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing
{
  public static class FeedJobQueuer
  {
    internal static readonly int MaxRetriesOnJobQueueFailure = 1;
    private static readonly TraceData TraceData = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.FeedJobQueuer.TraceData;
    private static readonly TimeSpan FirstRetryInterval = TimeSpan.FromMinutes(15.0);
    private static readonly TimeSpan SecondRetryInterval = TimeSpan.FromMinutes(120.0);

    public static bool QueueFeedProcessingJob(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition feedJobDefinition,
      FeedJobPriority feedJobPriority = FeedJobPriority.CatchupInitiated,
      int maxDelaySeconds = 0)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, FeedJobQueuer.TraceData, 5724500, nameof (QueueFeedProcessingJob)))
      {
        if (feedJobDefinition == null)
        {
          tracer.TraceInfo("feedJobDefinition is empty. No need to schedule.");
          return false;
        }
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        try
        {
          ITeamFoundationJobService jobService = service;
          IVssRequestContext requestContext1 = requestContext;
          List<Guid> jobIds = new List<Guid>();
          jobIds.Add(feedJobDefinition.JobId);
          int maxDelaySeconds1 = maxDelaySeconds;
          int priorityLevel = (int) feedJobPriority.GetPriorityLevel();
          jobService.QueueDelayedJobs(requestContext1, (IEnumerable<Guid>) jobIds, maxDelaySeconds1, (JobPriorityLevel) priorityLevel);
        }
        catch (JobDefinitionNotFoundException ex)
        {
          tracer.TraceInfo(string.Format("Creating feed job {0}. Job id = {1}", (object) feedJobDefinition.Name, (object) feedJobDefinition.JobId));
          FeedJobQueuer.RegisterJobDefinition(requestContext, feedJobDefinition);
          ITeamFoundationJobService jobService = service;
          IVssRequestContext requestContext2 = requestContext;
          List<Guid> jobIds = new List<Guid>();
          jobIds.Add(feedJobDefinition.JobId);
          int maxDelaySeconds2 = maxDelaySeconds;
          int priorityLevel = (int) feedJobPriority.GetPriorityLevel();
          jobService.QueueDelayedJobs(requestContext2, (IEnumerable<Guid>) jobIds, maxDelaySeconds2, (JobPriorityLevel) priorityLevel);
        }
        return true;
      }
    }

    public static void TryRetryFailedJob(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, FeedJobQueuer.TraceData, 5724530, nameof (TryRetryFailedJob)))
      {
        try
        {
          ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
          int int32 = Convert.ToInt32(FeedJobQueuer.QueryLatestJobResult(requestContext, jobDefinition) == TeamFoundationJobResult.Succeeded ? FeedJobQueuer.FirstRetryInterval.TotalSeconds : FeedJobQueuer.SecondRetryInterval.TotalSeconds);
          IVssRequestContext requestContext1 = requestContext;
          List<Guid> jobIds = new List<Guid>();
          jobIds.Add(jobDefinition.JobId);
          int maxDelaySeconds = int32;
          service.QueueDelayedJobs(requestContext1, (IEnumerable<Guid>) jobIds, maxDelaySeconds);
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
        }
      }
    }

    public static async Task TryQueueUserInitiatedFeedJob(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition feedJobDefinition)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, FeedJobQueuer.TraceData, 5724550, nameof (TryQueueUserInitiatedFeedJob)))
      {
        try
        {
          int num = await new RetryHelper(requestContext, FeedJobQueuer.MaxRetriesOnJobQueueFailure, RetryUtils.GetDefaultMaxRetryDelay(requestContext), (Func<Exception, bool>) (x => true)).Invoke<bool>((Func<Task<bool>>) (() => Task.FromResult<bool>(FeedJobQueuer.QueueFeedProcessingJob(requestContext, feedJobDefinition, FeedJobPriority.UserInitiated)))) ? 1 : 0;
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
        }
      }
    }

    public static async Task TryQueueCatchupInitiatedFeedJob(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition feedJobDefinition,
      int? failureEventId)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, FeedJobQueuer.TraceData, 5724550, nameof (TryQueueCatchupInitiatedFeedJob)))
      {
        try
        {
          int num = await new RetryHelper(requestContext, FeedJobQueuer.MaxRetriesOnJobQueueFailure, RetryUtils.GetDefaultMaxRetryDelay(requestContext), (Func<Exception, bool>) (x => true)).Invoke<bool>((Func<Task<bool>>) (() => Task.FromResult<bool>(FeedJobQueuer.QueueFeedProcessingJob(requestContext, feedJobDefinition)))) ? 1 : 0;
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          if (failureEventId.HasValue)
            TeamFoundationEventLog.Default.LogException(requestContext, "Failed to queue the job " + feedJobDefinition?.Name + ". Reason:" + ex.Message, ex, failureEventId.Value, EventLogEntryType.Error);
        }
      }
    }

    private static TeamFoundationJobResult QueryLatestJobResult(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, FeedJobQueuer.TraceData, 5724540, nameof (QueryLatestJobResult)))
      {
        List<TeamFoundationJobHistoryEntry> source = requestContext.GetService<ITeamFoundationJobService>().QueryLatestJobHistory(requestContext, (IEnumerable<Guid>) new List<Guid>()
        {
          jobDefinition.JobId
        });
        return (source != null ? source.First<TeamFoundationJobHistoryEntry>()?.Result : new TeamFoundationJobResult?()).GetValueOrDefault();
      }
    }

    private static void RegisterJobDefinition(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition feedJobDefintion)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, FeedJobQueuer.TraceData, 5724510, nameof (RegisterJobDefinition)))
      {
        try
        {
          requestContext.GetService<ITeamFoundationJobService>().UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
          {
            feedJobDefintion
          });
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
    }
  }
}
