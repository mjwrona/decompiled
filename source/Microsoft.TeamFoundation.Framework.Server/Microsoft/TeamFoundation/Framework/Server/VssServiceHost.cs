// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssServiceHost
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssServiceHost : 
    ServiceHost<VssRequestContext>,
    IVssServiceHost,
    IVssServiceHostControl,
    IDisposable,
    IVssServiceHostProperties,
    IServiceHostInternal
  {
    private readonly VssServiceHost m_parentServiceHost;
    private IDisposableReadOnlyList<ITeamFoundationRequestFilter> m_requestFilters;
    private readonly bool m_sendWatsonReports = true;
    private CultureInfo m_serverCulture = CultureInfo.InstalledUICulture;
    private readonly IDictionary<string, object> m_itemsCollection = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private DateTime m_startTime;
    private string m_dataDirectory;
    private string m_dataDirectoryRoot;
    private HostProperties m_hostProperties;
    protected ReaderWriterLock m_hostPropertiesLock = new ReaderWriterLock();
    private readonly LockManager m_lockManager;
    private readonly ServiceProvider m_serviceProvider;
    private bool m_isRegistered;
    private int m_queuedRequestThreshold;
    private long m_queuedRequestElapsedThreshold;
    private long m_totalExecutionElapsedThreshold;
    private bool m_monitorEnabled;
    private TimeSpan m_impactedUsersMetricIntervalDefault = TimeSpan.FromMinutes(1.0);
    private const int c_queuedRequestThresholdDefault = 10;
    private const long c_queuedRequestElapsedThresholdDefault = 15;
    private const long c_totalExecutionElapsedThresholdDefault = 10;
    private const bool c_monitorEnabledDefault = false;
    private const int c_healthMonitorIntervalDefault = 15000;
    private const int c_monitorIntervalDefault = 30000;
    private const int m_heartbeatIntervalSecondsDefault = 0;
    private bool m_queuedRequestThresholdExceeded;
    private bool m_queuedRequestElapsedThresholdExceeded;
    private bool m_totalExecutionElapsedThresholdExceeded;
    private static long s_LockNameUniquifier;
    private static readonly TimeSpan s_flushTimeout = TimeSpan.FromSeconds(120.0);
    protected static readonly Guid s_hostSecurityNamespace = new Guid("89612D3A-2EF7-43ad-BA2B-31D0E3A5B2EF");
    private const string c_Area = "HostManagement";
    private const string c_Layer = "ServiceHost";
    private static readonly TimeSpan s_maxFlushTimeout = TimeSpan.FromMinutes(5.0);
    private static readonly TimeSpan s_maxCancellationWaitTime = TimeSpan.FromMinutes(1.0);
    private int m_partitionId;

    internal VssServiceHost(HostProperties hostProperties, VssServiceHost parentServiceHost)
    {
      ArgumentUtility.CheckForNull<HostProperties>(hostProperties, nameof (hostProperties));
      TeamFoundationTracingService.TraceRaw(59020, TraceLevel.Verbose, "HostManagement", "ServiceHost", "Start ServiceHost creation, type: {0}", (object) hostProperties.HostType);
      if ((hostProperties.HostType & TeamFoundationHostType.Deployment) != TeamFoundationHostType.Deployment)
        ArgumentUtility.CheckForNull<VssServiceHost>(parentServiceHost, nameof (parentServiceHost));
      if (hostProperties.HostType == TeamFoundationHostType.Unknown)
        throw new ArgumentException();
      this.m_startTime = DateTime.UtcNow;
      this.m_parentServiceHost = parentServiceHost;
      this.m_hostProperties = new HostProperties(hostProperties);
      this.m_lockManager = (hostProperties.HostType & TeamFoundationHostType.Deployment) != TeamFoundationHostType.Deployment ? parentServiceHost.LockManager : (LockManager) new VssLockManager();
      this.m_serviceProvider = new ServiceProvider(new ServiceResolver((IVssServiceHost) this));
      bool result;
      if (ConfigurationManager.AppSettings["sendWatsonReports"] != null && bool.TryParse(ConfigurationManager.AppSettings["sendWatsonReports"], out result))
        this.m_sendWatsonReports = result;
      using (VssRequestContext requestContext = (VssRequestContext) new DefaultRequestContext((IVssServiceHost) this, RequestContextType.SystemContext, (LockHelper) null, this.DeploymentServiceHost.UserRequestTimeout))
      {
        this.InitializeDataDirectory(requestContext);
        if (this.HasDatabaseAccess)
        {
          try
          {
            this.m_partitionId = this.GetPartitionId((IVssRequestContext) requestContext, hostProperties);
          }
          catch (DatabaseConfigurationException ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(59000, "HostManagement", "ServiceHost", (Exception) ex);
          }
        }
        this.UpdatePerformanceCounter((IVssRequestContext) requestContext, VssServiceHost.PerformanceCounterOperationType.Increment);
      }
      TeamFoundationTracingService.TraceRaw(59021, TraceLevel.Verbose, "HostManagement", "ServiceHost", "End ServiceHost creation, type: " + hostProperties.HostType.ToString());
    }

    public string Name
    {
      get
      {
        this.CheckDisposed();
        return this.m_hostProperties.Name;
      }
    }

    public TeamFoundationHostType HostType
    {
      get
      {
        this.CheckDisposed();
        return this.m_hostProperties.HostType;
      }
    }

    public string PhysicalDirectory
    {
      get
      {
        this.CheckDisposed();
        return this.ParentServiceHost != null ? this.ParentServiceHost.PhysicalDirectory : this.m_hostProperties.PhysicalDirectory;
      }
    }

    public string PlugInDirectory
    {
      get
      {
        this.CheckDisposed();
        return this.ParentServiceHost != null ? this.ParentServiceHost.PlugInDirectory : this.m_hostProperties.PlugInDirectory;
      }
    }

    public TeamFoundationServiceHostStatus Status
    {
      get
      {
        this.CheckDisposed();
        return this.m_hostProperties.Status;
      }
    }

    public string StatusReason
    {
      get
      {
        this.CheckDisposed();
        string statusReason = (string) null;
        if (this.m_parentServiceHost != null)
          statusReason = this.m_parentServiceHost.StatusReason;
        if (statusReason == null && this.Status != TeamFoundationServiceHostStatus.Started)
          statusReason = this.m_hostProperties.StatusReason;
        return statusReason;
      }
    }

    public string Description
    {
      get
      {
        this.CheckDisposed();
        return this.m_hostProperties.Description;
      }
    }

    public string StaticContentDirectory
    {
      get
      {
        this.CheckDisposed();
        return VirtualPathUtility.ToAbsolute(FrameworkServerConstants.ServiceHostResourcesDirectory);
      }
    }

    public IVssServiceHost ParentServiceHost
    {
      get
      {
        this.CheckDisposed();
        return (IVssServiceHost) this.m_parentServiceHost;
      }
    }

    public IVssDeploymentServiceHost DeploymentServiceHost
    {
      get
      {
        this.CheckDisposed();
        IVssServiceHost deploymentServiceHost = (IVssServiceHost) this;
        while (deploymentServiceHost.ParentServiceHost != null)
          deploymentServiceHost = deploymentServiceHost.ParentServiceHost;
        return (IVssDeploymentServiceHost) deploymentServiceHost;
      }
    }

    public IVssServiceHost OrganizationServiceHost
    {
      get
      {
        this.CheckDisposed();
        IVssServiceHost organizationServiceHost = (IVssServiceHost) this;
        do
          ;
        while (!organizationServiceHost.Is(TeamFoundationHostType.Application) && (organizationServiceHost = organizationServiceHost.ParentServiceHost) != null);
        return organizationServiceHost;
      }
    }

    public IVssServiceHost CollectionServiceHost
    {
      get
      {
        this.CheckDisposed();
        IVssServiceHost collectionServiceHost = (IVssServiceHost) this;
        do
          ;
        while (!collectionServiceHost.Is(TeamFoundationHostType.ProjectCollection) && (collectionServiceHost = collectionServiceHost.ParentServiceHost) != null);
        return collectionServiceHost;
      }
    }

    public bool HasDatabaseAccess
    {
      get
      {
        this.CheckDisposed();
        return this.m_hostProperties.DatabaseId != DatabaseManagementConstants.InvalidDatabaseId && this.m_hostProperties.DatabaseId != -2 && this.DeploymentServiceHost.DatabaseProperties != null && this.DeploymentServiceHost.DatabaseProperties.SqlConnectionInfo != null && !string.IsNullOrEmpty(this.DeploymentServiceHost.DatabaseProperties.SqlConnectionInfo.ConnectionString);
      }
    }

    public bool IsVirtualServiceHost
    {
      get
      {
        this.CheckDisposed();
        return this.m_hostProperties.DatabaseId == -2;
      }
    }

    public virtual bool IsProduction => this.DeploymentServiceHost.IsProduction;

    public int PartitionId
    {
      get
      {
        if (this.m_partitionId == 0)
        {
          using (IVssRequestContext systemContext = this.DeploymentServiceHost.CreateSystemContext(false))
            this.m_partitionId = this.GetPartitionId(systemContext, this.m_hostProperties);
        }
        return this.m_partitionId;
      }
    }

    public bool SendWatsonReports
    {
      get
      {
        this.CheckDisposed();
        return this.m_sendWatsonReports;
      }
    }

    public bool Is(TeamFoundationHostType hostType)
    {
      this.CheckDisposed();
      return (this.HostType & hostType) == hostType;
    }

    public bool IsOnly(TeamFoundationHostType hostType)
    {
      if (hostType == TeamFoundationHostType.Deployment)
        throw new ArgumentException("A host may never be \"only\" a deployment.");
      this.CheckDisposed();
      return (this.HostType & ~hostType) == TeamFoundationHostType.Unknown;
    }

    public ILockName CreateLockName(string name) => (ILockName) new LockName<short, Guid, string>((short) this.HostType, this.InstanceId, name, LockLevel.Resource);

    public ILockName CreateUniqueLockName(string name) => (ILockName) new LockName<short, Guid, string>((short) this.HostType, this.InstanceId, string.Format("{0}:{1}", (object) name, (object) Interlocked.Increment(ref VssServiceHost.s_LockNameUniquifier)), LockLevel.Resource);

    public virtual IVssRequestContext CreateServicingContext() => (IVssRequestContext) this.CreateRequest(RequestContextType.ServicingContext, (LockHelper) null, HostRequestType.Default);

    public CultureInfo GetCulture(IVssRequestContext requestContext)
    {
      this.CheckDisposed();
      if (this.m_serverCulture == CultureInfo.InstalledUICulture)
        this.m_serverCulture = this.GetInstalledUICulture(requestContext);
      return this.m_serverCulture;
    }

    public virtual void ReportException(
      string watsonReportingName,
      string eventCategory,
      Exception exception,
      string[] additionalInfo)
    {
      if (this.ParentServiceHost == null)
        return;
      this.ParentServiceHost.ReportException(watsonReportingName, eventCategory, exception, additionalInfo);
    }

    public event EventHandler<HostStatusChangedEventArgs> StatusChanged;

    public event EventHandler<HostPropertiesChangedEventArgs> PropertiesChanged;

    void IServiceHostInternal.CheckShutdown() => this.CheckShutdown();

    protected void CheckShutdown()
    {
      try
      {
        this.m_hostPropertiesLock.AcquireReaderLock(-1);
        if (this.m_hostProperties.Status != TeamFoundationServiceHostStatus.Paused && this.m_hostProperties.Status != TeamFoundationServiceHostStatus.Stopped)
          return;
        this.m_hostProperties.ThrowShutdownException();
      }
      finally
      {
        if (this.m_hostPropertiesLock.IsReaderLockHeld || this.m_hostPropertiesLock.IsWriterLockHeld)
          this.m_hostPropertiesLock.ReleaseReaderLock();
      }
    }

    int IServiceHostInternal.DatabaseId => this.m_hostProperties.DatabaseId;

    string IServiceHostInternal.DataDirectory
    {
      get
      {
        this.CheckDisposed();
        return this.m_dataDirectory;
      }
    }

    string IServiceHostInternal.ServiceLevel => this.m_hostProperties.ServiceLevel;

    int IServiceHostInternal.StorageAccountId => this.m_hostProperties.StorageAccountId;

    LockManager IServiceHostInternal.LockManager => this.LockManager;

    protected LockManager LockManager => this.m_lockManager;

    TeamFoundationMetabase IServiceHostInternal.Metabase => this.Metabase;

    protected virtual TeamFoundationMetabase Metabase => this.DeploymentServiceHost.DeploymentServiceHostInternal().SharedMetabase;

    IDisposableReadOnlyList<ITeamFoundationRequestFilter> IServiceHostInternal.RequestFilters
    {
      get => this.RequestFilters;
      set => this.RequestFilters = value;
    }

    private IDisposableReadOnlyList<ITeamFoundationRequestFilter> RequestFilters
    {
      get
      {
        this.CheckDisposed();
        if (this.m_requestFilters != null)
          return this.m_requestFilters;
        return this.m_parentServiceHost == null ? (IDisposableReadOnlyList<ITeamFoundationRequestFilter>) null : this.m_parentServiceHost.RequestFilters;
      }
      set => this.m_requestFilters = this.m_requestFilters == null ? value : throw new InvalidOperationException("RequestFilters already set");
    }

    long IServiceHostInternal.TotalExecutionElapsedThreshold => this.m_totalExecutionElapsedThreshold;

    IVssFrameworkServiceProvider IServiceHostInternal.ServiceProvider => (IVssFrameworkServiceProvider) this.m_serviceProvider;

    DateTime IServiceHostInternal.StartTime => this.m_startTime;

    void IServiceHostInternal.Initialize(
      DateTime startTime,
      TeamFoundationServiceHostStatus initialStatus)
    {
      this.m_startTime = startTime;
      this.m_hostProperties.Status = initialStatus;
    }

    void IServiceHostInternal.ApplicationEndRequest(IVssRequestContext requestContext)
    {
      TeamFoundationTracingService.TraceEnterRaw(59060, "HostManagement", "ServiceHost", "ApplicationEndRequest", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      IRequestContextInternal requestContextInternal = requestContext.RequestContextInternal();
      try
      {
        lock (requestContext)
        {
          requestContextInternal.ResetCancel();
          requestContextInternal.DisposeDisposableResources();
          if (this.RequestFilters != null)
          {
            foreach (ITeamFoundationRequestFilter requestFilter in (IEnumerable<ITeamFoundationRequestFilter>) this.RequestFilters)
            {
              try
              {
                requestFilter.EndRequest(requestContext);
              }
              catch (Exception ex)
              {
                TeamFoundationTracingService.TraceExceptionRaw(59062, TraceLevel.Error, "HostManagement", "ServiceHost", ex);
              }
            }
          }
          try
          {
            requestContext.LeaveMethod();
            requestContext.Trace(59063, TraceLevel.Verbose, "HostManagement", "ServiceHost", "Calling EndRequest on Service Host for request {0}", (object) requestContext.ContextId);
            this.EndRequest(requestContext);
            requestContext.Trace(59064, TraceLevel.Verbose, "HostManagement", "ServiceHost", "Called EndRequest on Service Host for request {0}", (object) requestContext.ContextId);
          }
          finally
          {
            try
            {
              requestContext.GetService<WebRequestLogger>().LogRequest(requestContext);
              if (requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ServiceHostExtended"))
                requestContext.To(TeamFoundationHostType.Deployment).GetService<WebHostLogger>().LogRequest(requestContext);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(59066, TraceLevel.Error, "HostManagement", "ServiceHost", ex);
            }
          }
          requestContext.Trace(59065, TraceLevel.Verbose, "HostManagement", "ServiceHost", "Disposing TfsRequestContext {0}", (object) requestContext.ContextId);
          requestContext.Dispose();
        }
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(59067, "HostManagement", "ServiceHost", "ApplicationEndRequest");
      }
    }

    bool IServiceHostInternal.FlushNotificationQueue(IVssRequestContext context) => this.FlushNotificationQueue((VssRequestContext) context);

    bool IServiceHostInternal.FlushNotificationQueue(
      IVssRequestContext requestContext,
      Guid processId,
      TimeSpan flushTimeout,
      bool ignoreDefaultMaxFlushTimeout)
    {
      requestContext.TraceEnter(508353517, "HostManagement", "ServiceHost", "FlushNotificationQueue");
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_WaitingOnPingResponse").Increment();
      try
      {
        ArgumentUtility.CheckForOutOfRange((int) flushTimeout.TotalMilliseconds, nameof (flushTimeout), 0);
        Guid processId1 = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationHostManagementService>().ProcessId;
        if (flushTimeout > VssServiceHost.s_maxFlushTimeout && !ignoreDefaultMaxFlushTimeout)
        {
          requestContext.Trace(554734162, TraceLevel.Warning, "HostManagement", "ServiceHost", "Reducing flush timeout from {0} to {1}.", (object) flushTimeout, (object) VssServiceHost.s_maxFlushTimeout);
          flushTimeout = VssServiceHost.s_maxFlushTimeout;
        }
        Guid eventClass = SqlNotificationEventClasses.FlushNotificationQueueRequest;
        FlushNotificationQueueRequest request1 = new FlushNotificationQueueRequest()
        {
          RequestId = Guid.NewGuid(),
          ProcessId = processId
        };
        if (processId == processId1)
          eventClass = request1.RequestId;
        else if (!requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        {
          IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
          IsHostLoadedRequest request2 = new IsHostLoadedRequest()
          {
            RequestId = Guid.NewGuid(),
            ProcessId = processId,
            HostId = requestContext.ServiceHost.InstanceId
          };
          Stopwatch stopwatch = Stopwatch.StartNew();
          NotificationEventArgs notificationArgs;
          if (!this.SendRequestAndWaitForResponse<IsHostLoadedRequest>(requestContext1, request2, SqlNotificationEventClasses.IsHostLoaded, flushTimeout, out notificationArgs))
            return false;
          stopwatch.Stop();
          bool flag = notificationArgs.Deserialize<bool>();
          requestContext.Trace(74524884, TraceLevel.Info, "HostManagement", "ServiceHost", "Host Loaded Request {0}, Response {1}", (object) request2.RequestId, (object) flag);
          if (!flag)
            return true;
          flushTimeout = flushTimeout.Subtract(stopwatch.Elapsed);
        }
        return this.SendRequestAndWaitForResponse<FlushNotificationQueueRequest>(requestContext, request1, eventClass, flushTimeout, out NotificationEventArgs _);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1848106630, "HostManagement", "ServiceHost", ex);
        throw;
      }
      finally
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_WaitingOnPingResponse").Decrement();
        requestContext.TraceLeave(703082612, "HostManagement", "ServiceHost", "FlushNotificationQueue");
      }
    }

    ServiceHostSubStatus IServiceHostInternal.SubStatus => this.m_hostProperties.SubStatus;

    private bool SendRequestAndWaitForResponse<T>(
      IVssRequestContext requestContext,
      T request,
      Guid eventClass,
      TimeSpan flushTimeout,
      out NotificationEventArgs notificationArgs)
      where T : IFlushRequest
    {
      notificationArgs = (NotificationEventArgs) null;
      using (ManualResetEvent receivedEvent = new ManualResetEvent(false))
      {
        NotificationEventArgs savedArgs = (NotificationEventArgs) null;
        SqlNotificationHandler handler = (SqlNotificationHandler) ((rc, args) =>
        {
          savedArgs = args;
          receivedEvent.Set();
        });
        ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
        try
        {
          service.RegisterNotification(requestContext, "Default", request.RequestId, handler, false);
          requestContext.Trace(26579722, TraceLevel.Verbose, "HostManagement", "ServiceHost", "Writing Flush Notification Queue Request {0}", (object) request.RequestId);
          long num = service.SendNotification(requestContext, eventClass, TeamFoundationSerializationUtility.SerializeToString<T>(request));
          Stopwatch stopwatch = Stopwatch.StartNew();
          switch (WaitHandle.WaitAny(new WaitHandle[2]
          {
            (WaitHandle) receivedEvent,
            requestContext.CancellationToken.WaitHandle
          }, (int) flushTimeout.TotalMilliseconds))
          {
            case 0:
              requestContext.Trace(1153986613, TraceLevel.Verbose, "HostManagement", "ServiceHost", "Received flush response. ProcessId: {0}; RequestId: {1}; EventId: {2}; Elapsed time: {3}", (object) request.ProcessId, (object) request.RequestId, (object) num, (object) stopwatch.Elapsed);
              notificationArgs = savedArgs;
              return true;
            case 1:
              requestContext.Trace(1153986617, TraceLevel.Warning, "HostManagement", "ServiceHost", "Request was cancelled before receiving response. ProcessId: {0}; RequestId: {1}; EventId: {2}; Elapsed time: {3}", (object) request.ProcessId, (object) request.RequestId, (object) num, (object) stopwatch.Elapsed);
              return false;
            default:
              requestContext.Trace(522037907, TraceLevel.Error, "HostManagement", "ServiceHost", "Timed out waiting for flush response. ProcessId: {0}; RequestId: {1}; EventId: {2}; Elapsed time: {3}; Stack: {4}", (object) request.ProcessId, (object) request.RequestId, (object) num, (object) flushTimeout, (object) Environment.StackTrace);
              return false;
          }
        }
        finally
        {
          service.UnregisterNotification(requestContext, "Default", request.RequestId, handler, true);
        }
      }
    }

    IVssRequestContext[] IServiceHostInternal.GetActiveRequests() => (IVssRequestContext[]) this.GetActiveRequests();

    void IServiceHostInternal.RecycleServices(IVssRequestContext requestContext) => this.StopServices(requestContext, false);

    void IServiceHostInternal.SetRegistered() => this.IsRegistered = true;

    bool IServiceHostInternal.TryGetRequest(long requestId, out IVssRequestContext activeRequest)
    {
      activeRequest = (IVssRequestContext) null;
      VssRequestContext requestContext;
      int num = this.TryGetRequest(requestId, out requestContext) ? 1 : 0;
      if (num == 0)
        return num != 0;
      activeRequest = (IVssRequestContext) requestContext;
      return num != 0;
    }

    public override string ToString() => VssServiceHost.ToString(this.InstanceId, this.Name);

    protected override void Dispose(bool disposing)
    {
      try
      {
        TeamFoundationTracingService.TraceEnterRaw(59001, "HostManagement", "ServiceHost", nameof (Dispose), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
        if (disposing)
        {
          using (VssRequestContext requestContext = (VssRequestContext) new DefaultRequestContext((IVssServiceHost) this, RequestContextType.SystemContext, (LockHelper) null, this.DeploymentServiceHost.UserRequestTimeout))
            this.UpdatePerformanceCounter((IVssRequestContext) requestContext, VssServiceHost.PerformanceCounterOperationType.Decrement);
          TeamFoundationTracingService.TraceRaw(59006, TraceLevel.Info, "HostManagement", "ServiceHost", "Host counter decrement: host type: {0}", (object) this.m_hostProperties.HostType);
          if (this.m_requestFilters != null)
          {
            this.m_requestFilters.Dispose();
            this.m_requestFilters = (IDisposableReadOnlyList<ITeamFoundationRequestFilter>) null;
          }
        }
        base.Dispose(disposing);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(59009, TraceLevel.Error, "HostManagement", "ServiceHost", ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(59010, "HostManagement", "ServiceHost", nameof (Dispose));
      }
    }

    public override Guid InstanceId => this.m_hostProperties.Id;

    protected override void BeginRequest(VssRequestContext requestContext)
    {
      this.CheckDisposed();
      try
      {
        this.m_hostPropertiesLock.AcquireReaderLock(-1);
        if (!requestContext.IsServicingContext && this.m_hostProperties.Status != TeamFoundationServiceHostStatus.Started)
          this.m_hostProperties.ThrowShutdownException((IVssRequestContext) requestContext);
        base.BeginRequest(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(59013, "HostManagement", "ServiceHost", ex);
        throw;
      }
      finally
      {
        if (this.m_hostPropertiesLock.IsReaderLockHeld || this.m_hostPropertiesLock.IsWriterLockHeld)
          this.m_hostPropertiesLock.ReleaseReaderLock();
      }
      if (!(this.LastUse.AddSeconds(15.0) < DateTime.UtcNow))
        return;
      this.LastUse = DateTime.UtcNow;
      requestContext.GetService<TeamFoundationHostManagementService>().SetLastAccessTime((IVssRequestContext) requestContext);
    }

    protected override void EndRequest(VssRequestContext requestContext)
    {
      try
      {
        if (this.LastUse.AddSeconds(15.0) < DateTime.UtcNow)
        {
          this.LastUse = DateTime.UtcNow;
          requestContext.GetService<TeamFoundationHostManagementService>().SetLastAccessTime((IVssRequestContext) requestContext);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "HostManagement", "ServiceHost", ex);
      }
      try
      {
        base.EndRequest(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(59012, "HostManagement", "ServiceHost", ex);
        throw;
      }
    }

    internal override HostProperties ServiceHostProperties => this.m_hostProperties;

    internal override VssRequestContext CreateRequest(
      RequestContextType requestType,
      LockHelper helper,
      HostRequestType type,
      params object[] additional)
    {
      TimeSpan userRequestTimeout = this.DeploymentServiceHost.UserRequestTimeout;
      switch (type)
      {
        case HostRequestType.Default:
          VssRequestContext rootContext = (VssRequestContext) null;
          if (additional != null && additional.Length != 0 && additional[0] is VssRequestContext)
            rootContext = (VssRequestContext) additional[0];
          return (VssRequestContext) new DefaultRequestContext((IVssServiceHost) this, requestType, helper, rootContext, userRequestTimeout);
        case HostRequestType.AspNet:
          HttpContextWrapper httpContext1 = new HttpContextWrapper(HttpContext.Current);
          return (VssRequestContext) new AspNetRequestContext((IVssServiceHost) this, requestType, (HttpContextBase) httpContext1, helper, userRequestTimeout);
        case HostRequestType.Job:
          return (VssRequestContext) new JobRequestContext((IVssServiceHost) this, requestType, (TeamFoundationJobService) additional[0], (TeamFoundationJobQueueEntry) additional[1], helper, userRequestTimeout);
        case HostRequestType.GenericHttp:
          HttpContextBase httpContext2 = (HttpContextBase) additional[0];
          return (VssRequestContext) new GenericHttpRequestContext(this, requestType, httpContext2, helper, userRequestTimeout);
        case HostRequestType.Ssh:
          HttpContextBase httpContext3 = additional.Length >= 1 ? (HttpContextBase) additional[0] : (HttpContextBase) null;
          string userAgent = additional.Length >= 2 ? (string) additional[1] : string.Empty;
          return (VssRequestContext) new SshRequestContext((IVssServiceHost) this, requestType, userAgent, httpContext3, helper, userRequestTimeout);
        default:
          throw new ArgumentOutOfRangeException(nameof (type));
      }
    }

    internal override void Start(VssRequestContext systemRequestContext)
    {
      this.CheckDisposed();
      bool flag = false;
      TeamFoundationHostManagementService service1 = systemRequestContext.GetService<TeamFoundationHostManagementService>();
      if (!this.IsRegistered && (this.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
      {
        service1.RegisterServiceHostInstance(systemRequestContext.To(TeamFoundationHostType.Deployment), this);
        this.IsRegistered = true;
      }
      systemRequestContext.TraceEnter(59040, "HostManagement", "ServiceHost", "IVssServiceHost.Start");
      try
      {
        systemRequestContext.Trace(59041, TraceLevel.Info, "HostManagement", "ServiceHost", "Calling BeginStatusChange for Host {0}", (object) this.InstanceId);
        flag = this.BeginStatusChange(systemRequestContext.To(TeamFoundationHostType.Deployment), TeamFoundationServiceHostStatus.Starting, false);
        if (flag)
        {
          systemRequestContext.Trace(59042, TraceLevel.Info, "HostManagement", "ServiceHost", "Starting Services for Host {0}", (object) this.InstanceId);
          this.StartServices();
          using (IVssRequestContext vssRequestContext1 = service1.BeginRequest((IVssRequestContext) systemRequestContext, this.InstanceId, RequestContextType.SystemContext, true, false))
          {
            int healthMonitorInterval = 15000;
            int clientDisconnectMonitorInterval = 30000;
            int jobTimeoutMonitorInterval = 30000;
            int num = 0;
            this.m_queuedRequestThreshold = 10;
            this.m_queuedRequestElapsedThreshold = 15L;
            this.m_totalExecutionElapsedThreshold = 10L;
            this.m_monitorEnabled = false;
            try
            {
              systemRequestContext.Trace(59043, TraceLevel.Info, "HostManagement", "ServiceHost", "Publishing HostReadyEvent for Host {0}", (object) this.InstanceId);
              vssRequestContext1.GetService<TeamFoundationEventService>().PublishNotification(vssRequestContext1, (object) new HostReadyEvent((IVssServiceHost) this));
              if ((this.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
              {
                SqlRegistryService service2 = vssRequestContext1.GetService<SqlRegistryService>();
                this.ReadServiceHostMonitorSettings(vssRequestContext1, service2, out this.m_queuedRequestThreshold, out this.m_queuedRequestElapsedThreshold, out this.m_totalExecutionElapsedThreshold, out healthMonitorInterval, out this.m_monitorEnabled, out clientDisconnectMonitorInterval, out jobTimeoutMonitorInterval);
                if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
                  num = service2.GetValue<int>(vssRequestContext1, (RegistryQuery) FrameworkServerConstants.OiHeartbeatIntervalSeconds, true, 300);
              }
              else
              {
                IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Deployment);
                SqlRegistryService service3 = (SqlRegistryService) vssRequestContext2.GetService<CachedRegistryService>();
                this.ReadServiceHostMonitorSettings(vssRequestContext2, service3, out this.m_queuedRequestThreshold, out this.m_queuedRequestElapsedThreshold, out this.m_totalExecutionElapsedThreshold, out healthMonitorInterval, out this.m_monitorEnabled, out clientDisconnectMonitorInterval, out jobTimeoutMonitorInterval);
              }
              if (this.m_totalExecutionElapsedThreshold != 10L)
                vssRequestContext1.Trace(59163, TraceLevel.Warning, "HostManagement", "ServiceHost", "Overriding Total Execution Threshold from {0} to {1}", (object) 10L, (object) this.m_totalExecutionElapsedThreshold);
              vssRequestContext1.Trace(59123, TraceLevel.Info, "HostManagement", "ServiceHost", "Thresholds from registry: \r\n                                                    queued requests: {0}, \r\n                                                    max queued request elapsed time: {1} (sec), \r\n                                                    total elapsed time: {2} (sec)", (object) this.m_queuedRequestThreshold, (object) this.m_queuedRequestElapsedThreshold, (object) this.m_totalExecutionElapsedThreshold);
            }
            finally
            {
              TeamFoundationTaskService service4 = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
              TeamFoundationExecutionEnvironment executionEnvironment;
              if (this.DeploymentServiceHost.IsHostProcessType(HostProcessType.ApplicationTier))
              {
                if (this.m_monitorEnabled)
                {
                  systemRequestContext.Trace(59162, TraceLevel.Info, "HostManagement", "ServiceHost", "Adding MonitorServiceHostRequests Task");
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  service4.AddTask(vssRequestContext1, new TeamFoundationTask(VssServiceHost.\u003C\u003EO.\u003C0\u003E__MonitorServiceHostRequests ?? (VssServiceHost.\u003C\u003EO.\u003C0\u003E__MonitorServiceHostRequests = new TeamFoundationTaskCallback(VssServiceHost.MonitorServiceHostRequests)), (object) null, healthMonitorInterval));
                }
                systemRequestContext.Trace(59164, TraceLevel.Info, "HostManagement", "ServiceHost", "Adding Client Disconnect Task");
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                service4.AddTask(vssRequestContext1, new TeamFoundationTask(VssServiceHost.\u003C\u003EO.\u003C1\u003E__MonitorClientDisconnects ?? (VssServiceHost.\u003C\u003EO.\u003C1\u003E__MonitorClientDisconnects = new TeamFoundationTaskCallback(VssServiceHost.MonitorClientDisconnects)), (object) null, clientDisconnectMonitorInterval));
                if (this.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
                {
                  executionEnvironment = systemRequestContext.ExecutionEnvironment;
                  if (executionEnvironment.IsCloudDeployment)
                    systemRequestContext.GetService<IisLogsCleanupService>();
                }
                if (this.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
                {
                  systemRequestContext.Trace(59166, TraceLevel.Info, "HostManagement", "ServiceHost", "Adding ImpactedUsersMetric Task");
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  service4.AddTask(vssRequestContext1, new TeamFoundationTask(VssServiceHost.\u003C\u003EO.\u003C2\u003E__ImpactedUsersMetric ?? (VssServiceHost.\u003C\u003EO.\u003C2\u003E__ImpactedUsersMetric = new TeamFoundationTaskCallback(VssServiceHost.ImpactedUsersMetric)), (object) null, (int) this.m_impactedUsersMetricIntervalDefault.TotalMilliseconds));
                }
              }
              executionEnvironment = systemRequestContext.ExecutionEnvironment;
              if (executionEnvironment.IsHostedDeployment && systemRequestContext.IsHostProcessType(HostProcessType.JobAgent))
              {
                systemRequestContext.Trace(59164, TraceLevel.Info, "HostManagement", "ServiceHost", "Adding Job Timeout Task");
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                service4.AddTask(vssRequestContext1, new TeamFoundationTask(VssServiceHost.\u003C\u003EO.\u003C3\u003E__MonitorJobTimeouts ?? (VssServiceHost.\u003C\u003EO.\u003C3\u003E__MonitorJobTimeouts = new TeamFoundationTaskCallback(VssServiceHost.MonitorJobTimeouts)), (object) null, jobTimeoutMonitorInterval));
              }
              if (num > 0)
              {
                systemRequestContext.Trace(59165, TraceLevel.Info, "HostManagement", "ServiceHost", "Adding Oi Heartbeat Task");
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                service4.AddTask(vssRequestContext1, new TeamFoundationTask(VssServiceHost.\u003C\u003EO.\u003C4\u003E__OiHeartbeat ?? (VssServiceHost.\u003C\u003EO.\u003C4\u003E__OiHeartbeat = new TeamFoundationTaskCallback(VssServiceHost.OiHeartbeat)), (object) null, num * 1000));
              }
            }
          }
        }
        else
          systemRequestContext.Trace(59045, TraceLevel.Info, "HostManagement", "ServiceHost", "IVssServiceHost::Start Ignoring Status Change (Starting) for Host {0}", (object) this.InstanceId);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(59046, "HostManagement", "ServiceHost", ex);
        throw;
      }
      finally
      {
        if (flag)
        {
          systemRequestContext.Trace(59047, TraceLevel.Info, "HostManagement", "ServiceHost", "Ending Status Change for Host {0}", (object) this.InstanceId);
          this.EndStatusChange(systemRequestContext.To(TeamFoundationHostType.Deployment));
        }
        systemRequestContext.TraceLeave(59048, "HostManagement", "ServiceHost", "IVssServiceHost.Start");
      }
    }

    internal override void Stop(VssRequestContext systemRequestContext, bool force) => this.Stop(systemRequestContext, true, force);

    internal virtual void Stop(
      VssRequestContext systemRequestContext,
      bool stopCoreServices,
      bool force)
    {
      this.CheckDisposed();
      TeamFoundationTracingService.TraceEnterRaw(59050, "HostManagement", "ServiceHost", "Stop {0}", new Guid?(this.InstanceId), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        bool flag = this.BeginStatusChange(systemRequestContext.To(TeamFoundationHostType.Deployment), TeamFoundationServiceHostStatus.Stopping, force);
        TeamFoundationTracingService.TraceRaw(59052, TraceLevel.Info, "HostManagement", "ServiceHost", "Status Changing for Host {0} to Stopping: {1}", (object) this.InstanceId, (object) flag);
        TeamFoundationTracingService.TraceRaw(59053, TraceLevel.Info, "HostManagement", "ServiceHost", "Cancelling all requests for Host {0}", (object) this.InstanceId);
        this.CancelAllRequests(TimeSpan.FromSeconds(5.0));
        try
        {
          TeamFoundationTaskService service = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
          if (this.m_monitorEnabled)
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            service.RemoveTask((IVssRequestContext) systemRequestContext, new TeamFoundationTask(VssServiceHost.\u003C\u003EO.\u003C0\u003E__MonitorServiceHostRequests ?? (VssServiceHost.\u003C\u003EO.\u003C0\u003E__MonitorServiceHostRequests = new TeamFoundationTaskCallback(VssServiceHost.MonitorServiceHostRequests))));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          service.RemoveTask((IVssRequestContext) systemRequestContext, new TeamFoundationTask(VssServiceHost.\u003C\u003EO.\u003C1\u003E__MonitorClientDisconnects ?? (VssServiceHost.\u003C\u003EO.\u003C1\u003E__MonitorClientDisconnects = new TeamFoundationTaskCallback(VssServiceHost.MonitorClientDisconnects))));
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          service.RemoveTask((IVssRequestContext) systemRequestContext, new TeamFoundationTask(VssServiceHost.\u003C\u003EO.\u003C3\u003E__MonitorJobTimeouts ?? (VssServiceHost.\u003C\u003EO.\u003C3\u003E__MonitorJobTimeouts = new TeamFoundationTaskCallback(VssServiceHost.MonitorJobTimeouts))));
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(59054, TraceLevel.Error, "HostManagement", "ServiceHost", ex);
        }
        TeamFoundationTracingService.TraceRaw(59055, TraceLevel.Info, "HostManagement", "ServiceHost", "Stopping all services for  Host {0}", (object) this.InstanceId);
        this.StopServices((IVssRequestContext) systemRequestContext, stopCoreServices);
        if (stopCoreServices)
        {
          try
          {
            systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().RemoveAllTasks(this.InstanceId);
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(59058, TraceLevel.Error, "HostManagement", "ServiceHost", ex);
          }
        }
        if (!flag)
          return;
        TeamFoundationTracingService.TraceRaw(59054, TraceLevel.Info, "HostManagement", "ServiceHost", "Ending Status Change for Host {0}", (object) this.InstanceId);
        this.EndStatusChange(systemRequestContext.To(TeamFoundationHostType.Deployment));
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(59056, TraceLevel.Error, "HostManagement", "ServiceHost", ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(59057, "HostManagement", "ServiceHost", nameof (Stop));
      }
    }

    internal override bool FlushNotificationQueue(VssRequestContext requestContext)
    {
      TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
      return this.ServiceHostInternal().FlushNotificationQueue((IVssRequestContext) requestContext, service.ProcessId, VssServiceHost.s_flushTimeout);
    }

    internal override void UpdateHostProperties(
      VssRequestContext requestContext,
      HostProperties hostProperties)
    {
      this.CheckDisposed();
      int status = (int) this.m_hostProperties.Status;
      try
      {
        this.m_hostPropertiesLock.AcquireWriterLock(-1);
        TeamFoundationTracingService.TraceRaw(59075, TraceLevel.Info, "HostManagement", "ServiceHost", "Updating Host Properties from: {0} to {1}", (object) this.m_hostProperties, (object) hostProperties);
        if (hostProperties.Id != requestContext.ServiceHost.DeploymentServiceHost.InstanceId && hostProperties.DatabaseId != this.m_hostProperties.DatabaseId && hostProperties.DatabaseId != DatabaseManagementConstants.InvalidDatabaseId)
        {
          TeamFoundationTracingService.TraceRaw(59078, this.m_hostProperties.Status == TeamFoundationServiceHostStatus.Stopped ? TraceLevel.Info : TraceLevel.Error, "HostManagement", "ServiceHost", "Host {0} moved from database {1} to {2}. Status: {3}.", (object) this.m_hostProperties.Id, (object) this.m_hostProperties.DatabaseId, (object) hostProperties.DatabaseId, (object) this.m_hostProperties.Status);
          this.m_partitionId = this.GetPartitionId((IVssRequestContext) requestContext, hostProperties);
          this.StopServices((IVssRequestContext) requestContext, true);
        }
        this.m_hostProperties.UpdateProperties(hostProperties);
      }
      finally
      {
        if (this.m_hostPropertiesLock.IsWriterLockHeld)
          this.m_hostPropertiesLock.ReleaseWriterLock();
      }
      this.RaisePropertiesChanged((IVssRequestContext) requestContext, hostProperties);
    }

    protected bool IsRegistered
    {
      get => this.m_isRegistered;
      set => this.m_isRegistered = value;
    }

    private void InitializeDataDirectoryRoot(VssRequestContext requestContext)
    {
      if (this.m_dataDirectoryRoot != null)
        return;
      if (this.ParentServiceHost != null)
      {
        this.m_dataDirectoryRoot = this.m_parentServiceHost.m_dataDirectoryRoot;
      }
      else
      {
        this.m_dataDirectoryRoot = ConfigurationManager.AppSettings["dataDirectory"];
        if (this.m_dataDirectoryRoot == null && this.HasDatabaseAccess)
          this.m_dataDirectoryRoot = RegistryHelpers.GetDeploymentValueRaw(requestContext.FrameworkConnectionInfo, FrameworkServerConstants.DataDirectoryRegistryPath);
        if (!string.IsNullOrEmpty(this.m_dataDirectoryRoot))
        {
          try
          {
            if (!Path.IsPathRooted(this.m_dataDirectoryRoot))
            {
              TeamFoundationEventLog.Default.Log((IVssRequestContext) requestContext, FrameworkResources.DataDirectoryNotAbsolute((object) this.m_dataDirectoryRoot), TeamFoundationEventId.ConfigurationError, EventLogEntryType.Warning);
              this.m_dataDirectoryRoot = string.Empty;
            }
          }
          catch (Exception ex)
          {
            TeamFoundationEventLog.Default.LogException((IVssRequestContext) requestContext, FrameworkResources.DataDirectoryConfigurationException((object) this.m_dataDirectoryRoot, (object) ex.Message), ex, TeamFoundationEventId.ConfigurationError, EventLogEntryType.Warning);
            this.m_dataDirectoryRoot = string.Empty;
          }
        }
        if (string.IsNullOrEmpty(this.m_dataDirectoryRoot) && !string.IsNullOrEmpty(this.PhysicalDirectory))
          this.m_dataDirectoryRoot = TFCommonUtil.CombinePaths(this.PhysicalDirectory, "_tfs_data");
      }
      if (this.m_dataDirectoryRoot != null)
        return;
      this.m_dataDirectoryRoot = string.Empty;
    }

    private void InitializeDataDirectory(VssRequestContext requestContext)
    {
      if (!string.IsNullOrEmpty(this.m_dataDirectory))
        return;
      this.InitializeDataDirectoryRoot(requestContext);
      this.m_dataDirectory = TFCommonUtil.CombinePaths(this.m_dataDirectoryRoot, this.InstanceId.ToString("D"));
    }

    private void UpdatePerformanceCounter(
      IVssRequestContext requestContext,
      VssServiceHost.PerformanceCounterOperationType type)
    {
      TeamFoundationTracingService.TraceRaw(59015, TraceLevel.Info, "HostManagement", "ServiceHost", "Host counter increment: host type: {0}", (object) this.m_hostProperties.HostType);
      VssPerformanceCounter performanceCounter;
      if ((this.m_hostProperties.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ActiveDeploymentHosts");
      else if ((this.m_hostProperties.HostType & TeamFoundationHostType.Application) == TeamFoundationHostType.Application)
      {
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ActiveApplicationHosts");
      }
      else
      {
        if ((this.m_hostProperties.HostType & TeamFoundationHostType.ProjectCollection) != TeamFoundationHostType.ProjectCollection)
          return;
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ActiveCollectionHosts");
      }
      if (type == VssServiceHost.PerformanceCounterOperationType.Increment)
        performanceCounter.Increment();
      else
        performanceCounter.Decrement();
    }

    private int GetPartitionId(IVssRequestContext requestContext, HostProperties hostProperties)
    {
      if (hostProperties.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        return DatabasePartitionConstants.DeploymentHostPartitionId;
      using (DatabasePartitionComponent partitionComponent = VssServiceHost.CreateDatabasePartitionComponent(requestContext, hostProperties.DatabaseId))
        return (partitionComponent.QueryPartition(this.InstanceId) ?? throw new DatabasePartitionNotFoundException(this.InstanceId)).PartitionId;
    }

    private static DatabasePartitionComponent CreateDatabasePartitionComponent(
      IVssRequestContext requestContext,
      int databaseId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationResourceManagementService service = vssRequestContext.GetService<TeamFoundationResourceManagementService>();
      try
      {
        return service.CreateDatabaseComponent<DatabasePartitionComponent>(vssRequestContext, databaseId, (string) null);
      }
      catch (ServiceNotRegisteredException ex)
      {
        if (!vssRequestContext.ExecutionEnvironment.IsHostedDeployment)
          return DatabasePartitionComponent.CreateComponent(vssRequestContext.GetService<TeamFoundationDatabaseManagementService>().GetSqlConnectionInfo(vssRequestContext, databaseId));
        throw;
      }
    }

    protected void RaiseStatusChanged(Guid instanceId, TeamFoundationServiceHostStatus status)
    {
      this.CheckDisposed();
      if (this.StatusChanged == null)
        return;
      TeamFoundationTrace.Info(TraceKeywordSets.API, "Raising Status Changed for {0} with status {1}", (object) instanceId, (object) status);
      this.StatusChanged((object) this, new HostStatusChangedEventArgs(instanceId, status));
    }

    private void StartServices()
    {
    }

    protected void StopServices(IVssRequestContext systemRequestContext, bool stopCoreServices)
    {
      this.CheckDisposed();
      this.m_serviceProvider.StopServices(systemRequestContext, stopCoreServices);
    }

    private bool SetHostStatus(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostStatus newStatus)
    {
      this.CheckDisposed();
      return !this.IsRegistered || requestContext.GetService<TeamFoundationHostManagementService>().SetHostStatus(requestContext.To(TeamFoundationHostType.Deployment), this.InstanceId, newStatus);
    }

    private bool BeginStatusChange(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostStatus status,
      bool force)
    {
      TeamFoundationTracingService.TraceEnterRaw(59083, "HostManagement", "ServiceHost", "BeginStatusChange {0} {1}", (object) this.InstanceId, (object) status);
      this.CheckDisposed();
      try
      {
        this.m_hostPropertiesLock.AcquireWriterLock(-1);
        TeamFoundationTracingService.TraceRaw(59084, TraceLevel.Info, "HostManagement", "ServiceHost", "Beginning Status Change {2} from {0} to {1}", (object) this.m_hostProperties.Status, (object) status, (object) this);
        bool flag;
        try
        {
          flag = this.SetHostStatus(requestContext, status);
          if (flag)
            TeamFoundationTracingService.TraceRaw(59085, TraceLevel.Info, "HostManagement", "ServiceHost", "Host Status Changed for Host {0} - Status is now {1}", (object) this.InstanceId, (object) status);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(59086, TraceLevel.Error, "HostManagement", "ServiceHost", ex);
          if (ex is TeamFoundationServiceException)
          {
            if (status == TeamFoundationServiceHostStatus.Stopping && (force || ex is HostProcessNotFoundException))
            {
              TeamFoundationTracingService.TraceRaw(59087, TraceLevel.Info, "HostManagement", "ServiceHost", "Transitioning without notifying SQL");
              flag = true;
            }
            else
            {
              TeamFoundationTracingService.TraceRaw(59089, TraceLevel.Warning, "HostManagement", "ServiceHost", "Can't transition because exception type is {0} and status is {1} and force is {2}", (object) ex.GetType().Name, (object) (int) status, (object) force);
              throw;
            }
          }
          else
          {
            TeamFoundationTracingService.TraceRaw(59090, TraceLevel.Warning, "HostManagement", "ServiceHost", "Can't transition because exception type is {0}", (object) ex.GetType().Name);
            throw;
          }
        }
        if (flag)
        {
          this.m_hostProperties.Status = status;
          this.RaiseStatusChanged(this.InstanceId, status);
        }
        return flag;
      }
      finally
      {
        if (this.m_hostPropertiesLock.IsWriterLockHeld)
          this.m_hostPropertiesLock.ReleaseWriterLock();
        TeamFoundationTracingService.TraceLeaveRaw(59088, "HostManagement", "ServiceHost", nameof (BeginStatusChange));
      }
    }

    private void EndStatusChange(IVssRequestContext requestContext)
    {
      TeamFoundationTracingService.TraceEnterRaw(59090, "HostManagement", "ServiceHost", nameof (EndStatusChange), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      this.CheckDisposed();
      try
      {
        this.m_hostPropertiesLock.AcquireWriterLock(-1);
        TeamFoundationServiceHostStatus serviceHostStatus = this.m_hostProperties.Status;
        if (this.m_hostProperties.Status == TeamFoundationServiceHostStatus.Starting)
          serviceHostStatus = TeamFoundationServiceHostStatus.Started;
        else if (this.m_hostProperties.Status == TeamFoundationServiceHostStatus.Stopping)
          serviceHostStatus = TeamFoundationServiceHostStatus.Stopped;
        TeamFoundationTracingService.TraceRaw(59099, TraceLevel.Info, "HostManagement", "ServiceHost", "Ending Status Change {2} from {0} to {1}", (object) this.m_hostProperties.Status, (object) serviceHostStatus, (object) this);
        if (serviceHostStatus == this.m_hostProperties.Status)
          return;
        bool flag;
        try
        {
          flag = this.SetHostStatus(requestContext, serviceHostStatus);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(59100, TraceLevel.Error, "HostManagement", "ServiceHost", ex);
          if (ex is TeamFoundationServiceException)
          {
            if (serviceHostStatus == TeamFoundationServiceHostStatus.Stopped)
            {
              flag = true;
            }
            else
            {
              TeamFoundationTracingService.TraceRaw(59101, TraceLevel.Info, "HostManagement", "ServiceHost", "Failing Transition because exception type is {0} and newStatus is {1}", (object) ex.GetType().Name, (object) (int) serviceHostStatus);
              throw;
            }
          }
          else
          {
            TeamFoundationTracingService.TraceRaw(59102, TraceLevel.Info, "HostManagement", "ServiceHost", "Failing Transition because exception type is {0}", (object) ex.GetType().Name);
            throw;
          }
        }
        if (!flag)
          return;
        this.m_hostProperties.Status = serviceHostStatus;
        this.RaiseStatusChanged(this.InstanceId, serviceHostStatus);
      }
      finally
      {
        if (this.m_hostPropertiesLock.IsWriterLockHeld)
          this.m_hostPropertiesLock.ReleaseWriterLock();
        TeamFoundationTracingService.TraceLeaveRaw(59101, "HostManagement", "ServiceHost", nameof (EndStatusChange));
      }
    }

    private void ReadServiceHostMonitorSettings(
      IVssRequestContext requestContext,
      SqlRegistryService registryService,
      out int queuedRequestThreshold,
      out long queuedRequestElapsedThreshold,
      out long totalExecutionElapsedThreshold,
      out int healthMonitorInterval,
      out bool monitorEnabled,
      out int clientDisconnectMonitorInterval,
      out int jobTimeoutMonitorInterval)
    {
      RegistryEntryCollection registryEntryCollection = registryService.ReadEntriesFallThru(requestContext, (RegistryQuery) (FrameworkServerConstants.ServiceHostMonitorRoot + "/*"));
      queuedRequestThreshold = registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.QueuedRequestThreshold, 10);
      queuedRequestElapsedThreshold = registryEntryCollection.GetValueFromPath<long>(FrameworkServerConstants.QueuedRequestElapsedThreshold, 15L);
      totalExecutionElapsedThreshold = registryEntryCollection.GetValueFromPath<long>(FrameworkServerConstants.TotalExecutionElapsedThreshold, 10L);
      healthMonitorInterval = registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.RequestMonitorInterval, 15000);
      monitorEnabled = registryEntryCollection.GetValueFromPath<bool>(FrameworkServerConstants.ServiceHostMonitorEnabled, false);
      clientDisconnectMonitorInterval = registryService.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.ClientDisconnectMonitorInterval, true, 30000);
      jobTimeoutMonitorInterval = registryService.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Application/JobTimeoutMonitorInterval", true, 30000);
    }

    private CultureInfo GetInstalledUICulture(IVssRequestContext requestContext)
    {
      this.CheckDisposed();
      SqlRegistryService service = requestContext.GetService<SqlRegistryService>();
      CultureInfo installedUiCulture = CultureInfo.InstalledUICulture;
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) FrameworkServerConstants.InstalledUICulture;
      string str = service.GetValue(requestContext1, in local, true);
      if (str != null)
      {
        int length = str.IndexOf(FrameworkServerConstants.InstalledUICultureSeparator);
        if (length != -1)
          str = str.Substring(0, length);
        try
        {
          installedUiCulture = new CultureInfo(Convert.ToInt32(str, (IFormatProvider) CultureInfo.InvariantCulture));
        }
        catch (Exception ex)
        {
          TeamFoundationEventLog.Default.Log(requestContext, FrameworkResources.InstalledUICultureUnavailable(), TeamFoundationEventId.InstalledUICultureNotFound, EventLogEntryType.Warning);
        }
      }
      return installedUiCulture;
    }

    protected void RaisePropertiesChanged(
      IVssRequestContext requestContext,
      HostProperties serviceHostProperties)
    {
      this.CheckDisposed();
      if (this.PropertiesChanged == null)
        return;
      TeamFoundationTrace.Info(TraceKeywordSets.API, "Raising Properties Changed for {0}", (object) serviceHostProperties.Id);
      this.PropertiesChanged((object) this, new HostPropertiesChangedEventArgs(requestContext, serviceHostProperties));
    }

    public static string ToString(string hostId, string hostName) => !string.IsNullOrEmpty(hostName) ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1})", (object) hostId, (object) hostName) : hostId;

    public static string ToString(Guid hostId, string hostName) => VssServiceHost.ToString(hostId.ToString(), hostName);

    public static TimeSpan FlushTimeout() => VssServiceHost.s_flushTimeout;

    private static void MonitorClientDisconnects(IVssRequestContext requestContext, object taskArgs)
    {
      TeamFoundationTracingService.TraceEnterRaw(57560, "HostManagement", "ServiceHost", nameof (MonitorClientDisconnects), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        VssServiceHost.ForeachActiveRequest(requestContext, VssServiceHost.\u003C\u003EO.\u003C5\u003E__CancelTimedOutRequests ?? (VssServiceHost.\u003C\u003EO.\u003C5\u003E__CancelTimedOutRequests = new Action<IVssRequestContext, IVssRequestContext>(VssServiceHost.CancelTimedOutRequests)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(57568, "HostManagement", "ServiceHost", ex);
      }
      finally
      {
        requestContext.TraceLeave(57569, "HostManagement", "ServiceHost", nameof (MonitorClientDisconnects));
      }
    }

    private static void MonitorJobTimeouts(IVssRequestContext requestContext, object taskArgs)
    {
      TeamFoundationTracingService.TraceEnterRaw(57560, "HostManagement", "ServiceHost", nameof (MonitorJobTimeouts), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        VssServiceHost.ForeachActiveRequest(requestContext, VssServiceHost.\u003C\u003EO.\u003C6\u003E__CancelTimedOutJob ?? (VssServiceHost.\u003C\u003EO.\u003C6\u003E__CancelTimedOutJob = new Action<IVssRequestContext, IVssRequestContext>(VssServiceHost.CancelTimedOutJob)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(57568, "HostManagement", "ServiceHost", ex);
      }
      finally
      {
        requestContext.TraceLeave(57569, "HostManagement", "ServiceHost", nameof (MonitorJobTimeouts));
      }
    }

    private static void MonitorServiceHostRequests(
      IVssRequestContext requestContext,
      object taskArgs)
    {
      TeamFoundationTracingService.TraceEnterRaw(57550, "HostManagement", "ServiceHost", nameof (MonitorServiceHostRequests), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        VssServiceHost.MonitorRequests(requestContext);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57551, TraceLevel.Error, "HostManagement", "ServiceHost", ex);
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57552, "HostManagement", "ServiceHost", nameof (MonitorServiceHostRequests));
      }
    }

    private static void OiHeartbeat(IVssRequestContext requestContext, object taskArgs)
    {
      TeamFoundationTracingService.TraceEnterRaw(57570, "HostManagement", "ServiceHost", nameof (OiHeartbeat), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        VssServiceHost.GenerateOiHeartbeats(requestContext);
        VssServiceHost.SendMdmDeploymentStatusEvent(requestContext);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(57571, TraceLevel.Error, "HostManagement", "ServiceHost", ex);
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(57572, "HostManagement", "ServiceHost", nameof (OiHeartbeat));
      }
    }

    private static void ImpactedUsersMetric(IVssRequestContext requestContext, object taskArgs) => requestContext.TraceBlock(57573, 57576, 57574, "HostManagement", "ServiceHost", nameof (ImpactedUsersMetric), (Action) (() => VssServiceHost.EmitImpactedUsersMetric(requestContext)));

    private static void MonitorRequests(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(59120, "HostManagement", "ServiceHost", nameof (MonitorRequests));
      requestContext.ServiceHost.CheckDisposed();
      if (!(requestContext.ServiceHost is VssServiceHost serviceHost))
      {
        requestContext.Trace(59121, TraceLevel.Info, "HostManagement", "ServiceHost", "ServiceHost cannot be cast to private type. Host: (0).", (object) requestContext.ServiceHost.InstanceId);
      }
      else
      {
        IVssRequestContext[] activeRequests = (IVssRequestContext[]) serviceHost.GetActiveRequests();
        if (activeRequests == null)
        {
          requestContext.Trace(59121, TraceLevel.Info, "HostManagement", "ServiceHost", "No active requests to monitor on host {0}.", (object) requestContext.ServiceHost.InstanceId);
        }
        else
        {
          requestContext.Trace(59122, TraceLevel.Info, "HostManagement", "ServiceHost", "Preparing to scan {0} active requests.", (object) requestContext.ServiceHost.NumberOfActiveRequests);
          try
          {
            int num1 = 0;
            long num2 = 0;
            long num3 = 0;
            IVssRequestContext requestContext1 = (IVssRequestContext) null;
            IVssRequestContext requestContext2 = (IVssRequestContext) null;
            foreach (VssRequestContext requestContext3 in activeRequests)
            {
              try
              {
                if (requestContext3.ExecutionTime() > num3)
                {
                  if (requestContext3.Method != null)
                  {
                    if (requestContext3.Method.KeepsHostAwake)
                    {
                      num3 = requestContext3.ExecutionTime();
                      requestContext2 = (IVssRequestContext) requestContext3;
                    }
                  }
                }
              }
              catch (Exception ex)
              {
                requestContext.TraceException(59126, "HostManagement", "ServiceHost", ex);
              }
            }
            requestContext.Trace(59127, TraceLevel.Info, "HostManagement", "ServiceHost", "Request queue statistics: queued requests: {0}, max elapsed queued time: {1} (microseconds), max elapsed active time: {2} (microseconds)", (object) num1, (object) num2, (object) num3);
            long num4 = num2 / 1000000L;
            long num5 = num3 / 1000000L;
            if (num1 > serviceHost.m_queuedRequestThreshold && !serviceHost.m_queuedRequestThresholdExceeded)
            {
              requestContext.Trace(59128, TraceLevel.Warning, "HostManagement", "ServiceHost", "QueuedRequestThresholdExceeded");
              serviceHost.m_queuedRequestThresholdExceeded = true;
              TeamFoundationEventLog.Default.Log(FrameworkResources.MonitorServiceHostMessageBrief(), FrameworkResources.QueuedRequestThresholdExceeded((object) serviceHost.Name, (object) num1, (object) serviceHost.m_queuedRequestThreshold), TeamFoundationEventId.QueuedRequestThresholdExceeded, EventLogEntryType.Warning);
            }
            else if (num1 <= serviceHost.m_queuedRequestThreshold && serviceHost.m_queuedRequestThresholdExceeded)
            {
              requestContext.Trace(59129, TraceLevel.Warning, "HostManagement", "ServiceHost", "QueuedRequestThresholdCleared");
              serviceHost.m_queuedRequestThresholdExceeded = false;
              TeamFoundationEventLog.Default.Log(FrameworkResources.MonitorServiceHostMessageBrief(), FrameworkResources.QueuedRequestThresholdCleared((object) serviceHost.Name, (object) num1, (object) serviceHost.m_queuedRequestThreshold), TeamFoundationEventId.QueuedRequestThresholdCleared, EventLogEntryType.Warning);
            }
            if (num4 > serviceHost.m_queuedRequestElapsedThreshold && !serviceHost.m_queuedRequestElapsedThresholdExceeded)
            {
              requestContext.Trace(59130, TraceLevel.Warning, "HostManagement", "ServiceHost", "QueuedRequestElapsedThresholdExceeded");
              serviceHost.m_queuedRequestElapsedThresholdExceeded = true;
              TeamFoundationEventLog.Default.Log(requestContext1, FrameworkResources.MonitorServiceHostMessageBrief(), FrameworkResources.QueuedRequestElapsedThresholdExceeded((object) serviceHost.Name, (object) num4, (object) serviceHost.m_queuedRequestElapsedThreshold, (object) requestContext1.ToString()), TeamFoundationEventId.QueuedRequestElapsedThresholdExceeded, EventLogEntryType.Warning);
            }
            else if (num4 <= serviceHost.m_queuedRequestElapsedThreshold && serviceHost.m_queuedRequestElapsedThresholdExceeded)
            {
              requestContext.Trace(59131, TraceLevel.Warning, "HostManagement", "ServiceHost", "QueuedRequestElapsedThresholdCleared");
              serviceHost.m_queuedRequestElapsedThresholdExceeded = false;
              TeamFoundationEventLog.Default.Log(FrameworkResources.MonitorServiceHostMessageBrief(), FrameworkResources.QueuedRequestElapsedThresholdCleared((object) serviceHost.Name, (object) serviceHost.m_queuedRequestElapsedThreshold), TeamFoundationEventId.QueuedRequestElapsedThresholdCleared, EventLogEntryType.Warning);
            }
            if (num5 > serviceHost.m_totalExecutionElapsedThreshold && !serviceHost.m_totalExecutionElapsedThresholdExceeded)
            {
              requestContext.Trace(59132, TraceLevel.Warning, "HostManagement", "ServiceHost", "TotalExecutionElapsedThresholdExceeded");
              serviceHost.m_totalExecutionElapsedThresholdExceeded = true;
              TeamFoundationEventLog.Default.Log(requestContext2, FrameworkResources.MonitorServiceHostMessageBrief(), FrameworkResources.TotalExecutionElapsedThresholdExceeded((object) serviceHost.Name, (object) num5, (object) serviceHost.m_totalExecutionElapsedThreshold, (object) requestContext2.ToString()), TeamFoundationEventId.TotalExecutionElapsedThresholdExceeded, EventLogEntryType.Warning);
            }
            else if (num5 <= serviceHost.m_totalExecutionElapsedThreshold)
            {
              if (serviceHost.m_totalExecutionElapsedThresholdExceeded)
              {
                requestContext.Trace(59133, TraceLevel.Warning, "HostManagement", "ServiceHost", "TotalExecutionElapsedThresholdCleared");
                serviceHost.m_totalExecutionElapsedThresholdExceeded = false;
                TeamFoundationEventLog.Default.Log(FrameworkResources.MonitorServiceHostMessageBrief(), FrameworkResources.TotalExecutionElapsedThresholdCleared((object) serviceHost.Name, (object) serviceHost.m_totalExecutionElapsedThreshold), TeamFoundationEventId.TotalExecutionElapsedThresholdCleared, EventLogEntryType.Warning);
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(59134, "HostManagement", "ServiceHost", ex);
          }
          requestContext.TraceLeave(59135, "HostManagement", "ServiceHost", nameof (MonitorRequests));
        }
      }
    }

    private static void ForeachActiveRequest(
      IVssRequestContext systemRequestContext,
      Action<IVssRequestContext, IVssRequestContext> action)
    {
      systemRequestContext.TraceEnter(59140, "HostManagement", "ServiceHost", nameof (ForeachActiveRequest));
      try
      {
        systemRequestContext.ServiceHost.CheckDisposed();
        IVssRequestContext[] vssRequestContextArray = (IVssRequestContext[]) null;
        IServiceHostInternal serviceHostInternal = systemRequestContext.ServiceHost.ServiceHostInternal(false);
        if (serviceHostInternal != null)
          vssRequestContextArray = serviceHostInternal.GetActiveRequests();
        if (vssRequestContextArray == null || vssRequestContextArray.Length == 0)
        {
          systemRequestContext.Trace(59152, TraceLevel.Info, "HostManagement", "ServiceHost", "No active Requests. Internal Host Is Null: {0}, Active Requests Is Null: {1}.", (object) (serviceHostInternal == null), (object) (vssRequestContextArray == null));
        }
        else
        {
          foreach (IVssRequestContext vssRequestContext in vssRequestContextArray)
          {
            systemRequestContext.Trace(59153, TraceLevel.Verbose, "HostManagement", "ServiceHost", "Invoking action on active request ActivityId: {0}, Context Id: {1}", (object) vssRequestContext.ActivityId, (object) vssRequestContext.ContextId);
            action(systemRequestContext, vssRequestContext);
          }
        }
      }
      finally
      {
        systemRequestContext.TraceLeave(59145, "HostManagement", "ServiceHost", nameof (ForeachActiveRequest));
      }
    }

    private static void CancelTimedOutRequests(
      IVssRequestContext systemRequestContext,
      IVssRequestContext requestContext)
    {
      if (!requestContext.IsUserContext)
        return;
      if (!requestContext.RequestContextInternal().HasRequestTimedOut)
        return;
      try
      {
        IVssRequestContext requestContext1 = systemRequestContext;
        object[] objArray = new object[6];
        TimeSpan timeSpan = requestContext.RequestTimer.DelaySpan;
        objArray[0] = (object) timeSpan.TotalSeconds;
        timeSpan = requestContext.RequestTimer.ExecutionSpan;
        objArray[1] = (object) timeSpan.TotalSeconds;
        timeSpan = requestContext.RequestTimeout;
        objArray[2] = (object) timeSpan.TotalSeconds;
        objArray[3] = (object) requestContext.ServiceHost;
        objArray[4] = (object) requestContext;
        objArray[5] = (object) requestContext.Method;
        VssRequestContextExtensions.Trace(requestContext1, 59148, TraceLevel.Error, "HostManagement", "ServiceHost", "Request has exceeded execution timeout and will be cancelled. Delay: {0}s; Duration: {1}s; Timeout: {2}s. ServiceHost: {3}; Request: {4}; Method: {5}.", objArray);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(59143, "HostManagement", "ServiceHost", ex);
      }
      try
      {
        requestContext.Cancel(FrameworkResources.RequestTimeoutException(), HttpStatusCode.ServiceUnavailable);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(59144, "HostManagement", "ServiceHost", ex);
      }
    }

    private static void CancelTimedOutJob(
      IVssRequestContext systemRequestContext,
      IVssRequestContext requestContext)
    {
      if (!(requestContext is IJobRequestContext requestContext1))
        return;
      if (requestContext1.IsCanceled && requestContext1.RequestTimer.ExecutionSpan - VssServiceHost.s_maxCancellationWaitTime > requestContext1.RequestTimeout)
      {
        TeamFoundationEventLog.Default.Log((IVssRequestContext) requestContext1, string.Format("Job didn't respond to the cancellation request within the allotted time, {0}", (object) requestContext1), TeamFoundationEventId.JobCancellationTimeout, EventLogEntryType.Error);
        IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
        bool flag = vssRequestContext.IsFeatureEnabled("VisualStudio.Services.Framework.HealthAgentService.ResetOnJobCancellationTimeout");
        systemRequestContext.Trace(59149, TraceLevel.Error, "HostManagement", "ServiceHost", string.Format("Job with Id {0} didn't respond to the cancellation request within the allotted time. Requesting Reset: {1} Job Context: {2}", (object) requestContext1.JobId, (object) flag, (object) requestContext1));
        if (!flag)
          return;
        vssRequestContext.GetService<IHealthAgentService>().RequestReset(vssRequestContext, string.Format("Job cancellation timed out for job {0}.", (object) requestContext1.JobId));
      }
      else if (requestContext1.RequestContextInternal().HasRequestTimedOut)
      {
        try
        {
          IVssRequestContext requestContext2 = systemRequestContext;
          object[] objArray = new object[5]
          {
            (object) requestContext1.RequestTimer.DelaySpan.TotalSeconds,
            null,
            null,
            null,
            null
          };
          TimeSpan timeSpan = requestContext1.RequestTimer.ExecutionSpan;
          objArray[1] = (object) timeSpan.TotalSeconds;
          timeSpan = requestContext1.RequestTimeout;
          objArray[2] = (object) timeSpan.TotalSeconds;
          objArray[3] = (object) requestContext1.ServiceHost;
          objArray[4] = (object) requestContext1.JobId;
          VssRequestContextExtensions.Trace(requestContext2, 59148, TraceLevel.Error, "HostManagement", "ServiceHost", "Job has exceeded execution timeout and will be cancelled. Delay: {0}s; Duration: {1}s; Timeout: {2}s. ServiceHost: {3}; JobId: {4}.", objArray);
        }
        catch (Exception ex)
        {
          systemRequestContext.TraceException(59143, "HostManagement", "ServiceHost", ex);
        }
        try
        {
          requestContext1.Cancel(FrameworkResources.JobTimeoutException());
        }
        catch (Exception ex)
        {
          requestContext1.TraceException(59144, "HostManagement", "ServiceHost", ex);
        }
      }
      else
      {
        try
        {
          systemRequestContext.Trace(59150, TraceLevel.Info, "HostManagement", "ServiceHost", string.Format("Job execution for JobId {0} has not reached its timeout yet", (object) requestContext1.JobId));
        }
        catch (Exception ex)
        {
          systemRequestContext.TraceException(59151, "HostManagement", "ServiceHost", ex);
        }
      }
    }

    private static void SendMdmDeploymentStatusEvent(IVssRequestContext requestContext)
    {
      try
      {
        requestContext.Trace(57583, TraceLevel.Info, "HostManagement", "ServiceHost", "MdmDeploymentStatusTraceStart");
        requestContext.ServiceHost.CheckDisposed();
        string deploymentStatus = VssServiceHost.GetMdmDeploymentStatus(requestContext);
        TeamFoundationEventLog.Default.Log(requestContext, "MDM Deployment status", TeamFoundationEventId.MdmDeploymentStatus, EventLogEntryType.Information, (object[]) new string[1]
        {
          deploymentStatus
        });
        requestContext.Trace(57584, TraceLevel.Info, "HostManagement", "ServiceHost", string.Format("MdmDeploymentStatusTraceEnd. Status: {0}", (object) deploymentStatus));
      }
      catch
      {
        requestContext.Trace(57585, TraceLevel.Error, "HostManagement", "ServiceHost", "MdmDeploymentStatusTraceFailed");
      }
    }

    private static void EmitImpactedUsersMetric(IVssRequestContext requestContext)
    {
      try
      {
        IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
        if (!service1.GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.TracingServiceEnableImpactedUsersMetric, false))
          return;
        TeamFoundationTracingService service2 = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTracingService>();
        int minute = (DateTime.UtcNow.Minute + 59) % 60;
        int impactedUsersMetric = service2.GetImpactedUsersMetric(minute);
        service2.ClearStaleImpactedUsersMetric((minute + 1) % 60);
        KpiService service3 = requestContext.GetService<KpiService>();
        service3.EnsureKpiIsRegistered(requestContext, "ImpactedUsersMetric", "ImpactedUsersMetric", "ImpactedUsersMetric", "Impacted Users Metric", "Impacted Users Metric");
        service3.Publish(requestContext, "ImpactedUsersMetric", "ImpactedUsersMetric", "ImpactedUsersMetric", (double) impactedUsersMetric);
        long num1 = (long) service1.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.TracingServiceEnableImpactedUsersThreshold, 100);
        long num2 = (long) ((long) impactedUsersMetric > num1);
        service3.EnsureKpiIsRegistered(requestContext, "ImpactedUsersThresholdExceeded", "ImpactedUsersThresholdExceeded", "ImpactedUsersThresholdExceeded", "Impacted Users Threshold has been exceeded", "Impacted Users Threshold has been exceeded");
        service3.Publish(requestContext, "ImpactedUsersThresholdExceeded", "ImpactedUsersThresholdExceeded", "ImpactedUsersThresholdExceeded", (double) num2);
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ImpactedUsersInLastMinuteMetric").SetValue((long) impactedUsersMetric);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(57574, TraceLevel.Error, "HostManagement", "ServiceHost", ex);
      }
    }

    private static string GetMdmDeploymentStatus(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string deploymentStatus = service.GetValue<string>(requestContext, (RegistryQuery) VssServiceHost.GetMdmDeploymentStatusRegistryPath(requestContext, service), false, (string) null);
      if (string.IsNullOrEmpty(deploymentStatus))
        deploymentStatus = service.GetValue<string>(requestContext, (RegistryQuery) "/Diagnostics/Hosting/MdmDeploymentStatus", false, (string) null);
      return deploymentStatus;
    }

    private static string GetMdmDeploymentStatusRegistryPath(
      IVssRequestContext requestContext,
      IVssRegistryService registry)
    {
      string str1 = registry.GetValue<string>(requestContext, (RegistryQuery) "/Diagnostics/Hosting/MdmDiagnostics/MdmAccountName", false, (string) null);
      string str2 = registry.GetValue<string>(requestContext, (RegistryQuery) "/Diagnostics/Hosting/MdmDiagnostics/MdmNamespace", false, (string) null);
      if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
        return "/Diagnostics/Hosting/MdmDeploymentStatus";
      return "/" + str1 + "/" + str2 + "/mdmDeploymentStatus";
    }

    private static void GenerateOiHeartbeats(IVssRequestContext context)
    {
      context.TraceEnter(57580, "HostManagement", "ServiceHost", nameof (GenerateOiHeartbeats));
      context.ServiceHost.CheckDisposed();
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      vssRequestContext.Trace(57581, TraceLevel.Error, "HostManagement", "ServiceHost", "OiHeartbeat");
      TeamFoundationTracingService service1 = vssRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTracingService>();
      service1.TraceJobAgentHistory(vssRequestContext, "OiHeartbeat", "OiHeartbeat", vssRequestContext.ServiceHost.InstanceId, Guid.Empty, DateTime.UtcNow, DateTime.UtcNow, 1L, Guid.Empty, 0, "OiHeartbeat", 1, 0, (short) 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0L, 0, Guid.Empty, 0, 0, 0, 0L, Guid.Empty, Guid.Empty, 0L, 0L);
      IdentityTracingItems identityTracingItems = vssRequestContext.GetUserIdentityTracingItems();
      service1.TraceActivityLog(vssRequestContext.ServiceHost.InstanceId, vssRequestContext.ContextId, "Oi", "OiHeartbeat", 0, 1, DateTime.UtcNow, 0L, 0L, 1L, string.Empty, vssRequestContext.RemoteIPAddress(), vssRequestContext.UniqueIdentifier, string.Empty, "OiHeartbeat", (string) null, (string) null, vssRequestContext.ActivityId, vssRequestContext.ResponseCode, vssRequestContext.GetUserId(), identityTracingItems != null ? identityTracingItems.Cuid : Guid.Empty, identityTracingItems != null ? identityTracingItems.TenantId : Guid.Empty, identityTracingItems != null ? identityTracingItems.ProviderId : Guid.Empty, 1L, 0, string.Empty, 10000000L, false, 0, 0, 0, 0, "Service Insights", DateTime.UtcNow, (byte) 1, Guid.Empty, vssRequestContext.GetAnonymousIdentifier(), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0L, 0, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, 0L, 0.0, string.Empty, string.Empty, string.Empty, (byte) 0, Guid.Empty, 0L, 0L, 0L, 0L, (string) null, 0, 0, 0, 0L, (string) null, (string) null, (string) null, Guid.Empty);
      TeamFoundationTracingService.TraceServiceHostHistory(Guid.Empty, DateTime.UtcNow, (short) 2, Guid.Empty, "OIHeartbeat", "OIHeartbeat", DatabaseManagementConstants.InvalidDatabaseId, -1, "OIHeartbeat", (short) 1, "OIHeartbeat", (short) 3, DateTime.UtcNow, 0);
      vssRequestContext.GetService<CustomerIntelligenceService>().Publish(vssRequestContext, "OiHeartBeat", "OiHeartBeat", "OiHeartbeat", true);
      KpiService service2 = vssRequestContext.GetService<KpiService>();
      service2.EnsureKpiIsRegistered(vssRequestContext, "OiHeartbeat", "OiHeartbeat", "OiHeartbeat", "OI Heartbeat", "OI Heartbeat");
      service2.Publish(vssRequestContext, "OiHeartbeat", "OiHeartbeat", "OiHeartbeat", 1.0);
      vssRequestContext.TraceLeave(57582, "HostManagement", "ServiceHost", nameof (GenerateOiHeartbeats));
    }

    private enum PerformanceCounterOperationType
    {
      Increment,
      Decrement,
    }
  }
}
