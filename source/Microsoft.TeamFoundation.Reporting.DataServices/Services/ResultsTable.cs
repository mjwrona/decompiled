// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.ResultsTable
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Reporting.DataServices.FeatureUtilities;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  internal class ResultsTable
  {
    private Dictionary<object, AggregatedRecord> m_results = new Dictionary<object, AggregatedRecord>();
    private static readonly IEqualityComparer<object> m_comparer = (IEqualityComparer<object>) new CaseInsensitiveStringOrObjectComparar();
    private readonly HashSet<object> m_groupsSet = new HashSet<object>(ResultsTable.m_comparer);
    private readonly HashSet<object> m_seriesSet = new HashSet<object>(ResultsTable.m_comparer);

    public void RecordNewEntry(AggregatedRecord tabulatedValue)
    {
      AggregatedRecord.AggregationRecordKey key = tabulatedValue.CreateKey();
      if (this.m_results.ContainsKey((object) key))
        return;
      this.m_results[(object) key] = tabulatedValue;
      this.m_groupsSet.Add(tabulatedValue.Group);
      this.m_seriesSet.Add(tabulatedValue.Series);
    }

    public bool TryGetValue(object key, out AggregatedRecord tabulatedValue)
    {
      // ISSUE: reference to a compiler-generated field
      if (ResultsTable.\u003C\u003Eo__5.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ResultsTable.\u003C\u003Eo__5.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (ResultsTable)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target = ResultsTable.\u003C\u003Eo__5.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p1 = ResultsTable.\u003C\u003Eo__5.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (ResultsTable.\u003C\u003Eo__5.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ResultsTable.\u003C\u003Eo__5.\u003C\u003Ep__0 = CallSite<\u003C\u003EF\u007B00000200\u007D<CallSite, Dictionary<object, AggregatedRecord>, object, AggregatedRecord, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, nameof (TryGetValue), (IEnumerable<Type>) null, typeof (ResultsTable), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsOut, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = ResultsTable.\u003C\u003Eo__5.\u003C\u003Ep__0.Target((CallSite) ResultsTable.\u003C\u003Eo__5.\u003C\u003Ep__0, this.m_results, key, ref tabulatedValue);
      return target((CallSite) p1, obj);
    }

    public IEnumerable<AggregatedRecord> GetSortedResults(
      TransformOptions options,
      ChartDimensionality chartDimensionality,
      CancellationToken cancellationToken)
    {
      IComparer<AggregatedRecord> comparer = ComparisonHelper.GetComparer(options, (IDictionary<object, AggregatedRecord>) this.m_results, cancellationToken);
      return ResultsTable.SortResults(options, comparer, (IDictionary<object, AggregatedRecord>) this.m_results, chartDimensionality);
    }

    public void EnsureAllDatesExist(IEnumerable<DateTime> dates, bool historyInGroup)
    {
      ArgumentUtility.CheckForNull<IEnumerable<DateTime>>(dates, nameof (dates));
      HashSet<object> objectSet = historyInGroup ? this.m_groupsSet : this.m_seriesSet;
      foreach (DateTime date in dates)
        objectSet.Add((object) date);
    }

    public void FillInMissingGroups(CancellationToken cancellationToken)
    {
      foreach (object series in this.m_seriesSet)
      {
        foreach (object groups in this.m_groupsSet)
        {
          cancellationToken.ThrowIfCancellationRequested();
          this.RecordNewEntry(new AggregatedRecord()
          {
            Group = groups,
            Series = series,
            Measure = 0.0f
          });
        }
      }
    }

    internal static IEnumerable<AggregatedRecord> SortResults(
      TransformOptions options,
      IComparer<AggregatedRecord> comparer,
      IDictionary<object, AggregatedRecord> results,
      ChartDimensionality dimensionality)
    {
      IEnumerable<AggregatedRecord> aggregatedRecords;
      if (dimensionality.IsTrend)
      {
        bool useGroup = dimensionality.TotalDimensions == 1;
        IOrderedEnumerable<AggregatedRecord> source = results.Values.OrderBy<AggregatedRecord, object>((Func<AggregatedRecord, object>) (x => !useGroup ? x.Series : x.Group));
        aggregatedRecords = options.OrderBy.Direction == "descending" ? (IEnumerable<AggregatedRecord>) source.ThenByDescending<AggregatedRecord, AggregatedRecord>((Func<AggregatedRecord, AggregatedRecord>) (x => x), comparer) : (IEnumerable<AggregatedRecord>) source.ThenBy<AggregatedRecord, AggregatedRecord>((Func<AggregatedRecord, AggregatedRecord>) (x => x), comparer);
      }
      else
        aggregatedRecords = options.OrderBy.Direction == "descending" ? (IEnumerable<AggregatedRecord>) results.Values.OrderByDescending<AggregatedRecord, AggregatedRecord>((Func<AggregatedRecord, AggregatedRecord>) (x => x), comparer) : (IEnumerable<AggregatedRecord>) results.Values.OrderBy<AggregatedRecord, AggregatedRecord>((Func<AggregatedRecord, AggregatedRecord>) (x => x), comparer);
      return aggregatedRecords;
    }

    internal void ConvertTrendFromUtc(TimeZoneInfo localTimeZoneInfo, bool historyInGroup)
    {
      foreach (AggregatedRecord aggregatedRecord in this.m_results.Values)
      {
        if (historyInGroup)
          aggregatedRecord.Group = (object) ((DateTime) aggregatedRecord.Group).FromUtc(localTimeZoneInfo);
        else
          aggregatedRecord.Series = (object) ((DateTime) aggregatedRecord.Series).FromUtc(localTimeZoneInfo);
      }
    }
  }
}
