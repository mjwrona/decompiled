// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.GitPushYamlTriggerTracer
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class GitPushYamlTriggerTracer : IDisposable
  {
    private readonly IVssRequestContext _requestContext;
    private readonly int _tracePoint;
    private readonly string _traceLayer;
    private readonly string _traceArea;
    private readonly Queue<string> _infoQueue = new Queue<string>();

    public GitPushYamlTriggerTracer(
      IVssRequestContext requestContext,
      int tracePoint,
      string traceLayer,
      string traceArea = "Build2")
    {
      this._requestContext = requestContext;
      this._tracePoint = tracePoint;
      this._traceLayer = traceLayer;
      this._traceArea = traceArea;
      this._infoQueue.Enqueue("Trace start");
    }

    public void TraceInfo(string message) => this._infoQueue.Enqueue(message);

    public void TraceAlways(string message, int? tracepoint = null)
    {
      this._requestContext.TraceAlways(tracepoint ?? this._tracePoint, TraceLevel.Info, this._traceArea, this._traceLayer, message);
      this._infoQueue.Enqueue(message);
    }

    public void TraceException(Exception ex, int tracepoint)
    {
      this._requestContext.TraceException(tracepoint, this._traceArea, this._traceLayer, ex);
      this._infoQueue.Enqueue(ex.Message);
    }

    public void Dispose()
    {
      this._infoQueue.Enqueue("Trace completed");
      this._requestContext.TraceAlways(this._tracePoint, TraceLevel.Info, this._traceArea, this._traceLayer, string.Join("\n", (IEnumerable<string>) this._infoQueue));
    }
  }
}
