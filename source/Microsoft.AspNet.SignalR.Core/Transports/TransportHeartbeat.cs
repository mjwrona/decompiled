// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.TransportHeartbeat
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Tracing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Transports
{
  public class TransportHeartbeat : ITransportHeartbeat, IDisposable
  {
    private readonly ConcurrentDictionary<string, TransportHeartbeat.ConnectionMetadata> _connections = new ConcurrentDictionary<string, TransportHeartbeat.ConnectionMetadata>();
    private readonly Timer _timer;
    private readonly IConfigurationManager _configurationManager;
    private readonly TraceSource _trace;
    private readonly IPerformanceCounterManager _counters;
    private readonly object _counterLock = new object();
    private int _running;
    private ulong _heartbeatCount;

    public TransportHeartbeat(IDependencyResolver resolver)
    {
      this._configurationManager = resolver.Resolve<IConfigurationManager>();
      this._counters = resolver.Resolve<IPerformanceCounterManager>();
      this._trace = resolver.Resolve<ITraceManager>()["SignalR.Transports.TransportHeartBeat"];
      this._timer = new Timer(new TimerCallback(this.Beat), (object) null, this._configurationManager.HeartbeatInterval(), this._configurationManager.HeartbeatInterval());
    }

    private TraceSource Trace => this._trace;

    public ITrackingConnection AddOrUpdateConnection(ITrackingConnection connection)
    {
      TransportHeartbeat.ConnectionMetadata newMetadata = connection != null ? new TransportHeartbeat.ConnectionMetadata(connection) : throw new ArgumentNullException(nameof (connection));
      bool isNewConnection = true;
      ITrackingConnection oldConnection = (ITrackingConnection) null;
      this._connections.AddOrUpdate(connection.ConnectionId, newMetadata, (Func<string, TransportHeartbeat.ConnectionMetadata, TransportHeartbeat.ConnectionMetadata>) ((key, old) =>
      {
        this.Trace.TraceEvent(TraceEventType.Verbose, 0, "Connection {0} exists. Closing previous connection.", (object) old.Connection.ConnectionId);
        old.Connection.ApplyState(TransportConnectionStates.Replaced);
        old.Connection.End();
        isNewConnection = false;
        oldConnection = old.Connection;
        old.Connection.DecrementConnectionsCount();
        newMetadata.Connection.IncrementConnectionsCount();
        return newMetadata;
      }));
      if (isNewConnection)
      {
        this.Trace.TraceInformation("Connection {0} is New.", (object) connection.ConnectionId);
        connection.IncrementConnectionsCount();
      }
      lock (this._counterLock)
        this._counters.ConnectionsCurrent.RawValue = (long) this._connections.Count;
      newMetadata.Initial = DateTime.UtcNow;
      newMetadata.Connection.ApplyState(TransportConnectionStates.Added);
      return oldConnection;
    }

    public void RemoveConnection(ITrackingConnection connection)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      if (!this._connections.TryRemove(connection.ConnectionId, out TransportHeartbeat.ConnectionMetadata _))
        return;
      connection.DecrementConnectionsCount();
      lock (this._counterLock)
        this._counters.ConnectionsCurrent.RawValue = (long) this._connections.Count;
      connection.ApplyState(TransportConnectionStates.Removed);
      this.Trace.TraceInformation("Removing connection {0}", (object) connection.ConnectionId);
    }

    public void MarkConnection(ITrackingConnection connection)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      TransportHeartbeat.ConnectionMetadata connectionMetadata;
      if (!connection.IsAlive || !this._connections.TryGetValue(connection.ConnectionId, out connectionMetadata))
        return;
      connectionMetadata.LastMarked = DateTime.UtcNow;
    }

    public IList<ITrackingConnection> GetConnections() => (IList<ITrackingConnection>) this._connections.Select<KeyValuePair<string, TransportHeartbeat.ConnectionMetadata>, ITrackingConnection>((Func<KeyValuePair<string, TransportHeartbeat.ConnectionMetadata>, ITrackingConnection>) (p => p.Value.Connection)).ToList<ITrackingConnection>();

    private void Beat(object state)
    {
      if (Interlocked.Exchange(ref this._running, 1) == 1)
      {
        this.Trace.TraceEvent(TraceEventType.Verbose, 0, "Timer handler took longer than current interval");
      }
      else
      {
        lock (this._counterLock)
          this._counters.ConnectionsCurrent.RawValue = (long) this._connections.Count;
        try
        {
          ++this._heartbeatCount;
          foreach (TransportHeartbeat.ConnectionMetadata metadata in (IEnumerable<TransportHeartbeat.ConnectionMetadata>) this._connections.Values)
          {
            if (metadata.Connection.IsAlive)
            {
              this.CheckTimeoutAndKeepAlive(metadata);
            }
            else
            {
              this.Trace.TraceEvent(TraceEventType.Verbose, 0, metadata.Connection.ConnectionId + " is dead");
              this.CheckDisconnect(metadata);
            }
          }
        }
        catch (Exception ex)
        {
          this.Trace.TraceEvent(TraceEventType.Error, 0, "SignalR error during transport heart beat on background thread: {0}", (object) ex);
        }
        finally
        {
          Interlocked.Exchange(ref this._running, 0);
        }
      }
    }

    private void CheckTimeoutAndKeepAlive(TransportHeartbeat.ConnectionMetadata metadata)
    {
      if (this.RaiseTimeout(metadata))
      {
        metadata.Connection.Timeout();
      }
      else
      {
        if (this.RaiseKeepAlive(metadata))
        {
          this.Trace.TraceEvent(TraceEventType.Verbose, 0, "KeepAlive(" + metadata.Connection.ConnectionId + ")");
          metadata.Connection.KeepAlive().Catch<Task>((Action<AggregateException, object>) ((ex, state) => TransportHeartbeat.OnKeepAliveError(ex, state)), (object) this.Trace, this.Trace);
        }
        this.MarkConnection(metadata.Connection);
      }
    }

    private void CheckDisconnect(TransportHeartbeat.ConnectionMetadata metadata)
    {
      try
      {
        if (!this.RaiseDisconnect(metadata))
          return;
        this.RemoveConnection(metadata.Connection);
        metadata.Connection.Disconnect();
      }
      catch (Exception ex)
      {
        this.Trace.TraceEvent(TraceEventType.Error, 0, "Raising Disconnect failed: {0}", (object) ex);
      }
    }

    private bool RaiseDisconnect(TransportHeartbeat.ConnectionMetadata metadata) => DateTime.UtcNow - metadata.LastMarked >= metadata.Connection.DisconnectThreshold + this._configurationManager.DisconnectTimeout;

    private bool RaiseKeepAlive(TransportHeartbeat.ConnectionMetadata metadata) => this._configurationManager.KeepAlive.HasValue && metadata.Connection.SupportsKeepAlive && this._heartbeatCount % 2UL == 0UL;

    private bool RaiseTimeout(TransportHeartbeat.ConnectionMetadata metadata) => !metadata.Connection.IsTimedOut && (!this._configurationManager.KeepAlive.HasValue || metadata.Connection.RequiresTimeout) && DateTime.UtcNow - metadata.Initial >= this._configurationManager.ConnectionTimeout;

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this._timer != null)
        this._timer.Dispose();
      this.Trace.TraceInformation("Dispose(). Closing all connections");
      foreach (KeyValuePair<string, TransportHeartbeat.ConnectionMetadata> connection in this._connections)
      {
        TransportHeartbeat.ConnectionMetadata connectionMetadata;
        if (this._connections.TryGetValue(connection.Key, out connectionMetadata))
          connectionMetadata.Connection.End();
      }
    }

    public void Dispose() => this.Dispose(true);

    private static void OnKeepAliveError(AggregateException ex, object state) => ((TraceSource) state).TraceEvent(TraceEventType.Error, 0, "Failed to send keep alive: " + (object) ex.GetBaseException());

    private class ConnectionMetadata
    {
      public ConnectionMetadata(ITrackingConnection connection)
      {
        this.Connection = connection;
        this.Initial = DateTime.UtcNow;
        this.LastMarked = DateTime.UtcNow;
      }

      public ITrackingConnection Connection { get; set; }

      public DateTime LastMarked { get; set; }

      public DateTime Initial { get; set; }
    }
  }
}
