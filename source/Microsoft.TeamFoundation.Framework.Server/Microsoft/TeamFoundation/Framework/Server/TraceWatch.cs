// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TraceWatch
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TraceWatch : IDisposable
  {
    private const string c_StringFormat = "TimeElapsed:{0}ms Query:{1}";

    public TraceWatch(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel traceLevel,
      TimeSpan timeout,
      string area,
      string layer,
      string format,
      params object[] args)
    {
      this.Timeout = timeout;
      this.Tracepoint = tracepoint;
      this.TraceLevel = traceLevel;
      this.Area = area;
      this.Layer = layer;
      this.Format = format;
      this.Args = args;
      this.RequestContext = requestContext;
      this.Watch = new Stopwatch();
      this.Watch.Start();
    }

    public TraceLevel TraceLevel { get; private set; }

    public Stopwatch Watch { get; private set; }

    public TimeSpan Timeout { get; private set; }

    public int Tracepoint { get; private set; }

    public string Area { get; private set; }

    public string Layer { get; private set; }

    public string Format { get; private set; }

    private object[] Args { get; set; }

    private IVssRequestContext RequestContext { get; set; }

    public TimeSpan TimeElapsed => this.Watch.Elapsed;

    internal string GetTraceMessage()
    {
      for (int index = 0; index < this.Args.Length; ++index)
      {
        if (this.Args[index] is Lazy<string> lazy)
          this.Args[index] = (object) lazy.Value;
      }
      return string.Format(this.Format, this.Args);
    }

    public void Dispose()
    {
      this.Watch.Stop();
      if (!(this.Watch.Elapsed > this.Timeout))
      {
        if (this.Tracepoint == 0)
          return;
        IVssRequestContext requestContext = this.RequestContext;
        if ((requestContext != null ? (requestContext.IsTracing(this.Tracepoint, TraceLevel.Verbose, this.Area, this.Layer) ? 1 : 0) : 0) == 0)
          return;
      }
      TraceLevel level = this.Watch.Elapsed > this.Timeout ? this.TraceLevel : TraceLevel.Verbose;
      if (this.RequestContext != null)
        this.RequestContext.Trace(this.Tracepoint, level, this.Area, this.Layer, "TimeElapsed:{0}ms Query:{1}", (object) this.Watch.ElapsedMilliseconds, (object) this.GetTraceMessage());
      else
        TeamFoundationTracingService.TraceRaw(this.Tracepoint, level, this.Area, this.Layer, "TimeElapsed:{0}ms Query:{1}", (object) this.Watch.ElapsedMilliseconds, (object) this.GetTraceMessage());
    }
  }
}
