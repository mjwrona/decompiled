// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.RemoteServices.FrameworkProjectUpdateJobWorker
// Assembly: Microsoft.TeamFoundation.Server.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 836F660A-A756-49A7-82F0-68378533B43C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.RemoteServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.RemoteServices
{
  public class FrameworkProjectUpdateJobWorker : BaseProjectUpdateJobWorker
  {
    public static readonly Guid ProjectUpdateJob = new Guid("69223FAA-966E-42D2-A8C3-E0FFD9D3F433");
    private long m_projectWatermark = -1;
    private const string c_projectRegistryPath = "/Configuration/ProjectService/FrameworkProjectUpdateJob/";
    private const string c_projectProcessedRevisionRegistryKey = "/Configuration/ProjectService/FrameworkProjectUpdateJob/ProcessedRevision";
    private const string c_projectKnownRevisionRegistryKey = "/Configuration/ProjectService/FrameworkProjectUpdateJob/KnownRevision";
    private const string c_area = "Project";
    private const string c_layer = "FrameworkProjectUpdateJobWorker";

    protected override int Initialize(
      IVssRequestContext requestContext,
      ProjectOperation projectOperation)
    {
      long processedRevision = FrameworkProjectUpdateJobWorker.GetProcessedRevision(requestContext);
      long minRevision = Math.Max(processedRevision + 1L, projectOperation.Revision);
      requestContext.Trace(5500330, TraceLevel.Info, "Project", nameof (FrameworkProjectUpdateJobWorker), "Querying history for minRevision={0}, processedRevision={1}, projectRevision={2}", (object) minRevision, (object) processedRevision, (object) projectOperation.Revision);
      IList<ProjectInfo> projectHistory = this.GetProjectHistory(requestContext, minRevision);
      requestContext.Trace(5500331, TraceLevel.Info, "Project", nameof (FrameworkProjectUpdateJobWorker), "Got project history items={0}, minRevision={1}, maxRevision={2}", (object) projectHistory.Count, (object) this.GetRevision(projectHistory.FirstOrDefault<ProjectInfo>()), (object) this.GetRevision(projectHistory.LastOrDefault<ProjectInfo>()));
      if (projectHistory.Count > 0)
      {
        if (projectHistory.Any<ProjectInfo>((Func<ProjectInfo, bool>) (project => project.Revision == 0L)))
          throw new InvalidOperationException("Unexpected zero revisions in the reply from IProjectService.GetProjectHistory()");
        FrameworkProjectUpdateJobWorker.SetKnownRevision(requestContext, projectHistory.Last<ProjectInfo>().Revision);
        foreach (List<ProjectInfo> collection in projectHistory.GroupBy<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (project => project.Id)).Select<IGrouping<Guid, ProjectInfo>, List<ProjectInfo>>((Func<IGrouping<Guid, ProjectInfo>, List<ProjectInfo>>) (group => group.ToList<ProjectInfo>())))
        {
          ProjectInfo last = collection.FindLast((Predicate<ProjectInfo>) (project => project.State == ProjectState.Deleted));
          if (last != null)
          {
            requestContext.Trace(5500322, TraceLevel.Info, "Project", nameof (FrameworkProjectUpdateJobWorker), "Project {0} is deleted, revision={1}", (object) last.Id, (object) last.Revision);
            this.ProjectChanges.Add(last);
          }
          else
            this.ProjectChanges.AddRange((IEnumerable<ProjectInfo>) collection);
        }
        this.ProjectChanges.Sort((Comparison<ProjectInfo>) ((p1, p2) => {checked {(int) (p1.Revision - p2.Revision);}}));
      }
      requestContext.Trace(5500333, TraceLevel.Info, "Project", nameof (FrameworkProjectUpdateJobWorker), "Updated project history items={0}, minRevision={1}, maxRevision={2}", (object) this.ProjectChanges.Count, (object) this.GetRevision(this.ProjectChanges.FirstOrDefault<ProjectInfo>()), (object) this.GetRevision(this.ProjectChanges.LastOrDefault<ProjectInfo>()));
      return projectHistory.Count;
    }

    protected override long Finalize(IVssRequestContext requestContext) => this.m_projectWatermark;

    protected override void FinalizeOnFailure(IVssRequestContext requestContext, Exception ex)
    {
      TimeSpan timeSpan = TimeSpan.FromMinutes(5.0);
      requestContext.Trace(5500334, TraceLevel.Info, "Project", nameof (FrameworkProjectUpdateJobWorker), "Requeueing job {0} with delay={1}", (object) this.JobDefinition.JobId, (object) timeSpan);
      requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        this.JobDefinition.ToJobReference()
      }, (int) timeSpan.TotalSeconds);
    }

    protected override void UpdateWatermark(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      FrameworkProjectUpdateJobWorker.SetProcessedRevision(requestContext, projectInfo.Revision);
      this.m_projectWatermark = projectInfo.Revision;
    }

    protected virtual IList<ProjectInfo> GetProjectHistory(
      IVssRequestContext requestContext,
      long minRevision)
    {
      return requestContext.GetService<FrameworkProjectService>().GetProjectHistory(requestContext, minRevision);
    }

    internal static TeamFoundationJobDefinition CreateJobDefinition(
      IVssRequestContext requestContext,
      long revision)
    {
      return new TeamFoundationJobDefinition(FrameworkProjectUpdateJobWorker.ProjectUpdateJob, "Framework ProjectUpdate Job", typeof (FrameworkProjectUpdateJobWorker).FullName, TeamFoundationSerializationUtility.SerializeToXml((object) new ProjectOperation()
      {
        Revision = revision
      }), TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.Normal)
      {
        Schedule = {
          new TeamFoundationJobSchedule()
          {
            Interval = (int) TimeSpan.FromDays(1.0).TotalSeconds,
            ScheduledTime = DateTime.UtcNow
          }
        }
      };
    }

    private long GetRevision(ProjectInfo projectInfo) => projectInfo != null ? projectInfo.Revision : -1L;

    internal static long GetProcessedRevision(IVssRequestContext requestContext) => FrameworkProjectUpdateJobWorker.GetRevision(requestContext, "/Configuration/ProjectService/FrameworkProjectUpdateJob/ProcessedRevision");

    internal static void SetProcessedRevision(IVssRequestContext requestContext, long revision) => FrameworkProjectUpdateJobWorker.SetRevision(requestContext, "/Configuration/ProjectService/FrameworkProjectUpdateJob/ProcessedRevision", revision);

    internal static long GetKnownRevision(IVssRequestContext requestContext) => FrameworkProjectUpdateJobWorker.GetRevision(requestContext, "/Configuration/ProjectService/FrameworkProjectUpdateJob/KnownRevision");

    private static void SetKnownRevision(IVssRequestContext requestContext, long revision) => FrameworkProjectUpdateJobWorker.SetRevision(requestContext, "/Configuration/ProjectService/FrameworkProjectUpdateJob/KnownRevision", revision);

    private static long GetRevision(IVssRequestContext requestContext, string path) => (long) requestContext.GetService<ISqlRegistryService>().GetValue<int>(requestContext, (RegistryQuery) path, 0);

    private static void SetRevision(IVssRequestContext requestContext, string path, long revision)
    {
      requestContext.Trace(5500335, TraceLevel.Info, "Project", nameof (FrameworkProjectUpdateJobWorker), "Updating {0} to {1}", (object) path, (object) revision);
      requestContext.GetService<ISqlRegistryService>().SetValue<long>(requestContext, path, revision);
    }
  }
}
