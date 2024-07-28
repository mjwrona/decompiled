// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseManagementProjectJobService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
  public class ReleaseManagementProjectJobService : ReleaseManagement2ServiceBase
  {
    private readonly Func<IVssRequestContext, Guid, IDataAccessLayer> getDataAccessLayer;

    public ReleaseManagementProjectJobService(
      Func<IVssRequestContext, Guid, IDataAccessLayer> getDataAccessLayer)
    {
      this.getDataAccessLayer = getDataAccessLayer;
    }

    public ReleaseManagementProjectJobService()
      : this(ReleaseManagementProjectJobService.\u003C\u003EO.\u003C0\u003E__GetDataAccessLayer ?? (ReleaseManagementProjectJobService.\u003C\u003EO.\u003C0\u003E__GetDataAccessLayer = new Func<IVssRequestContext, Guid, IDataAccessLayer>(ReleaseManagementProjectJobService.GetDataAccessLayer)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public Guid QueueJobNow(
      IVssRequestContext requestContext,
      Guid projectId,
      string jobName,
      string jobExtensionName)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      ReleaseManagementJobInfo jobInfo = this.GetJobInfo(requestContext, projectId, jobName);
      if (jobInfo == null)
        return Guid.Empty;
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      try
      {
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          jobInfo.JobId
        }, false);
      }
      catch (JobDefinitionNotFoundException ex)
      {
        ReleaseManagementProjectJobService.CreateReleaseManagmentJob(requestContext, projectId, jobInfo.JobId, jobExtensionName, jobName);
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          jobInfo.JobId
        }, false);
      }
      return jobInfo.JobId;
    }

    public Guid QueueDelayedJob(
      IVssRequestContext requestContext,
      Guid projectId,
      string jobName,
      string jobExtensionName,
      int delayInSeconds)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      ReleaseManagementJobInfo jobInfo = this.GetJobInfo(requestContext, projectId, jobName);
      if (jobInfo == null)
        return Guid.Empty;
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      try
      {
        service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          jobInfo.JobId
        }, delayInSeconds);
      }
      catch (JobDefinitionNotFoundException ex)
      {
        ReleaseManagementProjectJobService.CreateReleaseManagmentJob(requestContext, projectId, jobInfo.JobId, jobExtensionName, jobName);
        service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          jobInfo.JobId
        }, delayInSeconds);
      }
      return jobInfo.JobId;
    }

    private static void CreateReleaseManagmentJob(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid jobId,
      string jobExtensionName,
      string jobName)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      ReleaseManagementProjectJobData managementProjectJobData = ReleaseManagementProjectJobData.GetReleaseManagementProjectJobData(projectId);
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition()
      {
        Data = TeamFoundationSerializationUtility.SerializeToXml((object) managementProjectJobData),
        EnabledState = TeamFoundationJobEnabledState.Enabled,
        ExtensionName = jobExtensionName,
        JobId = jobId,
        Name = jobName,
        IgnoreDormancy = service.IsIgnoreDormancyPermitted,
        PriorityClass = JobPriorityClass.Normal
      };
      service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
      {
        foundationJobDefinition
      });
    }

    private static IDataAccessLayer GetDataAccessLayer(IVssRequestContext context, Guid projectId) => (IDataAccessLayer) new DataAccessLayer(context, projectId);

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    private ReleaseManagementJobInfo GetJobInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      string jobName)
    {
      using (ReleaseManagementTimer.Create(requestContext, "Service", "SingletonJobIdService.GetSingletonJobId", 1976452))
        return this.getDataAccessLayer(requestContext, projectId).GetReleaseManagementJobInfo(jobName, true);
    }
  }
}
