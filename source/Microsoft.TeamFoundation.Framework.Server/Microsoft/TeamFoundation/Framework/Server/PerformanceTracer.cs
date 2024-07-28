// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PerformanceTracer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class PerformanceTracer : TimedActionTracer
  {
    private StandardPerformanceCounterSet m_counters;

    public PerformanceTracer(StandardPerformanceCounterSet counterSet, string area, string layer)
      : base(area, layer)
    {
      this.m_counters = counterSet;
    }

    protected override void OnCacheHit(string actionName)
    {
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.TotalCacheHits, actionName).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.CacheHitsPerSec, actionName).Increment();
    }

    protected override void OnCacheMiss(string actionName)
    {
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.TotalCacheMisses, actionName).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.CacheMissesPerSec, actionName).Increment();
    }

    protected override void OnCacheInvalidation(string actionName)
    {
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.TotalCacheInvalidations, actionName).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.CacheInvalidationsPerSec, actionName).Increment();
    }

    protected override void OnBeginAction(string actionName)
    {
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.TotalCalls, actionName).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.CallsPerSec, actionName).Increment();
    }

    protected override void OnSlowAction(string actionName)
    {
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.TotalSlowCalls, actionName).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.SlowCallsPerSec, actionName).Increment();
    }

    protected override void OnEndAction(string acionName, long timeTakenMilliSeconds)
    {
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.AverageCallTime, acionName).IncrementMilliseconds(timeTakenMilliSeconds);
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.AverageCallTimeBase, acionName).Increment();
    }

    protected override void OnException(string actionName)
    {
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.TotalExceptions, actionName).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.ExceptionsPerSec, actionName).Increment();
    }

    protected override void OnError(string actionName)
    {
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.TotalFailedOperations, actionName).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter(this.m_counters.FailedOperationsPerSec, actionName).Increment();
    }
  }
}
