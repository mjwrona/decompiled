// Decompiled with JetBrains decompiler
// Type: Nest.MatchBoolPrefixQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MatchBoolPrefixQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<MatchBoolPrefixQueryDescriptor<T>, IMatchBoolPrefixQuery, T>,
    IMatchBoolPrefixQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => MatchBoolPrefixQuery.IsConditionless((IMatchBoolPrefixQuery) this);

    string IMatchBoolPrefixQuery.Analyzer { get; set; }

    IFuzziness IMatchBoolPrefixQuery.Fuzziness { get; set; }

    MultiTermQueryRewrite IMatchBoolPrefixQuery.FuzzyRewrite { get; set; }

    bool? IMatchBoolPrefixQuery.FuzzyTranspositions { get; set; }

    int? IMatchBoolPrefixQuery.MaxExpansions { get; set; }

    Nest.MinimumShouldMatch IMatchBoolPrefixQuery.MinimumShouldMatch { get; set; }

    Nest.Operator? IMatchBoolPrefixQuery.Operator { get; set; }

    int? IMatchBoolPrefixQuery.PrefixLength { get; set; }

    string IMatchBoolPrefixQuery.Query { get; set; }

    public MatchBoolPrefixQueryDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IMatchBoolPrefixQuery, string>) ((a, v) => a.Analyzer = v));

    public MatchBoolPrefixQueryDescriptor<T> Fuzziness(Nest.Fuzziness fuzziness) => this.Assign<Nest.Fuzziness>(fuzziness, (Action<IMatchBoolPrefixQuery, Nest.Fuzziness>) ((a, v) => a.Fuzziness = (IFuzziness) v));

    public MatchBoolPrefixQueryDescriptor<T> FuzzyTranspositions(bool? fuzzyTranspositions = true) => this.Assign<bool?>(fuzzyTranspositions, (Action<IMatchBoolPrefixQuery, bool?>) ((a, v) => a.FuzzyTranspositions = v));

    public MatchBoolPrefixQueryDescriptor<T> FuzzyRewrite(MultiTermQueryRewrite rewrite) => this.Assign<MultiTermQueryRewrite>(rewrite, (Action<IMatchBoolPrefixQuery, MultiTermQueryRewrite>) ((a, v) => a.FuzzyRewrite = v));

    public MatchBoolPrefixQueryDescriptor<T> MaxExpansions(int? maxExpansions) => this.Assign<int?>(maxExpansions, (Action<IMatchBoolPrefixQuery, int?>) ((a, v) => a.MaxExpansions = v));

    public MatchBoolPrefixQueryDescriptor<T> MinimumShouldMatch(
      Nest.MinimumShouldMatch minimumShouldMatch)
    {
      return this.Assign<Nest.MinimumShouldMatch>(minimumShouldMatch, (Action<IMatchBoolPrefixQuery, Nest.MinimumShouldMatch>) ((a, v) => a.MinimumShouldMatch = v));
    }

    public MatchBoolPrefixQueryDescriptor<T> Operator(Nest.Operator? op) => this.Assign<Nest.Operator?>(op, (Action<IMatchBoolPrefixQuery, Nest.Operator?>) ((a, v) => a.Operator = v));

    public MatchBoolPrefixQueryDescriptor<T> PrefixLength(int? prefixLength) => this.Assign<int?>(prefixLength, (Action<IMatchBoolPrefixQuery, int?>) ((a, v) => a.PrefixLength = v));

    public MatchBoolPrefixQueryDescriptor<T> Query(string query) => this.Assign<string>(query, (Action<IMatchBoolPrefixQuery, string>) ((a, v) => a.Query = v));
  }
}
