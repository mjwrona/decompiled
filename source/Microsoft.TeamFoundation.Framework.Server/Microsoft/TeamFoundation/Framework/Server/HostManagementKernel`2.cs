// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostManagementKernel`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HostManagementKernel<Host, Request>
    where Host : ServiceHost<Request>
    where Request : class, IVssRequestContext
  {
    private static uint s_processId = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetCurrentProcessId();
    private SemaphoreSlim m_semaphore = new SemaphoreSlim(50);
    private LockManager m_lockManager;
    private HostTable<Host> m_hostTable;
    private Host m_rootHost;
    private HostPropertiesTable m_serviceHostProperties;
    private HostManagementKernel<Host, Request>.LoadHost m_loadHost;
    private ILockName m_mapLock;
    private ManualResetEvent m_started;
    private bool m_isHealthy;
    private LockName<short, Guid, short>[] m_deploymentHostLock;
    internal readonly int c_hashBuckets = Math.Min(Environment.ProcessorCount * 4, 64);
    private bool m_isShuttingDown;
    private int m_serviceHostTablePartitions = 16;
    private int m_maxHosts;
    private int m_minHosts;
    private static readonly string s_Area = "HostManagement";
    private static readonly string s_Layer = "Kernel";
    internal const short MaxTreeDepth = 3;
    private readonly object m_internalLock = new object();
    private Func<Guid, List<HostProperties>> m_resolveHost;

    public HostManagementKernel(
      Host rootHost,
      HostManagementKernel<Host, Request>.LoadHost loadHost,
      long requestId,
      int numberOfPartitions = 16,
      int maxTotalHosts = 7680,
      int minHosts = 1)
    {
      TeamFoundationTracingService.TraceEnterRaw(58020, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, nameof (HostManagementKernel<Host, Request>), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      this.m_minHosts = minHosts;
      this.m_lockManager = (LockManager) new HostManagementLockManager();
      this.m_maxHosts = maxTotalHosts;
      this.m_serviceHostTablePartitions = numberOfPartitions;
      this.m_started = new ManualResetEvent(false);
      this.m_serviceHostProperties = new HostPropertiesTable();
      this.m_hostTable = new HostTable<Host>(this.m_lockManager, this.m_serviceHostTablePartitions);
      this.m_loadHost = loadHost;
      this.m_mapLock = (ILockName) new LockName<string>("Map", LockLevel.Last);
      this.m_deploymentHostLock = new LockName<short, Guid, short>[this.c_hashBuckets];
      for (short nameValue3 = 0; (int) nameValue3 < this.c_hashBuckets; ++nameValue3)
        this.m_deploymentHostLock[(int) nameValue3] = new LockName<short, Guid, short>((short) 1, rootHost.InstanceId, nameValue3, LockLevel.Host);
      this.m_rootHost = rootHost;
      this.m_isShuttingDown = false;
      HostProperties properties = new HostProperties(this.m_rootHost.ServiceHostProperties);
      using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockExclusive, requestId))
      {
        this.TraceRaw(58026, TraceLevel.Info, this.m_rootHost.InstanceId, "Adding Root Host {0} to our map", (object) this.m_rootHost.InstanceId);
        this.m_serviceHostProperties.Add(properties);
      }
      this.TraceRaw(58028, TraceLevel.Info, this.m_rootHost.InstanceId, "Adding Root Host {0} to our table of live hosts", (object) this.m_rootHost.InstanceId);
      this.m_hostTable.AddHost(properties, this.m_rootHost, requestId);
      TeamFoundationTracingService.TraceLeaveRaw(58029, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, nameof (HostManagementKernel<Host, Request>));
    }

    public void Start(
      List<HostProperties> serviceHostProperties,
      Func<Guid, List<HostProperties>> resolveHost,
      long requestId)
    {
      TeamFoundationTracingService.TraceEnterRaw(58040, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "HostManagementKernel::Start", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      this.m_isShuttingDown = false;
      this.m_isHealthy = false;
      this.m_started.Reset();
      try
      {
        this.m_resolveHost = resolveHost;
        using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockExclusive, requestId))
        {
          serviceHostProperties.Sort(new Comparison<HostProperties>(this.CompareHostPropertiesRootFirst));
          foreach (HostProperties serviceHostProperty in serviceHostProperties)
          {
            if (serviceHostProperty.Id != this.m_rootHost.InstanceId)
            {
              this.AddServiceHost(requestId, serviceHostProperty);
            }
            else
            {
              this.m_rootHost.ServiceHostProperties.UpdateProperties(serviceHostProperty);
              HostProperties properties;
              this.m_serviceHostProperties.TryGetValue(this.m_rootHost.InstanceId, out properties);
              properties.UpdateProperties(serviceHostProperty);
            }
            if (serviceHostProperty.ParentId == Guid.Empty)
            {
              int num = this.m_rootHost.InstanceId != serviceHostProperty.Id ? 1 : 0;
            }
          }
        }
        Request request = this.CreateRequest(this.m_rootHost.InstanceId, requestId, RequestContextType.SystemContext, HostRequestType.Default, true, false, true);
        try
        {
          TeamFoundationTracingService.TraceRaw(58046, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Starting Root Host");
          this.m_rootHost.Start(request);
        }
        finally
        {
          if ((object) request != null)
            request.Dispose();
        }
        this.m_isHealthy = true;
      }
      finally
      {
        TeamFoundationTracingService.TraceRaw(58048, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Host Management Kernel successfully started");
        this.m_started.Set();
        TeamFoundationTracingService.TraceLeaveRaw(58049, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "HostManagementKernel::Start");
      }
    }

    public int HostCount => this.m_hostTable.Count;

    public void Stop(long requestId)
    {
      StackTracer tracer = new StackTracer();
      this.Stop(requestId, 300, (TimerCallback) (state =>
      {
        TeamFoundationTracingService.TraceRawAlwaysOn(58066, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Stop is taking too long - stop was triggered by {0} - exiting process now", (object) tracer);
        Environment.Exit(258);
      }));
    }

    internal void Stop(long requestId, int timeoutInSeconds, TimerCallback timeoutCallback)
    {
      TeamFoundationTracingService.TraceEnterRaw(58060, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "HostManagementKernel::Stop", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      using (new Timer(timeoutCallback, (object) null, timeoutInSeconds * 1000, -1))
      {
        lock (this.m_internalLock)
        {
          this.m_isShuttingDown = true;
          TeamFoundationTracingService.TraceRaw(58066, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Taking shared lock on Deployment Host");
          Stack<IDisposable> disposableStack = new Stack<IDisposable>();
          try
          {
            for (int requestId1 = 0; requestId1 < this.c_hashBuckets; ++requestId1)
            {
              TeamFoundationTracingService.TraceRawAlwaysOn(58067, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, string.Format("Taking update lock {0} on Deployment Host", (object) this.GetDeploymentHostLock((long) requestId1)));
              disposableStack.Push((IDisposable) this.m_lockManager.Lock((ILockName) this.GetDeploymentHostLock((long) requestId1), LockManager.LockType.HostLockUpdate, requestId));
            }
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
              Task task = Task.Run((Func<Task>) (async () =>
              {
                try
                {
                  while (!cancellationTokenSource.IsCancellationRequested)
                  {
                    this.CancelAllRequestsRecursive(this.m_rootHost.InstanceId, (long) -Environment.CurrentManagedThreadId);
                    await Task.Delay(100, cancellationTokenSource.Token);
                  }
                }
                catch (TaskCanceledException ex)
                {
                }
              }));
              for (int requestId2 = 0; requestId2 < this.c_hashBuckets; ++requestId2)
              {
                TeamFoundationTracingService.TraceRawAlwaysOn(58068, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, string.Format("Taking exclusive lock {0} on Deployment Host", (object) this.GetDeploymentHostLock((long) requestId2)));
                disposableStack.Push((IDisposable) this.m_lockManager.Lock((ILockName) this.GetDeploymentHostLock((long) requestId2), LockManager.LockType.HostLockExclusive, requestId));
              }
              TeamFoundationTracingService.TraceRawAlwaysOn(58074, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Got Exclusive Lock - waiting for shutdown thread to terminate");
              cancellationTokenSource.Cancel();
              task.Wait();
            }
            TeamFoundationTracingService.TraceRawAlwaysOn(58075, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Tearing down now");
            this.Teardown(requestId);
            TeamFoundationTracingService.TraceRawAlwaysOn(58076, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Shutdown complete");
          }
          finally
          {
            while (disposableStack.Count > 0)
              disposableStack.Pop().Dispose();
          }
        }
        this.m_isShuttingDown = false;
      }
      TeamFoundationTracingService.TraceLeaveRaw(58079, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "HostManagementKernel::Stop");
    }

    internal Request CreateRequest(
      Guid hostId,
      long requestId,
      RequestContextType requestKind,
      HostRequestType type,
      bool loadIfNecessary = true,
      bool throwIfShutdown = false,
      bool flushNotificationQueue = true,
      params object[] additionalParameters)
    {
      return this.TryCreateRequest(hostId, requestId, requestKind, type, TimeSpan.MaxValue, loadIfNecessary, throwIfShutdown, flushNotificationQueue, additionalParameters);
    }

    internal Request TryCreateRequest(
      Guid hostId,
      long requestId,
      RequestContextType requestKind,
      HostRequestType type,
      TimeSpan timeout,
      bool loadIfNecessary = true,
      bool throwIfShutdown = false,
      bool flushNotificationQueue = true,
      params object[] additionalParameters)
    {
      TeamFoundationTracingService.TraceEnterRaw(58090, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "HostManagementKernel::CreateRequest", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        Host host1 = default (Host);
        KeyValuePair<LockName<short, Guid, short>, bool>[] lockNames = (KeyValuePair<LockName<short, Guid, short>, bool>[]) null;
        LockHelper lockHelper1 = (LockHelper) null;
        HostProperties properties1 = (HostProperties) null;
        HostProperties properties2 = (HostProperties) null;
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        while (true)
        {
          using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
          {
            if (!this.m_serviceHostProperties.TryGetValue(hostId, out properties1))
            {
              TeamFoundationTracingService.TraceRaw(58092, TraceLevel.Warning, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Cannot find host {0}", (object) hostId);
              flag1 = true;
            }
            else if (throwIfShutdown)
            {
              if (!this.IsStarted(hostId, requestId))
                flag2 = true;
            }
          }
          if (flag1 | flag2)
          {
            if (!flag3)
            {
              try
              {
                this.RefreshHostByGuid(hostId, requestId);
              }
              catch (Exception ex)
              {
                throw new ArgumentException(string.Format("Unable to refresh Host with Id = {0} for RequestId = {1}. Host properties: {2}. ", (object) hostId, (object) requestId, (object) properties1.Serialize<HostProperties>()) + ex.Message, nameof (hostId), ex.InnerException);
              }
              flag3 = true;
            }
          }
          using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
          {
            if (!this.m_serviceHostProperties.TryGetValue(hostId, out properties1))
            {
              TeamFoundationTracingService.TraceRaw(58100, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Host {0} does not exist!", (object) hostId);
              throw new HostDoesNotExistException(hostId);
            }
            if (requestKind != RequestContextType.ServicingContext && throwIfShutdown && !this.IsStarted(hostId, requestId))
            {
              TeamFoundationTracingService.TraceRaw(58102, TraceLevel.Warning, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Host {0} is currently in {1} status", (object) hostId, (object) properties1.Status);
              properties1.ThrowShutdownException(type);
            }
            lockNames = this.GetLockNamesForHostId(hostId, requestId);
            if (properties1 != null)
            {
              if ((properties1.HostType & TeamFoundationHostType.ProjectCollection) == TeamFoundationHostType.ProjectCollection)
                this.m_serviceHostProperties.TryGetValue(properties1.ParentId, out properties2);
            }
          }
          bool flag4 = false;
          try
          {
            if (!LockHelper.TryGetLock(this.m_lockManager, requestId, lockNames, LockManager.LockType.HostLockShared, timeout, out lockHelper1))
              return default (Request);
            host1 = default (Host);
            if (type == HostRequestType.Job || type == HostRequestType.AspNet || type == HostRequestType.Ssh)
              properties1.LastAccessTime = DateTime.UtcNow;
            if (this.m_hostTable.TryGetHost(properties1, requestId, out host1))
            {
              if ((type == HostRequestType.Job || type == HostRequestType.AspNet || type == HostRequestType.Ssh) && host1.LastUse.AddSeconds(5.0) < DateTime.UtcNow)
              {
                host1.LastUse = DateTime.UtcNow;
                this.m_hostTable.SetLastAccessTime(properties1, requestId);
                if (properties2 != null && (properties2.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Unknown)
                  this.m_hostTable.SetLastAccessTime(properties2, requestId);
              }
              flag4 = true;
              return host1.CreateRequest(requestKind, lockHelper1, type, additionalParameters);
            }
          }
          catch (Exception ex)
          {
            flag4 = false;
            TeamFoundationTracingService.TraceExceptionRaw(58104, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, ex);
            throw;
          }
          finally
          {
            if (!flag4 && lockHelper1 != null)
            {
              lockHelper1.Dispose();
              lockHelper1 = (LockHelper) null;
            }
          }
          if (loadIfNecessary)
          {
            int partitionSize = this.m_hostTable.GetPartitionSize(requestId, properties1);
            int partitionId = this.m_hostTable.GetPartitionId(properties1);
            if (partitionSize >= this.m_maxHosts / this.m_serviceHostTablePartitions && this.HasOnlyDeploymentHostLock(requestId))
            {
              TeamFoundationTracingService.TraceRaw(58107, TraceLevel.Warning, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Host Table Partition {0} is too full ({1}) - evicting hosts now", (object) partitionId, (object) partitionSize);
              this.EvictFromPartition(requestId, partitionId, TimeSpan.Zero, true);
            }
            for (int index = 1; index < lockNames.Length - 1; ++index)
            {
              Request request = this.CreateRequest(lockNames[index].Key.NameValue2, requestId, requestKind, HostRequestType.Default, loadIfNecessary, throwIfShutdown, true);
              try
              {
              }
              finally
              {
                if ((object) request != null)
                  request.Dispose();
              }
            }
            LockHelper lockHelper2 = (LockHelper) null;
            try
            {
              Random random = new Random(Environment.CurrentManagedThreadId);
              if (LockHelper.TryGetLock(this.m_lockManager, requestId, lockNames, LockManager.LockType.HostLockExclusive, TimeSpan.FromMilliseconds((double) random.Next(5, 25)), out lockHelper2))
              {
                Guid nameValue2 = lockNames[lockNames.Length - 2].Key.NameValue2;
                Host host2 = default (Host);
                if (!this.m_hostTable.TryGetHost(properties1, requestId, out host1))
                {
                  bool flag5;
                  using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
                  {
                    if (!this.NeedsRetry(hostId, throwIfShutdown && requestKind != 0, lockNames, requestId))
                    {
                      properties2 = this.m_serviceHostProperties[properties1.ParentId];
                      properties1 = this.m_serviceHostProperties[hostId];
                      flag5 = this.IsStarted(hostId, requestId);
                    }
                    else
                      continue;
                  }
                  if (this.m_hostTable.TryGetHost(properties2, requestId, out host2))
                  {
                    host1 = this.m_loadHost(properties1, host2);
                    host1.LastUse = DateTime.UtcNow;
                    this.m_hostTable.AddHost(properties1, host1, requestId);
                    if (flag5)
                    {
                      Request request = this.CreateRequest(this.m_rootHost.InstanceId, requestId, RequestContextType.SystemContext, HostRequestType.Default, loadIfNecessary, throwIfShutdown, true);
                      try
                      {
                        host1.Start(request);
                      }
                      finally
                      {
                        if ((object) request != null)
                          request.Dispose();
                      }
                    }
                  }
                }
              }
            }
            finally
            {
              lockHelper2?.Dispose();
            }
          }
          else
            break;
        }
        TeamFoundationTracingService.TraceRaw(58106, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Not starting request because host {0} is dormant", (object) hostId);
        return default (Request);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(58112, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(58114, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "HostManagementKernel::CreateRequest");
      }
    }

    private bool HasOnlyDeploymentHostLock(long requestId)
    {
      int numberOfLocksHeld = this.m_lockManager.GetNumberOfLocksHeld(requestId);
      return numberOfLocksHeld == 0 || numberOfLocksHeld == 1;
    }

    private bool IsStarted(Guid hostId, long requestId)
    {
      HostProperties properties;
      while (this.m_serviceHostProperties.TryGetValue(hostId, out properties))
      {
        if (properties.Status != TeamFoundationServiceHostStatus.Started)
          return false;
        hostId = properties.ParentId;
        if (!(hostId != Guid.Empty))
          return true;
      }
      TeamFoundationTracingService.TraceRaw(58110, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Host {0} not found - retrying request", (object) hostId);
      return false;
    }

    public List<HostProperties> GetServiceHostChildrenProperties(Guid hostId, long requestId)
    {
      if (!this.m_serviceHostProperties.TryGetValue(hostId, out HostProperties _))
        this.RefreshHostByGuid(hostId, requestId);
      using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
        return this.m_serviceHostProperties.GetServiceHostChildrenProperties(hostId);
    }

    private LockName<short, Guid, short> GetDeploymentHostLock(long requestId) => this.m_deploymentHostLock[Math.Abs(requestId % (long) this.c_hashBuckets)];

    private bool NeedsRetry(
      Guid hostId,
      bool throwIfShutdown,
      KeyValuePair<LockName<short, Guid, short>, bool>[] lockNames,
      long requestId)
    {
      if (!this.CheckLockNamesForHostId(hostId, lockNames, requestId))
      {
        TeamFoundationTracingService.TraceRaw(58108, TraceLevel.Warning, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Host {0} moved - retrying request", (object) hostId);
        return true;
      }
      if (!(!this.IsStarted(hostId, requestId) & throwIfShutdown))
        return false;
      TeamFoundationTracingService.TraceRaw(58111, TraceLevel.Warning, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Host {0} not started - retrying request", (object) hostId);
      return true;
    }

    public HostProperties GetServiceHostProperties(Guid hostId, long requestId)
    {
      HostProperties serviceHostProperties = this.GetHostProperties(requestId, hostId);
      if (serviceHostProperties == null)
      {
        List<HostProperties> source = this.m_resolveHost(hostId);
        if (source != null)
          serviceHostProperties = source.FirstOrDefault<HostProperties>((Func<HostProperties, bool>) (p => p.Id == hostId));
      }
      return serviceHostProperties;
    }

    private bool IsUpdate(HostProperties existingProperties, HostProperties newProperties) => string.Compare(existingProperties.Name, newProperties.Name, StringComparison.OrdinalIgnoreCase) != 0 || existingProperties.DatabaseId != newProperties.DatabaseId;

    private bool IsStatusChange(HostProperties existingProperties, HostProperties newProperties) => existingProperties.Status != newProperties.Status;

    private bool RefreshHost(
      Func<List<HostProperties>> refreshHost,
      Func<bool> checkDelegate,
      string lockName,
      long requestId)
    {
      bool flag = false;
      List<HostProperties> source1 = new List<HostProperties>(2);
      List<HostProperties> source2 = new List<HostProperties>(2);
      using (this.m_lockManager.Lock((ILockName) new LockName<string>(lockName, LockLevel.Authority), LockManager.LockType.AuthorityExclusive, requestId))
      {
        if (checkDelegate != null && checkDelegate())
          return true;
        this.m_semaphore.Wait();
        List<HostProperties> hostPropertiesList;
        try
        {
          hostPropertiesList = refreshHost();
        }
        finally
        {
          this.m_semaphore.Release();
        }
        if (hostPropertiesList != null)
        {
          if (hostPropertiesList.Count > 0)
          {
            flag = true;
            hostPropertiesList.Sort(new Comparison<HostProperties>(this.CompareHostPropertiesRootFirst));
            foreach (HostProperties hostProperties in hostPropertiesList)
            {
              using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockExclusive, requestId))
              {
                if ((hostProperties.HostType & TeamFoundationHostType.Deployment) != TeamFoundationHostType.Deployment)
                {
                  HostProperties properties;
                  if (this.m_serviceHostProperties.TryGetValue(hostProperties.Id, out properties))
                  {
                    if (this.IsUpdate(properties, hostProperties))
                      source1.Add(hostProperties);
                    if (this.IsStatusChange(properties, hostProperties))
                      source2.Add(hostProperties);
                  }
                  else
                    this.AddServiceHost(requestId, hostProperties);
                }
              }
            }
          }
        }
      }
      if (source1.Count > 0 || source2.Count > 0)
      {
        flag = true;
        Guid id = (source1.FirstOrDefault<HostProperties>() ?? source2.FirstOrDefault<HostProperties>()).Id;
        TeamFoundationTracingService.TraceRaw(58096, TraceLevel.Warning, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Flushing Notification Queue to wait for host {0}", (object) id);
        Request request = this.CreateRequest(this.m_rootHost.InstanceId, requestId, RequestContextType.SystemContext, HostRequestType.Default, false, false, true);
        try
        {
          this.m_rootHost.FlushNotificationQueue(request);
        }
        finally
        {
          if ((object) request != null)
            request.Dispose();
        }
        TeamFoundationTracingService.TraceRaw(58098, TraceLevel.Warning, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Flushed Notification Queue to wait for host {0}", (object) id);
      }
      return flag;
    }

    public void CheckForDormantHosts(TimeSpan timeToSleep, int dormancyThreads)
    {
      TeamFoundationTracingService.TraceEnterRaw(58120, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, nameof (CheckForDormantHosts), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      if (this.m_hostTable.Count <= this.m_minHosts)
        return;
      int numberOfPartitions = this.m_hostTable.NumberOfPartitions;
      ParallelOptions parallelOptions = new ParallelOptions();
      parallelOptions.MaxDegreeOfParallelism = Math.Min(this.m_hostTable.NumberOfPartitions, Math.Min(Environment.ProcessorCount, dormancyThreads));
      Action<int> body = (Action<int>) (partitionId => this.EvictFromPartition((long) -Environment.CurrentManagedThreadId, partitionId, timeToSleep, false));
      Parallel.For(0, numberOfPartitions, parallelOptions, body);
    }

    private Guid GetActivityId(int partitionId)
    {
      long ticks = DateTime.UtcNow.Ticks;
      return new Guid(HostManagementKernel<Host, Request>.s_processId, (ushort) 25858, (ushort) partitionId, (byte) (ticks >> 56), (byte) (ticks >> 48), (byte) (ticks >> 40), (byte) (ticks >> 32), (byte) (ticks >> 24), (byte) (ticks >> 16), (byte) (ticks >> 8), (byte) ticks);
    }

    private void EvictFromPartition(
      long requestId,
      int partitionId,
      TimeSpan timeToSleep,
      bool inline)
    {
      int num = 0;
      List<HostProperties> hostPropertiesList = inline ? this.m_hostTable.GetLeastRecentlyUsed(partitionId, requestId, 3) : this.m_hostTable.CheckForDormancyCandidates(partitionId, requestId, DateTime.UtcNow.Add(-timeToSleep));
      if (hostPropertiesList == null)
        return;
      if (hostPropertiesList.Count == 0)
        return;
      try
      {
        if (!inline)
        {
          this.AssertNoLocksHeld(requestId, "Dormancy Thread should have no locks at this point");
          Trace.CorrelationManager.ActivityId = this.GetActivityId(partitionId);
        }
        if (hostPropertiesList.Count <= 0)
          return;
        foreach (HostProperties hostProperties in hostPropertiesList)
        {
          if (inline && num >= 1)
          {
            this.TraceRaw(58121, TraceLevel.Info, hostProperties.Id, "Thread {0} evicted {1} hosts", (object) Environment.CurrentManagedThreadId, (object) num);
            break;
          }
          KeyValuePair<LockName<short, Guid, short>, bool>[] lockNames = (KeyValuePair<LockName<short, Guid, short>, bool>[]) null;
          HostProperties properties1 = (HostProperties) null;
          HostProperties properties2;
          KeyValuePair<LockName<short, Guid, short>, bool>[] lockNamesForHostId;
          using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
          {
            if (!this.m_serviceHostProperties.TryGetValue(hostProperties.Id, out properties2))
            {
              this.TraceRaw(58129, TraceLevel.Info, hostProperties.Id, "Host not found - skipping");
              continue;
            }
            lockNamesForHostId = this.GetLockNamesForHostId(hostProperties.Id, requestId);
            if (properties2.HostType == TeamFoundationHostType.ProjectCollection)
            {
              if (hostProperties.ParentId != this.m_rootHost.InstanceId)
              {
                if (this.m_serviceHostProperties.TryGetValue(hostProperties.ParentId, out properties1))
                  lockNames = this.GetLockNamesForHostId(hostProperties.ParentId, requestId);
              }
            }
          }
          if (this.StopHostTask(requestId, lockNamesForHostId, properties2, timeToSleep))
          {
            ++num;
            if (lockNames != null && this.StopHostTask(requestId, lockNames, properties1, timeToSleep))
              ++num;
          }
        }
      }
      finally
      {
        if (!inline)
        {
          Trace.CorrelationManager.ActivityId = Guid.Empty;
          this.AssertNoLocksHeld(requestId, "Dormancy Thread should have no locks at this point");
        }
      }
    }

    public void SetLastAccessTime(long requestId, Guid hostId)
    {
      HostProperties hostProperties1 = this.GetHostProperties(requestId, hostId);
      if (hostProperties1 != null)
        this.m_hostTable.SetLastAccessTime(hostProperties1, requestId);
      if ((hostProperties1.HostType & TeamFoundationHostType.ProjectCollection) != TeamFoundationHostType.ProjectCollection)
        return;
      HostProperties hostProperties2 = this.GetHostProperties(requestId, hostProperties1.ParentId);
      if ((hostProperties2.HostType & TeamFoundationHostType.Deployment) != TeamFoundationHostType.Unknown)
        return;
      this.m_hostTable.SetLastAccessTime(hostProperties2, requestId);
    }

    public void ForceSetLastAccessTime(long requestId, Guid hostId, DateTime lastAccessTime)
    {
      HostProperties hostProperties = this.GetHostProperties(requestId, hostId);
      if (hostProperties == null)
        return;
      this.m_hostTable.ForceSetLastAccessTime(hostProperties, requestId, lastAccessTime);
    }

    private HostProperties GetHostProperties(long requestId, Guid hostId)
    {
      HostProperties properties = (HostProperties) null;
      using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
      {
        if (!this.m_serviceHostProperties.TryGetValue(hostId, out properties))
          TeamFoundationTracingService.TraceRaw(58099, TraceLevel.Warning, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Cannot find host {0}", (object) hostId);
      }
      return properties;
    }

    private bool StopHostTask(
      long requestId,
      KeyValuePair<LockName<short, Guid, short>, bool>[] lockNames,
      HostProperties hostProperties,
      TimeSpan timeToSleep)
    {
      LockHelper lockHelper = (LockHelper) null;
      IEnumerable<HostProperties> hostPropertieses = (IEnumerable<HostProperties>) null;
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      try
      {
        VssPerformanceEventSource.Log.StopHostTaskStart(hostProperties.Id);
        if (LockHelper.TryGetLock(this.m_lockManager, requestId, lockNames, LockManager.LockType.HostLockExclusive, TimeSpan.Zero, out lockHelper))
        {
          Host host = default (Host);
          if (hostProperties.HostType != TeamFoundationHostType.ProjectCollection)
          {
            using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
            {
              HostProperties properties;
              if (this.m_serviceHostProperties.TryGetValue(hostProperties.Id, out properties))
              {
                if ((properties.HostType & TeamFoundationHostType.Application) == TeamFoundationHostType.Application)
                  hostPropertieses = (IEnumerable<HostProperties>) this.m_serviceHostProperties.GetServiceHostChildrenProperties(hostProperties.Id);
              }
            }
            if (hostPropertieses != null)
            {
              foreach (HostProperties properties in hostPropertieses)
              {
                if (this.m_hostTable.TryGetHost(properties, requestId, out Host _))
                {
                  this.TraceRaw(58123, TraceLevel.Info, hostProperties.Id, "Host {0} has children loaded", (object) hostProperties.Id);
                  return false;
                }
              }
            }
          }
          if (this.m_hostTable.TryGetHost(hostProperties, requestId, out host) && host.LastUse <= DateTime.UtcNow.Add(-timeToSleep))
          {
            this.TraceRaw(58124, TraceLevel.Info, host.InstanceId, "Host {0} is now going dormant", (object) host.InstanceId);
            if (this.StopHost(host, true))
            {
              this.m_hostTable.RemoveHost(hostProperties, requestId);
              return true;
            }
            this.TraceRaw(58129, TraceLevel.Warning, hostProperties.Id, "Host {0} could not be stopped", (object) hostProperties.Id);
            return false;
          }
          if ((object) host != null)
            this.TraceRaw(58125, TraceLevel.Info, host.InstanceId, "Host {0} was accessed or deleted since we checked on it", (object) host.InstanceId);
          return false;
        }
        this.TraceRaw(58126, TraceLevel.Info, hostProperties.Id, "Host {0} is currently being used", (object) hostProperties.Id);
        return false;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(58127, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, ex);
        return false;
      }
      finally
      {
        stopwatch.Stop();
        lockHelper?.Dispose();
        VssPerformanceEventSource.Log.StopHostTaskStop(hostProperties.Id, stopwatch.ElapsedMilliseconds);
        this.TraceRaw(58128, TraceLevel.Info, hostProperties.Id, "Thread {0} is done with host {1}", (object) Environment.CurrentManagedThreadId, (object) hostProperties.Name);
      }
    }

    private bool AddServiceHost(long requestId, HostProperties properties)
    {
      this.TraceRaw(58042, TraceLevel.Info, properties.Id, "Verifying Properties for Host {0}", (object) properties.Id);
      if (this.m_serviceHostProperties.ContainsKey(properties.Id))
      {
        this.TraceRaw(58152, TraceLevel.Warning, properties.Id, "Processing {0} as an update since we already have it in our map", (object) properties.Id);
        return false;
      }
      if (!this.m_serviceHostProperties.ContainsKey(properties.ParentId))
        return false;
      this.TraceRaw(58044, TraceLevel.Info, properties.Id, "Adding Host {0} with Properties {1} to our map", (object) properties.Id, (object) properties);
      this.m_serviceHostProperties.Add(new HostProperties(properties));
      return true;
    }

    public void OnServiceHostCreated(HostProperties properties, long requestId)
    {
      bool flag = false;
      TeamFoundationTracingService.TraceEnterRaw(58150, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, nameof (OnServiceHostCreated), (object) properties);
      try
      {
        this.ValidateKernelState();
        using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockExclusive, requestId))
          flag = !this.AddServiceHost(requestId, properties);
        if (!flag)
          return;
        this.OnServiceHostModified(properties, requestId);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(58156, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(58158, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, nameof (OnServiceHostCreated));
      }
    }

    public void OnServiceHostDeleted(HostProperties properties, long requestId)
    {
      Guid id = properties.Id;
      TeamFoundationTracingService.TraceEnterRaw(58170, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, nameof (OnServiceHostDeleted), (object) properties);
      try
      {
        this.ValidateKernelState();
        if (id == this.m_rootHost.InstanceId)
        {
          this.TraceRaw(58171, TraceLevel.Error, id, "Deployment host cannot be deleted!");
        }
        else
        {
          List<HostProperties> hostPropertiesList = (List<HostProperties>) null;
          KeyValuePair<LockName<short, Guid, short>, bool>[] lockNames = (KeyValuePair<LockName<short, Guid, short>, bool>[]) null;
          Host host = default (Host);
          HostProperties properties1;
          using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockExclusive, requestId))
          {
            if (!this.m_serviceHostProperties.TryGetValue(id, out properties1))
            {
              this.TraceRaw(58172, TraceLevel.Warning, id, "Host {0} is unknown - ignoring delete", (object) id);
              return;
            }
            lockNames = this.GetLockNamesForHostId(id, requestId);
          }
          using (LockHelper.Lock(this.m_lockManager, requestId, lockNames, LockManager.LockType.HostLockExclusive))
          {
            using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
            {
              hostPropertiesList = this.m_serviceHostProperties.GetServiceHostChildrenProperties(id);
              hostPropertiesList.Add(properties1);
            }
            foreach (HostProperties hostProperties in hostPropertiesList)
            {
              if (this.m_hostTable.TryGetHost(hostProperties, requestId, out host))
              {
                this.m_hostTable.RemoveHost(hostProperties, requestId);
                if (!this.StopHost(host, true))
                {
                  TeamFoundationTracingService.TraceRaw(58177, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Disposing Host {0} {1}", (object) properties1.Name, (object) id);
                  host.Dispose();
                }
              }
              else
                TeamFoundationTracingService.TraceRaw(58176, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "{0} {1} is not in memory", (object) properties1.Name, (object) id);
            }
            using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockExclusive, requestId))
            {
              foreach (HostProperties hostProperties in hostPropertiesList)
                this.m_serviceHostProperties.Remove(hostProperties.Id);
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(58178, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(58180, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, nameof (OnServiceHostDeleted));
      }
    }

    private void CancelAllRequestsRecursive(Guid instanceId, long requestId)
    {
      IEnumerable<HostProperties> hostPropertieses = (IEnumerable<HostProperties>) null;
      HostProperties properties;
      using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
      {
        if (!this.m_serviceHostProperties.TryGetValue(instanceId, out properties))
        {
          this.TraceRaw(58192, TraceLevel.Error, instanceId, "Unknown Host {0}", (object) instanceId);
          return;
        }
        if (instanceId != this.m_rootHost.InstanceId)
          hostPropertieses = (IEnumerable<HostProperties>) this.m_serviceHostProperties.GetServiceHostChildrenProperties(properties.Id);
      }
      if (instanceId == this.m_rootHost.InstanceId)
        hostPropertieses = (IEnumerable<HostProperties>) this.m_hostTable.GetAllHostProperties(requestId);
      if (hostPropertieses != null)
      {
        foreach (HostProperties hostProperties in hostPropertieses)
        {
          if (hostProperties.Id != this.m_rootHost.InstanceId)
            this.CancelAllRequestsRecursive(hostProperties.Id, requestId);
        }
      }
      Host host;
      if (!this.m_hostTable.TryGetHost(properties, requestId, out host))
        return;
      host.CancelAllRequests(TimeSpan.Zero);
    }

    public bool OnServiceHostStatusChanged(
      HostProperties incomingProperties,
      long requestId,
      TimeSpan timeout)
    {
      KeyValuePair<LockName<short, Guid, short>, bool>[] lockNames = (KeyValuePair<LockName<short, Guid, short>, bool>[]) null;
      Host host1 = default (Host);
      bool flag1 = incomingProperties.Id == this.m_rootHost.InstanceId;
      HostProperties properties1 = (HostProperties) null;
      TimeSpan timeSpan = TimeSpan.FromSeconds(3.0);
      TimeSpan timeout1 = timeout > timeSpan ? timeSpan : timeout;
      LockHelper lockHelper1 = (LockHelper) null;
      Task task = (Task) null;
      CancellationTokenSource cancellationToken = (CancellationTokenSource) null;
      LockHelper lockHelper2 = (LockHelper) null;
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      TeamFoundationTracingService.TraceEnterRaw(58190, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "OnServiceHostStatusChanged {0}", (object) incomingProperties);
      try
      {
        this.ValidateKernelState();
        using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockExclusive, requestId))
        {
          if (!this.m_serviceHostProperties.TryGetValue(incomingProperties.Id, out properties1))
          {
            this.TraceRaw(58192, TraceLevel.Warning, incomingProperties.Id, "Host {0} not loaded in map - ignoring status change", (object) incomingProperties.Id);
            return true;
          }
          bool flag2 = incomingProperties.Status != properties1.Status || incomingProperties.SubStatus != ServiceHostSubStatus.Unchanged && incomingProperties.SubStatus != properties1.SubStatus || !string.Equals(incomingProperties.StatusReason, properties1.StatusReason, StringComparison.OrdinalIgnoreCase);
          this.TraceRaw(58194, TraceLevel.Info, properties1.Id, "Updating Host Status from {0} to {1}", (object) this.m_serviceHostProperties[properties1.Id].Status, (object) incomingProperties.Status);
          if (incomingProperties.Status != TeamFoundationServiceHostStatus.Started)
          {
            this.m_serviceHostProperties[properties1.Id].Status = incomingProperties.Status;
            this.m_serviceHostProperties[properties1.Id].StatusReason = incomingProperties.StatusReason;
            this.m_serviceHostProperties[properties1.Id].SubStatus = incomingProperties.SubStatus;
          }
          if (!flag2)
          {
            this.TraceRaw(58193, TraceLevel.Warning, incomingProperties.Id, "Host Status for {0} has not changed ({1}) - bailing out", (object) incomingProperties.Id, (object) incomingProperties.Status);
            return true;
          }
          lockNames = !(properties1.Id == this.m_rootHost.InstanceId) ? this.GetLockNamesForHostId(properties1.Id, requestId) : this.GetLockNamesForDeploymentHost(requestId);
        }
        lockHelper1 = LockHelper.Lock(this.m_lockManager, requestId, lockNames, LockManager.LockType.HostLockShared);
        cancellationToken = new CancellationTokenSource();
        if (incomingProperties.Status == TeamFoundationServiceHostStatus.Stopped || incomingProperties.Status == TeamFoundationServiceHostStatus.Stopping)
          task = Task.Run((Action) (() =>
          {
            while (!cancellationToken.IsCancellationRequested)
            {
              this.CancelAllRequestsRecursive(incomingProperties.Id, (long) -Environment.CurrentManagedThreadId);
              Thread.Sleep(20);
            }
          }));
        while (!LockHelper.TryGetLock(this.m_lockManager, requestId, lockNames, LockManager.LockType.HostLockExclusive, timeout1, out lockHelper2))
        {
          if (stopwatch.Elapsed > timeout)
          {
            this.TraceRaw(58204, TraceLevel.Error, properties1.Id, "Failed to get the lock on host {0} within {1}", (object) properties1.Name, (object) timeout);
            return false;
          }
        }
        if (incomingProperties.Status == TeamFoundationServiceHostStatus.Started)
        {
          using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockExclusive, requestId))
          {
            this.m_serviceHostProperties[properties1.Id].Status = incomingProperties.Status;
            this.m_serviceHostProperties[properties1.Id].StatusReason = incomingProperties.StatusReason;
            this.m_serviceHostProperties[properties1.Id].SubStatus = incomingProperties.SubStatus;
          }
        }
        if (!this.m_hostTable.TryGetHost(properties1, requestId, out host1))
        {
          this.TraceRaw(58196, TraceLevel.Info, properties1.Id, "Host {0} is not loaded", (object) properties1.Id);
          return true;
        }
        Request request1 = this.CreateRequest(properties1.Id, requestId, RequestContextType.SystemContext, HostRequestType.Default, false, false, true);
        try
        {
          host1.UpdateHostProperties(request1, incomingProperties);
        }
        finally
        {
          if ((object) request1 != null)
            request1.Dispose();
        }
        switch (incomingProperties.Status)
        {
          case TeamFoundationServiceHostStatus.Started:
            this.TraceRaw(58198, TraceLevel.Info, host1.InstanceId, "Starting Host {0}", (object) host1.InstanceId);
            Request request2 = this.CreateRequest(properties1.Id, requestId, RequestContextType.SystemContext, HostRequestType.Default, false, false, true);
            try
            {
              this.Start(host1, request2, incomingProperties);
            }
            finally
            {
              if ((object) request2 != null)
                request2.Dispose();
            }
            List<HostProperties> hostPropertiesList1;
            if (flag1)
            {
              hostPropertiesList1 = this.m_hostTable.GetAllHostProperties(requestId);
              using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
              {
                hostPropertiesList1.Sort(new Comparison<HostProperties>(this.CompareHostPropertiesRootFirst));
                hostPropertiesList1.Remove(this.m_rootHost.ServiceHostProperties);
              }
            }
            else
            {
              using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
                hostPropertiesList1 = new List<HostProperties>((IEnumerable<HostProperties>) this.m_serviceHostProperties.GetServiceHostChildrenProperties(properties1.Id));
            }
            if (hostPropertiesList1 != null)
            {
              using (List<HostProperties>.Enumerator enumerator = hostPropertiesList1.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  HostProperties current = enumerator.Current;
                  using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
                  {
                    if (!this.IsStarted(current.Id, requestId))
                    {
                      this.TraceRaw(58197, TraceLevel.Info, current.Id, "Host {0} is not started", (object) current.Id);
                      continue;
                    }
                  }
                  Host host2;
                  if (this.m_hostTable.TryGetHost(current, requestId, out host2))
                  {
                    Request request3 = this.CreateRequest(current.Id, requestId, RequestContextType.SystemContext, HostRequestType.Default, false, false, true);
                    try
                    {
                      this.TraceRaw(58199, TraceLevel.Info, host1.InstanceId, "Starting Host {0}", (object) host1.InstanceId);
                      this.Start(host2, request3, host2.ServiceHostProperties);
                    }
                    finally
                    {
                      if ((object) request3 != null)
                        request3.Dispose();
                    }
                  }
                }
                break;
              }
            }
            else
              break;
          case TeamFoundationServiceHostStatus.Stopping:
          case TeamFoundationServiceHostStatus.Stopped:
            this.TraceRaw(58200, TraceLevel.Info, host1.InstanceId, "Stopping Host {0}", (object) host1.InstanceId);
            List<HostProperties> hostPropertiesList2;
            if (flag1)
            {
              hostPropertiesList2 = this.m_hostTable.GetAllHostProperties(requestId);
              using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
              {
                hostPropertiesList2.Sort(new Comparison<HostProperties>(this.CompareHostPropertiesCollectionFirst));
                hostPropertiesList2.Remove(this.m_rootHost.ServiceHostProperties);
              }
            }
            else
            {
              using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
                hostPropertiesList2 = new List<HostProperties>((IEnumerable<HostProperties>) this.m_serviceHostProperties.GetServiceHostChildrenProperties(properties1.Id));
            }
            if (hostPropertiesList2 != null)
            {
              foreach (HostProperties properties2 in hostPropertiesList2)
              {
                Host host3;
                if (this.m_hostTable.TryGetHost(properties2, requestId, out host3))
                {
                  this.TraceRaw(58202, TraceLevel.Info, host1.InstanceId, "Stopping Host {0}", (object) host1.InstanceId);
                  this.StopHost(host3, false);
                }
              }
            }
            this.StopHost(host1, false);
            break;
        }
        return true;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(58206, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, ex);
        throw;
      }
      finally
      {
        if (cancellationToken != null)
        {
          cancellationToken.Cancel();
          task?.Wait();
          cancellationToken.Dispose();
          cancellationToken = (CancellationTokenSource) null;
        }
        lockHelper2?.Dispose();
        lockHelper1?.Dispose();
        TeamFoundationTracingService.TraceLeaveRaw(58208, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, nameof (OnServiceHostStatusChanged));
      }
    }

    public bool OnServiceHostModified(HostProperties properties, long requestId)
    {
      bool flag1 = false;
      bool flag2 = false;
      KeyValuePair<LockName<short, Guid, short>, bool>[] lockNames = (KeyValuePair<LockName<short, Guid, short>, bool>[]) null;
      LockHelper lockHelper = (LockHelper) null;
      Host host1 = default (Host);
      List<HostProperties> hostPropertiesList = (List<HostProperties>) null;
      TeamFoundationTracingService.TraceEnterRaw(58220, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "OnServiceHostModified {0}", (object) properties);
      try
      {
        this.ValidateKernelState();
        HostProperties properties1;
        using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
        {
          if (this.m_serviceHostProperties.TryGetValue(properties.Id, out properties1))
          {
            if (!flag2 && !string.IsNullOrEmpty(properties.Name) && string.CompareOrdinal(properties1.Name, properties.Name) != 0)
              this.TraceRaw(58223, TraceLevel.Info, properties1.Id, "Host {0} was renamed from {1} to {2}", (object) properties1.Id, (object) properties1.Name, (object) properties.Name);
            if (!flag2 && properties.DatabaseId != 0 && properties.DatabaseId != properties1.DatabaseId)
            {
              flag2 = true;
              this.TraceRaw(58269, TraceLevel.Info, properties1.Id, "Host {0} needs to be flushed out because it was moved from database {1} to {2}", (object) properties1.Id, (object) properties1.DatabaseId, (object) properties.DatabaseId);
            }
            if (properties1.ParentId != properties.ParentId && properties.ParentId != Guid.Empty)
            {
              flag2 = true;
              if (!this.m_serviceHostProperties.ContainsKey(properties.ParentId))
                flag1 = true;
              this.TraceRaw(58222, TraceLevel.Info, properties1.Id, "Host {0} needs to be flushed out and reloaded because its parent changed", (object) properties1.Name);
            }
            if (properties.Id == this.m_rootHost.InstanceId)
            {
              flag2 = false;
              this.TraceRaw(58229, TraceLevel.Info, properties1.Id, "Host {0} cannot be flushed because it is the deployment host", (object) properties1.Name);
            }
            else
              lockNames = this.GetLockNamesForHostId(properties.Id, requestId);
          }
        }
        if (flag1)
        {
          this.RefreshHostByGuid(properties.ParentId, requestId);
          using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
          {
            if (!this.m_serviceHostProperties.ContainsKey(properties.ParentId))
              throw new HostDoesNotExistException(properties.ParentId);
          }
        }
        if (lockNames != null)
          lockHelper = LockHelper.Lock(this.m_lockManager, requestId, lockNames, flag2 ? LockManager.LockType.HostLockExclusive : LockManager.LockType.HostLockShared);
        if (flag2)
        {
          using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
          {
            hostPropertiesList = new List<HostProperties>();
            List<HostProperties> childrenProperties = this.m_serviceHostProperties.GetServiceHostChildrenProperties(properties.Id);
            if (childrenProperties != null)
              hostPropertiesList.AddRange((IEnumerable<HostProperties>) childrenProperties);
            hostPropertiesList.Add(properties1);
          }
          foreach (HostProperties hostProperties in hostPropertiesList)
          {
            Host host2;
            if (this.m_hostTable.TryGetHost(hostProperties, requestId, out host2))
            {
              this.m_hostTable.RemoveHost(hostProperties, requestId);
              this.TraceRaw(58224, TraceLevel.Info, hostProperties.Id, "Removing host {0} from the host table", (object) hostProperties.Id);
              if (!this.StopHost(host2, true))
              {
                this.TraceRaw(58225, TraceLevel.Info, hostProperties.Id, "Disposing host {0} {1}", (object) hostProperties.Name, (object) hostProperties.Id);
                host1.Dispose();
              }
            }
          }
        }
        using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockExclusive, requestId))
        {
          this.TraceRaw(58226, TraceLevel.Info, properties.Id, "Updating Properties for host {0}", (object) properties.Id);
          if (!this.m_serviceHostProperties.TryGetValue(properties.Id, out properties1))
          {
            this.TraceRaw(58227, TraceLevel.Warning, properties.Id, "Ignoring host modification for host {0} because we haven't loaded it yet", (object) properties.Id);
            return false;
          }
          properties1 = new HostProperties(properties1);
          this.TraceRaw(58230, TraceLevel.Info, properties.Id, "Updating Properties from {0} to {1}", (object) this.m_serviceHostProperties[properties.Id], (object) properties);
          this.m_serviceHostProperties.Remove(properties1.Id);
          properties1.UpdateProperties(properties);
          this.m_serviceHostProperties.Add(properties1);
        }
        if (properties1 != null && this.m_hostTable.TryGetHost(properties1, requestId, out host1))
        {
          TeamFoundationTracingService.TraceRaw(58234, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Updating Properties for Host object {0}", (object) properties.Id);
          Request request = this.CreateRequest(properties.Id, requestId, RequestContextType.SystemContext, HostRequestType.Default, false, false, true);
          try
          {
            host1.UpdateHostProperties(request, properties);
          }
          finally
          {
            if ((object) request != null)
              request.Dispose();
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(58236, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, ex);
        throw;
      }
      finally
      {
        lockHelper?.Dispose();
        TeamFoundationTracingService.TraceLeaveRaw(58238, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "OnServiceHostStatusChanged");
      }
    }

    internal void OnServiceHostPropertiesChanged(
      Action<HostProperties> hostPropertiesUpdater,
      long requestId)
    {
      using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
        this.m_serviceHostProperties.UpdateProperties(hostPropertiesUpdater);
      foreach (HostProperties allHostProperty in this.m_hostTable.GetAllHostProperties(requestId))
      {
        Request request = this.CreateRequest(allHostProperty.Id, requestId, RequestContextType.SystemContext, HostRequestType.Default, false, false, false);
        try
        {
          if ((object) request != null)
          {
            Host host;
            if (this.m_hostTable.TryGetHost(allHostProperty, requestId, out host))
              hostPropertiesUpdater(host.ServiceHostProperties);
          }
        }
        finally
        {
          if ((object) request != null)
            request.Dispose();
        }
      }
    }

    public void AssertNoLocksHeld(long requestId, string message) => this.m_lockManager.AssertNoLocksHeld(requestId, message);

    internal void EnsureNoLocksHeld(long requestId, string message, bool assert) => this.m_lockManager.EnsureNoLocksHeld(requestId, message, assert);

    public string DumpMap() => this.m_serviceHostProperties.DumpHostPropertiesTable();

    internal void RunOnEachHost(
      HostManagementKernel<Host, Request>.HostCallback hostCallback,
      object state,
      IEnumerable<Guid> hostIds,
      long requestId)
    {
      List<Guid> allHostIds = this.m_hostTable.GetAllHostIds(requestId);
      foreach (Guid hostId in hostIds == null ? (IEnumerable<Guid>) allHostIds : allHostIds.Intersect<Guid>(hostIds))
      {
        try
        {
          Request request = this.CreateRequest(hostId, requestId, RequestContextType.UserContext, HostRequestType.Default, false, true, false);
          try
          {
            if ((object) request != null)
              hostCallback(request, state);
          }
          finally
          {
            if ((object) request != null)
              request.Dispose();
          }
        }
        catch (Exception ex)
        {
          this.TraceRaw(58240, TraceLevel.Error, hostId, "{0}", (object) ex);
        }
      }
    }

    private void Start(Host host, Request request, HostProperties properties)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      try
      {
        host.Start(request);
      }
      finally
      {
        stopwatch.Stop();
        TraceLevel level = TraceLevel.Info;
        if (stopwatch.ElapsedMilliseconds > 1000L)
          level = stopwatch.ElapsedMilliseconds > 3000L ? TraceLevel.Error : TraceLevel.Warning;
        this.TraceRaw(58241, level, properties.Id, "Host '{0}' ({1}) took {2} ms to start", (object) properties.Name, (object) properties.Id, (object) stopwatch.ElapsedMilliseconds);
      }
    }

    private bool StopHost(Host host, bool dispose)
    {
      Guid instanceId = host.InstanceId;
      bool flag = false;
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      TeamFoundationTracingService.TraceEnterRaw(58250, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "HostManagementKernel::StopHost", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      while (!this.m_isShuttingDown)
      {
        try
        {
          Request request = host.CreateRequest(RequestContextType.SystemContext, (LockHelper) null, HostRequestType.Default);
          try
          {
            if (host.ServiceHostProperties.Status != TeamFoundationServiceHostStatus.Stopped)
            {
              this.TraceRaw(58252, TraceLevel.Info, instanceId, "Host {0} is stopping", (object) instanceId);
              host.Stop(request, false);
              this.TraceRaw(58254, TraceLevel.Info, instanceId, "Host {0} is stopped", (object) instanceId);
            }
            else if (dispose)
            {
              this.TraceRaw(58256, TraceLevel.Info, instanceId, "Cleaning stopped Host {0} for dispose.", (object) instanceId);
              host.Stop(request, false);
              this.TraceRaw(58258, TraceLevel.Info, instanceId, "Host {0} is stopped", (object) instanceId);
            }
          }
          finally
          {
            if ((object) request != null)
              request.Dispose();
          }
          if (dispose)
          {
            this.TraceRaw(58260, TraceLevel.Info, instanceId, "Host {0} is getting disposed", (object) instanceId);
            host.Dispose();
            flag = true;
            this.TraceRaw(58262, TraceLevel.Info, instanceId, "Host {0} is now disposed", (object) instanceId);
          }
          return flag;
        }
        catch (DatabaseConnectionException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(58264, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, (Exception) ex);
          Thread.Sleep(500);
        }
        catch (DatabaseOperationTimeoutException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(58265, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, (Exception) ex);
          Thread.Sleep(500);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(58266, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, ex);
          TeamFoundationEventLog.Default.LogException(FrameworkResources.FailedToStopHost(), ex);
          throw;
        }
        finally
        {
          stopwatch.Stop();
          this.TraceRaw(58268, TraceLevel.Warning, instanceId, "Stopping Host {0} took {1} ms", (object) instanceId, (object) stopwatch.ElapsedMilliseconds);
          stopwatch.Restart();
        }
      }
      return false;
    }

    private void ValidateKernelState()
    {
      this.m_started.WaitOne();
      if (!this.m_isHealthy)
        throw new InvalidOperationException();
    }

    private void Teardown(long requestId)
    {
      TeamFoundationTracingService.TraceRaw(58322, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Taking Host Table lock (S) to determine which hosts need to be shutdown");
      List<HostProperties> allHostProperties = this.m_hostTable.GetAllHostProperties(requestId);
      TeamFoundationTracingService.TraceRaw(58324, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Taking Map lock (S) to determine in which order the hosts will be stopped");
      using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockShared, requestId))
        allHostProperties.Sort(new Comparison<HostProperties>(this.CompareHostPropertiesCollectionFirst));
      Request request1 = this.CreateRequest(this.m_rootHost.InstanceId, requestId, RequestContextType.SystemContext, HostRequestType.Default, false, false, true);
      try
      {
        TeamFoundationTracingService.TraceRaw(58328, TraceLevel.Info, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "Taking Host Table Lock (S) to stop all non root hosts");
        for (int index = 0; index < allHostProperties.Count; ++index)
        {
          Host host;
          this.m_hostTable.TryGetHost(allHostProperties[index], requestId, out host);
          Request request2 = host.CreateRequest(RequestContextType.SystemContext, (LockHelper) null, HostRequestType.Default);
          try
          {
            this.TraceRaw(58330, TraceLevel.Info, host.InstanceId, "Shutting down Host");
            try
            {
              host.Stop(request2, true);
              this.TraceRaw(58332, TraceLevel.Info, host.InstanceId, "Host successfully stopped");
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(58334, TraceLevel.Error, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, ex);
            }
          }
          finally
          {
            if ((object) request2 != null)
              request2.Dispose();
          }
          if (index != allHostProperties.Count - 1)
          {
            host.Dispose();
            this.m_hostTable.RemoveHost(allHostProperties[index], requestId);
          }
        }
        using (this.m_lockManager.Lock(this.m_mapLock, LockManager.LockType.MapLockExclusive, requestId))
        {
          HostProperties serviceHostProperty = this.m_serviceHostProperties[this.m_rootHost.InstanceId];
          this.m_serviceHostProperties.Clear();
          this.m_serviceHostProperties.Add(serviceHostProperty);
        }
      }
      finally
      {
        if ((object) request1 != null)
          request1.Dispose();
      }
      TeamFoundationTracingService.TraceLeaveRaw(58340, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, "HostManagementKernel::Teardown");
    }

    private void RefreshHostByGuid(Guid hostId, long requestId) => this.RefreshHost((Func<List<HostProperties>>) (() => this.m_resolveHost(hostId)), (Func<bool>) null, hostId.ToString(), requestId);

    private void TraceRaw(int tracepoint, TraceLevel level, Guid hostId, string message) => this.TraceRaw(tracepoint, level, hostId, message, (object[]) null);

    private void TraceRaw(
      int tracepoint,
      TraceLevel level,
      Guid hostId,
      string message,
      params object[] args)
    {
      TraceEvent trace = new TraceEvent(message, args);
      TeamFoundationTracingService.GetTraceEvent(ref trace, tracepoint, level, HostManagementKernel<Host, Request>.s_Area, HostManagementKernel<Host, Request>.s_Layer, (string[]) null, (string) null);
      trace.ServiceHost = hostId;
      TeamFoundationTracingService.TraceRaw(ref trace);
    }

    private Guid GetParentId(Guid hostId, long requestId)
    {
      HostProperties properties;
      if (this.m_serviceHostProperties.TryGetValue(hostId, out properties))
        return properties.ParentId;
      throw new HostDoesNotExistException(hostId);
    }

    private int CompareHostPropertiesRootFirst(HostProperties p1, HostProperties p2) => p1.HostTypeValue == p2.HostTypeValue ? Comparer<Guid>.Default.Compare(p1.Id, p2.Id) : ((int) TeamFoundationHostTypeHelper.NormalizeHostType(p1.HostType)).CompareTo((int) TeamFoundationHostTypeHelper.NormalizeHostType(p2.HostType));

    private int CompareHostPropertiesCollectionFirst(HostProperties p1, HostProperties p2)
    {
      if (p1.HostTypeValue == p2.HostTypeValue)
        return Comparer<Guid>.Default.Compare(p1.Id, p2.Id);
      int num = (int) TeamFoundationHostTypeHelper.NormalizeHostType(p1.HostType);
      return ((int) TeamFoundationHostTypeHelper.NormalizeHostType(p2.HostType)).CompareTo(num);
    }

    private bool CheckLockNamesForHostId(
      Guid hostId,
      KeyValuePair<LockName<short, Guid, short>, bool>[] existingLockNames,
      long requestId)
    {
      KeyValuePair<LockName<short, Guid, short>, bool>[] lockNamesForHostId = this.GetLockNamesForHostId(hostId, requestId);
      if (lockNamesForHostId.Length != existingLockNames.Length)
        return false;
      for (int index = 0; index < lockNamesForHostId.Length; ++index)
      {
        if ((int) lockNamesForHostId[index].Key.NameValue1 != (int) existingLockNames[index].Key.NameValue1 || lockNamesForHostId[index].Key.NameValue2 != existingLockNames[index].Key.NameValue2 || lockNamesForHostId[index].Value != existingLockNames[index].Value)
          return false;
      }
      return true;
    }

    private KeyValuePair<LockName<short, Guid, short>, bool>[] GetLockNamesForDeploymentHost(
      long requestId)
    {
      KeyValuePair<LockName<short, Guid, short>, bool>[] forDeploymentHost = new KeyValuePair<LockName<short, Guid, short>, bool>[this.c_hashBuckets];
      for (int index = 0; index < this.c_hashBuckets; ++index)
        forDeploymentHost[index] = new KeyValuePair<LockName<short, Guid, short>, bool>(this.m_deploymentHostLock[index], true);
      return forDeploymentHost;
    }

    private KeyValuePair<LockName<short, Guid, short>, bool>[] GetLockNamesForHostId(
      Guid hostId,
      long requestId)
    {
      Guid[] guidArray = new Guid[3];
      Guid hostId1 = hostId;
      short length = 0;
      do
      {
        guidArray[(int) length++] = hostId1;
        hostId1 = this.GetParentId(hostId1, requestId);
      }
      while (hostId1 != Guid.Empty);
      KeyValuePair<LockName<short, Guid, short>, bool>[] lockNamesForHostId = new KeyValuePair<LockName<short, Guid, short>, bool>[(int) length];
      for (short index = 0; (int) index < (int) length; ++index)
      {
        TeamFoundationHostType nameValue1 = TeamFoundationHostTypeHelper.NormalizeHostType(this.m_serviceHostProperties[guidArray[(int) length - 1 - (int) index]].HostType);
        lockNamesForHostId[(int) index] = nameValue1 != TeamFoundationHostType.Deployment ? new KeyValuePair<LockName<short, Guid, short>, bool>(new LockName<short, Guid, short>((short) nameValue1, guidArray[(int) length - 1 - (int) index], (short) 0, LockLevel.Host), (int) index == (int) length - 1) : new KeyValuePair<LockName<short, Guid, short>, bool>(this.GetDeploymentHostLock(requestId), length == (short) 1);
      }
      return lockNamesForHostId;
    }

    public delegate Host LoadHost(HostProperties properties, Host parentHost)
      where Host : ServiceHost<Request>
      where Request : class, IVssRequestContext;

    public delegate void HostCallback(Request hostCallback, object state)
      where Host : ServiceHost<Request>
      where Request : class, IVssRequestContext;
  }
}
