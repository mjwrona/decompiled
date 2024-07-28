// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.MessageResult
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public struct MessageResult
  {
    private static readonly List<ArraySegment<Message>> _emptyList = new List<ArraySegment<Message>>();
    public static readonly MessageResult TerminalMessage = new MessageResult(true);

    public IList<ArraySegment<Message>> Messages { get; private set; }

    public int TotalCount { get; private set; }

    public bool Terminal { get; set; }

    public MessageResult(bool terminal)
      : this((IList<ArraySegment<Message>>) MessageResult._emptyList, 0)
    {
      this.Terminal = terminal;
    }

    public MessageResult(IList<ArraySegment<Message>> messages, int totalCount)
      : this()
    {
      this.Messages = messages;
      this.TotalCount = totalCount;
    }
  }
}
