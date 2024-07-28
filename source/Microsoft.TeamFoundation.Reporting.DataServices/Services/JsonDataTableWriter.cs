// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.JsonDataTableWriter
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public class JsonDataTableWriter : IDataServicesWriter
  {
    private JsonDataFormatter m_jsonDataFormatterFormatter;
    private IVssRequestContext m_requestContext;
    private const string m_oneDimensionalDataLabel = "oneDimensional";
    private const string m_noSeries = "----------";
    private const string m_resultPackageName = "result";
    private const string m_dataSectionName = "data";
    private const string m_keyName = "key";
    private const string m_valueName = "value";
    private const string m_queryText = "queryText";
    private const string trendDateField = "System.CreatedDate";
    private const string tagsField = "System.Tags";
    private const string m_keyCustomData = "customData";
    private const string ProjectId = "ProjectId";
    private const string QueryText = "QueryText";
    private const string areaPathField = "System.AreaPath";
    private const string iterationPathField = "System.IterationPath";

    public IInterpretQueryText QueryInterpreter { get; set; }

    public void WriteResultsToStream(Stream stream, IEnumerable<TransformResult> results)
    {
      if (this.RequestContext == null)
        throw new InvalidOperationException("RequestContext cannot be null");
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      if (results == null)
        throw new ArgumentNullException(nameof (results));
      this.RequestContext.TraceEnter(1017911, "Reporting", nameof (JsonDataTableWriter), "WriteRecordsToStream");
      try
      {
        IEnumerator<TransformResult> resultEnumerator = this.GetResultEnumerator(results);
        using (ChartingJsonWriter jsonWriter = new ChartingJsonWriter(stream))
        {
          jsonWriter.WriteStartObjectArrayPair("result");
          do
          {
            this.WriteTransforms(jsonWriter, resultEnumerator.Current);
          }
          while (resultEnumerator.MoveNext());
          jsonWriter.WriteEndObjectArrayPair();
        }
      }
      catch (Exception ex)
      {
        TelemetryHelper.PublishChartingRequestFailure(this.m_requestContext, "Framework", nameof (WriteResultsToStream), ex);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave(1017910, "Reporting", nameof (JsonDataTableWriter), "WriteRecordsToStream");
      }
    }

    private void WriteTransforms(ChartingJsonWriter jsonWriter, TransformResult currentResult)
    {
      jsonWriter.WriteStartObject();
      this.WriteOptions(jsonWriter, currentResult.Options);
      jsonWriter.WritePropertyName("data");
      jsonWriter.WriteStartArray();
      ChartDimensionality dimensionality = ChartDimensionality.FromTransformOptions(currentResult.Options, currentResult.AreHistoryOptionsUnmodified);
      this.WriteRecordsToStream(jsonWriter, currentResult, dimensionality);
      jsonWriter.WriteEndObjectArrayPair();
    }

    private void WriteRecordsToStream(
      ChartingJsonWriter jsonWriter,
      TransformResult currentReults,
      ChartDimensionality dimensionality)
    {
      string str = "----------";
      foreach (AggregatedRecord record in currentReults.Records)
      {
        FormattedRecord formattedRecord = this.m_jsonDataFormatterFormatter.FormatRecord(record);
        string seriesName = this.GetSeriesName(formattedRecord, dimensionality.IsOneDimensionalSnapshot);
        if (!str.Equals(seriesName, StringComparison.OrdinalIgnoreCase))
        {
          if (str != "----------")
            jsonWriter.WriteEndObjectArrayPair();
          str = seriesName;
          string keyValue = (dimensionality.TotalDimensions == 1 || dimensionality.IsTrend ? (currentReults.Options.GroupBy.Equals("System.AreaPath", StringComparison.OrdinalIgnoreCase) ? 1 : (currentReults.Options.GroupBy.Equals("System.IterationPath", StringComparison.OrdinalIgnoreCase) ? 1 : 0)) : (currentReults.Options.Series.Equals("System.AreaPath", StringComparison.OrdinalIgnoreCase) ? 1 : (currentReults.Options.Series.Equals("System.IterationPath", StringComparison.OrdinalIgnoreCase) ? 1 : 0))) != 0 ? str.Substring(Math.Max(seriesName.LastIndexOf('\\') + 1, 0)) : str;
          jsonWriter.WriteStartObjectArrayPair("key", keyValue, "value");
        }
        this.WriteRecordToStream(jsonWriter, formattedRecord, currentReults.Options.GroupBy, !dimensionality.IsTrend || dimensionality.TotalDimensions <= 1 ? currentReults.Options.Series : seriesName, record, dimensionality, currentReults.Options.GetFilteredQuery);
      }
      if (!(str != "----------"))
        return;
      jsonWriter.WriteEndObjectArrayPair();
    }

    private string GetSeriesName(FormattedRecord formattedRecord, bool isOneDimensionalSnapshot)
    {
      string seriesName = formattedRecord.Series;
      if (isOneDimensionalSnapshot)
        seriesName = "oneDimensional";
      return seriesName;
    }

    private void WriteRecordToStream(
      ChartingJsonWriter jsonWriter,
      FormattedRecord record,
      string groupBy,
      string series,
      AggregatedRecord dataset,
      ChartDimensionality dimensionality,
      bool getFilteredQuery)
    {
      jsonWriter.WriteStartObject();
      int num1 = groupBy.Equals("System.AreaPath", StringComparison.OrdinalIgnoreCase) ? 1 : (groupBy.Equals("System.IterationPath", StringComparison.OrdinalIgnoreCase) ? 1 : 0);
      string str1 = record.Group;
      int num2 = str1.LastIndexOf('\\');
      if (num1 != 0 && num2 > -1 && num2 < str1.Length - 1)
        str1 = str1.Substring(num2 + 1);
      jsonWriter.WritePropertyNameValue("key", str1);
      jsonWriter.WritePropertyNameValue("value", record.Measure);
      object obj1;
      if (getFilteredQuery && this.RequestContext.Items.TryGetValue("QueryText", out obj1))
      {
        int num3 = !dimensionality.IsTrend ? 0 : (dimensionality.TotalDimensions == 2 ? 1 : 0);
        bool flag = dimensionality.IsTrend && dimensionality.TotalDimensions == 1;
        string filteredQuery;
        if (num3 != 0)
        {
          filteredQuery = this.BuildQueryWithWorkItemId(obj1.ToString(), string.Join<int>(",", (IEnumerable<int>) dataset.WorkItemIds));
        }
        else
        {
          string str2;
          if (string.IsNullOrEmpty(record.Series) || string.IsNullOrEmpty(series))
            str2 = string.Empty;
          else
            str2 = " and [" + (dimensionality.IsTrend ? "System.CreatedDate" : series) + "]" + (dimensionality.IsTrend ? " <= " : (string.Equals(series.ToLower(), "System.Tags".ToLower()) ? " Contains " : " = ")) + "'" + (dataset.Series != null ? record.Series.Replace("'", "''") : "") + "'";
          string seriesFilter = str2;
          string groupByFilter = " and [" + (!flag || !string.IsNullOrEmpty(groupBy) ? groupBy : "System.CreatedDate") + "]" + (!flag || !string.IsNullOrEmpty(groupBy) ? (string.Equals(groupBy.ToLower(), "System.Tags".ToLower()) ? " Contains " : " = ") : " <= ") + "'" + (dataset.Group != null ? record.Group.Replace("'", "''") : "") + "'";
          filteredQuery = this.BuildQuery(obj1.ToString(), seriesFilter, groupByFilter);
          object obj2;
          this.RequestContext.Items.TryGetValue("ProjectId", out obj2);
          if (string.IsNullOrEmpty(filteredQuery) || !this.QueryInterpreter.IsValidQueryText(this.m_requestContext, filteredQuery.ToString(), obj2?.ToString() ?? ""))
          {
            this.RequestContext.Trace(1017915, TraceLevel.Verbose, "Reporting", nameof (JsonDataTableWriter), "Invalid query text: " + filteredQuery + " --- Chart Dimensionality: " + JsonConvert.SerializeObject((object) dimensionality) + " --- Chart Json: " + JsonConvert.SerializeObject((object) jsonWriter) + " --- " + groupBy + " --- " + series + " --- " + JsonConvert.SerializeObject((object) record));
            filteredQuery = this.BuildQueryWithWorkItemId(obj1.ToString(), string.Join<int>(",", (IEnumerable<int>) dataset.WorkItemIds));
          }
        }
        this.WriteRecordToStream(jsonWriter, filteredQuery);
      }
      jsonWriter.WriteEndObject();
    }

    private string BuildQuery(string queryText, string seriesFilter, string groupByFilter)
    {
      try
      {
        NodeSelect syntax = Parser.ParseSyntax(queryText);
        string str = (syntax.Where?.ToString() ?? "") + seriesFilter + groupByFilter;
        string queryText1 = string.Format("{0}{1}{2}{3}{4}{5}", (object) "Select ", (object) syntax.Fields, (object) " From ", (object) syntax.From, (object) " Where ", (object) str);
        string queryText2 = syntax.GroupBy != null ? this.AppendClause(queryText1, "Group By", syntax.GroupBy.ToString()) : queryText1;
        string queryText3 = syntax.OrderBy != null ? this.AppendClause(queryText2, "Order By", syntax.OrderBy.ToString()) : queryText2;
        return syntax.AsOf != null ? this.AppendClause(queryText3, "AsOf", syntax.AsOf.ToString()) : queryText3;
      }
      catch (InvalidQueryStringException ex)
      {
        TelemetryHelper.PublishChartingRequestFailure(this.m_requestContext, "Framework", nameof (BuildQuery), (Exception) ex);
        return string.Empty;
      }
    }

    private string AppendClause(string queryText, string clauseType, string clauseContent)
    {
      if (string.IsNullOrEmpty(clauseContent))
        return queryText;
      return queryText + " " + clauseType + " " + clauseContent;
    }

    private string BuildQueryWithWorkItemId(string queryText, string workItemIds)
    {
      try
      {
        NodeSelect syntax = Parser.ParseSyntax(queryText);
        workItemIds = string.IsNullOrEmpty(workItemIds) ? "0" : workItemIds;
        return string.Format("{0}{1}{2}{3}{4}{5}{6}", (object) "Select ", (object) syntax.Fields, (object) " From  ", (object) syntax.From, (object) " Where [System.Id] In (", (object) workItemIds, (object) ")");
      }
      catch (InvalidQueryStringException ex)
      {
        TelemetryHelper.PublishChartingRequestFailure(this.m_requestContext, "Framework", nameof (BuildQueryWithWorkItemId), (Exception) ex);
        return string.Empty;
      }
    }

    private void WriteRecordToStream(ChartingJsonWriter jsonWriter, string filteredQuery)
    {
      jsonWriter.WritePropertyName("customData");
      jsonWriter.WriteStartObject();
      jsonWriter.WritePropertyNameValue("queryText", filteredQuery);
      jsonWriter.WriteEndObject();
    }

    private void WriteOptions(ChartingJsonWriter jsonWriter, TransformOptions options)
    {
      this.RequestContext.Trace(1017900, TraceLevel.Info, "Reporting", nameof (JsonDataTableWriter), "Writing a set of options");
      jsonWriter.WritePropertyName("transform");
      jsonWriter.WriteStartObject();
      jsonWriter.WritePropertyNameValue("filter", options.Filter);
      if (options.HistoryRange != null)
        jsonWriter.WritePropertyNameValue("series", options.HistoryRange);
      if (options.Series != null)
        jsonWriter.WritePropertyNameValue("series", options.Series);
      jsonWriter.WritePropertyNameValue("groupBy", options.GroupBy);
      JsonDataTableWriter.WriteOrderBy(jsonWriter, options.OrderBy);
      JsonDataTableWriter.WriteMeasure(jsonWriter, options.Measure);
      if (options.TransformId.HasValue)
      {
        jsonWriter.WritePropertyName("transformId");
        jsonWriter.WriteValue(options.TransformId);
      }
      jsonWriter.WriteEndObject();
    }

    private static void WriteMeasure(ChartingJsonWriter jsonWriter, Measure measure)
    {
      jsonWriter.WritePropertyName(nameof (measure));
      jsonWriter.WriteStartObject();
      jsonWriter.WritePropertyNameValue("propertyName", measure.PropertyName);
      jsonWriter.WritePropertyNameValue("aggregation", measure.Aggregation);
      jsonWriter.WriteEndObject();
    }

    private static void WriteOrderBy(ChartingJsonWriter jsonWriter, OrderBy orderBy)
    {
      jsonWriter.WritePropertyName(nameof (orderBy));
      jsonWriter.WriteStartObject();
      jsonWriter.WritePropertyNameValue("propertyName", orderBy.PropertyName);
      jsonWriter.WritePropertyNameValue("direction", orderBy.Direction);
      jsonWriter.WriteEndObject();
    }

    public string ContentType => "application/json";

    public IVssRequestContext RequestContext
    {
      get => this.m_requestContext;
      set
      {
        this.m_requestContext = value;
        this.m_jsonDataFormatterFormatter = new JsonDataFormatter();
      }
    }

    private IEnumerator<TransformResult> GetResultEnumerator(IEnumerable<TransformResult> results)
    {
      IEnumerator<TransformResult> enumerator = results.GetEnumerator();
      this.RequestContext.Trace(1017920, TraceLevel.Info, "Reporting", nameof (JsonDataTableWriter), "Moving to first result set in enumerator");
      enumerator.MoveNext();
      return enumerator;
    }
  }
}
