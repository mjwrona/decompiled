// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class TeamFoundationJobServiceExtensions
  {
    public static TeamFoundationJobDefinition QueryJobDefinition(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      Guid jobId)
    {
      List<TeamFoundationJobDefinition> foundationJobDefinitionList = jobService.QueryJobDefinitions(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        jobId
      });
      return foundationJobDefinitionList == null || foundationJobDefinitionList.Count == 0 ? (TeamFoundationJobDefinition) null : foundationJobDefinitionList[0];
    }

    public static void UpdateJobDefinitions(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobDefinition> jobUpdates)
    {
      jobService.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, jobUpdates);
    }

    public static void DeleteJobDefinitions(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobsToDelete)
    {
      jobService.UpdateJobDefinitions(requestContext, jobsToDelete, (IEnumerable<TeamFoundationJobDefinition>) null);
    }

    public static int QueueDelayedJobs(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds)
    {
      List<TeamFoundationJobReference> jobReferences = TeamFoundationJobReference.ConvertJobIdsToJobReferences(jobIds);
      return jobService.QueueDelayedJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences, -1);
    }

    public static int QueueDelayedJobs(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds,
      int maxDelaySeconds,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal)
    {
      return jobService.QueueDelayedJobs(requestContext, jobIds, maxDelaySeconds, priorityLevel, false);
    }

    public static int QueueDelayedJobs(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds,
      int maxDelaySeconds,
      JobPriorityLevel priorityLevel,
      bool queueAsDormant)
    {
      List<TeamFoundationJobReference> jobReferences = TeamFoundationJobReference.ConvertJobIdsToJobReferences(jobIds);
      return jobService.QueueJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences, priorityLevel, maxDelaySeconds, queueAsDormant);
    }

    public static int QueueDelayedJobs(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobReference> jobReferences,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal)
    {
      return jobService.QueueJobs(requestContext, jobReferences, priorityLevel, -1, false);
    }

    public static int QueueDelayedJobs(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobReference> jobReferences,
      int maxDelaySeconds,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal)
    {
      return jobService.QueueJobs(requestContext, jobReferences, priorityLevel, maxDelaySeconds, false);
    }

    public static int QueueJobsNow(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds,
      bool highPriority)
    {
      List<TeamFoundationJobReference> jobReferences = TeamFoundationJobReference.ConvertJobIdsToJobReferences(jobIds);
      JobPriorityLevel level = TeamFoundationJobService.ConvertPriorityBitToLevel(highPriority);
      return jobService.QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences, level);
    }

    public static int QueueJobsNow(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal,
      bool queueAsDormant = false)
    {
      List<TeamFoundationJobReference> jobReferences = TeamFoundationJobReference.ConvertJobIdsToJobReferences(jobIds);
      return jobService.QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences, priorityLevel, queueAsDormant);
    }

    public static int QueueJobsNow(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobReference> jobReferences,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal,
      bool queueAsDormant = false)
    {
      return jobService.QueueJobs(requestContext, jobReferences, priorityLevel, 0, queueAsDormant);
    }

    public static List<TeamFoundationJobHistoryEntry> QueryJobHistory(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      Guid jobId)
    {
      return jobService.QueryJobHistory(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        jobId
      });
    }

    public static TeamFoundationJobHistoryEntry QueryLatestJobHistory(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      Guid jobId)
    {
      List<TeamFoundationJobHistoryEntry> foundationJobHistoryEntryList = jobService.QueryLatestJobHistory(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        jobId
      });
      return foundationJobHistoryEntryList == null || foundationJobHistoryEntryList.Count == 0 ? (TeamFoundationJobHistoryEntry) null : foundationJobHistoryEntryList[0];
    }

    public static TeamFoundationJobQueueEntry QueryJobQueue(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      Guid jobId)
    {
      return jobService.QueryJobQueue(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        jobId
      })[0];
    }

    public static Guid QueueOneTimeJob(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      string jobName,
      string extensionName,
      XmlNode jobData,
      bool highPriority)
    {
      JobPriorityLevel level = TeamFoundationJobService.ConvertPriorityBitToLevel(highPriority);
      return jobService.QueueOneTimeJob(requestContext, jobName, extensionName, jobData, level, TimeSpan.Zero);
    }

    public static Guid QueueOneTimeJob(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      string jobName,
      string extensionName,
      XmlNode jobData,
      JobPriorityLevel priorityLevel)
    {
      return jobService.QueueOneTimeJob(requestContext, jobName, extensionName, jobData, priorityLevel, TimeSpan.Zero);
    }

    public static Guid QueueOneTimeJob(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      string jobName,
      string extensionName,
      XmlNode jobData,
      TimeSpan startOffset)
    {
      return jobService.QueueOneTimeJob(requestContext, jobName, extensionName, jobData, JobPriorityLevel.Normal, startOffset);
    }

    public static Guid QueueOneTimeJob(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      string jobName,
      string extensionName,
      XmlNode jobData,
      JobPriorityLevel priorityLevel,
      TimeSpan startOffset)
    {
      requestContext.Trace(1030, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "JobService.QueueOneTimeJob jobName {0} extensionName {1} jobData {2} priorityLevel {3} startOffset {4}", (object) jobName, (object) extensionName, (object) jobData, (object) priorityLevel, (object) startOffset);
      return jobService.QueueOneTimeJob(requestContext, jobName, extensionName, jobData, priorityLevel, JobPriorityClass.None, startOffset);
    }

    public static Guid QueueOneTimeJob(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      string jobName,
      string extensionName,
      XmlNode jobData,
      JobPriorityLevel priorityLevel,
      JobPriorityClass priorityClass,
      TimeSpan startOffset)
    {
      requestContext.Trace(1030, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "JobService.QueueOneTimeJob jobName {0} extensionName {1} jobData {2} priorityLevel {3} priorityClass {4} startOffset {5}", (object) jobName, (object) extensionName, (object) jobData, (object) priorityLevel, (object) priorityClass, (object) startOffset);
      Guid jobId = Guid.NewGuid();
      jobService.QueueOneTimeJob(requestContext, jobName, extensionName, jobId, jobData, priorityLevel, priorityClass, startOffset);
      return jobId;
    }

    public static bool QueueOneTimeJob(
      this ITeamFoundationJobService jobService,
      IVssRequestContext requestContext,
      string jobName,
      string extensionName,
      Guid jobId,
      XmlNode jobData,
      JobPriorityLevel priorityLevel,
      JobPriorityClass priorityClass,
      TimeSpan startOffset)
    {
      if (jobId == Guid.Empty)
        throw new ArgumentException(FrameworkResources.InvalidOneTimeJobIdError((object) jobId));
      requestContext.Trace(1030, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "JobService.QueueOneTimeJob jobName {0} extensionName {1} jobData {2} priorityLevel {3} priorityClass {4} startOffset {5} jobId {6}", (object) jobName, (object) extensionName, (object) jobData, (object) priorityLevel, (object) priorityClass, (object) startOffset, (object) jobId);
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(jobId, jobName, extensionName, jobData, TeamFoundationJobEnabledState.Enabled, true, false, priorityClass);
      requestContext.Trace(1031, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Updating Job Definition {0}", (object) foundationJobDefinition);
      jobService.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      });
      requestContext.Trace(1032, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Queuing Job {0}", (object) foundationJobDefinition.JobId);
      IEnumerable<TeamFoundationJobReference> jobReferences = (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        foundationJobDefinition.ToJobReference()
      };
      return jobService.QueueJobs(requestContext, jobReferences, priorityLevel, (int) startOffset.TotalSeconds, false) == 1;
    }
  }
}
