// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PerfManager
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class PerfManager : IVssFrameworkService
  {
    internal static readonly string Area = "Performance";
    internal static readonly string Feature = "LayerMap";
    internal static readonly string Layer = nameof (PerfManager);
    internal static readonly string PerformanceTracingFeatureFlag = "TestManagement.Tracing.Performance";
    private const string c_probabilityModifierRegistryKey = "/Service/TestManagement/Settings/PerformanceSamplingProbabilityModifier";
    private float m_configuredModifier = 1f;
    private ConcurrentDictionary<Guid, Stack<PerfDescriptor>> m_perfScenarioTracker = new ConcurrentDictionary<Guid, Stack<PerfDescriptor>>();

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      try
      {
        this.m_configuredModifier = systemRequestContext.GetService<IVssRegistryService>().GetValue<float>(systemRequestContext, (RegistryQuery) "/Service/TestManagement/Settings/PerformanceSamplingProbabilityModifier", 1f);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(PerfManager.Layer, ex);
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public static PerfDescriptor Measure(
      IVssRequestContext context,
      string layer,
      string actionName,
      int samplingRate = 1,
      bool isTopLevelScenario = false,
      Action<CustomerIntelligenceData> publishDurationDelegate = null,
      long duration = 0)
    {
      return context.GetService<PerfManager>().MeasureBlock(context, layer, actionName, samplingRate, isTopLevelScenario, publishDurationDelegate, duration);
    }

    public PerfDescriptor MeasureBlock(
      IVssRequestContext context,
      string layer,
      string actionName,
      int samplingRate = 1,
      bool isTopLevelScenario = false,
      Action<CustomerIntelligenceData> publishDurationDelegate = null,
      long duration = 0)
    {
      try
      {
        if (!context.IsFeatureEnabled(PerfManager.PerformanceTracingFeatureFlag))
          return PerfDescriptor.Empty;
        if (isTopLevelScenario && this.ShouldTrackPerformance(context, samplingRate))
          this.m_perfScenarioTracker.TryAdd(context.ActivityId, new Stack<PerfDescriptor>());
        Stack<PerfDescriptor> perfDescriptorStack = (Stack<PerfDescriptor>) null;
        if (!this.m_perfScenarioTracker.TryGetValue(context.ActivityId, out perfDescriptorStack))
          return PerfDescriptor.Empty;
        PerfDescriptor perfDescriptor = new PerfDescriptor(context, layer, actionName, perfDescriptorStack.Count, new Action<long, Guid>(this.ElapsedTimeHandler), publishDurationDelegate, duration);
        perfDescriptorStack.Push(perfDescriptor);
        return perfDescriptor;
      }
      catch (Exception ex)
      {
        context.TraceException(PerfManager.Layer, ex);
        return PerfDescriptor.Empty;
      }
    }

    private void ElapsedTimeHandler(long elapsedTime, Guid activityId)
    {
      Stack<PerfDescriptor> perfDescriptorStack = this.m_perfScenarioTracker[activityId];
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
      if (maxValue == 1)
        return true;
      Guid empty = Guid.Empty;
      Guid uniqueIdentifier = context.UniqueIdentifier;
      return new Random((!(context.UniqueIdentifier != Guid.Empty) ? context.ActivityId : context.UniqueIdentifier).GetHashCode()).Next(1, maxValue) == 1;
    }
  }
}
