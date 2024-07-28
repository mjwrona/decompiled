// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CommandService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class CommandService : Command
  {
    private readonly Guid m_e2eId;
    private readonly Guid m_uniqueIdentifier;
    private readonly string m_orchestrationId;

    public CommandService(
      IVssRequestContext requestContext,
      CommandGroupKey group,
      Action run = null,
      Action fallback = null)
      : this(requestContext, new CommandSetter(group), run, fallback)
    {
    }

    public CommandService(
      IVssRequestContext requestContext,
      CommandSetter setter,
      Action run = null,
      Action fallback = null)
      : this(requestContext, setter.GroupKey, setter.CommandKey, (ICircuitBreaker) null, (ICommandProperties) new CommandPropertiesRegistry(requestContext, setter.CommandKey, setter.CommandPropertiesDefaults), (CommandMetrics) null, (IEventNotifier) EventNotifierPerformanceCounters.Instance, run, fallback)
    {
    }

    public CommandService(
      IVssRequestContext requestContext,
      CommandSetter setter,
      ICommandProperties properties,
      Action run = null,
      Action fallback = null)
      : this(requestContext, setter.GroupKey, setter.CommandKey, (ICircuitBreaker) null, properties, (CommandMetrics) null, (IEventNotifier) EventNotifierPerformanceCounters.Instance, run, fallback)
    {
    }

    private CommandService(
      IVssRequestContext requestContext,
      CommandGroupKey group,
      CommandKey key,
      ICircuitBreaker circuitBreaker,
      ICommandProperties properties,
      CommandMetrics metrics,
      IEventNotifier eventNotifier,
      Action run = null,
      Action fallback = null)
      : base(group, key, circuitBreaker, properties, metrics, eventNotifier, run, fallback)
    {
      this.m_e2eId = requestContext.E2EId;
      this.m_orchestrationId = requestContext.OrchestrationId;
      this.m_uniqueIdentifier = requestContext.UniqueIdentifier;
    }

    protected override void Trace(
      int tracepoint,
      TraceLevel level,
      string featurearea,
      string classname,
      string message)
    {
      TeamFoundationTracingService.TraceRaw(tracepoint, level, featurearea, classname, this.m_e2eId, this.m_orchestrationId, this.m_uniqueIdentifier, message);
    }

    protected override void TraceConditionally(
      int tracepoint,
      TraceLevel level,
      string featurearea,
      string classname,
      Func<string> message)
    {
      TeamFoundationTracingService.TraceRawConditionally(tracepoint, level, featurearea, classname, this.m_e2eId, this.m_orchestrationId, this.m_uniqueIdentifier, message);
    }
  }
}
