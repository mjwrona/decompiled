// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.ServerSentEventsTransport
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Transports
{
  public class ServerSentEventsTransport : ForeverTransport
  {
    private readonly IPerformanceCounterManager _counters;
    private static byte[] _keepAlive = Encoding.UTF8.GetBytes("data: {}\n\n");
    private static byte[] _dataInitialized = Encoding.UTF8.GetBytes("data: initialized\n\n");

    public ServerSentEventsTransport(HostContext context, IDependencyResolver resolver)
      : this(context, resolver, resolver.Resolve<IPerformanceCounterManager>())
    {
    }

    public ServerSentEventsTransport(
      HostContext context,
      IDependencyResolver resolver,
      IPerformanceCounterManager performanceCounterManager)
      : base(context, resolver)
    {
      this._counters = performanceCounterManager;
    }

    public override Task KeepAlive() => this.EnqueueOperation((Func<object, Task>) (state => ServerSentEventsTransport.PerformKeepAlive(state)), (object) this);

    public override Task Send(PersistentResponse response)
    {
      this.OnSendingResponse(response);
      return this.EnqueueOperation((Func<object, Task>) (state => ServerSentEventsTransport.PerformSend(state)), (object) new ServerSentEventsTransport.SendContext(this, (object) response));
    }

    public override void IncrementConnectionsCount() => this._counters.ConnectionsCurrentServerSentEvents.Increment();

    public override void DecrementConnectionsCount() => this._counters.ConnectionsCurrentServerSentEvents.Decrement();

    protected internal override Task InitializeResponse(ITransportConnection connection) => base.InitializeResponse(connection).Then<ServerSentEventsTransport>((Func<ServerSentEventsTransport, Task>) (s => ServerSentEventsTransport.WriteInit(s)), this);

    private static Task PerformKeepAlive(object state)
    {
      ServerSentEventsTransport sentEventsTransport = (ServerSentEventsTransport) state;
      sentEventsTransport.Context.Response.Write(new ArraySegment<byte>(ServerSentEventsTransport._keepAlive));
      return sentEventsTransport.Context.Response.Flush();
    }

    private static Task PerformSend(object state)
    {
      ServerSentEventsTransport.SendContext sendContext = (ServerSentEventsTransport.SendContext) state;
      using (BinaryMemoryPoolTextWriter writer = new BinaryMemoryPoolTextWriter(sendContext.Transport.Pool))
      {
        writer.Write("data: ");
        sendContext.Transport.JsonSerializer.Serialize(sendContext.State, (TextWriter) writer);
        writer.WriteLine();
        writer.WriteLine();
        writer.Flush();
        sendContext.Transport.Context.Response.Write(writer.Buffer);
        sendContext.Transport.TraceOutgoingMessage(writer.Buffer);
      }
      return sendContext.Transport.Context.Response.Flush();
    }

    private static Task WriteInit(ServerSentEventsTransport transport)
    {
      transport.Context.Response.ContentType = "text/event-stream";
      transport.Context.Response.Write(new ArraySegment<byte>(ServerSentEventsTransport._dataInitialized));
      return transport.Context.Response.Flush();
    }

    private class SendContext
    {
      public readonly ServerSentEventsTransport Transport;
      public readonly object State;

      public SendContext(ServerSentEventsTransport transport, object state)
      {
        this.Transport = transport;
        this.State = state;
      }
    }
  }
}
