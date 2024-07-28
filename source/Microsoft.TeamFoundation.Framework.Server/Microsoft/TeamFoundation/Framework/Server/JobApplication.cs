// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobApplication
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.JobAgent;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class JobApplication : IDisposable
  {
    private ISqlConnectionInfo m_applicationDbConnectionInfo;
    private Guid m_applicationId;
    private int m_maximumStopTime;
    private TimeSpan m_maxQueueCheckInterval;
    private int m_maxEventWaitTimeout;
    private DateTime m_lastInactiveProcessCheckTime;
    private DateTime m_lastQueueCheckTime;
    private DateTime m_nextQueueEntryTime = DateTime.MaxValue;
    private DateTime m_nextMinDisabledTraceTime = DateTime.MinValue;
    private DateTime m_nextMinThrottlingTraceTime = DateTime.MinValue;
    private DateTime m_nextMinThrottlingTraceTimeWithOffset = DateTime.MinValue;
    private bool m_checkQueue = true;
    private TeamFoundationJobService m_jobService;
    private Guid m_agentId;
    private ITeamFoundationSqlNotificationService m_sqlNotificationService;
    private TeamFoundationTaskService m_taskService;
    private TeamFoundationHostManagementService m_hostManagementService;
    private DeploymentServiceHost m_deploymentServiceHost;
    private static readonly string s_processLockName = string.Format("tfsjobagent:{0}", (object) Process.GetCurrentProcess().Id);
    private Mutex m_processLock;
    private static readonly string s_startupTime;
    private static int s_deploymentHostsCreated;
    private string m_deploymentServiceHostCreatedTime;
    private string m_teardownBeginTime;
    private string m_stopAllJobsBeginTime;
    private string m_disposeDeploymentServiceHostTime;
    private string m_jobApplicationDisposedTime;
    private string m_logAndTerminateTime;
    private TeamFoundationTask m_unregisterRescheduleTask;
    private bool m_deploymentWasActive;
    private bool m_disposing;
    private DateTime? m_disposed;
    private Thread m_runThread;
    private Dictionary<string, Type> m_plugins;
    private AutoResetEvent m_queueChangedEvent = new AutoResetEvent(true);
    private ManualResetEvent m_deploymentHostStoppedEvent = new ManualResetEvent(false);
    private ManualResetEvent m_processStoppedEvent = new ManualResetEvent(false);
    private AutoResetEvent m_runnerCompletedEvent = new AutoResetEvent(false);
    private AutoResetEvent m_leaseRenewedEvent = new AutoResetEvent(false);
    private IDisposableReadOnlyList<IJobApplicationExtension> m_applicationExtensions;
    private int m_retryDelaySeconds = 20;
    private const int DefaultSlowQueueThresholdMilliseconds = 1000;
    private ILockName m_jobRunnerLockName;
    private readonly Dictionary<TeamFoundationJobQueueEntry, JobRunner> m_jobRunners = new Dictionary<TeamFoundationJobQueueEntry, JobRunner>((IEqualityComparer<TeamFoundationJobQueueEntry>) JobApplication.JobComparer.Instance);
    private List<JobRunner> m_jobsToRelease = new List<JobRunner>();
    private List<JobRunner> m_newlyCompletedJobs = new List<JobRunner>();
    private Exception m_fatalException;
    private object m_startDisposeLock = new object();
    private bool m_deploymentSettingsChanged = true;
    private JobApplicationSettings m_jobApplicationSettings;
    private static Timer[] s_disposeTimers;
    private static Timer[] s_teardownTimers;
    public static short s_cpuPercent = 0;
    public static DateTime s_lastUpdate = DateTime.MinValue;
    public const int c_defaultCpuPercentThreshold = 95;
    public const int c_defaultCpuPercentOffset = 10;
    private static readonly string s_userAgent;
    private static readonly string s_jobApplicationArea = "JobAgent";
    private static readonly string s_jobApplicationLayer = "BusinessLogic";

    static JobApplication()
    {
      JobApplication.s_startupTime = DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (string.Equals(Environment.GetEnvironmentVariable("DEPLOYMENT_ENVIRONMENT"), "Vmss", StringComparison.Ordinal))
      {
        string environmentVariable = Environment.GetEnvironmentVariable("TfsTemp");
        if (!string.IsNullOrEmpty(environmentVariable))
        {
          Environment.SetEnvironmentVariable("TEMP", environmentVariable);
          Environment.SetEnvironmentVariable("TMP", environmentVariable);
        }
      }
      string str1 = string.Empty;
      try
      {
        str1 = Path.GetFileName(Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetModuleFileName());
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
      string str2 = string.Empty;
      try
      {
        foreach (object customAttribute in Assembly.GetAssembly(typeof (JobApplication)).GetCustomAttributes(false))
        {
          if (customAttribute is AssemblyFileVersionAttribute)
          {
            str2 = ((AssemblyFileVersionAttribute) customAttribute).Version;
            break;
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
      JobApplication.s_userAgent = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TFS JobAgent({0}, {1})", (object) str1, (object) str2);
    }

    internal static string UserAgent => JobApplication.s_userAgent;

    public JobApplication(object jobAgentSettings)
      : this((IJobAgentSettings) jobAgentSettings)
    {
    }

    internal JobApplication()
    {
    }

    public JobApplication(IJobAgentSettings settings)
    {
      string connectionString = ConnectionStringUtility.DecryptAndNormalizeConnectionString(settings.ConfigDbConnectionString);
      if (string.IsNullOrEmpty(connectionString))
        throw new ApplicationException(FrameworkResources.JobAgentConfigDbSettingInvalid());
      SecureString password = (SecureString) null;
      if (!string.IsNullOrEmpty(settings.ConfigDbPassword))
        password = EncryptionUtility.DecryptSecret(settings.ConfigDbPassword);
      this.m_applicationDbConnectionInfo = SqlConnectionInfoFactory.Create(connectionString, settings.ConfigDbUserId, password);
      this.m_applicationId = settings.InstanceId;
      this.m_maximumStopTime = (int) settings.MaximumStopTime.TotalMilliseconds;
      this.m_maxEventWaitTimeout = (int) settings.ForceQueueCheckInterval.TotalMilliseconds;
      this.m_maxQueueCheckInterval = settings.ForceQueueCheckInterval;
      MutexSecurity mutexSecurity = new MutexSecurity();
      mutexSecurity.AddAccessRule(new MutexAccessRule((IdentityReference) new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, (SecurityIdentifier) null), MutexRights.Modify | MutexRights.Synchronize, AccessControlType.Allow));
      bool createdNew;
      this.m_processLock = new Mutex(false, JobApplication.s_processLockName, out createdNew, mutexSecurity);
      this.ExitProcessOnFatalException = true;
      TfsAssemblyResolver assemblyResolver = new TfsAssemblyResolver();
    }

    public bool ExitProcessOnFatalException { get; set; }

    public void Dispose()
    {
      TeamFoundationTracingService.TraceRaw(1150, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Dispose()");
      lock (this.m_startDisposeLock)
        this.m_disposing = true;
      this.Stop();
      if (this.m_queueChangedEvent != null)
      {
        this.m_queueChangedEvent.Close();
        this.m_queueChangedEvent = (AutoResetEvent) null;
      }
      if (this.m_deploymentHostStoppedEvent != null)
      {
        this.m_deploymentHostStoppedEvent.Close();
        this.m_deploymentHostStoppedEvent = (ManualResetEvent) null;
      }
      if (this.m_processStoppedEvent != null)
      {
        this.m_processStoppedEvent.Close();
        this.m_processStoppedEvent = (ManualResetEvent) null;
      }
      if (this.m_leaseRenewedEvent != null)
      {
        this.m_leaseRenewedEvent.Close();
        this.m_leaseRenewedEvent = (AutoResetEvent) null;
      }
      if (this.m_applicationExtensions != null)
      {
        this.m_applicationExtensions.Dispose();
        this.m_applicationExtensions = (IDisposableReadOnlyList<IJobApplicationExtension>) null;
      }
      if (this.m_deploymentServiceHost != null)
      {
        using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext(true))
        {
          TeamFoundationTracingService.TraceRaw(1151, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Acquiring jobRunnerLock");
          using (systemContext.AcquireWriterLock(this.m_jobRunnerLockName))
          {
            if (this.m_runnerCompletedEvent != null)
            {
              this.m_runnerCompletedEvent.Close();
              this.m_runnerCompletedEvent = (AutoResetEvent) null;
            }
          }
        }
      }
      if (this.m_processLock != null)
      {
        this.m_processLock.Dispose();
        this.m_processLock = (Mutex) null;
      }
      JobApplication.s_disposeTimers = new Timer[2]
      {
        new Timer(new TimerCallback(this.LogAndTerminateProcess), (object) null, TimeSpan.FromSeconds(8.0), TimeSpan.Zero),
        new Timer(new TimerCallback(this.TerminateProcess), (object) null, TimeSpan.FromSeconds(10.0), TimeSpan.Zero)
      };
      this.m_jobApplicationDisposedTime = DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.m_disposed = new DateTime?(DateTime.UtcNow);
      TeamFoundationTracingService.TraceRaw(1152, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Leaving Dispose at {0}", (object) this.m_disposed);
    }

    private int GetCurrentJobRunnerCount(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1301, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (GetCurrentJobRunnerCount));
      try
      {
        using (requestContext.AcquireReaderLock(this.m_jobRunnerLockName))
          return this.m_jobRunners.Count;
      }
      finally
      {
        requestContext.TraceLeave(1302, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (GetCurrentJobRunnerCount));
      }
    }

    private void LogAndTerminateProcess(object state)
    {
      this.m_logAndTerminateTime = DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (this.m_deploymentServiceHost != null)
      {
        using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext(true))
        {
          TeamFoundationTracingService.TraceRaw(1280, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Acquiring m_jobRunnerLock.Reader");
          using (systemContext.AcquireReaderLock(this.m_jobRunnerLockName))
          {
            if (this.m_jobRunners.Count > 0)
            {
              foreach (JobRunner jobRunner in this.m_jobRunners.Values)
                TeamFoundationTracingService.TraceRaw(1281, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "LogAndTerminateProcess: Still running: {0}.", (object) jobRunner);
              TeamFoundationTracingService.TraceRaw(1282, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Throwing JobsStillRunningException");
              JobsStillRunningException runningException = new JobsStillRunningException(this.ProcessIsStopping(), (IEnumerable<JobRunner>) this.m_jobRunners.Values);
              TeamFoundationEventLog.Default.Log(runningException.Message, TeamFoundationEventId.JobsStillRunningError, EventLogEntryType.Error);
              if (!this.m_disposing)
                TeamFoundationEventLog.Default.Log(FrameworkResources.JobAgentTeardownTimedOutError(), TeamFoundationEventId.ApplicationStopped, EventLogEntryType.Error);
              if (runningException.ReportException)
              {
                TeamFoundationTracingService.TraceRaw(1283, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobsStillRunningException has watson reporting set. Throwing exception to produce a watson with a dump.");
                throw runningException;
              }
              TeamFoundationTracingService.TraceRaw(1284, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobsStillRunningException doesn't have watson reporting set. Exiting with code 2 instead of throwing.");
              Environment.Exit(2);
            }
            TeamFoundationTracingService.TraceRaw(1285, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Releasing m_jobRunnerLock.Reader");
          }
        }
      }
      if (this.m_disposing)
      {
        TeamFoundationEventLog.Default.Log(FrameworkResources.ProcessStillRunningError(), TeamFoundationEventId.ProcessStillRunningError, EventLogEntryType.Error);
        TeamFoundationTracingService.TraceRaw(1286, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Throwing TeamFoundationProcessStillRunningException to produce a watson with a dump.");
        throw new TeamFoundationProcessStillRunningException();
      }
      if (JobApplication.s_teardownTimers != null)
      {
        TeamFoundationEventLog.Default.Log(FrameworkResources.JobAgentTeardownTimedOutError(), TeamFoundationEventId.JobAgentTeardownTimedOutError, EventLogEntryType.Error);
        TeamFoundationEventLog.Default.Log(FrameworkResources.JobAgentTeardownTimedOutError(), TeamFoundationEventId.ApplicationStopped, EventLogEntryType.Error);
        TeamFoundationTracingService.TraceRaw(1287, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Throwing JobAgentTeardownTimedOutException to produce a watson with a dump.");
        throw new JobAgentTeardownTimeoutException();
      }
    }

    private void TerminateProcess(object state)
    {
      if (this.m_disposing)
        throw new TeamFoundationProcessStillRunningException();
      if (JobApplication.s_teardownTimers != null)
        throw new JobAgentTeardownTimeoutException();
    }

    public void Start()
    {
      TeamFoundationTracingService.TraceRaw(1153, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Start()");
      lock (this.m_startDisposeLock)
      {
        if (this.m_disposing)
        {
          TeamFoundationTracingService.TraceRaw(1154, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Start was called on a disposed JobApplication");
          throw new ObjectDisposedException(nameof (JobApplication));
        }
        TeamFoundationTracingService.TraceRaw(1155, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Creating Run thread");
        this.m_runThread = new Thread(new ThreadStart(this.Run));
        TeamFoundationTracingService.TraceRaw(1156, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobApplication.Start: Starting Job Queue Processing thread.");
        this.m_runThread.Start();
      }
    }

    public void Stop()
    {
      TeamFoundationTracingService.TraceRaw(1157, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Stop()");
      this.m_processStoppedEvent?.Set();
      Thread runThread = this.m_runThread;
      if (runThread != null)
      {
        TeamFoundationTracingService.TraceRaw(1158, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Joining m_runThread");
        if (runThread.Join(this.m_maximumStopTime - 1000))
          TeamFoundationTracingService.TraceRaw(1159, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobApplication.Stop: Joined job queue processing thread.");
        else
          TeamFoundationTracingService.TraceRaw(1160, TraceLevel.Warning, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobApplication.Stop: Timeout joining job queue processing thread.");
      }
      TeamFoundationTracingService.TraceRaw(1161, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Leaving Stop()");
    }

    private void Run()
    {
      TeamFoundationTracingService.TraceRaw(1162, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Run()");
      try
      {
        try
        {
          Thread.CurrentThread.Name = "Job Queue Processing";
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceRaw(1164, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting name failed with {0}", (object) ex);
        }
        try
        {
          if (Thread.CurrentThread.Priority < ThreadPriority.AboveNormal)
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceRaw(2032100, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting thread priority failed with {0}", (object) ex);
        }
        while (!this.ProcessIsStopping())
        {
          this.Setup();
          this.ProcessJobQueue();
          this.Teardown();
        }
        TeamFoundationTracingService.TraceRaw(1163, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Leaving Job Queue Processing's Run loop.");
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(1164, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Caught {0}", (object) ex);
        TeamFoundationEventLog.Default.LogException(FrameworkResources.JobAgentStoppingDueToUnhandledException(), ex);
        Environment.Exit(100);
      }
      finally
      {
        TeamFoundationTracingService.TraceRaw(1162, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Leaving Run()");
      }
    }

    private void Setup()
    {
      TeamFoundationTracingService.TraceRaw(1163, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Starting JobAgent Setup().");
      TeamFoundationEventLog.Default.Log(FrameworkResources.JobAgentEnteringLifecycleStage((object) nameof (Setup)), TeamFoundationEventId.JobAgentLifecycleChange, EventLogEntryType.Information);
      JobServiceUtil.RetryOperationsUntilSuccessful(new RetryOperations(this.SetupInternal), ref this.m_retryDelaySeconds);
      TeamFoundationTracingService.TraceRaw(1165, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Leaving Setup()");
    }

    private void ProcessJobQueue()
    {
      TeamFoundationTracingService.TraceRaw(1166, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "ProcessJobQueue().");
      TeamFoundationEventLog.Default.Log(FrameworkResources.JobAgentEnteringLifecycleStage((object) nameof (ProcessJobQueue)), TeamFoundationEventId.JobAgentLifecycleChange, EventLogEntryType.Information);
      JobServiceUtil.RetryOperationsUntilSuccessful(new RetryOperations(this.ProcessJobQueueInternal), ref this.m_retryDelaySeconds);
      TeamFoundationTracingService.TraceRaw(1167, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Leaving ProcessJobQueue().");
    }

    private void Teardown()
    {
      TeamFoundationTracingService.TraceRaw(1168, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Starting JobAgent Teardown().");
      TeamFoundationEventLog.Default.Log(FrameworkResources.JobAgentEnteringLifecycleStage((object) nameof (Teardown)), TeamFoundationEventId.JobAgentLifecycleChange, EventLogEntryType.Information);
      this.m_teardownBeginTime = DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      try
      {
        if (!this.m_disposing)
          JobApplication.s_teardownTimers = new Timer[2]
          {
            new Timer(new TimerCallback(this.LogAndTerminateProcess), (object) null, TimeSpan.FromSeconds(110.0), TimeSpan.Zero),
            new Timer(new TimerCallback(this.TerminateProcess), (object) null, TimeSpan.FromSeconds(120.0), TimeSpan.Zero)
          };
        try
        {
          if (this.m_deploymentServiceHost != null)
          {
            using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext(false))
            {
              ArgumentUtility.CheckForNull<IVssRequestContext>(systemContext, "m_requestContext");
              this.TeardownApplicationExtensions(systemContext);
              if (this.m_taskService != null)
              {
                if (this.m_unregisterRescheduleTask != null)
                {
                  try
                  {
                    systemContext.Trace(1169, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Removing unregisterLocalInactiveProcesses Task");
                    this.m_taskService.RemoveTask(systemContext, this.m_unregisterRescheduleTask);
                  }
                  catch (Exception ex)
                  {
                    systemContext.Trace(1170, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Caught Exception {0} while removing task", (object) ex);
                  }
                }
              }
              this.m_taskService = (TeamFoundationTaskService) null;
              if (this.m_sqlNotificationService != null)
              {
                try
                {
                  this.m_deploymentServiceHost.StatusChanged -= new EventHandler<HostStatusChangedEventArgs>(this.OnServiceHostChanged);
                  systemContext.Trace(1171, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Unregistering Notification for OnQueueChanged");
                  this.m_sqlNotificationService.UnregisterNotification(systemContext, "Default", SqlNotificationEventClasses.JobQueueChanged, new SqlNotificationCallback(this.OnQueueChanged), false);
                  systemContext.Trace(1172, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Unregistering Notification for OnRecycle");
                  this.m_sqlNotificationService.UnregisterNotification(systemContext, "Default", SqlNotificationEventClasses.Recycle, new SqlNotificationCallback(this.OnRecycle), false);
                  systemContext.Trace(1256, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Unregistering Notification for OnSettingsChanged");
                  systemContext.GetService<CachedRegistryService>().UnregisterNotification(systemContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));
                }
                catch (Exception ex)
                {
                  systemContext.Trace(1172, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Caught {0} while unregistering Notification for OnQueueChanged", (object) ex);
                }
                this.m_sqlNotificationService = (ITeamFoundationSqlNotificationService) null;
              }
              if (this.m_hostManagementService != null)
              {
                try
                {
                  systemContext.Trace(1293, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Removing LeaseRenewed callback.");
                  this.m_hostManagementService.LeaseRenewed -= new EventHandler(this.OnLeaseRenewed);
                }
                catch (Exception ex)
                {
                  systemContext.TraceException(1294, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, ex);
                }
                this.m_hostManagementService = (TeamFoundationHostManagementService) null;
              }
            }
          }
          else
            TeamFoundationTracingService.TraceRaw(1253, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "deploymentHost is null at beginning of Teardown.");
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceRaw(1254, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Caught {0} while creating system request context during Teardown", (object) ex);
        }
        try
        {
          if (this.m_deploymentServiceHost == null)
            return;
          TeamFoundationTracingService.TraceRaw(1173, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Stopping All Jobs");
          this.StopAllJobs();
          this.m_jobService = (TeamFoundationJobService) null;
          if (this.m_fatalException != null)
          {
            TeamFoundationTracingService.TraceRaw(1174, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Throwing {0}", (object) this.m_fatalException);
            throw this.m_fatalException;
          }
          this.m_disposeDeploymentServiceHostTime = DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          try
          {
            TeamFoundationTracingService.TraceRaw(1175, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobAgent Teardown: Disposing of DeploymentServiceHost.");
            this.m_deploymentServiceHost.Dispose();
            TeamFoundationTracingService.TraceRaw(1176, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobAgent Teardown: Successfully disposed of DeploymentServiceHost.");
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceRaw(1177, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobAgent Teardown: Exception while disposing of DeploymentServiceHost.", (object) ex);
            throw;
          }
          this.m_deploymentServiceHost = (DeploymentServiceHost) null;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceRaw(1178, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Caught {0}", (object) ex);
          ExceptionHandlerUtility.HandleException(ex);
          TeamFoundationTracingService.TraceRaw(1179, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Shutting down on {0}", (object) ex);
          this.Shutdown(ex);
        }
      }
      finally
      {
        if (JobApplication.s_teardownTimers != null)
        {
          foreach (Timer teardownTimer in JobApplication.s_teardownTimers)
            teardownTimer.Dispose();
          JobApplication.s_teardownTimers = (Timer[]) null;
        }
      }
    }

    private void SetupInternal()
    {
      try
      {
        TeamFoundationTracingService.TraceEnterRaw(1303, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (SetupInternal), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
        if (this.ProcessIsStopping())
        {
          TeamFoundationTracingService.TraceRaw(1181, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "ProcessIsStopping - leaving SetupInternal()");
        }
        else
        {
          this.m_teardownBeginTime = (string) null;
          this.m_stopAllJobsBeginTime = (string) null;
          this.m_disposeDeploymentServiceHostTime = (string) null;
          TeamFoundationTracingService.TraceRaw(1182, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting m_queueChangedEvent");
          this.m_queueChangedEvent.Set();
          TeamFoundationTracingService.TraceRaw(1183, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting m_deploymentHostStoppedEvent");
          this.m_deploymentHostStoppedEvent.Reset();
          if (this.m_applicationExtensions != null)
          {
            try
            {
              if (this.m_deploymentServiceHost != null)
              {
                using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext(false))
                  this.TeardownApplicationExtensions(systemContext);
              }
            }
            catch (AggregateException ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(1107, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, (Exception) ex);
            }
            finally
            {
              this.m_applicationExtensions = (IDisposableReadOnlyList<IJobApplicationExtension>) null;
            }
          }
          if (this.m_hostManagementService != null)
          {
            try
            {
              this.m_hostManagementService.LeaseRenewed -= new EventHandler(this.OnLeaseRenewed);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(1106, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, ex);
            }
            this.m_hostManagementService = (TeamFoundationHostManagementService) null;
          }
          if (this.m_deploymentServiceHost != null)
          {
            try
            {
              this.m_deploymentServiceHost.StatusChanged -= new EventHandler<HostStatusChangedEventArgs>(this.OnServiceHostChanged);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(1105, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, ex);
            }
            this.m_deploymentServiceHost.Dispose();
            this.m_deploymentServiceHost = (DeploymentServiceHost) null;
          }
          this.m_jobRunners.Clear();
          int num = DatabaseManagementConstants.InvalidDatabaseId;
          using (DatabaseManagementComponent componentRaw = this.m_applicationDbConnectionInfo.CreateComponentRaw<DatabaseManagementComponent>())
          {
            using (ResultCollection resultCollection = componentRaw.QueryDatabases())
            {
              foreach (InternalDatabaseProperties databaseProperties in resultCollection.GetCurrent<InternalDatabaseProperties>().Items)
              {
                if (string.Equals(this.m_applicationDbConnectionInfo.ConnectionString, databaseProperties.ConnectionInfoWrapper.ConnectionString, StringComparison.OrdinalIgnoreCase))
                  num = databaseProperties.DatabaseId;
              }
            }
          }
          TeamFoundationServiceHostProperties serviceHostProperties = new TeamFoundationServiceHostProperties();
          serviceHostProperties.HostType = TeamFoundationHostType.Application | TeamFoundationHostType.Deployment;
          serviceHostProperties.Id = this.m_applicationId;
          serviceHostProperties.PlugInDirectory = TFCommonUtil.CombinePaths(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
          serviceHostProperties.PhysicalDirectory = AppDomain.CurrentDomain.BaseDirectory;
          serviceHostProperties.DatabaseId = num;
          VssExtensionManagementService.DefaultPluginPath = serviceHostProperties.PlugInDirectory;
          TeamFoundationTracingService.TraceRaw(1184, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Creating new DeploymentServiceHost Instance with {0}", (object) serviceHostProperties);
          this.m_deploymentServiceHost = new DeploymentServiceHost((HostProperties) serviceHostProperties, this.m_applicationDbConnectionInfo, new DeploymentServiceHostOptions(HostProcessType.JobAgent));
          this.m_deploymentServiceHostCreatedTime = DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          ++JobApplication.s_deploymentHostsCreated;
          try
          {
            using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext(true))
            {
              this.m_jobRunnerLockName = systemContext.ServiceHost.CreateLockName(string.Format("{0}/jobRunner", (object) this.GetType().FullName));
              this.m_jobService = systemContext.GetService<TeamFoundationJobService>();
              this.m_hostManagementService = systemContext.GetService<TeamFoundationHostManagementService>();
              this.m_agentId = this.m_hostManagementService.ProcessId;
              systemContext.Trace(1100, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Detecting Inactive Processes");
              this.m_hostManagementService.DetectInactiveProcesses(systemContext);
              systemContext.Trace(1101, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Registering Notification for OnQueueChanged");
              this.m_sqlNotificationService = (ITeamFoundationSqlNotificationService) systemContext.GetService<TeamFoundationSqlNotificationService>();
              this.m_sqlNotificationService.RegisterNotification(systemContext, "Default", SqlNotificationEventClasses.JobQueueChanged, new SqlNotificationCallback(this.OnQueueChanged), false);
              this.m_sqlNotificationService.RegisterNotification(systemContext, "Default", SqlNotificationEventClasses.Recycle, new SqlNotificationCallback(this.OnRecycle), false);
              systemContext.GetService<IHealthAgentService>();
              this.m_deploymentServiceHost.StatusChanged += new EventHandler<HostStatusChangedEventArgs>(this.OnServiceHostChanged);
              systemContext.GetService<CachedRegistryService>().RegisterNotification(systemContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), RegistryQueryConstants.JobServiceSettings, "/Diagnostics/Hosting/...");
              this.UpdateSettings(systemContext);
              this.m_deploymentWasActive = this.IsDeploymentActive(systemContext);
              systemContext.Trace(1108, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Adding UnregisterRescheduleTask Task");
              TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.UnregisterRescheduleTask), (object) null, 60000);
              this.m_taskService = systemContext.GetService<TeamFoundationTaskService>();
              this.m_taskService.AddTask(systemContext, task);
              this.m_unregisterRescheduleTask = task;
              systemContext.Trace(1104, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Loading plugins for ITeamFoundationJobExtension in {0}", (object) this.m_deploymentServiceHost.PlugInDirectory);
              this.m_plugins = VssExtensionManagementService.GetTypeMapRaw<ITeamFoundationJobExtension>(this.m_deploymentServiceHost.PlugInDirectory);
              WebApiConfiguration.Initialize(systemContext);
              this.SetupApplicationExtensions(systemContext);
              this.m_hostManagementService.LeaseRenewed += new EventHandler(this.OnLeaseRenewed);
              systemContext.Trace(1109, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Leaving SetupInternal()");
            }
          }
          catch (HostShutdownException ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(1288, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, (Exception) ex);
            TeamFoundationTracingService.TraceRaw(1289, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Determining whether to unregister local inactive processes in response to host shutdown.");
            if (DateTime.UtcNow > this.m_lastInactiveProcessCheckTime.AddSeconds(60.0))
            {
              this.m_lastInactiveProcessCheckTime = DateTime.UtcNow;
              using (IVssRequestContext servicingContext = this.m_deploymentServiceHost.CreateServicingContext())
              {
                try
                {
                  servicingContext.GetService<CachedRegistryService>();
                  if (this.m_jobApplicationSettings.UnregisterLocalInnactiveProcesses)
                  {
                    TeamFoundationTracingService.TraceRaw(1290, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Unregistering local inactive processes outside of task service..");
                    this.UnregisterRescheduleTask(servicingContext, (object) null);
                  }
                  else
                    TeamFoundationTracingService.TraceRaw(1291, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Skipping unregistering of local inactive processes. Disabled in registry.");
                }
                catch
                {
                  TeamFoundationTracingService.TraceExceptionRaw(1292, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, (Exception) ex);
                }
              }
            }
            else
              TeamFoundationTracingService.TraceRaw(1293, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "UnregisterLocalInactiveProcesses was attempted within the last 60 seconds.");
            throw;
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1180, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(1304, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (SetupInternal));
      }
    }

    private void UpdateSettings(IVssRequestContext requestContext)
    {
      this.m_jobApplicationSettings = requestContext.GetService<IJobApplicationSettingsService>().GetJobApplicationSettings();
      this.m_retryDelaySeconds = this.m_jobApplicationSettings.RetryDelaySeconds;
    }

    private void OnLeaseRenewed(object sender, EventArgs e) => this.m_leaseRenewedEvent.Set();

    private void UnregisterRescheduleTask(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(1307, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (UnregisterRescheduleTask));
      try
      {
        bool flag1 = this.m_leaseRenewedEvent.WaitOne(0, false);
        if (!flag1 && !this.m_jobApplicationSettings.UnregisterLocalInnactiveProcesses)
        {
          requestContext.Trace(1119, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "UnregisterRescheduleTask returning - nothing to do.");
        }
        else
        {
          bool flag2 = false;
          if (this.m_jobApplicationSettings.UnregisterLocalInnactiveProcesses)
          {
            flag2 = requestContext.GetService<TeamFoundationHostManagementService>().DetectAndUnregisterProcesses(requestContext);
            if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              requestContext.GetService<TeamFoundationServicingService>().CheckServicingJobs(requestContext);
          }
          if (!(flag1 | flag2))
            return;
          requestContext.GetService<TeamFoundationJobService>().RescheduleInactiveJobs(requestContext);
        }
      }
      finally
      {
        requestContext.TraceLeave(1308, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (UnregisterRescheduleTask));
      }
    }

    private void ProcessJobQueueInternal()
    {
      TeamFoundationTracingService.TraceRaw(1184, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Starting JobAgent ProcessJobQueueInternal().");
      if (this.ApplicationHostOrProcessIsStopping())
      {
        TeamFoundationTracingService.TraceRaw(1185, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "ApplicationHostOrProcessIsStopping - leaving ProcessJobQueueInternal().");
      }
      else
      {
        TeamFoundationTracingService.TraceRaw(1186, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting m_queueChangedEvent");
        this.m_queueChangedEvent.Set();
label_3:
        using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext(true))
        {
          try
          {
            if (this.ProcessJobQueueOnce(systemContext))
              goto label_3;
          }
          catch (Exception ex) when (systemContext.ExecutionEnvironment.IsHostedDeployment)
          {
            TeamFoundationEventLog.Default.LogException(systemContext, "Error procesing job queue", ex, TeamFoundationEventId.ProcessJobQueueError, EventLogEntryType.Error);
            throw;
          }
        }
      }
    }

    private bool ProcessJobQueueOnce(IVssRequestContext systemRequestContext)
    {
      int num1 = this.m_maxEventWaitTimeout;
      int currentJobRunnerCount1 = this.GetCurrentJobRunnerCount(systemRequestContext);
      bool flag1 = currentJobRunnerCount1 < this.m_jobApplicationSettings.MaxJobRunners;
      systemRequestContext.Trace(1187, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobRunnerCount {0}, jobThreadsAvailable {1}", (object) currentJobRunnerCount1, (object) flag1);
      bool flag2 = this.IsDeploymentActive(systemRequestContext);
      if (flag1 & flag2 && this.m_nextQueueEntryTime != DateTime.MaxValue)
      {
        double val1 = (this.m_nextQueueEntryTime - DateTime.UtcNow).TotalMilliseconds;
        if (val1 < 0.0)
          val1 = 500.0;
        num1 = (int) Math.Min(val1, (double) num1);
      }
      systemRequestContext.Trace(1188, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "[ProcessJobQueue] Waiting for {0} milliseconds. JobRunners: {1}/{2}. NextQueueEntryTime: {3}.", (object) num1, (object) currentJobRunnerCount1, (object) this.m_jobApplicationSettings.MaxJobRunners, (object) this.m_nextQueueEntryTime);
      Stopwatch stopwatch = Stopwatch.StartNew();
      int num2 = WaitHandle.WaitAny(new WaitHandle[4]
      {
        (WaitHandle) this.m_processStoppedEvent,
        (WaitHandle) this.m_deploymentHostStoppedEvent,
        (WaitHandle) this.m_runnerCompletedEvent,
        (WaitHandle) this.m_queueChangedEvent
      }, num1, false);
      stopwatch.Stop();
      systemRequestContext.Trace(1189, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "[ProcessJobQueue] Done waiting. WaitEvent={0}, ElapsedTime={1:0,0}/{2:0,0} ms.", (object) num2, (object) stopwatch.Elapsed.TotalMilliseconds, (object) num1);
      systemRequestContext.RequestTimer.RequestTimerInternal().AddTimeSpentDelayed(stopwatch.ElapsedTicks);
      if (num2 == 0 || num2 == 1)
        return false;
      this.UpdateSettings(systemRequestContext);
      if (this.m_deploymentSettingsChanged)
      {
        this.m_deploymentSettingsChanged = false;
        systemRequestContext.Trace(1267, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Reloading deployment settings.");
        this.ConsumeDeploymentState(systemRequestContext);
      }
      systemRequestContext.Trace(1190, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Releasing Outstanding Jobs");
      int num3 = this.ConsumeNewlyCompletedJobs(systemRequestContext);
      this.m_checkQueue = this.m_checkQueue || num3 > 0 || num2 == 3 || this.m_queueChangedEvent.WaitOne(0, false) || DateTime.UtcNow >= this.m_nextQueueEntryTime || DateTime.UtcNow - this.m_lastQueueCheckTime >= this.m_maxQueueCheckInterval || this.m_lastQueueCheckTime > DateTime.UtcNow;
      if (this.m_checkQueue)
      {
        if (num3 > 0)
        {
          this.ReleaseAndAcquireJobs(systemRequestContext);
          this.m_checkQueue = false;
        }
        else
        {
          int currentJobRunnerCount2 = this.GetCurrentJobRunnerCount(systemRequestContext);
          bool flag3 = currentJobRunnerCount2 < this.m_jobApplicationSettings.MaxJobRunners;
          systemRequestContext.Trace(1191, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "[ProcessJobQueue] CheckQueue: True. JobRunners: {0}/{1}.", (object) currentJobRunnerCount2, (object) this.m_jobApplicationSettings.MaxJobRunners);
          if (flag3)
          {
            this.ReleaseAndAcquireJobs(systemRequestContext);
            this.m_checkQueue = false;
          }
        }
      }
      else
        systemRequestContext.Trace(1192, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "[ProcessJobQueue] CheckQueue: False.");
      if (systemRequestContext.ExecutionTime() > (long) (this.m_jobApplicationSettings.SlowQueueThresholdMilliseconds * 1000))
      {
        DiagnosticDumper diagnosticDumper = systemRequestContext.RequestTimer.RequestTimerInternal().GetDiagnosticDumper((long) this.m_jobApplicationSettings.SlowQueueThresholdMilliseconds);
        systemRequestContext.Trace(1194, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, (string[]) null, "{0}", (object) diagnosticDumper);
      }
      return true;
    }

    private bool IsDeploymentActive(IVssRequestContext systemRequestContext)
    {
      try
      {
        if (this.m_processLock.WaitOne(0))
        {
          this.m_processLock.ReleaseMutex();
        }
        else
        {
          systemRequestContext.Trace(1193, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Someone holds mutex {0}, will not acquire jobs", (object) JobApplication.s_processLockName);
          return false;
        }
      }
      catch (AbandonedMutexException ex)
      {
        this.m_processLock.ReleaseMutex();
      }
      return !systemRequestContext.ExecutionEnvironment.IsCloudDeployment || systemRequestContext.GetService<IHostedTenantService>().ShouldJobAgentAquireJobs(systemRequestContext);
    }

    private void ConsumeDeploymentState(IVssRequestContext requestContext)
    {
      int num = this.IsDeploymentActive(requestContext) ? 1 : 0;
      if (num != 0)
      {
        this.m_deploymentWasActive = true;
        if (this.m_applicationExtensions == null)
          this.SetupApplicationExtensions(requestContext);
      }
      if (num != 0 || !this.m_deploymentWasActive || this.m_applicationExtensions == null)
        return;
      this.TeardownApplicationExtensions(requestContext);
      this.m_deploymentWasActive = false;
    }

    private void SetupApplicationExtensions(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1311, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (SetupApplicationExtensions));
      try
      {
        requestContext.Trace(1120, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Loading plugins for IJobApplicationExtension in {0}", (object) this.m_deploymentServiceHost.PlugInDirectory);
        if (this.m_applicationExtensions != null)
          return;
        List<Exception> innerExceptions = (List<Exception>) null;
        this.m_applicationExtensions = requestContext.GetExtensions<IJobApplicationExtension>();
        if (this.m_applicationExtensions != null)
        {
          foreach (IJobApplicationExtension applicationExtension in (IEnumerable<IJobApplicationExtension>) this.m_applicationExtensions)
          {
            requestContext.Trace(1121, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Initializing extension {0}", (object) applicationExtension);
            try
            {
              applicationExtension.Start(requestContext);
            }
            catch (Exception ex)
            {
              if (innerExceptions == null)
                innerExceptions = new List<Exception>();
              requestContext.TraceException(1123, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, ex);
              innerExceptions.Add(ex);
            }
          }
        }
        if (innerExceptions != null)
          throw new AggregateException((IEnumerable<Exception>) innerExceptions);
      }
      finally
      {
        requestContext.TraceLeave(1312, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (SetupApplicationExtensions));
      }
    }

    private void TeardownApplicationExtensions(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1313, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (TeardownApplicationExtensions));
      try
      {
        requestContext.Trace(1120, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Disposing plugins for IJobApplicationExtension");
        if (this.m_applicationExtensions == null)
          return;
        List<Exception> innerExceptions = new List<Exception>();
        try
        {
          foreach (IJobApplicationExtension applicationExtension in (IEnumerable<IJobApplicationExtension>) this.m_applicationExtensions)
          {
            requestContext.Trace(1121, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Disposing extension {0}", (object) applicationExtension);
            try
            {
              applicationExtension.Stop(requestContext);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(1122, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, ex);
              innerExceptions.Add(ex);
            }
          }
          this.m_applicationExtensions.Dispose();
        }
        finally
        {
          this.m_applicationExtensions = (IDisposableReadOnlyList<IJobApplicationExtension>) null;
        }
        if (innerExceptions.Count > 0)
          throw new AggregateException((IEnumerable<Exception>) innerExceptions);
      }
      finally
      {
        requestContext.TraceLeave(1314, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (TeardownApplicationExtensions));
      }
    }

    private void ReleaseAndAcquireJobs(IVssRequestContext deploymentContext)
    {
      deploymentContext.TraceEnter(1315, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (ReleaseAndAcquireJobs));
      try
      {
        SqlStatistics sqlStatistics = (SqlStatistics) null;
        int maxJobsToAcquire = -1;
        int capacity = -1;
        try
        {
          if (deploymentContext.IsTracing(1277, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer))
            sqlStatistics = new SqlStatistics();
          int nextScheduledJob = -1;
          TeamFoundationHostManagementService service = deploymentContext.GetService<TeamFoundationHostManagementService>();
          if (this.ApplicationHostOrProcessIsStopping())
          {
            deploymentContext.Trace(1199, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "ApplicationHostOrProcessIsStopping - leaving ReleaseAndAcquireJobs()");
          }
          else
          {
            this.ReleaseOutstandingJobs(deploymentContext, false, false);
            maxJobsToAcquire = this.m_jobApplicationSettings.MaxJobRunners - this.GetCurrentJobRunnerCount(deploymentContext);
            bool flag1 = this.IsDeploymentActive(deploymentContext);
            bool flag2 = this.Throttle(deploymentContext);
            if (maxJobsToAcquire > 0 & flag1 && !flag2)
            {
              deploymentContext.Trace(1196, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Acquiring up to {0} jobs.", (object) maxJobsToAcquire);
              List<TeamFoundationJobQueueEntry> queueEntries;
              try
              {
                List<TeamFoundationJobQueueEntry> allAcquiredQueueEntries = this.m_jobService.AcquireJobs(deploymentContext, this.m_agentId, maxJobsToAcquire, out nextScheduledJob);
                queueEntries = this.FilterInProgressJobs(deploymentContext, allAcquiredQueueEntries);
              }
              catch (Exception ex1)
              {
                deploymentContext.TraceException(1203, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, ex1);
                while (!this.ApplicationHostOrProcessIsStopping())
                {
                  try
                  {
                    List<TeamFoundationJobQueueEntry> allAcquiredQueueEntries = this.m_jobService.QueryAcquiredJobs(deploymentContext, this.m_agentId);
                    queueEntries = this.FilterInProgressJobs(deploymentContext, allAcquiredQueueEntries);
                    goto label_14;
                  }
                  catch (Exception ex2)
                  {
                    deploymentContext.TraceException(1204, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, ex2);
                    Thread.Sleep(this.m_jobApplicationSettings.RetryDelaySeconds * 1000);
                  }
                }
                deploymentContext.Trace(1198, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "ApplicationHostOrProcessIsStopping - stopping QueryAcquiredJobs()");
                return;
              }
label_14:
              capacity = queueEntries.Count;
              deploymentContext.Trace(1197, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Acquired {0}/{1} jobs.", (object) capacity, (object) maxJobsToAcquire);
              this.m_lastQueueCheckTime = DateTime.UtcNow;
              this.m_nextQueueEntryTime = DateTime.MaxValue;
              if (capacity > 0)
              {
                JobDefinitionResolver jobDefinitionResolver = new JobDefinitionResolver(deploymentContext, queueEntries);
                List<JobRunner> jobRunnerList = new List<JobRunner>(capacity);
                foreach (TeamFoundationJobQueueEntry acquiredJob in queueEntries)
                {
                  JobRunner jobRunner = new JobRunner((IVssDeploymentServiceHost) this.m_deploymentServiceHost, this.m_jobService, this.m_jobApplicationSettings.MaxJobResultMessageLength, acquiredJob, (IJobDefinitionResolver) jobDefinitionResolver, (IReadOnlyDictionary<string, Type>) this.m_plugins);
                  jobRunnerList.Add(jobRunner);
                }
                JobRunner preExistingJobRunner = (JobRunner) null;
                TeamFoundationJobQueueEntry queueEntry = (TeamFoundationJobQueueEntry) null;
                bool flag3 = false;
                using (deploymentContext.AcquireWriterLock(this.m_jobRunnerLockName))
                {
                  try
                  {
                    if (this.ApplicationHostOrProcessIsStopping())
                    {
                      deploymentContext.Trace(1199, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "ApplicationHostOrProcessIsStopping - leaving ReleaseAndAcquireJobs()");
                      return;
                    }
                    foreach (JobRunner jobRunner in jobRunnerList)
                    {
                      deploymentContext.Trace(1200, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobQueueEntry is {0} ", (object) jobRunner.QueueEntry);
                      if (this.m_jobRunners.TryGetValue(jobRunner.QueueEntry, out preExistingJobRunner))
                      {
                        queueEntry = jobRunner.QueueEntry;
                        break;
                      }
                      this.m_jobRunners.Add(jobRunner.QueueEntry, jobRunner);
                      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentRunningJobs").Increment();
                      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalRunningJobs").Increment();
                      try
                      {
                        jobRunner.StartExecution(new EventHandler<EventArgs>(this.OnRunnerCompleted));
                      }
                      catch (Exception ex)
                      {
                        this.m_jobRunners.Remove(jobRunner.QueueEntry);
                        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentRunningJobs").Decrement();
                        throw;
                      }
                    }
                    if (queueEntry == null)
                      flag3 = true;
                  }
                  finally
                  {
                    if (!flag3)
                    {
                      foreach (JobRunner jobRunner in jobRunnerList)
                      {
                        if (!jobRunner.ExecutionStarted)
                          this.m_jobsToRelease.Add(jobRunner);
                      }
                    }
                    deploymentContext.Trace(1212, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Releasing JobRunner Lock");
                  }
                }
                if (queueEntry != null)
                {
                  this.HandleJobAlreadyInProgress(deploymentContext, service, preExistingJobRunner, queueEntry);
                  return;
                }
              }
              if (nextScheduledJob >= 0)
                this.m_nextQueueEntryTime = this.m_lastQueueCheckTime.AddSeconds((double) nextScheduledJob + 0.5);
              deploymentContext.Trace(1213, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "[ReleaseAndAcquireJobs] Jobs acquired: {0}/{1}. Next scheduled job: {2} seconds. NextQueueEntryTime: {3}. LastQueueCheckTime: {4}.", (object) capacity, (object) maxJobsToAcquire, (object) nextScheduledJob, (object) this.m_nextQueueEntryTime, (object) this.m_lastQueueCheckTime);
            }
            else
            {
              if (DateTime.UtcNow > this.m_nextMinDisabledTraceTime)
              {
                this.m_nextMinDisabledTraceTime = DateTime.UtcNow.AddSeconds(30.0);
                deploymentContext.TraceAlways(1214, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Not attempting to acquire jobs, at least one of the following is true or there are no threads available: Job threads available: {0}, Deployment Disabled: {1}, Throttling on CPU: {2}", (object) (maxJobsToAcquire <= 0), (object) !flag1, (object) flag2);
              }
              this.m_nextQueueEntryTime = DateTime.MaxValue;
            }
          }
        }
        catch (Exception ex)
        {
          deploymentContext.TraceException(2032101, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, ex);
          throw;
        }
        finally
        {
          if (sqlStatistics != null)
          {
            string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[ReleaseAndAcquireJobs] Queued jobs acquired: {0}/{1}. stats: {2}", (object) capacity, (object) maxJobsToAcquire, (object) sqlStatistics);
            deploymentContext.Trace(1277, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, message);
            sqlStatistics.Dispose();
          }
        }
      }
      finally
      {
        deploymentContext.TraceLeave(1316, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (ReleaseAndAcquireJobs));
      }
    }

    internal bool Throttle(IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled(FrameworkServerConstants.JACpuThrottlingEnabledFeatureFlag) && DateTime.UtcNow - JobApplication.s_lastUpdate < TimeSpan.FromSeconds(15.0))
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        short num1 = service.GetValue<short>(requestContext, (RegistryQuery) FrameworkServerConstants.RequestThrottlingJACpuThreshold, (short) 95);
        short num2 = service.GetValue<short>(requestContext, (RegistryQuery) FrameworkServerConstants.RequestThrottlingJACpuOffset, (short) 10);
        DateTime utcNow;
        if ((int) JobApplication.s_cpuPercent >= (int) num1 - (int) num2 && DateTime.UtcNow > this.m_nextMinThrottlingTraceTimeWithOffset)
        {
          utcNow = DateTime.UtcNow;
          this.m_nextMinThrottlingTraceTimeWithOffset = utcNow.AddSeconds(30.0);
          string format = "Job Agent with total percent CPU of " + JobApplication.s_cpuPercent.ToString() + " is less than " + num2.ToString() + " percent away from having its Job Acquisition throttled. The throttling threshold is " + num1.ToString() + " percent";
          requestContext.TraceAlways(98103, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, format);
        }
        if ((int) JobApplication.s_cpuPercent >= (int) num1)
        {
          if (DateTime.UtcNow > this.m_nextMinThrottlingTraceTime)
          {
            utcNow = DateTime.UtcNow;
            this.m_nextMinThrottlingTraceTime = utcNow.AddSeconds(30.0);
            string format = "Throttling Job Agent Job Acquisition.  JA CPU Percent = " + JobApplication.s_cpuPercent.ToString() + " percent. Threshold = " + num1.ToString();
            requestContext.TraceAlways(98101, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, format);
          }
          return true;
        }
      }
      return false;
    }

    private List<TeamFoundationJobQueueEntry> FilterInProgressJobs(
      IVssRequestContext requestContext,
      List<TeamFoundationJobQueueEntry> allAcquiredQueueEntries)
    {
      requestContext.TraceEnter(1317, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (FilterInProgressJobs));
      try
      {
        if (allAcquiredQueueEntries.Count == 0)
          return allAcquiredQueueEntries;
        HashSet<TeamFoundationJobQueueEntry> jobsInProgress = new HashSet<TeamFoundationJobQueueEntry>((IEqualityComparer<TeamFoundationJobQueueEntry>) JobApplication.JobComparer.Instance);
        using (requestContext.AcquireReaderLock(this.m_jobRunnerLockName))
          jobsInProgress.UnionWith(this.m_jobRunners.Values.Concat<JobRunner>((IEnumerable<JobRunner>) this.m_newlyCompletedJobs).Concat<JobRunner>((IEnumerable<JobRunner>) this.m_jobsToRelease).Select<JobRunner, TeamFoundationJobQueueEntry>((Func<JobRunner, TeamFoundationJobQueueEntry>) (x => x.QueueEntry)));
        List<TeamFoundationJobQueueEntry> foundationJobQueueEntryList = new List<TeamFoundationJobQueueEntry>(allAcquiredQueueEntries.Count);
        foundationJobQueueEntryList.AddRange(allAcquiredQueueEntries.Where<TeamFoundationJobQueueEntry>((Func<TeamFoundationJobQueueEntry, bool>) (x => !jobsInProgress.Contains(x))));
        return foundationJobQueueEntryList;
      }
      finally
      {
        requestContext.TraceLeave(1318, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (FilterInProgressJobs));
      }
    }

    private void HandleJobAlreadyInProgress(
      IVssRequestContext deploymentContext,
      TeamFoundationHostManagementService hostManagementService,
      JobRunner preExistingJobRunner,
      TeamFoundationJobQueueEntry queueEntry)
    {
      if (this.ApplicationHostOrProcessIsStopping())
      {
        deploymentContext.Trace(1268, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Jobs were released and queued again before we could react to losing lease.");
      }
      else
      {
        deploymentContext.Trace(1201, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "We're in a bad state - build a watson exception and proceed to TearDown.");
        string machineName = (string) null;
        Guid currentAgentId = Guid.Empty;
        List<TeamFoundationServiceHostProcess> processes = (List<TeamFoundationServiceHostProcess>) null;
        TeamFoundationJobHistoryEntry lastHistoryEntry = (TeamFoundationJobHistoryEntry) null;
        try
        {
          machineName = Environment.MachineName;
          currentAgentId = hostManagementService.ProcessId;
          processes = hostManagementService.QueryServiceHostProcesses(deploymentContext, Guid.Empty);
          List<TeamFoundationJobHistoryEntry> foundationJobHistoryEntryList = this.m_jobService.QueryLatestJobHistory(deploymentContext, queueEntry.JobSource, (IEnumerable<Guid>) new Guid[1]
          {
            queueEntry.JobId
          });
          if (foundationJobHistoryEntryList != null)
          {
            if (foundationJobHistoryEntryList.Count == 1)
              lastHistoryEntry = foundationJobHistoryEntryList[0];
          }
        }
        catch (Exception ex)
        {
          deploymentContext.TraceException(1270, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, ex);
        }
        using (deploymentContext.AcquireWriterLock(this.m_jobRunnerLockName))
        {
          this.m_fatalException = (Exception) new ExecutingUnassignedJobException(preExistingJobRunner, queueEntry, lastHistoryEntry, JobApplication.s_startupTime, this.m_deploymentServiceHostCreatedTime, JobApplication.s_deploymentHostsCreated, this.m_agentId, currentAgentId, machineName, processes);
          deploymentContext.Trace(1276, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Proceeding to shutdown: {0}", (object) this.m_fatalException);
          deploymentContext.Trace(1202, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting m_processStoppedEvent.");
          this.m_processStoppedEvent.Set();
        }
      }
    }

    private void ReleaseOutstandingJobs(
      IVssRequestContext requestContext,
      bool consumeNewlyCompletedJobs,
      bool catchAndLogJobServiceExceptions)
    {
      requestContext.TraceEnter(1321, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (ReleaseOutstandingJobs));
      try
      {
        SqlStatistics sqlStatistics = (SqlStatistics) null;
        int num = 0;
        try
        {
          if (requestContext.IsTracing(1278, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer))
            sqlStatistics = new SqlStatistics();
          if (consumeNewlyCompletedJobs)
            this.ConsumeNewlyCompletedJobs(requestContext);
          requestContext.Trace(1218, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "[ReleaseOutstandingJobs] Releasing {0} jobs.", (object) this.m_jobsToRelease.Count);
          if (this.m_jobsToRelease.Count <= 0)
            return;
          requestContext.Trace(1219, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Releasing {0} jobRunners.", (object) this.m_jobsToRelease.Count);
          try
          {
            this.m_jobService.ReleaseJobs(requestContext, this.m_agentId, (int) this.m_maxQueueCheckInterval.TotalSeconds, this.m_jobsToRelease);
          }
          catch (TeamFoundationServiceException ex)
          {
            requestContext.Trace(1444, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Exception releasing jobs / recording job results.");
            requestContext.TraceException(1445, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, (Exception) ex);
            if (catchAndLogJobServiceExceptions)
              TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.ErrorRecordingJobResult((object) this.m_jobsToRelease.Count), (Exception) ex, TeamFoundationEventId.UnableToRecordJobResult, EventLogEntryType.Warning);
            else
              throw;
          }
          if (requestContext.IsTracing(1219, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer))
          {
            foreach (JobRunner jobRunner in this.m_jobsToRelease)
              requestContext.Trace(1219, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Released jobRunner: {0}", (object) jobRunner);
          }
          num = this.m_jobsToRelease.Count;
          this.m_jobsToRelease.Clear();
        }
        finally
        {
          if (sqlStatistics != null)
          {
            string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[ReleaseOutstandingJobs] Jobs released: {0}. stats: {1}", (object) num, (object) sqlStatistics);
            requestContext.Trace(1278, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, message);
            sqlStatistics.Dispose();
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(1322, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (ReleaseOutstandingJobs));
      }
    }

    private int ConsumeNewlyCompletedJobs(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1323, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (ConsumeNewlyCompletedJobs));
      try
      {
        List<JobRunner> jobRunnerList = new List<JobRunner>();
        requestContext.Trace(2032102, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Acquiring m_jobRunnerLock.Writer");
        List<JobRunner> newlyCompletedJobs;
        using (requestContext.AcquireWriterLock(this.m_jobRunnerLockName))
        {
          newlyCompletedJobs = this.m_newlyCompletedJobs;
          this.m_newlyCompletedJobs = jobRunnerList;
          requestContext.Trace(2032103, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Releasing m_jobRunnerLock.Writer");
        }
        newlyCompletedJobs.AddRange((IEnumerable<JobRunner>) this.m_jobsToRelease);
        this.m_jobsToRelease = newlyCompletedJobs;
        requestContext.Trace(2032104, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "[ConsumeNewlyCompletedJobs] {0} jobs ready for release.", (object) this.m_jobsToRelease.Count);
        return this.m_jobsToRelease.Count;
      }
      finally
      {
        requestContext.TraceLeave(1324, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (ConsumeNewlyCompletedJobs));
      }
    }

    private void StopAllJobs()
    {
      TeamFoundationTracingService.TraceRaw(1221, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "StopAllJobs()");
      this.m_stopAllJobsBeginTime = DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext(false))
      {
        TeamFoundationTracingService.TraceRaw(1222, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Acquiring m_jobRunnerLock.Reader");
        List<JobRunner> jobRunnerList;
        using (systemContext.AcquireReaderLock(this.m_jobRunnerLockName))
        {
          jobRunnerList = new List<JobRunner>((IEnumerable<JobRunner>) this.m_jobRunners.Values);
          TeamFoundationTracingService.TraceRaw(1225, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Releasing m_jobRunnerLock.Reader");
        }
        foreach (JobRunner jobRunner in jobRunnerList)
        {
          TeamFoundationTracingService.TraceRaw(1223, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "StopAllJobs: Issuing a stop to job runner {0}.", (object) jobRunner);
          jobRunner.Stop(true, true, FrameworkResources.JobStoppedUponServiceShutdownReason());
          TeamFoundationTracingService.TraceRaw(1224, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobRunner {0} stopped", (object) jobRunner);
        }
        while (this.GetCurrentJobRunnerCount(systemContext) > 0)
        {
          Thread.Sleep(1000);
          this.ReleaseOutstandingJobs(systemContext, true, true);
          TeamFoundationTracingService.TraceRaw(1227, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Acquiring m_jobRunnerLock.Reader");
          using (systemContext.AcquireReaderLock(this.m_jobRunnerLockName))
          {
            foreach (JobRunner jobRunner in this.m_jobRunners.Values)
              TeamFoundationTracingService.TraceRaw(1228, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "StopAllJobs: Still running: {0}.", (object) jobRunner);
            TeamFoundationTracingService.TraceRaw(1230, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Releasing m_jobRunnerLock.Reader");
          }
        }
        this.ReleaseOutstandingJobs(systemContext, true, true);
      }
      TeamFoundationTracingService.TraceRaw(1231, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Leaving StopAllJobs()");
    }

    private void OnRunnerCompleted(object sender, EventArgs args)
    {
      TeamFoundationTracingService.TraceRaw(1232, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "OnRunnerCompleted()");
      JobRunner jobRunner = sender as JobRunner;
      TeamFoundationTracingService.TraceRaw(1233, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Acquiring m_jobRunnerLock.Writer");
      using (this.m_deploymentServiceHost.ServiceHostInternal().LockManager.Lock(this.m_jobRunnerLockName, LockManager.LockType.ResourceExclusive, (long) -Environment.CurrentManagedThreadId))
      {
        if (jobRunner != null)
        {
          TeamFoundationTracingService.TraceRaw(1234, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Removing JobRunner {0}", (object) jobRunner);
          this.m_newlyCompletedJobs.Add(jobRunner);
          if (!this.m_jobRunners.Remove(jobRunner.QueueEntry))
            TeamFoundationTracingService.TraceRaw(2032105, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobRunner wasn't in dictionary to begin with: {0}", (object) jobRunner);
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentRunningJobs").Decrement();
        }
        else
          TeamFoundationTracingService.TraceRaw(1235, TraceLevel.Warning, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "OnRunnerCompleted: sender is not a JobRunner");
        if (this.m_runnerCompletedEvent != null)
        {
          TeamFoundationTracingService.TraceRaw(1236, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting m_runnerCompletedEvent");
          this.m_runnerCompletedEvent.Set();
        }
        TeamFoundationTracingService.TraceRaw(1237, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Releasing m_jobRunnerLock.Writer");
      }
      TeamFoundationTracingService.TraceRaw(1238, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Leaving OnRunnerCompleted");
    }

    private void OnQueueChanged(IVssRequestContext requestContext, Guid eventId, string eventData)
    {
      requestContext.Trace(1239, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "OnQueueChanged {0} {1}", (object) eventId, (object) eventData);
      try
      {
        if (this.ProcessIsStopping())
          requestContext.Trace(1240, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "OnQueueChanged ProcessIsStopping() - returning");
        else if (eventData == null)
        {
          requestContext.Trace(1241, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting m_queueChangedEvent");
          this.m_queueChangedEvent.Set();
        }
        else
        {
          XmlReaderSettings settings = new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          };
          using (StringReader input = new StringReader(eventData))
          {
            using (XmlReader xmlReader = XmlReader.Create((TextReader) input, settings))
            {
              xmlReader.Read();
              if (xmlReader.NodeType != XmlNodeType.Element || !(xmlReader.LocalName == "update"))
                return;
              int num = int.Parse(xmlReader.GetAttribute("t"), (IFormatProvider) CultureInfo.InvariantCulture);
              if (num == 0)
              {
                requestContext.Trace(1241, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting m_queueChangedEvent");
                this.m_queueChangedEvent.Set();
              }
              else
              {
                Guid guid1 = new Guid(xmlReader.GetAttribute("i"));
                Guid guid2 = new Guid(xmlReader.GetAttribute("s"));
                TeamFoundationJobQueueEntry foundationJobQueueEntry = new TeamFoundationJobQueueEntry();
                foundationJobQueueEntry.JobSource = guid2;
                foundationJobQueueEntry.JobId = guid1;
                TeamFoundationJobQueueEntry key = foundationJobQueueEntry;
                JobRunner jobRunner = (JobRunner) null;
                requestContext.Trace(1242, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Acquiring m_jobRunnerLock.Reader");
                using (requestContext.AcquireReaderLock(this.m_jobRunnerLockName))
                {
                  this.m_jobRunners.TryGetValue(key, out jobRunner);
                  requestContext.Trace(1244, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Releasing m_jobRunnerLock.Reader");
                }
                if (jobRunner == null)
                  return;
                requestContext.Trace(1243, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "JobRunner updateType {0}", (object) num);
                if (num != 1)
                  return;
                jobRunner.Stop(false, false, FrameworkResources.JobStoppedUponUserRequestReason());
              }
            }
          }
        }
      }
      finally
      {
        requestContext.Trace(1245, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Leaving OnQueueChanged");
      }
    }

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(1325, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (OnSettingsChanged));
      try
      {
        if (changedEntries.Count <= 0)
          return;
        this.m_deploymentSettingsChanged = true;
        TeamFoundationTracingService.TraceRaw(1255, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting m_queueChangedEvent due to settings changes.");
        this.m_queueChangedEvent.Set();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1272, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1326, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (OnSettingsChanged));
      }
    }

    private void OnRecycle(IVssRequestContext requestContext, Guid eventId, string eventData)
    {
      TeamFoundationTracingService.TraceRaw(1306, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (OnRecycle));
      RecycleRole recycleRole = RecycleRole.All;
      Guid result = Guid.Empty;
      if (eventData != null)
        recycleRole = Guid.TryParse(eventData, out result) ? RecycleRole.None : (RecycleRole) Enum.Parse(typeof (RecycleRole), eventData);
      if (recycleRole.HasFlag((Enum) RecycleRole.JobAgent) || this.m_deploymentServiceHost != null && this.m_deploymentServiceHost.DeploymentServiceHostInternal().HostManagement != null && result != Guid.Empty && result == this.m_deploymentServiceHost.DeploymentServiceHostInternal().HostManagement.ProcessId)
      {
        TeamFoundationTracingService.TraceRaw(1308, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting m_deploymentHostStoppedEvent");
        this.m_deploymentHostStoppedEvent.Set();
      }
      TeamFoundationTracingService.TraceRaw(1309, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Leaving OnRecycle");
    }

    private void OnServiceHostChanged(object sender, HostStatusChangedEventArgs eventArgs)
    {
      TeamFoundationTracingService.TraceRaw(1246, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, nameof (OnServiceHostChanged));
      if (eventArgs.Status == TeamFoundationServiceHostStatus.Started)
      {
        TeamFoundationTracingService.TraceRaw(1247, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting m_queueChangedEvent");
        this.m_queueChangedEvent.Set();
      }
      if (eventArgs.HostId == this.m_deploymentServiceHost.InstanceId && (eventArgs.Status == TeamFoundationServiceHostStatus.Stopped || eventArgs.Status == TeamFoundationServiceHostStatus.Stopping))
      {
        TeamFoundationTracingService.TraceRaw(1248, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting m_deploymentHostStoppedEvent");
        this.m_deploymentHostStoppedEvent.Set();
      }
      TeamFoundationTracingService.TraceRaw(1249, TraceLevel.Verbose, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Leaving OnServiceHostChanged");
    }

    private bool ProcessIsStopping()
    {
      ManualResetEvent processStoppedEvent = this.m_processStoppedEvent;
      if (this.m_disposing)
        return true;
      return processStoppedEvent != null && processStoppedEvent.WaitOne(0, false);
    }

    private bool ApplicationHostOrProcessIsStopping()
    {
      if (this.ProcessIsStopping())
        return true;
      ManualResetEvent hostStoppedEvent = this.m_deploymentHostStoppedEvent;
      return hostStoppedEvent != null && hostStoppedEvent.WaitOne(0, false);
    }

    private void Shutdown(Exception exception)
    {
      TeamFoundationEventLog.Default.LogException(FrameworkResources.UnexpectedJobAgentError(), exception, TeamFoundationEventId.ApplicationStopped, EventLogEntryType.Error);
      TeamFoundationTracingService.TraceRaw(1250, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Unexpected JobAgent Error {0}", (object) exception);
      if (this.ExitProcessOnFatalException | (this.m_disposed.HasValue && this.m_disposed.Value.AddSeconds(8.0) < DateTime.UtcNow))
      {
        TeamFoundationTracingService.TraceRaw(1251, TraceLevel.Error, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Exiting Process by throwing fatal exception.");
        throw exception;
      }
      ManualResetEvent processStoppedEvent = this.m_processStoppedEvent;
      if (processStoppedEvent == null)
        return;
      TeamFoundationTracingService.TraceRaw(1252, TraceLevel.Info, JobApplication.s_jobApplicationArea, JobApplication.s_jobApplicationLayer, "Setting StoppedEvent");
      processStoppedEvent.Set();
    }

    private class JobComparer : EqualityComparer<TeamFoundationJobQueueEntry>
    {
      public static JobApplication.JobComparer Instance { get; } = new JobApplication.JobComparer();

      public override bool Equals(TeamFoundationJobQueueEntry x, TeamFoundationJobQueueEntry y) => x.JobSource == y.JobSource && x.JobId == y.JobId;

      public override int GetHashCode(TeamFoundationJobQueueEntry obj)
      {
        Guid guid = obj.JobSource;
        int hashCode1 = guid.GetHashCode();
        guid = obj.JobId;
        int hashCode2 = guid.GetHashCode();
        return hashCode1 ^ hashCode2;
      }
    }
  }
}
