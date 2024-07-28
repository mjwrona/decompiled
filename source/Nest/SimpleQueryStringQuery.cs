// Decompiled with JetBrains decompiler
// Type: Nest.SimpleQueryStringQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class SimpleQueryStringQuery : QueryBase, ISimpleQueryStringQuery, IQuery
  {
    public string Analyzer { get; set; }

    public bool? AnalyzeWildcard { get; set; }

    public bool? AutoGenerateSynonymsPhraseQuery { get; set; }

    public Operator? DefaultOperator { get; set; }

    public Fields Fields { get; set; }

    public SimpleQueryStringFlags? Flags { get; set; }

    public int? FuzzyMaxExpansions { get; set; }

    public int? FuzzyPrefixLength { get; set; }

    public bool? FuzzyTranspositions { get; set; }

    public bool? Lenient { get; set; }

    public MinimumShouldMatch MinimumShouldMatch { get; set; }

    public string Query { get; set; }

    public string QuoteFieldSuffix { get; set; }

    protected override bool Conditionless => SimpleQueryStringQuery.IsConditionless((ISimpleQueryStringQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.SimpleQueryString = (ISimpleQueryStringQuery) this;

    internal static bool IsConditionless(ISimpleQueryStringQuery q) => q.Query.IsNullOrEmpty();
  }
}
