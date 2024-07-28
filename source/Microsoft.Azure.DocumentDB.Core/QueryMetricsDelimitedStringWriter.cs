// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.QueryMetricsDelimitedStringWriter
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class QueryMetricsDelimitedStringWriter : QueryMetricsWriter
  {
    private readonly StringBuilder stringBuilder;
    private const string RetrievedDocumentCount = "retrievedDocumentCount";
    private const string RetrievedDocumentSize = "retrievedDocumentSize";
    private const string OutputDocumentCount = "outputDocumentCount";
    private const string OutputDocumentSize = "outputDocumentSize";
    private const string IndexHitRatio = "indexUtilizationRatio";
    private const string IndexHitDocumentCount = "indexHitDocumentCount";
    private const string TotalQueryExecutionTimeInMs = "totalExecutionTimeInMs";
    private const string QueryCompileTimeInMs = "queryCompileTimeInMs";
    private const string LogicalPlanBuildTimeInMs = "queryLogicalPlanBuildTimeInMs";
    private const string PhysicalPlanBuildTimeInMs = "queryPhysicalPlanBuildTimeInMs";
    private const string QueryOptimizationTimeInMs = "queryOptimizationTimeInMs";
    private const string IndexLookupTimeInMs = "indexLookupTimeInMs";
    private const string DocumentLoadTimeInMs = "documentLoadTimeInMs";
    private const string VMExecutionTimeInMs = "VMExecutionTimeInMs";
    private const string DocumentWriteTimeInMs = "writeOutputTimeInMs";
    private const string QueryEngineTimes = "queryEngineTimes";
    private const string SystemFunctionExecuteTimeInMs = "systemFunctionExecuteTimeInMs";
    private const string UserDefinedFunctionExecutionTimeInMs = "userFunctionExecuteTimeInMs";
    private const string KeyValueDelimiter = "=";
    private const string KeyValuePairDelimiter = ";";

    public QueryMetricsDelimitedStringWriter(StringBuilder stringBuilder) => this.stringBuilder = stringBuilder != null ? stringBuilder : throw new ArgumentNullException("stringBuilder must not be null.");

    protected override void WriteBeforeQueryMetrics()
    {
    }

    protected override void WriteRetrievedDocumentCount(long retrievedDocumentCount) => this.AppendKeyValuePair<long>(nameof (retrievedDocumentCount), retrievedDocumentCount);

    protected override void WriteRetrievedDocumentSize(long retrievedDocumentSize) => this.AppendKeyValuePair<long>(nameof (retrievedDocumentSize), retrievedDocumentSize);

    protected override void WriteOutputDocumentCount(long outputDocumentCount) => this.AppendKeyValuePair<long>(nameof (outputDocumentCount), outputDocumentCount);

    protected override void WriteOutputDocumentSize(long outputDocumentSize) => this.AppendKeyValuePair<long>(nameof (outputDocumentSize), outputDocumentSize);

    protected override void WriteIndexHitRatio(double indexHitRatio) => this.AppendKeyValuePair<double>("indexUtilizationRatio", indexHitRatio);

    protected override void WriteTotalQueryExecutionTime(TimeSpan totalQueryExecutionTime) => this.AppendTimeSpan("totalExecutionTimeInMs", totalQueryExecutionTime);

    protected override void WriteBeforeQueryPreparationTimes()
    {
    }

    protected override void WriteQueryCompilationTime(TimeSpan queryCompilationTime) => this.AppendTimeSpan("queryCompileTimeInMs", queryCompilationTime);

    protected override void WriteLogicalPlanBuildTime(TimeSpan logicalPlanBuildTime) => this.AppendTimeSpan("queryLogicalPlanBuildTimeInMs", logicalPlanBuildTime);

    protected override void WritePhysicalPlanBuildTime(TimeSpan physicalPlanBuildTime) => this.AppendTimeSpan("queryPhysicalPlanBuildTimeInMs", physicalPlanBuildTime);

    protected override void WriteQueryOptimizationTime(TimeSpan queryOptimizationTime) => this.AppendTimeSpan("queryOptimizationTimeInMs", queryOptimizationTime);

    protected override void WriteAfterQueryPreparationTimes()
    {
    }

    protected override void WriteIndexLookupTime(TimeSpan indexLookupTime) => this.AppendTimeSpan("indexLookupTimeInMs", indexLookupTime);

    protected override void WriteDocumentLoadTime(TimeSpan documentLoadTime) => this.AppendTimeSpan("documentLoadTimeInMs", documentLoadTime);

    protected override void WriteVMExecutionTime(TimeSpan vmExecutionTime) => this.AppendTimeSpan("VMExecutionTimeInMs", vmExecutionTime);

    protected override void WriteBeforeRuntimeExecutionTimes()
    {
    }

    protected override void WriteQueryEngineExecutionTime(TimeSpan queryEngineExecutionTime) => this.AppendTimeSpan("queryEngineTimes", queryEngineExecutionTime);

    protected override void WriteSystemFunctionExecutionTime(TimeSpan systemFunctionExecutionTime) => this.AppendTimeSpan("systemFunctionExecuteTimeInMs", systemFunctionExecutionTime);

    protected override void WriteUserDefinedFunctionExecutionTime(
      TimeSpan userDefinedFunctionExecutionTime)
    {
      this.AppendTimeSpan("userFunctionExecuteTimeInMs", userDefinedFunctionExecutionTime);
    }

    protected override void WriteAfterRuntimeExecutionTimes()
    {
    }

    protected override void WriteDocumentWriteTime(TimeSpan documentWriteTime) => this.AppendTimeSpan("writeOutputTimeInMs", documentWriteTime);

    protected override void WriteBeforeClientSideMetrics()
    {
    }

    protected override void WriteRetries(long retries)
    {
    }

    protected override void WriteRequestCharge(double requestCharge)
    {
    }

    protected override void WriteBeforePartitionExecutionTimeline()
    {
    }

    protected override void WriteBeforeFetchExecutionRange()
    {
    }

    protected override void WriteFetchPartitionKeyRangeId(string partitionId)
    {
    }

    protected override void WriteActivityId(string activityId)
    {
    }

    protected override void WriteStartTime(DateTime startTime)
    {
    }

    protected override void WriteEndTime(DateTime endTime)
    {
    }

    protected override void WriteFetchDocumentCount(long numberOfDocuments)
    {
    }

    protected override void WriteFetchRetryCount(long retryCount)
    {
    }

    protected override void WriteAfterFetchExecutionRange()
    {
    }

    protected override void WriteAfterPartitionExecutionTimeline()
    {
    }

    protected override void WriteBeforeSchedulingMetrics()
    {
    }

    protected override void WriteBeforePartitionSchedulingTimeSpan()
    {
    }

    protected override void WritePartitionSchedulingTimeSpanId(string partitionId)
    {
    }

    protected override void WriteResponseTime(TimeSpan responseTime)
    {
    }

    protected override void WriteRunTime(TimeSpan runTime)
    {
    }

    protected override void WriteWaitTime(TimeSpan waitTime)
    {
    }

    protected override void WriteTurnaroundTime(TimeSpan turnaroundTime)
    {
    }

    protected override void WriteNumberOfPreemptions(long numPreemptions)
    {
    }

    protected override void WriteAfterPartitionSchedulingTimeSpan()
    {
    }

    protected override void WriteAfterSchedulingMetrics()
    {
    }

    protected override void WriteAfterClientSideMetrics()
    {
    }

    protected override void WriteBeforeIndexUtilizationInfo()
    {
    }

    protected override void WriteIndexUtilizationInfo(IndexUtilizationInfo indexUtilizationInfo)
    {
    }

    protected override void WriteAfterIndexUtilizationInfo()
    {
    }

    protected override void WriteAfterQueryMetrics() => --this.stringBuilder.Length;

    private void AppendKeyValuePair<T>(string name, T value)
    {
      this.stringBuilder.Append(name);
      this.stringBuilder.Append("=");
      this.stringBuilder.Append((object) value);
      this.stringBuilder.Append(";");
    }

    private void AppendTimeSpan(string name, TimeSpan dateTime) => this.AppendKeyValuePair<string>(name, dateTime.TotalMilliseconds.ToString("0.00"));
  }
}
