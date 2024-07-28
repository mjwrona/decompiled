// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.PlatformArtifactYamlTriggerServiceTracer
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  public class PlatformArtifactYamlTriggerServiceTracer : IDisposable
  {
    private readonly IVssRequestContext _requestContext;
    private readonly int _tracePoint;
    private Queue<string> _queue = new Queue<string>();

    public PlatformArtifactYamlTriggerServiceTracer(
      IVssRequestContext requestContext,
      int tracePoint)
    {
      this._requestContext = requestContext;
      this._tracePoint = tracePoint;
      this._queue.Enqueue("Trace start.");
    }

    public void AddTrace(string message) => this._queue.Enqueue(message);

    public void Dispose()
    {
      this._queue.Enqueue("Trace completed.");
      this._requestContext.TraceAlways(this._tracePoint, TraceLevel.Info, "Deployment", "PlatformArtifactYamlTriggerService", string.Join("\n", (IEnumerable<string>) this._queue));
    }
  }
}
