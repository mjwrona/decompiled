// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.QueryMetricsTextWriter
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class QueryMetricsTextWriter : QueryMetricsWriter
  {
    private readonly StringBuilder stringBuilder;
    private const string ActivityIds = "Activity Ids";
    private const string RetrievedDocumentCount = "Retrieved Document Count";
    private const string RetrievedDocumentSize = "Retrieved Document Size";
    private const string OutputDocumentCount = "Output Document Count";
    private const string OutputDocumentSize = "Output Document Size";
    private const string IndexUtilization = "Index Utilization";
    private const string TotalQueryExecutionTime = "Total Query Execution Time";
    private const string QueryPreparationTimes = "Query Preparation Times";
    private const string QueryCompileTime = "Query Compilation Time";
    private const string LogicalPlanBuildTime = "Logical Plan Build Time";
    private const string PhysicalPlanBuildTime = "Physical Plan Build Time";
    private const string QueryOptimizationTime = "Query Optimization Time";
    private const string QueryEngineTimes = "Query Engine Times";
    private const string IndexLookupTime = "Index Lookup Time";
    private const string DocumentLoadTime = "Document Load Time";
    private const string DocumentWriteTime = "Document Write Time";
    private const string RuntimeExecutionTimes = "Runtime Execution Times";
    private const string TotalExecutionTime = "Query Engine Execution Time";
    private const string SystemFunctionExecuteTime = "System Function Execution Time";
    private const string UserDefinedFunctionExecutionTime = "User-defined Function Execution Time";
    private const string ClientSideQueryMetrics = "Client Side Metrics";
    private const string Retries = "Retry Count";
    private const string RequestCharge = "Request Charge";
    private const string FetchExecutionRanges = "Partition Execution Timeline";
    private const string SchedulingMetrics = "Scheduling Metrics";
    private const string IndexUtilizationInfo = "Index Utilization Information";
    private const string UtilizedIndexes = "Utilized Indexes";
    private const string PotentialIndexes = "Potential Indexes";
    private const string FilterExpression = "Filter Expression";
    private const string IndexExpression = "IndexSpec";
    private const string FilterExpressionPrecision = "FilterPreciseSet";
    private const string IndexPlanFullFidelity = "IndexPreciseSet";
    private const string StartTimeHeader = "Start Time (UTC)";
    private const string EndTimeHeader = "End Time (UTC)";
    private const string DurationHeader = "Duration (ms)";
    private const string PartitionKeyRangeIdHeader = "Partition Id";
    private const string NumberOfDocumentsHeader = "Number of Documents";
    private const string RetryCountHeader = "Retry Count";
    private const string ActivityIdHeader = "Activity Id";
    private const string PartitionIdHeader = "Partition Id";
    private const string ResponseTimeHeader = "Response Time (ms)";
    private const string RunTimeHeader = "Run Time (ms)";
    private const string WaitTimeHeader = "Wait Time (ms)";
    private const string TurnaroundTimeHeader = "Turnaround Time (ms)";
    private const string NumberOfPreemptionHeader = "Number of Preemptions";
    private const string DateTimeFormat = "HH':'mm':'ss.ffff'Z'";
    private const string IndexUtilizationSeparator = "---";
    private static readonly int MaxDateTimeStringLength;
    private static readonly int StartTimeHeaderLength;
    private static readonly int EndTimeHeaderLength;
    private static readonly int DurationHeaderLength;
    private static readonly int PartitionKeyRangeIdHeaderLength;
    private static readonly int NumberOfDocumentsHeaderLength;
    private static readonly int RetryCountHeaderLength;
    private static readonly int ActivityIdHeaderLength;
    private static readonly TextTable.Column[] PartitionExecutionTimelineColumns;
    private static readonly TextTable PartitionExecutionTimelineTable;
    private static readonly int MaxTimeSpanStringLength;
    private static readonly int PartitionIdHeaderLength;
    private static readonly int ResponseTimeHeaderLength;
    private static readonly int RunTimeHeaderLength;
    private static readonly int WaitTimeHeaderLength;
    private static readonly int TurnaroundTimeHeaderLength;
    private static readonly int NumberOfPreemptionHeaderLength;
    private static readonly TextTable.Column[] SchedulingMetricsColumns;
    private static readonly TextTable SchedulingMetricsTable;
    private string lastFetchPartitionId;
    private string lastActivityId;
    private DateTime lastStartTime;
    private DateTime lastEndTime;
    private long lastFetchDocumentCount;
    private long lastFetchRetryCount;
    private string lastSchedulingPartitionId;
    private TimeSpan lastResponseTime;
    private TimeSpan lastRunTime;
    private TimeSpan lastWaitTime;
    private TimeSpan lastTurnaroundTime;
    private long lastNumberOfPreemptions;

    public QueryMetricsTextWriter(StringBuilder stringBuilder) => this.stringBuilder = stringBuilder != null ? stringBuilder : throw new ArgumentNullException("stringBuilder must not be null.");

    protected override void WriteBeforeQueryMetrics()
    {
    }

    protected override void WriteRetrievedDocumentCount(long retrievedDocumentCount) => QueryMetricsTextWriter.AppendCountToStringBuilder(this.stringBuilder, "Retrieved Document Count", retrievedDocumentCount, 0);

    protected override void WriteRetrievedDocumentSize(long retrievedDocumentSize) => QueryMetricsTextWriter.AppendBytesToStringBuilder(this.stringBuilder, "Retrieved Document Size", retrievedDocumentSize, 0);

    protected override void WriteOutputDocumentCount(long outputDocumentCount) => QueryMetricsTextWriter.AppendCountToStringBuilder(this.stringBuilder, "Output Document Count", outputDocumentCount, 0);

    protected override void WriteOutputDocumentSize(long outputDocumentSize) => QueryMetricsTextWriter.AppendBytesToStringBuilder(this.stringBuilder, "Output Document Size", outputDocumentSize, 0);

    protected override void WriteIndexHitRatio(double indexHitRatio) => QueryMetricsTextWriter.AppendPercentageToStringBuilder(this.stringBuilder, "Index Utilization", indexHitRatio, 0);

    protected override void WriteTotalQueryExecutionTime(TimeSpan totalQueryExecutionTime) => QueryMetricsTextWriter.AppendTimeSpanToStringBuilder(this.stringBuilder, "Total Query Execution Time", totalQueryExecutionTime, 0);

    protected override void WriteBeforeQueryPreparationTimes() => QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Query Preparation Times", 1);

    protected override void WriteQueryCompilationTime(TimeSpan queryCompilationTime) => QueryMetricsTextWriter.AppendTimeSpanToStringBuilder(this.stringBuilder, "Query Compilation Time", queryCompilationTime, 2);

    protected override void WriteLogicalPlanBuildTime(TimeSpan logicalPlanBuildTime) => QueryMetricsTextWriter.AppendTimeSpanToStringBuilder(this.stringBuilder, "Logical Plan Build Time", logicalPlanBuildTime, 2);

    protected override void WritePhysicalPlanBuildTime(TimeSpan physicalPlanBuildTime) => QueryMetricsTextWriter.AppendTimeSpanToStringBuilder(this.stringBuilder, "Physical Plan Build Time", physicalPlanBuildTime, 2);

    protected override void WriteQueryOptimizationTime(TimeSpan queryOptimizationTime) => QueryMetricsTextWriter.AppendTimeSpanToStringBuilder(this.stringBuilder, "Query Optimization Time", queryOptimizationTime, 2);

    protected override void WriteAfterQueryPreparationTimes()
    {
    }

    protected override void WriteIndexLookupTime(TimeSpan indexLookupTime) => QueryMetricsTextWriter.AppendTimeSpanToStringBuilder(this.stringBuilder, "Index Lookup Time", indexLookupTime, 1);

    protected override void WriteDocumentLoadTime(TimeSpan documentLoadTime) => QueryMetricsTextWriter.AppendTimeSpanToStringBuilder(this.stringBuilder, "Document Load Time", documentLoadTime, 1);

    protected override void WriteVMExecutionTime(TimeSpan vmExecutionTime)
    {
    }

    protected override void WriteBeforeRuntimeExecutionTimes() => QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Runtime Execution Times", 1);

    protected override void WriteQueryEngineExecutionTime(TimeSpan queryEngineExecutionTime) => QueryMetricsTextWriter.AppendTimeSpanToStringBuilder(this.stringBuilder, "Query Engine Times", queryEngineExecutionTime, 2);

    protected override void WriteSystemFunctionExecutionTime(TimeSpan systemFunctionExecutionTime) => QueryMetricsTextWriter.AppendTimeSpanToStringBuilder(this.stringBuilder, "System Function Execution Time", systemFunctionExecutionTime, 2);

    protected override void WriteUserDefinedFunctionExecutionTime(
      TimeSpan userDefinedFunctionExecutionTime)
    {
      QueryMetricsTextWriter.AppendTimeSpanToStringBuilder(this.stringBuilder, "User-defined Function Execution Time", userDefinedFunctionExecutionTime, 2);
    }

    protected override void WriteAfterRuntimeExecutionTimes()
    {
    }

    protected override void WriteDocumentWriteTime(TimeSpan documentWriteTime) => QueryMetricsTextWriter.AppendTimeSpanToStringBuilder(this.stringBuilder, "Document Write Time", documentWriteTime, 1);

    protected override void WriteBeforeClientSideMetrics() => QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Client Side Metrics", 0);

    protected override void WriteRetries(long retries) => QueryMetricsTextWriter.AppendCountToStringBuilder(this.stringBuilder, "Retry Count", retries, 1);

    protected override void WriteRequestCharge(double requestCharge) => QueryMetricsTextWriter.AppendRUToStringBuilder(this.stringBuilder, "Request Charge", requestCharge, 1);

    protected override void WriteBeforePartitionExecutionTimeline()
    {
      QueryMetricsTextWriter.AppendNewlineToStringBuilder(this.stringBuilder);
      QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Partition Execution Timeline", 1);
      QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, QueryMetricsTextWriter.PartitionExecutionTimelineTable.TopLine, 1);
      QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, QueryMetricsTextWriter.PartitionExecutionTimelineTable.Header, 1);
      QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, QueryMetricsTextWriter.PartitionExecutionTimelineTable.MiddleLine, 1);
    }

    protected override void WriteBeforeFetchExecutionRange()
    {
    }

    protected override void WriteFetchPartitionKeyRangeId(string partitionId) => this.lastFetchPartitionId = partitionId;

    protected override void WriteActivityId(string activityId) => this.lastActivityId = activityId;

    protected override void WriteStartTime(DateTime startTime) => this.lastStartTime = startTime;

    protected override void WriteEndTime(DateTime endTime) => this.lastEndTime = endTime;

    protected override void WriteFetchDocumentCount(long numberOfDocuments) => this.lastFetchDocumentCount = numberOfDocuments;

    protected override void WriteFetchRetryCount(long retryCount) => this.lastFetchRetryCount = retryCount;

    protected override void WriteAfterFetchExecutionRange() => QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, QueryMetricsTextWriter.PartitionExecutionTimelineTable.GetRow((object) this.lastFetchPartitionId, (object) this.lastActivityId, (object) this.lastStartTime.ToUniversalTime().ToString("HH':'mm':'ss.ffff'Z'"), (object) this.lastEndTime.ToUniversalTime().ToString("HH':'mm':'ss.ffff'Z'"), (object) (this.lastEndTime - this.lastStartTime).TotalMilliseconds.ToString("0.00"), (object) this.lastFetchDocumentCount, (object) this.lastFetchRetryCount), 1);

    protected override void WriteAfterPartitionExecutionTimeline() => QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, QueryMetricsTextWriter.PartitionExecutionTimelineTable.BottomLine, 1);

    protected override void WriteBeforeSchedulingMetrics()
    {
      QueryMetricsTextWriter.AppendNewlineToStringBuilder(this.stringBuilder);
      QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Scheduling Metrics", 1);
      QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, QueryMetricsTextWriter.SchedulingMetricsTable.TopLine, 1);
      QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, QueryMetricsTextWriter.SchedulingMetricsTable.Header, 1);
      QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, QueryMetricsTextWriter.SchedulingMetricsTable.MiddleLine, 1);
    }

    protected override void WriteBeforePartitionSchedulingTimeSpan()
    {
    }

    protected override void WritePartitionSchedulingTimeSpanId(string partitionId) => this.lastSchedulingPartitionId = partitionId;

    protected override void WriteResponseTime(TimeSpan responseTime) => this.lastResponseTime = responseTime;

    protected override void WriteRunTime(TimeSpan runTime) => this.lastRunTime = runTime;

    protected override void WriteWaitTime(TimeSpan waitTime) => this.lastWaitTime = waitTime;

    protected override void WriteTurnaroundTime(TimeSpan turnaroundTime) => this.lastTurnaroundTime = turnaroundTime;

    protected override void WriteNumberOfPreemptions(long numPreemptions) => this.lastNumberOfPreemptions = numPreemptions;

    protected override void WriteAfterPartitionSchedulingTimeSpan() => QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, QueryMetricsTextWriter.SchedulingMetricsTable.GetRow((object) this.lastSchedulingPartitionId, (object) this.lastResponseTime.TotalMilliseconds.ToString("0.00"), (object) this.lastRunTime.TotalMilliseconds.ToString("0.00"), (object) this.lastWaitTime.TotalMilliseconds.ToString("0.00"), (object) this.lastTurnaroundTime.TotalMilliseconds.ToString("0.00"), (object) this.lastNumberOfPreemptions), 1);

    protected override void WriteAfterSchedulingMetrics() => QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, QueryMetricsTextWriter.SchedulingMetricsTable.BottomLine, 1);

    protected override void WriteAfterClientSideMetrics()
    {
    }

    protected override void WriteBeforeIndexUtilizationInfo() => QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Index Utilization Information", 0);

    protected override void WriteIndexUtilizationInfo(Microsoft.Azure.Documents.IndexUtilizationInfo indexUtilizationInfo)
    {
      QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Utilized Indexes", 1);
      foreach (IndexUtilizationData utilizedIndex in (IEnumerable<IndexUtilizationData>) indexUtilizationInfo.UtilizedIndexes)
        WriteIndexUtilizationData(utilizedIndex);
      QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Potential Indexes", 1);
      foreach (IndexUtilizationData potentialIndex in (IEnumerable<IndexUtilizationData>) indexUtilizationInfo.PotentialIndexes)
        WriteIndexUtilizationData(potentialIndex);

      void WriteIndexUtilizationData(IndexUtilizationData indexUtilizationData)
      {
        QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, "Filter Expression: " + indexUtilizationData.FilterExpression, 2);
        QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, "IndexSpec: " + indexUtilizationData.IndexDocumentExpression, 2);
        QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, string.Format("{0}: {1}", (object) "FilterPreciseSet", (object) indexUtilizationData.FilterExpressionPrecision), 2);
        QueryMetricsTextWriter.AppendHeaderToStringBuilder(this.stringBuilder, "---", 2);
      }
    }

    protected override void WriteAfterIndexUtilizationInfo()
    {
    }

    protected override void WriteAfterQueryMetrics()
    {
    }

    private static void AppendToStringBuilder(
      StringBuilder stringBuilder,
      string property,
      string value,
      string units,
      int indentLevel)
    {
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0,-40} : {1,15} {2,-12}{3}", (object) (string.Concat(Enumerable.Repeat<string>("  ", indentLevel)) + property), (object) value, (object) units, (object) Environment.NewLine);
    }

    private static void AppendBytesToStringBuilder(
      StringBuilder stringBuilder,
      string property,
      long bytes,
      int indentLevel)
    {
      QueryMetricsTextWriter.AppendToStringBuilder(stringBuilder, property, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:n0}", (object) bytes), nameof (bytes), indentLevel);
    }

    private static void AppendCountToStringBuilder(
      StringBuilder stringBuilder,
      string property,
      long count,
      int indentLevel)
    {
      QueryMetricsTextWriter.AppendToStringBuilder(stringBuilder, property, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:n0}", (object) count), "", indentLevel);
    }

    private static void AppendPercentageToStringBuilder(
      StringBuilder stringBuilder,
      string property,
      double percentage,
      int indentLevel)
    {
      QueryMetricsTextWriter.AppendToStringBuilder(stringBuilder, property, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:n2}", (object) (percentage * 100.0)), "%", indentLevel);
    }

    private static void AppendTimeSpanToStringBuilder(
      StringBuilder stringBuilder,
      string property,
      TimeSpan timeSpan,
      int indentLevel)
    {
      QueryMetricsTextWriter.AppendToStringBuilder(stringBuilder, property, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:n2}", (object) timeSpan.TotalMilliseconds), "milliseconds", indentLevel);
    }

    private static void AppendHeaderToStringBuilder(
      StringBuilder stringBuilder,
      string headerTitle,
      int indentLevel)
    {
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) (string.Concat(Enumerable.Repeat<string>("  ", indentLevel)) + headerTitle), (object) Environment.NewLine);
    }

    private static void AppendRUToStringBuilder(
      StringBuilder stringBuilder,
      string property,
      double requestCharge,
      int indentLevel)
    {
      QueryMetricsTextWriter.AppendToStringBuilder(stringBuilder, property, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:n2}", (object) requestCharge), "RUs", indentLevel);
    }

    private static void AppendNewlineToStringBuilder(StringBuilder stringBuilder) => QueryMetricsTextWriter.AppendHeaderToStringBuilder(stringBuilder, string.Empty, 0);

    static QueryMetricsTextWriter()
    {
      DateTime dateTime = DateTime.MaxValue;
      dateTime = dateTime.ToUniversalTime();
      QueryMetricsTextWriter.MaxDateTimeStringLength = dateTime.ToString("HH':'mm':'ss.ffff'Z'").Length;
      QueryMetricsTextWriter.StartTimeHeaderLength = Math.Max(QueryMetricsTextWriter.MaxDateTimeStringLength, "Start Time (UTC)".Length);
      QueryMetricsTextWriter.EndTimeHeaderLength = Math.Max(QueryMetricsTextWriter.MaxDateTimeStringLength, "End Time (UTC)".Length);
      QueryMetricsTextWriter.DurationHeaderLength = Math.Max("Duration (ms)".Length, TimeSpan.MaxValue.TotalMilliseconds.ToString("0.00").Length);
      QueryMetricsTextWriter.PartitionKeyRangeIdHeaderLength = "Partition Id".Length;
      QueryMetricsTextWriter.NumberOfDocumentsHeaderLength = "Number of Documents".Length;
      QueryMetricsTextWriter.RetryCountHeaderLength = "Retry Count".Length;
      QueryMetricsTextWriter.ActivityIdHeaderLength = Guid.Empty.ToString().Length;
      QueryMetricsTextWriter.PartitionExecutionTimelineColumns = new TextTable.Column[7]
      {
        new TextTable.Column("Partition Id", QueryMetricsTextWriter.PartitionKeyRangeIdHeaderLength),
        new TextTable.Column("Activity Id", QueryMetricsTextWriter.ActivityIdHeaderLength),
        new TextTable.Column("Start Time (UTC)", QueryMetricsTextWriter.StartTimeHeaderLength),
        new TextTable.Column("End Time (UTC)", QueryMetricsTextWriter.EndTimeHeaderLength),
        new TextTable.Column("Duration (ms)", QueryMetricsTextWriter.DurationHeaderLength),
        new TextTable.Column("Number of Documents", QueryMetricsTextWriter.NumberOfDocumentsHeaderLength),
        new TextTable.Column("Retry Count", QueryMetricsTextWriter.RetryCountHeaderLength)
      };
      QueryMetricsTextWriter.PartitionExecutionTimelineTable = new TextTable(QueryMetricsTextWriter.PartitionExecutionTimelineColumns);
      QueryMetricsTextWriter.MaxTimeSpanStringLength = Math.Max(TimeSpan.MaxValue.TotalMilliseconds.ToString("G17").Length, "Turnaround Time (ms)".Length);
      QueryMetricsTextWriter.PartitionIdHeaderLength = "Partition Id".Length;
      QueryMetricsTextWriter.ResponseTimeHeaderLength = QueryMetricsTextWriter.MaxTimeSpanStringLength;
      QueryMetricsTextWriter.RunTimeHeaderLength = QueryMetricsTextWriter.MaxTimeSpanStringLength;
      QueryMetricsTextWriter.WaitTimeHeaderLength = QueryMetricsTextWriter.MaxTimeSpanStringLength;
      QueryMetricsTextWriter.TurnaroundTimeHeaderLength = QueryMetricsTextWriter.MaxTimeSpanStringLength;
      QueryMetricsTextWriter.NumberOfPreemptionHeaderLength = "Number of Preemptions".Length;
      QueryMetricsTextWriter.SchedulingMetricsColumns = new TextTable.Column[6]
      {
        new TextTable.Column("Partition Id", QueryMetricsTextWriter.PartitionIdHeaderLength),
        new TextTable.Column("Response Time (ms)", QueryMetricsTextWriter.ResponseTimeHeaderLength),
        new TextTable.Column("Run Time (ms)", QueryMetricsTextWriter.RunTimeHeaderLength),
        new TextTable.Column("Wait Time (ms)", QueryMetricsTextWriter.WaitTimeHeaderLength),
        new TextTable.Column("Turnaround Time (ms)", QueryMetricsTextWriter.TurnaroundTimeHeaderLength),
        new TextTable.Column("Number of Preemptions", QueryMetricsTextWriter.NumberOfPreemptionHeaderLength)
      };
      QueryMetricsTextWriter.SchedulingMetricsTable = new TextTable(QueryMetricsTextWriter.SchedulingMetricsColumns);
    }
  }
}
