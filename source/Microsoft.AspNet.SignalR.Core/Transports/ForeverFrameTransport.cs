// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.ForeverFrameTransport
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Transports
{
  public class ForeverFrameTransport : ForeverTransport
  {
    private const string _initPrefix = "<!DOCTYPE html><html><head><title>SignalR Forever Frame Transport Stream</title>\r\n<script>\r\n    var $ = window.parent.jQuery,\r\n        ff = $ ? $.signalR.transports.foreverFrame : null,\r\n        c =  ff ? ff.getConnection('";
    private const string _initSuffix = "') : null,\r\n        r = ff ? ff.receive : function() {};\r\n    ff ? ff.started(c) : '';</script></head><body>\r\n";
    private readonly IPerformanceCounterManager _counters;

    public ForeverFrameTransport(HostContext context, IDependencyResolver resolver)
      : this(context, resolver, resolver.Resolve<IPerformanceCounterManager>())
    {
    }

    public ForeverFrameTransport(
      HostContext context,
      IDependencyResolver resolver,
      IPerformanceCounterManager performanceCounterManager)
      : base(context, resolver)
    {
      this._counters = performanceCounterManager;
    }

    public override void IncrementConnectionsCount() => this._counters.ConnectionsCurrentForeverFrame.Increment();

    public override void DecrementConnectionsCount() => this._counters.ConnectionsCurrentForeverFrame.Decrement();

    public override Task KeepAlive() => this.EnqueueOperation((Func<object, Task>) (state => ForeverFrameTransport.PerformKeepAlive(state)), (object) this);

    public override Task Send(PersistentResponse response)
    {
      this.OnSendingResponse(response);
      return this.EnqueueOperation((Func<object, Task>) (s => ForeverFrameTransport.PerformSend(s)), (object) new ForeverFrameTransport.ForeverFrameTransportContext(this, (object) response));
    }

    protected internal override Task InitializeResponse(ITransportConnection connection)
    {
      string s1 = this.Context.Request.QueryString["frameId"];
      uint result;
      ForeverFrameTransport.ForeverFrameTransportContext transportContext = !string.IsNullOrWhiteSpace(s1) && uint.TryParse(s1, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? new ForeverFrameTransport.ForeverFrameTransportContext(this, (object) ("<!DOCTYPE html><html><head><title>SignalR Forever Frame Transport Stream</title>\r\n<script>\r\n    var $ = window.parent.jQuery,\r\n        ff = $ ? $.signalR.transports.foreverFrame : null,\r\n        c =  ff ? ff.getConnection('" + result.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "') : null,\r\n        r = ff ? ff.receive : function() {};\r\n    ff ? ff.started(c) : '';</script></head><body>\r\n")) : throw new InvalidOperationException(Resources.Error_InvalidForeverFrameId);
      return base.InitializeResponse(connection).Then<ForeverFrameTransport.ForeverFrameTransportContext>((Func<ForeverFrameTransport.ForeverFrameTransportContext, Task>) (s => ForeverFrameTransport.Initialize((object) s)), transportContext);
    }

    internal override MemoryPoolTextWriter CreateMemoryPoolWriter(IMemoryPool memoryPool) => (MemoryPoolTextWriter) new ForeverFrameTransport.HTMLTextWriter(memoryPool);

    private static Task Initialize(object state)
    {
      ForeverFrameTransport.ForeverFrameTransportContext transportContext = (ForeverFrameTransport.ForeverFrameTransportContext) state;
      return ForeverFrameTransport.WriteInit(new ForeverFrameTransport.ForeverFrameTransportContext(transportContext.Transport, transportContext.State));
    }

    private static Task WriteInit(
      ForeverFrameTransport.ForeverFrameTransportContext context)
    {
      context.Transport.Context.Response.ContentType = "text/html; charset=UTF-8";
      using (ForeverFrameTransport.HTMLTextWriter htmlTextWriter = new ForeverFrameTransport.HTMLTextWriter(context.Transport.Pool))
      {
        htmlTextWriter.NewLine = "\n";
        htmlTextWriter.WriteRaw((string) context.State);
        htmlTextWriter.Flush();
        context.Transport.Context.Response.Write(htmlTextWriter.Buffer);
      }
      return context.Transport.Context.Response.Flush();
    }

    private static Task PerformSend(object state)
    {
      ForeverFrameTransport.ForeverFrameTransportContext transportContext = (ForeverFrameTransport.ForeverFrameTransportContext) state;
      using (ForeverFrameTransport.HTMLTextWriter writer = new ForeverFrameTransport.HTMLTextWriter(transportContext.Transport.Pool))
      {
        writer.NewLine = "\n";
        writer.WriteRaw("<script>r(c, ");
        transportContext.Transport.JsonSerializer.Serialize(transportContext.State, (TextWriter) writer);
        writer.WriteRaw(");</script>\r\n");
        writer.Flush();
        transportContext.Transport.Context.Response.Write(writer.Buffer);
      }
      return transportContext.Transport.Context.Response.Flush();
    }

    private static Task PerformKeepAlive(object state)
    {
      ForeverFrameTransport foreverFrameTransport = (ForeverFrameTransport) state;
      using (ForeverFrameTransport.HTMLTextWriter htmlTextWriter = new ForeverFrameTransport.HTMLTextWriter(foreverFrameTransport.Pool))
      {
        htmlTextWriter.NewLine = "\n";
        htmlTextWriter.WriteRaw("<script>r(c, {});</script>");
        htmlTextWriter.WriteLine();
        htmlTextWriter.WriteLine();
        htmlTextWriter.Flush();
        foreverFrameTransport.Context.Response.Write(htmlTextWriter.Buffer);
      }
      return foreverFrameTransport.Context.Response.Flush();
    }

    private struct ForeverFrameTransportContext
    {
      public readonly ForeverFrameTransport Transport;
      public readonly object State;

      public ForeverFrameTransportContext(ForeverFrameTransport transport, object state)
      {
        this.Transport = transport;
        this.State = state;
      }
    }

    private class HTMLTextWriter : MemoryPoolTextWriter
    {
      public HTMLTextWriter(IMemoryPool pool)
        : base(pool)
      {
      }

      public void WriteRaw(string value) => base.Write(value);

      public override void Write(string value) => base.Write(ForeverFrameTransport.HTMLTextWriter.JavascriptEncode(value));

      public override void WriteLine(string value) => base.WriteLine(ForeverFrameTransport.HTMLTextWriter.JavascriptEncode(value));

      private static string JavascriptEncode(string input) => input.Replace("<", "\\u003c").Replace(">", "\\u003e");
    }
  }
}
