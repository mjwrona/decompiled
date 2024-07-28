// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobDefinitionResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobDefinitionResolver : IJobDefinitionResolver
  {
    private List<TeamFoundationJobQueueEntry> m_queueEntries;
    private Dictionary<Guid, JobDefinitionResolver.JobDefinitionCache> m_queueEntryMap;
    private ILockName m_lockName;
    private const string s_area = "JobAgent";
    private const string s_layer = "JobDefinitionResolver";

    public JobDefinitionResolver(
      IVssRequestContext requestContext,
      List<TeamFoundationJobQueueEntry> queueEntries)
    {
      this.m_queueEntries = queueEntries;
      this.m_lockName = requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/GetJobDefinition", (object) this.GetType().FullName));
    }

    public TeamFoundationJobDefinition GetJobDefinition(
      IVssRequestContext requestContext,
      TeamFoundationJobQueueEntry queueEntry)
    {
      if (requestContext.ServiceHost.InstanceId != queueEntry.JobSource)
        throw new ArgumentException(FrameworkResources.IncorrectRequestContextForJobSourceError((object) queueEntry.JobSource, (object) requestContext.ServiceHost));
      this.InitializeQueueEntryMap(requestContext);
      JobDefinitionResolver.JobDefinitionCache queueEntry1;
      using (requestContext.Lock(this.m_lockName))
        queueEntry1 = this.m_queueEntryMap[queueEntry.JobSource];
      TeamFoundationJobDefinition jobDefinition = (TeamFoundationJobDefinition) null;
      bool flag = queueEntry1.ResolveTime == new DateTime() || queueEntry1.ResolveTime.AddMinutes(5.0) < DateTime.UtcNow;
      List<TeamFoundationJobDefinition> jobDefinitions = (List<TeamFoundationJobDefinition>) null;
      if (flag)
      {
        List<Guid> list = queueEntry1.JobDefinitionMap.Keys.ToList<Guid>();
        jobDefinitions = requestContext.GetService<TeamFoundationJobService>().QueryJobDefinitions(requestContext, (IEnumerable<Guid>) list);
      }
      if (queueEntry1.ResolveTime.AddMinutes(5.0) < DateTime.UtcNow)
        requestContext.Trace(2032201, TraceLevel.Warning, "JobAgent", nameof (JobDefinitionResolver), "One instance of JobDefinitionResolver used for over 5 minutes.");
      using (requestContext.Lock(this.m_lockName))
      {
        if (flag)
        {
          this.ResolveJobDefinitions(requestContext, queueEntry1.JobDefinitionMap, jobDefinitions);
          queueEntry1.ResolveTime = DateTime.UtcNow;
        }
        jobDefinition = queueEntry1.JobDefinitionMap[queueEntry.JobId];
      }
      if (jobDefinition == null)
        requestContext.Trace(2032202, TraceLevel.Error, "JobAgent", nameof (JobDefinitionResolver), "Job queue entry could not be resolved to a job definition: {0}", (object) queueEntry);
      return jobDefinition;
    }

    private void InitializeQueueEntryMap(IVssRequestContext requestContext)
    {
      if (this.m_queueEntryMap != null)
        return;
      using (requestContext.Lock(this.m_lockName))
      {
        if (this.m_queueEntryMap != null)
          return;
        Dictionary<Guid, JobDefinitionResolver.JobDefinitionCache> dictionary = new Dictionary<Guid, JobDefinitionResolver.JobDefinitionCache>();
        foreach (TeamFoundationJobQueueEntry queueEntry in this.m_queueEntries)
        {
          JobDefinitionResolver.JobDefinitionCache jobDefinitionCache;
          if (!dictionary.TryGetValue(queueEntry.JobSource, out jobDefinitionCache))
          {
            jobDefinitionCache = new JobDefinitionResolver.JobDefinitionCache();
            dictionary.Add(queueEntry.JobSource, jobDefinitionCache);
          }
          jobDefinitionCache.JobDefinitionMap.Add(queueEntry.JobId, (TeamFoundationJobDefinition) null);
        }
        this.m_queueEntryMap = dictionary;
      }
    }

    private void ResolveJobDefinitions(
      IVssRequestContext requestContext,
      Dictionary<Guid, TeamFoundationJobDefinition> jobDefinitionMap,
      List<TeamFoundationJobDefinition> jobDefinitions)
    {
      List<Guid> list = jobDefinitionMap.Keys.ToList<Guid>();
      int num = 0;
      for (int index = 0; index < list.Count; ++index)
      {
        Guid key = list[index];
        TeamFoundationJobDefinition jobDefinition = jobDefinitions[index];
        if (jobDefinition == null)
        {
          jobDefinitionMap[key] = (TeamFoundationJobDefinition) null;
          ++num;
        }
        else if (key == jobDefinition.JobId)
          jobDefinitionMap[key] = jobDefinition;
        else
          requestContext.Trace(2032203, TraceLevel.Error, "JobAgent", nameof (JobDefinitionResolver), "JobIds and JobDefinitions not in sync. Index {0}. Input JobId: {1}. Output JobDefinition.JobId: {2}", (object) index, (object) key, (object) jobDefinition.JobId);
      }
      if (num <= 0)
        return;
      requestContext.Trace(2032204, TraceLevel.Error, "JobAgent", nameof (JobDefinitionResolver), "{0}/{1} jobDefinitions not found. Jobsource Host: {2}.", (object) num, (object) jobDefinitionMap.Count, (object) requestContext.ServiceHost);
    }

    public class JobDefinitionCache
    {
      public JobDefinitionCache() => this.JobDefinitionMap = new Dictionary<Guid, TeamFoundationJobDefinition>();

      public DateTime ResolveTime { get; set; }

      public Dictionary<Guid, TeamFoundationJobDefinition> JobDefinitionMap { get; set; }
    }
  }
}
