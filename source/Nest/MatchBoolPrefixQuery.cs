// Decompiled with JetBrains decompiler
// Type: Nest.MatchBoolPrefixQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class MatchBoolPrefixQuery : 
    FieldNameQueryBase,
    IMatchBoolPrefixQuery,
    IFieldNameQuery,
    IQuery
  {
    public string Analyzer { get; set; }

    public IFuzziness Fuzziness { get; set; }

    public MultiTermQueryRewrite FuzzyRewrite { get; set; }

    public bool? FuzzyTranspositions { get; set; }

    public int? MaxExpansions { get; set; }

    public MinimumShouldMatch MinimumShouldMatch { get; set; }

    public Nest.Operator? Operator { get; set; }

    public int? PrefixLength { get; set; }

    public string Query { get; set; }

    protected override bool Conditionless => MatchBoolPrefixQuery.IsConditionless((IMatchBoolPrefixQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.MatchBoolPrefix = (IMatchBoolPrefixQuery) this;

    internal static bool IsConditionless(IMatchBoolPrefixQuery q) => q.Field.IsConditionless() || q.Query.IsNullOrEmpty();
  }
}
