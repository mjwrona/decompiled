// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.AgregatedRecordValueComparer
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public class AgregatedRecordValueComparer : IComparer<AggregatedRecord>
  {
    private readonly TransformOptions m_options;
    private readonly Dictionary<object, float> m_seriesRanks;
    private readonly Dictionary<object, float> m_groupRanks;
    private CancellationToken m_cancellationToken;
    private static readonly object m_nullKey = new object();
    private static readonly IEqualityComparer<object> m_comparer = (IEqualityComparer<object>) new CaseInsensitiveStringOrObjectComparar();

    public AgregatedRecordValueComparer(
      TransformOptions options,
      IDictionary<object, AggregatedRecord> results,
      CancellationToken cancellationToken)
    {
      this.m_options = options;
      this.m_groupRanks = results.GroupBy<KeyValuePair<object, AggregatedRecord>, object>((Func<KeyValuePair<object, AggregatedRecord>, object>) (x => x.Value.Group), AgregatedRecordValueComparer.m_comparer).ToDictionary<IGrouping<object, KeyValuePair<object, AggregatedRecord>>, object, float>((Func<IGrouping<object, KeyValuePair<object, AggregatedRecord>>, object>) (x => x.Key ?? AgregatedRecordValueComparer.m_nullKey), (Func<IGrouping<object, KeyValuePair<object, AggregatedRecord>>, float>) (x => x.Sum<KeyValuePair<object, AggregatedRecord>>((Func<KeyValuePair<object, AggregatedRecord>, float>) (y => y.Value.Measure))), AgregatedRecordValueComparer.m_comparer);
      this.m_seriesRanks = results.GroupBy<KeyValuePair<object, AggregatedRecord>, object>((Func<KeyValuePair<object, AggregatedRecord>, object>) (x => x.Value.Series), AgregatedRecordValueComparer.m_comparer).ToDictionary<IGrouping<object, KeyValuePair<object, AggregatedRecord>>, object, float>((Func<IGrouping<object, KeyValuePair<object, AggregatedRecord>>, object>) (x => x.Key ?? AgregatedRecordValueComparer.m_nullKey), (Func<IGrouping<object, KeyValuePair<object, AggregatedRecord>>, float>) (x => x.Sum<KeyValuePair<object, AggregatedRecord>>((Func<KeyValuePair<object, AggregatedRecord>, float>) (y => y.Value.Measure))), AgregatedRecordValueComparer.m_comparer);
      this.m_cancellationToken = cancellationToken;
    }

    public int Compare(AggregatedRecord x, AggregatedRecord y)
    {
      this.m_cancellationToken.ThrowIfCancellationRequested();
      return this.CompareWeightedValueSets(this.BuildSet(x), this.BuildSet(y));
    }

    private object[] BuildSet(AggregatedRecord record) => string.IsNullOrEmpty(this.m_options.Series) ? new object[2]
    {
      (object) this.m_groupRanks[record.Group ?? AgregatedRecordValueComparer.m_nullKey],
      record.Group
    } : new object[4]
    {
      (object) this.m_seriesRanks[record.Series ?? AgregatedRecordValueComparer.m_nullKey],
      record.Series,
      (object) this.m_groupRanks[record.Group ?? AgregatedRecordValueComparer.m_nullKey],
      record.Group
    };

    public int CompareWeightedValueSets(object[] set1, object[] set2)
    {
      for (int index = 0; index < set1.Length; ++index)
      {
        int num = this.CompareNullableProperty(set1[index], set2[index]);
        if (num != 0)
          return num;
      }
      return 0;
    }

    public int CompareNullableProperty(object x, object y)
    {
      if (x == null && y == null)
        return 0;
      if (x == null)
        return -1;
      if (y == null)
        return 1;
      switch (x)
      {
        case string _:
          return StringComparer.OrdinalIgnoreCase.Compare(x, y);
        case IComparable comparable:
          return comparable.CompareTo(y);
        default:
          return -1;
      }
    }
  }
}
