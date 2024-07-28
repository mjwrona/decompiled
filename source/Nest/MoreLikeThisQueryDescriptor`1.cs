// Decompiled with JetBrains decompiler
// Type: Nest.MoreLikeThisQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class MoreLikeThisQueryDescriptor<T> : 
    QueryDescriptorBase<MoreLikeThisQueryDescriptor<T>, IMoreLikeThisQuery>,
    IMoreLikeThisQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => MoreLikeThisQuery.IsConditionless((IMoreLikeThisQuery) this);

    string IMoreLikeThisQuery.Analyzer { get; set; }

    double? IMoreLikeThisQuery.BoostTerms { get; set; }

    Nest.Fields IMoreLikeThisQuery.Fields { get; set; }

    bool? IMoreLikeThisQuery.Include { get; set; }

    IEnumerable<Nest.Like> IMoreLikeThisQuery.Like { get; set; }

    int? IMoreLikeThisQuery.MaxDocumentFrequency { get; set; }

    int? IMoreLikeThisQuery.MaxQueryTerms { get; set; }

    int? IMoreLikeThisQuery.MaxWordLength { get; set; }

    int? IMoreLikeThisQuery.MinDocumentFrequency { get; set; }

    Nest.MinimumShouldMatch IMoreLikeThisQuery.MinimumShouldMatch { get; set; }

    int? IMoreLikeThisQuery.MinTermFrequency { get; set; }

    int? IMoreLikeThisQuery.MinWordLength { get; set; }

    IPerFieldAnalyzer IMoreLikeThisQuery.PerFieldAnalyzer { get; set; }

    Nest.Routing IMoreLikeThisQuery.Routing { get; set; }

    Nest.StopWords IMoreLikeThisQuery.StopWords { get; set; }

    IEnumerable<Nest.Like> IMoreLikeThisQuery.Unlike { get; set; }

    long? IMoreLikeThisQuery.Version { get; set; }

    Elasticsearch.Net.VersionType? IMoreLikeThisQuery.VersionType { get; set; }

    public MoreLikeThisQueryDescriptor<T> Fields(Func<FieldsDescriptor<T>, IPromise<Nest.Fields>> fields) => this.Assign<Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>(fields, (Action<IMoreLikeThisQuery, Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Nest.Fields) null));

    public MoreLikeThisQueryDescriptor<T> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<IMoreLikeThisQuery, Nest.Fields>) ((a, v) => a.Fields = v));

    public MoreLikeThisQueryDescriptor<T> StopWords(IEnumerable<string> stopWords) => this.Assign<List<string>>(stopWords.ToListOrNullIfEmpty<string>(), (Action<IMoreLikeThisQuery, List<string>>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public MoreLikeThisQueryDescriptor<T> StopWords(params string[] stopWords) => this.Assign<string[]>(stopWords, (Action<IMoreLikeThisQuery, string[]>) ((a, v) => a.StopWords = (Nest.StopWords) v));

    public MoreLikeThisQueryDescriptor<T> StopWords(Nest.StopWords stopWords) => this.Assign<Nest.StopWords>(stopWords, (Action<IMoreLikeThisQuery, Nest.StopWords>) ((a, v) => a.StopWords = v));

    public MoreLikeThisQueryDescriptor<T> MaxQueryTerms(int? maxQueryTerms) => this.Assign<int?>(maxQueryTerms, (Action<IMoreLikeThisQuery, int?>) ((a, v) => a.MaxQueryTerms = v));

    public MoreLikeThisQueryDescriptor<T> MinTermFrequency(int? minTermFrequency) => this.Assign<int?>(minTermFrequency, (Action<IMoreLikeThisQuery, int?>) ((a, v) => a.MinTermFrequency = v));

    public MoreLikeThisQueryDescriptor<T> MinDocumentFrequency(int? minDocumentFrequency) => this.Assign<int?>(minDocumentFrequency, (Action<IMoreLikeThisQuery, int?>) ((a, v) => a.MinDocumentFrequency = v));

    public MoreLikeThisQueryDescriptor<T> MaxDocumentFrequency(int? maxDocumentFrequency) => this.Assign<int?>(maxDocumentFrequency, (Action<IMoreLikeThisQuery, int?>) ((a, v) => a.MaxDocumentFrequency = v));

    public MoreLikeThisQueryDescriptor<T> MinWordLength(int? minWordLength) => this.Assign<int?>(minWordLength, (Action<IMoreLikeThisQuery, int?>) ((a, v) => a.MinWordLength = v));

    public MoreLikeThisQueryDescriptor<T> MaxWordLength(int? maxWordLength) => this.Assign<int?>(maxWordLength, (Action<IMoreLikeThisQuery, int?>) ((a, v) => a.MaxWordLength = v));

    public MoreLikeThisQueryDescriptor<T> BoostTerms(double? boostTerms) => this.Assign<double?>(boostTerms, (Action<IMoreLikeThisQuery, double?>) ((a, v) => a.BoostTerms = v));

    public MoreLikeThisQueryDescriptor<T> MinimumShouldMatch(Nest.MinimumShouldMatch minMatch) => this.Assign<Nest.MinimumShouldMatch>(minMatch, (Action<IMoreLikeThisQuery, Nest.MinimumShouldMatch>) ((a, v) => a.MinimumShouldMatch = v));

    public MoreLikeThisQueryDescriptor<T> Include(bool? include = true) => this.Assign<bool?>(include, (Action<IMoreLikeThisQuery, bool?>) ((a, v) => a.Include = v));

    public MoreLikeThisQueryDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IMoreLikeThisQuery, string>) ((a, v) => a.Analyzer = v));

    public MoreLikeThisQueryDescriptor<T> Like(
      Func<LikeDescriptor<T>, IPromise<List<Nest.Like>>> selector)
    {
      return this.Assign<Func<LikeDescriptor<T>, IPromise<List<Nest.Like>>>>(selector, (Action<IMoreLikeThisQuery, Func<LikeDescriptor<T>, IPromise<List<Nest.Like>>>>) ((a, v) => a.Like = v != null ? (IEnumerable<Nest.Like>) v(new LikeDescriptor<T>())?.Value : (IEnumerable<Nest.Like>) null));
    }

    public MoreLikeThisQueryDescriptor<T> Unlike(
      Func<LikeDescriptor<T>, IPromise<List<Nest.Like>>> selector)
    {
      return this.Assign<Func<LikeDescriptor<T>, IPromise<List<Nest.Like>>>>(selector, (Action<IMoreLikeThisQuery, Func<LikeDescriptor<T>, IPromise<List<Nest.Like>>>>) ((a, v) => a.Unlike = v != null ? (IEnumerable<Nest.Like>) v(new LikeDescriptor<T>())?.Value : (IEnumerable<Nest.Like>) null));
    }

    public MoreLikeThisQueryDescriptor<T> PerFieldAnalyzer(
      Func<PerFieldAnalyzerDescriptor<T>, IPromise<IPerFieldAnalyzer>> analyzerSelector)
    {
      return this.Assign<Func<PerFieldAnalyzerDescriptor<T>, IPromise<IPerFieldAnalyzer>>>(analyzerSelector, (Action<IMoreLikeThisQuery, Func<PerFieldAnalyzerDescriptor<T>, IPromise<IPerFieldAnalyzer>>>) ((a, v) => a.PerFieldAnalyzer = v != null ? v(new PerFieldAnalyzerDescriptor<T>())?.Value : (IPerFieldAnalyzer) null));
    }

    public MoreLikeThisQueryDescriptor<T> Version(long? version) => this.Assign<long?>(version, (Action<IMoreLikeThisQuery, long?>) ((a, v) => a.Version = v));

    public MoreLikeThisQueryDescriptor<T> VersionType(Elasticsearch.Net.VersionType? versionType) => this.Assign<Elasticsearch.Net.VersionType?>(versionType, (Action<IMoreLikeThisQuery, Elasticsearch.Net.VersionType?>) ((a, v) => a.VersionType = v));

    public MoreLikeThisQueryDescriptor<T> Routing(Nest.Routing routing) => this.Assign<Nest.Routing>(routing, (Action<IMoreLikeThisQuery, Nest.Routing>) ((a, v) => a.Routing = v));
  }
}
