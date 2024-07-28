// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceHost`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal abstract class ServiceHost<T> : IDisposable, IVssServiceHostControl where T : IVssRequestContext
  {
    private long m_requestsProcessed;
    private ManualResetEvent m_requestsDrainedEvent;
    private readonly Dictionary<long, T> m_activeRequests = new Dictionary<long, T>(50);
    private readonly object m_activeRequestsLock = new object();
    private static readonly string s_Area = "HostManagement";
    private static readonly string s_Layer = nameof (ServiceHost<T>);

    public ServiceHost() => this.m_requestsDrainedEvent = new ManualResetEvent(true);

    internal abstract HostProperties ServiceHostProperties { get; }

    internal abstract T CreateRequest(
      RequestContextType requestKind,
      LockHelper helper,
      HostRequestType type,
      params object[] additionalParameters);

    internal abstract void Start(T requestContext);

    internal abstract void Stop(T requestContext, bool isShuttingDown);

    internal abstract void UpdateHostProperties(T requestContext, HostProperties hostProperties);

    internal abstract bool FlushNotificationQueue(T requestContext);

    internal virtual void CancelAllRequests(TimeSpan waitTime)
    {
      this.CheckDisposed();
      if (this.ServiceHostProperties.Status != TeamFoundationServiceHostStatus.Stopped)
        this.ServiceHostProperties.Status = TeamFoundationServiceHostStatus.Stopping;
      lock (this.m_activeRequestsLock)
      {
        if (this.m_activeRequests.Count > 0)
        {
          TeamFoundationTracingService.TraceRawAlwaysOn(480646836, TraceLevel.Info, ServiceHost<T>.s_Area, ServiceHost<T>.s_Layer, string.Format("Cancelling {0} requests on {1}", (object) this.m_activeRequests.Count, (object) this.ServiceHostProperties.Id));
          foreach (T obj1 in this.m_activeRequests.Values)
          {
            if (!obj1.IsCanceled)
            {
              TeamFoundationTracingService.TraceRawAlwaysOn(792702627, TraceLevel.Info, ServiceHost<T>.s_Area, ServiceHost<T>.s_Layer, string.Format("Cancelling request {0} on {1}", (object) obj1.ToString(), (object) this.ServiceHostProperties.Id));
              ref T local = ref obj1;
              T obj2 = default (T);
              if ((object) obj2 == null)
              {
                obj2 = local;
                local = ref obj2;
              }
              string statusReason = this.ServiceHostProperties.StatusReason;
              local.Cancel(statusReason);
            }
          }
        }
      }
      if (!(waitTime > TimeSpan.Zero) || this.m_requestsDrainedEvent.WaitOne(0))
        return;
      TeamFoundationTracingService.TraceRawAlwaysOn(852746874, TraceLevel.Info, ServiceHost<T>.s_Area, ServiceHost<T>.s_Layer, string.Format("Waiting for all requests to drain on {0}", (object) this.ServiceHostProperties.Id));
      if (!this.m_requestsDrainedEvent.WaitOne(waitTime, false))
      {
        TeamFoundationTracingService.TraceRawAlwaysOn(447617829, TraceLevel.Info, ServiceHost<T>.s_Area, ServiceHost<T>.s_Layer, string.Format("Timed out while waiting for active requests to drain for {0}", (object) this.ServiceHostProperties.Id));
        lock (this.m_activeRequestsLock)
        {
          if (this.m_activeRequests.Count <= 0)
            return;
          foreach (T obj in this.m_activeRequests.Values)
            TeamFoundationTracingService.TraceRawAlwaysOn(464933725, TraceLevel.Info, ServiceHost<T>.s_Area, ServiceHost<T>.s_Layer, string.Format("Request {0} on {1} is not going away!", (object) obj.ToString(), (object) this.ServiceHostProperties.Id));
        }
      }
      else
        TeamFoundationTracingService.TraceRawAlwaysOn(954290611, TraceLevel.Info, ServiceHost<T>.s_Area, ServiceHost<T>.s_Layer, string.Format("All requests have drained on {0}", (object) this.ServiceHostProperties.Id));
    }

    public void Dispose()
    {
      if (this.IsDisposed)
        return;
      this.IsDisposing = true;
      this.Dispose(true);
      this.IsDisposed = true;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_requestsDrainedEvent == null)
        return;
      this.m_requestsDrainedEvent.Close();
      this.m_requestsDrainedEvent = (ManualResetEvent) null;
    }

    public void BeginRequest(IVssRequestContext requestContext) => this.BeginRequest((T) requestContext);

    public void EndRequest(IVssRequestContext requestContext) => this.EndRequest((T) requestContext);

    public abstract Guid InstanceId { get; }

    public DateTime LastUse { get; set; }

    public bool IsDisposing { get; private set; }

    public bool IsDisposed { get; private set; }

    public int NumberOfActiveRequests
    {
      get
      {
        lock (this.m_activeRequestsLock)
          return this.m_activeRequests.Count;
      }
    }

    protected virtual void BeginRequest(T requestContext)
    {
      lock (this.m_activeRequestsLock)
      {
        if (!requestContext.IsServicingContext && this.ServiceHostProperties.Status != TeamFoundationServiceHostStatus.Started)
          this.ServiceHostProperties.ThrowShutdownException((IVssRequestContext) requestContext);
        requestContext.Trace(59500, TraceLevel.Info, ServiceHost<T>.s_Area, ServiceHost<T>.s_Layer, "Adding Request {0} to active requests", (object) requestContext.ContextId);
        if (this.m_activeRequests.ContainsKey(requestContext.ContextId))
        {
          requestContext.Trace(59501, TraceLevel.Info, ServiceHost<T>.s_Area, ServiceHost<T>.s_Layer, "Request {0} is already in active requests", (object) requestContext.ContextId);
        }
        else
        {
          this.m_requestsDrainedEvent.Reset();
          this.m_activeRequests.Add(requestContext.ContextId, requestContext);
          ++this.m_requestsProcessed;
          requestContext.Trace(59502, TraceLevel.Info, ServiceHost<T>.s_Area, ServiceHost<T>.s_Layer, "Request {0} is now in active requests", (object) requestContext.ContextId);
        }
      }
    }

    protected virtual void EndRequest(T requestContext)
    {
      lock (this.m_activeRequestsLock)
      {
        requestContext.Trace(59010, TraceLevel.Info, ServiceHost<T>.s_Area, ServiceHost<T>.s_Layer, "Removing Request {0} from active requests", (object) requestContext.ContextId);
        if (!this.m_activeRequests.Remove(requestContext.ContextId) || this.m_activeRequests.Count != 0)
          return;
        requestContext.Trace(59011, TraceLevel.Info, ServiceHost<T>.s_Area, ServiceHost<T>.s_Layer, "There are no more active requests for host {0}", (object) this.ServiceHostProperties.Name);
        this.m_requestsDrainedEvent.Set();
      }
    }

    protected T[] GetActiveRequests()
    {
      lock (this.m_activeRequestsLock)
      {
        T[] array = new T[this.m_activeRequests.Count];
        this.m_activeRequests.Values.CopyTo(array, 0);
        return array;
      }
    }

    protected bool TryGetRequest(long requestId, out T requestContext)
    {
      this.CheckDisposed();
      lock (this.m_activeRequestsLock)
        return this.m_activeRequests.TryGetValue(requestId, out requestContext);
    }
  }
}
