// Decompiled with JetBrains decompiler
// Type: Nest.MatchQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MatchQuery : FieldNameQueryBase, IMatchQuery, IFieldNameQuery, IQuery
  {
    public string Analyzer { get; set; }

    public bool? AutoGenerateSynonymsPhraseQuery { get; set; }

    [Obsolete("Deprecated in 7.3.0. This option can be omitted since MatchQuery can skip blocks of documents efficiently if the total number of hits is not tracked.")]
    public double? CutoffFrequency { get; set; }

    public IFuzziness Fuzziness { get; set; }

    public MultiTermQueryRewrite FuzzyRewrite { get; set; }

    public bool? FuzzyTranspositions { get; set; }

    public bool? Lenient { get; set; }

    public int? MaxExpansions { get; set; }

    public MinimumShouldMatch MinimumShouldMatch { get; set; }

    public Nest.Operator? Operator { get; set; }

    public int? PrefixLength { get; set; }

    public string Query { get; set; }

    public Nest.ZeroTermsQuery? ZeroTermsQuery { get; set; }

    protected override bool Conditionless => MatchQuery.IsConditionless((IMatchQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Match = (IMatchQuery) this;

    internal static bool IsConditionless(IMatchQuery q) => q.Field.IsConditionless() || q.Query.IsNullOrEmpty();
  }
}
