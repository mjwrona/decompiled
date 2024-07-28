// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PerformanceTimer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Performance;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct PerformanceTimer : IDisposable
  {
    private const string c_area = "Performance";
    private const string c_layer = "PerformanceTimer";
    private const string c_contextPropertyName = "Context";
    private const int c_maxEntryContextLength = 100;
    internal const int MaxEntriesPerGroup = 50;
    private Stopwatch m_stopwatch;
    private PerformanceTimingEntry m_entry;
    private PerformanceTimingGroup m_timingGroup;

    public long Duration => this.m_entry.ElapsedTicks;

    public void Start(IVssRequestContext requestContext, string groupName)
    {
      try
      {
        this.m_stopwatch = Stopwatch.StartNew();
        if (requestContext == null)
          return;
        this.m_entry.StartOffset = requestContext.RequestTimer.ExecutionTime;
        IDictionary<string, PerformanceTimingGroup> allTimings = PerformanceTimer.GetAllTimings(requestContext);
        if (!allTimings.TryGetValue(groupName, out this.m_timingGroup))
        {
          this.m_timingGroup = new PerformanceTimingGroup();
          allTimings[groupName] = this.m_timingGroup;
        }
        ++this.m_timingGroup.Count;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(419664869, "Performance", nameof (PerformanceTimer), ex);
      }
    }

    public void Invalidate()
    {
      try
      {
        if (this.m_stopwatch == null || !this.m_stopwatch.IsRunning)
          return;
        this.m_stopwatch.Stop();
        if (this.m_timingGroup == null)
          return;
        --this.m_timingGroup.Count;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(229734115, "Performance", nameof (PerformanceTimer), ex);
      }
    }

    public void End()
    {
      try
      {
        if (this.m_stopwatch == null || !this.m_stopwatch.IsRunning)
          return;
        this.m_stopwatch.Stop();
        long ticks = this.m_stopwatch.Elapsed.Ticks;
        this.m_entry.ElapsedTicks = ticks;
        if (this.m_timingGroup == null)
          return;
        this.m_timingGroup.ElapsedTicks += ticks;
        if (this.m_timingGroup.Timings.Count >= 50)
          return;
        this.m_timingGroup.Timings.Add(this.m_entry);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(420269399, "Performance", nameof (PerformanceTimer), ex);
      }
    }

    public void AddProperty(string key, object value)
    {
      if (this.m_entry.Properties == null)
        this.m_entry.Properties = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.Ordinal);
      this.m_entry.Properties[key] = value;
    }

    public void Dispose() => this.End();

    public static PerformanceTimer StartMeasure(
      IVssRequestContext requestContext,
      string groupName,
      string contextPropertyValue = null)
    {
      PerformanceTimer performanceTimer = new PerformanceTimer();
      if (!string.IsNullOrEmpty(contextPropertyValue))
      {
        if (contextPropertyValue.Length > 100)
          contextPropertyValue = contextPropertyValue.Substring(0, 100);
        performanceTimer.AddProperty("Context", (object) contextPropertyValue);
      }
      if (requestContext == null && HttpContext.Current != null)
        requestContext = HttpContext.Current.Items[(object) HttpContextConstants.IVssRequestContext] as IVssRequestContext;
      performanceTimer.Start(requestContext, groupName);
      return performanceTimer;
    }

    public static IDictionary<string, PerformanceTimingGroup> SendCustomerIntelligenceData(
      IVssRequestContext requestContext,
      Action<CustomerIntelligenceData> addCustomProperties = null)
    {
      IDictionary<string, PerformanceTimingGroup> dictionary = (IDictionary<string, PerformanceTimingGroup>) null;
      if (requestContext != null)
      {
        try
        {
          dictionary = PerformanceTimer.GetAllTimings(requestContext);
          if (dictionary.Count > 0)
          {
            CustomerIntelligenceData properties = new CustomerIntelligenceData();
            properties.Add("WebServerTimings", (object) dictionary);
            properties.Add("ActivityId", (object) requestContext.ActivityId);
            requestContext.GetTraceTimingAsString();
            if (addCustomProperties != null)
              addCustomProperties(properties);
            requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Performance", "ServerTimings", properties);
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(599999, "Performance", nameof (PerformanceTimer), ex);
        }
      }
      if (dictionary == null)
        dictionary = (IDictionary<string, PerformanceTimingGroup>) new Dictionary<string, PerformanceTimingGroup>((IEqualityComparer<string>) StringComparer.Ordinal);
      return dictionary;
    }

    public static IDictionary<string, PerformanceTimingGroup> GetAllTimings(
      IVssRequestContext requestContext)
    {
      object allTimings1;
      if (requestContext != null && requestContext.RootContext.Items.TryGetValue("PerformanceTimings", out allTimings1) && allTimings1 is IDictionary<string, PerformanceTimingGroup>)
        return (IDictionary<string, PerformanceTimingGroup>) allTimings1;
      IDictionary<string, PerformanceTimingGroup> allTimings2 = (IDictionary<string, PerformanceTimingGroup>) new Dictionary<string, PerformanceTimingGroup>((IEqualityComparer<string>) StringComparer.Ordinal);
      if (requestContext != null)
        requestContext.RootContext.Items["PerformanceTimings"] = (object) allTimings2;
      return allTimings2;
    }

    public static IDictionary<string, TimingGroup> GetAllTimings(
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      IDictionary<string, TimingGroup> allTimings1 = (IDictionary<string, TimingGroup>) new Dictionary<string, TimingGroup>((IEqualityComparer<string>) StringComparer.Ordinal);
      IDictionary<string, PerformanceTimingGroup> allTimings2 = PerformanceTimer.GetAllTimings(requestContext);
      if (allTimings2 != null)
      {
        foreach (KeyValuePair<string, PerformanceTimingGroup> keyValuePair in (IEnumerable<KeyValuePair<string, PerformanceTimingGroup>>) allTimings2)
        {
          TimingGroup timingGroup = new TimingGroup(securedObject)
          {
            ElapsedTicks = keyValuePair.Value.ElapsedTicks,
            Count = keyValuePair.Value.Count
          };
          foreach (PerformanceTimingEntry timing in keyValuePair.Value.Timings)
          {
            TimingEntry timingEntry = new TimingEntry(securedObject)
            {
              ElapsedTicks = timing.ElapsedTicks,
              StartOffset = timing.StartOffset
            };
            if (timing.Properties != null)
            {
              timingEntry.Properties = (IDictionary<string, object>) new Dictionary<string, object>();
              timing.Properties.Copy<string, object>(timingEntry.Properties);
            }
            timingGroup.Timings.Add(timingEntry);
          }
          allTimings1[keyValuePair.Key] = timingGroup;
        }
      }
      return allTimings1;
    }

    public static WellKnownPerformanceTimings GetWellKnownParsedPerformanceTimings(
      IVssRequestContext requestContext)
    {
      WellKnownPerformanceTimings performanceTimings = new WellKnownPerformanceTimings();
      IDictionary<string, PerformanceTimingGroup> allTimings = PerformanceTimer.GetAllTimings(requestContext);
      PerformanceTimingGroup performanceTimingGroup;
      if (allTimings.TryGetValue("SQL", out performanceTimingGroup))
      {
        performanceTimings.SqlExecutionTime = (int) (performanceTimingGroup.ElapsedTicks / 10000L);
        performanceTimings.SqlExecutionCount = performanceTimingGroup.Count;
        if (allTimings.TryGetValue("FinalSQLCommand", out performanceTimingGroup))
          performanceTimings.FinalSqlCommandExecutionTime = (int) (performanceTimingGroup.ElapsedTicks / 10000L);
        if (allTimings.TryGetValue("SQLRetries", out performanceTimingGroup))
        {
          performanceTimings.SqlRetryExecutionTime = (int) (performanceTimingGroup.ElapsedTicks / 10000L);
          performanceTimings.SqlRetryExecutionCount = performanceTimingGroup.Count;
        }
        if (allTimings.TryGetValue("SQLReadOnly", out performanceTimingGroup))
        {
          performanceTimings.SqlReadOnlyExecutionTime = (int) (performanceTimingGroup.ElapsedTicks / 10000L);
          performanceTimings.SqlReadOnlyExecutionCount = performanceTimingGroup.Count;
        }
      }
      if (allTimings.TryGetValue("Redis", out performanceTimingGroup))
      {
        performanceTimings.RedisExecutionTime = (int) (performanceTimingGroup.ElapsedTicks / 10000L);
        performanceTimings.RedisExecutionCount = performanceTimingGroup.Count;
      }
      if (allTimings.TryGetValue("AadToken", out performanceTimingGroup))
      {
        performanceTimings.AadTokenExecutionTime = (int) (performanceTimingGroup.ElapsedTicks / 10000L);
        performanceTimings.AadTokenExecutionCount = performanceTimingGroup.Count;
      }
      if (allTimings.TryGetValue("AadGraph", out performanceTimingGroup))
      {
        performanceTimings.AadGraphExecutionTime = (int) (performanceTimingGroup.ElapsedTicks / 10000L);
        performanceTimings.AadGraphExecutionCount = performanceTimingGroup.Count;
      }
      if (allTimings.TryGetValue("BlobStorage", out performanceTimingGroup))
      {
        performanceTimings.BlobStorageExecutionTime = (int) (performanceTimingGroup.ElapsedTicks / 10000L);
        performanceTimings.BlobStorageExecutionCount = performanceTimingGroup.Count;
      }
      if (allTimings.TryGetValue("TableStorage", out performanceTimingGroup))
      {
        performanceTimings.TableStorageExecutionTime = (int) (performanceTimingGroup.ElapsedTicks / 10000L);
        performanceTimings.TableStorageExecutionCount = performanceTimingGroup.Count;
      }
      if (allTimings.TryGetValue("ServiceBus", out performanceTimingGroup))
      {
        performanceTimings.ServiceBusExecutionTime = (int) (performanceTimingGroup.ElapsedTicks / 10000L);
        performanceTimings.ServiceBusExecutionCount = performanceTimingGroup.Count;
      }
      if (allTimings.TryGetValue("VssClient", out performanceTimingGroup))
      {
        performanceTimings.VssClientExecutionTime = (int) (performanceTimingGroup.ElapsedTicks / 10000L);
        performanceTimings.VssClientExecutionCount = performanceTimingGroup.Count;
      }
      if (allTimings.TryGetValue("DocumentDB", out performanceTimingGroup))
      {
        performanceTimings.DocDBExecutionTime = (int) (performanceTimingGroup.ElapsedTicks / 10000L);
        performanceTimings.DocDBExecutionCount = performanceTimingGroup.Count;
        performanceTimings.DocDBRUsConsumed = performanceTimingGroup.Timings.Sum<PerformanceTimingEntry>((Func<PerformanceTimingEntry, int>) (x => x.Properties == null || !x.Properties.ContainsKey("DocDBRUsConsumed") ? 0 : (int) x.Properties["DocDBRUsConsumed"]));
      }
      return performanceTimings;
    }

    public static void AggregatePerformanceTimings(
      IVssRequestContext sourceContext,
      IVssRequestContext targetContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(sourceContext, nameof (sourceContext));
      ArgumentUtility.CheckForNull<IVssRequestContext>(targetContext, nameof (targetContext));
      IDictionary<string, PerformanceTimingGroup> allTimings1 = PerformanceTimer.GetAllTimings(sourceContext);
      IDictionary<string, PerformanceTimingGroup> allTimings2 = PerformanceTimer.GetAllTimings(targetContext);
      if (allTimings1 == allTimings2)
        throw new ApplicationException("AggregatePerformanceTimings was called with source's and target's sharing the same timings dictionary.");
      foreach (KeyValuePair<string, PerformanceTimingGroup> keyValuePair in (IEnumerable<KeyValuePair<string, PerformanceTimingGroup>>) allTimings1)
      {
        PerformanceTimingGroup performanceTimingGroup;
        if (!allTimings2.TryGetValue(keyValuePair.Key, out performanceTimingGroup))
        {
          performanceTimingGroup = new PerformanceTimingGroup();
          allTimings2[keyValuePair.Key] = performanceTimingGroup;
        }
        performanceTimingGroup.Count += keyValuePair.Value.Count;
        performanceTimingGroup.ElapsedTicks += keyValuePair.Value.ElapsedTicks;
        int count = 50 - performanceTimingGroup.Timings.Count;
        if (count > 0)
          performanceTimingGroup.Timings.AddRange(keyValuePair.Value.Timings.Take<PerformanceTimingEntry>(count));
      }
    }
  }
}
