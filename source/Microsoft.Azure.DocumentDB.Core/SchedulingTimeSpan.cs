// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SchedulingTimeSpan
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  internal struct SchedulingTimeSpan
  {
    private readonly TimeSpan turnaroundTime;
    private readonly TimeSpan responseTime;
    private readonly TimeSpan runTime;
    private readonly TimeSpan waitTime;
    private readonly long numPreemptions;

    public SchedulingTimeSpan(
      TimeSpan turnaroundTime,
      TimeSpan responseTime,
      TimeSpan runTime,
      TimeSpan waitTime,
      long numPreemptions)
    {
      this.turnaroundTime = turnaroundTime;
      this.responseTime = responseTime;
      this.runTime = runTime;
      this.waitTime = waitTime;
      this.numPreemptions = numPreemptions;
    }

    public long NumPreemptions => this.numPreemptions;

    public TimeSpan TurnaroundTime => this.turnaroundTime;

    public TimeSpan ResponseTime => this.responseTime;

    public TimeSpan RunTime => this.runTime;

    public TimeSpan WaitTime => this.waitTime;

    public static TimeSpan GetAverageTurnaroundTime(
      IEnumerable<SchedulingTimeSpan> schedulingTimeSpans)
    {
      return SchedulingTimeSpan.GetAverageTime(schedulingTimeSpans, (Func<SchedulingTimeSpan, long>) (schedulingMetric => schedulingMetric.TurnaroundTime.Ticks));
    }

    public static TimeSpan GetAverageResponseTime(
      IEnumerable<SchedulingTimeSpan> schedulingTimeSpans)
    {
      return SchedulingTimeSpan.GetAverageTime(schedulingTimeSpans, (Func<SchedulingTimeSpan, long>) (schedulingMetric => schedulingMetric.ResponseTime.Ticks));
    }

    public static TimeSpan GetAverageRunTime(
      IEnumerable<SchedulingTimeSpan> schedulingTimeSpans)
    {
      return SchedulingTimeSpan.GetAverageTime(schedulingTimeSpans, (Func<SchedulingTimeSpan, long>) (schedulingMetric => schedulingMetric.RunTime.Ticks));
    }

    public static double GetThroughput(
      IEnumerable<SchedulingTimeSpan> schedulingTimeSpans)
    {
      TimeSpan timeSpan = new TimeSpan(schedulingTimeSpans.Sum<SchedulingTimeSpan>((Func<SchedulingTimeSpan, long>) (schedulingMetric => schedulingMetric.TurnaroundTime.Ticks)));
      return (double) schedulingTimeSpans.Count<SchedulingTimeSpan>() / timeSpan.TotalSeconds;
    }

    public static double GetCpuUtilization(
      IEnumerable<SchedulingTimeSpan> schedulingTimeSpans)
    {
      long num = schedulingTimeSpans.Max<SchedulingTimeSpan>((Func<SchedulingTimeSpan, long>) (schedulingMetric => schedulingMetric.TurnaroundTime.Ticks));
      return (double) Convert.ToInt64(schedulingTimeSpans.Sum<SchedulingTimeSpan>((Func<SchedulingTimeSpan, long>) (schedulingMetric => schedulingMetric.RunTime.Ticks))) / (double) num;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Turnaround Time: {0}ms, Response Time: {1}ms, Run Time: {2}ms, Wait Time: {3}ms, Number Of Preemptions: {4}", (object) this.TurnaroundTime.TotalMilliseconds, (object) this.ResponseTime.TotalMilliseconds, (object) this.RunTime.TotalMilliseconds, (object) this.WaitTime.TotalMilliseconds, (object) this.NumPreemptions);

    private static TimeSpan GetAverageTime(
      IEnumerable<SchedulingTimeSpan> schedulingTimeSpans,
      Func<SchedulingTimeSpan, long> propertySelectorCallback)
    {
      if (schedulingTimeSpans == null)
        throw new ArgumentNullException(nameof (schedulingTimeSpans));
      return schedulingTimeSpans.Count<SchedulingTimeSpan>() != 0 ? new TimeSpan(Convert.ToInt64(schedulingTimeSpans.Average<SchedulingTimeSpan>(propertySelectorCallback))) : throw new ArgumentException("schedulingMetricsResults has no items.");
    }
  }
}
