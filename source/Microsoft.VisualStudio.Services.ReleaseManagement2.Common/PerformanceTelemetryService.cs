// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.PerformanceTelemetryService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common
{
  public class PerformanceTelemetryService : IVssFrameworkService
  {
    private const string ProbabilityModifierRegistryKey = "/Service/ReleaseManagement/Settings/PerformanceSamplingProbabilityModifier";
    [StaticSafe]
    private static readonly ConcurrentDictionary<Guid, Stack<PerformanceDescriptor>> PerfScenarioTracker = new ConcurrentDictionary<Guid, Stack<PerformanceDescriptor>>();
    [StaticSafe]
    private static float configuredModifier = 1f;
    public const string Area = "Performance";
    public const string Layer = "PerformanceManager";

    private static void PublishEventDelegate(
      IVssRequestContext requestContext,
      string action,
      string feature,
      CustomerIntelligenceData customerIntelligenceData)
    {
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, action, feature, customerIntelligenceData);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions must be caught inorder to prevent them from being thrown")]
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      try
      {
        PerformanceTelemetryService.configuredModifier = systemRequestContext.GetService<IVssRegistryService>().GetValue<float>(systemRequestContext, (RegistryQuery) "/Service/ReleaseManagement/Settings/PerformanceSamplingProbabilityModifier", 1f);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(0, "Performance", "PerformanceManager", ex);
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public static PerformanceDescriptor Measure(
      IVssRequestContext context,
      string layer,
      string actionName)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return context.GetService<PerformanceTelemetryService>().MeasureBlock(context, layer, actionName, publishEvent: PerformanceTelemetryService.\u003C\u003EO.\u003C0\u003E__PublishEventDelegate ?? (PerformanceTelemetryService.\u003C\u003EO.\u003C0\u003E__PublishEventDelegate = new Action<IVssRequestContext, string, string, CustomerIntelligenceData>(PerformanceTelemetryService.PublishEventDelegate)));
    }

    public static PerformanceDescriptor Measure(
      IVssRequestContext context,
      string layer,
      string actionName,
      int samplingRate,
      bool isTopLevelScenario)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return context.GetService<PerformanceTelemetryService>().MeasureBlock(context, layer, actionName, samplingRate, isTopLevelScenario, PerformanceTelemetryService.\u003C\u003EO.\u003C0\u003E__PublishEventDelegate ?? (PerformanceTelemetryService.\u003C\u003EO.\u003C0\u003E__PublishEventDelegate = new Action<IVssRequestContext, string, string, CustomerIntelligenceData>(PerformanceTelemetryService.PublishEventDelegate)));
    }

    public static PerformanceDescriptor Measure(
      IVssRequestContext context,
      string layer,
      string actionName,
      int samplingRate,
      bool isTopLevelScenario,
      Action<IVssRequestContext, string, string, CustomerIntelligenceData> publishEvent)
    {
      return context.GetService<PerformanceTelemetryService>().MeasureBlock(context, layer, actionName, samplingRate, isTopLevelScenario, publishEvent);
    }

    private static bool ShouldTrackPerformance(IVssRequestContext context, int samplingRate)
    {
      int maxValue = (int) ((double) samplingRate * (double) PerformanceTelemetryService.configuredModifier);
      if (maxValue <= 0)
        return false;
      if (maxValue == 1)
        return true;
      Guid empty = Guid.Empty;
      Guid uniqueIdentifier = context.UniqueIdentifier;
      return new Random((!(context.UniqueIdentifier != Guid.Empty) ? context.ActivityId : context.UniqueIdentifier).GetHashCode()).Next(1, maxValue) == 1;
    }

    private static PerformanceDescriptor CreatePerformanceDescriptor(
      IVssRequestContext context,
      string layer,
      string actionName,
      int level,
      Action<IVssRequestContext, string, string, CustomerIntelligenceData> publishEventDelegate = null)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return publishEventDelegate == null ? new PerformanceDescriptor(context, layer, actionName, level, PerformanceTelemetryService.\u003C\u003EO.\u003C0\u003E__PublishEventDelegate ?? (PerformanceTelemetryService.\u003C\u003EO.\u003C0\u003E__PublishEventDelegate = new Action<IVssRequestContext, string, string, CustomerIntelligenceData>(PerformanceTelemetryService.PublishEventDelegate)), PerformanceTelemetryService.\u003C\u003EO.\u003C1\u003E__DescriptorDisposeHandler ?? (PerformanceTelemetryService.\u003C\u003EO.\u003C1\u003E__DescriptorDisposeHandler = new Action<Guid>(PerformanceTelemetryService.DescriptorDisposeHandler))) : new PerformanceDescriptor(context, layer, actionName, level, publishEventDelegate, PerformanceTelemetryService.\u003C\u003EO.\u003C1\u003E__DescriptorDisposeHandler ?? (PerformanceTelemetryService.\u003C\u003EO.\u003C1\u003E__DescriptorDisposeHandler = new Action<Guid>(PerformanceTelemetryService.DescriptorDisposeHandler)));
    }

    private static void DescriptorDisposeHandler(Guid activityId)
    {
      Stack<PerformanceDescriptor> performanceDescriptorStack = PerformanceTelemetryService.PerfScenarioTracker[activityId];
      performanceDescriptorStack.Pop();
      if (performanceDescriptorStack.Count > 0)
        return;
      PerformanceTelemetryService.PerfScenarioTracker.TryRemove(activityId, out performanceDescriptorStack);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Not a static member")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions must be caught inorder to prevent them from being thrown")]
    private PerformanceDescriptor MeasureBlock(
      IVssRequestContext context,
      string layer,
      string actionName,
      int samplingRate = 1,
      bool isTopLevelScenario = false,
      Action<IVssRequestContext, string, string, CustomerIntelligenceData> publishEvent = null)
    {
      PerformanceDescriptor performanceDescriptor = (PerformanceDescriptor) null;
      try
      {
        if (isTopLevelScenario && PerformanceTelemetryService.ShouldTrackPerformance(context, samplingRate))
          PerformanceTelemetryService.PerfScenarioTracker.TryAdd(context.ActivityId, new Stack<PerformanceDescriptor>());
        Stack<PerformanceDescriptor> performanceDescriptorStack = (Stack<PerformanceDescriptor>) null;
        if (!PerformanceTelemetryService.PerfScenarioTracker.TryGetValue(context.ActivityId, out performanceDescriptorStack))
          return PerformanceDescriptor.Empty;
        performanceDescriptor = PerformanceTelemetryService.CreatePerformanceDescriptor(context, layer, actionName, performanceDescriptorStack.Count, publishEvent);
        performanceDescriptorStack.Push(performanceDescriptor);
        return performanceDescriptor;
      }
      catch (Exception ex)
      {
        performanceDescriptor?.Dispose();
        context.TraceException(0, "Performance", layer, ex);
        return PerformanceDescriptor.Empty;
      }
    }
  }
}
