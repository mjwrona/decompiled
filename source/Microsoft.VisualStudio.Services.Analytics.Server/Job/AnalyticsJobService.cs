// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Job.AnalyticsJobService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Job
{
  public class AnalyticsJobService : IAnalyticsJobService, IVssFrameworkService
  {
    private static readonly RegistryQuery s_analyticsJobsQuery = new RegistryQuery("/Service/Analytics/AnalyticsJobs/*");
    private const string c_analyticsJobIdRegistryPathPattern = "/Service/Analytics/AnalyticsJobs/*";
    private const string c_trace = "AnalyticsJobService";
    private const int c_forceStateDelaySeconds = 60;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void UpdateAnalyticsJobSchedules(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      List<Guid> list = this.GetAnalyticsJobs(requestContext).ToList<Guid>();
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      List<TeamFoundationJobDefinition> source = service.QueryJobDefinitions(requestContext, (IEnumerable<Guid>) list);
      int? count1 = source?.Count;
      int count2 = list.Count;
      if (!(count1.GetValueOrDefault() == count2 & count1.HasValue))
        throw new ApplicationException(AnalyticsResources.JOB_COUNT_MISMATCH((object) source.Count, (object) list.Count));
      IEnumerable<Guid> jobIdsInQueue = service.QueryJobQueue(requestContext, (IEnumerable<Guid>) list).Where<TeamFoundationJobQueueEntry>((Func<TeamFoundationJobQueueEntry, bool>) (jobQueueEntry => jobQueueEntry != null)).Select<TeamFoundationJobQueueEntry, Guid>((Func<TeamFoundationJobQueueEntry, Guid>) (x => x.JobId));
      IEnumerable<Guid> guids = source.Where<TeamFoundationJobDefinition>((Func<TeamFoundationJobDefinition, bool>) (job => !jobIdsInQueue.Contains<Guid>(job.JobId))).Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (x => x.JobId));
      if (!guids.Any<Guid>())
        return;
      service.QueueDelayedJobs(requestContext, guids, 60);
    }

    public ICollection<Guid> GetAnalyticsJobs(IVssRequestContext requestContext)
    {
      HashSet<Guid> analyticsJobs = new HashSet<Guid>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      foreach (RegistryItem registryItem in vssRequestContext.GetService<IVssRegistryService>().Read(vssRequestContext, in AnalyticsJobService.s_analyticsJobsQuery))
      {
        Guid result;
        if (Guid.TryParse(registryItem.Value, out result))
          analyticsJobs.Add(result);
        else
          requestContext.Trace(15220001, TraceLevel.Error, "Analytics", nameof (AnalyticsJobService), "Could not parse job id from registry path: " + registryItem.Path);
      }
      return (ICollection<Guid>) analyticsJobs;
    }
  }
}
