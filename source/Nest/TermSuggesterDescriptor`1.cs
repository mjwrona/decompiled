// Decompiled with JetBrains decompiler
// Type: Nest.TermSuggesterDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TermSuggesterDescriptor<T> : 
    SuggestDescriptorBase<TermSuggesterDescriptor<T>, ITermSuggester, T>,
    ITermSuggester,
    ISuggester
    where T : class
  {
    bool? ITermSuggester.LowercaseTerms { get; set; }

    int? ITermSuggester.MaxEdits { get; set; }

    int? ITermSuggester.MaxInspections { get; set; }

    float? ITermSuggester.MaxTermFrequency { get; set; }

    float? ITermSuggester.MinDocFrequency { get; set; }

    int? ITermSuggester.MinWordLength { get; set; }

    int? ITermSuggester.PrefixLength { get; set; }

    int? ITermSuggester.ShardSize { get; set; }

    SuggestSort? ITermSuggester.Sort { get; set; }

    Nest.StringDistance? ITermSuggester.StringDistance { get; set; }

    Elasticsearch.Net.SuggestMode? ITermSuggester.SuggestMode { get; set; }

    string ITermSuggester.Text { get; set; }

    public TermSuggesterDescriptor<T> Text(string text) => this.Assign<string>(text, (Action<ITermSuggester, string>) ((a, v) => a.Text = v));

    public TermSuggesterDescriptor<T> ShardSize(int? shardSize) => this.Assign<int?>(shardSize, (Action<ITermSuggester, int?>) ((a, v) => a.ShardSize = v));

    public TermSuggesterDescriptor<T> SuggestMode(Elasticsearch.Net.SuggestMode? mode) => this.Assign<Elasticsearch.Net.SuggestMode?>(mode, (Action<ITermSuggester, Elasticsearch.Net.SuggestMode?>) ((a, v) => a.SuggestMode = v));

    public TermSuggesterDescriptor<T> MinWordLength(int? length) => this.Assign<int?>(length, (Action<ITermSuggester, int?>) ((a, v) => a.MinWordLength = v));

    public TermSuggesterDescriptor<T> PrefixLength(int? length) => this.Assign<int?>(length, (Action<ITermSuggester, int?>) ((a, v) => a.PrefixLength = v));

    public TermSuggesterDescriptor<T> MaxEdits(int? maxEdits) => this.Assign<int?>(maxEdits, (Action<ITermSuggester, int?>) ((a, v) => a.MaxEdits = v));

    public TermSuggesterDescriptor<T> MaxInspections(int? maxInspections) => this.Assign<int?>(maxInspections, (Action<ITermSuggester, int?>) ((a, v) => a.MaxInspections = v));

    public TermSuggesterDescriptor<T> MinDocFrequency(float? frequency) => this.Assign<float?>(frequency, (Action<ITermSuggester, float?>) ((a, v) => a.MinDocFrequency = v));

    public TermSuggesterDescriptor<T> MaxTermFrequency(float? frequency) => this.Assign<float?>(frequency, (Action<ITermSuggester, float?>) ((a, v) => a.MaxTermFrequency = v));

    public TermSuggesterDescriptor<T> Sort(SuggestSort? sort) => this.Assign<SuggestSort?>(sort, (Action<ITermSuggester, SuggestSort?>) ((a, v) => a.Sort = v));

    public TermSuggesterDescriptor<T> LowercaseTerms(bool? lowercaseTerms = true) => this.Assign<bool?>(lowercaseTerms, (Action<ITermSuggester, bool?>) ((a, v) => a.LowercaseTerms = v));

    public TermSuggesterDescriptor<T> StringDistance(Nest.StringDistance? distance) => this.Assign<Nest.StringDistance?>(distance, (Action<ITermSuggester, Nest.StringDistance?>) ((a, v) => a.StringDistance = v));
  }
}
