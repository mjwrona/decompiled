// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DeploymentServiceHost : 
    VssServiceHost,
    IVssDeploymentServiceHost,
    IVssServiceHost,
    IVssServiceHostControl,
    IDisposable,
    IVssServiceHostProperties,
    IDeploymentServiceHostInternal
  {
    private readonly string m_constructorCallStack;
    private string m_constructorConnectionString;
    private SemaphoreRemoteTask m_gcTask;
    private TeamFoundationHostManagementService m_hostManagement;
    private DefaultRequestContext m_systemRequestContext;
    private Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.TeamFoundationMetabases m_metabases;
    private Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.DeploymentServiceHostSettings m_settings;
    private readonly DeploymentServiceHostOptions m_options;
    private object m_settingsLock = new object();
    private const string c_area = "DeploymentServiceHost";
    private const string c_layer = "HostManagement";
    private static readonly Dictionary<string, Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost> s_deploymentHosts = new Dictionary<string, Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost>();
    private ITeamFoundationDatabaseProperties m_databaseProperties;
    private TeamFoundationSqlNotificationService m_sqlNotificationService;
    private TeamFoundationTracingService m_tracingService;
    private ISqlConnectionInfo m_originalConnectionInfo;

    internal DeploymentServiceHost(HostProperties hostProperties, ISqlConnectionInfo connectionInfo)
      : this(hostProperties, connectionInfo, new DeploymentServiceHostOptions())
    {
    }

    internal DeploymentServiceHost(
      HostProperties hostProperties,
      ISqlConnectionInfo connectionInfo,
      DeploymentServiceHostOptions options,
      IVssExceptionHandler exceptionHandler = null)
      : base(hostProperties, (VssServiceHost) null)
    {
      this.m_options = options;
      this.m_originalConnectionInfo = connectionInfo;
      if (connectionInfo != null && this.ServiceHostProperties != null && this.ServiceHostProperties.DatabaseId == DatabaseManagementConstants.InvalidDatabaseId)
        this.ServiceHostProperties.DatabaseId = TeamFoundationDatabaseManagementService.GetConfigurationDatabaseBootstrap(connectionInfo).DatabaseId;
      if (options.ProcessType == HostProcessType.ApplicationTier || options.ProcessType == HostProcessType.JobAgent || options.ProcessType == HostProcessType.Ssh)
      {
        string key = connectionInfo == null ? string.Empty : connectionInfo.ConnectionString ?? string.Empty;
        lock (Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.s_deploymentHosts)
        {
          if (Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.s_deploymentHosts.ContainsKey(key))
          {
            TeamFoundationTracingService.TraceRaw(186032191, TraceLevel.Error, nameof (DeploymentServiceHost), nameof (HostManagement), "Cannot create two DeploymentServiceHost objects for the same connection string in the same AppDomain - existing DeploymentHost was created by {0}", (object) Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.s_deploymentHosts[key].m_constructorCallStack);
            throw new InvalidOperationException();
          }
          this.m_constructorConnectionString = key;
          Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.s_deploymentHosts.Add(this.m_constructorConnectionString, this);
        }
      }
      this.m_constructorCallStack = new StackTrace(false).ToString();
      bool flag1 = false;
      try
      {
        if (connectionInfo != null && !string.IsNullOrEmpty(connectionInfo.ConnectionString))
        {
          ISqlConnectionInfo sqlConnectionInfo = connectionInfo != null ? connectionInfo : throw new InvalidOperationException(FrameworkResources.InvalidDeploymentHostConnectionInfo());
          DatabaseConnectionValidator connectionValidator = new DatabaseConnectionValidator();
          TeamFoundationConfigurationStatus configurationStatus = connectionValidator.ValidateApplicationConfiguration(sqlConnectionInfo, this.ServiceHostProperties.Id, (List<string>) null, true, false, false, true);
          bool flag2;
          using (ExtendedAttributeComponent componentRaw = sqlConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
            flag2 = componentRaw.ReadDeploymentTypeStamp() == DeploymentType.OnPremises;
          if (flag2 && options.FailOnInvalidConfiguration)
          {
            if (configurationStatus == TeamFoundationConfigurationStatus.Invalid)
              throw new ApplicationException(FrameworkResources.InvalidApplicationHostConfiguration((object) connectionValidator.GetMessagesForInvalidHosts()));
            if (configurationStatus == TeamFoundationConfigurationStatus.ValidAfterAutoFix)
              TeamFoundationEventLog.Default.Log(FrameworkResources.ApplicationHostConfigurationRepaired((object) connectionValidator.GetValidationMessagesForFixedHosts()), TeamFoundationEventId.ApplicationConfigurationFixed, EventLogEntryType.Warning);
          }
          HostProperties applicationHostProperties = (HostProperties) connectionValidator.ApplicationHostProperties;
          this.ServiceHostProperties.Description = applicationHostProperties.Description;
          this.ServiceHostProperties.Id = applicationHostProperties.Id;
          this.ServiceHostProperties.LastUserAccess = applicationHostProperties.LastUserAccess;
          this.ServiceHostProperties.Name = applicationHostProperties.Name;
          this.ServiceHostProperties.Status = applicationHostProperties.Status;
          this.ServiceHostProperties.StatusReason = applicationHostProperties.StatusReason;
          this.ServiceHostProperties.ServiceLevel = applicationHostProperties.ServiceLevel;
          this.m_databaseProperties = TeamFoundationDatabaseManagementService.GetDatabaseRaw(connectionInfo, this.ServiceHostProperties.DatabaseId);
          ((InternalDatabaseProperties) this.m_databaseProperties).SqlConnectionInfo = connectionInfo;
          if (this.m_databaseProperties.ServiceLevel != null)
          {
            ServiceLevel serviceLevel = new ServiceLevel(this.m_databaseProperties.ServiceLevel.Split(';')[0]);
            ServiceLevel currentServiceLevel = TeamFoundationSqlResourceComponent.CurrentServiceLevel;
            if (serviceLevel > currentServiceLevel)
              throw new ApplicationException(string.Format("Creating DeploymentServiceHost failed because configuration database service level is ahead of binaries service level. Database service level: {0}, Binaries service level: {1}", (object) serviceLevel, (object) currentServiceLevel));
          }
        }
        else
        {
          TeamFoundationTracingService.TraceRaw(186032192, TraceLevel.Info, nameof (DeploymentServiceHost), nameof (HostManagement), "Defaulting host properties due to lack of connection string");
          this.ServiceHostProperties.Id = hostProperties.Id;
        }
        if (string.IsNullOrEmpty(this.ServiceHostProperties.PhysicalDirectory))
          this.ServiceHostProperties.PhysicalDirectory = string.Empty;
        if (!string.IsNullOrEmpty(this.ServiceHostProperties.PlugInDirectory))
          VssExtensionManagementService.DefaultPluginPath = this.ServiceHostProperties.PlugInDirectory;
        this.m_systemRequestContext = new DefaultRequestContext((IVssServiceHost) this, RequestContextType.SystemContext, (LockHelper) null, this.UserRequestTimeout);
        this.m_hostManagement = new TeamFoundationHostManagementService();
        ((IVssFrameworkService) this.m_hostManagement).ServiceStart((IVssRequestContext) this.m_systemRequestContext);
        this.m_hostManagement.StartRootHost(this, (VssRequestContext) this.m_systemRequestContext, connectionInfo, true);
        this.m_hostManagement.ShutdownRequested += new EventHandler(this.OnShutdownRequested);
        if (options.FailOnInvalidConfiguration && connectionInfo != null && !string.IsNullOrEmpty(connectionInfo.ConnectionString) && (options.ProcessType == HostProcessType.ApplicationTier || options.ProcessType == HostProcessType.JobAgent || options.ProcessType == HostProcessType.Ssh) && this.m_systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          DatabaseConnectionValidator.ValidateOnPremConfigurationDatabase((IVssRequestContext) this.m_systemRequestContext);
        this.m_systemRequestContext.GetService<AlertPublishingService>();
        try
        {
          FeatureFlagService.UseNewFeatureService = RegistryHelpers.GetDeploymentValueRaw(this.m_systemRequestContext.FrameworkConnectionInfo, "/FeatureAvailability/Entries/Microsoft.VisualStudio.Services.FeaturesV2/AvailabilityState") == "1";
        }
        catch
        {
          TeamFoundationTracingService.TraceRawAlwaysOn(1180896581, TraceLevel.Warning, nameof (DeploymentServiceHost), nameof (HostManagement), "Failed to read feature state - Falling back to old FF service");
        }
        AbuseSkipCircuitBreakerService.SkipCircuitBreakers = false;
        TeamFoundationExecutionEnvironment executionEnvironment = this.m_systemRequestContext.ExecutionEnvironment;
        if (executionEnvironment.IsHostedDeployment)
        {
          try
          {
            AbuseSkipCircuitBreakerService.SkipCircuitBreakers = RegistryHelpers.GetDeploymentValueRaw(this.m_systemRequestContext.FrameworkConnectionInfo, "/FeatureAvailability/Entries/Microsoft.VisualStudio.Services.HostManagement.AbusiveHostSkipCircuitBreakers/AvailabilityState") == "1";
            this.m_systemRequestContext.GetService<AbuseSkipCircuitBreakerService>();
          }
          catch
          {
            TeamFoundationTracingService.TraceRawAlwaysOn(1180896582, TraceLevel.Warning, nameof (DeploymentServiceHost), nameof (HostManagement), "Failed to read AbusiveHostSkipCircuitBreakers feature state - Defaulting to off.");
          }
        }
        IVssExceptionHandler exceptionHandler1 = exceptionHandler;
        if (exceptionHandler1 == null)
        {
          executionEnvironment = this.m_systemRequestContext.ExecutionEnvironment;
          exceptionHandler1 = (IVssExceptionHandler) new ExceptionHandler(executionEnvironment.IsHostedDeployment);
        }
        ExceptionHandlerUtility.Initialize(exceptionHandler1);
        try
        {
          this.m_gcTask = new SemaphoreRemoteTask("forcegen2gc", new Action(this.FullCompactingGen2GC), TimeSpan.FromSeconds(5.0), TimeSpan.FromHours(1.0), (Action<Exception>) (exception => TeamFoundationTracingService.TraceExceptionRaw(1180896577, nameof (DeploymentServiceHost), nameof (HostManagement), exception)));
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(1180896576, nameof (DeploymentServiceHost), nameof (HostManagement), ex);
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        AppDomain.CurrentDomain.UnhandledException += Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.\u003C\u003EO.\u003C0\u003E__CurrentDomain_UnhandledException ?? (Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.\u003C\u003EO.\u003C0\u003E__CurrentDomain_UnhandledException = new UnhandledExceptionEventHandler(Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.CurrentDomain_UnhandledException));
        flag1 = true;
      }
      finally
      {
        if (!flag1)
        {
          if (this.m_constructorConnectionString != null)
          {
            lock (Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.s_deploymentHosts)
              Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.s_deploymentHosts.Remove(this.m_constructorConnectionString);
            this.m_constructorConnectionString = (string) null;
          }
          this.Dispose();
        }
      }
    }

    private static void CurrentDomain_UnhandledException(
      object sender,
      UnhandledExceptionEventArgs args)
    {
      try
      {
        TeamFoundationTracingService.TraceRawAlwaysOn(1180897777, TraceLevel.Error, nameof (DeploymentServiceHost), "HostManagement", "Unhandled exception, process is terminating={0}, exception={1}", (object) args.IsTerminating, args.ExceptionObject);
      }
      catch
      {
      }
    }

    private void OnShutdownRequested(object sender, EventArgs e) => this.RaiseStatusChanged(this.InstanceId, TeamFoundationServiceHostStatus.Stopping);

    private void FullCompactingGen2GC()
    {
      Stopwatch stopwatch = new Stopwatch();
      TeamFoundationTracingService.TraceRawAlwaysOn(1180896578, TraceLevel.Info, nameof (DeploymentServiceHost), "HostManagement", string.Format("Forcing full compacting GC now at {0}", (object) DateTime.UtcNow));
      try
      {
        stopwatch.Start();
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect(2, GCCollectionMode.Forced, true);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1180896579, nameof (DeploymentServiceHost), "HostManagement", ex);
      }
      finally
      {
        stopwatch.Stop();
        TeamFoundationTracingService.TraceRawAlwaysOn(1180896580, TraceLevel.Info, nameof (DeploymentServiceHost), "HostManagement", string.Format("Full compacting GC done in {0}ms at {1}", (object) stopwatch.ElapsedMilliseconds, (object) DateTime.UtcNow));
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.m_hostManagement != null)
        {
          this.m_hostManagement.DisposeTimers();
          if (this.m_gcTask != null)
          {
            this.m_gcTask.Dispose();
            this.m_gcTask = (SemaphoreRemoteTask) null;
          }
          this.m_systemRequestContext.GetService<TeamFoundationTaskService>().Shutdown();
          TeamFoundationTracingService.TraceRaw(189678889, TraceLevel.Info, nameof (DeploymentServiceHost), "HostManagement", "Stopping all hosts");
          this.Stop((VssRequestContext) this.m_systemRequestContext, false, true);
          TeamFoundationTracingService.TraceRaw(187930546, TraceLevel.Info, nameof (DeploymentServiceHost), "HostManagement", "Stopping Core Services");
          this.StopServices((IVssRequestContext) this.m_systemRequestContext, true);
          TeamFoundationTracingService.TraceRaw(187964961, TraceLevel.Info, nameof (DeploymentServiceHost), "HostManagement", "Core Services Stopped");
          if (this.IsRegistered)
          {
            TeamFoundationTracingService.TraceRaw(184413644, TraceLevel.Info, nameof (DeploymentServiceHost), "HostManagement", "Unregistering DeploymentServiceHost");
            try
            {
              this.m_hostManagement.UnregisterServiceHostInstance((IVssRequestContext) this.m_systemRequestContext, (IVssServiceHost) this);
              this.IsRegistered = false;
              TeamFoundationTracingService.TraceRaw(189277589, TraceLevel.Info, nameof (DeploymentServiceHost), "HostManagement", "Successfully Unregistered DeploymentServiceHost");
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(183813961, TraceLevel.Error, nameof (DeploymentServiceHost), "HostManagement", ex);
            }
          }
          this.m_hostManagement.ShutdownRequested -= new EventHandler(this.OnShutdownRequested);
          ((IVssFrameworkService) this.m_hostManagement).ServiceEnd((IVssRequestContext) this.m_systemRequestContext);
          this.m_hostManagement.Dispose();
          this.m_hostManagement = (TeamFoundationHostManagementService) null;
          this.m_systemRequestContext.Dispose();
          this.m_systemRequestContext = (DefaultRequestContext) null;
        }
        lock (Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.s_deploymentHosts)
        {
          if (this.m_constructorConnectionString != null)
          {
            Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.s_deploymentHosts.Remove(this.m_constructorConnectionString);
            this.m_constructorConnectionString = (string) null;
          }
        }
      }
      base.Dispose(disposing);
    }

    public override IVssRequestContext CreateServicingContext()
    {
      this.CheckDisposed();
      return (IVssRequestContext) this.m_hostManagement.BeginRequest((IVssRequestContext) this.m_systemRequestContext, this.InstanceId, RequestContextType.ServicingContext, true, false, (IReadOnlyList<IRequestActor>) null, HostRequestType.Default, (object[]) null);
    }

    public override void ReportException(
      string watsonReportingName,
      string eventCategory,
      Exception exception,
      string[] additionalInfo)
    {
      ExceptionHandlerUtility.ReportException(watsonReportingName, eventCategory, exception, additionalInfo);
    }

    internal override void Start(VssRequestContext systemRequestContext)
    {
      base.Start(systemRequestContext);
      if (this.HasDatabaseAccess)
      {
        systemRequestContext.GetService<TeamFoundationDatabaseManagementService>();
        this.m_sqlNotificationService = systemRequestContext.GetService<TeamFoundationSqlNotificationService>();
        this.m_sqlNotificationService.RegisterNotification((IVssRequestContext) systemRequestContext, "Default", SqlNotificationEventClasses.RegistrySettingsChanged, new SqlNotificationCallback(this.OnRegistrySettingsChanged), false);
        this.m_tracingService = systemRequestContext.GetService<TeamFoundationTracingService>();
      }
      this.ReloadDeploymentServiceHostSettings((IVssRequestContext) systemRequestContext, true);
      systemRequestContext.Trace(59123, TraceLevel.Info, nameof (DeploymentServiceHost), "HostManagement", "Deployment thresholds from registry: \r\n                                    default user request timeout {0} (sec)", (object) this.m_settings.UserRequestTimeout.TotalSeconds);
      if (!this.HasDatabaseAccess || this.m_options.ProcessType != HostProcessType.ApplicationTier)
        return;
      Interlocked.CompareExchange<Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.TeamFoundationMetabases>(ref this.m_metabases, new Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.TeamFoundationMetabases((IVssRequestContext) systemRequestContext), (Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.TeamFoundationMetabases) null);
    }

    private void ReloadDeploymentServiceHostSettings(
      IVssRequestContext requestContext,
      bool isHostStart = false)
    {
      requestContext.CheckDeploymentRequestContext();
      Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.DeploymentServiceHostSettings serviceHostSettings = new Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.DeploymentServiceHostSettings(requestContext);
      lock (this.m_settingsLock)
      {
        if (isHostStart)
        {
          if (this.m_settings != null)
            goto label_8;
        }
        this.m_settings = serviceHostSettings;
        this.LockManager.IsProduction = serviceHostSettings.IsProductionEnvironment;
        this.LockManager.CheckForLockViolations = serviceHostSettings.CheckForLockViolations;
      }
label_8:
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      int processorCount = Environment.ProcessorCount;
      ThreadPool.SetMinThreads(serviceHostSettings.MinWorkerThreadsPerCpu * processorCount, serviceHostSettings.MinCompletionPortThreadsPerCpu * processorCount);
      HostManagementTraceHelper.TraceThreadPool(true);
    }

    internal override void Stop(VssRequestContext systemRequestContext, bool isShuttingDown)
    {
      base.Stop(systemRequestContext, false, isShuttingDown);
      if (!this.HasDatabaseAccess)
        return;
      try
      {
        if (this.m_sqlNotificationService == null)
          return;
        this.m_sqlNotificationService.UnregisterNotification((IVssRequestContext) systemRequestContext, "Default", SqlNotificationEventClasses.RegistrySettingsChanged, new SqlNotificationCallback(this.OnRegistrySettingsChanged), false);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(59051, TraceLevel.Error, nameof (DeploymentServiceHost), "HostManagement", ex);
      }
    }

    internal override void Stop(
      VssRequestContext systemRequestContext,
      bool stopCoreServices,
      bool isShuttingDown)
    {
      this.m_hostManagement.Stop((IVssRequestContext) this.m_systemRequestContext);
      this.Stop((VssRequestContext) this.m_systemRequestContext, isShuttingDown);
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      IEnumerable<RegistryItem> entries = SqlRegistryService.DeserializeSqlNotification(requestContext, eventData);
      if (entries.Filter((IEnumerable<RegistryQuery>) Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.DeploymentServiceHostSettings.Queries).Any<RegistryItem>())
        this.ReloadDeploymentServiceHostSettings(requestContext);
      if (this.m_options.ProcessType != HostProcessType.ApplicationTier || !entries.Filter((IEnumerable<RegistryQuery>) TeamFoundationMetabase.Queries).Any<RegistryItem>())
        return;
      Volatile.Write<Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.TeamFoundationMetabases>(ref this.m_metabases, new Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.TeamFoundationMetabases(requestContext));
    }

    public IVssRequestContext CreateSystemContext(bool throwIfShutdown = true) => this.CreateContext(HostRequestType.Default, throwIfShutdown);

    public override bool IsProduction
    {
      get
      {
        Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.DeploymentServiceHostSettings settings = this.m_settings;
        return settings == null || settings.IsProductionEnvironment;
      }
    }

    public TimeSpan UserRequestTimeout
    {
      get
      {
        Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.DeploymentServiceHostSettings settings = this.m_settings;
        return settings == null ? TimeSpan.MaxValue : settings.UserRequestTimeout;
      }
    }

    public bool IsHosted => this.HostManagement.IsHosted;

    public Guid ServiceInstanceType
    {
      get
      {
        Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.DeploymentServiceHostSettings settings = this.m_settings;
        return settings == null ? Guid.Empty : settings.InstanceType;
      }
    }

    public ITeamFoundationDatabaseProperties DatabaseProperties => this.m_databaseProperties;

    public ISqlConnectionInfo OriginalConnectionInfo => this.m_originalConnectionInfo;

    public HostProcessType ProcessType => this.m_options.ProcessType;

    TeamFoundationMetabase IDeploymentServiceHostInternal.SharedMetabase => this.m_metabases?.SharedMetabase;

    protected override TeamFoundationMetabase Metabase => this.m_metabases?.DeploymentMetabase;

    int IDeploymentServiceHostInternal.MaxSqlComponents
    {
      get
      {
        Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.DeploymentServiceHostSettings settings = this.m_settings;
        return settings == null ? 0 : settings.MaxSqlComponents;
      }
    }

    IVssRequestContext IDeploymentServiceHostInternal.CreateContext(
      HostRequestType type,
      bool throwIfShutdown)
    {
      return this.CreateContext(type, throwIfShutdown);
    }

    private IVssRequestContext CreateContext(HostRequestType type, bool throwIfShutdown)
    {
      this.CheckDisposed();
      if (throwIfShutdown)
        this.CheckShutdown();
      return (IVssRequestContext) this.m_hostManagement.BeginRequest((IVssRequestContext) this.m_systemRequestContext, this.InstanceId, RequestContextType.SystemContext, true, throwIfShutdown, (IReadOnlyList<IRequestActor>) null, type, (object[]) null);
    }

    TeamFoundationHostManagementService IDeploymentServiceHostInternal.HostManagement => this.HostManagement;

    private TeamFoundationHostManagementService HostManagement => this.m_hostManagement;

    Guid IDeploymentServiceHostInternal.SystemServicePrincipal => this.m_settings != null ? this.m_settings.SystemServicePrincipal : Guid.Empty;

    Guid IDeploymentServiceHostInternal.S2STenantId => this.m_settings != null ? this.m_settings.S2STenantId : Guid.Empty;

    IdentityDescriptor IDeploymentServiceHostInternal.SystemDescriptor => this.m_settings?.SystemDescriptor;

    IRequestActor IDeploymentServiceHostInternal.SystemActor => this.m_settings?.SystemActor;

    DeploymentServiceHostOptions IDeploymentServiceHostInternal.CreationOptions => this.m_options;

    TeamFoundationTracingService IDeploymentServiceHostInternal.TracingService => this.m_tracingService;

    private class DeploymentServiceHostSettings
    {
      public readonly bool IsProductionEnvironment;
      public readonly Guid InstanceType;
      public readonly bool CheckForLockViolations;
      public readonly TimeSpan UserRequestTimeout;
      public readonly int MaxSqlComponents;
      public readonly Guid S2STenantId;
      public readonly Guid SystemServicePrincipal;
      public readonly IdentityDescriptor SystemDescriptor;
      public readonly IRequestActor SystemActor;
      public readonly int MinWorkerThreadsPerCpu;
      public readonly int MinCompletionPortThreadsPerCpu;
      public static readonly IReadOnlyList<RegistryQuery> Queries = (IReadOnlyList<RegistryQuery>) new RegistryQuery[8]
      {
        (RegistryQuery) FrameworkServerConstants.IsProductionEnvironment,
        (RegistryQuery) ConfigurationConstants.InstanceType,
        (RegistryQuery) FrameworkServerConstants.CheckForLockViolations,
        (RegistryQuery) FrameworkServerConstants.UserRequestTimeoutInterval,
        (RegistryQuery) FrameworkServerConstants.MaxSqlComponents,
        (RegistryQuery) OAuth2RegistryConstants.S2STenantId,
        (RegistryQuery) FrameworkServerConstants.MinWorkerThreadsPerCpu,
        (RegistryQuery) FrameworkServerConstants.MinCompletionPortThreadsPerCpu
      };
      private const int c_defaultUserRequestTimeoutSecs = 3600;
      private const int c_hostedDefaultUserRequestTimeoutSecs = 300;
      private const int c_defaultMinWorkerThreadsPerCpu = 32;
      private const int c_defaultMinCompletionPortThreadsPerCpu = 4;

      public DeploymentServiceHostSettings(IVssRequestContext requestContext)
      {
        requestContext.CheckDeploymentRequestContext();
        using (IEnumerator<IEnumerable<RegistryItem>> enumerator = RegistryHelpers.DeploymentReadRaw(requestContext.FrameworkConnectionInfo, (IEnumerable<RegistryQuery>) Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost.DeploymentServiceHostSettings.Queries).GetEnumerator())
        {
          enumerator.MoveNext();
          this.IsProductionEnvironment = enumerator.Current.GetSingleValue<bool>(true);
          enumerator.MoveNext();
          this.InstanceType = enumerator.Current.GetSingleValue<Guid>();
          TeamFoundationExecutionEnvironment executionEnvironment;
          if (this.InstanceType == Guid.Empty)
          {
            executionEnvironment = requestContext.ExecutionEnvironment;
            if (executionEnvironment.IsOnPremisesDeployment)
              this.InstanceType = ServiceInstanceTypes.TFSOnPremises;
          }
          enumerator.MoveNext();
          this.CheckForLockViolations = enumerator.Current.GetSingleValue<bool>(true);
          executionEnvironment = requestContext.ExecutionEnvironment;
          if (executionEnvironment.IsHostedDeployment)
          {
            enumerator.MoveNext();
            this.UserRequestTimeout = TimeSpan.FromSeconds((double) enumerator.Current.GetSingleValue<int>(300));
          }
          else
          {
            enumerator.MoveNext();
            this.UserRequestTimeout = TimeSpan.FromSeconds((double) enumerator.Current.GetSingleValue<int>(3600));
          }
          enumerator.MoveNext();
          this.MaxSqlComponents = enumerator.Current.GetSingleValue<int>();
          enumerator.MoveNext();
          this.S2STenantId = enumerator.Current.GetSingleValue<Guid>(Guid.Empty);
          enumerator.MoveNext();
          this.MinWorkerThreadsPerCpu = enumerator.Current.GetSingleValue<int>(32);
          enumerator.MoveNext();
          this.MinCompletionPortThreadsPerCpu = enumerator.Current.GetSingleValue<int>(4);
        }
        this.SystemServicePrincipal = InstanceManagementHelper.ServicePrincipalFromServiceInstance(this.InstanceType);
        this.SystemDescriptor = new IdentityDescriptor("System:ServicePrincipal", this.SystemServicePrincipal.ToString("D") + "@" + this.S2STenantId.ToString("D"));
        this.SystemActor = RequestActor.CreateRequestActor(requestContext, this.SystemDescriptor, this.SystemServicePrincipal);
      }
    }

    private class TeamFoundationMetabases
    {
      public readonly TeamFoundationMetabase DeploymentMetabase;
      public readonly TeamFoundationMetabase SharedMetabase;

      public TeamFoundationMetabases(IVssRequestContext requestContext)
      {
        requestContext.CheckDeploymentRequestContext();
        this.DeploymentMetabase = new TeamFoundationMetabase(requestContext, true);
        this.SharedMetabase = new TeamFoundationMetabase(requestContext, false);
      }
    }
  }
}
