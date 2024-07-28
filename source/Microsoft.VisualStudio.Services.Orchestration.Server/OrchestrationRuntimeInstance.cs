// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationRuntimeInstance
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Orchestration.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationRuntimeInstance : IDisposable
  {
    private bool m_disposed;
    private OrchestrationTraceListener m_traceListener;

    public OrchestrationRuntimeInstance(
      IVssRequestContext requestContext,
      OrchestrationHubDescription description,
      OrchestrationRuntime runtime,
      IDisposableReadOnlyList<IOrchestrationRuntimeExtension> extensions)
    {
      this.Description = description;
      this.Extensions = extensions;
      this.RequestContext = requestContext;
      this.Runtime = runtime;
      this.m_traceListener = new OrchestrationTraceListener(requestContext);
      this.m_traceListener.GeneralEvent = new Action<TraceEventType, string>(this.OnGeneralEvent);
      this.m_traceListener.InstanceEvent = new Action<TraceEventType, string, string, string>(this.OnInstanceEvent);
      this.m_traceListener.SessionEvent = new Action<TraceEventType, string, string>(this.OnSessionEvent);
      this.Runtime.TraceSource.Listeners.Add((TraceListener) this.m_traceListener);
    }

    private void OnSessionEvent(TraceEventType eventType, string sessionId, string message) => this.RequestContext.Trace(0, Utilities.ToTraceLevel(eventType), "Orchestration", "Dispatcher", "{0}: {1}", (object) sessionId, (object) message);

    private void OnInstanceEvent(
      TraceEventType eventType,
      string instanceId,
      string executionId,
      string message)
    {
      this.RequestContext.Trace(0, Utilities.ToTraceLevel(eventType), "Orchestration", "Dispatcher", "{0} ({1}): {2}", (object) instanceId, (object) executionId, (object) message);
    }

    private void OnGeneralEvent(TraceEventType eventType, string message) => this.RequestContext.Trace(0, Utilities.ToTraceLevel(eventType), "Orchestration", "Dispatcher", message);

    public OrchestrationHubDescription Description { get; private set; }

    public IDisposableReadOnlyList<IOrchestrationRuntimeExtension> Extensions { get; private set; }

    public IVssRequestContext RequestContext { get; private set; }

    public OrchestrationRuntime Runtime { get; private set; }

    public void Dispose() => this.Dispose(true);

    private void Dispose(bool disposing)
    {
      if (this.m_disposed || !disposing)
        return;
      if (this.Extensions != null)
      {
        foreach (IOrchestrationRuntimeExtension extension in (IEnumerable<IOrchestrationRuntimeExtension>) this.Extensions)
        {
          try
          {
            extension.OnStop(this.RequestContext, this.Description, this.Runtime);
          }
          catch (Exception ex)
          {
            this.RequestContext.Trace(0, TraceLevel.Error, "Orchestration", "Dispatcher", "Error encountered will stopping extension {0}: {1}", (object) extension.GetType().FullName, (object) ex.ToReadableStackTrace());
          }
        }
        this.Extensions.Dispose();
        this.Extensions = (IDisposableReadOnlyList<IOrchestrationRuntimeExtension>) null;
      }
      if (this.Runtime != null)
      {
        this.Runtime.Dispose();
        this.Runtime = (OrchestrationRuntime) null;
      }
      if (this.m_traceListener != null)
      {
        this.m_traceListener.GeneralEvent = (Action<TraceEventType, string>) null;
        this.m_traceListener.InstanceEvent = (Action<TraceEventType, string, string, string>) null;
        this.m_traceListener.SessionEvent = (Action<TraceEventType, string, string>) null;
        this.m_traceListener.Dispose();
        this.m_traceListener = (OrchestrationTraceListener) null;
      }
      this.m_disposed = true;
    }

    public T GetObjectFromMessage<T>(OrchestrationMessage message) => Utilities.GetObjectFromMessage<T>(this.Runtime.Serializer, message);

    public OrchestrationMessage GetMessageFromObject(TaskMessage messageObject) => Utilities.GetMessageFromObject(this.Description, this.Runtime.Serializer, (string) null, messageObject);

    public OrchestrationMessage GetMessageFromObject(string sessionId, TaskMessage messageObject) => Utilities.GetMessageFromObject(this.Description, this.Runtime.Serializer, sessionId, messageObject);
  }
}
