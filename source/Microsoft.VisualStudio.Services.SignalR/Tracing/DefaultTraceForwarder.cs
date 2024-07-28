// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.Tracing.DefaultTraceForwarder
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;
using System.Web;

namespace Microsoft.VisualStudio.Services.SignalR.Tracing
{
  internal class DefaultTraceForwarder : IVssSignalRTraceForwarder
  {
    public void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message)
    {
      IVssRequestContext requestContext = HttpContext.Current != null ? HttpContext.Current.Items[(object) HttpContextConstants.IVssRequestContext] as IVssRequestContext : (IVssRequestContext) null;
      if (requestContext != null)
        requestContext.Trace(tracepoint, level, area, layer, message);
      else
        TeamFoundationTracingService.TraceRaw(tracepoint, level, area, layer, message);
    }
  }
}
