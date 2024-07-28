// Decompiled with JetBrains decompiler
// Type: Nest.MultiMatchQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class MultiMatchQuery : QueryBase, IMultiMatchQuery, IQuery
  {
    public string Analyzer { get; set; }

    public bool? AutoGenerateSynonymsPhraseQuery { get; set; }

    public double? CutoffFrequency { get; set; }

    public Fields Fields { get; set; }

    public Fuzziness Fuzziness { get; set; }

    public MultiTermQueryRewrite FuzzyRewrite { get; set; }

    public bool? FuzzyTranspositions { get; set; }

    public bool? Lenient { get; set; }

    public int? MaxExpansions { get; set; }

    public MinimumShouldMatch MinimumShouldMatch { get; set; }

    public Nest.Operator? Operator { get; set; }

    public int? PrefixLength { get; set; }

    public string Query { get; set; }

    public int? Slop { get; set; }

    public double? TieBreaker { get; set; }

    public TextQueryType? Type { get; set; }

    public bool? UseDisMax { get; set; }

    public Nest.ZeroTermsQuery? ZeroTermsQuery { get; set; }

    protected override bool Conditionless => MultiMatchQuery.IsConditionless((IMultiMatchQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.MultiMatch = (IMultiMatchQuery) this;

    internal static bool IsConditionless(IMultiMatchQuery q) => q.Query.IsNullOrEmpty();
  }
}
