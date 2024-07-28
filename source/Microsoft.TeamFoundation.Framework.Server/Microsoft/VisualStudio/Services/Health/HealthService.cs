// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Health.HealthService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Health;
using System;
using System.Diagnostics;
using System.Web;

namespace Microsoft.VisualStudio.Services.Health
{
  internal class HealthService : IVssFrameworkService
  {
    private readonly Stopwatch m_slotMissmatchStopwatch = new Stopwatch();
    private HealthService.DeploymentSlotState m_deploymentSlotState = HealthService.DeploymentSlotState.Offline;
    private string m_deploymentId;
    private string m_deploymentSlot;
    private bool m_isVMSS;
    private bool m_httpAppErrorReported;
    private static readonly RegistryQuery s_afdHealthCheckQuality = new RegistryQuery(FrameworkServerConstants.AfdHealthCheckQuality);
    private DateTime m_lastDatabasePropertyRead = DateTime.MinValue;
    private static readonly TimeSpan s_databaseCacheExpiration = TimeSpan.FromSeconds(15.0);
    private static readonly TimeSpan s_1minutes = TimeSpan.FromMinutes(1.0);
    private bool m_configDbHealthy;
    private ServiceLevel m_configDbServiceLevel = ServiceLevel.MaxServiceLevel;
    private int m_pid;
    private bool m_continueProcessRequests;
    private readonly object m_lockConfigDbHealth = new object();
    private readonly TimeSpan m_taskInterval = TimeSpan.FromSeconds(5.0);
    private const string c_processRequests = "/Configuration/Health/ProcessRequestsConfigDbUnavailable";
    private const string c_loadBalancerUserAgent = "Load Balancer Agent";
    private const string c_AfdUserAgent = "Edge Health Probe";
    private const string c_area = "Health";
    private const string c_layer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        this.m_deploymentId = systemRequestContext.GetService<IHostedTenantService>().DeploymentId;
        this.m_isVMSS = string.Equals(Environment.GetEnvironmentVariable("DEPLOYMENT_ENVIRONMENT", EnvironmentVariableTarget.Machine), "Vmss", StringComparison.Ordinal);
        if (this.m_isVMSS)
        {
          string environmentVariable = Environment.GetEnvironmentVariable("ROLE_INSTANCE_ID");
          if (!string.IsNullOrEmpty(environmentVariable))
          {
            if (environmentVariable.EndsWith("_Blue", StringComparison.OrdinalIgnoreCase))
              this.m_deploymentSlot = "Blue";
            else if (environmentVariable.EndsWith("_Green", StringComparison.OrdinalIgnoreCase))
              this.m_deploymentSlot = "Green";
          }
        }
      }
      else
        this.m_pid = Process.GetCurrentProcess().Id;
      ITeamFoundationTaskService service = systemRequestContext.GetService<ITeamFoundationTaskService>();
      TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.UpdateConfigDbHealthState), (object) null, DateTime.UtcNow, (int) this.m_taskInterval.TotalMilliseconds);
      Guid instanceId = systemRequestContext.ServiceHost.InstanceId;
      TeamFoundationTask task = teamFoundationTask;
      service.AddTask(instanceId, task);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal static bool IsDisabledByHealthAgent()
    {
      bool flag = string.Equals(Environment.GetEnvironmentVariable("VSSHEALTHAGENT_TAKE_VM_OUT_OF_ROTATION", EnvironmentVariableTarget.Machine), bool.TrueString, StringComparison.OrdinalIgnoreCase);
      if (!flag)
        flag = string.Equals(Environment.GetEnvironmentVariable("VSSHEALTHAGENT_TAKE_VM_OUT_OF_ROTATION_HIGH_CPU", EnvironmentVariableTarget.Machine), bool.TrueString, StringComparison.OrdinalIgnoreCase);
      if (!flag)
        flag = string.Equals(Environment.GetEnvironmentVariable(ScheduledEventsConstants.VMOutOfRotationEnvVariable, EnvironmentVariableTarget.Machine), bool.TrueString, StringComparison.OrdinalIgnoreCase);
      if (!flag)
        flag = string.Equals(Environment.GetEnvironmentVariable("VssDisableHealthEndpoint", EnvironmentVariableTarget.Machine), bool.TrueString, StringComparison.OrdinalIgnoreCase);
      return flag;
    }

    internal HealthData InitializeAndGetHealthData(
      IVssRequestContext requestContext,
      out bool isHealthy)
    {
      bool flag1 = false;
      try
      {
        flag1 = requestContext.GetService<VssSiteWarmUpService>().CheckAndStartWarmup(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.Trace(10025015, TraceLevel.Warning, "Health", "Service", "Call to warmup failed.");
      }
      this.CheckApplicationInstanceType(requestContext);
      BuildInfo[] buildInfo = BuildInfoReader.GetBuildInfo();
      HealthData healthData = new HealthData()
      {
        TimeStamp = DateTime.UtcNow,
        DeploymentId = this.m_deploymentId,
        DeploymentSlot = this.m_deploymentSlot,
        Build = buildInfo,
        DataServiceLevel = this.m_configDbServiceLevel == ServiceLevel.MaxServiceLevel ? (string) null : this.m_configDbServiceLevel?.ToString(),
        Pid = this.m_pid
      };
      string userAgent = requestContext.UserAgent;
      if (string.Equals(userAgent, "Edge Health Probe", StringComparison.Ordinal))
      {
        isHealthy = true;
      }
      else
      {
        isHealthy = flag1 && this.IsConfigDbHealthy() && !HealthService.IsDisabledByHealthAgent() && !this.IsDatabaseServiceLevelAhead(requestContext);
        if (isHealthy && this.m_isVMSS && string.Equals(userAgent, "Load Balancer Agent", StringComparison.Ordinal))
        {
          IHostedTenantService service = requestContext.GetService<IHostedTenantService>();
          bool flag2 = string.Equals(this.m_deploymentId, service.GetProductionDeploymentId(requestContext), StringComparison.OrdinalIgnoreCase);
          string environmentVariable = Environment.GetEnvironmentVariable("MONITORING_ROLE_DEPLOYMENTSLOT", EnvironmentVariableTarget.Machine);
          bool flag3 = string.Equals(environmentVariable, "Production");
          if (flag2 != flag3)
          {
            flag2 = string.Equals(service.DeploymentId, service.GetProductionDeploymentIdFromDatabase(requestContext), StringComparison.OrdinalIgnoreCase);
            if (flag2 == flag3)
            {
              if (!this.m_slotMissmatchStopwatch.IsRunning)
                this.m_slotMissmatchStopwatch.Restart();
              if (this.m_slotMissmatchStopwatch.Elapsed > HealthService.s_1minutes)
                requestContext.Trace(10025019, TraceLevel.Error, "Health", "Service", "MONITORING_ROLE_DEPLOYMENTSLOT env variable is set to {0}, but hostedTenantService.GetProductionDeploymentId() returned stale value {1}. Elapsed time in seconds: {2:0.00}", (object) environmentVariable, (object) !flag2, (object) this.m_slotMissmatchStopwatch.Elapsed.TotalSeconds);
            }
            else
            {
              if (!this.m_slotMissmatchStopwatch.IsRunning)
                this.m_slotMissmatchStopwatch.Restart();
              if (this.m_slotMissmatchStopwatch.Elapsed > HealthService.s_1minutes)
                requestContext.Trace(10025020, TraceLevel.Error, "Health", "Service", "MONITORING_ROLE_DEPLOYMENTSLOT env variable is set to {0}, which is a stale value. Elapsed time in seconds: {1:0.00}", (object) environmentVariable, (object) this.m_slotMissmatchStopwatch.Elapsed.TotalSeconds);
            }
          }
          else
            this.m_slotMissmatchStopwatch.Reset();
          if (flag2)
          {
            if (this.m_deploymentSlotState != HealthService.DeploymentSlotState.Online)
            {
              requestContext.TraceAlways(10025012, TraceLevel.Info, "Health", "Service", "HealthService: Transitioning from {0} to Online", (object) this.m_deploymentSlotState);
              this.m_deploymentSlotState = HealthService.DeploymentSlotState.Online;
            }
          }
          else if (this.m_deploymentSlotState < HealthService.DeploymentSlotState.Offline)
          {
            if (requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.GoingOfflineTimeInSeconds, FrameworkServerConstants.GoingOfflineTimeInSecondsDefaultValue) > 0)
            {
              this.m_deploymentSlotState = this.TransitionToOffline(requestContext, this.m_deploymentSlotState);
            }
            else
            {
              requestContext.TraceAlways(10025013, TraceLevel.Info, "Health", "Service", "HealthService: Transitioning from {0} to {1}", (object) this.m_deploymentSlotState, (object) (this.m_deploymentSlotState + 1));
              ++this.m_deploymentSlotState;
            }
          }
          isHealthy = this.m_deploymentSlotState < HealthService.DeploymentSlotState.Offline;
        }
      }
      return healthData;
    }

    internal HealthService.DeploymentSlotState TransitionToOffline(
      IVssRequestContext requestContext,
      HealthService.DeploymentSlotState deploymentSlotState)
    {
      if (deploymentSlotState == HealthService.DeploymentSlotState.Online)
      {
        requestContext.TraceAlways(10025013, TraceLevel.Info, "Health", "Service", string.Format("HealthService: Transitioning from {0} to {1}", (object) deploymentSlotState, (object) HealthService.DeploymentSlotState.GoingOffline));
        return HealthService.DeploymentSlotState.GoingOffline;
      }
      if (!requestContext.GetService<IHostedTenantService>().IsGoingOfflineTimeElapsed(requestContext))
        return deploymentSlotState;
      requestContext.TraceAlways(10025013, TraceLevel.Info, "Health", "Service", string.Format("HealthService: Transitioning from {0} to {1}", (object) deploymentSlotState, (object) HealthService.DeploymentSlotState.Offline));
      return HealthService.DeploymentSlotState.Offline;
    }

    internal string GetAfdOutgoingQualityOfResponse(IVssRequestContext requestContext)
    {
      int? nullable = requestContext.GetService<IVssRegistryService>().GetValue<int?>(requestContext, in HealthService.s_afdHealthCheckQuality, new int?());
      if (!nullable.HasValue)
        return (string) null;
      return nullable?.ToString();
    }

    internal void UpdateConfigDbHealthState(IVssRequestContext requestContext, object taskArgs)
    {
      if (!requestContext.ServiceHost.HasDatabaseAccess)
      {
        lock (this.m_lockConfigDbHealth)
          this.m_configDbHealthy = true;
      }
      else
      {
        try
        {
          bool flag = requestContext.GetService<ICachedRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Configuration/Health/ProcessRequestsConfigDbUnavailable", false);
          ServiceLevel databaseServiceLevel = this.GetDatabaseServiceLevel(requestContext);
          lock (this.m_lockConfigDbHealth)
          {
            this.m_configDbHealthy = true;
            this.m_continueProcessRequests = flag;
            this.m_configDbServiceLevel = databaseServiceLevel;
          }
        }
        catch (DatabaseConnectionException ex)
        {
          lock (this.m_lockConfigDbHealth)
            this.m_configDbHealthy = false;
          requestContext.Trace(10025016, TraceLevel.Error, "Health", "Service", "Error connecting to configuration database and obtaining service level. Database may be unhealthy.");
        }
        catch (ServiceLevelException ex)
        {
          lock (this.m_lockConfigDbHealth)
            this.m_configDbServiceLevel = ServiceLevel.MaxServiceLevel;
          requestContext.TraceException(10025017, "Health", "Service", (Exception) ex);
          requestContext.Trace(10025016, TraceLevel.Error, "Health", "Service", "ServiceLevelException caught obtaining service level. Service level is unknown, but not healthy.");
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10025018, "Health", "Service", ex);
        }
      }
    }

    internal bool IsConfigDbHealthy() => this.m_configDbHealthy || this.m_continueProcessRequests;

    internal bool IsDatabaseServiceLevelAhead(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.HasDatabaseAccess)
        return false;
      ServiceLevel configDbServiceLevel = this.m_configDbServiceLevel;
      ServiceLevel currentServiceLevel = TeamFoundationSqlResourceComponent.CurrentServiceLevel;
      int num = configDbServiceLevel > currentServiceLevel ? 1 : 0;
      if (num == 0)
        return num != 0;
      requestContext.Trace(10025011, TraceLevel.Error, "Health", "Service", "Database service level is ahead of binaries service level and this is unsupported!" + string.Format("DB ServiceLevel = {0}; Binaries ServiceLevel = {1}", (object) configDbServiceLevel, (object) currentServiceLevel));
      return num != 0;
    }

    private ServiceLevel GetDatabaseServiceLevel(IVssRequestContext requestContext)
    {
      ITeamFoundationDatabaseManagementService service = requestContext.GetService<ITeamFoundationDatabaseManagementService>();
      bool flag = DateTime.UtcNow - this.m_lastDatabasePropertyRead < HealthService.s_databaseCacheExpiration;
      IVssRequestContext requestContext1 = requestContext;
      int databaseId = requestContext.ServiceHost.ServiceHostInternal().DatabaseId;
      int num = flag ? 1 : 0;
      ITeamFoundationDatabaseProperties database = service.GetDatabase(requestContext1, databaseId, num != 0);
      if (!flag)
        this.m_lastDatabasePropertyRead = DateTime.UtcNow;
      return new ServiceLevel(database.ServiceLevel.Split(ServiceLevel.ServiceLevelSeparator)[0]);
    }

    private void CheckApplicationInstanceType(IVssRequestContext requestContext)
    {
      HttpApplication applicationInstance = HttpContext.Current.ApplicationInstance;
      if (!(applicationInstance is VisualStudioServicesApplication))
      {
        if (this.m_httpAppErrorReported)
          return;
        requestContext.Trace(10025010, TraceLevel.Error, "Health", "Service", "The HttpApplication object is not derived from VisualStudioServicesApplication. Type: {0}, BaseType: {1}", (object) applicationInstance.GetType().FullName, (object) applicationInstance.GetType().BaseType.FullName);
        this.m_httpAppErrorReported = true;
      }
      else
      {
        if (!this.m_httpAppErrorReported)
          return;
        requestContext.TraceAlways(10025014, TraceLevel.Info, "Health", "Service", "The HttpApplication was not derived from VisualStudioServicesApplication, but now it is. Type: {0}, BaseType: {1}", (object) applicationInstance.GetType().FullName, (object) applicationInstance.GetType().BaseType.FullName);
        this.m_httpAppErrorReported = false;
      }
    }

    internal enum DeploymentSlotState
    {
      Online,
      GoingOffline,
      Offline,
    }
  }
}
