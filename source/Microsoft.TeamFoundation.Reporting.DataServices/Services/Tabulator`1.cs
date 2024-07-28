// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.Tabulator`1
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public class Tabulator<RecordType> : ITabulator<RecordType>, ICountLoad
  {
    private ResultsTable m_resultsTable = new ResultsTable();
    private LoadCounter m_acceptedLoadCounter = new LoadCounter();
    public const string rowColumnFilterBasedOnTagsFeatureFlag = "WebAccess.Widgets.RowColumnFilterBasedOnTags";

    public TransformInstructions<RecordType> TabulationInstructions { get; set; }

    public IInterpretTimedData<RecordType> RecordInterpreter { get; set; }

    public int CountedIterations => this.m_acceptedLoadCounter.CountedIterations;

    public TimeSpan ElapsedProcessingTime => this.m_acceptedLoadCounter.ElapsedProcessingTime;

    public TimeSpan ElapsedUserTime => this.m_acceptedLoadCounter.ElapsedUserTime;

    public void Tabulate(IEnumerable<RecordType> recordBuffer, IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IEnumerable<RecordType>>(recordBuffer, "record");
      bool flag = requestContext.IsFeatureEnabled("WebAccess.Widgets.RowColumnFilterBasedOnTags");
      foreach (RecordType recordType in recordBuffer)
      {
        ArgumentUtility.CheckGenericForNull((object) recordType, "record");
        if (flag)
          this.TabulateRecordForRowColFilter(recordType);
        else
          this.TabulateRecord(recordType);
      }
    }

    public void TabulateRecord(RecordType record)
    {
      AggregatedRecord sampledRecord;
      if (!this.RecordInterpreter.SamplePoint(record, this.TabulationInstructions, out sampledRecord) || sampledRecord == null)
        return;
      this.m_acceptedLoadCounter.Start();
      if (sampledRecord.Group != null && sampledRecord.Group.GetType().IsArray)
      {
        foreach (object group in sampledRecord.Group as IEnumerable)
          this.TabulateSamplePoint(record, sampledRecord, group);
      }
      else
        this.TabulateSamplePoint(record, sampledRecord, sampledRecord.Group);
      this.m_acceptedLoadCounter.Stop();
    }

    public void TabulateRecordForRowColFilter(RecordType record)
    {
      AggregatedRecord sampledRecord;
      if (!this.RecordInterpreter.SamplePoint(record, this.TabulationInstructions, out sampledRecord) || sampledRecord == null)
        return;
      this.m_acceptedLoadCounter.Start();
      if (sampledRecord.Group != null && sampledRecord.Group.GetType().IsArray)
      {
        foreach (object group in sampledRecord.Group as IEnumerable)
        {
          if (this.IsValidGroupForTabulation(group))
            this.TabulateSamplePointForRowColFilter(record, sampledRecord, group);
        }
      }
      else if (this.IsValidGroupForTabulation(sampledRecord.Group))
        this.TabulateSamplePointForRowColFilter(record, sampledRecord, sampledRecord.Group);
      this.m_acceptedLoadCounter.Stop();
    }

    public TransformResult PackResultsForLocalZoneTime(
      TimeZoneInfo localTimeZoneInfo,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TransformResult transformResult = new TransformResult();
      transformResult.Records = this.GetTabulatedResults(localTimeZoneInfo, cancellationToken);
      transformResult.Options = this.TabulationInstructions.Options;
      transformResult.AreHistoryOptionsUnmodified = true;
      transformResult.SetSecuredObject((ISecuredObject) this.TabulationInstructions.Options);
      return transformResult;
    }

    private IEnumerable<AggregatedRecord> GetTabulatedResults(
      TimeZoneInfo localTimeZoneInfo,
      CancellationToken cancellationToken)
    {
      if (this.TabulationInstructions.ChartDimensionality.IsTrend)
        this.m_resultsTable.EnsureAllDatesExist(this.TabulationInstructions.HistorySamplePoints, this.TabulationInstructions.ChartDimensionality.TotalDimensions == 1);
      this.m_resultsTable.FillInMissingGroups(cancellationToken);
      if (this.TabulationInstructions.ChartDimensionality.IsTrend)
        this.m_resultsTable.ConvertTrendFromUtc(localTimeZoneInfo, this.TabulationInstructions.ChartDimensionality.TotalDimensions == 1);
      return this.m_resultsTable.GetSortedResults(this.TabulationInstructions.Options, this.TabulationInstructions.ChartDimensionality, cancellationToken);
    }

    private void TabulateSamplePoint(
      RecordType record,
      AggregatedRecord sampledValue,
      object group)
    {
      if (sampledValue.Series != null && sampledValue.Series.GetType().IsArray)
      {
        foreach (object obj in sampledValue.Series as IEnumerable)
        {
          if (!this.IsGroupNotPresentInFilter(obj))
            this.TabulateSamplePoint(record, sampledValue, obj, group);
        }
      }
      else
      {
        if (this.IsGroupNotPresentInFilter(group))
          return;
        this.TabulateSamplePoint(record, sampledValue, sampledValue.Series, group);
      }
    }

    private void TabulateSamplePointForRowColFilter(
      RecordType record,
      AggregatedRecord sampledValue,
      object group)
    {
      if (sampledValue.Series != null && sampledValue.Series.GetType().IsArray)
      {
        foreach (object series in sampledValue.Series as IEnumerable)
        {
          if (this.ShouldTabulateSamplePoint(series, group, true))
            this.TabulateSamplePoint(record, sampledValue, series, group);
        }
      }
      else
      {
        if (!this.ShouldTabulateSamplePoint(sampledValue.Series, group, false))
          return;
        this.TabulateSamplePoint(record, sampledValue, sampledValue.Series, group);
      }
    }

    private void TabulateSamplePoint(
      RecordType record,
      AggregatedRecord sampledValue,
      object series,
      object group)
    {
      AggregatedRecord.AggregationRecordKey key = AggregatedRecord.CreateKey(series, group);
      double num = (double) this.TabulationInstructions.AggregationStrategy.GetValue(record);
      AggregatedRecord tabulatedValue;
      if (this.m_resultsTable.TryGetValue((object) key, out tabulatedValue))
      {
        this.TabulationInstructions.AggregationStrategy.UpdateAggregateRecord(sampledValue.Measure, tabulatedValue, record);
      }
      else
      {
        float measure = sampledValue.Measure;
        AggregatedRecord aggregatedRecord = new AggregatedRecord(series, group);
        aggregatedRecord.Measure = 0.0f;
        this.TabulationInstructions.AggregationStrategy.UpdateAggregateRecord(measure, aggregatedRecord, record);
        this.m_resultsTable.RecordNewEntry(aggregatedRecord);
      }
    }

    private bool IsGroupNotPresentInFilter(object group)
    {
      string[] filteredGroups = this.TabulationInstructions.Options?.FilteredGroups;
      return filteredGroups != null && ((IEnumerable<string>) filteredGroups).Count<string>() > 0 && !((IEnumerable<object>) filteredGroups).Contains<object>(group);
    }

    private bool ShouldTabulateSamplePoint(object series, object group, bool tabulatebasedonseries)
    {
      if (this.TabulationInstructions.Options.IsRowColFilterAllowedForChartType)
      {
        string[] rowFilteredGroups = this.TabulationInstructions.Options.RowFilteredGroups;
        return (rowFilteredGroups != null ? (rowFilteredGroups.Length != 0 ? 1 : 0) : 0) == 0 || this.IsRowFilterHasSeriesValue(series, this.TabulationInstructions.Options.RowFilteredGroups);
      }
      return !this.IsGroupNotPresentInFilter(tabulatebasedonseries ? series : group);
    }

    private bool IsRowFilterHasSeriesValue(object series, string[] rowTags) => series != null && ((IEnumerable<string>) rowTags).Contains<string>(series.ToString());

    private bool IsValidGroupForTabulation(object group)
    {
      string[] columnFilteredGroups = this.TabulationInstructions.Options.ColumnFilteredGroups;
      return (columnFilteredGroups != null ? (columnFilteredGroups.Length != 0 ? 1 : 0) : 0) == 0 || !this.TabulationInstructions.Options.IsRowColFilterAllowedForChartType || this.IsColumnFilterHasGroupValue(group, this.TabulationInstructions.Options.ColumnFilteredGroups);
    }

    private bool IsColumnFilterHasGroupValue(object group, string[] columnTags) => group != null && ((IEnumerable<string>) columnTags).Contains<string>(group.ToString());
  }
}
