// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Api.IVssRequestContextJobExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Api, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 95106129-8DAA-451C-9244-BDEB920E7E03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.Api.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Api
{
  internal static class IVssRequestContextJobExtensions
  {
    internal static void DisableJob(
      this IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      jobDefinition.EnabledState = TeamFoundationJobEnabledState.SchedulesDisabled;
      Stopwatch stopwatch = Stopwatch.StartNew();
      IVssRequestContext requestContext1 = requestContext;
      TeamFoundationJobDefinition[] jobUpdates = new TeamFoundationJobDefinition[1]
      {
        jobDefinition
      };
      service.UpdateJobDefinitions(requestContext1, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("UpdateJobDefinitionsCallTime", "Indexing Pipeline", (double) stopwatch.ElapsedMilliseconds);
    }

    internal static TeamFoundationJobQueueEntry QueryJobQueue(
      this IVssRequestContext requestContext,
      Guid jobId)
    {
      IEnumerable<TeamFoundationJobQueueEntry> source = requestContext.QueryJobQueue((IEnumerable<Guid>) new List<Guid>()
      {
        jobId
      });
      return source != null ? source.FirstOrDefault<TeamFoundationJobQueueEntry>() : (TeamFoundationJobQueueEntry) null;
    }

    internal static IEnumerable<TeamFoundationJobQueueEntry> QueryJobQueue(
      this IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds)
    {
      return (IEnumerable<TeamFoundationJobQueueEntry>) requestContext.GetService<ITeamFoundationJobService>().QueryJobQueue(requestContext, jobIds);
    }

    internal static void QueueDelayedNamedJob(
      this IVssRequestContext requestContext,
      Guid jobId,
      int delayInSeconds,
      JobPriorityLevel jobPriority)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      JobQueueController jobQueueController = new JobQueueController(requestContext);
      delayInSeconds += (int) jobQueueController.GetQueueDelayFactor().TotalSeconds;
      Stopwatch stopwatch = Stopwatch.StartNew();
      JobPriorityLevel accountBasedPriority = new PriorityManager().GetAccountBasedPriority(requestContext);
      if (accountBasedPriority != JobPriorityLevel.None)
        jobPriority = accountBasedPriority;
      IVssRequestContext requestContext1 = requestContext;
      Guid[] jobIds = new Guid[1]{ jobId };
      int maxDelaySeconds = delayInSeconds;
      int priorityLevel = (int) jobPriority;
      service.QueueDelayedJobs(requestContext1, (IEnumerable<Guid>) jobIds, maxDelaySeconds, (JobPriorityLevel) priorityLevel);
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("QueueJobCallTime", "Indexing Pipeline", (double) stopwatch.ElapsedMilliseconds);
    }

    internal static Guid QueueDelayedOneTimeJob(
      this IVssRequestContext requestContext,
      string jobName,
      string jobExtension,
      XmlNode jobData,
      int delayInSeconds,
      JobPriorityLevel jobPriority,
      JobPriorityClass jobPriorityClass)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      JobQueueController jobQueueController = new JobQueueController(requestContext);
      delayInSeconds += (int) jobQueueController.GetQueueDelayFactor().TotalSeconds;
      TimeSpan timeSpan = new TimeSpan(0, 0, delayInSeconds);
      Stopwatch stopwatch = Stopwatch.StartNew();
      JobPriorityLevel accountBasedPriority = new PriorityManager().GetAccountBasedPriority(requestContext);
      if (accountBasedPriority != JobPriorityLevel.None)
        jobPriority = accountBasedPriority;
      IVssRequestContext requestContext1 = requestContext;
      string jobName1 = jobName;
      string extensionName = jobExtension;
      XmlNode jobData1 = jobData;
      int priorityLevel = (int) jobPriority;
      int priorityClass = (int) jobPriorityClass;
      TimeSpan startOffset = timeSpan;
      Guid guid = service.QueueOneTimeJob(requestContext1, jobName1, extensionName, jobData1, (JobPriorityLevel) priorityLevel, (JobPriorityClass) priorityClass, startOffset);
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("QueueJobCallTime", "Indexing Pipeline", (double) stopwatch.ElapsedMilliseconds);
      return guid;
    }

    internal static void QueuePeriodicCatchUpJob(
      this IVssRequestContext requestContext,
      int jobDelay)
    {
      requestContext.QueueDelayedNamedJob(JobConstants.PeriodicCatchUpJobId, jobDelay, JobPriorityLevel.Normal);
    }

    internal static void QueuePeriodicWikiCatchUpJob(
      this IVssRequestContext requestContext,
      int jobDelay)
    {
      requestContext.QueueDelayedNamedJob(JobConstants.PeriodicWikiCatchUpJobId, jobDelay, JobPriorityLevel.Normal);
    }

    internal static void QueuePeriodicMaintenanceJob(
      this IVssRequestContext requestContext,
      int jobDelay)
    {
      requestContext.QueueDelayedNamedJob(JobConstants.PeriodicMaintenanceJobId, jobDelay, JobPriorityLevel.Normal);
    }

    internal static void UpdateJobDefinition(
      this IVssRequestContext requestContext,
      XmlNode jobData,
      Guid jobGuid)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = service.QueryJobDefinition(requestContext, jobGuid);
      foundationJobDefinition.Data = jobData;
      service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) new Guid[0], (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
      {
        foundationJobDefinition
      });
    }
  }
}
