// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.JobHelper
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  public class JobHelper
  {
    private const string c_layer = "JobHelper";

    public static void QueueJob(
      IVssRequestContext requestContext,
      string jobName,
      string extensionName,
      XmlNode jobData)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      try
      {
        requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext, jobName, extensionName, jobData, JobPriorityLevel.Highest);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, TraceLevel.Error, "Pipeline.Checks", nameof (JobHelper), ex);
        throw;
      }
    }

    public static void ScheduleJob(
      IVssRequestContext requestContext,
      Guid jobId,
      string jobName,
      string extensionName,
      XmlNode jobData,
      DateTime scheduleTime)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(jobId, jobName, extensionName, jobData);
      if (foundationJobDefinition == null)
        throw new ArgumentNullException("job");
      TeamFoundationJobSchedule foundationJobSchedule = new TeamFoundationJobSchedule()
      {
        PriorityLevel = JobPriorityLevel.Normal,
        ScheduledTime = scheduleTime,
        Interval = 0
      };
      foundationJobDefinition.Schedule.Add(foundationJobSchedule);
      foundationJobDefinition.EnabledState = TeamFoundationJobEnabledState.Enabled;
      requestContext.GetService<ITeamFoundationJobService>().UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      });
    }

    public static void CancelJobs(IVssRequestContext requestContext, IEnumerable<Guid> jobIds)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      requestContext.GetService<ITeamFoundationJobService>().DeleteJobDefinitions(requestContext, jobIds);
    }
  }
}
