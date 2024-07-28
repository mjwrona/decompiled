// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.EventNotifierPerformanceCounters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class EventNotifierPerformanceCounters : IEventNotifier
  {
    private static readonly EventNotifierPerformanceCounters instance = new EventNotifierPerformanceCounters();

    public static EventNotifierPerformanceCounters Instance => EventNotifierPerformanceCounters.instance;

    protected EventNotifierPerformanceCounters()
    {
    }

    public virtual void MarkEvent(CommandGroupKey group, CommandKey key, EventType eventType)
    {
      switch (eventType)
      {
        case EventType.Success:
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerSuccessPerSec", group.Name + "." + key.Name).Increment();
          break;
        case EventType.Failure:
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFailurePerSec", group.Name + "." + key.Name).Increment();
          break;
        case EventType.Timeout:
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerTimeoutPerSec", group.Name + "." + key.Name).Increment();
          break;
        case EventType.ShortCircuited:
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerShortCircuitedPerSec", group.Name + "." + key.Name).Increment();
          break;
        case EventType.ConcurrencyRejected:
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerConcurrencyRejectedPerSec", group.Name + "." + key.Name).Increment();
          break;
        case EventType.LimitRejected:
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerLimitRejectedPerSec", group.Name + "." + key.Name).Increment();
          break;
        case EventType.FallbackSuccess:
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFallbackSuccessPerSec", group.Name + "." + key.Name).Increment();
          break;
        case EventType.FallbackFailure:
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFallbackFailurePerSec", group.Name + "." + key.Name).Increment();
          break;
        case EventType.FallbackConcurrencyRejected:
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFallbackConcurrencyRejectedPerSec", group.Name + "." + key.Name).Increment();
          break;
        case EventType.FallbackLimitRejected:
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFallbackLimitRejectedPerSec", group.Name + "." + key.Name).Increment();
          break;
      }
    }

    public virtual void MarkCommandExecution(
      CommandGroupKey group,
      CommandKey key,
      long elapsedTimeInMilliseconds)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerCommandExecutionTime", group.Name + "." + key.Name).SetValue(elapsedTimeInMilliseconds);
    }

    public virtual void MarkExecutionConcurrency(
      CommandGroupKey group,
      CommandKey key,
      long executionSemaphoreNumberOfPermitsUsed)
    {
      if (executionSemaphoreNumberOfPermitsUsed >= 0L)
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerExecutionConcurrency", group.Name + "." + key.Name).SetValue(executionSemaphoreNumberOfPermitsUsed);
      else
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerMaxExecutionConcurrency", group.Name + "." + key.Name).SetValue(executionSemaphoreNumberOfPermitsUsed * -1L);
    }

    public virtual void MarkFallbackConcurrency(
      CommandGroupKey group,
      CommandKey key,
      long fallbackSemaphoreNumberOfPermitsUsed)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFallbackConcurrency", group.Name + "." + key.Name).SetValue(fallbackSemaphoreNumberOfPermitsUsed);
    }

    public virtual void MarkExecutionCount(
      CommandGroupKey group,
      CommandKey key,
      long executionCount)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerExecutionCount", group.Name + "." + key.Name).SetValue(executionCount);
    }

    public virtual void MarkFallbackCount(
      CommandGroupKey group,
      CommandKey key,
      long fallbackCount)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFallbackCount", group.Name + "." + key.Name).SetValue(fallbackCount);
    }

    public virtual void TraceRaw(
      int tracepoint,
      TraceLevel level,
      string featurearea,
      string classname,
      string message)
    {
      TeamFoundationTracingService.TraceRaw(tracepoint, level, featurearea, classname, message);
    }

    public virtual void TraceRawConditionally(
      int tracepoint,
      TraceLevel level,
      string featurearea,
      string classname,
      Func<string> message)
    {
      TeamFoundationTracingService.TraceRawConditionally(tracepoint, level, featurearea, classname, message);
    }
  }
}
