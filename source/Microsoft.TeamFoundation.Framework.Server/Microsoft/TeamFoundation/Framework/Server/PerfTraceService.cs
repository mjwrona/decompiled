// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PerfTraceService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class PerfTraceService : IPerfTraceService, IVssFrameworkService
  {
    internal static readonly string Area = "Performance";
    internal static readonly string Feature = "LayerMap";
    internal static readonly string Layer = "PerfManager";
    internal static readonly string PerformanceTracingFeatureFlag = "VisualStudio.Services.Tracing.Performance";
    private const string c_probabilityModifierRegistryKey = "/Service/PerfTraceManager/Settings/PerformanceSamplingProbabilityModifier";
    private float m_configuredModifier = 1f;
    private ConcurrentDictionary<Guid, Stack<PerfTraceService.PerfDescriptor>> m_perfScenarioTracker = new ConcurrentDictionary<Guid, Stack<PerfTraceService.PerfDescriptor>>();
    private IPerfTraceStopWatchFactory m_stopWatchCreator;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_configuredModifier = systemRequestContext.GetService<IVssRegistryService>().GetValue<float>(systemRequestContext, (RegistryQuery) "/Service/PerfTraceManager/Settings/PerformanceSamplingProbabilityModifier", 1f);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public T MeasureTopLevelScenario<T>(
      IVssRequestContext requestContext,
      string layer,
      string actionName,
      int samplingRate,
      Func<T> action)
    {
      return this.MeasureBlockInternal<T>(requestContext, layer, actionName, action, samplingRate, true);
    }

    public void MeasureTopLevelScenario(
      IVssRequestContext requestContext,
      string layer,
      string actionName,
      int samplingRate,
      Action action)
    {
      this.MeasureTopLevelScenario<object>(requestContext, layer, actionName, samplingRate, (Func<object>) (() =>
      {
        action();
        return (object) null;
      }));
    }

    public T Measure<T>(
      IVssRequestContext requestContext,
      string layer,
      string actionName,
      Func<T> action)
    {
      return this.MeasureBlockInternal<T>(requestContext, layer, actionName, action, 0, false);
    }

    public void Measure(
      IVssRequestContext requestContext,
      string layer,
      string actionName,
      Action action)
    {
      this.Measure<object>(requestContext, layer, actionName, (Func<object>) (() =>
      {
        action();
        return (object) null;
      }));
    }

    internal PerfTraceService()
      : this((IPerfTraceStopWatchFactory) null)
    {
    }

    internal PerfTraceService(IPerfTraceStopWatchFactory stopWatchCreator)
    {
      if (stopWatchCreator == null)
        this.m_stopWatchCreator = (IPerfTraceStopWatchFactory) new PerfTraceStopWatchFactory();
      else
        this.m_stopWatchCreator = stopWatchCreator;
    }

    private T MeasureBlockInternal<T>(
      IVssRequestContext requestContext,
      string layer,
      string actionName,
      Func<T> action,
      int samplingRate,
      bool isTopLevelScenario)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return this.MeasureBlockInternal<T>(requestContext.To(TeamFoundationHostType.Deployment), layer, actionName, action, samplingRate, isTopLevelScenario);
      bool flag = false;
      Stack<PerfTraceService.PerfDescriptor> perfDescriptorStack = (Stack<PerfTraceService.PerfDescriptor>) null;
      if (requestContext.IsFeatureEnabled(PerfTraceService.PerformanceTracingFeatureFlag))
      {
        if (isTopLevelScenario && this.ShouldTrackPerformance(requestContext, samplingRate))
          this.m_perfScenarioTracker.TryAdd(requestContext.ActivityId, new Stack<PerfTraceService.PerfDescriptor>());
        if (this.m_perfScenarioTracker.TryGetValue(requestContext.ActivityId, out perfDescriptorStack))
          flag = true;
      }
      if (!flag)
        return action();
      using (PerfTraceService.PerfDescriptor perfDescriptor = new PerfTraceService.PerfDescriptor(requestContext, layer, actionName, perfDescriptorStack.Count, new Action<long, Guid>(this.ElapsedTimeHandler), this.m_stopWatchCreator.Create()))
      {
        perfDescriptorStack.Push(perfDescriptor);
        return action();
      }
    }

    private void ElapsedTimeHandler(long elapsedTime, Guid activityId)
    {
      Stack<PerfTraceService.PerfDescriptor> perfDescriptorStack = this.m_perfScenarioTracker[activityId];
      perfDescriptorStack.Pop();
      if (perfDescriptorStack.Count > 0)
        perfDescriptorStack.Peek().AddTimeInDescendant(elapsedTime);
      else
        this.m_perfScenarioTracker.TryRemove(activityId, out perfDescriptorStack);
    }

    private bool ShouldTrackPerformance(IVssRequestContext context, int samplingRate)
    {
      int maxValue = (int) ((double) samplingRate * (double) this.m_configuredModifier);
      if (maxValue <= 0)
        return false;
      return maxValue == 1 || new Random(context.ActivityId.GetHashCode()).Next(1, maxValue) == 1;
    }

    private class PerfDescriptor : IDisposable
    {
      private IVssRequestContext m_context;
      private string m_layer;
      private string m_actionName;
      private int m_level;
      private IPerfTraceStopWatch m_stopWatch;
      private Action<long, Guid> m_elapsedTimeDelegate;
      private long m_timeSpentInDescendants;
      private long m_inclusiveTimeInMs;
      private long m_exclusiveTimeInMs;

      internal PerfDescriptor(
        IVssRequestContext requestContext,
        string layer,
        string actionName,
        int level,
        Action<long, Guid> elapsedTimeDelegate,
        IPerfTraceStopWatch stopWatch)
      {
        this.m_context = requestContext;
        this.m_layer = layer;
        this.m_actionName = actionName;
        this.m_level = level;
        this.m_elapsedTimeDelegate = elapsedTimeDelegate;
        this.m_stopWatch = stopWatch;
        this.m_stopWatch.Start();
      }

      internal void AddTimeInDescendant(long duration) => this.m_timeSpentInDescendants += duration;

      void IDisposable.Dispose()
      {
        try
        {
          this.m_stopWatch.Stop();
          this.m_inclusiveTimeInMs = this.m_stopWatch.ElapsedMilliseconds;
          this.m_exclusiveTimeInMs = this.m_inclusiveTimeInMs - this.m_timeSpentInDescendants;
          if (this.m_elapsedTimeDelegate != null)
            this.m_elapsedTimeDelegate(this.m_inclusiveTimeInMs, this.m_context.ActivityId);
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("Layer", this.m_layer);
          properties.Add("ActionName", this.m_actionName);
          properties.Add("Level", (double) this.m_level);
          properties.Add("ActivityId", (object) this.m_context.ActivityId);
          properties.Add("UniqueIdentifier", (object) this.m_context.UniqueIdentifier);
          properties.Add("InclusiveTimeMs", (double) this.m_inclusiveTimeInMs);
          properties.Add("ExclusiveTimeMs", (double) this.m_exclusiveTimeInMs);
          this.m_context.GetService<CustomerIntelligenceService>().Publish(this.m_context, PerfTraceService.Area, PerfTraceService.Layer, properties);
        }
        catch (Exception ex)
        {
          this.m_context.TraceException(6300000, PerfTraceService.Area, PerfTraceService.Layer, ex);
        }
      }
    }
  }
}
