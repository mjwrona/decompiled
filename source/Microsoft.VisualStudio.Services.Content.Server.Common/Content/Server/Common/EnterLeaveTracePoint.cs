// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.EnterLeaveTracePoint
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class EnterLeaveTracePoint
  {
    public readonly int EnterTracePoint;
    public readonly int LeaveTracePoint;
    public readonly string Area;
    public readonly string Layer;

    public EnterLeaveTracePoint(
      int enterTracePoint,
      int leaveTracePoint,
      string area,
      string layer)
    {
      this.EnterTracePoint = enterTracePoint;
      this.LeaveTracePoint = leaveTracePoint;
      this.Area = area;
      this.Layer = layer;
    }

    public IDisposable Enter(IVssRequestContext context, [CallerMemberName] string methodName = "")
    {
      context.TraceEnter(this.EnterTracePoint, this.Area, this.Layer, methodName);
      return (IDisposable) new EnterLeaveTracePoint.InternalTracer(this, context, methodName);
    }

    private class InternalTracer : IDisposable
    {
      private readonly EnterLeaveTracePoint tracePoint;
      private readonly IVssRequestContext requestContext;
      private readonly string methodName;

      public InternalTracer(
        EnterLeaveTracePoint tracePoint,
        IVssRequestContext requestContext,
        string methodName)
      {
        this.tracePoint = tracePoint ?? throw new ArgumentNullException(nameof (tracePoint));
        this.requestContext = requestContext ?? throw new ArgumentNullException(nameof (requestContext));
        this.methodName = methodName;
      }

      public void Dispose() => this.requestContext.TraceLeave(this.tracePoint.LeaveTracePoint, this.tracePoint.Area, this.tracePoint.Layer, this.methodName);
    }
  }
}
