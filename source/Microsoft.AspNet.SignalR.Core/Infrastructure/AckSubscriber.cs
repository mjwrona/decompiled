// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.AckSubscriber
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal class AckSubscriber : ISubscriber, IDisposable
  {
    private readonly IMessageBus _messageBus;
    private readonly IAckHandler _ackHandler;
    private IDisposable _subscription;
    private const int MaxMessages = 10;
    private static readonly string[] ServerSignals = new string[1]
    {
      "__SIGNALR__SERVER__"
    };
    public const string Signal = "__SIGNALR__SERVER__";

    public AckSubscriber(IDependencyResolver resolver)
      : this(resolver.Resolve<IMessageBus>(), resolver.Resolve<IAckHandler>())
    {
    }

    public AckSubscriber(IMessageBus messageBus, IAckHandler ackHandler)
    {
      this._messageBus = messageBus;
      this._ackHandler = ackHandler;
      this.Identity = Guid.NewGuid().ToString();
      this.ProcessMessages();
    }

    public IList<string> EventKeys => (IList<string>) AckSubscriber.ServerSignals;

    public event Action<ISubscriber, string> EventKeyAdded
    {
      add
      {
      }
      remove
      {
      }
    }

    public event Action<ISubscriber, string> EventKeyRemoved
    {
      add
      {
      }
      remove
      {
      }
    }

    public Action<TextWriter> WriteCursor { get; set; }

    public string Identity { get; private set; }

    public Subscription Subscription { get; set; }

    public void Dispose()
    {
      if (this._subscription == null)
        return;
      this._subscription.Dispose();
    }

    private void ProcessMessages() => this._subscription = this._messageBus.Subscribe((ISubscriber) this, (string) null, new Func<MessageResult, object, Task<bool>>(this.TriggerAcks), 10, (object) null);

    private Task<bool> TriggerAcks(MessageResult result, object state)
    {
      result.Messages.Enumerate<object>((Func<Message, bool>) (m => m.IsAck), (Action<object, Message>) ((s, m) => ((IAckHandler) s).TriggerAck(m.CommandId)), (object) this._ackHandler);
      return TaskAsyncHelper.True;
    }
  }
}
