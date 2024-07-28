// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.SchedulingTimeSpan
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal readonly struct SchedulingTimeSpan
  {
    public SchedulingTimeSpan(
      TimeSpan turnaroundTime,
      TimeSpan responseTime,
      TimeSpan runTime,
      TimeSpan waitTime,
      long numPreemptions)
    {
      this.TurnaroundTime = turnaroundTime;
      this.ResponseTime = responseTime;
      this.RunTime = runTime;
      this.WaitTime = waitTime;
      this.NumPreemptions = numPreemptions;
    }

    public long NumPreemptions { get; }

    public TimeSpan TurnaroundTime { get; }

    public TimeSpan ResponseTime { get; }

    public TimeSpan RunTime { get; }

    public TimeSpan WaitTime { get; }

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

    public void WriteJsonObject(JsonWriter jsonWriter)
    {
      if (jsonWriter == null)
        throw new ArgumentNullException(nameof (jsonWriter));
      jsonWriter.WriteStartObject();
      jsonWriter.WritePropertyName("TurnaroundTimeInMs");
      JsonWriter jsonWriter1 = jsonWriter;
      TimeSpan timeSpan = this.TurnaroundTime;
      double totalMilliseconds1 = timeSpan.TotalMilliseconds;
      jsonWriter1.WriteValue(totalMilliseconds1);
      jsonWriter.WritePropertyName("ResponseTimeInMs");
      JsonWriter jsonWriter2 = jsonWriter;
      timeSpan = this.ResponseTime;
      double totalMilliseconds2 = timeSpan.TotalMilliseconds;
      jsonWriter2.WriteValue(totalMilliseconds2);
      jsonWriter.WritePropertyName("RunTimeInMs");
      JsonWriter jsonWriter3 = jsonWriter;
      timeSpan = this.RunTime;
      double totalMilliseconds3 = timeSpan.TotalMilliseconds;
      jsonWriter3.WriteValue(totalMilliseconds3);
      jsonWriter.WritePropertyName("WaitTime");
      JsonWriter jsonWriter4 = jsonWriter;
      timeSpan = this.WaitTime;
      double totalMilliseconds4 = timeSpan.TotalMilliseconds;
      jsonWriter4.WriteValue(totalMilliseconds4);
      jsonWriter.WritePropertyName("NumberOfPreemptions");
      jsonWriter.WriteValue(this.NumPreemptions);
      jsonWriter.WriteEndObject();
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
