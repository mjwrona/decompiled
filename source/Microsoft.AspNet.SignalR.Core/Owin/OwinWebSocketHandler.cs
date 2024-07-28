// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Owin.OwinWebSocketHandler
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.WebSockets;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Owin
{
  internal class OwinWebSocketHandler
  {
    private readonly Func<IWebSocket, Task> _callback;
    private readonly Action<IWebSocket> _prepareWebSocket;
    private readonly int? _maxIncomingMessageSize;

    public OwinWebSocketHandler(
      Func<IWebSocket, Task> callback,
      Action<IWebSocket> prepareWebsocket,
      int? maxIncomingMessageSize)
    {
      this._callback = callback;
      this._prepareWebSocket = prepareWebsocket;
      this._maxIncomingMessageSize = maxIncomingMessageSize;
    }

    public Task ProcessRequest(IDictionary<string, object> environment)
    {
      object obj;
      WebSocket webSocket = environment.TryGetValue(typeof (WebSocketContext).FullName, out obj) ? ((WebSocketContext) obj).WebSocket : (WebSocket) new OwinWebSocketHandler.OwinWebSocket(environment);
      CancellationTokenSource cts = new CancellationTokenSource();
      DefaultWebSocketHandler handler = new DefaultWebSocketHandler(this._maxIncomingMessageSize);
      this._prepareWebSocket((IWebSocket) handler);
      Task task = handler.ProcessWebSocketRequestAsync(webSocket, cts.Token);
      this.RunWebSocketHandler(handler, cts);
      return task;
    }

    private void RunWebSocketHandler(DefaultWebSocketHandler handler, CancellationTokenSource cts) => Task.Run((Func<Task>) (async () =>
    {
      try
      {
        await this._callback((IWebSocket) handler).PreserveCulture();
      }
      catch
      {
      }
      await handler.CloseAsync().PreserveCulture();
      cts.Cancel();
    }));

    private class OwinWebSocket : WebSocket
    {
      private readonly Func<ArraySegment<byte>, int, bool, CancellationToken, Task> _sendAsync;
      private readonly Func<ArraySegment<byte>, CancellationToken, Task<Tuple<int, bool, int>>> _receiveAsync;
      private readonly Func<int, string, CancellationToken, Task> _closeAsync;

      public OwinWebSocket(IDictionary<string, object> env)
      {
        this._sendAsync = (Func<ArraySegment<byte>, int, bool, CancellationToken, Task>) env["websocket.SendAsync"];
        this._receiveAsync = (Func<ArraySegment<byte>, CancellationToken, Task<Tuple<int, bool, int>>>) env["websocket.ReceiveAsync"];
        this._closeAsync = (Func<int, string, CancellationToken, Task>) env["websocket.CloseAsync"];
      }

      public override void Abort()
      {
      }

      public override Task CloseAsync(
        WebSocketCloseStatus closeStatus,
        string statusDescription,
        CancellationToken cancellationToken)
      {
        return this._closeAsync((int) closeStatus, statusDescription, cancellationToken);
      }

      public override Task CloseOutputAsync(
        WebSocketCloseStatus closeStatus,
        string statusDescription,
        CancellationToken cancellationToken)
      {
        return this.CloseAsync(closeStatus, statusDescription, cancellationToken);
      }

      public override WebSocketCloseStatus? CloseStatus => throw new NotImplementedException();

      public override string CloseStatusDescription => throw new NotImplementedException();

      public override void Dispose()
      {
      }

      public override async Task<WebSocketReceiveResult> ReceiveAsync(
        ArraySegment<byte> buffer,
        CancellationToken cancellationToken)
      {
        Tuple<int, bool, int> tuple = await this._receiveAsync(buffer, cancellationToken).PreserveCulture<Tuple<int, bool, int>>();
        int messageType = tuple.Item1;
        bool endOfMessage = tuple.Item2;
        return new WebSocketReceiveResult(tuple.Item3, OwinWebSocketHandler.OwinWebSocket.OpCodeToEnum(messageType), endOfMessage);
      }

      public override Task SendAsync(
        ArraySegment<byte> buffer,
        WebSocketMessageType messageType,
        bool endOfMessage,
        CancellationToken cancellationToken)
      {
        return this._sendAsync(buffer, OwinWebSocketHandler.OwinWebSocket.EnumToOpCode(messageType), endOfMessage, cancellationToken);
      }

      public override WebSocketState State => throw new NotImplementedException();

      public override string SubProtocol => throw new NotImplementedException();

      private static WebSocketMessageType OpCodeToEnum(int messageType)
      {
        if (messageType == 1)
          return WebSocketMessageType.Text;
        if (messageType == 2)
          return WebSocketMessageType.Binary;
        if (messageType == 8)
          return WebSocketMessageType.Close;
        throw new ArgumentOutOfRangeException(nameof (messageType), (object) messageType, string.Empty);
      }

      private static int EnumToOpCode(WebSocketMessageType webSocketMessageType)
      {
        switch (webSocketMessageType)
        {
          case WebSocketMessageType.Text:
            return 1;
          case WebSocketMessageType.Binary:
            return 2;
          case WebSocketMessageType.Close:
            return 8;
          default:
            throw new ArgumentOutOfRangeException(nameof (webSocketMessageType), (object) webSocketMessageType, string.Empty);
        }
      }
    }
  }
}
