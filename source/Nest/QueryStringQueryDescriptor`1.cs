// Decompiled with JetBrains decompiler
// Type: Nest.QueryStringQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class QueryStringQueryDescriptor<T> : 
    QueryDescriptorBase<QueryStringQueryDescriptor<T>, IQueryStringQuery>,
    IQueryStringQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => QueryStringQuery.IsConditionless((IQueryStringQuery) this);

    bool? IQueryStringQuery.AllowLeadingWildcard { get; set; }

    string IQueryStringQuery.Analyzer { get; set; }

    bool? IQueryStringQuery.AnalyzeWildcard { get; set; }

    bool? IQueryStringQuery.AutoGenerateSynonymsPhraseQuery { get; set; }

    Field IQueryStringQuery.DefaultField { get; set; }

    Operator? IQueryStringQuery.DefaultOperator { get; set; }

    bool? IQueryStringQuery.EnablePositionIncrements { get; set; }

    bool? IQueryStringQuery.Escape { get; set; }

    Nest.Fields IQueryStringQuery.Fields { get; set; }

    Nest.Fuzziness IQueryStringQuery.Fuzziness { get; set; }

    int? IQueryStringQuery.FuzzyMaxExpansions { get; set; }

    int? IQueryStringQuery.FuzzyPrefixLength { get; set; }

    MultiTermQueryRewrite IQueryStringQuery.FuzzyRewrite { get; set; }

    bool? IQueryStringQuery.FuzzyTranspositions { get; set; }

    bool? IQueryStringQuery.Lenient { get; set; }

    int? IQueryStringQuery.MaximumDeterminizedStates { get; set; }

    Nest.MinimumShouldMatch IQueryStringQuery.MinimumShouldMatch { get; set; }

    double? IQueryStringQuery.PhraseSlop { get; set; }

    string IQueryStringQuery.Query { get; set; }

    string IQueryStringQuery.QuoteAnalyzer { get; set; }

    string IQueryStringQuery.QuoteFieldSuffix { get; set; }

    MultiTermQueryRewrite IQueryStringQuery.Rewrite { get; set; }

    double? IQueryStringQuery.TieBreaker { get; set; }

    string IQueryStringQuery.TimeZone { get; set; }

    TextQueryType? IQueryStringQuery.Type { get; set; }

    public QueryStringQueryDescriptor<T> DefaultField(Field field) => this.Assign<Field>(field, (Action<IQueryStringQuery, Field>) ((a, v) => a.DefaultField = v));

    public QueryStringQueryDescriptor<T> DefaultField<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IQueryStringQuery, Expression<Func<T, TValue>>>) ((a, v) => a.DefaultField = (Field) (Expression) v));

    public QueryStringQueryDescriptor<T> Fields(Func<FieldsDescriptor<T>, IPromise<Nest.Fields>> fields) => this.Assign<Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>(fields, (Action<IQueryStringQuery, Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Nest.Fields) null));

    public QueryStringQueryDescriptor<T> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<IQueryStringQuery, Nest.Fields>) ((a, v) => a.Fields = v));

    public QueryStringQueryDescriptor<T> Type(TextQueryType? type) => this.Assign<TextQueryType?>(type, (Action<IQueryStringQuery, TextQueryType?>) ((a, v) => a.Type = v));

    public QueryStringQueryDescriptor<T> Query(string query) => this.Assign<string>(query, (Action<IQueryStringQuery, string>) ((a, v) => a.Query = v));

    public QueryStringQueryDescriptor<T> DefaultOperator(Operator? op) => this.Assign<Operator?>(op, (Action<IQueryStringQuery, Operator?>) ((a, v) => a.DefaultOperator = v));

    public QueryStringQueryDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IQueryStringQuery, string>) ((a, v) => a.Analyzer = v));

    public QueryStringQueryDescriptor<T> QuoteAnalyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IQueryStringQuery, string>) ((a, v) => a.QuoteAnalyzer = v));

    public QueryStringQueryDescriptor<T> AllowLeadingWildcard(bool? allowLeadingWildcard = true) => this.Assign<bool?>(allowLeadingWildcard, (Action<IQueryStringQuery, bool?>) ((a, v) => a.AllowLeadingWildcard = v));

    public QueryStringQueryDescriptor<T> Fuzziness(Nest.Fuzziness fuzziness) => this.Assign<Nest.Fuzziness>(fuzziness, (Action<IQueryStringQuery, Nest.Fuzziness>) ((a, v) => a.Fuzziness = v));

    public QueryStringQueryDescriptor<T> FuzzyPrefixLength(int? fuzzyPrefixLength) => this.Assign<int?>(fuzzyPrefixLength, (Action<IQueryStringQuery, int?>) ((a, v) => a.FuzzyPrefixLength = v));

    public QueryStringQueryDescriptor<T> FuzzyMaxExpansions(int? fuzzyMaxExpansions) => this.Assign<int?>(fuzzyMaxExpansions, (Action<IQueryStringQuery, int?>) ((a, v) => a.FuzzyMaxExpansions = v));

    public QueryStringQueryDescriptor<T> FuzzyTranspositions(bool? fuzzyTranspositions = true) => this.Assign<bool?>(fuzzyTranspositions, (Action<IQueryStringQuery, bool?>) ((a, v) => a.FuzzyTranspositions = v));

    public QueryStringQueryDescriptor<T> PhraseSlop(double? phraseSlop) => this.Assign<double?>(phraseSlop, (Action<IQueryStringQuery, double?>) ((a, v) => a.PhraseSlop = v));

    public QueryStringQueryDescriptor<T> MinimumShouldMatch(Nest.MinimumShouldMatch minimumShouldMatch) => this.Assign<Nest.MinimumShouldMatch>(minimumShouldMatch, (Action<IQueryStringQuery, Nest.MinimumShouldMatch>) ((a, v) => a.MinimumShouldMatch = v));

    public QueryStringQueryDescriptor<T> Lenient(bool? lenient = true) => this.Assign<bool?>(lenient, (Action<IQueryStringQuery, bool?>) ((a, v) => a.Lenient = v));

    public QueryStringQueryDescriptor<T> AnalyzeWildcard(bool? analyzeWildcard = true) => this.Assign<bool?>(analyzeWildcard, (Action<IQueryStringQuery, bool?>) ((a, v) => a.AnalyzeWildcard = v));

    public QueryStringQueryDescriptor<T> TieBreaker(double? tieBreaker) => this.Assign<double?>(tieBreaker, (Action<IQueryStringQuery, double?>) ((a, v) => a.TieBreaker = v));

    public QueryStringQueryDescriptor<T> MaximumDeterminizedStates(int? maxDeterminizedStates) => this.Assign<int?>(maxDeterminizedStates, (Action<IQueryStringQuery, int?>) ((a, v) => a.MaximumDeterminizedStates = v));

    public QueryStringQueryDescriptor<T> FuzzyRewrite(MultiTermQueryRewrite rewrite) => this.Assign<MultiTermQueryRewrite>(rewrite, (Action<IQueryStringQuery, MultiTermQueryRewrite>) ((a, v) => a.FuzzyRewrite = v));

    public QueryStringQueryDescriptor<T> Rewrite(MultiTermQueryRewrite rewrite) => this.Assign<MultiTermQueryRewrite>(rewrite, (Action<IQueryStringQuery, MultiTermQueryRewrite>) ((a, v) => a.Rewrite = v));

    public QueryStringQueryDescriptor<T> QuoteFieldSuffix(string quoteFieldSuffix) => this.Assign<string>(quoteFieldSuffix, (Action<IQueryStringQuery, string>) ((a, v) => a.QuoteFieldSuffix = v));

    public QueryStringQueryDescriptor<T> Escape(bool? escape = true) => this.Assign<bool?>(escape, (Action<IQueryStringQuery, bool?>) ((a, v) => a.Escape = v));

    public QueryStringQueryDescriptor<T> EnablePositionIncrements(bool? enablePositionIncrements = true) => this.Assign<bool?>(enablePositionIncrements, (Action<IQueryStringQuery, bool?>) ((a, v) => a.EnablePositionIncrements = v));

    public QueryStringQueryDescriptor<T> TimeZone(string timezone) => this.Assign<string>(timezone, (Action<IQueryStringQuery, string>) ((a, v) => a.TimeZone = v));

    public QueryStringQueryDescriptor<T> AutoGenerateSynonymsPhraseQuery(
      bool? autoGenerateSynonymsPhraseQuery = true)
    {
      return this.Assign<bool?>(autoGenerateSynonymsPhraseQuery, (Action<IQueryStringQuery, bool?>) ((a, v) => a.AutoGenerateSynonymsPhraseQuery = v));
    }
  }
}
