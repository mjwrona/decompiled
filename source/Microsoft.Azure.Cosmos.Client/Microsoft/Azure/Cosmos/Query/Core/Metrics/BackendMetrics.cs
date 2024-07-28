// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.BackendMetrics
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class BackendMetrics
  {
    public static readonly BackendMetrics Empty = new BackendMetrics(0L, 0L, 0L, 0L, 0.0, new TimeSpan(), QueryPreparationTimes.Zero, new TimeSpan(), new TimeSpan(), new TimeSpan(), RuntimeExecutionTimes.Empty, new TimeSpan());

    public BackendMetrics(
      long retrievedDocumentCount,
      long retrievedDocumentSize,
      long outputDocumentCount,
      long outputDocumentSize,
      double indexHitRatio,
      TimeSpan totalQueryExecutionTime,
      QueryPreparationTimes queryPreparationTimes,
      TimeSpan indexLookupTime,
      TimeSpan documentLoadTime,
      TimeSpan vmExecutionTime,
      RuntimeExecutionTimes runtimeExecutionTimes,
      TimeSpan documentWriteTime)
    {
      this.RetrievedDocumentCount = retrievedDocumentCount;
      this.RetrievedDocumentSize = retrievedDocumentSize;
      this.OutputDocumentCount = outputDocumentCount;
      this.OutputDocumentSize = outputDocumentSize;
      this.IndexHitRatio = indexHitRatio;
      this.TotalTime = totalQueryExecutionTime;
      this.QueryPreparationTimes = queryPreparationTimes ?? throw new ArgumentNullException("queryPreparationTimes can not be null.");
      this.IndexLookupTime = indexLookupTime;
      this.DocumentLoadTime = documentLoadTime;
      this.VMExecutionTime = vmExecutionTime;
      this.RuntimeExecutionTimes = runtimeExecutionTimes ?? throw new ArgumentNullException("runtimeExecutionTimes can not be null.");
      this.DocumentWriteTime = documentWriteTime;
    }

    public TimeSpan TotalTime { get; }

    public long RetrievedDocumentCount { get; }

    public long RetrievedDocumentSize { get; }

    public long OutputDocumentCount { get; }

    public long OutputDocumentSize { get; }

    public QueryPreparationTimes QueryPreparationTimes { get; }

    public TimeSpan IndexLookupTime { get; }

    public TimeSpan DocumentLoadTime { get; }

    public RuntimeExecutionTimes RuntimeExecutionTimes { get; }

    public TimeSpan DocumentWriteTime { get; }

    public double IndexHitRatio { get; }

    public TimeSpan VMExecutionTime { get; }

    public override string ToString() => string.Format("totalExecutionTimeInMs={0};queryCompileTimeInMs={1};queryLogicalPlanBuildTimeInMs={2};queryPhysicalPlanBuildTimeInMs={3};queryOptimizationTimeInMs={4};indexLookupTimeInMs={5};documentLoadTimeInMs={6};systemFunctionExecuteTimeInMs={7};userFunctionExecuteTimeInMs={8};retrievedDocumentCount={9};retrievedDocumentSize={10};outputDocumentCount={11};outputDocumentSize={12};writeOutputTimeInMs={13};indexUtilizationRatio={14}", (object) this.TotalTime.TotalMilliseconds, (object) this.QueryPreparationTimes.QueryCompilationTime.TotalMilliseconds, (object) this.QueryPreparationTimes.LogicalPlanBuildTime.TotalMilliseconds, (object) this.QueryPreparationTimes.PhysicalPlanBuildTime.TotalMilliseconds, (object) this.QueryPreparationTimes.QueryOptimizationTime.TotalMilliseconds, (object) this.IndexLookupTime.TotalMilliseconds, (object) this.DocumentLoadTime.TotalMilliseconds, (object) this.RuntimeExecutionTimes.SystemFunctionExecutionTime.TotalMilliseconds, (object) this.RuntimeExecutionTimes.UserDefinedFunctionExecutionTime.TotalMilliseconds, (object) this.RetrievedDocumentCount, (object) this.RetrievedDocumentSize, (object) this.OutputDocumentCount, (object) this.OutputDocumentSize, (object) this.DocumentWriteTime.TotalMilliseconds, (object) this.IndexHitRatio);

    public static BackendMetrics CreateFromIEnumerable(
      IEnumerable<BackendMetrics> backendMetricsEnumerable)
    {
      BackendMetrics.Accumulator accumulator = new BackendMetrics.Accumulator();
      foreach (BackendMetrics backendMetrics in backendMetricsEnumerable)
        accumulator = accumulator.Accumulate(backendMetrics);
      return BackendMetrics.Accumulator.ToBackendMetrics(accumulator);
    }

    public static bool TryParseFromDelimitedString(
      string delimitedString,
      out BackendMetrics backendMetrics)
    {
      return BackendMetricsParser.TryParse(delimitedString, out backendMetrics);
    }

    public static BackendMetrics ParseFromDelimitedString(string delimitedString)
    {
      BackendMetrics backendMetrics;
      if (!BackendMetricsParser.TryParse(delimitedString, out backendMetrics))
        throw new FormatException();
      return backendMetrics;
    }

    public ref struct Accumulator
    {
      public Accumulator(
        TimeSpan totalTime,
        long retrievedDocumentCount,
        long retrievedDocumentSize,
        long outputDocumentCount,
        long outputDocumentSize,
        double indexHitRatio,
        QueryPreparationTimes.Accumulator queryPreparationTimesAccumulator,
        TimeSpan indexLookupTime,
        TimeSpan documentLoadTime,
        RuntimeExecutionTimes.Accumulator runtimeExecutionTimesAccumulator,
        TimeSpan documentWriteTime,
        TimeSpan vmExecutionTime)
      {
        this.TotalTime = totalTime;
        this.RetrievedDocumentCount = retrievedDocumentCount;
        this.RetrievedDocumentSize = retrievedDocumentSize;
        this.OutputDocumentCount = outputDocumentCount;
        this.OutputDocumentSize = outputDocumentSize;
        this.IndexHitRatio = indexHitRatio;
        this.QueryPreparationTimesAccumulator = queryPreparationTimesAccumulator;
        this.IndexLookupTime = indexLookupTime;
        this.DocumentLoadTime = documentLoadTime;
        this.RuntimeExecutionTimesAccumulator = runtimeExecutionTimesAccumulator;
        this.DocumentWriteTime = documentWriteTime;
        this.VMExecutionTime = vmExecutionTime;
      }

      public TimeSpan TotalTime { get; }

      public long RetrievedDocumentCount { get; }

      public long RetrievedDocumentSize { get; }

      public long OutputDocumentCount { get; }

      public long OutputDocumentSize { get; }

      public double IndexHitRatio { get; }

      public QueryPreparationTimes.Accumulator QueryPreparationTimesAccumulator { get; }

      public TimeSpan IndexLookupTime { get; }

      public TimeSpan DocumentLoadTime { get; }

      public RuntimeExecutionTimes.Accumulator RuntimeExecutionTimesAccumulator { get; }

      public TimeSpan DocumentWriteTime { get; }

      public TimeSpan VMExecutionTime { get; }

      public BackendMetrics.Accumulator Accumulate(BackendMetrics backendMetrics) => new BackendMetrics.Accumulator(this.TotalTime + backendMetrics.TotalTime, this.RetrievedDocumentCount + backendMetrics.RetrievedDocumentCount, this.RetrievedDocumentSize + backendMetrics.RetrievedDocumentSize, this.OutputDocumentCount + backendMetrics.OutputDocumentCount, this.OutputDocumentSize + backendMetrics.OutputDocumentSize, ((double) this.OutputDocumentCount * this.IndexHitRatio + (double) backendMetrics.OutputDocumentCount * backendMetrics.IndexHitRatio) / (double) (this.RetrievedDocumentCount + backendMetrics.RetrievedDocumentCount), this.QueryPreparationTimesAccumulator.Accumulate(backendMetrics.QueryPreparationTimes), this.IndexLookupTime + backendMetrics.IndexLookupTime, this.DocumentLoadTime + backendMetrics.DocumentLoadTime, this.RuntimeExecutionTimesAccumulator.Accumulate(backendMetrics.RuntimeExecutionTimes), this.DocumentWriteTime + backendMetrics.DocumentWriteTime, this.VMExecutionTime + backendMetrics.VMExecutionTime);

      public static BackendMetrics ToBackendMetrics(BackendMetrics.Accumulator accumulator) => new BackendMetrics(accumulator.RetrievedDocumentCount, accumulator.RetrievedDocumentSize, accumulator.OutputDocumentCount, accumulator.OutputDocumentSize, accumulator.IndexHitRatio, accumulator.TotalTime, QueryPreparationTimes.Accumulator.ToQueryPreparationTimes(accumulator.QueryPreparationTimesAccumulator), accumulator.IndexLookupTime, accumulator.DocumentLoadTime, accumulator.VMExecutionTime, RuntimeExecutionTimes.Accumulator.ToRuntimeExecutionTimes(accumulator.RuntimeExecutionTimesAccumulator), accumulator.DocumentWriteTime);
    }
  }
}
