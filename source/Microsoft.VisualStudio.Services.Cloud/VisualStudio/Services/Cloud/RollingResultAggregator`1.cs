// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RollingResultAggregator`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RollingResultAggregator<T>
  {
    private ConcurrentDictionary<DateTime, IDictionary<T, int>> m_intervals;
    private IDictionary<T, int> m_resultTotals;
    private readonly int m_numIntervals;
    private readonly object m_sourceData;
    private object m_lock = new object();

    public event RollingResultAggregator<T>.IntervalProcessor OnInterval;

    public static RollingResultAggregator<T> CreateInstance(int numIntervals, object sourceData) => new RollingResultAggregator<T>(numIntervals, sourceData);

    protected RollingResultAggregator(int numIntervals, object sourceData)
    {
      this.m_intervals = new ConcurrentDictionary<DateTime, IDictionary<T, int>>();
      this.m_resultTotals = (IDictionary<T, int>) new Dictionary<T, int>();
      this.m_numIntervals = numIntervals;
      this.m_sourceData = sourceData;
    }

    public virtual void LogResult(IVssRequestContext requestContext, T result)
    {
      DateTime intervalStartTime = this.GetIntervalStartTime(DateTime.UtcNow);
      IDictionary<T, int> intervalCounts = this.GetIntervalCounts(requestContext, intervalStartTime);
      lock (this.m_lock)
      {
        int num1 = 0;
        intervalCounts.TryGetValue(result, out num1);
        intervalCounts[result] = num1 + 1;
        int num2 = 0;
        this.m_resultTotals.TryGetValue(result, out num2);
        this.m_resultTotals[result] = num2 + 1;
      }
    }

    private DateTime GetIntervalStartTime(DateTime time) => new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0, time.Kind);

    private IDictionary<T, int> GetIntervalCounts(
      IVssRequestContext requestContext,
      DateTime intervalStartTime)
    {
      bool created = false;
      IDictionary<T, int> intervalCounts = this.m_intervals.GetOrAdd(intervalStartTime, (Func<DateTime, IDictionary<T, int>>) (newInterval =>
      {
        created = true;
        return (IDictionary<T, int>) new Dictionary<T, int>();
      }));
      if (created)
      {
        IDictionary<T, int> interval = this.m_intervals[intervalStartTime];
        if (intervalCounts == interval)
        {
          TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.ProcessInterval), (object) intervalStartTime, intervalStartTime.AddMinutes((double) this.m_numIntervals), 0);
          requestContext.GetService<ITeamFoundationTaskService>().AddTask(requestContext, task);
        }
        else
          intervalCounts = interval;
      }
      return intervalCounts;
    }

    private void ProcessInterval(IVssRequestContext requestContext, object taskArgs)
    {
      IDictionary<T, int> dictionary;
      if (this.OnInterval == null || !this.m_intervals.TryRemove((DateTime) taskArgs, out dictionary))
        return;
      Dictionary<T, Tuple<int, int>> resultCounts = new Dictionary<T, Tuple<int, int>>();
      lock (this.m_lock)
      {
        foreach (T key in (IEnumerable<T>) dictionary.Keys)
        {
          resultCounts[key] = new Tuple<int, int>(dictionary[key], this.m_resultTotals[key]);
          this.m_resultTotals[key] -= dictionary[key];
        }
      }
      this.OnInterval(requestContext, (IDictionary<T, Tuple<int, int>>) resultCounts, this.m_sourceData);
    }

    public delegate void IntervalProcessor(
      IVssRequestContext requestContext,
      IDictionary<T, Tuple<int, int>> resultCounts,
      object sourceData);
  }
}
