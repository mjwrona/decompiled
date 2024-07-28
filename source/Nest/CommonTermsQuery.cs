// Decompiled with JetBrains decompiler
// Type: Nest.CommonTermsQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  [Obsolete("Deprecated in 7.3.0. Use MatchQuery instead, which skips blocks of documents efficiently, without any configuration, provided that the total number of hits is not tracked.")]
  public class CommonTermsQuery : FieldNameQueryBase, ICommonTermsQuery, IFieldNameQuery, IQuery
  {
    public string Analyzer { get; set; }

    public double? CutoffFrequency { get; set; }

    public Operator? HighFrequencyOperator { get; set; }

    public Operator? LowFrequencyOperator { get; set; }

    public MinimumShouldMatch MinimumShouldMatch { get; set; }

    public string Query { get; set; }

    protected override bool Conditionless => CommonTermsQuery.IsConditionless((ICommonTermsQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.CommonTerms = (ICommonTermsQuery) this;

    internal static bool IsConditionless(ICommonTermsQuery q) => q.Field.IsConditionless() || q.Query.IsNullOrEmpty();
  }
}
