// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.JobHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce.Common
{
  internal static class JobHelper
  {
    private const string Area = "Commerce";
    private const string Layer = "JobHelper";

    internal static bool DoesJobDefinitionExist(IVssRequestContext requestContext, Guid jobGuid)
    {
      TeamFoundationJobDefinition foundationJobDefinition = requestContext.GetService<ITeamFoundationJobService>().QueryJobDefinition(requestContext, jobGuid);
      int num = foundationJobDefinition != null ? 1 : 0;
      string message = num != 0 ? string.Format("Job definition for {0} already exists: {1}.", (object) jobGuid, (object) foundationJobDefinition) : string.Format("Job definition for {0} doesn't exist.", (object) jobGuid);
      requestContext.Trace(5108868, TraceLevel.Info, "Commerce", nameof (JobHelper), message);
      return num != 0;
    }

    internal static void CreateJobDefinition(
      IVssRequestContext requestContext,
      Guid jobGuid,
      string jobName,
      string jobExtensionName,
      JobPriorityClass jobPriorityClass = JobPriorityClass.Normal)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(jobGuid, jobName, jobExtensionName)
      {
        PriorityClass = jobPriorityClass
      };
      IVssRequestContext requestContext1 = requestContext;
      TeamFoundationJobDefinition[] jobUpdates = new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      };
      service.UpdateJobDefinitions(requestContext1, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      requestContext.Trace(5108868, TraceLevel.Info, "Commerce", nameof (JobHelper), string.Format("Created new job definition: {0} for job {1}.", (object) foundationJobDefinition, (object) jobGuid));
    }

    internal static bool IsJobScheduled(IVssRequestContext requestContext, Guid jobGuid)
    {
      TeamFoundationJobQueueEntry foundationJobQueueEntry = requestContext.GetService<ITeamFoundationJobService>().QueryJobQueue(requestContext, jobGuid);
      bool flag = foundationJobQueueEntry != null && foundationJobQueueEntry.State == TeamFoundationJobState.QueuedScheduled;
      requestContext.Trace(5108870, TraceLevel.Info, "Commerce", nameof (JobHelper), string.Format("Is job {0} scheduled: {1}.", (object) jobGuid, (object) flag));
      return flag;
    }

    internal static void QueueDelayedJob(
      IVssRequestContext requestContext,
      Guid jobGuid,
      TimeSpan scheduleDelay)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      int num1 = (int) Math.Ceiling(scheduleDelay.TotalSeconds);
      IVssRequestContext requestContext1 = requestContext;
      Guid[] jobIds = new Guid[1]{ jobGuid };
      int maxDelaySeconds = num1;
      int num2 = service.QueueDelayedJobs(requestContext1, (IEnumerable<Guid>) jobIds, maxDelaySeconds);
      requestContext.Trace(5108870, TraceLevel.Info, "Commerce", nameof (JobHelper), string.Format("Attempted to queue job {0}; successfully queued jobs is: {1}.", (object) jobGuid, (object) num2));
    }
  }
}
