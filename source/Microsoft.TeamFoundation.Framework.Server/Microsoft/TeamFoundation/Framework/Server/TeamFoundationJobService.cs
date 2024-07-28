// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationJobService : ITeamFoundationJobService, IVssFrameworkService
  {
    private bool m_allowDeferJobs;
    private int m_dormancyInterval;
    private int m_failureIgnoreDormancySeconds;
    private bool m_allowIgnoreDormancy;
    private bool m_logSuccessfulJobs;
    private VssPerformanceCounter[] m_jobResultCounters;
    private HashSet<string> m_excludedJobExtensions = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private int m_stopJobTimeLimit;
    private int m_minimumJobInterval;
    private int m_defaultDelayedJobDelay;
    internal static readonly string s_jobServiceArea = nameof (TeamFoundationJobService);
    internal static readonly string s_jobServiceLayer = "Service";
    private static readonly int s_defaultStopTimeLimit = 120;
    private static readonly int s_minStopTimeLimit = 10;
    private static readonly int s_maxStopTimeLimit = 240;
    private static readonly int s_defaultMinimumJobInterval = 15;
    private static readonly int s_defaultDefaultDelayedJobDelay = 120;
    internal const JobPriorityLevel DefaultPriorityLevel = JobPriorityLevel.Normal;
    private const JobPriorityLevel c_backCompatHighPriorityLevel = JobPriorityLevel.Highest;

    internal TeamFoundationJobService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      string message = "Registering for notifications for RegistrySettingsChanged on ServiceHost {0}";
      systemRequestContext.Trace(1001, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "TeamFoundationJobService.ServiceStart");
      systemRequestContext.Trace(1002, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, message, (object) systemRequestContext.ServiceHost.InstanceId);
      this.m_jobResultCounters = new VssPerformanceCounter[14];
      for (int index = 0; index < this.m_jobResultCounters.Length; ++index)
        this.m_jobResultCounters[index] = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_JobService_JobResultPerSec", index.ToString());
      systemRequestContext.GetService<CachedRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), true, FrameworkServerConstants.AllowDormantHosts, "/Configuration/JobService/*");
      systemRequestContext.Trace(1004, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Loading Settings");
      this.LoadSettings(systemRequestContext);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.Trace(1005, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "TeamFoundationJobService.ServiceEnd");
      systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));
    }

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadSettings(requestContext);
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      requestContext.Trace(1009, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "TeamFoundationJobService.LoadSettings");
      RegistryEntryCollection registryEntryCollection1 = requestContext.GetService<CachedRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (FrameworkServerConstants.JobServiceRegistryRootPath + "/**"));
      this.m_minimumJobInterval = registryEntryCollection1.GetValueFromPath<int>(FrameworkServerConstants.MinimumJobInterval, TeamFoundationJobService.s_defaultMinimumJobInterval);
      if (this.m_minimumJobInterval < 0)
      {
        requestContext.Trace(1010, TraceLevel.Warning, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "MinimumJobInterval registry value is out of range ({0}). Using zero.", (object) this.m_minimumJobInterval);
        this.m_minimumJobInterval = 0;
      }
      else
        requestContext.Trace(1011, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "MinimumJobInterval registry value is now {0}.", (object) this.m_minimumJobInterval);
      this.m_defaultDelayedJobDelay = registryEntryCollection1.GetValueFromPath<int>(FrameworkServerConstants.DefaultDelayedJobDelay, TeamFoundationJobService.s_defaultDefaultDelayedJobDelay);
      if (this.m_defaultDelayedJobDelay < 0)
      {
        requestContext.Trace(1012, TraceLevel.Warning, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "DefaultDelayedJobDelay registry value is out of range {0}. Using zero.", (object) this.m_defaultDelayedJobDelay);
        this.m_defaultDelayedJobDelay = 0;
      }
      else
        requestContext.Trace(1013, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "DefaultDelayedJobDelay registry value is now {0}.", (object) this.m_defaultDelayedJobDelay);
      int num = 0;
      while (true)
      {
        string path = FrameworkServerConstants.AlertsIgnoreJobExtension + num.ToString();
        string valueFromPath = registryEntryCollection1.GetValueFromPath<string>(path, (string) null);
        if (valueFromPath != null)
        {
          if (!this.m_excludedJobExtensions.Contains(valueFromPath))
            this.m_excludedJobExtensions.Add(valueFromPath);
          ++num;
        }
        else
          break;
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
      this.m_stopJobTimeLimit = service.GetValue<int>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.JobStopTimeLimitPath, TeamFoundationJobService.s_defaultStopTimeLimit);
      if (this.m_stopJobTimeLimit < TeamFoundationJobService.s_minStopTimeLimit)
        this.m_stopJobTimeLimit = TeamFoundationJobService.s_minStopTimeLimit;
      else if (this.m_stopJobTimeLimit > TeamFoundationJobService.s_maxStopTimeLimit)
        this.m_stopJobTimeLimit = TeamFoundationJobService.s_maxStopTimeLimit;
      requestContext.Trace(1014, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "m_stopJobTimeLimit is now {0}", (object) this.m_stopJobTimeLimit);
      RegistryEntryCollection registryEntryCollection2 = service.ReadEntries(vssRequestContext, (RegistryQuery) "/Configuration/*");
      this.m_allowDeferJobs = registryEntryCollection2.GetValueFromPath<bool>(FrameworkServerConstants.AllowDormantHosts, true);
      requestContext.Trace(1015, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "m_allowDeferJobs is now {0}", (object) this.m_allowDeferJobs);
      this.m_dormancyInterval = Math.Max(1, registryEntryCollection2.GetValueFromPath<int>(FrameworkServerConstants.HostDormancyInterval, 4)) + 1;
      requestContext.Trace(1016, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "m_dormancyInterval is now {0}", (object) this.m_dormancyInterval);
      this.m_failureIgnoreDormancySeconds = service.GetValue<int>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.FailureIgnoreDormancySeconds, int.MaxValue);
      requestContext.Trace(2032300, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "m_failureIgnoreDormancySeconds is now {0}", (object) this.m_failureIgnoreDormancySeconds);
      this.m_allowIgnoreDormancy = service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.AllowIgnoreDormancy, true);
      requestContext.Trace(2032301, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "m_allowIgnoreDormancy is now {0}", (object) this.m_allowIgnoreDormancy);
      this.m_logSuccessfulJobs = service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.LogSuccessfulJobs, true);
      requestContext.Trace(2032302, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "m_logSuccessfulJobs is now {0}", (object) this.m_logSuccessfulJobs);
    }

    public int DeleteOneTimeJobDefinitions(
      IVssRequestContext requestContext,
      DateTime? completedTo = null,
      int batchSize = 5000)
    {
      using (JobDefinitionComponent component = requestContext.CreateComponent<JobDefinitionComponent>())
        return component.DeleteOneTimeJobs(completedTo, batchSize);
    }

    public List<TeamFoundationJobDefinition> QueryJobDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds)
    {
      requestContext.TraceEnter(1017, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, nameof (QueryJobDefinitions));
      try
      {
        ITeamFoundationJobTemplateService service = requestContext.GetService<ITeamFoundationJobTemplateService>();
        if (jobIds == null)
        {
          if (requestContext.ExecutionEnvironment.IsHostedDeployment)
            requestContext.Trace(1019, TraceLevel.Error, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Unexpected request to query all jobs from: {0}", (object) EnvironmentWrapper.ToReadableStackTrace());
          List<TeamFoundationJobDefinition> foundationJobDefinitionList;
          using (JobDefinitionComponent component = requestContext.CreateComponent<JobDefinitionComponent>())
            foundationJobDefinitionList = component.QueryJobs((IEnumerable<Guid>) null);
          Dictionary<Guid, ITeamFoundationJobDefinitionTemplate> dictionary = service.GetJobTemplates(requestContext).ToDictionary<ITeamFoundationJobDefinitionTemplate, Guid>((Func<ITeamFoundationJobDefinitionTemplate, Guid>) (x => x.JobId));
          for (int index = 0; index < foundationJobDefinitionList.Count; ++index)
          {
            ITeamFoundationJobDefinitionTemplate jobTemplate;
            if (dictionary.TryGetValue(foundationJobDefinitionList[index].JobId, out jobTemplate) && !jobTemplate.AllowTemplateOverride())
              foundationJobDefinitionList[index] = service.CreateJobDefinition(requestContext, jobTemplate);
            dictionary.Remove(foundationJobDefinitionList[index].JobId);
          }
          foreach (TeamFoundationJobDefinitionTemplate jobTemplate in dictionary.Values)
            foundationJobDefinitionList.Add(service.CreateJobDefinition(requestContext, (ITeamFoundationJobDefinitionTemplate) jobTemplate));
          return foundationJobDefinitionList;
        }
        if (!requestContext.ServiceHost.IsProduction && !requestContext.IsServicingContext && !requestContext.IsVirtualServiceHost())
          this.AssertNoTemplateOverrides(requestContext, jobIds, false);
        List<TeamFoundationJobDefinition> foundationJobDefinitionList1 = new List<TeamFoundationJobDefinition>();
        List<KeyValuePair<Guid, int>> source = (List<KeyValuePair<Guid, int>>) null;
        foreach (Guid jobId in jobIds)
        {
          TeamFoundationJobDefinition foundationJobDefinition = (TeamFoundationJobDefinition) null;
          ITeamFoundationJobDefinitionTemplate jobTemplate;
          if (service.TryGetJobTemplate(requestContext, jobId, out jobTemplate))
            foundationJobDefinition = service.CreateJobDefinition(requestContext, jobTemplate);
          if (foundationJobDefinition == null || jobTemplate.AllowTemplateOverride())
          {
            if (source == null)
              source = new List<KeyValuePair<Guid, int>>();
            source.Add(new KeyValuePair<Guid, int>(jobId, foundationJobDefinitionList1.Count));
          }
          foundationJobDefinitionList1.Add(foundationJobDefinition);
        }
        // ISSUE: explicit non-virtual call
        if (source != null && __nonvirtual (source.Count) > 0)
        {
          using (JobDefinitionComponent component = requestContext.CreateComponent<JobDefinitionComponent>())
          {
            if (source.Count == 1)
            {
              JobDefinitionComponent definitionComponent = component;
              KeyValuePair<Guid, int> keyValuePair = source[0];
              Guid key = keyValuePair.Key;
              TeamFoundationJobDefinition foundationJobDefinition1 = definitionComponent.QueryJob(key);
              if (foundationJobDefinition1 != null)
              {
                List<TeamFoundationJobDefinition> foundationJobDefinitionList2 = foundationJobDefinitionList1;
                keyValuePair = source[0];
                int index = keyValuePair.Value;
                TeamFoundationJobDefinition foundationJobDefinition2 = foundationJobDefinition1;
                foundationJobDefinitionList2[index] = foundationJobDefinition2;
              }
            }
            else
            {
              List<TeamFoundationJobDefinition> foundationJobDefinitionList3 = component.QueryJobs(source.Select<KeyValuePair<Guid, int>, Guid>((Func<KeyValuePair<Guid, int>, Guid>) (x => x.Key)));
              for (int index = 0; index < foundationJobDefinitionList3.Count; ++index)
              {
                if (foundationJobDefinitionList3[index] != null)
                  foundationJobDefinitionList1[source[index].Value] = foundationJobDefinitionList3[index];
              }
            }
          }
        }
        return foundationJobDefinitionList1;
      }
      finally
      {
        requestContext.TraceLeave(1018, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, nameof (QueryJobDefinitions));
      }
    }

    public void UpdateJobDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobUpdates)
    {
      this.UpdateJobDefinitions(requestContext, jobsToDelete, jobUpdates, false, true);
    }

    public void UpdateJobDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobUpdates,
      bool allowDefinitionUpdates)
    {
      this.UpdateJobDefinitions(requestContext, jobsToDelete, jobUpdates, false, allowDefinitionUpdates);
    }

    private void UpdateJobDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobUpdates,
      bool repairQueueOnly,
      bool allowDefinitionUpdates)
    {
      this.UpdateJobDefinitions(requestContext, jobsToDelete, jobUpdates, repairQueueOnly, this.m_minimumJobInterval, this.IsIgnoreDormancyPermitted, allowDefinitionUpdates, (ITFLogger) null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal void UpdateJobDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobsToDelete,
      IEnumerable<TeamFoundationJobDefinition> jobUpdates,
      bool repairQueueOnly,
      int minimumJobInterval,
      bool allowIgnoreDormancy,
      bool allowDefinitionUpdates,
      ITFLogger logger)
    {
      if ((jobsToDelete == null || !jobsToDelete.Any<Guid>()) && (jobUpdates == null || !jobUpdates.Any<TeamFoundationJobDefinition>()))
        return;
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      requestContext.Trace(1020, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "JobService.UpdateJobDefinitions");
      logger.Info("--> JobService.UpdateJobDefinitions()");
      List<TeamFoundationJobSchedule> schedulesToUpdate = new List<TeamFoundationJobSchedule>();
      if (jobUpdates != null)
      {
        foreach (TeamFoundationJobDefinition jobUpdate in jobUpdates)
        {
          requestContext.Trace(1021, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Processing {0}", (object) jobUpdate);
          logger.Info("Processing {0}", (object) jobUpdate);
          jobUpdate.Validate(requestContext, nameof (jobUpdates), minimumJobInterval, allowIgnoreDormancy);
          if (jobUpdate.RunOnce && jobUpdate.Queried)
          {
            requestContext.Trace(1024, TraceLevel.Error, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Cannot update the definition of a queried RunOnceJob.");
            jobUpdate.RunOnce = false;
          }
          if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && jobUpdate.QueueAsDormant)
          {
            requestContext.Trace(1023, TraceLevel.Error, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Unable to queue deployment level job {0} as dormant", (object) jobUpdate.JobId);
            throw new ArgumentException(nameof (jobUpdates), string.Format("Unable to queue deployment level job {0} as dormant", (object) jobUpdate.JobId));
          }
          if (jobUpdate.Schedule != null && jobUpdate.Schedule.Count > 0)
          {
            requestContext.TraceAlways(1057, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Processing {0} schedules for job definition with id '{1}'", (object) jobUpdate.Schedule.Count, (object) jobUpdate.JobId);
            foreach (TeamFoundationJobSchedule foundationJobSchedule in jobUpdate.Schedule)
            {
              requestContext.Trace(1022, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Adding Schedule {0}", (object) foundationJobSchedule);
              logger.Info("Adding Schedule {0}", (object) foundationJobSchedule);
              foundationJobSchedule.JobSource = requestContext.ServiceHost.InstanceId;
              foundationJobSchedule.JobId = jobUpdate.JobId;
              schedulesToUpdate.Add(foundationJobSchedule);
            }
          }
        }
      }
      if (!repairQueueOnly)
      {
        requestContext.Trace(1025, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Updating jobs");
        logger.Info("Updating jobs");
        TeamFoundationJobDefinition foundationJobDefinition = jobUpdates != null ? jobUpdates.FirstOrDefault<TeamFoundationJobDefinition>((Func<TeamFoundationJobDefinition, bool>) (x => x.IsTemplateJob)) : (TeamFoundationJobDefinition) null;
        if (foundationJobDefinition != null)
          throw new InvalidOperationException(string.Format("Unable to update job definition for the template job [{0}]", (object) foundationJobDefinition));
        if (!requestContext.ServiceHost.IsProduction && !requestContext.IsServicingContext && !requestContext.IsVirtualServiceHost())
        {
          IEnumerable<Guid> jobIds = (jobUpdates ?? Enumerable.Empty<TeamFoundationJobDefinition>()).Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (x => x.JobId)).Concat<Guid>(jobsToDelete ?? Enumerable.Empty<Guid>());
          this.AssertNoTemplateOverrides(requestContext, jobIds, true);
        }
        using (JobDefinitionComponent component = requestContext.CreateComponent<JobDefinitionComponent>())
        {
          if (jobsToDelete != null && jobsToDelete.Any<Guid>())
          {
            if (jobUpdates != null && jobUpdates.Any<TeamFoundationJobDefinition>())
              jobsToDelete = jobsToDelete.Except<Guid>(jobsToDelete.Intersect<Guid>(jobUpdates.Select<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (s => s.JobId))));
            requestContext.TraceConditionally(1026, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, (Func<string>) (() => "Deleting jobs [" + string.Join<Guid>(", ", jobsToDelete) + "] from " + EnvironmentWrapper.ToReadableStackTrace()));
            component.DeleteJobs(jobsToDelete, allowDefinitionUpdates);
          }
          if (jobUpdates != null)
          {
            if (jobUpdates.Any<TeamFoundationJobDefinition>())
              component.UpdateJobs(jobUpdates, allowDefinitionUpdates);
          }
        }
      }
      requestContext.Trace(1025, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Updating Job Queue");
      logger.Info("Updating Job Queue");
      using (JobQueueComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<JobQueueComponent>())
        component.UpdateJobQueue(requestContext.ServiceHost.InstanceId, jobsToDelete, jobUpdates, (IEnumerable<TeamFoundationJobSchedule>) schedulesToUpdate);
    }

    private void AssertNoTemplateOverrides(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds,
      bool isWriteOperation)
    {
      ITeamFoundationJobTemplateService jobTemplateService = requestContext.GetService<ITeamFoundationJobTemplateService>();
      ITeamFoundationJobDefinitionTemplate jobTemplate;
      List<Guid> list = jobIds.Where<Guid>((Func<Guid, bool>) (x => jobTemplateService.TryGetJobTemplate(requestContext, x, out jobTemplate) && !jobTemplate.AllowTemplateOverride())).ToList<Guid>();
      if (list.Count <= 0)
        return;
      if (isWriteOperation)
      {
        ReportError(list[0]);
      }
      else
      {
        using (JobDefinitionComponent component = requestContext.CreateComponent<JobDefinitionComponent>())
        {
          TeamFoundationJobDefinition foundationJobDefinition = component.QueryJobs((IEnumerable<Guid>) list).FirstOrDefault<TeamFoundationJobDefinition>((Func<TeamFoundationJobDefinition, bool>) (x => x != null));
          if (foundationJobDefinition == null)
            return;
          ReportError(foundationJobDefinition.JobId);
        }
      }

      void ReportError(Guid jobId)
      {
        string message = string.Format("Unexpected job template override for job {0}", (object) jobId);
        requestContext.Trace(1039, TraceLevel.Error, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, message);
        throw new InvalidOperationException(message);
      }
    }

    internal void ClearJobQueueForOneHost(IVssRequestContext requestContext, Guid jobSource)
    {
      requestContext.CheckDeploymentRequestContext();
      using (JobQueueComponent component = requestContext.CreateComponent<JobQueueComponent>())
        component.ClearJobQueue(jobSource);
    }

    public int QueueJobs(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobReference> jobReferences,
      JobPriorityLevel priorityLevel,
      int maxDelaySeconds,
      bool queueAsDormant)
    {
      if (maxDelaySeconds == -1)
        maxDelaySeconds = this.m_defaultDelayedJobDelay;
      return this.QueueJobs(requestContext, jobReferences, priorityLevel, maxDelaySeconds, (ITFLogger) null, queueAsDormant);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal int QueueJobs(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobReference> jobReferences,
      JobPriorityLevel priorityLevel,
      int maxDelaySeconds,
      ITFLogger logger,
      bool queueAsDormant = false)
    {
      ArgumentUtility.CheckForNull<IEnumerable<TeamFoundationJobReference>>(jobReferences, nameof (jobReferences));
      if (maxDelaySeconds < 0 || maxDelaySeconds > TeamFoundationJobSchedule.MaxScheduleInterval)
      {
        requestContext.Trace(1028, TraceLevel.Error, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "maxDelaySeconds {0} is out of range", (object) maxDelaySeconds);
        throw new ArgumentOutOfRangeException(nameof (maxDelaySeconds), FrameworkResources.MaxDelaySecondsOutOfRange((object) maxDelaySeconds, (object) 0, (object) TeamFoundationJobSchedule.MaxScheduleInterval));
      }
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) & queueAsDormant)
      {
        TeamFoundationJobReference foundationJobReference = jobReferences.FirstOrDefault<TeamFoundationJobReference>();
        Guid guid = foundationJobReference != null ? foundationJobReference.JobId : Guid.Empty;
        requestContext.Trace(1023, TraceLevel.Error, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Unable to queue deployment level job {0} as dormant", (object) guid);
        throw new ArgumentException(nameof (queueAsDormant), string.Format("Unable to queue deployment level job {0} as dormant", (object) guid));
      }
      jobReferences = this.ResolveJobPriorityClasses(requestContext, jobReferences);
      List<Tuple<Guid, int>> tupleList = new List<Tuple<Guid, int>>();
      foreach (TeamFoundationJobReference jobReference in jobReferences)
      {
        int num;
        if (JobServiceUtil.IsServiceHostIdle(requestContext))
        {
          num = TeamFoundationJobService.DetermineBasePriority(jobReference.PriorityClass, priorityLevel) * -1;
          requestContext.TraceAlways(15288130, TraceLevel.Info, "JobService", nameof (QueueJobs), string.Format("Queueing for idle host {0} with priority {1} on JobId: {2}", (object) requestContext.ServiceHost.InstanceId, (object) num, (object) jobReference.JobId));
        }
        else
          num = TeamFoundationJobService.DetermineBasePriority(jobReference.PriorityClass, priorityLevel);
        Tuple<Guid, int> tuple = new Tuple<Guid, int>(jobReference.JobId, num);
        tupleList.Add(tuple);
      }
      TeamFoundationJobService.LogAndTraceQueuedJobs(requestContext, (IList<Tuple<Guid, int>>) tupleList, priorityLevel, maxDelaySeconds, logger);
      using (JobQueueComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<JobQueueComponent>())
        return component.QueueJobs(requestContext.ServiceHost.InstanceId, (IEnumerable<Tuple<Guid, int>>) tupleList, requestContext.ActivityId, requestContext.GetUserId(), priorityLevel, maxDelaySeconds, queueAsDormant);
    }

    private static void LogAndTraceQueuedJobs(
      IVssRequestContext requestContext,
      IList<Tuple<Guid, int>> jobIdAndPriorities,
      JobPriorityLevel priorityLevel,
      int maxDelaySeconds,
      ITFLogger logger)
    {
      if (logger != null || requestContext.IsTracing(1029, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer))
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Queuing {0} Jobs with priority level {1} maxDelaySeconds {2}", (object) jobIdAndPriorities.Count, (object) priorityLevel, (object) maxDelaySeconds);
        requestContext.Trace(1029, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, message);
        logger?.Info(message);
      }
      if (logger == null && !requestContext.IsTracing(1036, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer))
        return;
      foreach (Tuple<Guid, int> jobIdAndPriority in (IEnumerable<Tuple<Guid, int>>) jobIdAndPriorities)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Queuing job: {0}, priority: {1}", (object) jobIdAndPriority.Item1, (object) jobIdAndPriority.Item2);
        requestContext.Trace(1036, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, message);
        logger?.Info(message);
      }
    }

    private IEnumerable<TeamFoundationJobReference> ResolveJobPriorityClasses(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobReference> jobReferences)
    {
      List<Guid> resolvePriorityJobIds = new List<Guid>();
      foreach (TeamFoundationJobReference jobReference in jobReferences)
      {
        if (jobReference.PriorityClass == JobPriorityClass.None)
          resolvePriorityJobIds.Add(jobReference.JobId);
      }
      List<TeamFoundationJobDefinition> jobs = (List<TeamFoundationJobDefinition>) null;
      if (resolvePriorityJobIds.Count > 0)
      {
        requestContext.Trace(2032303, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Resolving the priority class for {0} Jobs", (object) resolvePriorityJobIds.Count);
        jobs = this.QueryJobDefinitions(requestContext, (IEnumerable<Guid>) resolvePriorityJobIds);
      }
      int i = 0;
      foreach (TeamFoundationJobReference jobReference in jobReferences)
      {
        if (jobReference.PriorityClass == JobPriorityClass.None)
        {
          if (jobs[i] == null)
            throw new JobDefinitionNotFoundException(resolvePriorityJobIds[i]);
          yield return jobs[i++].ToJobReference();
        }
        else
          yield return jobReference;
      }
    }

    internal List<TeamFoundationJobQueueEntry> AcquireJobs(
      IVssRequestContext requestContext,
      Guid agentId,
      int maxJobsToAcquire,
      out int nextScheduledJob)
    {
      requestContext.TraceEnter(1037, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, nameof (AcquireJobs));
      try
      {
        requestContext.Trace(1033, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "JobService.AcquireJobs agentId {0} maxJobsToAcquire {1}", (object) agentId, (object) maxJobsToAcquire);
        List<TeamFoundationJobQueueEntry> foundationJobQueueEntryList = (List<TeamFoundationJobQueueEntry>) null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        using (JobQueueComponent component = requestContext.CreateComponent<JobQueueComponent>())
        {
          ResultCollection resultCollection = component.AcquireJobs(agentId, maxJobsToAcquire, this.m_allowDeferJobs, this.m_dormancyInterval);
          foundationJobQueueEntryList = resultCollection.GetCurrent<TeamFoundationJobQueueEntry>().Items;
          resultCollection.TryNextResult();
          ObjectBinder<int> current = resultCollection.GetCurrent<int>();
          nextScheduledJob = !current.MoveNext() ? -1 : current.Current;
        }
        stopwatch.Stop();
        requestContext.Trace(1033, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Acquired {0} jobs. Sproc elapsed time: {1} ms.", (object) foundationJobQueueEntryList.Count, (object) (int) stopwatch.Elapsed.TotalMilliseconds);
        return foundationJobQueueEntryList;
      }
      finally
      {
        requestContext.TraceLeave(1038, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, nameof (AcquireJobs));
      }
    }

    internal List<TeamFoundationJobQueueEntry> QueryAcquiredJobs(
      IVssRequestContext requestContext,
      Guid agentId)
    {
      requestContext.TraceEnter(1051, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, nameof (QueryAcquiredJobs));
      try
      {
        requestContext.Trace(1034, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "JobService.QueryAcquiredJobs agentId {0}", (object) agentId);
        List<TeamFoundationJobQueueEntry> foundationJobQueueEntryList;
        using (JobQueueComponent component = requestContext.CreateComponent<JobQueueComponent>())
          foundationJobQueueEntryList = !(component is JobQueueComponent8 jobQueueComponent8) ? new List<TeamFoundationJobQueueEntry>() : jobQueueComponent8.QueryAcquiredJobs(agentId).GetCurrent<TeamFoundationJobQueueEntry>().Items;
        requestContext.Trace(1035, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Number of acquired jobs returned: {0}.", (object) foundationJobQueueEntryList.Count);
        return foundationJobQueueEntryList;
      }
      finally
      {
        requestContext.TraceLeave(1052, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, nameof (QueryAcquiredJobs));
      }
    }

    internal void ChangeJobState(
      IVssRequestContext deploymentRequestContext,
      Guid agentId,
      Guid jobSource,
      Guid jobId,
      TeamFoundationJobState newState)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      using (JobQueueComponent component = deploymentRequestContext.CreateComponent<JobQueueComponent>())
        component.ChangeJobState(agentId, jobSource, jobId, newState);
    }

    public bool StopJob(IVssRequestContext requestContext, Guid jobId) => this.StopJob(requestContext, jobId, 0);

    public bool StopJob(IVssRequestContext requestContext, Guid jobId, int commandTimeout)
    {
      using (JobQueueComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<JobQueueComponent>())
        return component.StopJob(requestContext.ServiceHost.InstanceId, jobId, commandTimeout);
    }

    internal void ReleaseJobs(
      IVssRequestContext requestContext,
      Guid agentId,
      int notificationRequiredSeconds,
      List<JobRunner> jobRunnersToRelease)
    {
      requestContext.TraceEnter(1053, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, nameof (ReleaseJobs));
      try
      {
        ArgumentUtility.CheckForNull<List<JobRunner>>(jobRunnersToRelease, nameof (jobRunnersToRelease));
        requestContext.Trace(1033, TraceLevel.Verbose, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "JobService.ReleaseJobs agentId {0} jobRunnersToRelease: {1}", (object) agentId, (object) jobRunnersToRelease.Count);
        TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
        List<ReleaseJobInfo> jobsToRelease = new List<ReleaseJobInfo>(jobRunnersToRelease.Count);
        List<TeamFoundationJobSchedule> jobsToReleaseSchedules = new List<TeamFoundationJobSchedule>(jobRunnersToRelease.Count);
        foreach (JobRunner jobRunner in jobRunnersToRelease)
        {
          if (jobRunner.ReleaseJobInfo == null)
          {
            requestContext.Trace(2032304, TraceLevel.Error, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Can't release job - ReleaseJobInfo is null: {0}", (object) jobRunner);
          }
          else
          {
            jobsToRelease.Add(jobRunner.ReleaseJobInfo);
            this.DiagnoseJobResult(requestContext, jobRunner);
          }
          if (jobRunner.JobDefinition != null && jobRunner.JobDefinition.Schedule != null)
          {
            foreach (TeamFoundationJobSchedule foundationJobSchedule in jobRunner.JobDefinition.Schedule)
            {
              foundationJobSchedule.JobSource = jobRunner.QueueEntry.JobSource;
              foundationJobSchedule.JobId = jobRunner.QueueEntry.JobId;
              jobsToReleaseSchedules.Add(foundationJobSchedule);
            }
          }
        }
        Stopwatch stopwatch = Stopwatch.StartNew();
        using (JobQueueComponent component = requestContext.CreateComponent<JobQueueComponent>())
          component.ReleaseJobs(agentId, service.HostDormancySeconds, this.m_failureIgnoreDormancySeconds, notificationRequiredSeconds, jobsToRelease, jobsToReleaseSchedules, this.m_logSuccessfulJobs);
        stopwatch.Stop();
        requestContext.Trace(1033, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Released {0} jobs. Sproc elapsed time: {1} ms.", (object) jobsToRelease.Count, (object) (int) stopwatch.Elapsed.TotalMilliseconds);
      }
      finally
      {
        requestContext.TraceLeave(1054, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, nameof (ReleaseJobs));
      }
    }

    internal virtual void UpdateLastExecutionTime(IVssRequestContext requestContext, Guid jobId)
    {
      requestContext.TraceEnter(1055, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, nameof (UpdateLastExecutionTime));
      try
      {
        using (JobDefinitionComponent component = requestContext.CreateComponent<JobDefinitionComponent>())
        {
          requestContext.Trace(1051, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Updating LastExecutionTime for JobId {0} in host: {1}", (object) jobId, (object) requestContext.ServiceHost);
          component.UpdateLastExecutionTime(jobId);
        }
      }
      finally
      {
        requestContext.TraceLeave(1056, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, nameof (UpdateLastExecutionTime));
      }
    }

    public List<TeamFoundationJobHistoryEntry> QueryJobHistory(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds)
    {
      using (JobQueueComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<JobQueueComponent>())
        return jobIds == null || jobIds.Any<Guid>() ? component.QueryJobHistory(requestContext.ServiceHost.InstanceId, jobIds) : new List<TeamFoundationJobHistoryEntry>();
    }

    public List<TeamFoundationJobHistoryEntry> QueryLatestJobHistory(
      IVssRequestContext requestContext,
      IEnumerable<Guid> jobIds)
    {
      return this.QueryLatestJobHistory(requestContext.To(TeamFoundationHostType.Deployment), requestContext.ServiceHost.InstanceId, jobIds);
    }

    internal List<TeamFoundationJobHistoryEntry> QueryLatestJobHistory(
      IVssRequestContext deploymentRequestContext,
      Guid jobSource,
      IEnumerable<Guid> jobIds)
    {
      using (JobQueueComponent component = deploymentRequestContext.CreateComponent<JobQueueComponent>())
        return jobIds == null || jobIds.Any<Guid>() ? component.QueryLatestJobHistory(jobSource, jobIds) : new List<TeamFoundationJobHistoryEntry>();
    }

    internal void CleanupJobHistory(IVssRequestContext requestContext, int jobAge)
    {
      using (JobQueueComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<JobQueueComponent>())
        component.CleanupJobHistory(jobAge);
    }

    internal void CleanupJobHistory(IVssRequestContext requestContext, Guid jobSource)
    {
      using (JobQueueComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<JobQueueComponent>())
        component.CleanupJobHistory(jobSource);
    }

    public void RepairQueue(IVssRequestContext requestContext, ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckSystemRequestContext();
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      List<TeamFoundationJobDefinition> foundationJobDefinitionList = this.QueryJobDefinitions(requestContext, (IEnumerable<Guid>) null);
      logger.Info(string.Format("Found {0} job definition(s).", (object) foundationJobDefinitionList.Count));
      List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>();
      List<TeamFoundationJobReference> jobReferences = new List<TeamFoundationJobReference>();
      foreach (TeamFoundationJobDefinition foundationJobDefinition in foundationJobDefinitionList)
      {
        if (foundationJobDefinition.RunOnce)
        {
          if (!foundationJobDefinition.LastExecutionTime.HasValue && !string.Equals(foundationJobDefinition.ExtensionName, "N/A", StringComparison.OrdinalIgnoreCase))
          {
            logger.Info(string.Format("Adding Job {0} to list of outstanding jobs", (object) foundationJobDefinition));
            jobReferences.Add(foundationJobDefinition.ToJobReference());
          }
        }
        else
          jobUpdates.Add(foundationJobDefinition);
      }
      logger.Info(string.Format("Updating {0} job definitions", (object) jobUpdates.Count));
      this.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates, true, this.m_minimumJobInterval, this.IsIgnoreDormancyPermitted, true, logger);
      if (jobReferences.Count <= 0)
        return;
      logger.Info(string.Format("Queuing {0} job(s) now", (object) jobReferences.Count));
      this.QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences);
    }

    public List<TeamFoundationJobQueueEntry> QueryJobQueue(
      IVssRequestContext jobSourceRequestContext,
      IEnumerable<Guid> jobIds)
    {
      return this.QueryJobQueue(jobSourceRequestContext, jobIds, false);
    }

    internal List<TeamFoundationJobQueueEntry> QueryJobQueue(
      IVssRequestContext jobSourceRequestContext,
      IEnumerable<Guid> jobIds,
      bool useJobSourceRequestContextForQueueAccess,
      bool getAllJobs = false)
    {
      IVssRequestContext context = jobSourceRequestContext;
      string databaseCategory = "Default";
      if (!useJobSourceRequestContextForQueueAccess)
        context = jobSourceRequestContext.To(TeamFoundationHostType.Deployment);
      if (getAllJobs && jobIds != null && jobIds.Any<Guid>())
        throw new ArgumentException("The value of getAllJobs argument cannot be true when jobIds is not empty");
      if (!getAllJobs && jobIds != null && !jobIds.Any<Guid>())
        return new List<TeamFoundationJobQueueEntry>();
      using (JobQueueComponent component = context.CreateComponent<JobQueueComponent>(databaseCategory))
      {
        Guid jobSource = getAllJobs ? Guid.Empty : jobSourceRequestContext.ServiceHost.InstanceId;
        return component.QueryJobQueueForOneHost(jobSource, jobIds);
      }
    }

    [Obsolete]
    internal void QueryJobQueue(
      IVssRequestContext requestContext,
      out List<TeamFoundationJobQueueEntry> runningJobs,
      out List<TeamFoundationJobQueueEntry> queuedJobs,
      out List<TeamFoundationJobQueueEntry> scheduledJobs)
    {
      using (JobQueueComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<JobQueueComponent>())
      {
        ResultCollection resultCollection = component.QueryJobQueue();
        runningJobs = resultCollection.GetCurrent<TeamFoundationJobQueueEntry>().Items;
        resultCollection.TryNextResult();
        queuedJobs = resultCollection.GetCurrent<TeamFoundationJobQueueEntry>().Items;
        resultCollection.TryNextResult();
        scheduledJobs = resultCollection.GetCurrent<TeamFoundationJobQueueEntry>().Items;
      }
    }

    public List<TeamFoundationJobQueueEntry> QueryRunningJobs(
      IVssRequestContext requestContext,
      bool includePendingJobs = false)
    {
      using (JobQueueComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<JobQueueComponent>())
        return component.QueryRunningJobs(includePendingJobs);
    }

    internal void ReenableJobs(IVssRequestContext requestContext, ICollection<Guid> jobSources)
    {
      requestContext.CheckDeploymentRequestContext();
      if (jobSources.Count <= 0)
        return;
      requestContext.Trace(1052, TraceLevel.Info, TeamFoundationJobService.s_jobServiceArea, TeamFoundationJobService.s_jobServiceLayer, "Re-enabling dormant jobs associated with {0} hosts.", (object) jobSources.Count);
      using (JobQueueComponent component = requestContext.CreateComponent<JobQueueComponent>())
        component.ReenableJobs((IEnumerable<Guid>) jobSources);
    }

    internal void RescheduleInactiveJobs(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      List<TeamFoundationServiceHostProcess> source = requestContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProcesses(requestContext.Elevate(), Guid.Empty);
      List<TeamFoundationJobQueueEntry> foundationJobQueueEntryList;
      using (JobQueueComponent component = requestContext.CreateComponent<JobQueueComponent>())
        foundationJobQueueEntryList = component.RescheduleJobs(source.Select<TeamFoundationServiceHostProcess, Guid>((Func<TeamFoundationServiceHostProcess, Guid>) (process => process.ProcessId)));
      foreach (TeamFoundationJobQueueEntry queueEntry in foundationJobQueueEntryList)
        JobRunner.TraceJobAgentHistory(queueEntry, TeamFoundationJobResult.Inactive);
      this.m_jobResultCounters[7].IncrementBy((long) foundationJobQueueEntryList.Count);
    }

    public int StopJobTimeLimit => this.m_stopJobTimeLimit;

    public bool IsIgnoreDormancyPermitted => this.m_allowIgnoreDormancy;

    internal static JobPriorityLevel ConvertPriorityBitToLevel(bool highPriority) => !highPriority ? JobPriorityLevel.Normal : JobPriorityLevel.Highest;

    internal static int DetermineBasePriority(
      JobPriorityClass priorityClass,
      JobPriorityLevel priorityLevel)
    {
      if (priorityClass == JobPriorityClass.None)
        throw new EnumerationNoneArgumentException(typeof (JobPriorityClass));
      if (priorityLevel == JobPriorityLevel.None)
        throw new EnumerationNoneArgumentException(typeof (JobPriorityLevel));
      switch (priorityClass)
      {
        case JobPriorityClass.Idle:
          switch (priorityLevel)
          {
            case JobPriorityLevel.Idle:
              return 1;
            case JobPriorityLevel.Lowest:
              return 2;
            case JobPriorityLevel.BelowNormal:
              return 3;
            case JobPriorityLevel.Normal:
              return 4;
            case JobPriorityLevel.Highest:
              return 6;
            default:
              throw new ArgumentOutOfRangeException(nameof (priorityLevel));
          }
        case JobPriorityClass.Normal:
          switch (priorityLevel)
          {
            case JobPriorityLevel.Idle:
              return 1;
            case JobPriorityLevel.Lowest:
              return 6;
            case JobPriorityLevel.BelowNormal:
              return 7;
            case JobPriorityLevel.Normal:
              return 8;
            case JobPriorityLevel.Highest:
              return 10;
            default:
              throw new ArgumentOutOfRangeException(nameof (priorityLevel));
          }
        case JobPriorityClass.AboveNormal:
          switch (priorityLevel)
          {
            case JobPriorityLevel.Idle:
              return 1;
            case JobPriorityLevel.Lowest:
              return 8;
            case JobPriorityLevel.BelowNormal:
              return 9;
            case JobPriorityLevel.Normal:
              return 10;
            case JobPriorityLevel.Highest:
              return 12;
            default:
              throw new ArgumentOutOfRangeException(nameof (priorityLevel));
          }
        case JobPriorityClass.High:
          switch (priorityLevel)
          {
            case JobPriorityLevel.Idle:
              return 1;
            case JobPriorityLevel.Lowest:
              return 11;
            case JobPriorityLevel.BelowNormal:
              return 12;
            case JobPriorityLevel.Normal:
              return 13;
            case JobPriorityLevel.Highest:
              return 15;
            default:
              throw new ArgumentOutOfRangeException(nameof (priorityLevel));
          }
        default:
          throw new ArgumentOutOfRangeException(nameof (priorityClass));
      }
    }

    private void DiagnoseJobResult(IVssRequestContext requestContext, JobRunner jobRunner)
    {
      if (jobRunner != null)
      {
        TeamFoundationJobResult? jobResult = jobRunner.ReleaseJobInfo?.JobResult;
        TeamFoundationJobResult foundationJobResult = TeamFoundationJobResult.Succeeded;
        if (jobResult.GetValueOrDefault() >= foundationJobResult & jobResult.HasValue)
          this.m_jobResultCounters[(int) jobRunner.ReleaseJobInfo.JobResult].Increment();
      }
      if (jobRunner.JobDefinition == null || !requestContext.ExecutionEnvironment.IsHostedDeployment || !TeamFoundationJobService.IsAlertableJobResult(jobRunner.ReleaseJobInfo.JobResult) || this.m_excludedJobExtensions.Contains(jobRunner.JobDefinition.ExtensionName))
        return;
      EventLogEntryType level = EventLogEntryType.Warning;
      if (jobRunner.ReleaseJobInfo.PriorityClass >= JobPriorityClass.High)
        level = EventLogEntryType.Error;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}", (object) jobRunner.JobDefinition.ExtensionName, (object) jobRunner.ReleaseJobInfo.PriorityClass);
      TeamFoundationEventLog.Default.Log(requestContext, FrameworkResources.JobFailedEventError((object) jobRunner.ReleaseJobInfo.JobId, (object) jobRunner.ReleaseJobInfo.PriorityClass, (object) jobRunner.ReleaseJobInfo.JobSource, (object) jobRunner.JobDefinition.ExtensionName, (object) jobRunner.JobDefinition.Name, (object) jobRunner.ReleaseJobInfo.JobResult, (object) jobRunner.ReleaseJobInfo.ResultMessage), TeamFoundationEventId.JobFailedError, level, (object) str);
    }

    private static bool IsAlertableJobResult(TeamFoundationJobResult jobResult) => jobResult == TeamFoundationJobResult.Failed || jobResult == TeamFoundationJobResult.Killed || jobResult == TeamFoundationJobResult.ExtensionNotFound || jobResult == TeamFoundationJobResult.JobDefinitionNotFound || jobResult == TeamFoundationJobResult.JobInitializationError;
  }
}
