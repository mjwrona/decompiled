// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.WebSockets.DefaultWebSocketHandler
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.WebSockets
{
  public class DefaultWebSocketHandler : WebSocketHandler, IWebSocket
  {
    private readonly IWebSocket _webSocket;
    private volatile bool _closed;

    public DefaultWebSocketHandler(int? maxIncomingMessageSize)
      : base(maxIncomingMessageSize)
    {
      this._webSocket = (IWebSocket) this;
      this._webSocket.OnClose = (System.Action) (() => { });
      this._webSocket.OnError = (Action<Exception>) (e => { });
      this._webSocket.OnMessage = (Action<string>) (msg => { });
    }

    public override void OnClose()
    {
      this._closed = true;
      this._webSocket.OnClose();
    }

    public override void OnError() => this._webSocket.OnError(this.Error);

    public override void OnMessage(string message) => this._webSocket.OnMessage(message);

    Action<string> IWebSocket.OnMessage { get; set; }

    System.Action IWebSocket.OnClose { get; set; }

    Action<Exception> IWebSocket.OnError { get; set; }

    public override Task Send(string message) => this._closed ? TaskAsyncHelper.Empty : base.Send(message);

    public Task Send(ArraySegment<byte> message) => this._closed ? TaskAsyncHelper.Empty : this.SendAsync(message, WebSocketMessageType.Text);

    public override Task CloseAsync() => this._closed ? TaskAsyncHelper.Empty : base.CloseAsync();
  }
}
