// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.CloseConnectionInStagingSlotFilter
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Health;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class CloseConnectionInStagingSlotFilter : ITeamFoundationRequestFilter
  {
    private string m_malformedExpirationTime;
    private int m_requestCounter;
    private protected Stopwatch m_stagingStopwatch;
    private readonly VssRefreshCache<CloseConnectionInStagingSlotFilter.CloseConnectionSettings> m_closeConnectionSettings;
    private readonly ConcurrentDictionary<IPEndPoint, Stopwatch> m_connections = new ConcurrentDictionary<IPEndPoint, Stopwatch>();
    private readonly Task m_cleanupTask;
    private readonly CancellationTokenSource m_cleanupTaskCancellationTokenSource = new CancellationTokenSource();
    private static readonly ThreadLocal<Random> s_swapRandom = new ThreadLocal<Random>((Func<Random>) (() => new Random()));
    private static readonly VssPerformanceCounter s_closedConnectionsCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.ConnectionTracking.ClosedConnectionsPerSecond");
    private static readonly VssPerformanceCounter s_openedConnectionsCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.ConnectionTracking.OpenedConnectionsPerSecond");
    private static readonly TimeSpan s_closeConnectionCheckInterval = TimeSpan.FromSeconds(5.0);
    private static readonly TimeSpan s_connectionCleanupInterval = TimeSpan.FromMinutes(5.0);
    private const string c_area = "CloseConnectionInStagingSlotFilter";
    private const string c_layer = "CloseConnectionInStagingSlotFilter";
    private const string HealthAgentRegistryKey = "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\TeamFoundationServer\\VssHealthAgent";

    public CloseConnectionInStagingSlotFilter()
    {
      this.m_closeConnectionSettings = new VssRefreshCache<CloseConnectionInStagingSlotFilter.CloseConnectionSettings>(CloseConnectionInStagingSlotFilter.s_closeConnectionCheckInterval, new Func<IVssRequestContext, CloseConnectionInStagingSlotFilter.CloseConnectionSettings>(this.GetCloseConnectionSettings), true);
      this.m_cleanupTask = Task.Run((Action) (() => CloseConnectionInStagingSlotFilter.CleanupConnectionsTaskAsync(this.m_cleanupTaskCancellationTokenSource.Token, this.GetConnectionCleanupTaskDelay(), this.GetConnectionExpirationLength(), this.m_connections)), this.m_cleanupTaskCancellationTokenSource.Token);
    }

    ~CloseConnectionInStagingSlotFilter() => this.m_cleanupTaskCancellationTokenSource.Cancel();

    void ITeamFoundationRequestFilter.BeginRequest(IVssRequestContext requestContext)
    {
      HttpContextBase httpContext = requestContext.WebRequestContextInternal().HttpContext;
      CloseConnectionInStagingSlotFilter.CloseConnectionSettings settings = this.GetSettings(requestContext);
      bool vmIsDisabled = settings.VMIsDisabled;
      if (!(!settings.VipSwapping ? ((vmIsDisabled ? 1 : 0) | (this.ShouldCloseConnectionForPersistentResets(settings) ? 1 : (this.ShouldCloseConnectionForConnectionLength(settings, httpContext) ? 1 : 0))) != 0 : vmIsDisabled | this.ShouldCloseConnectionForVipSwap(settings)))
        return;
      CloseConnectionInStagingSlotFilter.s_closedConnectionsCounter.Increment();
      httpContext.Response.Headers.Add("Connection", "close");
    }

    public Task BeginRequestAsync(IVssRequestContext requestContext) => Task.CompletedTask;

    private bool ShouldCloseConnectionForVipSwap(
      CloseConnectionInStagingSlotFilter.CloseConnectionSettings settings)
    {
      Stopwatch stagingStopwatch = this.m_stagingStopwatch;
      if (!settings.VipSwapping || stagingStopwatch == null)
        return false;
      double elapsedPercentage = this.GetVipSwapElapsedPercentage(stagingStopwatch, settings);
      double num1;
      if (settings.VipSwapExponentiationFactor.HasValue)
      {
        double? exponentiationFactor = settings.VipSwapExponentiationFactor;
        double num2 = 1.0;
        if (exponentiationFactor.GetValueOrDefault() > num2 & exponentiationFactor.HasValue)
        {
          exponentiationFactor = settings.VipSwapExponentiationFactor;
          double num3 = Math.Pow(exponentiationFactor.Value, elapsedPercentage) - 1.0;
          exponentiationFactor = settings.VipSwapExponentiationFactor;
          double num4 = exponentiationFactor.Value - 1.0;
          num1 = num3 / num4;
          goto label_6;
        }
      }
      num1 = elapsedPercentage;
label_6:
      return this.GetRandom().NextDouble() < num1;
    }

    private bool ShouldCloseConnectionForPersistentResets(
      CloseConnectionInStagingSlotFilter.CloseConnectionSettings settings)
    {
      int num = Interlocked.Increment(ref this.m_requestCounter);
      return settings.CloseHttpConnectionInterval >= 20 && num % settings.CloseHttpConnectionInterval == 0;
    }

    private bool ShouldCloseConnectionForConnectionLength(
      CloseConnectionInStagingSlotFilter.CloseConnectionSettings settings,
      HttpContextBase httpContext)
    {
      IPAddress address;
      int result;
      if (!IPAddress.TryParse(httpContext.Request.UserHostAddress, out address) || !int.TryParse(httpContext.Request.ServerVariables["REMOTE_PORT"], out result))
        return false;
      IPEndPoint key = new IPEndPoint(address, result);
      Stopwatch orAdd = this.m_connections.GetOrAdd(key, (Func<IPEndPoint, Stopwatch>) (_ =>
      {
        CloseConnectionInStagingSlotFilter.s_openedConnectionsCounter.Increment();
        return Stopwatch.StartNew();
      }));
      double num = 1.1 - this.GetRandom().NextDouble() * 0.2;
      TimeSpan timeSpan = TimeSpan.FromTicks((long) ((double) settings.MaxConnectionTime.Ticks * num));
      if (!(orAdd.Elapsed > timeSpan))
        return false;
      this.m_connections.TryRemove(key, out Stopwatch _);
      return true;
    }

    private static async void CleanupConnectionsTaskAsync(
      CancellationToken cancellationToken,
      TimeSpan delay,
      TimeSpan connectionExpirationLength,
      ConcurrentDictionary<IPEndPoint, Stopwatch> connections)
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        try
        {
          await Task.Delay(delay, cancellationToken);
          CloseConnectionInStagingSlotFilter.RemoveExpiredConnections(connectionExpirationLength, connections);
        }
        catch (OperationCanceledException ex)
        {
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(1030028158, nameof (CloseConnectionInStagingSlotFilter), nameof (CloseConnectionInStagingSlotFilter), ex);
        }
      }
    }

    internal static void RemoveExpiredConnections(
      TimeSpan connectionExpirationLength,
      ConcurrentDictionary<IPEndPoint, Stopwatch> connections)
    {
      foreach (KeyValuePair<IPEndPoint, Stopwatch> connection in connections)
      {
        if (connection.Value.Elapsed > connectionExpirationLength)
          connections.TryRemove(connection.Key, out Stopwatch _);
      }
    }

    private CloseConnectionInStagingSlotFilter.CloseConnectionSettings GetCloseConnectionSettings(
      IVssRequestContext requestContext)
    {
      try
      {
        requestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        int num1 = requestContext.GetService<IHostedTenantService>().CloseConnectionsForVipSwap(requestContext) ? 1 : 0;
        if (num1 != 0)
        {
          if (this.m_stagingStopwatch == null)
            this.m_stagingStopwatch = Stopwatch.StartNew();
        }
        else
          this.m_stagingStopwatch = (Stopwatch) null;
        IVssRegistryService registryService1 = service;
        IVssRequestContext requestContext1 = requestContext;
        RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.VipSwapTimeInSeconds;
        ref RegistryQuery local1 = ref registryQuery;
        int secondsDefaultValue1 = FrameworkServerConstants.VipSwapTimeInSecondsDefaultValue;
        int secondsDefaultValue2 = registryService1.GetValue<int>(requestContext1, in local1, secondsDefaultValue1);
        if (secondsDefaultValue2 > FrameworkServerConstants.VipSwapTimeInSecondsDefaultValue)
        {
          IVssRegistryService registryService2 = service;
          IVssRequestContext requestContext2 = requestContext;
          registryQuery = (RegistryQuery) FrameworkServerConstants.PerformingVipSwapBack;
          ref RegistryQuery local2 = ref registryQuery;
          if (registryService2.GetValue<bool>(requestContext2, in local2, false))
            secondsDefaultValue2 = FrameworkServerConstants.VipSwapTimeInSecondsDefaultValue;
        }
        IVssRegistryService registryService3 = service;
        IVssRequestContext requestContext3 = requestContext;
        registryQuery = (RegistryQuery) FrameworkServerConstants.VipSwapExponentiationFactor;
        ref RegistryQuery local3 = ref registryQuery;
        double? defaultValue = new double?(40.0);
        double? vipSwapExponentiation = registryService3.GetValue<double?>(requestContext3, in local3, defaultValue);
        bool vmIsDisabled = HealthService.IsDisabledByHealthAgent();
        int connectionInterval = this.GetCloseConnectionInterval(requestContext);
        IVssRegistryService registryService4 = service;
        IVssRequestContext requestContext4 = requestContext;
        registryQuery = (RegistryQuery) FrameworkServerConstants.MaxConnectionTimeInSeconds;
        ref RegistryQuery local4 = ref registryQuery;
        int num2 = registryService4.GetValue<int>(requestContext4, in local4, 300);
        return new CloseConnectionInStagingSlotFilter.CloseConnectionSettings(num1 != 0, secondsDefaultValue2, vipSwapExponentiation, vmIsDisabled, connectionInterval, TimeSpan.FromSeconds((double) num2));
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(1030028157, TraceLevel.Error, nameof (CloseConnectionInStagingSlotFilter), nameof (CloseConnectionInStagingSlotFilter), ex);
        return CloseConnectionInStagingSlotFilter.CloseConnectionSettings.Defaults;
      }
    }

    private int GetCloseConnectionInterval(IVssRequestContext deploymentContext)
    {
      deploymentContext.CheckDeploymentRequestContext();
      IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
      connectionInterval = service.GetValue<int>(deploymentContext, (RegistryQuery) FrameworkServerConstants.CloseHttpConnectionInterval, 0);
      string str = service.GetValue<string>(deploymentContext, (RegistryQuery) FrameworkServerConstants.CloseHttpConnectionExpirationTime, false, (string) null);
      if (!string.IsNullOrEmpty(str))
      {
        DateTime result;
        if (!DateTime.TryParse(str, out result))
        {
          if (!string.Equals(this.m_malformedExpirationTime, str, StringComparison.Ordinal))
          {
            service.SetValue(deploymentContext, FrameworkServerConstants.CloseHttpConnectionExpirationTime, (object) null);
            deploymentContext.Trace(570111, TraceLevel.Error, nameof (CloseConnectionInStagingSlotFilter), nameof (CloseConnectionInStagingSlotFilter), "'{0}' is not a well-formed DateTime. Update {1} registry value. Close connection feature is disabled.", (object) str, (object) FrameworkServerConstants.CloseHttpConnectionExpirationTime);
            this.m_malformedExpirationTime = str;
          }
          connectionInterval = 0;
        }
        else if (result < DateTime.UtcNow)
          connectionInterval = 0;
      }
      if (connectionInterval != 0 || !(Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\TeamFoundationServer\\VssHealthAgent", "ConnectionRebalancingInterval", (object) 0) is int connectionInterval))
        ;
      if (connectionInterval == 0)
        connectionInterval = service.GetValue<int>(deploymentContext, (RegistryQuery) FrameworkServerConstants.PersistentCloseHttpConnectionInterval, 0);
      return connectionInterval;
    }

    internal ConcurrentDictionary<IPEndPoint, Stopwatch> GetConnections() => this.m_connections;

    internal virtual Random GetRandom() => CloseConnectionInStagingSlotFilter.s_swapRandom.Value;

    internal virtual CloseConnectionInStagingSlotFilter.CloseConnectionSettings GetSettings(
      IVssRequestContext requestContext)
    {
      return this.m_closeConnectionSettings.Get(requestContext);
    }

    internal virtual double GetVipSwapElapsedPercentage(
      Stopwatch stagingStopwatch,
      CloseConnectionInStagingSlotFilter.CloseConnectionSettings settings)
    {
      return (double) stagingStopwatch.ElapsedMilliseconds / 1000.0 / (double) settings.VipSwapTimeInSeconds;
    }

    internal virtual TimeSpan GetConnectionCleanupTaskDelay() => CloseConnectionInStagingSlotFilter.s_connectionCleanupInterval;

    internal virtual TimeSpan GetConnectionExpirationLength() => CloseConnectionInStagingSlotFilter.CloseConnectionSettings.Defaults.MaxConnectionTime + CloseConnectionInStagingSlotFilter.CloseConnectionSettings.Defaults.MaxConnectionTime;

    Task ITeamFoundationRequestFilter.PostAuthenticateRequest(IVssRequestContext requestContext) => (Task) Task.FromResult<int>(0);

    void ITeamFoundationRequestFilter.EnterMethod(IVssRequestContext requestContext)
    {
    }

    void ITeamFoundationRequestFilter.LeaveMethod(IVssRequestContext requestContext)
    {
    }

    Task ITeamFoundationRequestFilter.PostLogRequestAsync(IVssRequestContext requestContext) => (Task) Task.FromResult<int>(0);

    void ITeamFoundationRequestFilter.EndRequest(IVssRequestContext requestContext)
    {
    }

    public void PostAuthorizeRequest(IVssRequestContext requestContext)
    {
    }

    internal sealed class CloseConnectionSettings
    {
      public static readonly CloseConnectionInStagingSlotFilter.CloseConnectionSettings Defaults = new CloseConnectionInStagingSlotFilter.CloseConnectionSettings(false, 300, new double?(0.0), false, 0, TimeSpan.FromSeconds(300.0));

      public CloseConnectionSettings(
        bool vipSwapping,
        int vipSwapSeconds,
        double? vipSwapExponentiation,
        bool vmIsDisabled,
        int closeConnectionInterval,
        TimeSpan maxConnectionTime)
      {
        this.VipSwapping = vipSwapping;
        this.VipSwapTimeInSeconds = vipSwapSeconds;
        this.VipSwapExponentiationFactor = vipSwapExponentiation;
        this.VMIsDisabled = vmIsDisabled;
        this.CloseHttpConnectionInterval = closeConnectionInterval;
        this.MaxConnectionTime = maxConnectionTime;
      }

      public bool VipSwapping { get; }

      public int VipSwapTimeInSeconds { get; }

      public double? VipSwapExponentiationFactor { get; }

      public bool VMIsDisabled { get; }

      public int CloseHttpConnectionInterval { get; }

      public TimeSpan MaxConnectionTime { get; }
    }
  }
}
