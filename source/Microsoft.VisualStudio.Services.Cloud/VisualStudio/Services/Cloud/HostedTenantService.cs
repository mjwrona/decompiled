// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostedTenantService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostedTenantService : IHostedTenantService, IVssFrameworkService
  {
    private string m_deploymentId;
    private string m_tenantName;
    private string m_productionDeploymentId;
    private bool m_retireProductionSlot;
    private bool m_takeMachineOutRotation;
    private RegistryQuery m_registryQuery;
    private IVssDateTimeProvider m_dateTimeProvider;
    private bool m_isJobAgent;
    private readonly Stopwatch m_stopwatch = new Stopwatch();
    private readonly TimeSpan s_checkInterval = TimeSpan.FromSeconds(3.0);
    private static readonly string s_area = nameof (HostedTenantService);
    private static readonly string s_layer = nameof (HostedTenantService);

    public HostedTenantService()
      : this(VssDateTimeProvider.DefaultProvider)
    {
    }

    public HostedTenantService(IVssDateTimeProvider dateTimeProvider)
    {
      string processName = Process.GetCurrentProcess().ProcessName;
      this.m_isJobAgent = string.Equals(processName, "tfsjobagent", StringComparison.OrdinalIgnoreCase) || string.Equals(processName, "waworkerhost", StringComparison.OrdinalIgnoreCase);
      this.m_dateTimeProvider = dateTimeProvider;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.CheckHostedDeployment();
      this.m_deploymentId = AzureRoleUtil.Environment.DeploymentId;
      this.m_tenantName = ConfigurationManager.AppSettings["tenantName"];
      this.m_registryQuery = new RegistryQuery(FrameworkServerConstants.TenantAwareRootRegistryPath(this.m_tenantName) + "/...");
      this.GetProductionDeploymentId(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string DeploymentId => this.m_deploymentId;

    public string TenantName => this.m_tenantName;

    public string GetProductionDeploymentId(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      this.EnsureDataMembersUpdated(requestContext);
      return this.m_productionDeploymentId;
    }

    public string GetProductionDeploymentIdFromDatabase(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      return requestContext.GetService<ISqlRegistryService>().GetValue(requestContext, (RegistryQuery) FrameworkServerConstants.TenantAwareDeploymentIdRegistryPath(this.m_tenantName), (string) null);
    }

    private bool IsProductionDeployment(IVssRequestContext requestContext) => string.Equals(this.m_deploymentId, this.GetProductionDeploymentId(requestContext), StringComparison.OrdinalIgnoreCase);

    public bool CloseConnectionsForVipSwap(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.GoingOfflineTimeInSeconds, FrameworkServerConstants.GoingOfflineTimeInSecondsDefaultValue) > 0 ? (this.m_retireProductionSlot || !this.IsProductionDeployment(requestContext)) && this.IsGoingOfflineTimeElapsed(requestContext) : this.m_retireProductionSlot || !this.IsProductionDeployment(requestContext);

    public bool CloseAllConnections(IVssRequestContext requestContext) => this.m_takeMachineOutRotation;

    public bool IsGoingOfflineTimeElapsed(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int num = service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.GoingOfflineTimeInSeconds, FrameworkServerConstants.GoingOfflineTimeInSecondsDefaultValue);
      if (service.GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.PerformingVipSwapBack, false))
        num = 0;
      string s = service.GetValue(requestContext, (RegistryQuery) FrameworkServerConstants.VipSwapTimeStamp, DateTime.MinValue.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      DateTime result;
      if (DateTime.TryParse(s, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
        return this.m_dateTimeProvider.UtcNow >= result.AddSeconds((double) num);
      requestContext.TraceAlways(1709528772, TraceLevel.Error, HostedTenantService.s_area, HostedTenantService.s_layer, "Unable to parse VipSwapTimeStamp: " + s + ", going offline time is ignored.");
      return true;
    }

    public bool ShouldJobAgentAquireJobs(IVssRequestContext requestContext)
    {
      bool flag1 = this.IsProductionDeployment(requestContext);
      bool flag2 = !this.m_retireProductionSlot & flag1 && !this.m_takeMachineOutRotation;
      if (!flag2)
      {
        if (flag1)
          requestContext.TraceAlways(1709528770, TraceLevel.Verbose, HostedTenantService.s_area, HostedTenantService.s_layer, string.Format("Job agent should not acquire jobs. RetireProductionSlot : {0}, IsProductionDeployment : {1}, TakeMachineOutRotation : {2} ", (object) this.m_retireProductionSlot, (object) flag1, (object) this.m_takeMachineOutRotation));
        else
          requestContext.Trace(1709528771, TraceLevel.Verbose, HostedTenantService.s_area, HostedTenantService.s_layer, string.Format("Job agent should not acquire jobs. RetireProductionSlot : {0}, IsProductionDeployment : {1}, TakeMachineOutRotation : {2} ", (object) this.m_retireProductionSlot, (object) flag1, (object) this.m_takeMachineOutRotation));
      }
      if (flag2)
      {
        IGeoReplicationService service = requestContext.GetService<IGeoReplicationService>();
        if (service.GetGeoReplicationMode(requestContext) == GeoReplicationMode.All)
          flag2 = service.IsPrimaryInstance(requestContext);
        if (!flag2)
          requestContext.TraceAlways(485412566, TraceLevel.Verbose, HostedTenantService.s_area, HostedTenantService.s_layer, "Job agent should not acquire jobs because this SU is fully replicated and is not primary");
      }
      return flag2;
    }

    private void EnsureDataMembersUpdated(IVssRequestContext requestContext)
    {
      if (this.m_stopwatch.IsRunning && !(this.m_stopwatch.Elapsed > this.s_checkInterval))
        return;
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, this.m_registryQuery);
      this.m_productionDeploymentId = registryEntryCollection[FrameworkServerConstants.TenantAwareDeploymentIdRegistryPath(this.m_tenantName)]?.Value;
      this.m_retireProductionSlot = string.Equals(registryEntryCollection[FrameworkServerConstants.RetireProductionSlot(this.m_tenantName)]?.Value, bool.TrueString, StringComparison.OrdinalIgnoreCase);
      this.m_takeMachineOutRotation = string.Equals(Environment.GetEnvironmentVariable(ScheduledEventsConstants.VMOutOfRotationEnvVariable, EnvironmentVariableTarget.Machine), bool.TrueString, StringComparison.OrdinalIgnoreCase);
      if (!this.m_takeMachineOutRotation)
        this.m_takeMachineOutRotation = string.Equals(Environment.GetEnvironmentVariable("VSSHEALTHAGENT_TAKE_VM_OUT_OF_ROTATION", EnvironmentVariableTarget.Machine), bool.TrueString, StringComparison.OrdinalIgnoreCase);
      if (!this.m_takeMachineOutRotation)
        this.m_takeMachineOutRotation = string.Equals(Environment.GetEnvironmentVariable("VSSHEALTHAGENT_TAKE_VM_OUT_OF_ROTATION_HIGH_CPU", EnvironmentVariableTarget.Machine), bool.TrueString, StringComparison.OrdinalIgnoreCase);
      if (!this.m_takeMachineOutRotation)
        this.m_takeMachineOutRotation = string.Equals(Environment.GetEnvironmentVariable("VssDisableHealthEndpoint", EnvironmentVariableTarget.Machine), bool.TrueString, StringComparison.OrdinalIgnoreCase);
      if (this.m_isJobAgent)
        return;
      this.m_stopwatch.Restart();
    }
  }
}
