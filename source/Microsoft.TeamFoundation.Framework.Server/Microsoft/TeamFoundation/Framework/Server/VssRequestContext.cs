// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssRequestContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.TeamFoundation.Framework.Server.HostManagement;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Performance;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Users.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssRequestContext : IVssRequestContext, IDisposable, IRequestContextInternal
  {
    private TimeSpan m_requestTimeout;
    private IVssLockManager m_lockManager;
    private VssRequestContext.LogRequest m_requestLogger;
    private VssRequestContext.TraceRequest m_requestTracer;
    private IReadOnlyList<IRequestActor> m_actors;
    private bool m_isRootBeingDisposed;
    private readonly object m_disposableResourcesLock;
    private HashSet<IDisposable> m_disposableResources;
    private int m_disposableSqlComponents;
    private Guid m_scopedToDataspace;
    private Dictionary<string, ILeaseInfo> m_leases;
    private string m_cancellationReason;
    private IdentityValidationStatus m_validationStatus;
    private IVssServiceHost m_serviceHost;
    private readonly Guid m_serviceHostIdForTracing;
    private string m_serviceName = string.Empty;
    protected string m_userAgent = string.Empty;
    private string m_authenticatedUserName = string.Empty;
    private string m_authenticationType = string.Empty;
    protected Guid m_uniqueIdentifier = Guid.Empty;
    private Guid m_e2eId;
    private string m_orchestrationId;
    private MethodInformation m_methodInfo;
    private Thread m_methodExecutionThread;
    private long m_cpuCycles;
    private long m_allocatedBytes;
    private Exception m_status;
    private IdentityDescriptor m_userContext;
    private string m_domainUserName = string.Empty;
    private TeamFoundationExecutionEnvironment m_executionEnvironment;
    private readonly RequestContextType m_requestContextType;
    private VssRequestContextPriority m_priority;
    private IDictionary<string, object> m_items;
    private int m_canceled;
    private List<ICancelable> m_cancelableObjects;
    private object m_contextLock = new object();
    private Dictionary<Guid, VssRequestContext> m_systemRequestContexts;
    private Dictionary<Guid, VssRequestContext> m_userRequestContexts;
    private Dictionary<Guid, VssRequestContext> m_servicingRequestContexts;
    private IList<IVssRequestContext> m_associatedRequestContexts;
    private readonly VssRequestContext m_rootContext;
    private static readonly string s_ProcessName;
    private bool m_isDisposed;
    private Guid m_activityId;
    private LockHelper m_lockHelper;
    private readonly long m_contextId;
    private bool m_isTracked;
    private static long s_contextIdCounter;
    private static readonly string s_area = "HostManagement";
    private static readonly string s_layer = "Kernel";
    private const string s_Area = "VssRequestContext";
    private const string s_Layer = "HostManagement";
    private static readonly string[] s_tagPerformance = new string[1]
    {
      "performance"
    };
    protected readonly VssRequestContext.TimeRequest m_timer;
    private readonly IVssHttpClientProvider m_clientProvider;
    private CancellationTokenSource m_cancellationTokenSource;
    private object m_cancellationTokenSourceLock = new object();
    private static bool s_shouldSuppressFinalizerWarnings = false;
    private readonly StackTracer m_constructorStackTrace;
    private static readonly Guid s_activityIdSource = Guid.NewGuid();
    private AsyncResourcesCounter m_resourcesCounter;

    static VssRequestContext()
    {
      using (Process currentProcess = Process.GetCurrentProcess())
        VssRequestContext.s_ProcessName = currentProcess.ProcessName;
      AppDomain.CurrentDomain.ProcessExit += new EventHandler(VssRequestContext.OnProcessExit);
    }

    private static void OnProcessExit(object sender, EventArgs e) => VssRequestContext.s_shouldSuppressFinalizerWarnings = true;

    protected VssRequestContext(
      IVssServiceHost serviceHost,
      RequestContextType requestContextType,
      LockHelper lockHelper,
      TimeSpan timeout,
      VssRequestContext rootContext)
    {
      ArgumentUtility.CheckForNull<IVssServiceHost>(serviceHost, nameof (serviceHost));
      this.m_lockHelper = lockHelper;
      this.m_contextId = Interlocked.Increment(ref VssRequestContext.s_contextIdCounter);
      this.m_serviceHost = serviceHost;
      this.m_serviceHostIdForTracing = serviceHost.InstanceId;
      this.m_rootContext = rootContext ?? this;
      this.m_requestContextType = requestContextType;
      if (rootContext != null)
      {
        this.m_activityId = rootContext.ActivityId;
      }
      else
      {
        this.m_activityId = Trace.CorrelationManager.ActivityId;
        if (this.m_activityId == Guid.Empty)
          this.ResetActivityId();
        this.m_cancellationTokenSource = new CancellationTokenSource();
      }
      this.m_e2eId = this.m_activityId;
      this.m_disposableResourcesLock = new object();
      this.m_disposableResources = new HashSet<IDisposable>();
      if (rootContext == null)
      {
        this.m_timer = new VssRequestContext.TimeRequest((IVssRequestContext) this);
        this.m_timer.BeginRequest();
        this.RequestTimeout = timeout;
        this.m_priority = VssRequestContextPriority.Normal;
        this.m_clientProvider = (IVssHttpClientProvider) new Microsoft.TeamFoundation.Framework.Server.ClientProvider();
      }
      if (this.m_requestContextType == RequestContextType.ServicingContext && !(this is IVssWebRequestContext) || this.m_requestContextType == RequestContextType.SystemContext)
      {
        IRequestActor actor = serviceHost.SystemActor();
        this.m_actors = actor != null ? actor.ToList() : (IReadOnlyList<IRequestActor>) null;
      }
      if (!TeamFoundationTracingService.IsRawTracingEnabled(36999, TraceLevel.Info, nameof (VssRequestContext), "HostManagement", (string[]) null))
        return;
      this.m_constructorStackTrace = new StackTracer();
    }

    ~VssRequestContext()
    {
      if (this.m_isDisposed)
        return;
      if (VssRequestContext.s_shouldSuppressFinalizerWarnings)
        return;
      try
      {
        if (this.m_constructorStackTrace != null)
          TeamFoundationTracingService.TraceRaw(36100, TraceLevel.Error, nameof (VssRequestContext), "HostManagement", "RequestContext finalizer without dispose {0}", (object) this.m_constructorStackTrace);
        else
          TeamFoundationTracingService.TraceRaw(36100, TraceLevel.Error, nameof (VssRequestContext), "HostManagement", "RequestContext finalizer without dispose {0}", (object) this.ToString());
      }
      catch (Exception ex)
      {
      }
    }

    public void Dispose() => this.Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_isDisposed)
        return;
      TeamFoundationTracingService.TraceEnterRaw(58000, VssRequestContext.s_area, VssRequestContext.s_layer, nameof (Dispose), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      if (this.m_rootContext == this)
      {
        this.m_isRootBeingDisposed = true;
        if (!this.m_serviceHost.IsDisposed)
          this.ValidateRequest();
      }
      else if (!this.m_rootContext.m_isRootBeingDisposed)
      {
        TeamFoundationTracingService.TraceRaw(36108, TraceLevel.Error, nameof (VssRequestContext), "HostManagement", "Caller is attempting to dispose of an internal request");
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The request context {0} is owned by the system and cannot be directly disposed", (object) this.m_contextId));
      }
      try
      {
        try
        {
          this.EndRequest();
          this.m_methodExecutionThread = (Thread) null;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(36110, nameof (VssRequestContext), "HostManagement", ex);
        }
        if (this.m_systemRequestContexts != null || this.m_userRequestContexts != null || this.m_servicingRequestContexts != null)
        {
          lock (this.m_contextLock)
          {
            if (this.m_systemRequestContexts != null)
            {
              foreach (IVssRequestContext vssRequestContext in this.m_systemRequestContexts.Values)
              {
                try
                {
                  vssRequestContext.Dispose();
                }
                catch (Exception ex)
                {
                  TeamFoundationTracingService.TraceExceptionRaw(36111, nameof (VssRequestContext), "HostManagement", ex);
                }
              }
              this.m_systemRequestContexts = (Dictionary<Guid, VssRequestContext>) null;
            }
            if (this.m_userRequestContexts != null)
            {
              foreach (IVssRequestContext vssRequestContext in this.m_userRequestContexts.Values)
              {
                try
                {
                  vssRequestContext.Dispose();
                }
                catch (Exception ex)
                {
                  TeamFoundationTracingService.TraceExceptionRaw(36112, nameof (VssRequestContext), "HostManagement", ex);
                }
              }
              this.m_userRequestContexts = (Dictionary<Guid, VssRequestContext>) null;
            }
            if (this.m_servicingRequestContexts != null)
            {
              foreach (IVssRequestContext vssRequestContext in this.m_servicingRequestContexts.Values)
              {
                try
                {
                  vssRequestContext.Dispose();
                }
                catch (Exception ex)
                {
                  TeamFoundationTracingService.TraceExceptionRaw(36113, nameof (VssRequestContext), "HostManagement", ex);
                }
              }
              this.m_servicingRequestContexts = (Dictionary<Guid, VssRequestContext>) null;
            }
            lock (this.m_cancellationTokenSourceLock)
            {
              if (this.m_cancellationTokenSource != null)
              {
                this.m_cancellationTokenSource.Dispose();
                this.m_cancellationTokenSource = (CancellationTokenSource) null;
              }
            }
          }
        }
        if (this.m_timer != null)
          this.m_timer.Dispose();
        if (this.m_resourcesCounter != null)
          this.m_resourcesCounter.Dispose();
        if (this.m_clientProvider != null && this.m_clientProvider is IDisposable clientProvider)
          clientProvider.Dispose();
        this.m_isDisposed = true;
        this.m_serviceHost = (IVssServiceHost) null;
        GC.SuppressFinalize((object) this);
      }
      finally
      {
        try
        {
          if (disposing)
          {
            if (this.m_lockHelper != null)
            {
              this.m_lockHelper.Dispose();
              this.m_lockHelper = (LockHelper) null;
            }
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(58004, VssRequestContext.s_area, VssRequestContext.s_layer, ex);
          throw;
        }
        finally
        {
          TeamFoundationTracingService.TraceLeaveRaw(58005, VssRequestContext.s_area, VssRequestContext.s_layer, nameof (Dispose));
        }
      }
    }

    public override string ToString() => this.GetSummary();

    private void ValidateRequest()
    {
      if (this.m_serviceHost == null)
        return;
      TeamFoundationHostManagementService hostManagement = this.m_serviceHost.DeploymentServiceHost.DeploymentServiceHostInternal().HostManagement;
      if (hostManagement != null && !hostManagement.CheckRequestId(this.RequestId))
      {
        TeamFoundationTracingService.TraceRaw(36119, TraceLevel.Error, nameof (VssRequestContext), "HostManagement", "You may not call this function on a different context (thread) than the original request at: {0}", (object) new StackTracer().ToString());
        throw new InvalidOperationException("You may not call this function on a different context (thread) than the original request. To prevent lifetime and scoping related errors, do not use the same VssRequestContext instance in multiple threads or across threads. Instead, only pass the data you need or create a new request context for each thread.");
      }
    }

    private void CheckDisposed()
    {
      if (this.m_isDisposed)
        throw new ObjectDisposedException(nameof (VssRequestContext));
    }

    private void CheckCanceled(bool throwIfShutdown)
    {
      if (this.IsCanceled)
        throw this.CreateRequestCanceledException();
      if (throwIfShutdown && !this.IsServicingContext && !this.m_rootContext.IsServicingContext)
        this.m_serviceHost.ServiceHostInternal().CheckShutdown();
      Dictionary<string, ILeaseInfo> leases = this.m_rootContext.m_leases;
      // ISSUE: explicit non-virtual call
      if ((leases != null ? (__nonvirtual (leases.Count) > 0 ? 1 : 0) : 0) == 0)
        return;
      foreach (ILeaseInfo leaseInfo in this.m_rootContext.m_leases.Values)
      {
        if (leaseInfo.LeaseExpires < DateTime.UtcNow)
          throw new LeaseLostException(leaseInfo.LeaseName, leaseInfo.LeaseOwner, leaseInfo.LeaseObtained, leaseInfo.ProcessId, leaseInfo.LeaseExpires);
      }
    }

    public virtual Guid ActivityId => this.m_activityId;

    public CancellationToken CancellationToken
    {
      get
      {
        if (!this.IsRootContext)
          return this.m_rootContext.CancellationToken;
        lock (this.m_cancellationTokenSourceLock)
          return this.m_cancellationTokenSource != null ? this.m_cancellationTokenSource.Token : throw new ObjectDisposedException("m_cancellationTokenSource");
      }
    }

    public IDisposable LinkTokenSource(CancellationTokenSource toLink)
    {
      if (!this.IsRootContext)
        return this.m_rootContext.LinkTokenSource(toLink);
      this.CheckDisposed();
      lock (this.m_cancellationTokenSourceLock)
        return (IDisposable) new VssRequestContext.VssLinkedCancellationTokenSource(this, CancellationTokenSource.CreateLinkedTokenSource(this.m_cancellationTokenSource.Token, toLink.Token));
    }

    public long ContextId => this.m_contextId;

    public Guid UniqueIdentifier => this.m_rootContext.m_uniqueIdentifier;

    public Guid E2EId
    {
      get => this.m_rootContext.m_e2eId;
      protected set
      {
        if (!this.IsRootContext)
          throw new InvalidOperationException("E2EId may be set on the root context only");
        if (!(value != Guid.Empty))
          return;
        this.m_e2eId = value;
      }
    }

    public string OrchestrationId
    {
      get => this.m_rootContext.m_orchestrationId;
      protected set
      {
        if (!this.IsRootContext)
          throw new InvalidOperationException("OrchestrationId may be set on the root context only");
        if (this.IsFeatureEnabled("VisualStudio.Services.HostManagement.DispatchersUseOrchestrationIds"))
        {
          this.m_orchestrationId = string.IsNullOrEmpty(value) ? (string) null : value;
        }
        else
        {
          if (string.IsNullOrEmpty(value))
            return;
          this.m_orchestrationId = value;
        }
      }
    }

    public string UserAgent => this.m_rootContext.m_userAgent;

    public string DomainUserName
    {
      get => this.m_rootContext.m_domainUserName;
      private set => this.m_rootContext.m_domainUserName = value;
    }

    public TeamFoundationExecutionEnvironment ExecutionEnvironment
    {
      get
      {
        if (!this.m_executionEnvironment.IsInitialized)
          this.m_executionEnvironment = this.CreateExecutionEnvironment();
        return this.m_executionEnvironment;
      }
    }

    public ISqlConnectionInfo FrameworkConnectionInfo
    {
      get
      {
        if ((this.m_serviceHost.HostType & TeamFoundationHostType.Deployment) != TeamFoundationHostType.Unknown)
          return ((IRequestContextInternal) this).DeploymentFrameworkConnectionInfo;
        IVssRequestContext vssRequestContext = this.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<TeamFoundationDatabaseManagementService>().GetSqlConnectionInfo(vssRequestContext, this.m_serviceHost.ServiceHostInternal().DatabaseId);
      }
    }

    public virtual bool IsCanceled => this.m_canceled > 0;

    public bool IsServicingContext => this.m_requestContextType == RequestContextType.ServicingContext;

    public bool IsSystemContext => this.m_requestContextType == RequestContextType.SystemContext || this.m_requestContextType == RequestContextType.ServicingContext;

    public bool IsUserContext => this.m_requestContextType == RequestContextType.UserContext;

    public bool IsTracked
    {
      get => this.m_isTracked;
      set => this.m_isTracked = value;
    }

    public long CPUCycles { get; private set; }

    public long AllocatedBytes { get; private set; }

    public long CPUCyclesAsync { get; private set; }

    public long AllocatedBytesAsync { get; private set; }

    public double TSTUs { get; set; }

    public Guid DataspaceIdentifier => this.m_scopedToDataspace;

    public IDictionary<string, object> Items
    {
      get
      {
        if (this.m_items == null)
        {
          lock (this)
          {
            if (this.m_items == null)
              this.m_items = (IDictionary<string, object>) new ConcurrentDictionary<string, object>(2, 31);
          }
        }
        return this.m_items;
      }
    }

    public MethodInformation Method => this.m_rootContext.m_methodInfo;

    public TimeSpan RequestTimeout
    {
      get => this.m_rootContext.m_requestTimeout;
      set => this.m_rootContext.m_requestTimeout = value;
    }

    public int ResponseCode { get; private set; }

    public IVssRequestContext RootContext => (IVssRequestContext) this.m_rootContext;

    public IVssServiceHost ServiceHost => this.m_serviceHost;

    public string ServiceName
    {
      get => this.m_rootContext.m_serviceName;
      set => this.m_rootContext.m_serviceName = value;
    }

    public Exception Status
    {
      get => this.m_rootContext.m_status;
      set => this.m_rootContext.m_status = value;
    }

    public IdentityDescriptor UserContext
    {
      get
      {
        if (this.m_rootContext.m_userContext == (IdentityDescriptor) null)
          this.m_rootContext.CacheUserContext();
        return this.m_rootContext.m_userContext;
      }
    }

    public ISqlComponentCreator SqlComponentCreator => (ISqlComponentCreator) this.ServiceProvider.GetService<TeamFoundationResourceManagementService>((IVssRequestContext) this);

    public IVssHttpClientProvider ClientProvider
    {
      get
      {
        this.ValidateRequest();
        this.CheckDisposed();
        this.CheckCanceled(false);
        return this.m_rootContext.m_clientProvider;
      }
    }

    public virtual IVssFrameworkServiceProvider ServiceProvider
    {
      get
      {
        this.ValidateRequest();
        this.CheckDisposed();
        this.CheckCanceled(false);
        return this.m_serviceHost.ServiceHostInternal().ServiceProvider;
      }
    }

    public IVssLockManager LockManager
    {
      get
      {
        if (this.m_rootContext.m_lockManager == null)
          this.m_rootContext.m_lockManager = (IVssLockManager) new VssRequestContext.RequestLockManager(this, this.m_serviceHost.ServiceHostInternal());
        return this.m_rootContext.m_lockManager;
      }
    }

    public ILogRequest RequestLogger
    {
      get
      {
        if (this.m_requestLogger == null)
          this.m_requestLogger = new VssRequestContext.LogRequest(this);
        return (ILogRequest) this.m_requestLogger;
      }
    }

    public ITraceRequest RequestTracer
    {
      get
      {
        if (this.m_requestTracer == null)
          this.m_requestTracer = new VssRequestContext.TraceRequest(this, this.m_serviceHostIdForTracing);
        return (ITraceRequest) this.m_requestTracer;
      }
    }

    public ITimeRequest RequestTimer => (ITimeRequest) this.m_rootContext.m_timer;

    public void AddDisposableResource(IDisposable resource)
    {
      if (this.IsRootContext)
      {
        int maxSqlComponents = this.m_serviceHost?.DeploymentServiceHost is IDeploymentServiceHostInternal deploymentServiceHost ? deploymentServiceHost.MaxSqlComponents : 0;
        this.CheckDisposed();
        lock (this.m_disposableResourcesLock)
        {
          if (!this.m_disposableResources.Add(resource) || !(resource is ISqlResourceComponent) || this.IsExempt(resource as ISqlResourceComponent))
            return;
          ++this.m_disposableSqlComponents;
          if (maxSqlComponents == 0 || this.m_disposableSqlComponents <= Math.Abs(maxSqlComponents))
            return;
          string str = string.Join(",", this.m_disposableResources.OfType<ISqlResourceComponent>().Select<ISqlResourceComponent, string>((Func<ISqlResourceComponent, string>) (x => x.GetType().Name)));
          string message = string.Format("Exceeding maximum amount of {0} concurrent SQL components [{1}]", (object) maxSqlComponents, (object) str);
          TeamFoundationTracingService.TraceRawAlwaysOn(36101, TraceLevel.Error, nameof (VssRequestContext), "HostManagement", message);
          if (maxSqlComponents > 0)
            throw new InvalidOperationException(message);
        }
      }
      else
        this.m_rootContext.AddDisposableResource(resource);
    }

    IdentityValidationStatus IRequestContextInternal.IdentityValidationStatus
    {
      get => !this.IsRootContext ? this.m_rootContext.RequestContextInternal().IdentityValidationStatus : this.m_validationStatus;
      set
      {
        if (!this.IsRootContext)
          this.m_rootContext.RequestContextInternal().IdentityValidationStatus = value;
        else
          this.m_validationStatus = value;
      }
    }

    public virtual RequestDetails GetRequestDetails(
      TeamFoundationLoggingLevel loggingLevel = TeamFoundationLoggingLevel.Normal,
      long executionTimeThreshold = 10000000,
      bool isExceptionExpected = false,
      bool canAggregate = true)
    {
      return this.GetBasicRequestDetails(loggingLevel, executionTimeThreshold, isExceptionExpected, canAggregate);
    }

    public virtual void Cancel(string reason) => this.Cancel(reason, HttpStatusCode.InternalServerError);

    public virtual void Cancel(string reason, HttpStatusCode httpStatusCode)
    {
      if (this.m_canceled != 0)
        return;
      if (!Monitor.TryEnter((object) this, 0))
        return;
      try
      {
        if (this.m_canceled == 0 && this.RequestTimer.EndTime == DateTime.MinValue)
        {
          this.m_canceled = 1;
          this.Items[RequestContextItemsKeys.CancellationReason] = (object) reason;
          this.m_cancellationReason = !string.IsNullOrEmpty(reason) ? FrameworkResources.RequestCanceledErrorWithReason((object) reason) : FrameworkResources.RequestCanceledError();
          if (this.IsRootContext && this.Status == null)
          {
            this.Status = (Exception) new RequestCanceledException(this.m_cancellationReason, httpStatusCode);
            this.ResponseCode = (int) httpStatusCode;
          }
          if (this.m_cancelableObjects != null)
          {
            ICancelable[] cancelableArray = (ICancelable[]) null;
            lock (this.m_cancelableObjects)
              cancelableArray = this.m_cancelableObjects.ToArray();
            for (int index = cancelableArray.Length - 1; index >= 0; --index)
            {
              try
              {
                cancelableArray[index].Cancel();
              }
              catch (Exception ex)
              {
                TeamFoundationTracingService.TraceExceptionRaw(36135, nameof (VssRequestContext), "HostManagement", ex);
              }
            }
          }
          lock (this.m_cancellationTokenSourceLock)
          {
            if (this.m_cancellationTokenSource != null)
            {
              try
              {
                this.m_cancellationTokenSource.Cancel();
              }
              catch (Exception ex)
              {
                if (ex is AggregateException)
                {
                  foreach (Exception innerException in (ex as AggregateException).Flatten().InnerExceptions)
                    TeamFoundationTracingService.TraceExceptionRaw(36130, nameof (VssRequestContext), "HostManagement", innerException);
                }
                else
                  TeamFoundationTracingService.TraceExceptionRaw(36131, nameof (VssRequestContext), "HostManagement", ex);
              }
            }
          }
          if (this.m_systemRequestContexts != null || this.m_userRequestContexts != null || this.m_servicingRequestContexts != null || this.m_associatedRequestContexts != null)
          {
            if (Monitor.TryEnter(this.m_contextLock, 1000))
            {
              try
              {
                VssRequestContext.CancelAllRequests(this.m_servicingRequestContexts, reason, httpStatusCode, 36112);
                VssRequestContext.CancelAllRequests(this.m_systemRequestContexts, reason, httpStatusCode, 36113);
                VssRequestContext.CancelAllRequests(this.m_userRequestContexts, reason, httpStatusCode, 36114);
                VssRequestContext.CancelAllRequests((IEnumerable<IVssRequestContext>) this.m_associatedRequestContexts, reason, httpStatusCode, 36116);
              }
              finally
              {
                Monitor.Exit(this.m_contextLock);
              }
            }
            else
              TeamFoundationTracingService.TraceRaw(36115, TraceLevel.Error, nameof (VssRequestContext), "HostManagement", "Timeout getting a lock to cancel child request contexts of context Id {0}.", (object) this.ContextId);
          }
        }
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CancelledServerRequestsTotal").Increment();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CancelledRequestsPerSec").Increment();
      }
      finally
      {
        Monitor.Exit((object) this);
      }
    }

    public void EnterCancelableRegion(ICancelable cancelableObject)
    {
      if (this.m_cancelableObjects == null)
        this.m_cancelableObjects = new List<ICancelable>();
      lock (this.m_cancelableObjects)
      {
        if (this.IsCanceled)
          throw this.CreateRequestCanceledException();
        this.m_cancelableObjects.Add(cancelableObject);
      }
    }

    public void ExitCancelableRegion(ICancelable cancelableObject)
    {
      if (this.m_cancelableObjects == null)
        return;
      lock (this.m_cancelableObjects)
        this.m_cancelableObjects.Remove(cancelableObject);
    }

    public void LinkCancellation(IVssRequestContext childContext)
    {
      lock (this.m_contextLock)
      {
        if (this.m_associatedRequestContexts == null)
          this.m_associatedRequestContexts = (IList<IVssRequestContext>) new List<IVssRequestContext>();
        if (this.m_associatedRequestContexts.Contains(childContext))
          return;
        this.m_associatedRequestContexts.Add(childContext);
      }
    }

    public virtual void EnterMethod(MethodInformation methodInformation)
    {
      if (!this.IsRootContext)
        return;
      if (this.m_methodInfo != null && this.m_methodInfo != methodInformation)
        throw new InvalidOperationException(FrameworkResources.CantEnterMethodTwice((object) this.m_methodInfo.Name, (object) methodInformation.Name));
      if (methodInformation.Timeout != new TimeSpan())
        this.RequestTimeout = methodInformation.Timeout;
      this.m_methodInfo = methodInformation;
      this.m_methodInfo.InProgress = true;
      this.m_methodExecutionThread = Thread.CurrentThread;
      if (methodInformation.CaptureAsyncResourcesUsage)
        this.m_resourcesCounter = this.m_resourcesCounter ?? new AsyncResourcesCounter();
      if (this.m_cpuCycles == 0L)
        this.m_cpuCycles = PerformanceNativeMethods.GetCPUTime();
      if (this.m_allocatedBytes == 0L)
        this.m_allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
      this.RequestTracer.TraceEnter(36107, nameof (VssRequestContext), "HostManagement", this.m_methodInfo.Name);
      VssPerformanceEventSource.Log.MethodStart(this.UniqueIdentifier, this.ServiceHost.InstanceId, methodInformation.Name);
      this.CheckCanceled(true);
      IServiceHostInternal serviceHostInternal = this.m_serviceHost.ServiceHostInternal();
      if (serviceHostInternal.RequestFilters == null)
        return;
      foreach (ITeamFoundationRequestFilter requestFilter in (IEnumerable<ITeamFoundationRequestFilter>) serviceHostInternal.RequestFilters)
      {
        try
        {
          requestFilter.EnterMethod((IVssRequestContext) this);
        }
        catch (RequestFilterException ex)
        {
          this.Cancel(ex.Message, ex.HttpStatusCode);
          throw;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(36175, nameof (VssRequestContext), "HostManagement", ex);
        }
      }
    }

    public virtual void LeaveMethod()
    {
      if (!this.IsRootContext || this.m_isDisposed)
        return;
      this.ResetCancel();
      if (this.m_methodExecutionThread == Thread.CurrentThread)
      {
        this.CPUCycles = PerformanceNativeMethods.GetCPUTime() - this.m_cpuCycles;
        this.AllocatedBytes = GC.GetAllocatedBytesForCurrentThread() - this.m_allocatedBytes;
      }
      if (this.m_resourcesCounter != null)
        (this.CPUCyclesAsync, this.AllocatedBytesAsync) = this.m_resourcesCounter.CaptureCollectedCounters();
      if (this.m_methodInfo != null && this.m_methodInfo.InProgress)
      {
        this.RequestTracer.TraceLeave(36107, nameof (VssRequestContext), "HostManagement", this.m_methodInfo.Name);
        IServiceHostInternal serviceHostInternal = this.m_serviceHost.ServiceHostInternal();
        if (serviceHostInternal.RequestFilters != null)
        {
          foreach (ITeamFoundationRequestFilter requestFilter in (IEnumerable<ITeamFoundationRequestFilter>) serviceHostInternal.RequestFilters)
          {
            try
            {
              requestFilter.LeaveMethod((IVssRequestContext) this);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(36185, nameof (VssRequestContext), "HostManagement", ex);
            }
          }
        }
        VssPerformanceEventSource.Log.MethodStop(this.UniqueIdentifier, this.ServiceHost.InstanceId, this.m_methodInfo.Name, (long) this.m_timer.ExecutionSpan.TotalMilliseconds);
        this.m_methodInfo.InProgress = false;
      }
      this.m_methodExecutionThread = (Thread) null;
    }

    public IVssRequestContext Elevate(bool throwIfShutdown = true)
    {
      this.ValidateRequest();
      IVssRequestContext vssRequestContext;
      if (!this.IsSystemContext)
      {
        lock (this.m_contextLock)
          vssRequestContext = this.GetSystemRequestContextForHost(this.m_serviceHost.InstanceId);
      }
      else
        vssRequestContext = (IVssRequestContext) this;
      return vssRequestContext;
    }

    public IVssRequestContext To(TeamFoundationHostType hostType)
    {
      if (this.ServiceHost.Is(hostType))
        return (IVssRequestContext) this;
      IVssServiceHost targetHost;
      switch (hostType)
      {
        case TeamFoundationHostType.Parent:
          targetHost = this.ServiceHost.ParentServiceHost;
          break;
        case TeamFoundationHostType.Deployment:
          targetHost = (IVssServiceHost) this.ServiceHost.DeploymentServiceHost;
          break;
        case TeamFoundationHostType.Application:
          if (this.ServiceHost.Is(TeamFoundationHostType.Application))
          {
            targetHost = this.ServiceHost;
            break;
          }
          targetHost = this.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? this.ServiceHost.ParentServiceHost : throw new InvalidOperationException("Can't ask for an Application Host from the Deployment Host");
          break;
        default:
          throw new InvalidOperationException(string.Format("Invalid Host Target {0}", (object) hostType));
      }
      return this.To(targetHost);
    }

    public virtual string AuthenticatedUserName => this.m_rootContext.m_authenticatedUserName;

    public bool IsImpersonating => this.m_rootContext.m_actors != null && this.m_rootContext.m_actors.Count > 1;

    [Obsolete("Use CreateUserContext instead.", true)]
    public virtual IVssRequestContext CreateImpersonationContext(
      IdentityDescriptor identity,
      RequestContextType? newType = null)
    {
      return this.CreateImpersonationContext(this.m_serviceHost, identity, newType);
    }

    bool IRequestContextInternal.IsRootContext => this.IsRootContext;

    private bool IsRootContext => this.m_rootContext == this;

    public VssRequestContextPriority RequestPriority => this.IsRootContext ? this.m_priority : this.m_rootContext.RequestPriority;

    VssRequestContextPriority IRequestContextInternal.RequestPriority
    {
      set
      {
        if (this.IsRootContext)
          this.m_priority = value;
        else
          ((IRequestContextInternal) this.m_rootContext).RequestPriority = value;
      }
    }

    ISqlConnectionInfo IRequestContextInternal.DeploymentFrameworkConnectionInfo => this.ServiceHost != null && this.ServiceHost.DeploymentServiceHost != null && this.ServiceHost.DeploymentServiceHost.DatabaseProperties != null ? this.ServiceHost.DeploymentServiceHost.DatabaseProperties.SqlConnectionInfo : (ISqlConnectionInfo) null;

    bool IRequestContextInternal.HasRequestTimedOut => !this.IsCanceled && (this.Method == null || !this.Method.IsLongRunning) && this.m_rootContext.m_timer.ExecutionSpan > this.RequestTimeout;

    Thread IRequestContextInternal.MethodExecutionThread => this.m_rootContext.m_methodExecutionThread;

    IReadOnlyList<IRequestActor> IRequestContextInternal.Actors
    {
      get => this.Actors;
      set => this.Actors = value;
    }

    void IRequestContextInternal.ClearActors() => this.m_actors = (IReadOnlyList<IRequestActor>) null;

    private IReadOnlyList<IRequestActor> Actors
    {
      get => this.m_rootContext.m_actors;
      set => this.m_rootContext.m_actors = this.m_rootContext.m_actors == null ? value : throw new InvalidOperationException("Cannot reset the Actors for the Request.");
    }

    void IRequestContextInternal.CheckCanceled() => this.CheckCanceled(true);

    void IRequestContextInternal.ResetCancel() => this.ResetCancel();

    private void ResetCancel()
    {
      lock (this)
      {
        this.m_canceled = -1;
        if (!this.IsRootContext)
          return;
        if (this.m_systemRequestContexts != null || this.m_userRequestContexts != null)
        {
          lock (this.m_contextLock)
          {
            if (this.m_systemRequestContexts != null)
            {
              foreach (VssRequestContext vssRequestContext in this.m_systemRequestContexts.Values)
                vssRequestContext.ResetCancel();
            }
            if (this.m_userRequestContexts != null)
            {
              foreach (VssRequestContext vssRequestContext in this.m_userRequestContexts.Values)
                vssRequestContext.ResetCancel();
            }
          }
        }
        lock (this.m_cancellationTokenSourceLock)
          this.m_cancellationTokenSource = new CancellationTokenSource();
      }
    }

    void IRequestContextInternal.RemoveDisposableResource(IDisposable resource)
    {
      if (this.IsRootContext)
      {
        lock (this.m_disposableResourcesLock)
        {
          if (!this.m_disposableResources.Remove(resource) || !(resource is ISqlResourceComponent) || this.IsExempt(resource as ISqlResourceComponent))
            return;
          --this.m_disposableSqlComponents;
        }
      }
      else
        this.m_rootContext.RequestContextInternal().RemoveDisposableResource(resource);
    }

    void IRequestContextInternal.DisposeDisposableResources()
    {
      lock (this.m_disposableResourcesLock)
      {
        if (this.m_disposableResources.Count <= 0)
          return;
        IDisposable[] array = new IDisposable[this.m_disposableResources.Count];
        this.m_disposableResources.CopyTo(array);
        for (int index = 0; index < array.Length; ++index)
        {
          IDisposable disposable = array[index];
          if (!(disposable is TeamFoundationSqlResourceComponent))
            disposable.Dispose();
        }
        for (int index = 0; index < array.Length; ++index)
        {
          if (array[index] is TeamFoundationSqlResourceComponent resourceComponent && this.m_disposableResources.Contains((IDisposable) resourceComponent))
          {
            TeamFoundationTracingService.TraceRaw(36166, TraceLevel.Error, nameof (VssRequestContext), "HostManagement", "{0} has not been disposed explicitly. Last statement executed {1}. Please file a bug on the component owner", (object) resourceComponent.GetType().Name, (object) resourceComponent.LastExecutedCommandText);
            resourceComponent.Dispose();
          }
        }
        this.m_disposableResources.Clear();
      }
    }

    T[] IRequestContextInternal.GetDisposableResources<T>()
    {
      if (!this.IsRootContext)
        return this.m_rootContext.RequestContextInternal().GetDisposableResources<T>();
      this.CheckDisposed();
      lock (this.m_disposableResourcesLock)
        return this.m_disposableResources.OfType<T>().ToArray<T>();
    }

    void IRequestContextInternal.AddLease(ILeaseInfo item)
    {
      if (this.IsRootContext)
      {
        if (this.m_leases == null)
          this.m_leases = new Dictionary<string, ILeaseInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.m_leases[item.LeaseName] = item;
      }
      else
        this.m_rootContext.RequestContextInternal().AddLease(item);
    }

    void IRequestContextInternal.RemoveLease(string item)
    {
      if (this.IsRootContext)
      {
        if (this.m_leases == null)
          return;
        this.m_leases.Remove(item);
      }
      else
        this.m_rootContext.RequestContextInternal().RemoveLease(item);
    }

    void IRequestContextInternal.SetDataspaceIdentifier(Guid dataspaceIdentifier) => this.m_scopedToDataspace = !(this.m_scopedToDataspace != Guid.Empty) ? dataspaceIdentifier : throw new InvalidOperationException(string.Format("{0} is already set to {1}", (object) "m_scopedToDataspace", (object) this.m_scopedToDataspace));

    void IRequestContextInternal.ResetActivityId() => this.ResetActivityId();

    protected void ResetActivityId()
    {
      if (!(this.m_activityId == Guid.Empty) && !this.ShouldResetActivityId(this.m_activityId))
        return;
      this.m_activityId = this.UnsafeCreateNewActivityId();
      this.m_e2eId = this.m_activityId;
      Trace.CorrelationManager.ActivityId = this.m_activityId;
    }

    void IRequestContextInternal.SetE2EId(Guid identifier) => this.E2EId = identifier;

    void IRequestContextInternal.SetOrchestrationId(string identifier) => this.OrchestrationId = identifier;

    void IRequestContextInternal.SetAuthenticatedUserName(string authenticatedUserName) => this.m_authenticatedUserName = authenticatedUserName;

    void IRequestContextInternal.SetResponseCode(int responseCode) => this.ResponseCode = responseCode;

    void IRequestContextInternal.SetDomainUserName(string domainUserName) => this.DomainUserName = domainUserName;

    protected virtual void EndRequest()
    {
      try
      {
        TeamFoundationTracingService.TraceEnterRaw(36140, nameof (VssRequestContext), "HostManagement", nameof (EndRequest), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
        this.ResetCancel();
        if (this.RequestTimer.EndTime == DateTime.MinValue)
        {
          VssRequestContext.TimeRequest timer = this.m_rootContext.m_timer;
          timer.EndRequest();
          if (this.IsRootContext)
          {
            if (this.IsTracked)
            {
              long executionThresholdMilliseconds = this.m_serviceHost.ServiceHostInternal().TotalExecutionElapsedThreshold * 1000L;
              DiagnosticDumper diagnosticDumper = timer.GetDiagnosticDumper(executionThresholdMilliseconds);
              if (this.Method == null || !this.Method.IsLongRunning)
              {
                long totalMilliseconds = (long) timer.ExecutionSpan.TotalMilliseconds;
                if (totalMilliseconds > executionThresholdMilliseconds)
                  this.TraceAlwaysInChunks(36109, TraceLevel.Error, nameof (VssRequestContext), "HostManagement", diagnosticDumper.ToStringBuilder());
                else if (totalMilliseconds > 5000L)
                  this.RequestTracer.Trace(36117, TraceLevel.Warning, nameof (VssRequestContext), "HostManagement", (string[]) null, "{0}", (object) diagnosticDumper);
                else if (totalMilliseconds > 3000L)
                  this.RequestTracer.Trace(36118, TraceLevel.Info, nameof (VssRequestContext), "HostManagement", (string[]) null, "{0}", (object) diagnosticDumper);
              }
              this.RequestTracer.Trace(36113, TraceLevel.Verbose, nameof (VssRequestContext), "HostManagement", (string[]) null, "{0}", (object) diagnosticDumper);
            }
            bool flag1 = false;
            if (this.Items.TryGetValue<bool>(RequestContextItemsKeys.IncludePerformanceTimingsInResponse, out flag1) & flag1 && this is IWebRequestContextInternal requestContextInternal1 && requestContextInternal1.HttpContext != null && !requestContextInternal1.HttpContext.Response.HeadersWritten)
            {
              string str = JsonConvert.SerializeObject((object) PerformanceTimer.GetAllTimings((IVssRequestContext) this));
              requestContextInternal1.HttpContext.Response.AddHeader("X-VSS-PerfData", str);
            }
            bool flag2;
            if (this.Items.TryGetValue<bool>(RequestContextItemsKeys.IncludePerformanceTimingsInServerTimingHeader, out flag2) & flag2 && this is IWebRequestContextInternal requestContextInternal2 && requestContextInternal2.HttpContext != null && !requestContextInternal2.HttpContext.Response.HeadersWritten)
            {
              IDictionary<string, PerformanceTimingGroup> allTimings = PerformanceTimer.GetAllTimings((IVssRequestContext) this);
              int num = 0;
              StringBuilder stringBuilder1 = new StringBuilder();
              foreach (KeyValuePair<string, PerformanceTimingGroup> keyValuePair in (IEnumerable<KeyValuePair<string, PerformanceTimingGroup>>) allTimings)
              {
                stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0:D4};dur={1};desc=\"{2}\",", (object) num++, (object) ((double) keyValuePair.Value.ElapsedTicks / 10000.0), (object) keyValuePair.Key);
                foreach (PerformanceTimingEntry performanceTimingEntry in keyValuePair.Value.Timings.Where<PerformanceTimingEntry>((Func<PerformanceTimingEntry, bool>) (x => x.Properties != null)))
                {
                  StringBuilder stringBuilder2 = new StringBuilder();
                  stringBuilder2.Append(">");
                  foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) performanceTimingEntry.Properties)
                    stringBuilder2.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1} ", (object) property.Key, property.Value);
                  stringBuilder2.Replace("\"", "\\\"");
                  stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0:D4};dur={1};desc=\"{2}\",", (object) num++, (object) ((double) performanceTimingEntry.ElapsedTicks / 10000.0), (object) stringBuilder2.ToString());
                }
              }
              requestContextInternal2.HttpContext.Response.AddHeader("Server-Timing", stringBuilder1.ToString());
            }
          }
          if (!this.m_serviceHost.IsDisposed)
          {
            if (this.m_requestContextType == RequestContextType.UserContext && this.UserContext != (IdentityDescriptor) null && this.m_methodInfo != null && this.m_methodInfo.KeepsHostAwake && this.m_serviceHost.HasDatabaseAccess)
            {
              TeamFoundationHostManagementService service = this.ServiceProvider.GetService<TeamFoundationHostManagementService>((IVssRequestContext) this);
              bool? nullable = new bool?();
              IIdentityServiceInternal identityServiceInternal = this.ServiceProvider.GetService<IdentityService>((IVssRequestContext) this).IdentityServiceInternal();
              Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
              TeamFoundationExecutionEnvironment executionEnvironment;
              if (service.LastUserAccessUpdateScheduled((IVssRequestContext) this, this.m_serviceHost.InstanceId))
              {
                TeamFoundationTracingService.TraceRaw(36147, TraceLevel.Verbose, nameof (VssRequestContext), "HostManagement", "Host already marked as user accessed. Host: {0}; Url: {1}", (object) this.m_serviceHost, (object) this.RequestUriForTracing());
              }
              else
              {
                bool flag3;
                if (this.RootContext.Items.TryGetValue<bool>(RequestContextItemsKeys.SkipHostLastUserAccessUpdate, out flag3) & flag3)
                {
                  TeamFoundationTracingService.TraceRaw(36146, TraceLevel.Info, nameof (VssRequestContext), "HostManagement", "Request skipped user access update. Host: {0}; Url: {1}: User: {2}", (object) this.m_serviceHost, (object) this.RequestUriForTracing(), (object) this.UserContext);
                }
                else
                {
                  try
                  {
                    identity = this.GetUserIdentity();
                  }
                  catch (VssException ex)
                  {
                    TeamFoundationTracingService.TraceCatchRaw(36142, TraceLevel.Info, nameof (VssRequestContext), "HostManagement", (Exception) ex);
                  }
                  bool flag4;
                  this.RootContext.Items.TryGetValue<bool>(RequestContextItemsKeys.ForceHostLastUserAccessUpdate, out flag4);
                  bool flag5 = identity != null && !identity.Id.Equals(AnonymousAccessConstants.AnonymousSubjectId);
                  nullable = new bool?(flag5 && IdentityHelper.IsServiceIdentity((IVssRequestContext) this, (IReadOnlyVssIdentity) identity));
                  if (!nullable.Value)
                  {
                    executionEnvironment = this.ExecutionEnvironment;
                    if (executionEnvironment.IsOnPremisesDeployment)
                      nullable = new bool?(identityServiceInternal.IsMember((IVssRequestContext) this, IdentityHelper.CreateTeamFoundationDescriptor(BuildGroupWellKnownSecurityIds.BuildServicesGroup), this.UserContext));
                  }
                  if (this.m_serviceHost.IsHostProcessType(HostProcessType.ApplicationTier) && service.ForceLastUserAccessUpdate | flag4 || flag5 && !nullable.Value)
                  {
                    TeamFoundationTracingService.TraceRaw(36143, TraceLevel.Info, nameof (VssRequestContext), "HostManagement", "Marking host as user accessed. Host: {0}; Url: {1}; User: {2}", (object) this.m_serviceHost, (object) this.RequestUriForTracing(), (object) this.UserContext);
                    service.ScheduleLastUserAccessUpdate((IVssRequestContext) this, this.m_serviceHost.InstanceId);
                  }
                  else
                    TeamFoundationTracingService.TraceRaw(36145, TraceLevel.Info, nameof (VssRequestContext), "HostManagement", "Request from service account or out of scope identity not considered a user access. Host: {0}; Url: {1}; User: {2}", (object) this.m_serviceHost, (object) this.RequestUriForTracing(), (object) this.UserContext);
                }
              }
              executionEnvironment = this.ExecutionEnvironment;
              if (executionEnvironment.IsOnPremisesDeployment)
              {
                if (identity == null)
                  identity = this.GetUserIdentity();
                if (identity.MasterId != Guid.Empty && identity.MasterId != IdentityConstants.LinkedId)
                {
                  if (!identityServiceInternal.LastUserAccessUpdateScheduled((IVssRequestContext) this, identity.MasterId))
                  {
                    if (!nullable.HasValue)
                      nullable = new bool?(identityServiceInternal.IsMember((IVssRequestContext) this, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, this.UserContext));
                    if (!nullable.Value)
                    {
                      TeamFoundationTracingService.TraceRaw(36149, TraceLevel.Info, nameof (VssRequestContext), "HostManagement", "Marking user as recently accessed. User: {0}", (object) identity);
                      identityServiceInternal.ScheduleLastUserAccessUpdate((IVssRequestContext) this, identity.MasterId);
                    }
                    else
                      TeamFoundationTracingService.TraceRaw(36151, TraceLevel.Info, nameof (VssRequestContext), "HostManagement", "Request from service account not considered a user access. User: {0}", (object) identity);
                  }
                  else
                    TeamFoundationTracingService.TraceRaw(36153, TraceLevel.Verbose, nameof (VssRequestContext), "HostManagement", "User already marked as recently accessed. User: {0}", (object) identity);
                }
                else
                  TeamFoundationTracingService.TraceRaw(36155, TraceLevel.Verbose, nameof (VssRequestContext), "HostManagement", "User has no TeamFoundationId. User: {0}", (object) identity);
              }
              executionEnvironment = this.ExecutionEnvironment;
              if (!executionEnvironment.IsHostedDeployment)
                return;
              this.To(TeamFoundationHostType.Deployment).GetService<IUserAccessLoggingService>().LogUserAccess((IVssRequestContext) this);
            }
            else
              TeamFoundationTracingService.TraceRaw(36157, TraceLevel.Verbose, nameof (VssRequestContext), "HostManagement", "Request not considered a user access. Host: {0};  Url: {1}; RCType: {2}; User: {3}; MI: {4}; DBAccess: {5}", (object) this.m_serviceHost, (object) this.RequestUriForTracing(), (object) this.m_requestContextType, (object) this.UserContext, (object) this.m_methodInfo, (object) this.m_serviceHost.HasDatabaseAccess);
          }
          else
            TeamFoundationTracingService.TraceRaw(36159, TraceLevel.Error, nameof (VssRequestContext), "HostManagement", "ServiceHost disposed before containing request.");
        }
        else
          TeamFoundationTracingService.TraceRaw(36163, TraceLevel.Verbose, nameof (VssRequestContext), "HostManagement", "Ignoring duplicate call to EndRequest.");
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(36165, nameof (VssRequestContext), "HostManagement", ex);
        throw;
      }
      finally
      {
        ((IRequestContextInternal) this).DisposeDisposableResources();
        TeamFoundationTracingService.TraceLeaveRaw(36167, nameof (VssRequestContext), "HostManagement", nameof (EndRequest));
      }
    }

    internal static void CancelAllRequests(
      Dictionary<Guid, VssRequestContext> requests,
      string reason,
      HttpStatusCode httpStatusCode,
      int tracepoint)
    {
      if (requests == null || requests.Count <= 0)
        return;
      VssRequestContext.CancelAllRequests((IEnumerable<IVssRequestContext>) requests.Values, reason, httpStatusCode, tracepoint);
    }

    internal static void CancelAllRequests(
      IEnumerable<IVssRequestContext> requests,
      string reason,
      HttpStatusCode httpStatusCode,
      int tracepoint)
    {
      if (requests == null)
        return;
      foreach (VssRequestContext request in requests)
      {
        try
        {
          request.Cancel(reason, httpStatusCode);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(tracepoint, nameof (VssRequestContext), "HostManagement", ex);
        }
      }
    }

    internal long RequestId => this.m_lockHelper != null ? this.m_lockHelper.RequestId : 0L;

    private bool IsExempt(ISqlResourceComponent sqlComponent) => sqlComponent is LockingComponent || sqlComponent is DataspaceComponent;

    private RequestCanceledException CreateRequestCanceledException() => this.Status is TeamFoundationServiceException status ? new RequestCanceledException(this.m_cancellationReason, status.HttpStatusCode) : new RequestCanceledException(this.m_cancellationReason);

    private IVssRequestContext To(IVssServiceHost targetHost)
    {
      TeamFoundationHostManagementService hostManagement = this.m_serviceHost.DeploymentServiceHost.DeploymentServiceHostInternal().HostManagement;
      if (targetHost == null)
        return (IVssRequestContext) null;
      this.ValidateRequest();
      if (targetHost == this.ServiceHost)
        return (IVssRequestContext) this;
      lock (this.m_contextLock)
      {
        if (this.IsUserContext)
          return this.GetUserRequestContextForHost(targetHost.InstanceId);
        return this.IsServicingContext ? this.GetServicingContextForHost(targetHost.InstanceId) : this.GetSystemRequestContextForHost(targetHost.InstanceId);
      }
    }

    private IVssRequestContext GetSystemRequestContextForHost(Guid serviceHostInstanceId)
    {
      this.ValidateRequest();
      if (this.m_rootContext.m_systemRequestContexts == null)
        this.m_rootContext.m_systemRequestContexts = new Dictionary<Guid, VssRequestContext>();
      VssRequestContext requestContextForHost;
      if (!this.m_rootContext.m_systemRequestContexts.TryGetValue(serviceHostInstanceId, out requestContextForHost))
      {
        using (IVssRequestContext systemContext = this.ServiceHost.DeploymentServiceHost.CreateSystemContext(false))
          requestContextForHost = this.m_serviceHost.DeploymentServiceHost.DeploymentServiceHostInternal().HostManagement.BeginRequest(systemContext, serviceHostInstanceId, RequestContextType.SystemContext, true, false, (IReadOnlyList<IRequestActor>) null, HostRequestType.Default, new object[1]
          {
            (object) this.m_rootContext
          });
        this.m_rootContext.m_systemRequestContexts[serviceHostInstanceId] = requestContextForHost;
      }
      return (IVssRequestContext) requestContextForHost;
    }

    private IVssRequestContext GetServicingContextForHost(Guid serviceHostInstanceId)
    {
      if (this.m_rootContext.m_servicingRequestContexts == null)
        this.m_rootContext.m_servicingRequestContexts = new Dictionary<Guid, VssRequestContext>();
      VssRequestContext servicingContextForHost;
      if (!this.m_rootContext.m_servicingRequestContexts.TryGetValue(serviceHostInstanceId, out servicingContextForHost))
      {
        using (IVssRequestContext systemContext = this.ServiceHost.DeploymentServiceHost.CreateSystemContext(false))
          servicingContextForHost = this.m_serviceHost.DeploymentServiceHost.DeploymentServiceHostInternal().HostManagement.BeginRequest(systemContext, serviceHostInstanceId, RequestContextType.ServicingContext, true, false, (IReadOnlyList<IRequestActor>) null, HostRequestType.Default, new object[1]
          {
            (object) this.m_rootContext
          });
        this.m_rootContext.m_servicingRequestContexts[serviceHostInstanceId] = servicingContextForHost;
      }
      return (IVssRequestContext) servicingContextForHost;
    }

    private IVssRequestContext GetUserRequestContextForHost(Guid serviceHostInstanceId)
    {
      if (this.m_rootContext.m_userRequestContexts == null)
        this.m_rootContext.m_userRequestContexts = new Dictionary<Guid, VssRequestContext>();
      VssRequestContext requestContextForHost;
      if (!this.m_rootContext.m_userRequestContexts.TryGetValue(serviceHostInstanceId, out requestContextForHost))
      {
        using (IVssRequestContext systemContext = this.ServiceHost.DeploymentServiceHost.CreateSystemContext(false))
        {
          requestContextForHost = this.m_serviceHost.DeploymentServiceHost.DeploymentServiceHostInternal().HostManagement.BeginRequest(systemContext, serviceHostInstanceId, RequestContextType.UserContext, true, true, (IReadOnlyList<IRequestActor>) null, HostRequestType.Default, new object[1]
          {
            (object) this.m_rootContext
          });
          this.m_rootContext.m_userRequestContexts[serviceHostInstanceId] = requestContextForHost;
        }
      }
      return (IVssRequestContext) requestContextForHost;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe Guid UnsafeCreateNewActivityId()
    {
      Guid activityIdSource = VssRequestContext.s_activityIdSource;
      Guid* guidPtr = &activityIdSource;
      *(long*) guidPtr = *(long*) guidPtr ^ this.m_contextId;
      return activityIdSource;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe bool ShouldResetActivityId(Guid activityId) => *(long*) ((IntPtr) &VssRequestContext.s_activityIdSource + 8) == *(long*) ((IntPtr) &activityId + 8);

    private string GetSummary()
    {
      string str1 = string.Empty;
      string str2 = string.Empty;
      if (this.Method != null)
      {
        str1 = this.Method.Name;
        if (this.Method.Parameters != null && this.Method.Parameters.Count > 0)
        {
          List<string> stringList = new List<string>();
          foreach (string parameter in (NameObjectCollectionBase) this.Method.Parameters)
            stringList.Add(parameter + " = " + this.Method.Parameters[parameter]);
          str2 = string.Join(Environment.NewLine, stringList.ToArray());
        }
      }
      return FrameworkResources.RequestContextDetails((object) this.RawUrl(), (object) str1, (object) str2, (object) this.ActivityId, (object) this.GetType().FullName, (object) this.AuthenticatedUserName, (object) this.UserAgent, (object) this.UniqueIdentifier);
    }

    private static string HostTypeToString(IVssServiceHost host) => host == null ? "Unknown" : (!host.Is(TeamFoundationHostType.Application) ? (!host.Is(TeamFoundationHostType.Deployment) ? (!host.Is(TeamFoundationHostType.ProjectCollection) ? "Other" : "Collection") : "Deployment") : "Application");

    private TeamFoundationExecutionEnvironment CreateExecutionEnvironment()
    {
      if (!this.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return this.To(TeamFoundationHostType.Deployment).ExecutionEnvironment;
      this.ServiceHost.CheckDisposedOrDisposing();
      ExecutionEnvironmentFlags flags = ExecutionEnvironmentFlags.None;
      switch (this.ServiceProvider.GetService<TeamFoundationHostManagementService>((IVssRequestContext) this).DeploymentType)
      {
        case DeploymentType.Unknown:
          if (!this.ServiceHost.HasDatabaseAccess)
          {
            flags = ExecutionEnvironmentFlags.OnPremisesDeployment | ExecutionEnvironmentFlags.OnPremisesProxy;
            break;
          }
          break;
        case DeploymentType.OnPremises:
          flags = ExecutionEnvironmentFlags.OnPremisesDeployment;
          if (!this.ServiceHost.HasDatabaseAccess)
          {
            flags |= ExecutionEnvironmentFlags.OnPremisesProxy;
            break;
          }
          break;
        case DeploymentType.DevFabric:
          flags = ExecutionEnvironmentFlags.DevFabricDeployment;
          break;
        case DeploymentType.Cloud:
          flags = ExecutionEnvironmentFlags.CloudDeployment;
          break;
      }
      if (TeamFoundationApplicationCore.SslOnly)
        flags |= ExecutionEnvironmentFlags.SslOnly;
      return new TeamFoundationExecutionEnvironment(flags);
    }

    [Obsolete]
    private IVssRequestContext CreateImpersonationContext(
      IVssServiceHost serviceHost,
      IdentityDescriptor identity,
      RequestContextType? type = null)
    {
      ArgumentUtility.CheckForNull<IVssServiceHost>(serviceHost, nameof (serviceHost));
      RequestContextType contextType = this.m_requestContextType;
      if (type.HasValue)
        contextType = type.Value;
      if (contextType == RequestContextType.ServicingContext && this.m_serviceHost != serviceHost)
        contextType = RequestContextType.SystemContext;
      TeamFoundationHostManagementService service = this.ServiceProvider.GetService<TeamFoundationHostManagementService>((IVssRequestContext) this);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = this.GetService<IdentityService>().ReadIdentities((IVssRequestContext) this, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identity
      }, QueryMembership.None, (IEnumerable<string>) null);
      Microsoft.VisualStudio.Services.Identity.Identity userContextIdentity = source != null && source.Count != 0 ? source.Single<Microsoft.VisualStudio.Services.Identity.Identity>() : throw new IdentityNotFoundException(identity);
      if (userContextIdentity == null)
        throw new IdentityNotFoundException(identity);
      List<IRequestActor> actors = new List<IRequestActor>();
      actors.AddRange((IEnumerable<IRequestActor>) this.Actors);
      actors.Add(RequestActor.CreateRequestActor((IVssRequestContext) this, userContextIdentity.Descriptor, userContextIdentity.Id));
      IVssRequestContext impersonationContext = service.BeginImpersonatedRequest((IVssRequestContext) this, serviceHost.InstanceId, contextType, (IReadOnlyList<IRequestActor>) actors);
      this.SetUserIdentityTracingItems(userContextIdentity);
      return impersonationContext;
    }

    private void CacheUserContext()
    {
      if (!this.IsRootContext)
        throw new InvalidOperationException("CacheUserContext is only valid on the root context.");
      if (this.m_actors == null)
        return;
      this.m_userContext = this.m_actors[this.m_actors.Count - 1].Descriptor;
    }

    internal void ClearUserContextCache() => this.m_userContext = (IdentityDescriptor) null;

    private struct VssLinkedCancellationTokenSource : IDisposable
    {
      private readonly CancellationTokenSource m_previous;
      private CancellationTokenSource m_current;
      private readonly VssRequestContext m_requestContext;

      public VssLinkedCancellationTokenSource(
        VssRequestContext requestContext,
        CancellationTokenSource newToken)
      {
        this.m_requestContext = requestContext;
        this.m_previous = this.m_requestContext.m_cancellationTokenSource;
        this.m_requestContext.m_cancellationTokenSource = newToken;
        this.m_current = newToken;
      }

      public void Dispose()
      {
        if (this.m_current == null)
          return;
        lock (this.m_requestContext.m_cancellationTokenSourceLock)
        {
          if (this.m_current != this.m_requestContext.m_cancellationTokenSource)
            this.m_requestContext.Trace(134217727, TraceLevel.Error, VssRequestContext.s_area, VssRequestContext.s_layer, "Cancellation token being disposed is not the current cancellation token on the request context. \r\n                                      This means someone forgot to dispose a cancellation token created by CreateLinkedTokenSource()");
          this.m_requestContext.m_cancellationTokenSource = this.m_previous;
          this.m_current.Dispose();
          this.m_current = (CancellationTokenSource) null;
        }
      }
    }

    internal class RequestLockManager : IVssLockManager, IVssLockManagerInternal
    {
      private IServiceHostInternal m_serviceHost;
      private VssRequestContext m_requestContext;
      private object m_locksHeld;

      internal RequestLockManager(
        VssRequestContext requestContext,
        IServiceHostInternal serviceHost)
      {
        this.m_serviceHost = serviceHost;
        this.m_requestContext = requestContext;
        this.m_locksHeld = this.m_serviceHost.LockManager.CreateLocksHeldObject();
      }

      public NamedLockFrame Lock(ILockName lockName)
      {
        this.m_requestContext.ValidateRequest();
        return this.m_serviceHost.LockManager.Lock(lockName, Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceMonitorLock, this.m_locksHeld, this.m_requestContext.RequestId);
      }

      public bool IsLockHeld(ILockName lockName)
      {
        this.m_requestContext.ValidateRequest();
        return this.m_serviceHost.LockManager.TestLock(lockName, Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceMonitorLock, this.m_locksHeld, this.m_requestContext.RequestId);
      }

      public NamedLockFrame AcquireReaderLock(ILockName lockName)
      {
        this.m_requestContext.ValidateRequest();
        return this.m_serviceHost.LockManager.Lock(lockName, Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceShared, this.m_locksHeld, this.m_requestContext.RequestId);
      }

      public bool IsReaderLockHeld(ILockName lockName)
      {
        this.m_requestContext.ValidateRequest();
        return this.m_serviceHost.LockManager.TestLock(lockName, Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceShared, this.m_locksHeld, this.m_requestContext.RequestId);
      }

      public NamedLockFrame AcquireWriterLock(ILockName lockName)
      {
        this.m_requestContext.ValidateRequest();
        return this.m_serviceHost.LockManager.Lock(lockName, Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceExclusive, this.m_locksHeld, this.m_requestContext.RequestId);
      }

      public bool IsWriterLockHeld(ILockName lockName)
      {
        this.m_requestContext.ValidateRequest();
        return this.m_serviceHost.LockManager.TestLock(lockName, Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceExclusive, this.m_locksHeld, this.m_requestContext.RequestId);
      }

      public NamedLockFrame AcquireConnectionLock(ConnectionLockNameType type)
      {
        if (type != ConnectionLockNameType.REST)
          this.m_requestContext.ValidateRequest();
        return this.m_serviceHost.LockManager.Lock(Microsoft.TeamFoundation.Framework.Server.LockManager.GetConnectionLockName(type), Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceConnection, this.m_locksHeld, this.m_requestContext.RequestId);
      }

      public NamedLockFrame AcquireExemptionLock()
      {
        this.m_requestContext.ValidateRequest();
        return this.m_serviceHost.LockManager.Lock(Microsoft.TeamFoundation.Framework.Server.LockManager.GetExemptionLockName(), Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceExemption, this.m_locksHeld, this.m_requestContext.RequestId);
      }

      public bool TryGetLock(ILockName lockName, int millisecondsTimeout)
      {
        this.m_requestContext.ValidateRequest();
        return this.m_serviceHost.LockManager.TryGetLock(lockName, Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceMonitorLock, this.m_locksHeld, this.m_requestContext.RequestId, millisecondsTimeout);
      }

      public void ReleaseLock(ILockName lockName)
      {
        this.m_requestContext.ValidateRequest();
        this.m_serviceHost.LockManager.ReleaseLock(lockName, Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceMonitorLock, this.m_locksHeld, this.m_requestContext.RequestId);
      }

      bool IVssLockManagerInternal.TryGetReaderLock(ILockName lockName, int millisecondsTimeout)
      {
        this.m_requestContext.ValidateRequest();
        return this.m_serviceHost.LockManager.TryGetLock(lockName, Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceShared, this.m_locksHeld, this.m_requestContext.RequestId, millisecondsTimeout);
      }

      void IVssLockManagerInternal.ReleaseReaderLock(ILockName lockName)
      {
        this.m_requestContext.ValidateRequest();
        this.m_serviceHost.LockManager.ReleaseLock(lockName, Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceShared, this.m_locksHeld, this.m_requestContext.RequestId);
      }

      bool IVssLockManagerInternal.TryGetWriterLock(ILockName lockName, int millisecondsTimeout)
      {
        this.m_requestContext.ValidateRequest();
        return this.m_serviceHost.LockManager.TryGetLock(lockName, Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceExclusive, this.m_locksHeld, this.m_requestContext.RequestId, millisecondsTimeout);
      }

      void IVssLockManagerInternal.ReleaseWriterLock(ILockName lockName)
      {
        this.m_requestContext.ValidateRequest();
        this.m_serviceHost.LockManager.ReleaseLock(lockName, Microsoft.TeamFoundation.Framework.Server.LockManager.LockType.ResourceExclusive, this.m_locksHeld, this.m_requestContext.RequestId);
      }
    }

    private class LogRequest : ILogRequest, ILogRequestInternal
    {
      private readonly VssRequestContext m_requestContext;
      private IList<MethodTime> m_sqlCalls;
      private const int c_sqlCallLogLimit = 5000;

      internal LogRequest(VssRequestContext requestContext) => this.m_requestContext = requestContext;

      public void LogItem(string itemKey, string itemValue)
      {
        Dictionary<string, string> dictionary;
        if (!this.m_requestContext.TryGetItem<Dictionary<string, string>>(RequestContextItemsKeys.LogItems, out dictionary))
        {
          dictionary = new Dictionary<string, string>();
          this.m_requestContext.Items[RequestContextItemsKeys.LogItems] = (object) dictionary;
        }
        dictionary[itemKey] = itemValue;
      }

      void ILogRequestInternal.LogSqlCall(MethodTime methodTime)
      {
        if (this.m_sqlCalls == null)
          this.m_sqlCalls = (IList<MethodTime>) new List<MethodTime>();
        if (this.m_sqlCalls.Count < 5000)
          this.m_sqlCalls.Add(methodTime);
        this.LogicalReads += methodTime.LogicalReads;
        this.PhysicalReads += methodTime.PhysicalReads;
        this.CpuTime += methodTime.CPUTime;
        this.ElapsedTime += methodTime.ElapsedTime;
      }

      IList<MethodTime> ILogRequestInternal.SqlCalls => this.m_sqlCalls;

      IList<MethodTime> ILogRequestInternal.RecursiveSqlCalls
      {
        get
        {
          VssRequestContext rootContext = this.m_requestContext.m_rootContext;
          IList<MethodTime> recursiveSqlCalls = (IList<MethodTime>) new List<MethodTime>();
          if (this.m_sqlCalls != null)
          {
            string str = VssRequestContext.HostTypeToString(rootContext.ServiceHost);
            foreach (MethodTime sqlCall in (IEnumerable<MethodTime>) this.m_sqlCalls)
              recursiveSqlCalls.Add(new MethodTime(string.Format("{0}.{1}", (object) str, (object) sqlCall.Name), sqlCall.Time, sqlCall.LogicalReads, sqlCall.PhysicalReads, sqlCall.CPUTime, sqlCall.ElapsedTime));
          }
          lock (rootContext.m_contextLock)
          {
            if (rootContext.m_systemRequestContexts != null)
            {
              foreach (IVssRequestContext vssRequestContext in rootContext.m_systemRequestContexts.Values)
              {
                ILogRequestInternal logRequestInternal = vssRequestContext.RequestLogger.RequestLoggerInternal(false);
                if (logRequestInternal != null && logRequestInternal.SqlCalls != null)
                {
                  string str = VssRequestContext.HostTypeToString(vssRequestContext.ServiceHost);
                  foreach (MethodTime sqlCall in (IEnumerable<MethodTime>) logRequestInternal.SqlCalls)
                    recursiveSqlCalls.Add(new MethodTime(string.Format("{0}.{1}", (object) str, (object) sqlCall.Name), sqlCall.Time, sqlCall.LogicalReads, sqlCall.PhysicalReads, sqlCall.CPUTime, sqlCall.ElapsedTime));
                }
              }
            }
            if (rootContext.m_servicingRequestContexts != null)
            {
              foreach (IVssRequestContext vssRequestContext in rootContext.m_servicingRequestContexts.Values)
              {
                ILogRequestInternal logRequestInternal = vssRequestContext.RequestLogger.RequestLoggerInternal(false);
                if (logRequestInternal != null && logRequestInternal.SqlCalls != null)
                {
                  string str = VssRequestContext.HostTypeToString(vssRequestContext.ServiceHost);
                  foreach (MethodTime sqlCall in (IEnumerable<MethodTime>) logRequestInternal.SqlCalls)
                    recursiveSqlCalls.Add(new MethodTime(string.Format("{0}.{1}", (object) str, (object) sqlCall.Name), sqlCall.Time, sqlCall.LogicalReads, sqlCall.PhysicalReads, sqlCall.CPUTime, sqlCall.ElapsedTime));
                }
              }
            }
            if (rootContext.m_userRequestContexts != null)
            {
              foreach (VssRequestContext vssRequestContext in rootContext.m_userRequestContexts.Values)
              {
                ILogRequestInternal logRequestInternal = vssRequestContext.RequestLogger.RequestLoggerInternal(false);
                if (logRequestInternal != null && logRequestInternal.SqlCalls != null)
                {
                  string str = VssRequestContext.HostTypeToString(vssRequestContext.ServiceHost);
                  foreach (MethodTime sqlCall in (IEnumerable<MethodTime>) logRequestInternal.SqlCalls)
                    recursiveSqlCalls.Add(new MethodTime(string.Format("{0}.{1}", (object) str, (object) sqlCall.Name), sqlCall.Time, sqlCall.LogicalReads, sqlCall.PhysicalReads, sqlCall.CPUTime, sqlCall.ElapsedTime));
                }
              }
            }
          }
          return recursiveSqlCalls;
        }
      }

      public int LogicalReads { get; private set; }

      public int PhysicalReads { get; private set; }

      public int CpuTime { get; private set; }

      public int ElapsedTime { get; private set; }
    }

    internal class TimeRequest : ITimeRequest, ITimeRequestInternal, IDisposable
    {
      private bool disposedValue;
      private KeyValuePair<DiagnosticLocation, int>[] m_timings;
      private ushort m_timingsIndex;
      private int m_fastMethodsCount;
      private int m_truncatedMethodsCount;
      private long m_lastPausedTime = -1;
      private long m_totalTimePaused;
      private bool m_timeToFirstPageTimerIsPaused;
      private long m_timeToFirstPageStopTime;
      private IVssRequestContext m_requestContext;
      private DateTime m_startTime;
      private DateTime m_endTime;
      private long m_requestStartTimestamp;
      private long m_requestEndTimestamp;
      private long m_requestManagedStartTimestamp;
      private long m_requestDelayTime;
      private long m_requestConcurrencySemaphoreTime;
      private const double c_microsecondConversionFactor = 1000000.0;
      private const long c_tickConversionFactor = 10000000;
      private const int c_minTraceTimeMS = 5;
      private static ConcurrentStack<KeyValuePair<DiagnosticLocation, int>[]> s_diagnosticTrackers = new ConcurrentStack<KeyValuePair<DiagnosticLocation, int>[]>();
      private static int s_diagnosticTrackersCount;

      private static KeyValuePair<DiagnosticLocation, int>[] GetTimingsArray(
        IVssRequestContext requestContext)
      {
        KeyValuePair<DiagnosticLocation, int>[] result = (KeyValuePair<DiagnosticLocation, int>[]) null;
        if (VssRequestContext.TimeRequest.s_diagnosticTrackers.TryPop(out result))
          Interlocked.Decrement(ref VssRequestContext.TimeRequest.s_diagnosticTrackersCount);
        else
          result = new KeyValuePair<DiagnosticLocation, int>[(int) byte.MaxValue];
        return result;
      }

      private static void PutTimingsArray(
        IVssRequestContext requestContext,
        KeyValuePair<DiagnosticLocation, int>[] timingsArray)
      {
        if (VssRequestContext.TimeRequest.s_diagnosticTrackersCount >= 500)
          return;
        Interlocked.Increment(ref VssRequestContext.TimeRequest.s_diagnosticTrackersCount);
        VssRequestContext.TimeRequest.s_diagnosticTrackers.Push(timingsArray);
      }

      internal TimeRequest(IVssRequestContext context)
      {
        this.m_requestContext = context;
        this.m_timings = VssRequestContext.TimeRequest.GetTimingsArray(this.m_requestContext);
      }

      internal long StartTimestamp
      {
        get => this.m_requestStartTimestamp;
        set
        {
          if (this.m_requestStartTimestamp != 0L && (value <= 0L || value >= this.m_requestStartTimestamp))
            return;
          this.m_requestStartTimestamp = value;
          if (this.m_requestManagedStartTimestamp != 0L)
            return;
          this.m_requestManagedStartTimestamp = this.m_requestStartTimestamp;
        }
      }

      public DateTime StartTime
      {
        get => this.m_startTime;
        set
        {
          if (!(this.m_startTime == DateTime.MinValue) && !(value < this.m_startTime))
            return;
          this.m_startTime = value;
        }
      }

      public DateTime EndTime
      {
        get => this.m_endTime;
        private set => this.m_endTime = value;
      }

      public long QueueTime => this.ElapsedMicroseconds(this.m_requestStartTimestamp, this.m_requestManagedStartTimestamp);

      public long DelayTime => this.ElapsedMicroseconds(0L, this.m_requestDelayTime);

      public TimeSpan DelaySpan => new TimeSpan(this.StopwatchToTimeSpan(0L, this.m_requestDelayTime));

      public long ConcurrencySemaphoreTime => this.ElapsedMicroseconds(0L, this.m_requestConcurrencySemaphoreTime);

      public long ExecutionTime => this.m_requestEndTimestamp == 0L ? this.ElapsedMicroseconds(this.m_requestStartTimestamp, Stopwatch.GetTimestamp(), this.m_requestDelayTime + this.m_requestConcurrencySemaphoreTime) : this.ElapsedMicroseconds(this.m_requestStartTimestamp, this.m_requestEndTimestamp, this.m_requestDelayTime + this.m_requestConcurrencySemaphoreTime);

      public TimeSpan ExecutionSpan => this.m_requestEndTimestamp == 0L ? new TimeSpan(this.StopwatchToTimeSpan(this.m_requestStartTimestamp, Stopwatch.GetTimestamp(), this.m_requestDelayTime + this.m_requestConcurrencySemaphoreTime)) : new TimeSpan(this.StopwatchToTimeSpan(this.m_requestStartTimestamp, this.m_requestEndTimestamp, this.m_requestDelayTime + this.m_requestConcurrencySemaphoreTime));

      public TimeSpan LastTracedBlockSpan { get; set; }

      internal KeyValuePair<DiagnosticLocation, int>[] TraceTimings => this.m_timings;

      internal int FastMethodsCount => this.m_fastMethodsCount;

      internal int TruncatedMethodsCount => this.m_truncatedMethodsCount;

      public long TimeToFirstPage
      {
        get
        {
          if (this.m_lastPausedTime == -1L)
            return this.m_timeToFirstPageStopTime;
          if (this.m_timeToFirstPageTimerIsPaused)
            return this.m_lastPausedTime - this.m_totalTimePaused;
          return this.m_timeToFirstPageStopTime > 0L ? this.m_timeToFirstPageStopTime - this.m_totalTimePaused : this.ExecutionTime - this.m_totalTimePaused;
        }
      }

      public long PreControllerTime { get; private set; }

      public long ControllerTime { get; private set; }

      public long PostControllerTime { get; private set; }

      public void SetTimeToFirstPageEnd()
      {
        if (this.m_timeToFirstPageStopTime != 0L || this.m_requestStartTimestamp <= 0L)
          return;
        if (this.m_requestEndTimestamp == 0L)
          this.m_timeToFirstPageStopTime = this.ElapsedMicroseconds(this.m_requestStartTimestamp, Stopwatch.GetTimestamp(), this.m_requestDelayTime + this.m_requestConcurrencySemaphoreTime);
        else
          this.m_timeToFirstPageStopTime = this.ElapsedMicroseconds(this.m_requestStartTimestamp, this.m_requestEndTimestamp, this.m_requestDelayTime + this.m_requestConcurrencySemaphoreTime);
      }

      public void SetTimeToFirstPageBegin()
      {
        if (this.m_timeToFirstPageStopTime != 0L || this.m_requestStartTimestamp <= 0L || this.m_requestEndTimestamp != 0L || this.m_lastPausedTime != -1L)
          return;
        this.m_lastPausedTime = 0L;
        this.m_totalTimePaused += this.ElapsedMicroseconds(this.m_requestStartTimestamp, Stopwatch.GetTimestamp(), this.m_requestDelayTime + this.m_requestConcurrencySemaphoreTime) - this.m_lastPausedTime;
      }

      public void PauseTimeToFirstPageTimer()
      {
        if (this.m_timeToFirstPageStopTime == 0L && this.m_requestStartTimestamp > 0L && this.m_requestEndTimestamp == 0L && !this.m_timeToFirstPageTimerIsPaused)
        {
          this.m_lastPausedTime = this.ElapsedMicroseconds(this.m_requestStartTimestamp, Stopwatch.GetTimestamp(), this.m_requestDelayTime + this.m_requestConcurrencySemaphoreTime);
          this.m_timeToFirstPageTimerIsPaused = true;
        }
        else if (this.m_timeToFirstPageTimerIsPaused && !this.m_requestContext.ServiceHost.IsProduction)
          throw new TimeToFirstPageTimerException(FrameworkResources.TimeToFirstPageTimerAlreadyPausedExceptionMessage());
      }

      public void ResumeTimeToFirstPageTimer()
      {
        if (this.m_timeToFirstPageStopTime != 0L || this.m_requestStartTimestamp <= 0L || this.m_requestEndTimestamp != 0L || !this.m_timeToFirstPageTimerIsPaused)
          return;
        this.m_totalTimePaused += this.ElapsedMicroseconds(this.m_requestStartTimestamp, Stopwatch.GetTimestamp(), this.m_requestDelayTime + this.m_requestConcurrencySemaphoreTime) - this.m_lastPausedTime;
        this.m_timeToFirstPageTimerIsPaused = false;
      }

      internal void BeginRequest()
      {
        if (this.StartTime == DateTime.MinValue)
          this.StartTime = DateTime.UtcNow;
        if (this.StartTimestamp != 0L)
          return;
        this.StartTimestamp = Stopwatch.GetTimestamp();
      }

      internal void EndRequest()
      {
        if (!(this.EndTime == DateTime.MinValue))
          return;
        this.m_requestEndTimestamp = Stopwatch.GetTimestamp();
        this.EndTime = DateTime.UtcNow;
      }

      public void RecordTraceEnterTiming(string area, string layer, string methodName)
      {
        if (this.m_timings == null || (int) this.m_timingsIndex >= this.m_timings.Length)
          return;
        this.m_timings[(int) this.m_timingsIndex++] = new KeyValuePair<DiagnosticLocation, int>(new DiagnosticLocation(area, layer, methodName), (int) this.ExecutionSpan.TotalMilliseconds);
      }

      public void RecordTraceLeaveTiming(string area, string layer, string methodName)
      {
        if (this.m_timingsIndex <= (ushort) 0 || this.m_timings == null)
          return;
        if ((int) this.m_timingsIndex < this.m_timings.Length)
        {
          DiagnosticLocation diagnosticLocation = new DiagnosticLocation(area, layer, methodName);
          int totalMilliseconds = (int) this.ExecutionSpan.TotalMilliseconds;
          if (totalMilliseconds - this.m_timings[(int) this.m_timingsIndex - 1].Value < 5 && this.m_timings[(int) this.m_timingsIndex - 1].Key.Equals(diagnosticLocation))
          {
            ++this.m_fastMethodsCount;
            this.m_timings[(int) --this.m_timingsIndex] = new KeyValuePair<DiagnosticLocation, int>();
          }
          else
            this.m_timings[(int) this.m_timingsIndex++] = new KeyValuePair<DiagnosticLocation, int>(diagnosticLocation, -totalMilliseconds);
        }
        else
          ++this.m_truncatedMethodsCount;
      }

      public DiagnosticDumper GetDiagnosticDumper(long executionThresholdMilliseconds) => new DiagnosticDumper(this.DelaySpan, this.ExecutionSpan, this.TraceTimings, this.FastMethodsCount, this.TruncatedMethodsCount, executionThresholdMilliseconds);

      private long ElapsedMicroseconds() => this.ElapsedMicroseconds(this.StartTimestamp, Stopwatch.GetTimestamp());

      private long ElapsedMicroseconds(long start, long stop, long delay = 0) => stop < start ? 0L : (long) (1000000.0 * ((double) (stop - start - delay) / (double) Stopwatch.Frequency));

      private long StopwatchToTimeSpan(long start, long stop, long delay = 0) => stop < start ? 0L : (long) (10000000.0 * ((double) (stop - start - delay) / (double) Stopwatch.Frequency));

      public void SetPreControllerTime()
      {
        this.CheckForPauseCrossingTimingBoundry();
        this.PreControllerTime = this.ElapsedMicroseconds() - this.m_totalTimePaused;
      }

      public void SetControllerTime()
      {
        if (this.PreControllerTime <= 0L)
          return;
        this.ControllerTime = this.ElapsedMicroseconds() - this.PreControllerTime - this.m_totalTimePaused;
        this.CheckForPauseCrossingTimingBoundry();
      }

      public void SetPostControllerTime()
      {
        if (this.PreControllerTime <= 0L)
          return;
        this.CheckForPauseCrossingTimingBoundry();
        this.PostControllerTime = this.ElapsedMicroseconds() - this.PreControllerTime - this.ControllerTime - this.m_totalTimePaused;
      }

      private void CheckForPauseCrossingTimingBoundry()
      {
        if (!this.m_timeToFirstPageTimerIsPaused)
          return;
        this.m_requestContext.TraceAlways(99366779, TraceLevel.Error, nameof (VssRequestContext), "HostManagement", "Timer was in a paused state while crossing Pre/Controller/Post timer boundry.");
      }

      void ITimeRequestInternal.AddTimeSpentDelayed(long delay) => this.m_requestDelayTime += delay;

      void ITimeRequestInternal.AddTimeSpentInConcurrencySemaphore(long timeQueued) => this.m_requestConcurrencySemaphoreTime += timeQueued;

      void ITimeRequestInternal.SetManagedStartTime(long managedStartTime) => this.m_requestManagedStartTimestamp = managedStartTime;

      protected virtual void Dispose(bool disposing)
      {
        if (this.disposedValue)
          return;
        if (disposing && this.m_timings != null)
        {
          for (int index = 0; index < (int) this.m_timingsIndex; ++index)
            this.m_timings[index] = new KeyValuePair<DiagnosticLocation, int>();
          VssRequestContext.TimeRequest.PutTimingsArray(this.m_requestContext, this.m_timings);
          this.m_timings = (KeyValuePair<DiagnosticLocation, int>[]) null;
        }
        this.disposedValue = true;
      }

      public void Dispose() => this.Dispose(true);

      public IDisposable CreateTimeToFirstPageExclusionBlock() => (IDisposable) new VssRequestContext.TimeRequest.TimeToFirstPageExclusionBlock((ITimeRequest) this);

      private class TimeToFirstPageExclusionBlock : IDisposable
      {
        private readonly ITimeRequest m_timer;

        public TimeToFirstPageExclusionBlock(ITimeRequest timer)
        {
          this.m_timer = timer;
          this.m_timer.PauseTimeToFirstPageTimer();
        }

        public void Dispose() => this.m_timer.ResumeTimeToFirstPageTimer();
      }
    }

    public class TraceRequest : ITraceRequest, ITraceRequestInternal
    {
      private VssRequestContext m_requestContext;
      private Guid m_serviceHostId;
      private TeamFoundationTracingService m_tracingService;

      internal TraceRequest(VssRequestContext requestContext, Guid serviceHostId)
      {
        this.m_requestContext = requestContext;
        this.m_serviceHostId = serviceHostId;
        this.m_tracingService = requestContext.ServiceHost.DeploymentServiceHost.DeploymentServiceHostInternal().TracingService;
      }

      public bool IsTracing(
        int tracepoint,
        TraceLevel level,
        string area,
        string layer,
        string[] tags = null)
      {
        if (this.m_tracingService == null || !this.m_tracingService.IsStarted)
          return TeamFoundationTracingService.IsRawTracingEnabled(tracepoint, level, area, layer, tags);
        return ((ITraceRequestInternal) this).TracingService.IsTraceEnabled((IVssRequestContext) this.m_requestContext, ref new TraceEvent()
        {
          Tracepoint = tracepoint,
          Area = area,
          Layer = layer,
          Level = level,
          Method = this.m_requestContext.Method != null ? this.m_requestContext.Method.Name : (string) null,
          UserAgent = this.m_requestContext.UserAgent,
          UserLogin = this.m_requestContext.AuthenticatedUserName,
          Service = this.m_requestContext.ServiceName,
          ServiceHost = this.m_serviceHostId,
          Uri = this.m_requestContext.RawUrl(),
          Tags = tags
        });
      }

      public void Trace(
        int tracepoint,
        TraceLevel level,
        string area,
        string layer,
        string[] tags,
        string format)
      {
        TraceEvent traceEvent = new TraceEvent((IVssRequestContext) this.m_requestContext, tracepoint, level, area, layer, tags, (string) null, format);
        this.TraceCore(ref traceEvent, false);
      }

      public void Trace(
        int tracepoint,
        TraceLevel level,
        string area,
        string layer,
        string[] tags,
        string format,
        object arg0)
      {
        TraceEvent traceEvent = new TraceEvent((IVssRequestContext) this.m_requestContext, tracepoint, level, area, layer, tags, (string) null, format, arg0);
        this.TraceCore(ref traceEvent, false);
      }

      public void Trace(
        int tracepoint,
        TraceLevel level,
        string area,
        string layer,
        string[] tags,
        string format,
        object arg0,
        object arg1)
      {
        TraceEvent traceEvent = new TraceEvent((IVssRequestContext) this.m_requestContext, tracepoint, level, area, layer, tags, (string) null, format, arg0, arg1);
        this.TraceCore(ref traceEvent, false);
      }

      public void Trace(
        int tracepoint,
        TraceLevel level,
        string area,
        string layer,
        string[] tags,
        string format,
        object arg0,
        object arg1,
        object arg2)
      {
        TraceEvent traceEvent = new TraceEvent((IVssRequestContext) this.m_requestContext, tracepoint, level, area, layer, tags, (string) null, format, arg0, arg1, arg2);
        this.TraceCore(ref traceEvent, false);
      }

      public void Trace(
        int tracepoint,
        TraceLevel level,
        string area,
        string layer,
        string[] tags,
        string format,
        params object[] args)
      {
        TraceEvent traceEvent = new TraceEvent((IVssRequestContext) this.m_requestContext, tracepoint, level, area, layer, tags, (string) null, format, args);
        this.TraceCore(ref traceEvent, false);
      }

      public void TraceAlways(
        int tracepoint,
        TraceLevel level,
        string area,
        string layer,
        string[] tags,
        string format,
        params object[] args)
      {
        TraceEvent traceEvent = new TraceEvent((IVssRequestContext) this.m_requestContext, tracepoint, level, area, layer, tags, (string) null, format, args);
        this.TraceCore(ref traceEvent, true);
      }

      public void TraceAlways(
        int tracepoint,
        TraceLevel level,
        string area,
        string layer,
        string format,
        params object[] args)
      {
        this.TraceAlways(tracepoint, level, area, layer, (string[]) null, format, args);
      }

      public void TraceEnter(int tracepoint, string area, string layer, [CallerMemberName] string methodName = null)
      {
        if (!this.m_requestContext.IsRootContext)
        {
          this.m_requestContext.RootContext.TraceEnter(tracepoint, area, layer, methodName);
        }
        else
        {
          this.Trace(tracepoint, TraceLevel.Verbose, area, layer, VssRequestContext.s_tagPerformance, "Entering {0}", (object) methodName);
          if (SqlStatisticsContext.CollectingStatistics)
            SqlStatisticsContext.Enter(methodName);
          this.m_requestContext.m_timer.RecordTraceEnterTiming(area, layer, methodName);
        }
      }

      public void TraceLeave(int tracepoint, string area, string layer, [CallerMemberName] string methodName = null)
      {
        if (!this.m_requestContext.IsRootContext)
        {
          this.m_requestContext.RootContext.TraceLeave(tracepoint, area, layer, methodName);
        }
        else
        {
          if (SqlStatisticsContext.CollectingStatistics)
            SqlStatisticsContext.Leave(methodName);
          this.m_requestContext.m_timer.RecordTraceLeaveTiming(area, layer, methodName);
          this.Trace(tracepoint, TraceLevel.Verbose, area, layer, VssRequestContext.s_tagPerformance, "Leaving {0}", (object) methodName);
        }
      }

      public void TraceException(
        int tracepoint,
        TraceLevel level,
        string area,
        string layer,
        Exception exception)
      {
        TraceEvent traceEvent = new TraceEvent((IVssRequestContext) this.m_requestContext, tracepoint, level, area, layer, (string[]) null, exception.GetType().FullName, "{0}", (object) new Lazy<string>((Func<string>) (() => exception.ToReadableStackTrace())));
        this.TraceCore(ref traceEvent, false);
      }

      public void TraceException(
        int tracepoint,
        TraceLevel level,
        string area,
        string layer,
        Exception exception,
        string format,
        params object[] args)
      {
        TraceEvent traceEvent = new TraceEvent((IVssRequestContext) this.m_requestContext, tracepoint, level, area, layer, (string[]) null, exception.GetType().FullName, format, args);
        this.TraceCore(ref traceEvent, false);
      }

      TeamFoundationTracingService ITraceRequestInternal.TracingService
      {
        get
        {
          if (this.m_tracingService == null)
          {
            using (this.m_requestContext.AcquireExemptionLock())
              this.m_tracingService = this.m_requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTracingService>();
          }
          return this.m_tracingService;
        }
      }

      void ITraceRequestInternal.TraceSql(
        int tracepoint,
        string dataSource,
        string database,
        string operation,
        short retries,
        bool success,
        int totalTime,
        int connectTime,
        int executionTime,
        int waitTime,
        int sqlErrorCode,
        string sqlErrorMessage)
      {
        if (this.m_tracingService != null)
        {
          if (this.m_tracingService.IsStarted)
          {
            try
            {
              ((ITraceRequestInternal) this).TracingService.TraceSql((IVssRequestContext) this.m_requestContext, tracepoint, dataSource, database, operation, retries, success, totalTime, connectTime, executionTime, waitTime, sqlErrorCode, sqlErrorMessage);
              return;
            }
            catch (Exception ex)
            {
              return;
            }
          }
        }
        TeamFoundationTracingService.TraceSqlRaw(tracepoint, dataSource, database, operation, retries, success, totalTime, connectTime, executionTime, waitTime, sqlErrorCode, sqlErrorMessage);
      }

      private void TraceCore(ref TraceEvent traceEvent, bool traceAlways)
      {
        try
        {
          if (this.m_requestContext.m_isDisposed)
            TeamFoundationTracingService.TraceRaw(ref traceEvent, traceAlways);
          else if (this.m_tracingService == null || !this.m_tracingService.IsStarted)
            TeamFoundationTracingService.TraceRaw(ref traceEvent, traceAlways);
          else
            ((ITraceRequestInternal) this).TracingService.Trace((IVssRequestContext) this.m_requestContext, ref traceEvent, traceAlways);
        }
        catch (Exception ex)
        {
        }
      }
    }
  }
}
