// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.MessageStoreResult`1
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public struct MessageStoreResult<T> where T : class
  {
    private readonly ulong _firstMessageId;
    private readonly bool _hasMoreData;
    private readonly ArraySegment<T> _messages;

    public MessageStoreResult(ulong firstMessageId, ArraySegment<T> messages, bool hasMoreData)
    {
      this._firstMessageId = firstMessageId;
      this._messages = messages;
      this._hasMoreData = hasMoreData;
    }

    public ulong FirstMessageId => this._firstMessageId;

    public bool HasMoreData => this._hasMoreData;

    public ArraySegment<T> Messages => this._messages;
  }
}
