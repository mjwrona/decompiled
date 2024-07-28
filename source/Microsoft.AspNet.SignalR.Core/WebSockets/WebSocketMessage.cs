// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.WebSockets.WebSocketMessage
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Net.WebSockets;

namespace Microsoft.AspNet.SignalR.WebSockets
{
  internal sealed class WebSocketMessage
  {
    public static readonly WebSocketMessage EmptyTextMessage = new WebSocketMessage((object) string.Empty, WebSocketMessageType.Text);
    public static readonly WebSocketMessage EmptyBinaryMessage = new WebSocketMessage((object) new byte[0], WebSocketMessageType.Binary);
    public static readonly WebSocketMessage CloseMessage = new WebSocketMessage((object) null, WebSocketMessageType.Close);
    public readonly object Data;
    public readonly WebSocketMessageType MessageType;

    public WebSocketMessage(object data, WebSocketMessageType messageType)
    {
      this.Data = data;
      this.MessageType = messageType;
    }
  }
}
