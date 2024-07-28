// Decompiled with JetBrains decompiler
// Type: Nest.QueryStringQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class QueryStringQuery : QueryBase, IQueryStringQuery, IQuery
  {
    public bool? AllowLeadingWildcard { get; set; }

    public string Analyzer { get; set; }

    public bool? AnalyzeWildcard { get; set; }

    public bool? AutoGenerateSynonymsPhraseQuery { get; set; }

    public Field DefaultField { get; set; }

    public Operator? DefaultOperator { get; set; }

    public bool? EnablePositionIncrements { get; set; }

    public bool? Escape { get; set; }

    public Fields Fields { get; set; }

    public Fuzziness Fuzziness { get; set; }

    public int? FuzzyMaxExpansions { get; set; }

    public int? FuzzyPrefixLength { get; set; }

    public MultiTermQueryRewrite FuzzyRewrite { get; set; }

    public bool? FuzzyTranspositions { get; set; }

    public bool? Lenient { get; set; }

    public int? MaximumDeterminizedStates { get; set; }

    public MinimumShouldMatch MinimumShouldMatch { get; set; }

    public double? PhraseSlop { get; set; }

    public string Query { get; set; }

    public string QuoteAnalyzer { get; set; }

    public string QuoteFieldSuffix { get; set; }

    public MultiTermQueryRewrite Rewrite { get; set; }

    public double? TieBreaker { get; set; }

    public string TimeZone { get; set; }

    public TextQueryType? Type { get; set; }

    protected override bool Conditionless => QueryStringQuery.IsConditionless((IQueryStringQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.QueryString = (IQueryStringQuery) this;

    internal static bool IsConditionless(IQueryStringQuery q) => q.Query.IsNullOrEmpty();
  }
}
