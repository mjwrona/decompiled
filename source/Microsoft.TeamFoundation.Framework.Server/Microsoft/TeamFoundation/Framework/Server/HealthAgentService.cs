// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HealthAgentService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class HealthAgentService : IHealthAgentService, IVssFrameworkService
  {
    private HealthAgentServiceSettings m_settings;
    private Timer m_heartbeatTimer;
    private HealthAgentClient m_client;
    private int m_processId;
    private CommandSetter m_circuitBreakerSettings;
    private const string c_traceArea = "Microsoft.TeamFoundation.Framework.Server";
    private const string c_traceLayer = "HealthAgentService";

    public static bool IsEnabled(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      if (!requestContext.ExecutionEnvironment.IsCloudDeployment && Environment.GetEnvironmentVariable("ATTACH_DEBUGGER_VssHealthAgent") == null)
        return false;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Framework.HealthAgentService.ForceEnable"))
        return true;
      return requestContext.IsFeatureEnabled("VisualStudio.Services.Framework.HealthAgentService.Enable") && ((IEnumerable<ServiceController>) ServiceController.GetServices()).Any<ServiceController>((Func<ServiceController, bool>) (s => s.ServiceName == "VssHealthAgent"));
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!HealthAgentService.IsEnabled(systemRequestContext))
        return;
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.CheckSystemRequestContext();
      this.m_processId = Process.GetCurrentProcess().Id;
      this.m_heartbeatTimer = new Timer(new TimerCallback(this.HeartbeatTimer_Callback), (object) null, -1, -1);
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), in HealthAgentServiceSettings.RegistryPath);
      if (Interlocked.CompareExchange<HealthAgentServiceSettings>(ref this.m_settings, HealthAgentServiceSettings.Load(systemRequestContext), (HealthAgentServiceSettings) null) != null)
        return;
      this.UpdateSettings();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));
      if (this.m_heartbeatTimer != null)
      {
        using (ManualResetEvent notifyObject = new ManualResetEvent(false))
        {
          if (this.m_heartbeatTimer.Dispose((WaitHandle) notifyObject))
            notifyObject.WaitOne(TimeSpan.FromSeconds(5.0));
          this.m_heartbeatTimer = (Timer) null;
        }
      }
      if (this.m_circuitBreakerSettings != null)
        this.ClientCall((Action<HealthAgentClient>) (client => client.ResetSession()));
      this.m_client?.Dispose();
      this.m_client = (HealthAgentClient) null;
    }

    public void RequestReset(IVssRequestContext requestContext, string reason)
    {
      if (this.m_heartbeatTimer == null)
      {
        requestContext.Trace(3763060, TraceLevel.Info, "Microsoft.TeamFoundation.Framework.Server", nameof (HealthAgentService), "Reset requested when service was not running. Reason: " + reason + ".");
      }
      else
      {
        try
        {
          requestContext.CheckDeploymentRequestContext();
          requestContext.CheckSystemRequestContext();
          requestContext.TraceAlways(43754467, TraceLevel.Warning, "Microsoft.TeamFoundation.Framework.Server", nameof (HealthAgentService), "Reset requested for the current process: " + reason + ".");
          this.ClientCall((Action<HealthAgentClient>) (client => client.RequestReset(this.m_processId)));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(60030027, "Microsoft.TeamFoundation.Framework.Server", nameof (HealthAgentService), ex);
        }
      }
    }

    private void HeartbeatTimer_Callback(object state)
    {
      TeamFoundationTracingService.TraceRaw(52068688, TraceLevel.Info, nameof (HealthAgentService), nameof (HealthAgentService), "Sending HealthAgent hearbeat.");
      this.ClientCall((Action<HealthAgentClient>) (client => client.SendHeartbeat(this.m_processId)));
    }

    private void ClientCall(Action<HealthAgentClient> method)
    {
      Microsoft.VisualStudio.Services.CircuitBreaker.Command command = new Microsoft.VisualStudio.Services.CircuitBreaker.Command(this.m_circuitBreakerSettings, (Action) (() =>
      {
        if (this.m_client == null || this.m_client.State != CommunicationState.Opened)
        {
          this.m_client?.Dispose();
          this.m_client = new HealthAgentClient();
          this.m_client.Open();
        }
        method(this.m_client);
      }));
      try
      {
        command.Execute();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(921272959, "Microsoft.TeamFoundation.Framework.Server", nameof (HealthAgentService), ex);
      }
    }

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      try
      {
        this.m_settings = HealthAgentServiceSettings.Load(requestContext);
        this.UpdateSettings();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(922061941, "Microsoft.TeamFoundation.Framework.Server", nameof (HealthAgentService), ex);
      }
    }

    private void UpdateSettings()
    {
      this.m_circuitBreakerSettings = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) "Microsoft.TeamFoundation.Framework.Server.HealthAgentService").AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionMaxConcurrentRequests(1).WithExecutionTimeout(this.m_settings.CircuitBreakerTimeout).WithCircuitBreakerMinBackoff(this.m_settings.CircuitBreakerMinBackoff).WithCircuitBreakerMaxBackoff(this.m_settings.CircuitBreakerMaxBackoff).WithCircuitBreakerErrorThresholdPercentage(this.m_settings.CircuitBreakerErrorThresholdPercentage).WithMetricsRollingStatisticalWindow(this.m_settings.CircuitBreakerStatisticalWindow));
      if (this.m_settings.HeartbeatInterval > TimeSpan.Zero)
      {
        this.m_heartbeatTimer.Change(TimeSpan.Zero, this.m_settings.HeartbeatInterval);
      }
      else
      {
        this.m_heartbeatTimer.Change(-1, -1);
        this.ClientCall((Action<HealthAgentClient>) (client => client.ResetSession()));
      }
    }

    public static class FeatureFlags
    {
      public const string Enable = "VisualStudio.Services.Framework.HealthAgentService.Enable";
      public const string ForceEnable = "VisualStudio.Services.Framework.HealthAgentService.ForceEnable";
      public const string ResetOnJobCancellationTimeout = "VisualStudio.Services.Framework.HealthAgentService.ResetOnJobCancellationTimeout";
    }
  }
}
