// Decompiled with JetBrains decompiler
// Type: Nest.CommonTermsQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  [Obsolete("Deprecated in 7.3.0. Use MatchQuery instead, which skips blocks of documents efficiently, without any configuration, provided that the total number of hits is not tracked.")]
  public class CommonTermsQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<CommonTermsQueryDescriptor<T>, ICommonTermsQuery, T>,
    ICommonTermsQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => CommonTermsQuery.IsConditionless((ICommonTermsQuery) this);

    string ICommonTermsQuery.Analyzer { get; set; }

    double? ICommonTermsQuery.CutoffFrequency { get; set; }

    Nest.Field IFieldNameQuery.Field { get; set; }

    Operator? ICommonTermsQuery.HighFrequencyOperator { get; set; }

    Operator? ICommonTermsQuery.LowFrequencyOperator { get; set; }

    Nest.MinimumShouldMatch ICommonTermsQuery.MinimumShouldMatch { get; set; }

    string IQuery.Name { get; set; }

    string ICommonTermsQuery.Query { get; set; }

    public CommonTermsQueryDescriptor<T> Query(string query) => this.Assign<string>(query, (Action<ICommonTermsQuery, string>) ((a, v) => a.Query = v));

    public CommonTermsQueryDescriptor<T> HighFrequencyOperator(Operator? op) => this.Assign<Operator?>(op, (Action<ICommonTermsQuery, Operator?>) ((a, v) => a.HighFrequencyOperator = v));

    public CommonTermsQueryDescriptor<T> LowFrequencyOperator(Operator? op) => this.Assign<Operator?>(op, (Action<ICommonTermsQuery, Operator?>) ((a, v) => a.LowFrequencyOperator = v));

    public CommonTermsQueryDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<ICommonTermsQuery, string>) ((a, v) => a.Analyzer = v));

    public CommonTermsQueryDescriptor<T> CutoffFrequency(double? cutOffFrequency) => this.Assign<double?>(cutOffFrequency, (Action<ICommonTermsQuery, double?>) ((a, v) => a.CutoffFrequency = v));

    public CommonTermsQueryDescriptor<T> MinimumShouldMatch(Nest.MinimumShouldMatch minimumShouldMatch) => this.Assign<Nest.MinimumShouldMatch>(minimumShouldMatch, (Action<ICommonTermsQuery, Nest.MinimumShouldMatch>) ((a, v) => a.MinimumShouldMatch = v));
  }
}
