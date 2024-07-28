// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.WebSockets.WebSocketMessageReader
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.WebSockets
{
  internal static class WebSocketMessageReader
  {
    private static readonly ArraySegment<byte> _emptyArraySegment = new ArraySegment<byte>(new byte[0]);

    private static byte[] BufferSliceToByteArray(byte[] buffer, int count)
    {
      byte[] dst = new byte[count];
      System.Buffer.BlockCopy((Array) buffer, 0, (Array) dst, 0, count);
      return dst;
    }

    private static string BufferSliceToString(byte[] buffer, int count) => Encoding.UTF8.GetString(buffer, 0, count);

    public static async Task<WebSocketMessage> ReadMessageAsync(
      WebSocket webSocket,
      int bufferSize,
      int? maxMessageSize,
      CancellationToken disconnectToken)
    {
      WebSocketMessage message;
      if (WebSocketMessageReader.TryGetMessage(await webSocket.ReceiveAsync(WebSocketMessageReader._emptyArraySegment, disconnectToken).PreserveCultureNotContext<WebSocketReceiveResult>(), (byte[]) null, out message))
        return message;
      byte[] buffer = new byte[bufferSize];
      ArraySegment<byte> arraySegment = new ArraySegment<byte>(buffer);
      WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(arraySegment, disconnectToken).PreserveCultureNotContext<WebSocketReceiveResult>();
      if (WebSocketMessageReader.TryGetMessage(receiveResult, buffer, out message))
        return message;
      ByteBuffer bytebuffer = new ByteBuffer(maxMessageSize);
      bytebuffer.Append(WebSocketMessageReader.BufferSliceToByteArray(buffer, receiveResult.Count));
      WebSocketMessageType originalMessageType = receiveResult.MessageType;
      WebSocketReceiveResult socketReceiveResult;
      do
      {
        socketReceiveResult = await webSocket.ReceiveAsync(arraySegment, disconnectToken).PreserveCultureNotContext<WebSocketReceiveResult>();
        if (socketReceiveResult.MessageType == WebSocketMessageType.Close)
          return WebSocketMessage.CloseMessage;
        if (socketReceiveResult.MessageType != originalMessageType)
          throw new InvalidOperationException("Incorrect message type");
        bytebuffer.Append(WebSocketMessageReader.BufferSliceToByteArray(buffer, socketReceiveResult.Count));
      }
      while (!socketReceiveResult.EndOfMessage);
      switch (socketReceiveResult.MessageType)
      {
        case WebSocketMessageType.Text:
          return new WebSocketMessage((object) bytebuffer.GetString(), WebSocketMessageType.Text);
        case WebSocketMessageType.Binary:
          return new WebSocketMessage((object) bytebuffer.GetByteArray(), WebSocketMessageType.Binary);
        default:
          throw new InvalidOperationException("Unknown message type");
      }
    }

    private static bool TryGetMessage(
      WebSocketReceiveResult receiveResult,
      byte[] buffer,
      out WebSocketMessage message)
    {
      message = (WebSocketMessage) null;
      if (receiveResult.MessageType == WebSocketMessageType.Close)
        message = WebSocketMessage.CloseMessage;
      else if (receiveResult.EndOfMessage)
      {
        switch (receiveResult.MessageType)
        {
          case WebSocketMessageType.Text:
            message = buffer != null ? new WebSocketMessage((object) WebSocketMessageReader.BufferSliceToString(buffer, receiveResult.Count), WebSocketMessageType.Text) : WebSocketMessage.EmptyTextMessage;
            break;
          case WebSocketMessageType.Binary:
            message = buffer != null ? new WebSocketMessage((object) WebSocketMessageReader.BufferSliceToByteArray(buffer, receiveResult.Count), WebSocketMessageType.Binary) : WebSocketMessage.EmptyBinaryMessage;
            break;
          default:
            throw new InvalidOperationException("Unknown message type");
        }
      }
      return message != null;
    }
  }
}
