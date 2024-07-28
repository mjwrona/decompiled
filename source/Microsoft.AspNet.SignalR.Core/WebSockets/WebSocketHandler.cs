// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.WebSockets.WebSocketHandler
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.WebSockets
{
  public abstract class WebSocketHandler
  {
    private static readonly TimeSpan _closeTimeout = TimeSpan.FromMilliseconds(250.0);
    private const int _receiveLoopBufferSize = 4096;
    private readonly int? _maxIncomingMessageSize;
    private readonly TaskQueue _sendQueue = new TaskQueue();

    protected WebSocketHandler(int? maxIncomingMessageSize) => this._maxIncomingMessageSize = maxIncomingMessageSize;

    public virtual void OnOpen()
    {
    }

    public virtual void OnMessage(string message) => throw new NotImplementedException();

    public virtual void OnMessage(byte[] message) => throw new NotImplementedException();

    public virtual void OnError()
    {
    }

    public virtual void OnClose()
    {
    }

    public virtual Task Send(string message) => message != null ? this.SendAsync(message) : throw new ArgumentNullException(nameof (message));

    internal Task SendAsync(string message) => this.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text);

    internal virtual Task SendAsync(
      ArraySegment<byte> message,
      WebSocketMessageType messageType,
      bool endOfMessage = true)
    {
      return WebSocketHandler.GetWebSocketState(this.WebSocket) != WebSocketState.Open ? TaskAsyncHelper.Empty : this._sendQueue.Enqueue((Func<object, Task>) (async state =>
      {
        WebSocketHandler.SendContext sendContext = (WebSocketHandler.SendContext) state;
        if (WebSocketHandler.GetWebSocketState(sendContext.Handler.WebSocket) != WebSocketState.Open)
          return;
        try
        {
          await sendContext.Handler.WebSocket.SendAsync(sendContext.Message, sendContext.MessageType, sendContext.EndOfMessage, CancellationToken.None).PreserveCulture();
        }
        catch (Exception ex)
        {
          Trace.TraceError("Error while sending: " + (object) ex);
        }
      }), (object) new WebSocketHandler.SendContext(this, message, messageType, endOfMessage));
    }

    public virtual Task CloseAsync() => WebSocketHandler.IsClosedOrClosedSent(this.WebSocket) ? TaskAsyncHelper.Empty : this._sendQueue.Enqueue((Func<object, Task>) (async state =>
    {
      WebSocketHandler.CloseContext closeContext = (WebSocketHandler.CloseContext) state;
      if (WebSocketHandler.IsClosedOrClosedSent(closeContext.Handler.WebSocket))
        return;
      try
      {
        await closeContext.Handler.WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).PreserveCulture();
      }
      catch (Exception ex)
      {
        Trace.TraceError("Error while closing the websocket: " + (object) ex);
      }
    }), (object) new WebSocketHandler.CloseContext(this));

    public int? MaxIncomingMessageSize => this._maxIncomingMessageSize;

    internal WebSocket WebSocket { get; set; }

    public Exception Error { get; set; }

    internal Task ProcessWebSocketRequestAsync(
      WebSocket webSocket,
      CancellationToken disconnectToken)
    {
      if (webSocket == null)
        throw new ArgumentNullException(nameof (webSocket));
      WebSocketHandler.ReceiveContext state1 = new WebSocketHandler.ReceiveContext(webSocket, disconnectToken, this.MaxIncomingMessageSize, 4096);
      return this.ProcessWebSocketRequestAsync(webSocket, disconnectToken, (Func<object, Task<WebSocketMessage>>) (state =>
      {
        WebSocketHandler.ReceiveContext receiveContext = (WebSocketHandler.ReceiveContext) state;
        return WebSocketMessageReader.ReadMessageAsync(receiveContext.WebSocket, receiveContext.BufferSize, receiveContext.MaxIncomingMessageSize, receiveContext.DisconnectToken);
      }), (object) state1);
    }

    internal async Task ProcessWebSocketRequestAsync(
      WebSocket webSocket,
      CancellationToken disconnectToken,
      Func<object, Task<WebSocketMessage>> messageRetriever,
      object state)
    {
      bool closedReceived = false;
      try
      {
        this.WebSocket = webSocket;
        this.OnOpen();
        while (!disconnectToken.IsCancellationRequested)
        {
          if (!closedReceived)
          {
            WebSocketMessage webSocketMessage = await messageRetriever(state).PreserveCulture<WebSocketMessage>();
            switch (webSocketMessage.MessageType)
            {
              case WebSocketMessageType.Text:
                this.OnMessage((string) webSocketMessage.Data);
                continue;
              case WebSocketMessageType.Binary:
                this.OnMessage((byte[]) webSocketMessage.Data);
                continue;
              default:
                closedReceived = true;
                Task task = await Task.WhenAny(this.CloseAsync(), Task.Delay(WebSocketHandler._closeTimeout)).PreserveCulture<Task>();
                continue;
            }
          }
          else
            break;
        }
      }
      catch (OperationCanceledException ex)
      {
        if (!disconnectToken.IsCancellationRequested)
        {
          this.Error = (Exception) ex;
          this.OnError();
        }
      }
      catch (ObjectDisposedException ex)
      {
      }
      catch (Exception ex)
      {
        if (WebSocketHandler.IsFatalException(ex))
        {
          this.Error = ex;
          this.OnError();
        }
      }
      try
      {
      }
      finally
      {
        this.OnClose();
      }
    }

    private static bool IsFatalException(Exception ex)
    {
      if (ex is COMException comException)
      {
        switch ((uint) comException.ErrorCode)
        {
          case 2147942438:
          case 2147943395:
          case 2147943629:
            return false;
        }
      }
      return true;
    }

    private static bool IsClosedOrClosedSent(WebSocket webSocket)
    {
      WebSocketState webSocketState = WebSocketHandler.GetWebSocketState(webSocket);
      switch (webSocketState)
      {
        case WebSocketState.CloseSent:
        case WebSocketState.Closed:
          return true;
        default:
          return webSocketState == WebSocketState.Aborted;
      }
    }

    private static WebSocketState GetWebSocketState(WebSocket webSocket)
    {
      try
      {
        return webSocket.State;
      }
      catch (ObjectDisposedException ex)
      {
        return WebSocketState.Closed;
      }
    }

    private class CloseContext
    {
      public WebSocketHandler Handler;

      public CloseContext(WebSocketHandler webSocketHandler) => this.Handler = webSocketHandler;
    }

    private class SendContext
    {
      public WebSocketHandler Handler;
      public ArraySegment<byte> Message;
      public WebSocketMessageType MessageType;
      public bool EndOfMessage;

      public SendContext(
        WebSocketHandler webSocketHandler,
        ArraySegment<byte> message,
        WebSocketMessageType messageType,
        bool endOfMessage)
      {
        this.Handler = webSocketHandler;
        this.Message = message;
        this.MessageType = messageType;
        this.EndOfMessage = endOfMessage;
      }
    }

    private class ReceiveContext
    {
      public WebSocket WebSocket;
      public CancellationToken DisconnectToken;
      public int? MaxIncomingMessageSize;
      public int BufferSize;

      public ReceiveContext(
        WebSocket webSocket,
        CancellationToken disconnectToken,
        int? maxIncomingMessageSize,
        int bufferSize)
      {
        this.WebSocket = webSocket;
        this.DisconnectToken = disconnectToken;
        this.MaxIncomingMessageSize = maxIncomingMessageSize;
        this.BufferSize = bufferSize;
      }
    }
  }
}
