// Decompiled with JetBrains decompiler
// Type: Nest.MultiMatchQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class MultiMatchQueryDescriptor<T> : 
    QueryDescriptorBase<MultiMatchQueryDescriptor<T>, IMultiMatchQuery>,
    IMultiMatchQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => MultiMatchQuery.IsConditionless((IMultiMatchQuery) this);

    string IMultiMatchQuery.Analyzer { get; set; }

    bool? IMultiMatchQuery.AutoGenerateSynonymsPhraseQuery { get; set; }

    double? IMultiMatchQuery.CutoffFrequency { get; set; }

    Nest.Fields IMultiMatchQuery.Fields { get; set; }

    Nest.Fuzziness IMultiMatchQuery.Fuzziness { get; set; }

    MultiTermQueryRewrite IMultiMatchQuery.FuzzyRewrite { get; set; }

    bool? IMultiMatchQuery.FuzzyTranspositions { get; set; }

    bool? IMultiMatchQuery.Lenient { get; set; }

    int? IMultiMatchQuery.MaxExpansions { get; set; }

    Nest.MinimumShouldMatch IMultiMatchQuery.MinimumShouldMatch { get; set; }

    Nest.Operator? IMultiMatchQuery.Operator { get; set; }

    int? IMultiMatchQuery.PrefixLength { get; set; }

    string IMultiMatchQuery.Query { get; set; }

    int? IMultiMatchQuery.Slop { get; set; }

    double? IMultiMatchQuery.TieBreaker { get; set; }

    TextQueryType? IMultiMatchQuery.Type { get; set; }

    bool? IMultiMatchQuery.UseDisMax { get; set; }

    Nest.ZeroTermsQuery? IMultiMatchQuery.ZeroTermsQuery { get; set; }

    public MultiMatchQueryDescriptor<T> Fields(Func<FieldsDescriptor<T>, IPromise<Nest.Fields>> fields) => this.Assign<Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>(fields, (Action<IMultiMatchQuery, Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Nest.Fields) null));

    public MultiMatchQueryDescriptor<T> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<IMultiMatchQuery, Nest.Fields>) ((a, v) => a.Fields = v));

    public MultiMatchQueryDescriptor<T> Query(string query) => this.Assign<string>(query, (Action<IMultiMatchQuery, string>) ((a, v) => a.Query = v));

    public MultiMatchQueryDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IMultiMatchQuery, string>) ((a, v) => a.Analyzer = v));

    public MultiMatchQueryDescriptor<T> Fuzziness(Nest.Fuzziness fuzziness) => this.Assign<Nest.Fuzziness>(fuzziness, (Action<IMultiMatchQuery, Nest.Fuzziness>) ((a, v) => a.Fuzziness = v));

    public MultiMatchQueryDescriptor<T> CutoffFrequency(double? cutoffFrequency) => this.Assign<double?>(cutoffFrequency, (Action<IMultiMatchQuery, double?>) ((a, v) => a.CutoffFrequency = v));

    public MultiMatchQueryDescriptor<T> MinimumShouldMatch(Nest.MinimumShouldMatch minimumShouldMatch) => this.Assign<Nest.MinimumShouldMatch>(minimumShouldMatch, (Action<IMultiMatchQuery, Nest.MinimumShouldMatch>) ((a, v) => a.MinimumShouldMatch = v));

    public MultiMatchQueryDescriptor<T> FuzzyRewrite(MultiTermQueryRewrite rewrite) => this.Assign<MultiTermQueryRewrite>(rewrite, (Action<IMultiMatchQuery, MultiTermQueryRewrite>) ((a, v) => a.FuzzyRewrite = v));

    public MultiMatchQueryDescriptor<T> FuzzyTranspositions(bool? fuzzyTranpositions = true) => this.Assign<bool?>(fuzzyTranpositions, (Action<IMultiMatchQuery, bool?>) ((a, v) => a.FuzzyTranspositions = v));

    public MultiMatchQueryDescriptor<T> Lenient(bool? lenient = true) => this.Assign<bool?>(lenient, (Action<IMultiMatchQuery, bool?>) ((a, v) => a.Lenient = v));

    public MultiMatchQueryDescriptor<T> PrefixLength(int? prefixLength) => this.Assign<int?>(prefixLength, (Action<IMultiMatchQuery, int?>) ((a, v) => a.PrefixLength = v));

    public MultiMatchQueryDescriptor<T> MaxExpansions(int? maxExpansions) => this.Assign<int?>(maxExpansions, (Action<IMultiMatchQuery, int?>) ((a, v) => a.MaxExpansions = v));

    public MultiMatchQueryDescriptor<T> Slop(int? slop) => this.Assign<int?>(slop, (Action<IMultiMatchQuery, int?>) ((a, v) => a.Slop = v));

    public MultiMatchQueryDescriptor<T> Operator(Nest.Operator? op) => this.Assign<Nest.Operator?>(op, (Action<IMultiMatchQuery, Nest.Operator?>) ((a, v) => a.Operator = v));

    public MultiMatchQueryDescriptor<T> TieBreaker(double? tieBreaker) => this.Assign<double?>(tieBreaker, (Action<IMultiMatchQuery, double?>) ((a, v) => a.TieBreaker = v));

    public MultiMatchQueryDescriptor<T> Type(TextQueryType? type) => this.Assign<TextQueryType?>(type, (Action<IMultiMatchQuery, TextQueryType?>) ((a, v) => a.Type = v));

    public MultiMatchQueryDescriptor<T> UseDisMax(bool? useDisMax = true) => this.Assign<bool?>(useDisMax, (Action<IMultiMatchQuery, bool?>) ((a, v) => a.UseDisMax = v));

    public MultiMatchQueryDescriptor<T> ZeroTermsQuery(Nest.ZeroTermsQuery? zeroTermsQuery) => this.Assign<Nest.ZeroTermsQuery?>(zeroTermsQuery, (Action<IMultiMatchQuery, Nest.ZeroTermsQuery?>) ((a, v) => a.ZeroTermsQuery = v));

    public MultiMatchQueryDescriptor<T> AutoGenerateSynonymsPhraseQuery(
      bool? autoGenerateSynonymsPhraseQuery = true)
    {
      return this.Assign<bool?>(autoGenerateSynonymsPhraseQuery, (Action<IMultiMatchQuery, bool?>) ((a, v) => a.AutoGenerateSynonymsPhraseQuery = v));
    }
  }
}
