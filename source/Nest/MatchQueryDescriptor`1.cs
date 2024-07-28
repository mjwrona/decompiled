// Decompiled with JetBrains decompiler
// Type: Nest.MatchQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class MatchQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<MatchQueryDescriptor<T>, IMatchQuery, T>,
    IMatchQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => MatchQuery.IsConditionless((IMatchQuery) this);

    protected virtual string MatchQueryType => (string) null;

    string IMatchQuery.Analyzer { get; set; }

    bool? IMatchQuery.AutoGenerateSynonymsPhraseQuery { get; set; }

    [Obsolete("Deprecated in 7.3.0. This option can be omitted since MatchQuery can skip blocks of documents efficiently if the total number of hits is not tracked.")]
    double? IMatchQuery.CutoffFrequency { get; set; }

    IFuzziness IMatchQuery.Fuzziness { get; set; }

    MultiTermQueryRewrite IMatchQuery.FuzzyRewrite { get; set; }

    bool? IMatchQuery.FuzzyTranspositions { get; set; }

    bool? IMatchQuery.Lenient { get; set; }

    int? IMatchQuery.MaxExpansions { get; set; }

    Nest.MinimumShouldMatch IMatchQuery.MinimumShouldMatch { get; set; }

    Nest.Operator? IMatchQuery.Operator { get; set; }

    int? IMatchQuery.PrefixLength { get; set; }

    string IMatchQuery.Query { get; set; }

    Nest.ZeroTermsQuery? IMatchQuery.ZeroTermsQuery { get; set; }

    public MatchQueryDescriptor<T> Query(string query) => this.Assign<string>(query, (Action<IMatchQuery, string>) ((a, v) => a.Query = v));

    public MatchQueryDescriptor<T> Lenient(bool? lenient = true) => this.Assign<bool?>(lenient, (Action<IMatchQuery, bool?>) ((a, v) => a.Lenient = v));

    public MatchQueryDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IMatchQuery, string>) ((a, v) => a.Analyzer = v));

    public MatchQueryDescriptor<T> Fuzziness(Nest.Fuzziness fuzziness) => this.Assign<Nest.Fuzziness>(fuzziness, (Action<IMatchQuery, Nest.Fuzziness>) ((a, v) => a.Fuzziness = (IFuzziness) v));

    public MatchQueryDescriptor<T> FuzzyTranspositions(bool? fuzzyTranspositions = true) => this.Assign<bool?>(fuzzyTranspositions, (Action<IMatchQuery, bool?>) ((a, v) => a.FuzzyTranspositions = v));

    [Obsolete("Deprecated in 7.3.0. This option can be omitted since MatchQuery can skip blocks of documents efficiently if the total number of hits is not tracked.")]
    public MatchQueryDescriptor<T> CutoffFrequency(double? cutoffFrequency) => this.Assign<double?>(cutoffFrequency, (Action<IMatchQuery, double?>) ((a, v) => a.CutoffFrequency = v));

    public MatchQueryDescriptor<T> FuzzyRewrite(MultiTermQueryRewrite rewrite) => this.Assign<MultiTermQueryRewrite>(rewrite, (Action<IMatchQuery, MultiTermQueryRewrite>) ((a, v) => a.FuzzyRewrite = v));

    public MatchQueryDescriptor<T> MinimumShouldMatch(Nest.MinimumShouldMatch minimumShouldMatch) => this.Assign<Nest.MinimumShouldMatch>(minimumShouldMatch, (Action<IMatchQuery, Nest.MinimumShouldMatch>) ((a, v) => a.MinimumShouldMatch = v));

    public MatchQueryDescriptor<T> Operator(Nest.Operator? op) => this.Assign<Nest.Operator?>(op, (Action<IMatchQuery, Nest.Operator?>) ((a, v) => a.Operator = v));

    public MatchQueryDescriptor<T> ZeroTermsQuery(Nest.ZeroTermsQuery? zeroTermsQuery) => this.Assign<Nest.ZeroTermsQuery?>(zeroTermsQuery, (Action<IMatchQuery, Nest.ZeroTermsQuery?>) ((a, v) => a.ZeroTermsQuery = v));

    public MatchQueryDescriptor<T> PrefixLength(int? prefixLength) => this.Assign<int?>(prefixLength, (Action<IMatchQuery, int?>) ((a, v) => a.PrefixLength = v));

    public MatchQueryDescriptor<T> MaxExpansions(int? maxExpansions) => this.Assign<int?>(maxExpansions, (Action<IMatchQuery, int?>) ((a, v) => a.MaxExpansions = v));

    public MatchQueryDescriptor<T> AutoGenerateSynonymsPhraseQuery(
      bool? autoGenerateSynonymsPhraseQuery = true)
    {
      return this.Assign<bool?>(autoGenerateSynonymsPhraseQuery, (Action<IMatchQuery, bool?>) ((a, v) => a.AutoGenerateSynonymsPhraseQuery = v));
    }
  }
}
