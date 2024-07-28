// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.MessageBusExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public static class MessageBusExtensions
  {
    public static Task Publish(this IMessageBus bus, string source, string key, string value)
    {
      if (bus == null)
        throw new ArgumentNullException(nameof (bus));
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (string.IsNullOrEmpty(key))
        throw new ArgumentNullException(nameof (key));
      return bus.Publish(new Message(source, key, value));
    }

    internal static Task Ack(this IMessageBus bus, string acker, string commandId) => bus.Publish(new Message(acker, "__SIGNALR__SERVER__", (string) null)
    {
      CommandId = commandId,
      IsAck = true
    });

    public static void Enumerate(
      this IList<ArraySegment<Message>> messages,
      Action<Message> onMessage)
    {
      if (messages == null)
        throw new ArgumentNullException(nameof (messages));
      if (onMessage == null)
        throw new ArgumentNullException(nameof (onMessage));
      messages.Enumerate<object>((Func<Message, bool>) (message => true), (Action<object, Message>) ((state, message) => onMessage(message)), (object) null);
    }

    public static void Enumerate<T>(
      this IList<ArraySegment<Message>> messages,
      Func<Message, bool> filter,
      Action<T, Message> onMessage,
      T state)
    {
      if (messages == null)
        throw new ArgumentNullException(nameof (messages));
      if (filter == null)
        throw new ArgumentNullException(nameof (filter));
      if (onMessage == null)
        throw new ArgumentNullException(nameof (onMessage));
      for (int index = 0; index < messages.Count; ++index)
      {
        ArraySegment<Message> message1 = messages[index];
        for (int offset = message1.Offset; offset < message1.Offset + message1.Count; ++offset)
        {
          Message message2 = message1.Array[offset];
          if (filter(message2))
            onMessage(state, message2);
        }
      }
    }
  }
}
