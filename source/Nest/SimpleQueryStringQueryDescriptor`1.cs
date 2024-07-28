// Decompiled with JetBrains decompiler
// Type: Nest.SimpleQueryStringQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SimpleQueryStringQueryDescriptor<T> : 
    QueryDescriptorBase<SimpleQueryStringQueryDescriptor<T>, ISimpleQueryStringQuery>,
    ISimpleQueryStringQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => SimpleQueryStringQuery.IsConditionless((ISimpleQueryStringQuery) this);

    string ISimpleQueryStringQuery.Analyzer { get; set; }

    bool? ISimpleQueryStringQuery.AnalyzeWildcard { get; set; }

    bool? ISimpleQueryStringQuery.AutoGenerateSynonymsPhraseQuery { get; set; }

    Operator? ISimpleQueryStringQuery.DefaultOperator { get; set; }

    Nest.Fields ISimpleQueryStringQuery.Fields { get; set; }

    SimpleQueryStringFlags? ISimpleQueryStringQuery.Flags { get; set; }

    int? ISimpleQueryStringQuery.FuzzyMaxExpansions { get; set; }

    int? ISimpleQueryStringQuery.FuzzyPrefixLength { get; set; }

    bool? ISimpleQueryStringQuery.FuzzyTranspositions { get; set; }

    bool? ISimpleQueryStringQuery.Lenient { get; set; }

    Nest.MinimumShouldMatch ISimpleQueryStringQuery.MinimumShouldMatch { get; set; }

    string ISimpleQueryStringQuery.Query { get; set; }

    string ISimpleQueryStringQuery.QuoteFieldSuffix { get; set; }

    public SimpleQueryStringQueryDescriptor<T> Fields(
      Func<FieldsDescriptor<T>, IPromise<Nest.Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>(fields, (Action<ISimpleQueryStringQuery, Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Nest.Fields) null));
    }

    public SimpleQueryStringQueryDescriptor<T> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<ISimpleQueryStringQuery, Nest.Fields>) ((a, v) => a.Fields = v));

    public SimpleQueryStringQueryDescriptor<T> Query(string query) => this.Assign<string>(query, (Action<ISimpleQueryStringQuery, string>) ((a, v) => a.Query = v));

    public SimpleQueryStringQueryDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<ISimpleQueryStringQuery, string>) ((a, v) => a.Analyzer = v));

    public SimpleQueryStringQueryDescriptor<T> DefaultOperator(Operator? op) => this.Assign<Operator?>(op, (Action<ISimpleQueryStringQuery, Operator?>) ((a, v) => a.DefaultOperator = v));

    public SimpleQueryStringQueryDescriptor<T> Flags(SimpleQueryStringFlags? flags) => this.Assign<SimpleQueryStringFlags?>(flags, (Action<ISimpleQueryStringQuery, SimpleQueryStringFlags?>) ((a, v) => a.Flags = v));

    public SimpleQueryStringQueryDescriptor<T> AnalyzeWildcard(bool? analyzeWildcard = true) => this.Assign<bool?>(analyzeWildcard, (Action<ISimpleQueryStringQuery, bool?>) ((a, v) => a.AnalyzeWildcard = v));

    public SimpleQueryStringQueryDescriptor<T> Lenient(bool? lenient = true) => this.Assign<bool?>(lenient, (Action<ISimpleQueryStringQuery, bool?>) ((a, v) => a.Lenient = v));

    public SimpleQueryStringQueryDescriptor<T> MinimumShouldMatch(
      Nest.MinimumShouldMatch minimumShouldMatch)
    {
      return this.Assign<Nest.MinimumShouldMatch>(minimumShouldMatch, (Action<ISimpleQueryStringQuery, Nest.MinimumShouldMatch>) ((a, v) => a.MinimumShouldMatch = v));
    }

    public SimpleQueryStringQueryDescriptor<T> QuoteFieldSuffix(string quoteFieldSuffix) => this.Assign<string>(quoteFieldSuffix, (Action<ISimpleQueryStringQuery, string>) ((a, v) => a.QuoteFieldSuffix = v));

    public SimpleQueryStringQueryDescriptor<T> FuzzyPrefixLength(int? fuzzyPrefixLength) => this.Assign<int?>(fuzzyPrefixLength, (Action<ISimpleQueryStringQuery, int?>) ((a, v) => a.FuzzyPrefixLength = v));

    public SimpleQueryStringQueryDescriptor<T> FuzzyMaxExpansions(int? fuzzyMaxExpansions) => this.Assign<int?>(fuzzyMaxExpansions, (Action<ISimpleQueryStringQuery, int?>) ((a, v) => a.FuzzyMaxExpansions = v));

    public SimpleQueryStringQueryDescriptor<T> FuzzyTranspositions(bool? fuzzyTranspositions = true) => this.Assign<bool?>(fuzzyTranspositions, (Action<ISimpleQueryStringQuery, bool?>) ((a, v) => a.FuzzyTranspositions = v));

    public SimpleQueryStringQueryDescriptor<T> AutoGenerateSynonymsPhraseQuery(
      bool? autoGenerateSynonymsPhraseQuery = true)
    {
      return this.Assign<bool?>(autoGenerateSynonymsPhraseQuery, (Action<ISimpleQueryStringQuery, bool?>) ((a, v) => a.AutoGenerateSynonymsPhraseQuery = v));
    }
  }
}
