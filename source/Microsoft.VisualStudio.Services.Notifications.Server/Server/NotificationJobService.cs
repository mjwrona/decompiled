// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationJobService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class NotificationJobService : INotificationJobService, IVssFrameworkService
  {
    private Dictionary<Guid, bool> m_jobStatuses;
    private object m_jobStatusLock;
    private object m_processQueueStatusLock;
    private HashSet<string> m_activeProcessQueues;
    private Dictionary<Tuple<string, string>, Guid> m_processQueueChannelToJob;
    private static readonly HashSet<string> s_soapSupportedProcessQueues = new HashSet<string>()
    {
      "ms.vss-code.git-event-publisher",
      "ms.vss-work.work-event-publisher",
      "ms.vss-code.code-event-publisher",
      "ms.vss-codereview.codereview-event-publisher",
      "ms.vss-build.build-event-publisher"
    };
    private const int c_maxScheduleDelayDays = 365;
    private const int c_maxScheduleDelaySeconds = 31536000;

    public NotificationJobService()
    {
      this.m_jobStatuses = new Dictionary<Guid, bool>();
      this.m_jobStatusLock = new object();
      this.m_processQueueStatusLock = new object();
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public int QueueDelayedJob(
      IVssRequestContext requestContext,
      Guid jobId,
      int delay,
      JobPriorityClass priorityClass,
      JobPriorityLevel priorityLevel)
    {
      try
      {
        TeamFoundationJobReference[] jobReferences = new TeamFoundationJobReference[1]
        {
          new TeamFoundationJobReference(jobId, priorityClass)
        };
        int num = requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences, delay, priorityLevel);
        this.TraceQueueSuccess(requestContext, jobId);
        return num;
      }
      catch (Exception ex)
      {
        this.TraceQueueFailure(requestContext, jobId, ex);
        throw;
      }
    }

    public int QueueJobAt(
      IVssRequestContext requestContext,
      Guid jobId,
      DateTime when,
      JobPriorityClass priorityClass,
      JobPriorityLevel priorityLevel)
    {
      TimeSpan timeSpan = when.Subtract(DateTime.UtcNow);
      int delay = timeSpan.TotalSeconds <= 31536000.0 ? (timeSpan.TotalSeconds >= 0.0 ? Math.Max((int) timeSpan.TotalSeconds, 5) : 5) : 31536000;
      return this.QueueDelayedJob(requestContext, jobId, delay, priorityClass, priorityLevel);
    }

    private bool CheckAndSetJobStatus(Guid jobId, bool newStatus, out bool oldStatus)
    {
      bool flag = false;
      lock (this.m_jobStatusLock)
      {
        flag = this.m_jobStatuses.TryGetValue(jobId, out oldStatus);
        if (flag)
        {
          if (oldStatus == newStatus)
            goto label_7;
        }
        this.m_jobStatuses[jobId] = newStatus;
      }
label_7:
      return flag;
    }

    private void TraceQueueSuccess(IVssRequestContext requestContext, Guid jobId)
    {
      bool oldStatus;
      if (this.CheckAndSetJobStatus(jobId, true, out oldStatus) && oldStatus)
        return;
      requestContext.Trace(1002251, TraceLevel.Info, NotificationJobService.Area, NotificationJobService.Layer, "Notification Job " + jobId.ToString() + " was queued successfully.");
    }

    private void TraceQueueFailure(IVssRequestContext requestContext, Guid jobId, Exception ex)
    {
      requestContext.TraceExceptionMsg(1002250, NotificationJobService.Area, NotificationJobService.Layer, ex, "Notification Job " + jobId.ToString() + " failed to queue");
      this.CheckAndSetJobStatus(jobId, false, out bool _);
    }

    public Dictionary<Tuple<string, string>, Guid> GetJobMappings(IVssRequestContext requestContext)
    {
      HashSet<string> hashSet1 = requestContext.GetService<INotificationEventService>().GetPublishers(requestContext).Select<NotificationEventPublisher, string>((Func<NotificationEventPublisher, string>) (p => p.Id)).ToHashSet();
      Dictionary<Tuple<string, string>, Guid> jobMappings;
      lock (this.m_jobStatusLock)
      {
        this.m_activeProcessQueues?.UnionWith((IEnumerable<string>) hashSet1);
        int count1 = hashSet1.Count;
        int? count2 = this.m_activeProcessQueues?.Count;
        int valueOrDefault = count2.GetValueOrDefault();
        if (!(count1 == valueOrDefault & count2.HasValue))
        {
          this.m_processQueueChannelToJob = (Dictionary<Tuple<string, string>, Guid>) null;
          this.m_activeProcessQueues = (HashSet<string>) null;
        }
        jobMappings = this.m_processQueueChannelToJob;
      }
      if (jobMappings == null)
      {
        bool flag = true;
        jobMappings = new Dictionary<Tuple<string, string>, Guid>();
        using (IDisposableReadOnlyList<INotificationProcessingJob> extensions = requestContext.GetExtensions<INotificationProcessingJob>())
        {
          HashSet<string> hashSet2 = hashSet1.ToHashSet();
          Guid guid = Guid.Empty;
          foreach (INotificationProcessingJob notificationProcessingJob in (IEnumerable<INotificationProcessingJob>) extensions)
          {
            if (notificationProcessingJob.ProcessQueue == "*")
              guid = notificationProcessingJob.JobId;
            else if (notificationProcessingJob.CanBeQueued(requestContext))
            {
              hashSet2.Remove(notificationProcessingJob.ProcessQueue);
              jobMappings[new Tuple<string, string>(notificationProcessingJob.ProcessQueue, string.Empty)] = notificationProcessingJob.JobId;
            }
            else
              flag = false;
          }
          if (!guid.Equals(Guid.Empty))
          {
            foreach (string str in hashSet2)
              jobMappings[new Tuple<string, string>(str, string.Empty)] = guid;
          }
        }
        using (IDisposableReadOnlyList<INotificationDeliveryJob> extensions = requestContext.GetExtensions<INotificationDeliveryJob>())
        {
          HashSet<Tuple<string, string>> queueChannelCombos = this.GetProcessQueueChannelCombos(requestContext, hashSet1);
          Dictionary<string, Guid> source = new Dictionary<string, Guid>();
          foreach (INotificationDeliveryJob notificationDeliveryJob in (IEnumerable<INotificationDeliveryJob>) extensions)
          {
            if (notificationDeliveryJob.ProcessQueue == "*")
            {
              foreach (string channel in notificationDeliveryJob.Channels)
                source[channel] = notificationDeliveryJob.JobId;
            }
            else if (notificationDeliveryJob.CanBeQueued(requestContext))
            {
              foreach (string channel in notificationDeliveryJob.Channels)
              {
                Tuple<string, string> key = new Tuple<string, string>(notificationDeliveryJob.ProcessQueue, channel);
                queueChannelCombos.Remove(key);
                jobMappings[key] = notificationDeliveryJob.JobId;
              }
            }
            else
              flag = false;
          }
          if (source.Any<KeyValuePair<string, Guid>>())
          {
            foreach (Tuple<string, string> key in queueChannelCombos)
            {
              Guid guid;
              if (source.TryGetValue(key.Item2, out guid))
                jobMappings[key] = guid;
            }
          }
        }
        lock (this.m_processQueueStatusLock)
        {
          this.m_activeProcessQueues = hashSet1;
          if (flag)
            this.m_processQueueChannelToJob = jobMappings;
        }
      }
      return jobMappings;
    }

    public Guid GetProcessingJobId(IVssRequestContext requestContext, string processQueue) => this.GetJobMappings(requestContext).GetProcessingJobId(processQueue);

    public Guid GetDeliveryJobId(
      IVssRequestContext requestContext,
      string processQueue,
      string channel)
    {
      return this.GetJobMappings(requestContext).GetDeliveryJobId(processQueue, channel);
    }

    public HashSet<string> GetSupportedProcessingJobProcessQueues(
      IVssRequestContext requestContext,
      Guid jobId)
    {
      return this.GetJobMappings(requestContext).GetSupportedProcessingJobProcessQueues(jobId);
    }

    public HashSet<Tuple<string, string>> GetSupportedDeliveryJobProcessQueuesAndChannels(
      IVssRequestContext requestContext,
      Guid jobId)
    {
      return this.GetJobMappings(requestContext).GetSupportedDeliveryJobProcessQueuesAndChannels(jobId);
    }

    private HashSet<Tuple<string, string>> GetProcessQueueChannelCombos(
      IVssRequestContext requestContext,
      HashSet<string> activeProcessQueues)
    {
      HashSet<Tuple<string, string>> queueChannelCombos = new HashSet<Tuple<string, string>>();
      foreach (string activeProcessQueue in activeProcessQueues)
      {
        foreach (string activeDeliveryChannel in NotificationFrameworkConstants.AllActiveDeliveryChannels)
        {
          if (!activeDeliveryChannel.Equals("Soap", StringComparison.OrdinalIgnoreCase) || NotificationJobService.s_soapSupportedProcessQueues.Contains(activeProcessQueue))
            queueChannelCombos.Add(new Tuple<string, string>(activeProcessQueue, activeDeliveryChannel));
        }
      }
      return queueChannelCombos;
    }

    internal static string Area
    {
      [DebuggerStepThrough] get => "Notifications";
    }

    internal static string Layer
    {
      [DebuggerStepThrough] get => nameof (NotificationJobService);
    }
  }
}
