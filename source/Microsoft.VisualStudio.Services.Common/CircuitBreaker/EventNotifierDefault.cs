// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.EventNotifierDefault
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class EventNotifierDefault : IEventNotifier
  {
    private static EventNotifierDefault instance = new EventNotifierDefault();

    public static EventNotifierDefault Instance => EventNotifierDefault.instance;

    protected EventNotifierDefault()
    {
    }

    public virtual void MarkEvent(CommandGroupKey group, CommandKey key, EventType eventType)
    {
    }

    public virtual void MarkCommandExecution(
      CommandGroupKey group,
      CommandKey key,
      long elapsedTimeInMilliseconds)
    {
    }

    public virtual void MarkExecutionConcurrency(
      CommandGroupKey group,
      CommandKey key,
      long executionSemaphoreNumberOfPermitsUsed)
    {
    }

    public virtual void MarkFallbackConcurrency(
      CommandGroupKey group,
      CommandKey key,
      long fallbackSemaphoreNumberOfPermitsUsed)
    {
    }

    public void MarkExecutionCount(CommandGroupKey group, CommandKey key, long executionCount)
    {
    }

    public void MarkFallbackCount(CommandGroupKey group, CommandKey key, long fallbackCount)
    {
    }

    public virtual void TraceRaw(
      int tracepoint,
      TraceLevel level,
      string featurearea,
      string classname,
      string message)
    {
    }

    public virtual void TraceRawConditionally(
      int tracepoint,
      TraceLevel level,
      string featurearea,
      string classname,
      Func<string> message)
    {
    }
  }
}
