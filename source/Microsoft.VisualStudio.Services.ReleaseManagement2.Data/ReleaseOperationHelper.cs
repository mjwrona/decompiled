// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.ReleaseOperationHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data
{
  public static class ReleaseOperationHelper
  {
    public static void CreateJob(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition definition)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ReleaseOperationHelper.GetJobService(vssRequestContext).UpdateJobDefinitions(vssRequestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        definition
      });
      ReleaseOperationHelper.TraceJobCreation(requestContext, definition);
    }

    public static void UpdateJobs(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobDefinition> jobsToUpdate,
      IEnumerable<Guid> jobsToDelete)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if ((jobsToUpdate == null || !jobsToUpdate.Any<TeamFoundationJobDefinition>()) && (jobsToDelete == null || !jobsToDelete.Any<Guid>()))
        return;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ReleaseOperationHelper.GetJobService(vssRequestContext).UpdateJobDefinitions(vssRequestContext, jobsToDelete, jobsToUpdate);
      ReleaseOperationHelper.TraceJobUpdates(requestContext, jobsToUpdate, jobsToDelete);
    }

    public static ITeamFoundationJobService GetJobService(IVssRequestContext elevatedContext) => elevatedContext != null ? elevatedContext.GetService<ITeamFoundationJobService>() : throw new ArgumentNullException(nameof (elevatedContext));

    public static void DeleteJobDefinition(
      IVssRequestContext requestContext,
      Guid jobId,
      int releaseId)
    {
      ReleaseOperationHelper.DeleteJobDefinition(requestContext, (IEnumerable<Guid>) new List<Guid>()
      {
        jobId
      }, (IEnumerable<int>) new List<int>() { releaseId });
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is logged for diagnosability")]
    public static void DeleteJobDefinition(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds,
      IEnumerable<int> releaseIds)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (jobIds == null)
        return;
      IEnumerable<Guid> guids = jobIds.Where<Guid>((Func<Guid, bool>) (jobId => !jobId.Equals(Guid.Empty)));
      if (!guids.Any<Guid>())
        return;
      string str1 = string.Join<int>(",", releaseIds);
      string str2 = string.Join<Guid>(",", guids);
      try
      {
        requestContext.Trace(1960102, TraceLevel.Info, "ReleaseManagementService", "Pipeline", "ReleaseOperationHelper: DeleteJobDefinition: jobId(s): {0}, Release Id(s): {1}", (object) str2, (object) str1);
        ReleaseOperationHelper.GetJobService(requestContext.Elevate()).DeleteJobDefinitions(requestContext, guids);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1960102, TraceLevel.Error, "ReleaseManagementService", "Pipeline", "ReleaseOperationHelper: DeleteJobDefinition: Throwed exception {0}, jobId(s): {1}, Release Id(s): {2}", (object) ex, (object) str2, (object) str1);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void TraceJobCreation(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition definition)
    {
      requestContext.Trace(1960101, TraceLevel.Info, "ReleaseManagementService", "Service", "Created Job Definition : {0} with JobData : {1}", (object) definition.ToString(), (object) definition.Data.OuterXml);
      foreach (TeamFoundationJobSchedule foundationJobSchedule in definition.Schedule)
        requestContext.Trace(1960101, TraceLevel.Info, "ReleaseManagementService", "Service", "Job {0} Scheduled to Start at : {1} with Interval : {2}", (object) definition.JobId, (object) foundationJobSchedule.ScheduledTime, (object) foundationJobSchedule.Interval);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void TraceJobUpdates(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobDefinition> jobsToUpdate,
      IEnumerable<Guid> jobsToDelete)
    {
      int num1 = jobsToUpdate == null ? 0 : jobsToUpdate.Count<TeamFoundationJobDefinition>();
      int num2 = jobsToDelete == null ? 0 : jobsToDelete.Count<Guid>();
      requestContext.Trace(1960102, TraceLevel.Info, "ReleaseManagementService", "Service", "Updated {0} Job Definitions and Deleted {1} Job Definitions", (object) num1, (object) num2);
    }
  }
}
