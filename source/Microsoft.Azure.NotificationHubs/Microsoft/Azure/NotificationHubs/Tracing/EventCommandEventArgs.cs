// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.EventCommandEventArgs
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal class EventCommandEventArgs : EventArgs
  {
    internal EventSource eventSource;
    internal EventDispatcher dispatcher;

    public EventCommand Command { get; private set; }

    public IDictionary<string, string> Arguments { get; private set; }

    public bool EnableEvent(int eventId)
    {
      if (this.Command != EventCommand.Enable && this.Command != EventCommand.Disable)
        throw new InvalidOperationException();
      return this.eventSource.EnableEventForDispatcher(this.dispatcher, eventId, true);
    }

    public bool DisableEvent(int eventId)
    {
      if (this.Command != EventCommand.Enable && this.Command != EventCommand.Disable)
        throw new InvalidOperationException();
      return this.eventSource.EnableEventForDispatcher(this.dispatcher, eventId, false);
    }

    internal EventCommandEventArgs(
      EventCommand command,
      IDictionary<string, string> arguments,
      EventSource eventSource,
      EventDispatcher dispatcher)
    {
      this.Command = command;
      this.Arguments = arguments;
      this.eventSource = eventSource;
      this.dispatcher = dispatcher;
    }
  }
}
