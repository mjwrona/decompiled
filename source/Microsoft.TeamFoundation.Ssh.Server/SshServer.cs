// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.SshServer
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading;

namespace Microsoft.TeamFoundation.Ssh.Server
{
  public class SshServer : IDisposable
  {
    private ITeamFoundationSshService _teamFoundationSshService;
    private TeamFoundationHostManagementService m_hostManagementService;
    private int m_maximumStopTime;
    private const int DefaultRetryDelaySeconds = 20;
    private int m_retryDelaySeconds = 20;
    private AutoResetEvent m_leaseRenewedEvent = new AutoResetEvent(false);
    private ManualResetEvent m_stoppingEvent = new ManualResetEvent(false);
    private ManualResetEvent m_deploymentHostStoppedEvent = new ManualResetEvent(false);
    private ISqlConnectionInfo _dbConnectionInfo;
    private Guid _applicationId;
    private IVssDeploymentServiceHost m_deploymentServiceHost;
    private TeamFoundationTaskService m_taskService;
    private static string s_startupTime;
    private IDisposableReadOnlyList<ISshApplicationExtension> _sshApplicationExtensions;
    private JobHandle m_jobHandle;
    private const short s_DefaultMaxCpuRateLimit = 100;
    private const int s_DefaultMaxMemoryMB = -1;
    private const string s_JobName = "TeamFoundationSshService_WindowsJobObject";
    private const string s_MaxCpuRateLimitRegPath = "/Configuration/SshServer/MaxCpuRateLimit";
    private const string s_MaxMemoryLimitRegPath = "/Configuration/SshServer/MaxMemoryMB";
    private const string s_Area = "Ssh";
    private const string s_Layer = "SshServer";
    private Thread m_runThread;
    private bool m_disposing;
    private object m_startDisposeLock = new object();
    private string m_teardownBeginTime;
    private DateTime? m_disposed;
    private bool m_exitProcessOnFatalException;
    private string m_sshServerDisposedTime;
    private DateTime m_lastInactiveProcessCheckTime;
    private bool m_unregisterLocalInactiveProcesses;
    private TeamFoundationTask m_unregisterRescheduleTask;
    private TeamFoundationTask m_updateSshMemoryUsageTask;
    private const bool DefaultUnregisterLocalInactiveProcesses = true;
    private static Timer[] s_disposeTimers;
    private static Timer[] s_teardownTimers;
    private static readonly VssPerformanceCounter s_privateBytesUsed = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Ssh.PrivateBytesPercent");

    public SshServer(
      string databaseConnectionString,
      string sqlUser,
      string passwordEncrypted,
      Guid instanceId,
      int maxStopTime)
    {
      SshServer.s_startupTime = DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.InitializeDatabaseConnection(databaseConnectionString, sqlUser, passwordEncrypted, instanceId);
      this.m_maximumStopTime = maxStopTime;
      this.m_exitProcessOnFatalException = true;
    }

    public void Dispose()
    {
      TeamFoundationTracingService.TraceRaw(13000160, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Enter InitializeAppInfo");
      lock (this.m_startDisposeLock)
        this.m_disposing = true;
      if (this.m_jobHandle != null)
      {
        this.m_jobHandle.Dispose();
        this.m_jobHandle = (JobHandle) null;
      }
      if (this.m_stoppingEvent != null)
      {
        this.m_stoppingEvent.Close();
        this.m_stoppingEvent = (ManualResetEvent) null;
      }
      if (this.m_deploymentHostStoppedEvent != null)
      {
        this.m_deploymentHostStoppedEvent.Close();
        this.m_deploymentHostStoppedEvent = (ManualResetEvent) null;
      }
      SshServer.s_disposeTimers = new Timer[2]
      {
        new Timer(new TimerCallback(this.LogAndTerminateProcess), (object) null, TimeSpan.FromSeconds(8.0), TimeSpan.Zero),
        new Timer(new TimerCallback(this.TerminateProcess), (object) null, TimeSpan.FromSeconds(10.0), TimeSpan.Zero)
      };
      this.m_sshServerDisposedTime = DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.m_disposed = new DateTime?(DateTime.UtcNow);
      TeamFoundationTracingService.TraceRaw(13000233, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Leaving Dispose at {0}", (object) this.m_disposed);
    }

    public void Start()
    {
      TeamFoundationTracingService.TraceRaw(13000100, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Enter Start");
      lock (this.m_startDisposeLock)
      {
        if (this.m_disposing)
        {
          TeamFoundationTracingService.TraceRaw(13000234, TraceLevel.Error, "Ssh", nameof (SshServer), "Start was called on a disposed SshServer");
          throw new ObjectDisposedException(nameof (SshServer));
        }
        this.m_runThread = new Thread(new ThreadStart(this.Run));
        this.m_runThread.Start();
      }
    }

    private void Run()
    {
      TeamFoundationTracingService.TraceRaw(13000235, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Run()");
      try
      {
        try
        {
          Thread.CurrentThread.Name = "SshServer Run Thread";
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceRaw(13000236, TraceLevel.Error, "Ssh", nameof (SshServer), "Setting name failed with {0}", (object) ex);
        }
        try
        {
          if (Thread.CurrentThread.Priority < ThreadPriority.AboveNormal)
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceRaw(13000237, TraceLevel.Error, "Ssh", nameof (SshServer), "Setting thread priority failed with {0}", (object) ex);
        }
        while (!this.ProcessIsStopping())
        {
          this.Setup();
          this.ProcessExitEvents();
          this.Teardown();
        }
        TeamFoundationTracingService.TraceRaw(13000212, TraceLevel.Info, "Ssh", nameof (SshServer), "Leaving SshServer Run loop.");
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(13000213, TraceLevel.Error, "Ssh", nameof (SshServer), "Caught {0}", (object) ex);
        TeamFoundationEventLog.Default.LogException("SSH server is exiting due to an unhandled exception.", ex);
        Environment.Exit(100);
      }
      finally
      {
        TeamFoundationTracingService.TraceRaw(13000214, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Leaving Run()");
      }
    }

    private void Teardown()
    {
      TeamFoundationTracingService.TraceRaw(13000215, TraceLevel.Info, "Ssh", nameof (SshServer), "Starting SshServer Teardown().");
      TeamFoundationEventLog.Default.Log("The SshServer is entering the Teardown stage", 13000248, EventLogEntryType.Information);
      this.m_teardownBeginTime = DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (!this.m_disposing)
        SshServer.s_teardownTimers = new Timer[2]
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
            if (this.m_taskService != null)
            {
              try
              {
                systemContext.Trace(13000261, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Removing tasks registered in {0}", (object) "SetupInternal");
                if (this.m_unregisterRescheduleTask != null)
                  this.m_taskService.RemoveTask(systemContext, this.m_unregisterRescheduleTask);
                if (this.m_updateSshMemoryUsageTask != null)
                  this.m_taskService.RemoveTask(systemContext, this.m_updateSshMemoryUsageTask);
              }
              catch (Exception ex)
              {
                systemContext.Trace(13000262, TraceLevel.Error, "Ssh", nameof (SshServer), "Caught Exception {0} while removing task", (object) ex);
              }
              this.m_taskService = (TeamFoundationTaskService) null;
            }
            if (this.m_hostManagementService != null)
            {
              try
              {
                systemContext.Trace(13000263, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Removing LeaseRenewed callback.");
                this.m_hostManagementService.LeaseRenewed -= new EventHandler(this.OnLeaseRenewed);
              }
              catch (Exception ex)
              {
                systemContext.TraceException(13000264, TraceLevel.Error, "Ssh", nameof (SshServer), ex);
              }
              this.m_hostManagementService = (TeamFoundationHostManagementService) null;
            }
            this.UnRegisterRecycleRoleNotification(systemContext);
            this.UnRegisterMaxCpuRateLimitNotification(systemContext);
            this.UnRegisterMaxMeoryLimitNotification(systemContext);
            this.m_jobHandle.Close();
            this.m_jobHandle = (JobHandle) null;
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(13000216, TraceLevel.Error, "Ssh", nameof (SshServer), "Caught {0} while removing callbacks during Teardown", (object) ex);
      }
      try
      {
        if (this.m_deploymentServiceHost != null)
        {
          this.m_deploymentServiceHost.StatusChanged -= new EventHandler<HostStatusChangedEventArgs>(this.Host_StatusChanged);
          this._teamFoundationSshService?.CloseSessions();
          this._teamFoundationSshService = (ITeamFoundationSshService) null;
          this.TeardownApplicationExtensions();
          try
          {
            TeamFoundationTracingService.TraceRaw(13000217, TraceLevel.Info, "Ssh", nameof (SshServer), "SshServer Teardown: Disposing of DeploymentServiceHost.");
            this.m_deploymentServiceHost.Dispose();
            TeamFoundationTracingService.TraceRaw(13000218, TraceLevel.Info, "Ssh", nameof (SshServer), "SshServer Teardown: Successfully disposed of DeploymentServiceHost.");
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceRaw(13000219, TraceLevel.Error, "Ssh", nameof (SshServer), "SshServer Teardown: Exception while disposing of DeploymentServiceHost.", (object) ex);
            throw;
          }
          this.m_deploymentServiceHost = (IVssDeploymentServiceHost) null;
        }
        else
          TeamFoundationTracingService.TraceRaw(13000220, TraceLevel.Error, "Ssh", nameof (SshServer), "deploymentHost is null at beginning of Teardown.");
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(13000221, TraceLevel.Error, "Ssh", nameof (SshServer), "Caught {0}", (object) ex);
        ExceptionHandlerUtility.HandleException(ex);
        TeamFoundationTracingService.TraceRaw(13000222, TraceLevel.Info, "Ssh", nameof (SshServer), "Shutting down on {0}", (object) ex);
        this.Shutdown(ex);
      }
      finally
      {
        if (SshServer.s_teardownTimers != null)
        {
          foreach (Timer teardownTimer in SshServer.s_teardownTimers)
            teardownTimer.Dispose();
          SshServer.s_teardownTimers = (Timer[]) null;
        }
      }
    }

    private void Shutdown(Exception exception)
    {
      TeamFoundationEventLog.Default.LogException("SshServer had an unexpected error.", exception, TeamFoundationEventId.ApplicationStopped, EventLogEntryType.Error);
      TeamFoundationTracingService.TraceRaw(13000223, TraceLevel.Error, "Ssh", nameof (SshServer), "Unexpected SshServer Error {0}", (object) exception);
      if (this.m_exitProcessOnFatalException | (this.m_disposed.HasValue && this.m_disposed.Value.AddSeconds(8.0) < DateTime.UtcNow))
      {
        TeamFoundationTracingService.TraceRaw(13000224, TraceLevel.Error, "Ssh", nameof (SshServer), "Exiting Process by throwing fatal exception.");
        throw exception;
      }
      ManualResetEvent stoppingEvent = this.m_stoppingEvent;
      if (stoppingEvent == null)
        return;
      TeamFoundationTracingService.TraceRaw(13000225, TraceLevel.Info, "Ssh", nameof (SshServer), "Setting m_stoppingEvent");
      stoppingEvent.Set();
    }

    private void LogAndTerminateProcess(object state)
    {
      if (!this.m_disposed.HasValue)
      {
        TeamFoundationEventLog.Default.Log("The Sshserver process is still running despite shutdown being initiated several seconds ago.", TeamFoundationEventId.ProcessStillRunningError, EventLogEntryType.Error);
        TeamFoundationTracingService.TraceRaw(13000226, TraceLevel.Error, "Ssh", nameof (SshServer), "Throwing SshProcessStillRunningException to produce a watson with a dump.");
        throw new SshProcessStillRunningException();
      }
      if (SshServer.s_teardownTimers != null)
      {
        TeamFoundationEventLog.Default.Log("The Teardown operation timed out while the SshServer was recycling or shutting down.", 13000293, EventLogEntryType.Error);
        TeamFoundationEventLog.Default.Log("The Teardown operation timed out while the SshServer was recycling or shutting down.", TeamFoundationEventId.ApplicationStopped, EventLogEntryType.Error);
        TeamFoundationTracingService.TraceRaw(13000227, TraceLevel.Error, "Ssh", nameof (SshServer), "Throwing SshServerTeardownTimedOutException to produce a watson with a dump.");
        throw new SshServerTeardownTimeoutException();
      }
    }

    private void TerminateProcess(object state)
    {
      if (!this.m_disposed.HasValue)
        throw new SshProcessStillRunningException();
      if (SshServer.s_teardownTimers != null)
        throw new SshServerTeardownTimeoutException();
    }

    private void ProcessExitEvents()
    {
      TeamFoundationTracingService.TraceRaw(13000249, TraceLevel.Info, "Ssh", nameof (SshServer), "ProcessExitEvents().");
      TeamFoundationEventLog.Default.Log("The SshServer is entering the {ProcessExitEvents} stage.", 13000250, EventLogEntryType.Information);
      SshUtil.RetryOperationsUntilSuccessful(new RetryOperations(this.ProcessExitEventsInternal), ref this.m_retryDelaySeconds);
      TeamFoundationTracingService.TraceRaw(13000251, TraceLevel.Info, "Ssh", nameof (SshServer), "Leaving ProcessExitEvents().");
    }

    private void ProcessExitEventsInternal()
    {
      if (this.ApplicationHostOrProcessIsStopping())
      {
        TeamFoundationTracingService.TraceRaw(13000265, TraceLevel.Info, "Ssh", nameof (SshServer), "ApplicationHostOrProcessIsStopping - leaving ProcessExitEventsInternal().");
      }
      else
      {
label_2:
        Stopwatch stopwatch = Stopwatch.StartNew();
        int num = WaitHandle.WaitAny(new WaitHandle[2]
        {
          (WaitHandle) this.m_stoppingEvent,
          (WaitHandle) this.m_deploymentHostStoppedEvent
        }, -1, false);
        stopwatch.Stop();
        using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext(false))
        {
          systemContext.Trace(13000239, TraceLevel.Verbose, "Ssh", nameof (SshServer), "[ProcessExitEvents] Done waiting. WaitEvent={0}, ElapsedTime={1:0,0}/{2:0,0} ms.", (object) num, (object) stopwatch.Elapsed.TotalMilliseconds, (object) "No Timeout");
          systemContext.RequestTimer.RequestTimerInternal().AddTimeSpentDelayed(stopwatch.ElapsedTicks);
          if (num == 0)
            return;
          if (num != 1)
            goto label_2;
        }
      }
    }

    private bool ProcessIsStopping()
    {
      ManualResetEvent stoppingEvent = this.m_stoppingEvent;
      if (this.m_disposing)
        return true;
      return stoppingEvent != null && stoppingEvent.WaitOne(0, false);
    }

    private bool ApplicationHostOrProcessIsStopping()
    {
      if (this.ProcessIsStopping())
        return true;
      ManualResetEvent hostStoppedEvent = this.m_deploymentHostStoppedEvent;
      return hostStoppedEvent != null && hostStoppedEvent.WaitOne(0, false);
    }

    public void Stop()
    {
      try
      {
        TeamFoundationTracingService.TraceRaw(13000150, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Enter Stop");
        this.m_stoppingEvent?.Set();
        Thread runThread = this.m_runThread;
        if (runThread == null)
          return;
        TeamFoundationTracingService.TraceRaw(13000227, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Joining m_runThread");
        if (runThread.Join(this.m_maximumStopTime - 1000))
          TeamFoundationTracingService.TraceRaw(13000228, TraceLevel.Info, "Ssh", nameof (SshServer), "SshServer.Stop: Joined run processing thread.");
        else
          TeamFoundationTracingService.TraceRaw(13000229, TraceLevel.Warning, "Ssh", nameof (SshServer), "SshService.Stop: Timeout joining sshserver processing thread.");
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(13000153, TraceLevel.Error, "Ssh", nameof (SshServer), "SshServer.Stop: Exception while stopping of SshServer.", (object) ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceRaw(13000155, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Leave Stop");
      }
    }

    private void Setup()
    {
      TeamFoundationTracingService.TraceRaw(13000245, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Starting SshServer Setup().");
      TeamFoundationEventLog.Default.Log("The SshServer is entering the Setup stage.", 13000246, EventLogEntryType.Information);
      SshUtil.RetryOperationsUntilSuccessful(new RetryOperations(this.SetupInternal), ref this.m_retryDelaySeconds);
      TeamFoundationTracingService.TraceRaw(13000247, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Leaving Setup()");
    }

    private void SetupInternal()
    {
      try
      {
        TeamFoundationTracingService.TraceRaw(13000145, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Enter SetupInternal");
        if (this.ProcessIsStopping())
        {
          TeamFoundationTracingService.TraceRaw(13000273, TraceLevel.Info, "Ssh", nameof (SshServer), "ApplicationHostOrProcessIsStopping - leaving SetupInternal().");
        }
        else
        {
          VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Ssh.PrivateBytesPercentBase");
          Microsoft.TeamFoundation.Common.Internal.NativeMethods.MEMORYSTATUSEX lpBuffer = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.MEMORYSTATUSEX();
          if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.GlobalMemoryStatusEx(lpBuffer))
            throw new Win32Exception();
          performanceCounter.SetValue((long) lpBuffer.ullTotalPhys);
          this.m_teardownBeginTime = (string) null;
          if (this.m_hostManagementService != null)
          {
            try
            {
              this.m_hostManagementService.LeaseRenewed -= new EventHandler(this.OnLeaseRenewed);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(13000292, TraceLevel.Error, "Ssh", nameof (SshServer), ex);
            }
            this.m_hostManagementService = (TeamFoundationHostManagementService) null;
          }
          if (this.m_deploymentServiceHost != null)
          {
            try
            {
              this.m_deploymentServiceHost.StatusChanged -= new EventHandler<HostStatusChangedEventArgs>(this.Host_StatusChanged);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(13000291, TraceLevel.Error, "Ssh", nameof (SshServer), ex);
            }
            this.m_deploymentServiceHost.Dispose();
            this.m_deploymentServiceHost = (IVssDeploymentServiceHost) null;
          }
          this.CreateDeploymentServiceHost();
          using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext())
          {
            this.RegisterForRoleRecycleEvent(systemContext);
            IVssRegistryService service = systemContext.GetService<IVssRegistryService>();
            short maxCpuRateLimitPercentage = service.GetValue<short>(systemContext, (RegistryQuery) "/Configuration/SshServer/MaxCpuRateLimit", (short) 100);
            this.SetCpuRateControlHardCap(systemContext, maxCpuRateLimitPercentage);
            this.RegisterMaxCpuRateLimitNotification(systemContext);
            int maxMemoryLimitPercentage = service.GetValue<int>(systemContext, (RegistryQuery) "/Configuration/SshServer/MaxMemoryMB", -1);
            this.SetMemoryControlHardCap(systemContext, maxMemoryLimitPercentage);
            this.RegisterMaxMemoryLimitNotification(systemContext);
            systemContext.Trace(13000264, TraceLevel.Info, "Ssh", nameof (SshServer), "Adding UnregisterRescheduleTask Task");
            this.m_unregisterRescheduleTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.UnregisterRescheduleTask), (object) null, 60000);
            this.m_updateSshMemoryUsageTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.UpdateSshMemoryUsage), (object) null, 60000);
            this.m_unregisterLocalInactiveProcesses = service.GetValue<bool>(systemContext, (RegistryQuery) FrameworkServerConstants.UnregisterLocalInactiveProcessesPath, true);
            this.m_taskService = systemContext.GetService<TeamFoundationTaskService>();
            this.m_taskService.AddTask(systemContext, this.m_unregisterRescheduleTask);
            this.m_taskService.AddTask(systemContext, this.m_updateSshMemoryUsageTask);
            this.StartTeamFoundationSshService(systemContext);
            this.m_hostManagementService = systemContext.GetService<TeamFoundationHostManagementService>();
            systemContext.Trace(13000252, TraceLevel.Info, "Ssh", nameof (SshServer), "Detecting Inactive Processes");
            this.m_hostManagementService.DetectInactiveProcesses(systemContext);
            this.m_hostManagementService.LeaseRenewed += new EventHandler(this.OnLeaseRenewed);
            TeamFoundationTracingService.TraceRaw(13000230, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Setting m_deploymentHostStoppedEvent");
            this.m_deploymentHostStoppedEvent.Reset();
            systemContext.Trace(13000253, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Leaving SetupInternal()");
          }
        }
      }
      catch (HostShutdownException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(13000146, "Ssh", nameof (SshServer), (Exception) ex);
        TeamFoundationTracingService.TraceRaw(13000266, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Determining whether to unregister local inactive processes in response to host shutdown.");
        if (DateTime.UtcNow > this.m_lastInactiveProcessCheckTime.AddSeconds(60.0))
        {
          this.m_lastInactiveProcessCheckTime = DateTime.UtcNow;
          using (IVssRequestContext servicingContext = this.m_deploymentServiceHost.CreateServicingContext())
          {
            try
            {
              this.m_unregisterLocalInactiveProcesses = servicingContext.GetService<IVssRegistryService>().GetValue<bool>(servicingContext, (RegistryQuery) FrameworkServerConstants.UnregisterLocalInactiveProcessesPath, true);
              if (this.m_unregisterLocalInactiveProcesses)
              {
                TeamFoundationTracingService.TraceRaw(13000267, TraceLevel.Info, "Ssh", nameof (SshServer), "Unregistering local inactive processes outside of task service..");
                this.UnregisterRescheduleTask(servicingContext, (object) null);
              }
              else
                TeamFoundationTracingService.TraceRaw(13000268, TraceLevel.Info, "Ssh", nameof (SshServer), "Skipping unregistering of local inactive processes. Disabled in registry.");
            }
            catch
            {
              TeamFoundationTracingService.TraceExceptionRaw(13000290, "Ssh", nameof (SshServer), (Exception) ex);
            }
          }
        }
        else
          TeamFoundationTracingService.TraceRaw(13000269, TraceLevel.Verbose, "Ssh", nameof (SshServer), "UnregisterLocalInactiveProcesses was attempted within the last 60 seconds.");
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceRaw(13000147, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Leave SetupInternal");
      }
    }

    private void OnLeaseRenewed(object sender, EventArgs e) => this.m_leaseRenewedEvent.Set();

    private void UnregisterRescheduleTask(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(13000271, "Ssh", nameof (SshServer), nameof (UnregisterRescheduleTask));
      try
      {
        if (!this.m_leaseRenewedEvent.WaitOne(0, false) && !this.m_unregisterLocalInactiveProcesses)
        {
          requestContext.Trace(13000254, TraceLevel.Verbose, "Ssh", nameof (SshServer), "UnregisterRescheduleTask returning - nothing to do.");
        }
        else
        {
          TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
          List<TeamFoundationServiceHostProcess> serviceHostProcessList = service.QueryServiceHostProcesses(requestContext.Elevate(), Guid.Empty);
          if (!this.m_unregisterLocalInactiveProcesses)
            return;
          foreach (TeamFoundationServiceHostProcess serviceHostProcess in serviceHostProcessList)
          {
            if (serviceHostProcess.MachineId != Guid.Empty && service.MachineId != Guid.Empty)
            {
              if (serviceHostProcess.MachineId != service.MachineId && string.Equals(serviceHostProcess.MachineName, Environment.MachineName, StringComparison.OrdinalIgnoreCase))
              {
                requestContext.Trace(13000255, TraceLevel.Error, "Ssh", nameof (SshServer), "Skipping Process {0} because the machine Id don't match ({1},{2}) but the machine names do: ({3},{4})!!!", (object) serviceHostProcess.ProcessId, (object) serviceHostProcess.MachineId, (object) service.MachineId, (object) serviceHostProcess.MachineName, (object) Environment.MachineName);
                continue;
              }
              if (serviceHostProcess.MachineId == service.MachineId && !string.Equals(serviceHostProcess.MachineName, Environment.MachineName, StringComparison.OrdinalIgnoreCase))
              {
                requestContext.Trace(1116, TraceLevel.Error, "Ssh", nameof (SshServer), "Skipping Process {0} because the machine names don't match ({1},{2}) but the machine ids do: {3}!!!", (object) serviceHostProcess.ProcessId, (object) serviceHostProcess.MachineName, (object) Environment.MachineName, (object) service.MachineId);
                continue;
              }
              if (serviceHostProcess.MachineId != service.MachineId)
              {
                requestContext.Trace(13000256, TraceLevel.Info, "Ssh", nameof (SshServer), "Skipping Process {0} because it is not local to this machine", (object) serviceHostProcess.ProcessId);
                continue;
              }
            }
            else if (!string.Equals(serviceHostProcess.MachineName, Environment.MachineName, StringComparison.OrdinalIgnoreCase))
            {
              requestContext.Trace(13000257, TraceLevel.Info, "Ssh", nameof (SshServer), "Skipping Process {0} because it is not local to this machine", (object) serviceHostProcess.ProcessId);
              continue;
            }
            try
            {
              Process processById = Process.GetProcessById(serviceHostProcess.OSProcessId);
              if (serviceHostProcess.ProcessName.StartsWith(processById.ProcessName, StringComparison.OrdinalIgnoreCase))
              {
                requestContext.Trace(13000258, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Process {0} ID:{1} still exists", (object) serviceHostProcess.ProcessName, (object) serviceHostProcess.OSProcessId);
                continue;
              }
            }
            catch (ArgumentException ex)
            {
            }
            requestContext.Trace(13000259, TraceLevel.Warning, "Ssh", nameof (SshServer), "Process {0} ID:{1} disappeared without unregistering!", (object) serviceHostProcess.ProcessName, (object) serviceHostProcess.OSProcessId);
            service.UnregisterProcess(requestContext.Elevate(), serviceHostProcess.ProcessId);
            requestContext.Trace(13000260, TraceLevel.Info, "Ssh", nameof (SshServer), "Process {0} ID:{1} successfully unregistered", (object) serviceHostProcess.ProcessName, (object) serviceHostProcess.OSProcessId);
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(13000272, "Ssh", nameof (SshServer), nameof (UnregisterRescheduleTask));
      }
    }

    private void UpdateSshMemoryUsage(IVssRequestContext requestContext, object taskArgs) => SshServer.s_privateBytesUsed.SetValue(Process.GetCurrentProcess().PrivateMemorySize64);

    private void SetCpuRateControlHardCap(
      IVssRequestContext requestContext,
      short maxCpuRateLimitPercentage)
    {
      requestContext.TraceEnter(13000186, "Ssh", nameof (SshServer), nameof (SetCpuRateControlHardCap));
      try
      {
        using (Process currentProcess = Process.GetCurrentProcess())
        {
          if (this.m_jobHandle == null)
            this.m_jobHandle = new JobHandle("TeamFoundationSshService_WindowsJobObject");
          if (this.m_jobHandle.IsInvalid || this.m_jobHandle.IsClosed)
            return;
          this.m_jobHandle.AddProcess(currentProcess);
          this.m_jobHandle.SetCpuRateControlHardCap(maxCpuRateLimitPercentage);
        }
      }
      catch (Exception ex)
      {
        if (this.m_jobHandle != null)
          this.m_jobHandle.Dispose();
        requestContext.TraceException(13000187, "Ssh", nameof (SshServer), ex);
      }
      finally
      {
        requestContext.TraceLeave(13000188, "Ssh", nameof (SshServer), nameof (SetCpuRateControlHardCap));
      }
    }

    private void SetMemoryControlHardCap(
      IVssRequestContext requestContext,
      int maxMemoryLimitPercentage)
    {
      requestContext.TraceEnter(13000200, "Ssh", nameof (SshServer), nameof (SetMemoryControlHardCap));
      try
      {
        using (Process currentProcess = Process.GetCurrentProcess())
        {
          if (this.m_jobHandle == null)
            this.m_jobHandle = new JobHandle("TeamFoundationSshService_WindowsJobObject");
          if (this.m_jobHandle.IsInvalid || this.m_jobHandle.IsClosed)
            return;
          this.m_jobHandle.AddProcess(currentProcess);
          this.m_jobHandle.SetMemoryRateControlHardCap(maxMemoryLimitPercentage);
        }
      }
      catch (Exception ex)
      {
        if (this.m_jobHandle != null)
          this.m_jobHandle.Dispose();
        requestContext.TraceException(13000201, "Ssh", nameof (SshServer), ex);
      }
      finally
      {
        requestContext.TraceLeave(13000209, "Ssh", nameof (SshServer), nameof (SetMemoryControlHardCap));
      }
    }

    private void StartTeamFoundationSshService(IVssRequestContext systemRequestContext)
    {
      try
      {
        systemRequestContext.TraceEnter(13000140, "Ssh", nameof (SshServer), nameof (StartTeamFoundationSshService));
        WebApiConfiguration.Initialize(systemRequestContext);
        try
        {
          this._sshApplicationExtensions = systemRequestContext.GetExtensions<ISshApplicationExtension>();
          if (this._sshApplicationExtensions != null)
          {
            foreach (ISshApplicationExtension applicationExtension in (IEnumerable<ISshApplicationExtension>) this._sshApplicationExtensions)
            {
              systemRequestContext.Trace(13000143, TraceLevel.Info, "Ssh", nameof (SshServer), "Starting extension {0}", (object) applicationExtension);
              applicationExtension.Start(systemRequestContext);
            }
          }
        }
        catch (Exception ex)
        {
          systemRequestContext.TraceException(13000141, "Ssh", nameof (SshServer), ex);
        }
        this._teamFoundationSshService = systemRequestContext.GetService<ITeamFoundationSshService>();
      }
      catch (HostShutdownException ex)
      {
        systemRequestContext.TraceException(13000141, "Ssh", nameof (SshServer), (Exception) ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(13000142, "Ssh", nameof (SshServer), nameof (StartTeamFoundationSshService));
      }
    }

    private void InitializeDatabaseConnection(
      string databaseConnectionString,
      string sqlUser,
      string passwordEncrypted,
      Guid instanceId)
    {
      TeamFoundationTracingService.TraceRaw(13000135, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Enter InitializeDatabaseConnection");
      try
      {
        databaseConnectionString = ConnectionStringUtility.DecryptAndNormalizeConnectionString(databaseConnectionString);
        if (string.IsNullOrEmpty(databaseConnectionString))
          throw new ApplicationException(Resource.SSHServerConfigDbSettingInvalid);
        SecureString password = (SecureString) null;
        if (!string.IsNullOrEmpty(passwordEncrypted))
          password = EncryptionUtility.DecryptSecret(passwordEncrypted);
        TeamFoundationTracingService.TraceRaw(13000136, TraceLevel.Verbose, "Ssh", nameof (SshServer), "InitializeDatabaseConnection Creating DBConnectionInfo");
        this._dbConnectionInfo = SqlConnectionInfoFactory.Create(databaseConnectionString, sqlUser, password);
        this._applicationId = instanceId;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(13000137, "Ssh", nameof (SshServer), ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceRaw(13000138, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Leave InitializeDatabaseConnection");
      }
    }

    private void CreateDeploymentServiceHost()
    {
      TeamFoundationTracingService.TraceRaw(13000125, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Enter CreateDeploymentHost()");
      int num = DatabaseManagementConstants.InvalidDatabaseId;
      using (DatabaseManagementComponent componentRaw = this._dbConnectionInfo.CreateComponentRaw<DatabaseManagementComponent>())
      {
        using (ResultCollection resultCollection = componentRaw.QueryDatabases())
        {
          foreach (InternalDatabaseProperties databaseProperties in resultCollection.GetCurrent<InternalDatabaseProperties>().Items)
          {
            if (string.Equals(this._dbConnectionInfo.ConnectionString, databaseProperties.ConnectionInfoWrapper.ConnectionString, StringComparison.OrdinalIgnoreCase))
              num = databaseProperties.DatabaseId;
          }
        }
      }
      string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      TeamFoundationServiceHostProperties serviceHostProperties1 = new TeamFoundationServiceHostProperties();
      serviceHostProperties1.HostType = TeamFoundationHostType.Application | TeamFoundationHostType.Deployment;
      serviceHostProperties1.Id = this._applicationId;
      serviceHostProperties1.PhysicalDirectory = Path.GetDirectoryName(directoryName);
      serviceHostProperties1.DatabaseId = num;
      serviceHostProperties1.PlugInDirectory = Path.Combine(directoryName, "Plugins");
      TeamFoundationServiceHostProperties serviceHostProperties2 = serviceHostProperties1;
      VssExtensionManagementService.DefaultPluginPath = serviceHostProperties2.PlugInDirectory;
      TeamFoundationTracingService.TraceRaw(13000126, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Creating new DeploymentServiceHost Instance with {0}", (object) serviceHostProperties2);
      DeploymentServiceHostOptions options = new DeploymentServiceHostOptions(HostProcessType.Ssh);
      this.m_deploymentServiceHost = DeploymentServiceHostFactory.CreateDeploymentServiceHost((HostProperties) serviceHostProperties2, this._dbConnectionInfo, options);
      this.m_deploymentServiceHost.ServiceHostInternal().RequestFilters = VssExtensionManagementService.GetExtensionsRaw<ITeamFoundationRequestFilter>(VssExtensionManagementService.DefaultPluginPath);
      this.m_deploymentServiceHost.StatusChanged += new EventHandler<HostStatusChangedEventArgs>(this.Host_StatusChanged);
      TeamFoundationTracingService.TraceRaw(13000127, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Leave CreateDeploymentHost()");
    }

    private void RegisterForRoleRecycleEvent(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(13000110, "Ssh", nameof (SshServer), nameof (RegisterForRoleRecycleEvent));
      systemRequestContext.GetService<TeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.Recycle, new SqlNotificationCallback(this.OnRecycle), false);
      systemRequestContext.TraceLeave(13000111, "Ssh", nameof (SshServer), nameof (RegisterForRoleRecycleEvent));
    }

    private void Host_StatusChanged(object sender, HostStatusChangedEventArgs eventArgs)
    {
      if (eventArgs.HostId == this.m_deploymentServiceHost.InstanceId && (eventArgs.Status == TeamFoundationServiceHostStatus.Stopped || eventArgs.Status == TeamFoundationServiceHostStatus.Stopping))
      {
        TeamFoundationTracingService.TraceRaw(13000238, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Setting m_deploymentHostStoppedEvent");
        this.m_deploymentHostStoppedEvent.Set();
      }
      TeamFoundationTracingService.TraceRaw(13000231, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Leaving Host_StatusChanged");
    }

    private void TeardownApplicationExtensions()
    {
      using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext(false))
      {
        systemContext.TraceEnter(13000242, "Ssh", nameof (SshServer), nameof (TeardownApplicationExtensions));
        try
        {
          systemContext.Trace(13000240, TraceLevel.Info, "Ssh", nameof (SshServer), "Disposing plugins for ISshApplicationExtension");
          if (this._sshApplicationExtensions == null)
            return;
          List<Exception> innerExceptions = new List<Exception>();
          try
          {
            foreach (ISshApplicationExtension applicationExtension in (IEnumerable<ISshApplicationExtension>) this._sshApplicationExtensions)
            {
              systemContext.Trace(13000241, TraceLevel.Info, "Ssh", nameof (SshServer), "Disposing extension {0}", (object) applicationExtension);
              try
              {
                applicationExtension.Stop(systemContext);
              }
              catch (Exception ex)
              {
                systemContext.TraceException(13000244, "Ssh", nameof (SshServer), ex);
                innerExceptions.Add(ex);
              }
            }
            this._sshApplicationExtensions.Dispose();
          }
          finally
          {
            this._sshApplicationExtensions = (IDisposableReadOnlyList<ISshApplicationExtension>) null;
          }
          if (innerExceptions.Count > 0)
            throw new AggregateException((IEnumerable<Exception>) innerExceptions);
        }
        finally
        {
          systemContext.TraceLeave(13000243, "Ssh", nameof (SshServer), nameof (TeardownApplicationExtensions));
        }
      }
    }

    private void UnRegisterRecycleRoleNotification(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(13000120, "Ssh", nameof (SshServer), nameof (UnRegisterRecycleRoleNotification));
      systemRequestContext.GetService<TeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.Recycle, new SqlNotificationCallback(this.OnRecycle), false);
      systemRequestContext.TraceLeave(13000121, "Ssh", nameof (SshServer), nameof (UnRegisterRecycleRoleNotification));
    }

    private void OnRecycle(IVssRequestContext requestcontext, Guid eventclass, string eventData)
    {
      requestcontext.TraceEnter(13000115, "Ssh", nameof (SshServer), nameof (OnRecycle));
      RecycleRole recycleRole = RecycleRole.All;
      Guid result = Guid.Empty;
      if (eventData != null)
        recycleRole = Guid.TryParse(eventData, out result) ? RecycleRole.None : (RecycleRole) Enum.Parse(typeof (RecycleRole), eventData);
      if (recycleRole.HasFlag((Enum) RecycleRole.AT) || this.m_deploymentServiceHost != null && this.m_deploymentServiceHost.DeploymentServiceHostInternal().HostManagement != null && result != Guid.Empty && result == this.m_deploymentServiceHost.DeploymentServiceHostInternal().HostManagement.ProcessId)
      {
        TeamFoundationTracingService.TraceRaw(13000232, TraceLevel.Verbose, "Ssh", nameof (SshServer), "Setting m_deploymentHostStoppedEvent");
        this.m_deploymentHostStoppedEvent.Set();
      }
      requestcontext.TraceLeave(13000116, "Ssh", nameof (SshServer), nameof (OnRecycle));
    }

    private void RegisterMaxCpuRateLimitNotification(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(13000176, "Ssh", nameof (SshServer), nameof (RegisterMaxCpuRateLimitNotification));
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnMaxCpuRateLimitChanged), "/Configuration/SshServer/MaxCpuRateLimit");
      requestContext.TraceLeave(13000180, "Ssh", nameof (SshServer), nameof (RegisterMaxCpuRateLimitNotification));
    }

    private void UnRegisterMaxCpuRateLimitNotification(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(13000181, "Ssh", nameof (SshServer), nameof (UnRegisterMaxCpuRateLimitNotification));
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnMaxCpuRateLimitChanged));
      requestContext.TraceLeave(13000185, "Ssh", nameof (SshServer), nameof (UnRegisterMaxCpuRateLimitNotification));
    }

    private void RegisterMaxMemoryLimitNotification(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(13000176, "Ssh", nameof (SshServer), nameof (RegisterMaxMemoryLimitNotification));
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnMaxMemoryLimitChanged), "/Configuration/SshServer/MaxMemoryMB");
      requestContext.TraceLeave(13000180, "Ssh", nameof (SshServer), nameof (RegisterMaxMemoryLimitNotification));
    }

    private void UnRegisterMaxMeoryLimitNotification(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(13000181, "Ssh", nameof (SshServer), nameof (UnRegisterMaxMeoryLimitNotification));
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnMaxMemoryLimitChanged));
      requestContext.TraceLeave(13000185, "Ssh", nameof (SshServer), nameof (UnRegisterMaxMeoryLimitNotification));
    }

    private void OnMaxCpuRateLimitChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection entries)
    {
      requestContext.TraceEnter(13000170, "Ssh", nameof (SshServer), nameof (OnMaxCpuRateLimitChanged));
      try
      {
        RegistryEntry entry;
        if (!entries.TryGetValue("/Configuration/SshServer/MaxCpuRateLimit", out entry))
          return;
        short result = 0;
        if (!short.TryParse(entry.Value, out result))
          result = (short) 100;
        this.SetCpuRateControlHardCap(requestContext, result);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(13000173, "Ssh", nameof (SshServer), ex);
      }
      finally
      {
        requestContext.TraceLeave(13000175, "Ssh", nameof (SshServer), nameof (OnMaxCpuRateLimitChanged));
      }
    }

    private void OnMaxMemoryLimitChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection entries)
    {
      requestContext.TraceEnter(13000190, "Ssh", nameof (SshServer), nameof (OnMaxMemoryLimitChanged));
      try
      {
        RegistryEntry entry;
        if (!entries.TryGetValue("/Configuration/SshServer/MaxMemoryMB", out entry))
          return;
        int result = 0;
        if (!int.TryParse(entry.Value, out result))
          result = -1;
        this.SetMemoryControlHardCap(requestContext, result);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(13000191, "Ssh", nameof (SshServer), ex);
      }
      finally
      {
        requestContext.TraceLeave(13000199, "Ssh", nameof (SshServer), nameof (OnMaxMemoryLimitChanged));
      }
    }
  }
}
