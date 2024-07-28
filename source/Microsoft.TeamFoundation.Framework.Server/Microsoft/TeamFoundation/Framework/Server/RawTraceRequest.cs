// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RawTraceRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class RawTraceRequest : ITraceRequest
  {
    public RawTraceRequest(Guid e2eId = default (Guid))
    {
      if (e2eId == Guid.Empty)
      {
        this.E2EID = Guid.NewGuid();
        this.ActivityID = this.E2EID;
      }
      else
      {
        this.E2EID = e2eId;
        this.ActivityID = Guid.NewGuid();
      }
      System.Diagnostics.Trace.CorrelationManager.ActivityId = this.ActivityID;
    }

    public bool IsTracing(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags = null)
    {
      return TeamFoundationTracingService.IsRawTracingEnabled(tracepoint, level, area, layer, tags);
    }

    public void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, tags, (string) null, Guid.Empty, Guid.Empty, this.E2EID, (string) null, Guid.Empty, format);
    }

    public void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      object arg0)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, tags, (string) null, Guid.Empty, Guid.Empty, this.E2EID, (string) null, Guid.Empty, format, arg0);
    }

    public void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      object arg0,
      object arg1)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, tags, (string) null, Guid.Empty, Guid.Empty, this.E2EID, (string) null, Guid.Empty, format, arg0, arg1);
    }

    public void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      object arg0,
      object arg1,
      object arg2)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, tags, (string) null, Guid.Empty, Guid.Empty, this.E2EID, (string) null, Guid.Empty, format, arg0, arg1, arg2);
    }

    public void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, tags, (string) null, Guid.Empty, Guid.Empty, this.E2EID, (string) null, Guid.Empty, format, args);
    }

    public void TraceAlways(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string[] tags,
      string format,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawAlwaysOn(tracepoint, level, area, layer, this.E2EID, Guid.Empty, tags, format, args);
    }

    public void TraceAlways(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string format,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawAlwaysOn(tracepoint, level, area, layer, this.E2EID, Guid.Empty, (string[]) null, format, args);
    }

    public void TraceEnter(int tracepoint, string area, string layer, [CallerMemberName] string methodName = null) => TeamFoundationTracingService.TraceEnterRaw(tracepoint, area, layer, methodName, new Guid?(), new Guid?(), new Guid?(this.E2EID), new Guid?(Guid.Empty), (string) null);

    public void TraceException(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Exception exception)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, (string[]) null, exception.GetType().FullName, Guid.Empty, Guid.Empty, this.E2EID, (string) null, Guid.Empty, "{0}", (object) new Lazy<string>((Func<string>) (() => exception.ToReadableStackTrace())));
    }

    public void TraceException(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Exception exception,
      string format,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRawCore(tracepoint, level, area, layer, (string[]) null, exception.GetType().FullName, Guid.Empty, Guid.Empty, this.E2EID, (string) null, Guid.Empty, format, args);
    }

    public void TraceLeave(int tracepoint, string area, string layer, [CallerMemberName] string methodName = null) => TeamFoundationTracingService.TraceLeaveRaw(tracepoint, area, layer, methodName, e2eId: new Guid?(this.E2EID), uniqueIdentifier: new Guid?(Guid.Empty));

    public Guid ActivityID { get; }

    public Guid E2EID { get; }
  }
}
