// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.QueryMetrics
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Documents
{
  public sealed class QueryMetrics
  {
    internal static readonly QueryMetrics Zero = new QueryMetrics(0L, 0L, 0L, 0L, 0L, IndexUtilizationInfo.Empty, new TimeSpan(), QueryPreparationTimes.Zero, new TimeSpan(), new TimeSpan(), new TimeSpan(), RuntimeExecutionTimes.Zero, new TimeSpan(), ClientSideMetrics.Zero);
    private readonly long retrievedDocumentCount;
    private readonly long retrievedDocumentSize;
    private readonly long outputDocumentCount;
    private readonly long outputDocumentSize;
    private readonly long indexHitDocumentCount;
    private readonly IndexUtilizationInfo indexUtilizationInfo;
    private readonly TimeSpan totalQueryExecutionTime;
    private readonly QueryPreparationTimes queryPreparationTimes;
    private readonly TimeSpan indexLookupTime;
    private readonly TimeSpan documentLoadTime;
    private readonly TimeSpan vmExecutionTime;
    private readonly RuntimeExecutionTimes runtimeExecutionTimes;
    private readonly TimeSpan documentWriteTime;
    private readonly ClientSideMetrics clientSideMetrics;
    private readonly QueryEngineTimes queryEngineTimes;

    [JsonConstructor]
    internal QueryMetrics(
      long retrievedDocumentCount,
      long retrievedDocumentSize,
      long outputDocumentCount,
      long outputDocumentSize,
      long indexHitDocumentCount,
      IndexUtilizationInfo indexUtilizationInfo,
      TimeSpan totalQueryExecutionTime,
      QueryPreparationTimes queryPreparationTimes,
      TimeSpan indexLookupTime,
      TimeSpan documentLoadTime,
      TimeSpan vmExecutionTime,
      RuntimeExecutionTimes runtimeExecutionTimes,
      TimeSpan documentWriteTime,
      ClientSideMetrics clientSideMetrics)
    {
      if (queryPreparationTimes == null)
        throw new ArgumentNullException("queryPreparationTimes can not be null.");
      if (runtimeExecutionTimes == null)
        throw new ArgumentNullException("runtimeExecutionTimes can not be null.");
      if (clientSideMetrics == null)
        throw new ArgumentNullException("clientSideMetrics can not be null.");
      this.retrievedDocumentCount = retrievedDocumentCount;
      this.retrievedDocumentSize = retrievedDocumentSize;
      this.outputDocumentCount = outputDocumentCount;
      this.outputDocumentSize = outputDocumentSize;
      this.indexHitDocumentCount = indexHitDocumentCount;
      this.indexUtilizationInfo = indexUtilizationInfo;
      this.totalQueryExecutionTime = totalQueryExecutionTime;
      this.queryPreparationTimes = queryPreparationTimes;
      this.indexLookupTime = indexLookupTime;
      this.documentLoadTime = documentLoadTime;
      this.vmExecutionTime = vmExecutionTime;
      this.runtimeExecutionTimes = runtimeExecutionTimes;
      this.documentWriteTime = documentWriteTime;
      this.clientSideMetrics = clientSideMetrics;
      this.queryEngineTimes = new QueryEngineTimes(indexLookupTime, documentLoadTime, vmExecutionTime, documentWriteTime, runtimeExecutionTimes);
    }

    public TimeSpan TotalTime => this.totalQueryExecutionTime;

    public long RetrievedDocumentCount => this.retrievedDocumentCount;

    public long RetrievedDocumentSize => this.retrievedDocumentSize;

    public long OutputDocumentCount => this.outputDocumentCount;

    internal long OutputDocumentSize => this.outputDocumentSize;

    internal TimeSpan TotalQueryExecutionTime => this.totalQueryExecutionTime;

    public QueryPreparationTimes QueryPreparationTimes => this.queryPreparationTimes;

    public QueryEngineTimes QueryEngineTimes => this.queryEngineTimes;

    public long Retries => this.clientSideMetrics.Retries;

    internal TimeSpan IndexLookupTime => this.indexLookupTime;

    internal TimeSpan DocumentLoadTime => this.documentLoadTime;

    internal RuntimeExecutionTimes RuntimeExecutionTimes => this.runtimeExecutionTimes;

    internal TimeSpan DocumentWriteTime => this.documentWriteTime;

    [JsonProperty(PropertyName = "ClientSideMetrics")]
    internal ClientSideMetrics ClientSideMetrics => this.clientSideMetrics;

    public double IndexHitRatio => this.retrievedDocumentCount != 0L ? (double) this.indexHitDocumentCount / (double) this.retrievedDocumentCount : 1.0;

    internal long IndexHitDocumentCount => this.indexHitDocumentCount;

    internal IndexUtilizationInfo IndexUtilizationInfo => this.indexUtilizationInfo;

    internal TimeSpan VMExecutionTime => this.vmExecutionTime;

    private double IndexUtilization => this.IndexHitRatio * 100.0;

    public static QueryMetrics operator +(QueryMetrics queryMetrics1, QueryMetrics queryMetrics2) => queryMetrics1.Add(queryMetrics2);

    public override string ToString() => this.ToTextString();

    private string ToTextString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      new QueryMetricsTextWriter(stringBuilder).WriteQueryMetrics(this);
      return stringBuilder.ToString();
    }

    internal string ToDelimitedString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      new QueryMetricsDelimitedStringWriter(stringBuilder).WriteQueryMetrics(this);
      return stringBuilder.ToString();
    }

    internal static QueryMetrics CreateFromIEnumerable(IEnumerable<QueryMetrics> queryMetricsList)
    {
      if (queryMetricsList == null)
        throw new ArgumentNullException(nameof (queryMetricsList));
      long retrievedDocumentCount = 0;
      long retrievedDocumentSize = 0;
      long outputDocumentCount = 0;
      long outputDocumentSize = 0;
      long indexHitDocumentCount = 0;
      List<IndexUtilizationInfo> indexUtilizationInfoList = new List<IndexUtilizationInfo>();
      TimeSpan totalQueryExecutionTime = new TimeSpan();
      List<QueryPreparationTimes> queryPreparationTimesList = new List<QueryPreparationTimes>();
      TimeSpan indexLookupTime = new TimeSpan();
      TimeSpan documentLoadTime = new TimeSpan();
      TimeSpan vmExecutionTime = new TimeSpan();
      List<RuntimeExecutionTimes> runtimeExecutionTimesList = new List<RuntimeExecutionTimes>();
      TimeSpan documentWriteTime = new TimeSpan();
      List<ClientSideMetrics> clientSideMetricsList = new List<ClientSideMetrics>();
      foreach (QueryMetrics queryMetrics in queryMetricsList)
      {
        if (queryMetrics == null)
          throw new ArgumentNullException("queryMetricsList can not have null elements");
        retrievedDocumentCount += queryMetrics.retrievedDocumentCount;
        retrievedDocumentSize += queryMetrics.retrievedDocumentSize;
        outputDocumentCount += queryMetrics.outputDocumentCount;
        outputDocumentSize += queryMetrics.outputDocumentSize;
        indexHitDocumentCount += queryMetrics.indexHitDocumentCount;
        indexUtilizationInfoList.Add(queryMetrics.IndexUtilizationInfo);
        totalQueryExecutionTime += queryMetrics.totalQueryExecutionTime;
        queryPreparationTimesList.Add(queryMetrics.queryPreparationTimes);
        indexLookupTime += queryMetrics.indexLookupTime;
        documentLoadTime += queryMetrics.documentLoadTime;
        vmExecutionTime += queryMetrics.vmExecutionTime;
        runtimeExecutionTimesList.Add(queryMetrics.runtimeExecutionTimes);
        documentWriteTime += queryMetrics.documentWriteTime;
        clientSideMetricsList.Add(queryMetrics.clientSideMetrics);
      }
      return new QueryMetrics(retrievedDocumentCount, retrievedDocumentSize, outputDocumentCount, outputDocumentSize, indexHitDocumentCount, IndexUtilizationInfo.CreateFromIEnumerable((IEnumerable<IndexUtilizationInfo>) indexUtilizationInfoList), totalQueryExecutionTime, QueryPreparationTimes.CreateFromIEnumerable((IEnumerable<QueryPreparationTimes>) queryPreparationTimesList), indexLookupTime, documentLoadTime, vmExecutionTime, RuntimeExecutionTimes.CreateFromIEnumerable((IEnumerable<RuntimeExecutionTimes>) runtimeExecutionTimesList), documentWriteTime, ClientSideMetrics.CreateFromIEnumerable((IEnumerable<ClientSideMetrics>) clientSideMetricsList));
    }

    internal static QueryMetrics CreateFromDelimitedString(
      string delimitedString,
      string indexUtilization)
    {
      return QueryMetrics.CreateFromDelimitedStringAndClientSideMetrics(delimitedString, indexUtilization, new ClientSideMetrics(0L, 0.0, (IEnumerable<FetchExecutionRange>) new List<FetchExecutionRange>(), (IEnumerable<Tuple<string, SchedulingTimeSpan>>) new List<Tuple<string, SchedulingTimeSpan>>()));
    }

    internal static QueryMetrics CreateFromDelimitedStringAndClientSideMetrics(
      string delimitedString,
      string indexUtilization,
      ClientSideMetrics clientSideMetrics)
    {
      Dictionary<string, double> delimitedString1 = QueryMetricsUtils.ParseDelimitedString(delimitedString);
      double num;
      delimitedString1.TryGetValue("indexUtilizationRatio", out num);
      double retrievedDocumentCount;
      delimitedString1.TryGetValue("retrievedDocumentCount", out retrievedDocumentCount);
      long indexHitDocumentCount = (long) (num * retrievedDocumentCount);
      double outputDocumentCount;
      delimitedString1.TryGetValue("outputDocumentCount", out outputDocumentCount);
      double outputDocumentSize;
      delimitedString1.TryGetValue("outputDocumentSize", out outputDocumentSize);
      double retrievedDocumentSize;
      delimitedString1.TryGetValue("retrievedDocumentSize", out retrievedDocumentSize);
      TimeSpan totalQueryExecutionTime = QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "totalExecutionTimeInMs");
      IndexUtilizationInfo result;
      IndexUtilizationInfo.TryCreateFromDelimitedString(indexUtilization, out result);
      return new QueryMetrics((long) retrievedDocumentCount, (long) retrievedDocumentSize, (long) outputDocumentCount, (long) outputDocumentSize, indexHitDocumentCount, result, totalQueryExecutionTime, QueryPreparationTimes.CreateFromDelimitedString(delimitedString), QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "indexLookupTimeInMs"), QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "documentLoadTimeInMs"), QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "VMExecutionTimeInMs"), RuntimeExecutionTimes.CreateFromDelimitedString(delimitedString), QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "writeOutputTimeInMs"), clientSideMetrics);
    }

    internal static QueryMetrics CreateWithSchedulingMetrics(
      QueryMetrics queryMetrics,
      List<Tuple<string, SchedulingTimeSpan>> partitionSchedulingTimeSpans)
    {
      return new QueryMetrics(queryMetrics.RetrievedDocumentCount, queryMetrics.RetrievedDocumentSize, queryMetrics.OutputDocumentCount, queryMetrics.OutputDocumentSize, queryMetrics.IndexHitDocumentCount, queryMetrics.IndexUtilizationInfo, queryMetrics.TotalQueryExecutionTime, queryMetrics.QueryPreparationTimes, queryMetrics.IndexLookupTime, queryMetrics.DocumentLoadTime, queryMetrics.VMExecutionTime, queryMetrics.RuntimeExecutionTimes, queryMetrics.DocumentWriteTime, new ClientSideMetrics(queryMetrics.ClientSideMetrics.Retries, queryMetrics.ClientSideMetrics.RequestCharge, queryMetrics.ClientSideMetrics.FetchExecutionRanges, (IEnumerable<Tuple<string, SchedulingTimeSpan>>) partitionSchedulingTimeSpans));
    }

    internal QueryMetrics Add(params QueryMetrics[] queryMetricsList)
    {
      List<QueryMetrics> queryMetricsList1 = new List<QueryMetrics>(queryMetricsList.Length + 1);
      queryMetricsList1.Add(this);
      queryMetricsList1.AddRange((IEnumerable<QueryMetrics>) queryMetricsList);
      return QueryMetrics.CreateFromIEnumerable((IEnumerable<QueryMetrics>) queryMetricsList1);
    }
  }
}
