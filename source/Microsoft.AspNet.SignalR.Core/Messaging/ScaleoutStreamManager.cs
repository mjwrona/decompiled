// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.ScaleoutStreamManager
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Messaging
{
  internal class ScaleoutStreamManager
  {
    private readonly Func<int, IList<Message>, Task> _send;
    private readonly Action<int, ulong, ScaleoutMessage> _receive;
    private readonly ScaleoutStream[] _streams;

    public ScaleoutStreamManager(
      Func<int, IList<Message>, Task> send,
      Action<int, ulong, ScaleoutMessage> receive,
      int streamCount,
      TraceSource trace,
      IPerformanceCounterManager performanceCounters,
      ScaleoutConfiguration configuration)
      : this(send, receive, streamCount, trace, performanceCounters, configuration, (int) ushort.MaxValue)
    {
    }

    public ScaleoutStreamManager(
      Func<int, IList<Message>, Task> send,
      Action<int, ulong, ScaleoutMessage> receive,
      int streamCount,
      TraceSource trace,
      IPerformanceCounterManager performanceCounters,
      ScaleoutConfiguration configuration,
      int maxScaleoutMappings)
    {
      if (configuration.QueueBehavior != QueuingBehavior.Disabled && configuration.MaxQueueLength == 0)
        throw new InvalidOperationException(Resources.Error_ScaleoutQueuingConfig);
      this._streams = new ScaleoutStream[streamCount];
      this._send = send;
      this._receive = receive;
      ScaleoutMappingStore[] list = new ScaleoutMappingStore[streamCount];
      performanceCounters.ScaleoutStreamCountTotal.RawValue = (long) streamCount;
      performanceCounters.ScaleoutStreamCountBuffering.RawValue = (long) streamCount;
      performanceCounters.ScaleoutStreamCountOpen.RawValue = 0L;
      for (int index = 0; index < streamCount; ++index)
      {
        this._streams[index] = new ScaleoutStream(trace, "Stream(" + (object) index + ")", configuration.QueueBehavior, configuration.MaxQueueLength, performanceCounters);
        list[index] = new ScaleoutMappingStore(maxScaleoutMappings);
      }
      this.Streams = (IList<ScaleoutMappingStore>) new ReadOnlyCollection<ScaleoutMappingStore>((IList<ScaleoutMappingStore>) list);
    }

    public IList<ScaleoutMappingStore> Streams { get; private set; }

    public void Open(int streamIndex) => this._streams[streamIndex].Open();

    public void Close(int streamIndex) => this._streams[streamIndex].Close();

    public void OnError(int streamIndex, Exception exception) => this._streams[streamIndex].SetError(exception);

    public Task Send(int streamIndex, IList<Message> messages)
    {
      ScaleoutStreamManager.SendContext state1 = new ScaleoutStreamManager.SendContext(this, streamIndex, messages);
      return this._streams[streamIndex].Send((Func<object, Task>) (state => ScaleoutStreamManager.Send(state)), (object) state1);
    }

    public void OnReceived(int streamIndex, ulong id, ScaleoutMessage message)
    {
      this._receive(streamIndex, id, message);
      this.Open(streamIndex);
    }

    private static Task Send(object state)
    {
      ScaleoutStreamManager.SendContext sendContext = (ScaleoutStreamManager.SendContext) state;
      return sendContext.StreamManager._send(sendContext.Index, sendContext.Messages);
    }

    private class SendContext
    {
      public ScaleoutStreamManager StreamManager;
      public int Index;
      public IList<Message> Messages;

      public SendContext(ScaleoutStreamManager scaleoutStream, int index, IList<Message> messages)
      {
        this.StreamManager = scaleoutStream;
        this.Index = index;
        this.Messages = messages;
      }
    }
  }
}
