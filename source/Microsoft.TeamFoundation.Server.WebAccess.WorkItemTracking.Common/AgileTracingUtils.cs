// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.AgileTracingUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class AgileTracingUtils
  {
    private const string c_traceArea = "Agile";
    private static readonly string c_traceLayer = TfsTraceLayers.BusinessLogic;

    public virtual IDisposable TraceBlock(
      IVssRequestContext context,
      int start,
      int end,
      string layer = null,
      [CallerMemberName] string memberName = "")
    {
      if (layer == null)
        layer = AgileTracingUtils.c_traceLayer;
      return context.TraceBlock(start, end, "Agile", layer, memberName);
    }

    public virtual void TraceAction(
      IVssRequestContext context,
      int start,
      int end,
      Action action,
      [CallerMemberName] string memberName = "")
    {
      context.TraceBlock(start, end, "Agile", AgileTracingUtils.c_traceLayer, memberName, action);
    }

    public virtual void TraceException(
      IVssRequestContext context,
      int tracePoint,
      Exception ex,
      string layer = null)
    {
      if (layer == null)
        layer = AgileTracingUtils.c_traceLayer;
      context.TraceException(tracePoint, "Agile", layer, ex);
    }

    public virtual void TraceWarning(
      IVssRequestContext context,
      int tracePoint,
      string message,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context, tracePoint, TraceLevel.Warning, "Agile", AgileTracingUtils.c_traceLayer, message, args);
    }
  }
}
