// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.WebSocketTransport
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNet.SignalR.Owin;
using Microsoft.AspNet.SignalR.Tracing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Transports
{
  public class WebSocketTransport : ForeverTransport
  {
    private readonly HostContext _context;
    private IWebSocket _socket;
    private bool _isAlive = true;
    private readonly int? _maxIncomingMessageSize;
    private readonly Action<string> _message;
    private readonly Action _closed;
    private readonly Action<Exception> _error;
    private readonly IPerformanceCounterManager _counters;
    private static readonly byte[] _keepAlive = Encoding.UTF8.GetBytes("{}");

    public WebSocketTransport(HostContext context, IDependencyResolver resolver)
      : this(context, resolver.Resolve<JsonSerializer>(), resolver.Resolve<ITransportHeartbeat>(), resolver.Resolve<IPerformanceCounterManager>(), resolver.Resolve<ITraceManager>(), resolver.Resolve<IMemoryPool>(), resolver.Resolve<IConfigurationManager>().MaxIncomingWebSocketMessageSize)
    {
    }

    public WebSocketTransport(
      HostContext context,
      JsonSerializer serializer,
      ITransportHeartbeat heartbeat,
      IPerformanceCounterManager performanceCounterManager,
      ITraceManager traceManager,
      IMemoryPool pool,
      int? maxIncomingMessageSize)
      : base(context, serializer, heartbeat, performanceCounterManager, traceManager, pool)
    {
      this._context = context;
      this._maxIncomingMessageSize = maxIncomingMessageSize;
      this._message = new Action<string>(this.OnMessage);
      this._closed = new Action(this.OnClosed);
      this._error = new Action<Exception>(this.OnSocketError);
      this._counters = performanceCounterManager;
    }

    public override bool IsAlive => this._isAlive;

    public override CancellationToken CancellationToken => CancellationToken.None;

    public override Task KeepAlive() => this.EnqueueOperation((Func<object, Task>) (state => ((IWebSocket) state).Send(new ArraySegment<byte>(WebSocketTransport._keepAlive))), (object) this._socket);

    public override Task ProcessRequest(ITransportConnection connection)
    {
      if (!this.IsAbortRequest)
        return this.AcceptWebSocketRequest((Func<IWebSocket, Task>) (socket => this.ProcessRequestCore(connection)));
      this.Context.Response.ContentType = "text/plain";
      return connection.Abort(this.ConnectionId);
    }

    public override Task Send(object value) => this.EnqueueOperation((Func<object, Task>) (state => WebSocketTransport.PerformSend(state)), (object) new WebSocketTransport.WebSocketTransportContext(this, value));

    public override Task Send(PersistentResponse response)
    {
      this.OnSendingResponse(response);
      return this.Send((object) response);
    }

    public override void IncrementConnectionsCount() => this._counters.ConnectionsCurrentWebSockets.Increment();

    public override void DecrementConnectionsCount() => this._counters.ConnectionsCurrentWebSockets.Decrement();

    private Task AcceptWebSocketRequest(Func<IWebSocket, Task> callback)
    {
      Action<IDictionary<string, object>, Func<IDictionary<string, object>, Task>> action = this._context.Environment.Get<Action<IDictionary<string, object>, Func<IDictionary<string, object>, Task>>>("websocket.Accept");
      if (action == null)
      {
        this._context.Response.StatusCode = 400;
        return this._context.Response.End(Resources.Error_NotWebSocketRequest);
      }
      Action<IWebSocket> prepareWebsocket = (Action<IWebSocket>) (socket =>
      {
        this._socket = socket;
        socket.OnClose = this._closed;
        socket.OnMessage = this._message;
        socket.OnError = this._error;
      });
      OwinWebSocketHandler webSocketHandler = new OwinWebSocketHandler(callback, prepareWebsocket, this._maxIncomingMessageSize);
      action((IDictionary<string, object>) null, new Func<IDictionary<string, object>, Task>(webSocketHandler.ProcessRequest));
      return TaskAsyncHelper.Empty;
    }

    private static async Task PerformSend(object state)
    {
      WebSocketTransport.WebSocketTransportContext context = (WebSocketTransport.WebSocketTransportContext) state;
      IWebSocket socket = context.Transport._socket;
      using (BinaryMemoryPoolTextWriter writer = new BinaryMemoryPoolTextWriter(context.Transport.Pool))
      {
        try
        {
          context.Transport.JsonSerializer.Serialize(context.State, (TextWriter) writer);
          writer.Flush();
          await socket.Send(writer.Buffer).PreserveCulture();
          context.Transport.TraceOutgoingMessage(writer.Buffer);
        }
        catch (Exception ex)
        {
          context.Transport.OnError(ex);
          throw;
        }
      }
    }

    private void OnMessage(string message)
    {
      if (this.Received == null)
        return;
      this.Received(message).Catch<Task>(this.Trace);
    }

    private void OnClosed()
    {
      this.Trace.TraceInformation("CloseSocket({0})", (object) this.ConnectionId);
      this._isAlive = false;
    }

    private void OnSocketError(Exception error) => this.Trace.TraceError("OnError({0}, {1})", (object) this.ConnectionId, (object) error);

    private class WebSocketTransportContext
    {
      public readonly WebSocketTransport Transport;
      public readonly object State;

      public WebSocketTransportContext(WebSocketTransport transport, object state)
      {
        this.Transport = transport;
        this.State = state;
      }
    }
  }
}
