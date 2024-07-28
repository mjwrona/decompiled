// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationBacklogUtil
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

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class NotificationBacklogUtil
  {
    public static NotificationBacklogUtil.DoGetDateTimeNowUtc GetDateTimeNowUtc = new NotificationBacklogUtil.DoGetDateTimeNowUtc(NotificationBacklogUtil.GetDateTimeNowUtcImpl);
    private static readonly string s_Area = "Notifications";
    private static readonly string s_Layer = "BacklogStatus";
    private const int c_defaultMaxStuckTimeMs = 600000;
    private const int c_defaultMaxWaitForScheduleTimeMs = 600000;
    private static char[] s_slash = new char[1]{ '/' };

    private static DateTime GetDateTimeNowUtcImpl() => DateTime.UtcNow;

    private static Dictionary<T, NotificationBatchStatus> GetBatchStatuses<T>(
      IVssRequestContext requestContext,
      string registryRoot,
      int segmentMinimum,
      NotificationBacklogUtil.CreateKey<T> createKey)
    {
      Dictionary<T, NotificationBatchStatus> dictionary = new Dictionary<T, NotificationBatchStatus>();
      foreach (RegistryEntry readEntry in requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) (registryRoot + "/**")))
      {
        string[] pathParts = readEntry.Path.Split(NotificationBacklogUtil.s_slash, StringSplitOptions.RemoveEmptyEntries);
        if (pathParts.Length < segmentMinimum)
        {
          requestContext.Trace(1002916, TraceLevel.Error, NotificationBacklogUtil.s_Area, NotificationBacklogUtil.s_Layer, string.Format("BatchStatus entry '{0}' only has {1} segments", (object) readEntry.Path, (object) pathParts.Length));
        }
        else
        {
          T key = default (T);
          if (!createKey(pathParts, out key))
          {
            requestContext.Trace(1002916, TraceLevel.Error, NotificationBacklogUtil.s_Area, NotificationBacklogUtil.s_Layer, "key segment invalid '" + readEntry.Path + "'");
          }
          else
          {
            NotificationBatchStatus orAddValue = dictionary.GetOrAddValue<T, NotificationBatchStatus>(key);
            string s = readEntry.GetValue(string.Empty);
            string str = pathParts[segmentMinimum - 1];
            bool flag1 = str.Equals(NotificationFrameworkConstants.BatchStarted, StringComparison.OrdinalIgnoreCase);
            bool flag2 = !flag1 && str.Equals(NotificationFrameworkConstants.BatchCompleted, StringComparison.OrdinalIgnoreCase);
            if (flag1 | flag2)
            {
              DateTime result;
              if (!DateTime.TryParse(s, out result))
                requestContext.Trace(1002916, TraceLevel.Error, NotificationBacklogUtil.s_Area, NotificationBacklogUtil.s_Layer, "DateTime value invalid '" + s + "'");
              else if (flag1)
                orAddValue.BatchStarted = result;
              else if (flag2)
                orAddValue.BatchCompleted = result;
            }
          }
        }
      }
      return dictionary;
    }

    public static Dictionary<Guid, NotificationBatchStatus> GetJobBatchStatuses(
      IVssRequestContext requestContext)
    {
      return NotificationBacklogUtil.GetBatchStatuses<Guid>(requestContext, NotificationFrameworkConstants.JobStatusRoot, 6, (NotificationBacklogUtil.CreateKey<Guid>) ((string[] pathParts, out Guid key) => Guid.TryParse(pathParts[4], out key)));
    }

    public static Dictionary<string, NotificationBatchStatus> GetEventPublisherBatchStatuses(
      IVssRequestContext requestContext)
    {
      return NotificationBacklogUtil.GetBatchStatuses<string>(requestContext, NotificationFrameworkConstants.EventPublisherStatusRoot, 6, (NotificationBacklogUtil.CreateKey<string>) ((string[] pathParts, out string key) =>
      {
        key = pathParts[4];
        return true;
      }));
    }

    public static Dictionary<Tuple<string, string>, NotificationBatchStatus> GetNotificationPublisherBatchStatuses(
      IVssRequestContext requestContext)
    {
      return NotificationBacklogUtil.GetBatchStatuses<Tuple<string, string>>(requestContext, NotificationFrameworkConstants.NotificationPublisherStatusRoot, 7, (NotificationBacklogUtil.CreateKey<Tuple<string, string>>) ((string[] pathParts, out Tuple<string, string> key) =>
      {
        key = new Tuple<string, string>(pathParts[4], pathParts[5]);
        return true;
      }));
    }

    public static void MergeBacklogAndBatchStatuses(
      IVssRequestContext requestContext,
      NotificationEventBacklogStatus notificationEventBacklogStatus)
    {
      Dictionary<Tuple<string, string>, Guid> jobMappings = requestContext.GetService<INotificationJobService>().GetJobMappings(requestContext);
      Dictionary<Guid, NotificationBatchStatus> jobBatchStatuses = NotificationBacklogUtil.GetJobBatchStatuses(requestContext);
      NotificationBacklogUtil.MergeBacklogAndEventPublisherBatchStatus(requestContext, notificationEventBacklogStatus, jobMappings, jobBatchStatuses);
      NotificationBacklogUtil.MergeBacklogAndNotificationPublisherBatchStatus(requestContext, notificationEventBacklogStatus, jobMappings, jobBatchStatuses);
    }

    private static void MergeBacklogAndEventPublisherBatchStatus(
      IVssRequestContext requestContext,
      NotificationEventBacklogStatus notificationEventBacklogStatus,
      Dictionary<Tuple<string, string>, Guid> jobMappings,
      Dictionary<Guid, NotificationBatchStatus> jobBatchStatuses)
    {
      Dictionary<string, NotificationBatchStatus> publisherBatchStatuses = NotificationBacklogUtil.GetEventPublisherBatchStatuses(requestContext);
      foreach (EventBacklogStatus eventBacklogStatu in notificationEventBacklogStatus.EventBacklogStatus)
      {
        eventBacklogStatu.JobId = jobMappings.GetProcessingJobId(eventBacklogStatu.Publisher);
        if (eventBacklogStatu.JobId.Equals(Guid.Empty))
          requestContext.Trace(1002917, TraceLevel.Error, NotificationBacklogUtil.s_Area, NotificationBacklogUtil.s_Layer, "Missing JobId for Publisher '" + eventBacklogStatu.Publisher + "'");
        NotificationBatchStatus notificationBatchStatus1;
        if (jobBatchStatuses.TryGetValue(eventBacklogStatu.JobId, out notificationBatchStatus1))
        {
          eventBacklogStatu.LastJobBatchStartTime = notificationBatchStatus1.BatchStarted;
          eventBacklogStatu.LastJobProcessedTime = notificationBatchStatus1.BatchCompleted;
        }
        NotificationBatchStatus notificationBatchStatus2;
        if (publisherBatchStatuses.TryGetValue(eventBacklogStatu.Publisher, out notificationBatchStatus2))
        {
          eventBacklogStatu.LastEventBatchStartTime = notificationBatchStatus2.BatchStarted;
          eventBacklogStatu.LastEventProcessedTime = notificationBatchStatus2.BatchCompleted;
        }
      }
    }

    private static void MergeBacklogAndNotificationPublisherBatchStatus(
      IVssRequestContext requestContext,
      NotificationEventBacklogStatus notificationEventBacklogStatus,
      Dictionary<Tuple<string, string>, Guid> jobMappings,
      Dictionary<Guid, NotificationBatchStatus> jobBatchStatuses)
    {
      Dictionary<Tuple<string, string>, NotificationBatchStatus> publisherBatchStatuses = NotificationBacklogUtil.GetNotificationPublisherBatchStatuses(requestContext);
      List<NotificationBacklogStatus> notificationBacklogStatusList = new List<NotificationBacklogStatus>();
      Dictionary<Tuple<string, string>, NotificationBacklogStatus> dictionary = new Dictionary<Tuple<string, string>, NotificationBacklogStatus>();
      foreach (NotificationBacklogStatus notificationBacklogStatu in notificationEventBacklogStatus.NotificationBacklogStatus)
      {
        notificationBacklogStatu.JobId = jobMappings.GetDeliveryJobId(notificationBacklogStatu.Publisher, notificationBacklogStatu.Channel);
        if (notificationBacklogStatu.JobId.Equals(Guid.Empty))
          requestContext.TraceAlways(1002917, TraceLevel.Warning, NotificationBacklogUtil.s_Area, NotificationBacklogUtil.s_Layer, "Missing JobId for Publisher/Channel '" + notificationBacklogStatu.Publisher + "/" + notificationBacklogStatu.Channel + "'");
        NotificationBatchStatus notificationBatchStatus1;
        if (jobBatchStatuses.TryGetValue(notificationBacklogStatu.JobId, out notificationBatchStatus1))
        {
          notificationBacklogStatu.LastJobBatchStartTime = notificationBatchStatus1.BatchStarted;
          notificationBacklogStatu.LastJobProcessedTime = notificationBatchStatus1.BatchCompleted;
        }
        NotificationBatchStatus notificationBatchStatus2;
        if (publisherBatchStatuses.TryGetValue(new Tuple<string, string>(notificationBacklogStatu.Publisher, notificationBacklogStatu.Channel), out notificationBatchStatus2))
        {
          notificationBacklogStatu.LastNotificationBatchStartTime = notificationBatchStatus2.BatchStarted;
          notificationBacklogStatu.LastNotificationProcessedTime = notificationBatchStatus2.BatchCompleted;
        }
        if (string.IsNullOrEmpty(notificationBacklogStatu.Status))
          notificationBacklogStatu.Status = "Unprocessed";
        if (NotificationFrameworkConstants.UserDeliveryChannels.Contains(notificationBacklogStatu.Channel))
        {
          Tuple<string, string> key = new Tuple<string, string>(notificationBacklogStatu.Publisher, notificationBacklogStatu.Status);
          NotificationBacklogStatus notificationBacklogStatus;
          if (!dictionary.TryGetValue(key, out notificationBacklogStatus))
          {
            notificationBacklogStatus = new NotificationBacklogStatus()
            {
              JobId = notificationBacklogStatu.JobId,
              Publisher = notificationBacklogStatu.Publisher,
              Channel = "User",
              Status = notificationBacklogStatu.Status,
              LastNotificationBatchStartTime = notificationBacklogStatu.LastNotificationBatchStartTime,
              LastNotificationProcessedTime = notificationBacklogStatu.LastNotificationProcessedTime,
              LastJobBatchStartTime = notificationBacklogStatu.LastJobBatchStartTime,
              LastJobProcessedTime = notificationBacklogStatu.LastJobProcessedTime
            };
            dictionary[key] = notificationBacklogStatus;
          }
          else if (notificationBacklogStatu.LastNotificationBatchStartTime > notificationBacklogStatus.LastNotificationBatchStartTime)
          {
            notificationBacklogStatus.LastNotificationBatchStartTime = notificationBacklogStatu.LastNotificationBatchStartTime;
            notificationBacklogStatus.LastNotificationProcessedTime = notificationBacklogStatu.LastNotificationProcessedTime;
          }
          notificationBacklogStatus.UnprocessedNotifications += notificationBacklogStatu.UnprocessedNotifications;
        }
        else
          notificationBacklogStatusList.Add(notificationBacklogStatu);
      }
      notificationBacklogStatusList.AddRange((IEnumerable<NotificationBacklogStatus>) dictionary.Values);
      notificationEventBacklogStatus.NotificationBacklogStatus = notificationBacklogStatusList;
    }

    public static bool PublishNotificationBacklogStatus(
      IVssRequestContext requestContext,
      NotificationEventBacklogStatus notificationEventBacklogStatus)
    {
      IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
      int maxStuckTimeMs = service1.GetValue<int>(requestContext, (RegistryQuery) (NotificationFrameworkConstants.NotificationRootPath + "/" + NotificationFrameworkConstants.MaxJobStuckTimeMs), 600000);
      int maxWaitForScheduleTimeMs = service1.GetValue<int>(requestContext, (RegistryQuery) (NotificationFrameworkConstants.NotificationRootPath + "/" + NotificationFrameworkConstants.MaxJobWaitForScheduleTimeMs), 600000);
      bool flag = false;
      Dictionary<Guid, HashSet<string>> dictionary1 = new Dictionary<Guid, HashSet<string>>();
      Dictionary<Guid, List<string>> dictionary2 = new Dictionary<Guid, List<string>>();
      foreach (EventBacklogStatus eventBacklogStatu in notificationEventBacklogStatus.EventBacklogStatus)
      {
        flag = flag || eventBacklogStatu.UnprocessedEvents > 0;
        CustomerIntelligenceData ciData = new CustomerIntelligenceData();
        ciData.Add("JobId", (object) eventBacklogStatu.JobId);
        ciData.Add("Publisher", eventBacklogStatu.Publisher);
        ciData.Add("UnprocessedEvents", (double) eventBacklogStatu.UnprocessedEvents);
        ciData.Add("OldestPendingEventTime", (object) eventBacklogStatu.OldestPendingEventTime);
        ciData.Add("LastEventBatchStartTime", (object) eventBacklogStatu.LastEventBatchStartTime);
        ciData.Add("LastEventProcessedTime", (object) eventBacklogStatu.LastEventProcessedTime);
        ciData.Add("LastJobBatchStartTime", (object) eventBacklogStatu.LastJobBatchStartTime);
        ciData.Add("LastJobProcessedTime", (object) eventBacklogStatu.LastJobProcessedTime);
        ciData.Add("CaptureTime", (object) eventBacklogStatu.CaptureTime);
        NotificationCustomerIntelligence.PublishEvent(requestContext, NotificationCustomerIntelligence.BacklogStatusFeature, NotificationCustomerIntelligence.EventStatusAction, ciData);
        if (NotificationBacklogUtil.IsStuckJob(requestContext, maxStuckTimeMs, eventBacklogStatu.JobId, eventBacklogStatu.UnprocessedEvents, eventBacklogStatu.LastJobBatchStartTime, eventBacklogStatu.LastJobProcessedTime))
          dictionary1.GetOrAddValue<Guid, HashSet<string>>(eventBacklogStatu.JobId).Add(eventBacklogStatu.Publisher + " ");
        else if (NotificationBacklogUtil.ShouldRescheduleJobIfNeeded(requestContext, maxWaitForScheduleTimeMs, eventBacklogStatu.JobId, eventBacklogStatu.UnprocessedEvents, eventBacklogStatu.LastJobBatchStartTime, eventBacklogStatu.LastJobProcessedTime))
          dictionary2.GetOrAddValue<Guid, List<string>>(eventBacklogStatu.JobId).Add(string.Format("{0}:{1} ", (object) eventBacklogStatu.Publisher, (object) eventBacklogStatu.UnprocessedEvents));
      }
      foreach (NotificationBacklogStatus notificationBacklogStatu in notificationEventBacklogStatus.NotificationBacklogStatus)
      {
        flag = flag || notificationBacklogStatu.UnprocessedNotifications > 0;
        CustomerIntelligenceData ciData = new CustomerIntelligenceData();
        ciData.Add("JobId", (object) notificationBacklogStatu.JobId);
        ciData.Add("Publisher", notificationBacklogStatu.Publisher);
        ciData.Add("Channel", notificationBacklogStatu.Channel);
        ciData.Add("Status", notificationBacklogStatu.Status);
        ciData.Add("UnprocessedNotifications", (double) notificationBacklogStatu.UnprocessedNotifications);
        ciData.Add("OldestPendingNotificationTime", (object) notificationBacklogStatu.OldestPendingNotificationTime);
        ciData.Add("LastNotificationBatchStartTime", (object) notificationBacklogStatu.LastNotificationBatchStartTime);
        ciData.Add("LastNotificationProcessedTime", (object) notificationBacklogStatu.LastNotificationProcessedTime);
        ciData.Add("LastJobBatchStartTime", (object) notificationBacklogStatu.LastJobBatchStartTime);
        ciData.Add("LastJobProcessedTime", (object) notificationBacklogStatu.LastJobProcessedTime);
        ciData.Add("CaptureTime", (object) notificationBacklogStatu.CaptureTime);
        NotificationCustomerIntelligence.PublishEvent(requestContext, NotificationCustomerIntelligence.BacklogStatusFeature, NotificationCustomerIntelligence.NotificationStatusAction, ciData);
        if (NotificationBacklogUtil.IsStuckJob(requestContext, maxStuckTimeMs, notificationBacklogStatu.JobId, notificationBacklogStatu.UnprocessedNotifications, notificationBacklogStatu.LastJobBatchStartTime, notificationBacklogStatu.LastJobProcessedTime))
          dictionary1.GetOrAddValue<Guid, HashSet<string>>(notificationBacklogStatu.JobId).Add(notificationBacklogStatu.Publisher + "." + notificationBacklogStatu.Channel + " ");
        else if (NotificationBacklogUtil.ShouldRescheduleJobIfNeeded(requestContext, maxWaitForScheduleTimeMs, notificationBacklogStatu.JobId, notificationBacklogStatu.UnprocessedNotifications, notificationBacklogStatu.LastJobBatchStartTime, notificationBacklogStatu.LastJobProcessedTime))
          dictionary2.GetOrAddValue<Guid, List<string>>(notificationBacklogStatu.JobId).Add(string.Format("{0}.{1}.{2}:{3} ", (object) notificationBacklogStatu.Publisher, (object) notificationBacklogStatu.Channel, (object) notificationBacklogStatu.Status, (object) notificationBacklogStatu.UnprocessedNotifications));
      }
      foreach (KeyValuePair<Guid, HashSet<string>> keyValuePair in dictionary1)
      {
        Guid key = keyValuePair.Key;
        string str = string.Concat((IEnumerable<string>) keyValuePair.Value);
        requestContext.TraceAlways(1002918, TraceLevel.Warning, NotificationBacklogUtil.s_Area, NotificationBacklogUtil.s_Layer, string.Format("Job {0} has not processed an event within the last {1} milliseconds for '{2}'", (object) key, (object) maxStuckTimeMs, (object) str));
      }
      foreach (KeyValuePair<Guid, List<string>> keyValuePair in dictionary2)
      {
        Guid key = keyValuePair.Key;
        string str = string.Concat((IEnumerable<string>) keyValuePair.Value);
        INotificationJobService service2 = requestContext.GetService<INotificationJobService>();
        requestContext.TraceAlways(1002919, TraceLevel.Warning, NotificationBacklogUtil.s_Area, NotificationBacklogUtil.s_Layer, string.Format("Scheduling {0} since no batch has started within {1} milliseconds for '{2}'", (object) key, (object) maxWaitForScheduleTimeMs, (object) str));
        IVssRequestContext requestContext1 = requestContext;
        Guid jobId = key;
        service2.QueueDelayedJob(requestContext1, jobId, 1, JobPriorityClass.AboveNormal, JobPriorityLevel.Normal);
      }
      if (flag)
        requestContext.GetService<INotificationEventServiceInternal>().QueueNotificationBacklogStatusJob(requestContext);
      return flag;
    }

    internal static bool IsStuckJob(
      IVssRequestContext requestContext,
      int maxStuckTimeMs,
      Guid jobId,
      int pendingCount,
      DateTime lastBatchStart,
      DateTime lastBatchComplete)
    {
      bool flag = false;
      if (pendingCount > 0 && lastBatchStart > DateTime.MinValue && lastBatchComplete < lastBatchStart)
      {
        DateTime dateTime = NotificationBacklogUtil.GetDateTimeNowUtc();
        double totalMilliseconds = (dateTime - lastBatchStart).TotalMilliseconds;
        if (totalMilliseconds < 0.0)
          requestContext.Trace(1002920, TraceLevel.Warning, NotificationBacklogUtil.s_Area, NotificationBacklogUtil.s_Layer, string.Format("Job {0} appears to have started in the future: {1} now: {2}", (object) jobId, (object) lastBatchStart, (object) dateTime));
        flag = totalMilliseconds > (double) maxStuckTimeMs;
      }
      return flag;
    }

    internal static bool ShouldRescheduleJobIfNeeded(
      IVssRequestContext requestContext,
      int maxWaitForScheduleTimeMs,
      Guid jobId,
      int pendingCount,
      DateTime lastBatchStart,
      DateTime lastBatchComplete)
    {
      bool flag = false;
      if (pendingCount > 0)
      {
        flag = lastBatchStart == DateTime.MinValue;
        if (!flag)
        {
          DateTime dateTime = NotificationBacklogUtil.GetDateTimeNowUtc();
          if (lastBatchComplete > lastBatchStart)
            flag = (dateTime - lastBatchComplete).TotalMilliseconds > (double) maxWaitForScheduleTimeMs;
        }
      }
      return flag;
    }

    public delegate DateTime DoGetDateTimeNowUtc();

    private delegate bool CreateKey<T>(string[] pathParts, out T key);
  }
}
