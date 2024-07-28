// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.NotificationProcessor
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class NotificationProcessor
  {
    private static readonly RegistryQuery s_sqlNotificationQuery = new RegistryQuery("/Service/SqlNotification/...");
    private Guid m_author;
    private int m_pollingTimeoutMS = 1000;
    private bool m_alertHeartbeatEnabled;
    private ILockName m_databaseLock;
    private Dictionary<int, NotificationProcessor.DatabaseNotification> m_databaseNotifications = new Dictionary<int, NotificationProcessor.DatabaseNotification>();
    private Dictionary<Guid, NotificationProcessor.HostNotificationInformation> m_notificationInformationByHost = new Dictionary<Guid, NotificationProcessor.HostNotificationInformation>();
    private IVssDeploymentServiceHost m_deploymentServiceHost;
    private const int c_pollingTimeoutMS = 1000;
    private const string s_Area = "SqlNotification";
    private const string s_Layer = "NotificationProcessor";

    public NotificationProcessor(IVssRequestContext systemRequestContext)
    {
      TeamFoundationHostManagementService service = systemRequestContext.GetService<TeamFoundationHostManagementService>();
      this.m_deploymentServiceHost = systemRequestContext.ServiceHost.DeploymentServiceHost;
      if (systemRequestContext.IsServicingContext)
      {
        this.m_author = Guid.NewGuid();
        this.m_databaseLock = systemRequestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/Servicing", (object) this.GetType().FullName));
      }
      else
      {
        this.m_author = service.ProcessId;
        this.m_databaseLock = systemRequestContext.ServiceHost.CreateLockName(this.GetType().FullName);
      }
      this.LoadSettings(systemRequestContext);
      this.RegisterNotification(systemRequestContext, systemRequestContext.ServiceHost.InstanceId, (NotificationProcessor.INotification) new NotificationProcessor.NotificationSqlCallback("Default", Guid.Empty, SqlNotificationEventClasses.RegistrySettingsChanged, new SqlNotificationCallback(this.OnRegistryChanged)));
    }

    public void Unload(IVssRequestContext systemRequestContext)
    {
      using (systemRequestContext.Lock(this.m_databaseLock))
      {
        using (systemRequestContext.AcquireExemptionLock())
        {
          NotificationProcessor.HostNotificationInformation notificationInformation;
          if (!this.m_notificationInformationByHost.TryGetValue(systemRequestContext.ServiceHost.InstanceId, out notificationInformation))
            return;
          foreach (NotificationProcessor.DatabaseNotification databaseNotification in notificationInformation.DatabaseNotifications)
          {
            if (databaseNotification.Cleanup(systemRequestContext))
              this.m_databaseNotifications.Remove(databaseNotification.DatabaseId);
          }
          this.m_notificationInformationByHost.Remove(systemRequestContext.ServiceHost.InstanceId);
        }
      }
    }

    internal bool RegisterNotification(
      IVssRequestContext requestContext,
      Guid serviceHostId,
      NotificationProcessor.INotification notification)
    {
      ITeamFoundationDatabaseProperties databaseProperties = this.GetDatabaseProperties(requestContext, notification.DatabaseCategory, notification.DataspaceIdentifier);
      if (databaseProperties == null)
        return false;
      if (databaseProperties.SqlConnectionInfo == null)
      {
        TeamFoundationTracingService.TraceRaw(68080, TraceLevel.Warning, "SqlNotification", nameof (NotificationProcessor), "Ignoring RegisterNotification call. No SqlConnectionInfo for {0} (databaseId: {1}).", (object) databaseProperties.DatabaseName, (object) databaseProperties.DatabaseId);
        return false;
      }
      bool publishAlertHeartbeat = this.m_alertHeartbeatEnabled && !requestContext.IsServicingContext && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && databaseProperties.DatabaseId == requestContext.ServiceHost.ServiceHostInternal().DatabaseId;
      NotificationProcessor.NotificationKey key = new NotificationProcessor.NotificationKey(requestContext, notification.DatabaseCategory, notification.DataspaceIdentifier);
      NotificationProcessor.DatabaseNotification databaseNotification1;
      while (true)
      {
        NotificationProcessor.DatabaseNotification databaseNotification2;
        do
        {
          using (requestContext.Lock(this.m_databaseLock))
          {
            if (this.m_databaseNotifications.TryGetValue(databaseProperties.DatabaseId, out databaseNotification1))
            {
              NotificationProcessor.HostNotificationInformation notificationInformation;
              if (!this.m_notificationInformationByHost.TryGetValue(serviceHostId, out notificationInformation))
              {
                notificationInformation = NotificationProcessor.HostNotificationInformation.Create();
                this.m_notificationInformationByHost.Add(serviceHostId, notificationInformation);
              }
              notificationInformation.DatabaseNotifications.Add(databaseNotification1);
              int num;
              if (notificationInformation.DatabaseIds.TryGetValue(key, out num) && num != databaseProperties.DatabaseId)
                TeamFoundationTracingService.TraceRaw(68085, TraceLevel.Error, "SqlNotification", nameof (NotificationProcessor), "ServiceHost {0}, DatabaseCategory {1}, and DataspaceIdentifier {2} was previously used to register a notification on databaseId {3} but is now registering a notification on databaseId {4}", (object) requestContext.ServiceHost.InstanceId, (object) notification.DatabaseCategory, (object) notification.DataspaceIdentifier, (object) num, (object) databaseProperties.DatabaseId);
              notificationInformation.DatabaseIds[key] = databaseProperties.DatabaseId;
              goto label_23;
            }
          }
          databaseNotification2 = new NotificationProcessor.DatabaseNotification(requestContext.Elevate(), databaseProperties, this.m_pollingTimeoutMS, publishAlertHeartbeat, this.m_author);
          using (requestContext.Lock(this.m_databaseLock))
          {
            if (!this.m_databaseNotifications.TryGetValue(databaseProperties.DatabaseId, out databaseNotification1))
            {
              databaseNotification1 = databaseNotification2;
              this.m_databaseNotifications[databaseProperties.DatabaseId] = databaseNotification1;
              databaseNotification2 = (NotificationProcessor.DatabaseNotification) null;
            }
          }
        }
        while (databaseNotification2 == null);
        databaseNotification2.Cleanup(requestContext);
      }
label_23:
      databaseNotification1.AddNotification(requestContext, serviceHostId, notification);
      return true;
    }

    public void UnregisterNotification(
      IVssRequestContext requestContext,
      Guid serviceHostId,
      NotificationProcessor.INotification notification,
      bool waitForInFlightNotifications)
    {
      NotificationProcessor.NotificationKey key1 = new NotificationProcessor.NotificationKey(requestContext, notification.DatabaseCategory, notification.DataspaceIdentifier);
      NotificationProcessor.DatabaseNotification databaseNotification;
      using (requestContext.Lock(this.m_databaseLock))
      {
        NotificationProcessor.HostNotificationInformation notificationInformation;
        int key2;
        if (!this.m_notificationInformationByHost.TryGetValue(serviceHostId, out notificationInformation) || !notificationInformation.DatabaseIds.TryGetValue(key1, out key2))
          return;
        if (!this.m_databaseNotifications.TryGetValue(key2, out databaseNotification))
          return;
      }
      databaseNotification.RemoveNotification(requestContext, serviceHostId, notification, waitForInFlightNotifications);
    }

    public long SendNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData,
      Guid serviceHostId)
    {
      using (SqlNotificationComponent componentRaw = TeamFoundationResourceManagementService.CreateComponentRaw<SqlNotificationComponent>(requestContext.FrameworkConnectionInfo, deadlockPause: 0, maxDeadlockRetries: 8))
        return componentRaw.SendNotification(eventClass, eventData, this.Author, serviceHostId);
    }

    public long SendNotification(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid eventClass,
      string eventData,
      Guid serviceHostId)
    {
      using (SqlNotificationComponent component = requestContext.CreateComponent<SqlNotificationComponent>(databaseCategory))
        return component.SendNotification(eventClass, eventData, this.Author, serviceHostId);
    }

    internal void SetNotificationHangLimit(TimeSpan? hangLimit) => this.m_databaseNotifications.Values.ForEach<NotificationProcessor.DatabaseNotification>((Action<NotificationProcessor.DatabaseNotification>) (x => x.SetNotificationHangLimit(hangLimit)));

    internal string LastEventDetails() => string.Join(Environment.NewLine, this.m_databaseNotifications.Values.Select<NotificationProcessor.DatabaseNotification, string>((Func<NotificationProcessor.DatabaseNotification, string>) (x => string.Format("Database Id: {0} Last Event Id: {1}", (object) x.DatabaseId, (object) x.LastEventId))));

    private void LoadSettings(IVssRequestContext requestContext)
    {
      try
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          this.m_pollingTimeoutMS = RegistryHelpers.GetDeploymentValueRaw<int>(requestContext.FrameworkConnectionInfo, FrameworkServerConstants.SqlNotificationPollingTimeoutRegistryPath, 1000);
          if (!requestContext.IsServicingContext)
            this.m_alertHeartbeatEnabled = RegistryHelpers.GetDeploymentValueRaw<bool>(requestContext.FrameworkConnectionInfo, FrameworkServerConstants.SqlNotificationPublishAlertHeartRegistryPath, requestContext.ExecutionEnvironment.IsHostedDeployment);
        }
        else
        {
          IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
          this.m_pollingTimeoutMS = context.GetService<CachedRegistryService>().GetValue<int>(context.Elevate(), (RegistryQuery) FrameworkServerConstants.SqlNotificationPollingTimeoutRegistryPath, 1000);
          this.m_alertHeartbeatEnabled = false;
        }
      }
      catch (DatabaseConnectionException ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.DatabaseConnectionException(), (Exception) ex, TeamFoundationEventId.DatabaseConnectionException, EventLogEntryType.Warning);
        this.m_pollingTimeoutMS = 1000;
      }
      this.m_pollingTimeoutMS = Math.Max(100, Math.Min(this.m_pollingTimeoutMS, 60000));
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (!SqlRegistryService.DeserializeSqlNotification(requestContext, eventData).Filter(NotificationProcessor.s_sqlNotificationQuery).Any<RegistryItem>())
        return;
      this.LoadSettings(requestContext);
      using (requestContext.Lock(this.m_databaseLock))
      {
        foreach (NotificationProcessor.DatabaseNotification databaseNotification in this.m_databaseNotifications.Values)
          databaseNotification.UpdateNotificationSettings(requestContext, this.m_pollingTimeoutMS);
      }
    }

    private ITeamFoundationDatabaseProperties GetDatabaseProperties(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier)
    {
      if (!string.Equals("Default", dataspaceCategory, StringComparison.OrdinalIgnoreCase) || !(dataspaceIdentifier == Guid.Empty))
        return requestContext.GetService<IDataspaceService>().GetDatabaseProperties(requestContext, dataspaceCategory, dataspaceIdentifier);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties;
      if (!requestContext.ServiceHost.HasDatabaseAccess)
        return (ITeamFoundationDatabaseProperties) null;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(vssRequestContext, requestContext.ServiceHost.ServiceHostInternal().DatabaseId, true);
    }

    public Guid Author => this.m_author;

    private class DatabaseNotification
    {
      private static readonly Stopwatch NotificationElapsedTime = new Stopwatch();
      private IVssServiceHost m_serviceHost;
      private Guid m_serviceHostInstanceId;
      private Guid m_author;
      private int m_pollingTimeoutMS;
      private bool m_publishAlertHeartbeat;
      private long m_lastEventId = -1;
      private Dictionary<Guid, long> m_globalWatermarks = new Dictionary<Guid, long>();
      private ITeamFoundationDatabaseProperties m_targetDbProperties;
      private string m_ds;
      private string m_db;
      private Dictionary<Guid, Dictionary<Guid, HashSet<NotificationProcessor.INotification>>> m_notificationsByClassByHost = new Dictionary<Guid, Dictionary<Guid, HashSet<NotificationProcessor.INotification>>>();
      private TeamFoundationTaskService m_taskService;
      private ILockName m_notificationAccessLock;
      private const string s_Area = "SqlNotification";
      private const string s_Layer = "DatabaseNotification";
      private DateTime m_lastHeartBeat = DateTime.MinValue;
      private static readonly TimeSpan s_heartBeatInterval = TimeSpan.FromMinutes(5.0);
      private ProcessThread m_thread;
      private TimeSpan m_cpuTime;
      private TimeSpan m_hungNotification = NotificationProcessor.DatabaseNotification.s_defaultHungNotification;
      private bool m_checkHungNotificationForAllHosts;
      private const string c_kpiArea = "Microsoft.TeamFoundation.SqlNotificationMetrics";
      private const string c_kpiTaskTimeName = "DatabaseNotificationTaskTimeInMS";
      private const string c_kpiTaskCallbackTimeName = "DatabaseNotificationTaskCallbackTimeInMS";
      private const string c_kpiNotificationCountName = "DatabaseNotificationCount";
      private static readonly TimeSpan s_defaultHungNotification = TimeSpan.FromMinutes(3.0);

      static DatabaseNotification() => NotificationProcessor.DatabaseNotification.NotificationElapsedTime.Start();

      private static void IncrementElapsedTime(object taskArg) => VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_TimeSinceLastNotification").SetValue((long) NotificationProcessor.DatabaseNotification.NotificationElapsedTime.Elapsed.TotalSeconds);

      public static void ResetNotificationElapsedTime() => NotificationProcessor.DatabaseNotification.NotificationElapsedTime.Restart();

      public DatabaseNotification(
        IVssRequestContext systemRequestContext,
        ITeamFoundationDatabaseProperties targetDbProperties,
        int pollingTimeoutMS,
        bool publishAlertHeartbeat,
        Guid author)
      {
        this.m_serviceHost = systemRequestContext.ServiceHost;
        this.m_serviceHostInstanceId = this.m_serviceHost.InstanceId;
        this.m_targetDbProperties = targetDbProperties;
        this.m_pollingTimeoutMS = pollingTimeoutMS;
        this.m_publishAlertHeartbeat = publishAlertHeartbeat;
        this.m_author = author;
        this.m_db = targetDbProperties.SqlConnectionInfo.DataSource;
        this.m_ds = targetDbProperties.SqlConnectionInfo.InitialCatalog;
        this.m_notificationAccessLock = systemRequestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/{1}/{2}", (object) this.GetType().FullName, (object) this.m_ds, (object) this.m_db));
        this.GetWatermark(this.m_targetDbProperties.SqlConnectionInfo);
        if (this.GetAggressivelyLogHungNotifications(systemRequestContext))
          this.m_checkHungNotificationForAllHosts = true;
        TeamFoundationTracingService.TraceRaw(68080, TraceLevel.Info, "SqlNotification", nameof (DatabaseNotification), "Watermark {0} for ds:{1} db:{2}", (object) this.m_lastEventId, (object) this.m_ds, (object) this.m_db);
        this.m_taskService = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
        TeamFoundationTracingService.TraceRaw(68082, TraceLevel.Info, "SqlNotification", nameof (DatabaseNotification), "Adding Task For SQL Notification: ds:{0} db:{1}", (object) this.m_ds, (object) this.m_db);
        this.AddNotificationTask(systemRequestContext);
      }

      public bool Cleanup(IVssRequestContext requestContext)
      {
        using (requestContext.Lock(this.m_notificationAccessLock))
        {
          TeamFoundationTracingService.TraceRaw(68002, TraceLevel.Verbose, "SqlNotification", nameof (DatabaseNotification), "DatabaseNotification(instance {0}): Removing all notifications for host {1}", (object) this.GetHashCode(), (object) requestContext.ServiceHost);
          this.m_notificationsByClassByHost.Remove(requestContext.ServiceHost.InstanceId);
          if (this.m_notificationsByClassByHost.Count == 0)
          {
            if (this.m_taskService != null)
            {
              TeamFoundationTracingService.TraceRaw(68004, TraceLevel.Info, "SqlNotification", nameof (DatabaseNotification), "Removing Task For SQL Notification: ds:{0} db:{1}", (object) this.m_ds, (object) this.m_db);
              this.m_taskService.RemoveTask(this.m_targetDbProperties.DatabaseId, (TeamFoundationTask<int>) new TeamFoundationNotificationTask(new TeamFoundationTaskCallback(this.NotificationThread), (object) null, DateTime.UtcNow, this.m_pollingTimeoutMS));
            }
            this.m_taskService = (TeamFoundationTaskService) null;
            return true;
          }
        }
        return false;
      }

      public void AddNotification(
        IVssRequestContext requestContext,
        Guid hostId,
        NotificationProcessor.INotification notification)
      {
        using (requestContext.Lock(this.m_notificationAccessLock))
        {
          Dictionary<Guid, HashSet<NotificationProcessor.INotification>> dictionary;
          if (!this.m_notificationsByClassByHost.TryGetValue(hostId, out dictionary))
          {
            TeamFoundationTracingService.TraceRaw(68001, TraceLevel.Verbose, "SqlNotification", nameof (DatabaseNotification), "DatabaseNotification(instance {0}): Adding first notification for host {1}", (object) this.GetHashCode(), (object) hostId);
            dictionary = new Dictionary<Guid, HashSet<NotificationProcessor.INotification>>();
            this.m_notificationsByClassByHost[hostId] = dictionary;
          }
          HashSet<NotificationProcessor.INotification> notificationSet;
          if (!dictionary.TryGetValue(notification.EventClass, out notificationSet))
          {
            notificationSet = new HashSet<NotificationProcessor.INotification>();
            dictionary[notification.EventClass] = notificationSet;
          }
          if (notificationSet.Contains(notification))
            return;
          notificationSet.Add(notification);
        }
      }

      public List<NotificationProcessor.INotification> GetNotifications(
        IVssRequestContext requestContext)
      {
        using (requestContext.Lock(this.m_notificationAccessLock))
        {
          List<NotificationProcessor.INotification> notifications = new List<NotificationProcessor.INotification>();
          foreach (Dictionary<Guid, HashSet<NotificationProcessor.INotification>> dictionary in this.m_notificationsByClassByHost.Values)
          {
            foreach (HashSet<NotificationProcessor.INotification> collection in dictionary.Values)
              notifications.AddRange((IEnumerable<NotificationProcessor.INotification>) collection);
          }
          return notifications;
        }
      }

      public void RemoveNotification(
        IVssRequestContext requestContext,
        Guid hostId,
        NotificationProcessor.INotification notification,
        bool waitForInFlightNotifications)
      {
        using (requestContext.Lock(this.m_notificationAccessLock))
        {
          Dictionary<Guid, HashSet<NotificationProcessor.INotification>> dictionary;
          HashSet<NotificationProcessor.INotification> notificationSet;
          if (!this.m_notificationsByClassByHost.TryGetValue(hostId, out dictionary) || !dictionary.TryGetValue(notification.EventClass, out notificationSet))
            return;
          notificationSet.Remove(notification);
        }
      }

      public void UpdateNotificationSettings(
        IVssRequestContext requestContext,
        int pollingTimeoutMS)
      {
        if (this.m_pollingTimeoutMS == pollingTimeoutMS)
          return;
        this.m_pollingTimeoutMS = pollingTimeoutMS;
        this.AddNotificationTask(requestContext);
      }

      internal void SetNotificationHangLimit(TimeSpan? hangLimit) => this.m_hungNotification = hangLimit ?? NotificationProcessor.DatabaseNotification.s_defaultHungNotification;

      private bool GetAggressivelyLogHungNotifications(IVssRequestContext requestContext)
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          return RegistryHelpers.GetDeploymentValueRaw<bool>(requestContext.FrameworkConnectionInfo, FrameworkServerConstants.SqlNotificationAggressivelyLogHungNotifications);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<IVssRegistryService>().GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.SqlNotificationAggressivelyLogHungNotifications, false);
      }

      private void AddNotificationTask(IVssRequestContext requestContext) => this.m_taskService.AddTask(this.m_targetDbProperties.DatabaseId, (TeamFoundationTask<int>) new TeamFoundationNotificationTask(new TeamFoundationTaskCallback(this.NotificationThread), (object) null, DateTime.UtcNow, this.m_pollingTimeoutMS));

      private void GetWatermark(ISqlConnectionInfo connectionInfo)
      {
        using (SqlNotificationComponent componentRaw = connectionInfo.CreateComponentRaw<SqlNotificationComponent>(60, 0, 8))
        {
          List<KeyValuePair<Guid, long>> globalWatermarks;
          this.m_lastEventId = componentRaw.QueryNotificationWatermark(out globalWatermarks);
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, "SqlNotification", nameof (DatabaseNotification), "Retrieved Watermark {0} for ds:{1} db:{2}", (object) this.m_lastEventId, (object) this.m_ds, (object) this.m_db);
          foreach (KeyValuePair<Guid, long> keyValuePair in globalWatermarks)
            this.m_globalWatermarks[keyValuePair.Key] = keyValuePair.Value;
        }
      }

      [DllImport("kernel32")]
      private static extern int GetCurrentThreadId();

      private void NotificationThread(IVssRequestContext requestContext, object taskArgs)
      {
        long retrievalElapsedTimeMS = 0;
        Stopwatch stopwatch = Stopwatch.StartNew();
        Dictionary<int, int> source1 = new Dictionary<int, int>();
        Dictionary<Guid, NotificationProcessor.DatabaseNotification.EventClassTraceData> source2 = new Dictionary<Guid, NotificationProcessor.DatabaseNotification.EventClassTraceData>();
        Guid guid1 = Guid.Empty;
        Guid guid2 = Guid.Empty;
        int num1 = int.MinValue;
        try
        {
          requestContext.TraceEnter(68030, "SqlNotification", nameof (DatabaseNotification), nameof (NotificationThread));
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentSQLNotificationQueriesPerSec").Increment();
          this.PublishHeartbeat(requestContext);
          List<SqlNotification> sqlNotificationList = this.FetchQueuedNotifications(requestContext, out retrievalElapsedTimeMS);
          if (sqlNotificationList == null)
            return;
          using (requestContext.Lock(this.m_notificationAccessLock))
          {
            int num2 = 0;
            StringBuilder stringBuilder = (StringBuilder) null;
            foreach (SqlNotification sqlNotification in sqlNotificationList)
            {
              if (sqlNotification.IsGlobal)
              {
                long num3;
                if (this.m_globalWatermarks.TryGetValue(sqlNotification.EventClass, out num3) && num3 >= sqlNotification.EventId)
                {
                  requestContext.Trace(68034, TraceLevel.Info, "SqlNotification", nameof (DatabaseNotification), "Skipping global event {0}, current value {1}, retrieved value {2}", (object) sqlNotification.EventClass, (object) num3, (object) sqlNotification.EventId);
                  continue;
                }
                this.m_globalWatermarks[sqlNotification.EventClass] = sqlNotification.EventId;
                requestContext.Trace(68035, TraceLevel.Info, "SqlNotification", nameof (DatabaseNotification), "Global Event {0} Processed is {1}", (object) sqlNotification.EventClass, (object) sqlNotification.EventId);
              }
              else
              {
                if (this.m_lastEventId > 0L && sqlNotification.EventId != this.m_lastEventId + 1L)
                {
                  if (stringBuilder == null)
                    stringBuilder = new StringBuilder();
                  stringBuilder.Append(string.Format("({0} -> {1})", (object) this.m_lastEventId, (object) sqlNotification.EventId));
                }
                if (sqlNotification.EventId > this.m_lastEventId)
                {
                  this.m_lastEventId = sqlNotification.EventId;
                  requestContext.Trace(68036, TraceLevel.Info, "SqlNotification", nameof (DatabaseNotification), "Last Event Id Processed is {0}", (object) this.m_lastEventId);
                }
              }
              num2 += this.MatchNotificationsByHost(requestContext, sqlNotification);
            }
            if (this.m_lastEventId == -1L)
              this.m_lastEventId = 0L;
            if (stringBuilder != null)
            {
              bool flag = requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.DatabaseId == this.DatabaseId;
              requestContext.TraceAlways(68086, TraceLevel.Warning, "SqlNotification", nameof (DatabaseNotification), string.Format("Skipped one or more Event Ids, (for the deployment host Database: {0}): {1}", (object) flag, (object) stringBuilder));
            }
            if (num2 == 0)
            {
              requestContext.Trace(68038, TraceLevel.Verbose, "SqlNotification", nameof (DatabaseNotification), "Zero callbacks found - going back to sleep");
              return;
            }
          }
          TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
          foreach (SqlNotification sqlNotification1 in sqlNotificationList)
          {
            SqlNotification sqlNotification = sqlNotification1;
            if (sqlNotification.m_notifications != null)
            {
              Stopwatch watch = Stopwatch.StartNew();
              int count = 0;
              NotificationEventArgs args = new NotificationEventArgs(sqlNotification.EventId, sqlNotification.EventClass, sqlNotification.EventData);
              string scrubbedEventData = (string) null;
              foreach (KeyValuePair<Guid, List<NotificationProcessor.INotification>> notification1 in sqlNotification.m_notifications)
              {
                Guid targetHostId = notification1.Key;
                List<NotificationProcessor.INotification> notificationList = notification1.Value;
                requestContext.Trace(68045, TraceLevel.Verbose, "SqlNotification", nameof (DatabaseNotification), "Host {0} has {1} notifications registered for eventClass {2}.", (object) targetHostId, (object) notificationList.Count, (object) sqlNotification.EventClass);
                if (notificationList.Count > 0)
                {
                  try
                  {
                    using (IVssRequestContext requestContext1 = service.BeginRequest(requestContext, targetHostId, RequestContextType.SystemContext, false, false))
                    {
                      if (requestContext1 != null)
                      {
                        if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
                          requestContext1.IsTracked = true;
                        Stopwatch callbackWatch = new Stopwatch();
                        foreach (NotificationProcessor.INotification notification2 in notificationList)
                        {
                          MethodInfo methodInfo = notification2.Callback.Method;
                          callbackWatch.Restart();
                          Timer timer = (Timer) null;
                          bool hungTimerEnabled = true;
                          try
                          {
                            if (requestContext.IsTracing(68048, TraceLevel.Verbose, "SqlNotification", "SqlNotification"))
                            {
                              scrubbedEventData = scrubbedEventData ?? SecretUtility.ScrubSecrets(sqlNotification.EventData, false);
                              requestContext.Trace(68048, TraceLevel.Verbose, "SqlNotification", nameof (DatabaseNotification), "Calling callback {0}.{1} with eventClass {2} and eventData {3}", (object) methodInfo.ReflectedType?.FullName, (object) methodInfo.Name, (object) sqlNotification.EventClass, (object) scrubbedEventData);
                            }
                            VssPerformanceEventSource.Log.NotificationCallbackStart(requestContext.ServiceHost.InstanceId, methodInfo.Name);
                            if (this.m_checkHungNotificationForAllHosts || requestContext1.ServiceHost.Is(TeamFoundationHostType.Deployment))
                              timer = new Timer((TimerCallback) (state =>
                              {
                                Thread thread = (Thread) state;
                                if (hungTimerEnabled)
                                  TeamFoundationTracingService.TraceRaw(68079, TraceLevel.Error, "SqlNotification", nameof (DatabaseNotification), string.Format("Notification has been running for {0} and appears to be hung {1}.{2} with eventClass {3} for host {4} and eventData {5} on thread {6}", (object) callbackWatch.Elapsed, (object) methodInfo.ReflectedType?.FullName, (object) methodInfo.Name, (object) sqlNotification.EventClass, (object) targetHostId, (object) scrubbedEventData, (object) thread.Name));
                                else
                                  TeamFoundationTracingService.TraceRaw(68083, TraceLevel.Error, "SqlNotification", nameof (DatabaseNotification), string.Format("Notification has been running for{0} and appears to be running slowly {1}.{2} with eventClass {3} for host {4} and eventData {5} on thread {6}", (object) callbackWatch.Elapsed, (object) methodInfo.ReflectedType?.FullName, (object) methodInfo.Name, (object) sqlNotification.EventClass, (object) targetHostId, (object) scrubbedEventData, (object) thread.Name));
                              }), (object) Thread.CurrentThread, this.m_hungNotification, TimeSpan.Zero);
                            ++count;
                            notification2.Invoke(requestContext1, args);
                            hungTimerEnabled = false;
                          }
                          catch (Exception ex)
                          {
                            hungTimerEnabled = false;
                            requestContext.TraceException(68050, "SqlNotification", nameof (DatabaseNotification), ex);
                          }
                          finally
                          {
                            hungTimerEnabled = false;
                            timer?.Dispose();
                            callbackWatch.Stop();
                            VssPerformanceEventSource.Log.NotificationCallbackStop(requestContext.ServiceHost.InstanceId, methodInfo.Name, callbackWatch.ElapsedMilliseconds);
                            if (callbackWatch.ElapsedMilliseconds > (long) num1)
                            {
                              num1 = (int) callbackWatch.ElapsedMilliseconds;
                              guid1 = sqlNotification.EventClass;
                              guid2 = targetHostId;
                            }
                            int key = (int) (callbackWatch.ElapsedMilliseconds / 1000L);
                            int num4;
                            source1.TryGetValue(key, out num4);
                            source1[key] = num4 + 1;
                            TraceLevel level = traceLevel(callbackWatch.ElapsedMilliseconds, 30000L, 5000L);
                            requestContext.Trace(68055, level, "SqlNotification", nameof (DatabaseNotification), "Callback {0}.{1} with eventClass {2} took {3} ms for host {4}", (object) methodInfo.ReflectedType?.FullName, (object) methodInfo.Name, (object) sqlNotification.EventClass, (object) callbackWatch.ElapsedMilliseconds, (object) targetHostId);
                          }
                        }
                      }
                      else
                        requestContext.Trace(68058, TraceLevel.Warning, "SqlNotification", nameof (DatabaseNotification), "Host {0} is not loaded but has {1} notifications registered for eventClass {2}. Callbacks skipped.", (object) targetHostId, (object) notificationList.Count, (object) sqlNotification.EventClass);
                    }
                  }
                  catch (HostDoesNotExistException ex)
                  {
                    requestContext.Trace(68060, TraceLevel.Warning, "SqlNotification", nameof (DatabaseNotification), "Host {0} does not exist but has {1} notifications registered for eventClass {2}. Callbacks skipped. Exception: {3}", (object) targetHostId, (object) notificationList.Count, (object) sqlNotification.EventClass, (object) ex);
                  }
                  catch (Exception ex)
                  {
                    requestContext.TraceException(68065, "SqlNotification", nameof (DatabaseNotification), ex);
                  }
                }
              }
              watch.Stop();
              NotificationProcessor.DatabaseNotification.EventClassTraceData past;
              source2.TryGetValue(sqlNotification.EventClass, out past);
              source2[sqlNotification.EventClass] = new NotificationProcessor.DatabaseNotification.EventClassTraceData(past, watch, count);
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(68069, "SqlNotification", nameof (DatabaseNotification), ex);
        }
        finally
        {
          stopwatch.Stop();
          int notificationCount = source1.Sum<KeyValuePair<int, int>>((Func<KeyValuePair<int, int>, int>) (x => x.Value));
          TraceLevel level1 = traceLevel(stopwatch.ElapsedMilliseconds, 120000L, 60000L, 10000L);
          if (level1 != TraceLevel.Off)
          {
            if (notificationCount == 0)
              requestContext.Trace(68057, level1, "SqlNotification", nameof (DatabaseNotification), "No callbacks were completed For Database {0} in {1}s. Check traces for ActivityId: {2}", (object) this.DatabaseName, (object) stopwatch.Elapsed.TotalSeconds, (object) requestContext.ActivityId);
            else if (requestContext.IsTracing(68056, level1, "SqlNotification", nameof (DatabaseNotification)))
            {
              KeyValuePair<Guid, NotificationProcessor.DatabaseNotification.EventClassTraceData> keyValuePair1 = source2.OrderByDescending<KeyValuePair<Guid, NotificationProcessor.DatabaseNotification.EventClassTraceData>, long>((Func<KeyValuePair<Guid, NotificationProcessor.DatabaseNotification.EventClassTraceData>, long>) (x => x.Value.ElapsedTimeMS)).FirstOrDefault<KeyValuePair<Guid, NotificationProcessor.DatabaseNotification.EventClassTraceData>>();
              KeyValuePair<Guid, NotificationProcessor.DatabaseNotification.EventClassTraceData> keyValuePair2 = source2.OrderByDescending<KeyValuePair<Guid, NotificationProcessor.DatabaseNotification.EventClassTraceData>, int>((Func<KeyValuePair<Guid, NotificationProcessor.DatabaseNotification.EventClassTraceData>, int>) (x => x.Value.Count)).FirstOrDefault<KeyValuePair<Guid, NotificationProcessor.DatabaseNotification.EventClassTraceData>>();
              IVssRequestContext requestContext2 = requestContext;
              int level2 = (int) level1;
              object[] objArray = new object[12];
              objArray[0] = (object) notificationCount;
              objArray[1] = (object) this.DatabaseName;
              objArray[2] = (object) stopwatch.Elapsed.TotalSeconds;
              objArray[3] = (object) string.Join(", ", source1.OrderByDescending<KeyValuePair<int, int>, int>((Func<KeyValuePair<int, int>, int>) (x => x.Key)).Take<KeyValuePair<int, int>>(5).Select<KeyValuePair<int, int>, string>((Func<KeyValuePair<int, int>, string>) (x => string.Format("({0}s Count {1})", (object) x.Key, (object) x.Value))));
              objArray[4] = (object) string.Join(", ", source1.OrderByDescending<KeyValuePair<int, int>, int>((Func<KeyValuePair<int, int>, int>) (x => x.Value)).Take<KeyValuePair<int, int>>(5).Select<KeyValuePair<int, int>, string>((Func<KeyValuePair<int, int>, string>) (x => string.Format("({0}s Count {1})", (object) x.Key, (object) x.Value))));
              Guid key = keyValuePair1.Key;
              objArray[5] = (object) key.ToString("D");
              objArray[6] = (object) keyValuePair1.Value;
              key = keyValuePair2.Key;
              objArray[7] = (object) key.ToString("D");
              objArray[8] = (object) keyValuePair2.Value;
              objArray[9] = (object) guid2;
              objArray[10] = (object) guid1;
              objArray[11] = (object) (num1 / 1000);
              VssRequestContextExtensions.Trace(requestContext2, 68056, (TraceLevel) level2, "SqlNotification", nameof (DatabaseNotification), "Completed {0} Callbacks For Database {1} in {2}s. Longest Buckets {3}. Most populated buckets {4}. Event Class with max Elapsed Time {5} {6}. Event Class with max Notifications {7} {8}. Notification that took the longest to process Host {9}, Event Class {10}, Elapsed {11}s.", objArray);
            }
          }
          VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlNotificationTaskTime");
          performanceCounter.IncrementMilliseconds(stopwatch.ElapsedMilliseconds);
          performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlNotificationTaskTimeBase");
          performanceCounter.Increment();
          long num5 = stopwatch.ElapsedMilliseconds - retrievalElapsedTimeMS;
          performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlNotificationTaskCallbackTime");
          performanceCounter.IncrementMilliseconds(num5);
          performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlNotificationTaskCallbackTimeBase");
          performanceCounter.Increment();
          this.PublishNotificationTaskKpis(requestContext, stopwatch.ElapsedMilliseconds, num5, notificationCount);
        }

        static TraceLevel traceLevel(long elapsedMS, long error, long warn, long info = -1)
        {
          if (elapsedMS > error)
            return TraceLevel.Error;
          if (elapsedMS > warn)
            return TraceLevel.Warning;
          return elapsedMS <= info ? TraceLevel.Off : TraceLevel.Info;
        }
      }

      private void PublishHeartbeat(IVssRequestContext requestContext)
      {
        if (!(DateTime.UtcNow - this.m_lastHeartBeat > NotificationProcessor.DatabaseNotification.s_heartBeatInterval) || requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.DatabaseId != this.DatabaseId)
          return;
        if (this.m_thread == null)
        {
          int currentThreadId = NotificationProcessor.DatabaseNotification.GetCurrentThreadId();
          foreach (ProcessThread thread in (ReadOnlyCollectionBase) Process.GetCurrentProcess().Threads)
          {
            if (thread.Id == currentThreadId)
            {
              this.m_thread = thread;
              this.m_cpuTime = thread.TotalProcessorTime;
            }
          }
        }
        this.m_lastHeartBeat = DateTime.UtcNow;
        TeamFoundationTracingService.TraceRawAlwaysOn(68040, TraceLevel.Info, "SqlNotification", nameof (DatabaseNotification), string.Format("Deployment Notification Heartbeat CPU time: {0}, Last Event Id: {1}", (object) (this.m_thread.TotalProcessorTime - this.m_cpuTime).TotalMilliseconds, (object) this.m_lastEventId));
        this.m_cpuTime = this.m_thread.TotalProcessorTime;
      }

      private List<SqlNotification> FetchQueuedNotifications(
        IVssRequestContext requestContext,
        out long retrievalElapsedTimeMS)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
          using (SqlNotificationComponent componentRaw = this.m_targetDbProperties.GetDboConnectionInfo().CreateComponentRaw<SqlNotificationComponent>(60, 0, 8))
          {
            List<SqlNotification> sqlNotificationList = componentRaw.QueryNotifications(this.m_lastEventId);
            NotificationProcessor.DatabaseNotification.ResetNotificationElapsedTime();
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_LastNotificationQueueLength").SetValue((long) sqlNotificationList.Count);
            return sqlNotificationList;
          }
        }
        catch (TeamFoundationServiceException ex)
        {
          requestContext.TraceException(68032, "SqlNotification", nameof (DatabaseNotification), (Exception) ex);
          return (List<SqlNotification>) null;
        }
        finally
        {
          stopwatch.Stop();
          retrievalElapsedTimeMS = stopwatch.ElapsedMilliseconds;
          requestContext.Trace(68033, TraceLevel.Info, "SqlNotification", nameof (DatabaseNotification), "Notifications fetched in {0} ms for ds:{1} db:{2}", (object) retrievalElapsedTimeMS, (object) this.m_ds, (object) this.m_db);
        }
      }

      private int MatchNotificationsByHost(
        IVssRequestContext requestContext,
        SqlNotification sqlNotification)
      {
        int num = 0;
        Dictionary<Guid, List<NotificationProcessor.INotification>> dictionary = new Dictionary<Guid, List<NotificationProcessor.INotification>>();
        if (sqlNotification.HostId == Guid.Empty)
        {
          foreach (KeyValuePair<Guid, Dictionary<Guid, HashSet<NotificationProcessor.INotification>>> keyValuePair in this.m_notificationsByClassByHost)
          {
            List<NotificationProcessor.INotification> notificationList = this.MatchNotifications(requestContext, sqlNotification, keyValuePair.Value);
            if (notificationList.Count > 0)
            {
              dictionary.Add(keyValuePair.Key, notificationList);
              num += notificationList.Count;
            }
          }
        }
        else
        {
          Dictionary<Guid, HashSet<NotificationProcessor.INotification>> notificationsByClass;
          if (this.m_notificationsByClassByHost.TryGetValue(sqlNotification.HostId, out notificationsByClass))
          {
            List<NotificationProcessor.INotification> notificationList = this.MatchNotifications(requestContext, sqlNotification, notificationsByClass);
            if (notificationList.Count > 0)
            {
              dictionary.Add(sqlNotification.HostId, notificationList);
              num += notificationList.Count;
            }
          }
        }
        if (dictionary.Count > 0)
          sqlNotification.m_notifications = dictionary;
        else
          requestContext.Trace(68037, TraceLevel.Verbose, "SqlNotification", nameof (DatabaseNotification), "No callbacks matched");
        return num;
      }

      private void PublishNotificationTaskKpis(
        IVssRequestContext requestContext,
        long taskTimeMS,
        long taskCallbackTimeMS,
        int notificationCount)
      {
        IKpiService service = requestContext.GetService<IKpiService>();
        List<KpiMetric> kpiMetricList = new List<KpiMetric>()
        {
          new KpiMetric()
          {
            Name = "DatabaseNotificationTaskTimeInMS",
            Value = (double) taskTimeMS
          },
          new KpiMetric()
          {
            Name = "DatabaseNotificationTaskCallbackTimeInMS",
            Value = (double) taskCallbackTimeMS
          },
          new KpiMetric()
          {
            Name = "DatabaseNotificationCount",
            Value = (double) notificationCount
          }
        };
        IVssRequestContext requestContext1 = requestContext;
        string databaseName = this.DatabaseName;
        List<KpiMetric> metrics = kpiMetricList;
        service.Publish(requestContext1, "Microsoft.TeamFoundation.SqlNotificationMetrics", databaseName, metrics);
      }

      private List<NotificationProcessor.INotification> MatchNotifications(
        IVssRequestContext requestContext,
        SqlNotification sqlNotification,
        Dictionary<Guid, HashSet<NotificationProcessor.INotification>> notificationsByClass)
      {
        List<NotificationProcessor.INotification> notificationList = new List<NotificationProcessor.INotification>();
        int num1 = 0;
        int num2 = 0;
        HashSet<NotificationProcessor.INotification> notificationSet;
        if (notificationsByClass.TryGetValue(sqlNotification.EventClass, out notificationSet))
        {
          requestContext.Trace(68074, TraceLevel.Verbose, "SqlNotification", nameof (DatabaseNotification), "Found notification {0} {1}", (object) sqlNotification.EventClass, (object) notificationSet);
          foreach (NotificationProcessor.INotification notification in notificationSet)
          {
            if (!notification.FilterByAuthor || sqlNotification.EventAuthor != this.m_author)
            {
              notificationList.Add(notification);
              ++num1;
            }
            else
            {
              requestContext.Trace(68076, TraceLevel.Verbose, "SqlNotification", nameof (DatabaseNotification), "Ignoring Notification because authors match ({0})", (object) sqlNotification.EventAuthor);
              ++num2;
            }
          }
        }
        requestContext.Trace(68078, TraceLevel.Verbose, "SqlNotification", nameof (DatabaseNotification), "MatchCallbacks: {0} callbacks matched {1} eventclass. {1} callsbacks filtered by author.", (object) sqlNotification.EventClass, (object) num1, (object) num2);
        return notificationList;
      }

      public int DatabaseId => this.m_targetDbProperties.DatabaseId;

      public long LastEventId => this.m_lastEventId;

      private string DatabaseName => this.m_targetDbProperties.DatabaseName;

      private struct EventClassTraceData
      {
        public readonly long ElapsedTimeMS;
        public readonly int Count;

        public EventClassTraceData(
          NotificationProcessor.DatabaseNotification.EventClassTraceData past,
          Stopwatch watch,
          int count)
        {
          this.ElapsedTimeMS = past.ElapsedTimeMS + watch.ElapsedMilliseconds;
          this.Count = past.Count + count;
        }

        public override string ToString() => string.Format("{0}s {1}", (object) (int) (this.ElapsedTimeMS / 1000L), (object) this.Count);
      }
    }

    internal interface INotification
    {
      string DatabaseCategory { get; }

      Guid DataspaceIdentifier { get; }

      Guid EventClass { get; }

      bool FilterByAuthor { get; }

      Delegate Callback { get; }

      void Invoke(IVssRequestContext requestContext, NotificationEventArgs args);
    }

    internal abstract class Notification : NotificationProcessor.INotification
    {
      public Notification(
        string databaseCategory,
        Guid dataspaceIdentifier,
        Guid eventClass,
        Delegate callback,
        bool filterByAuthor = false)
      {
        this.DatabaseCategory = databaseCategory;
        this.DataspaceIdentifier = dataspaceIdentifier;
        this.EventClass = eventClass;
        this.FilterByAuthor = filterByAuthor;
        this.Callback = callback;
      }

      public override int GetHashCode() => this.Callback.Target != null ? this.Callback.Method.GetHashCode() ^ this.Callback.Target.GetHashCode() : this.Callback.Method.GetHashCode();

      public override bool Equals(object obj) => obj is NotificationProcessor.Notification notification && (object) notification.Callback != null && this.Callback.Method == notification.Callback.Method && this.Callback.Target == notification.Callback.Target;

      public abstract void Invoke(IVssRequestContext requestContext, NotificationEventArgs args);

      public string DatabaseCategory { get; private set; }

      public Guid DataspaceIdentifier { get; private set; }

      public Guid EventClass { get; private set; }

      public bool FilterByAuthor { get; private set; }

      public Delegate Callback { get; private set; }
    }

    internal class NotificationSqlCallback : NotificationProcessor.Notification
    {
      public NotificationSqlCallback(
        string databaseCategory,
        Guid dataspaceIdentifier,
        Guid eventClass,
        SqlNotificationCallback callback,
        bool filterByAuthor = false)
        : base(databaseCategory, dataspaceIdentifier, eventClass, (Delegate) callback, filterByAuthor)
      {
      }

      public override void Invoke(IVssRequestContext requestContext, NotificationEventArgs args) => this.Callback.DynamicInvoke((object) requestContext, (object) args.Class, (object) args.Data);
    }

    internal class NotificationSqlHandler : NotificationProcessor.Notification
    {
      public NotificationSqlHandler(
        string databaseCategory,
        Guid dataspaceIdentifier,
        Guid eventClass,
        SqlNotificationHandler handler,
        bool filterByAuthor = false)
        : base(databaseCategory, dataspaceIdentifier, eventClass, (Delegate) handler, filterByAuthor)
      {
      }

      public override void Invoke(IVssRequestContext requestContext, NotificationEventArgs args) => this.Callback.DynamicInvoke((object) requestContext, (object) args);
    }

    private struct HostNotificationInformation
    {
      public static NotificationProcessor.HostNotificationInformation Create() => new NotificationProcessor.HostNotificationInformation()
      {
        DatabaseIds = (IDictionary<NotificationProcessor.NotificationKey, int>) new Dictionary<NotificationProcessor.NotificationKey, int>(),
        DatabaseNotifications = new HashSet<NotificationProcessor.DatabaseNotification>()
      };

      public IDictionary<NotificationProcessor.NotificationKey, int> DatabaseIds { get; private set; }

      public HashSet<NotificationProcessor.DatabaseNotification> DatabaseNotifications { get; private set; }
    }

    private struct NotificationKey : IEquatable<NotificationProcessor.NotificationKey>
    {
      public NotificationKey(
        IVssRequestContext requestContext,
        string dataspaceCategory,
        Guid dataspaceIdentifier)
      {
        this.ServiceHostId = requestContext.ServiceHost.InstanceId;
        this.DataspaceCategory = dataspaceCategory?.ToLower();
        this.DataspaceIdentifier = dataspaceIdentifier;
      }

      public Guid ServiceHostId { get; }

      public string DataspaceCategory { get; }

      public Guid DataspaceIdentifier { get; }

      public bool Equals(NotificationProcessor.NotificationKey other) => this.ServiceHostId == other.ServiceHostId && string.Equals(this.DataspaceCategory, other.DataspaceCategory) && this.DataspaceIdentifier == other.DataspaceIdentifier;

      public override bool Equals(object obj) => obj is NotificationProcessor.NotificationKey other && this.Equals(other);

      public override int GetHashCode()
      {
        int num = (1231 * 3037 + this.ServiceHostId.GetHashCode()) * 3037;
        int? hashCode = this.DataspaceCategory?.GetHashCode();
        return (hashCode.HasValue ? new int?(num + hashCode.GetValueOrDefault()) : new int?()).GetValueOrDefault() * 3037 + this.DataspaceIdentifier.GetHashCode();
      }
    }
  }
}
