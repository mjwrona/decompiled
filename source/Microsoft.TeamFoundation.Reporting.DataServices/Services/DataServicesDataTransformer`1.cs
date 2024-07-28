// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.DataServicesDataTransformer`1
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public abstract class DataServicesDataTransformer<T> : 
    IDataServicesDataTransformer,
    IDataServicesService,
    IInterpretRecord<T>
  {
    private readonly IDictionary<object, AggregatedRecord> m_results = (IDictionary<object, AggregatedRecord>) new Dictionary<object, AggregatedRecord>();
    private bool m_hasEnumerated;
    private const string Valuelookup = "ValueLookup";
    private const string Total = "total";
    private static readonly IEqualityComparer<object> m_comparer = (IEqualityComparer<object>) new CaseInsensitiveStringOrObjectComparar();
    private readonly HashSet<object> m_groups = new HashSet<object>(DataServicesDataTransformer<T>.m_comparer);
    private readonly HashSet<object> m_series = new HashSet<object>(DataServicesDataTransformer<T>.m_comparer);
    private IAggregationStrategy<T> m_measureStrategy;
    private readonly IDictionary<string, long> m_elapsedTime = (IDictionary<string, long>) new Dictionary<string, long>()
    {
      {
        "ValueLookup",
        0L
      },
      {
        "total",
        0L
      }
    };

    protected virtual void AddRecord(T record)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      object series = (object) null;
      if (!string.IsNullOrEmpty(this.Options.Series))
        series = this.GetRecordValue(this.Options.Series, record);
      object recordValue = this.GetRecordValue(this.Options.GroupBy, record);
      if (this.m_measureStrategy == null)
        this.m_measureStrategy = AggregationMediator.GetStrategy<T>(this.RequestContext, (IInterpretRecord<T>) this, this.Options.Measure);
      float recordMeasureValue = this.m_measureStrategy.GetValue(record);
      this.m_elapsedTime["ValueLookup"] += stopwatch.ElapsedTicks;
      AggregatedRecord aggregateRecord = this.EnsureAggregateRecord(series, recordValue);
      this.m_measureStrategy.UpdateAggregateRecord(recordMeasureValue, aggregateRecord, record);
    }

    private AggregatedRecord EnsureAggregateRecord(object series, object group)
    {
      AggregatedRecord.AggregationRecordKey key = AggregatedRecord.CreateKey(series, group);
      AggregatedRecord aggregatedRecord;
      if (!this.m_results.TryGetValue((object) key, out aggregatedRecord))
      {
        this.m_groups.Add(group);
        this.m_series.Add(series);
        aggregatedRecord = new AggregatedRecord()
        {
          Series = series,
          Group = group,
          Measure = 0.0f
        };
        this.m_results.Add((object) key, aggregatedRecord);
      }
      return aggregatedRecord;
    }

    public abstract object GetRecordValue(string recordPropertyName, T record);

    public IVssRequestContext RequestContext { get; set; }

    public TransformOptions Options { get; set; }

    public void AddRecords(IEnumerable recordBuffer)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (this.RequestContext == null)
        throw new InvalidOperationException("RequestContext Is Null. Cannot add records.");
      this.RequestContext.TraceEnter(1017800, "Reporting", "DataTransformer", "AddRecord");
      try
      {
        ArgumentUtility.CheckForNull<IEnumerable>(recordBuffer, "record");
        if (this.m_hasEnumerated)
          throw new InvalidOperationException("Cannot add Records after calling GetTransformedResults.");
        foreach (object var in recordBuffer)
        {
          ArgumentUtility.CheckForNull<object>(var, "record");
          if (!(var is T record))
            throw new ArgumentException(string.Format("Expected {0} but record is {1}", (object) typeof (T), (object) var.GetType()), nameof (recordBuffer));
          this.AddRecord(record);
        }
      }
      finally
      {
        this.RequestContext.TraceLeave(1017810, "Reporting", "DataTransformer", "AddRecord");
        this.m_elapsedTime["total"] += stopwatch.ElapsedTicks;
      }
    }

    public IEnumerable<AggregatedRecord> GetTransformedResults()
    {
      this.RequestContext.TraceEnter(1017820, "Reporting", "DataTransformer", nameof (GetTransformedResults));
      try
      {
        this.m_hasEnumerated = true;
        this.FillInMissingGroups(this.m_series, this.m_groups, this.m_results);
        return this.SortResults(this.Options.OrderBy, this.m_results);
      }
      finally
      {
        this.RequestContext.Trace(1017830, TraceLevel.Info, "Reporting", "DataTransformer", this.m_elapsedTime.Aggregate<KeyValuePair<string, long>, string>(string.Empty, (Func<string, KeyValuePair<string, long>, string>) ((current, time) => current + string.Format("{0}:{1} ", (object) time.Key, (object) time.Value))));
        this.RequestContext.TraceLeave(1017830, "Reporting", "DataTransformer", nameof (GetTransformedResults));
      }
    }

    protected virtual IEnumerable<AggregatedRecord> SortResults(
      OrderBy orderBy,
      IDictionary<object, AggregatedRecord> results)
    {
      IComparer<AggregatedRecord> comparer = this.GetComparer(orderBy, results);
      return !(this.Options.OrderBy.Direction == "descending") ? (IEnumerable<AggregatedRecord>) this.m_results.Values.OrderBy<AggregatedRecord, AggregatedRecord>((Func<AggregatedRecord, AggregatedRecord>) (x => x), comparer) : (IEnumerable<AggregatedRecord>) this.m_results.Values.OrderByDescending<AggregatedRecord, AggregatedRecord>((Func<AggregatedRecord, AggregatedRecord>) (x => x), comparer);
    }

    protected virtual IComparer<AggregatedRecord> GetComparer(
      OrderBy orderBy,
      IDictionary<object, AggregatedRecord> results,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return orderBy.PropertyName == "value" ? (IComparer<AggregatedRecord>) new AgregatedRecordValueComparer(this.Options, this.m_results, cancellationToken) : (IComparer<AggregatedRecord>) new AgregatedRecordLabelComparer(this.Options);
    }

    protected virtual void FillInMissingGroups(
      HashSet<object> seriesSet,
      HashSet<object> groupsSet,
      IDictionary<object, AggregatedRecord> results)
    {
      foreach (object series in seriesSet)
      {
        foreach (object groups in groupsSet)
        {
          AggregatedRecord.AggregationRecordKey key = AggregatedRecord.CreateKey(series, groups);
          if (!results.ContainsKey((object) key))
            results[(object) key] = new AggregatedRecord()
            {
              Series = series,
              Group = groups,
              Measure = 0.0f
            };
        }
      }
    }

    public abstract int GetWorkIemId(T record);
  }
}
