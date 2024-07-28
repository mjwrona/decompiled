// Decompiled with JetBrains decompiler
// Type: Nest.TextPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TextPropertyDescriptor<T> : 
    CorePropertyDescriptorBase<TextPropertyDescriptor<T>, ITextProperty, T>,
    ITextProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public TextPropertyDescriptor()
      : base(FieldType.Text)
    {
    }

    string ITextProperty.Analyzer { get; set; }

    double? ITextProperty.Boost { get; set; }

    bool? ITextProperty.EagerGlobalOrdinals { get; set; }

    bool? ITextProperty.Fielddata { get; set; }

    IFielddataFrequencyFilter ITextProperty.FielddataFrequencyFilter { get; set; }

    bool? ITextProperty.Index { get; set; }

    Nest.IndexOptions? ITextProperty.IndexOptions { get; set; }

    bool? ITextProperty.IndexPhrases { get; set; }

    ITextIndexPrefixes ITextProperty.IndexPrefixes { get; set; }

    bool? ITextProperty.Norms { get; set; }

    int? ITextProperty.PositionIncrementGap { get; set; }

    string ITextProperty.SearchAnalyzer { get; set; }

    string ITextProperty.SearchQuoteAnalyzer { get; set; }

    TermVectorOption? ITextProperty.TermVector { get; set; }

    public TextPropertyDescriptor<T> Boost(double? boost) => this.Assign<double?>(boost, (Action<ITextProperty, double?>) ((a, v) => a.Boost = v));

    public TextPropertyDescriptor<T> EagerGlobalOrdinals(bool? eagerGlobalOrdinals = true) => this.Assign<bool?>(eagerGlobalOrdinals, (Action<ITextProperty, bool?>) ((a, v) => a.EagerGlobalOrdinals = v));

    public TextPropertyDescriptor<T> Fielddata(bool? fielddata = true) => this.Assign<bool?>(fielddata, (Action<ITextProperty, bool?>) ((a, v) => a.Fielddata = v));

    public TextPropertyDescriptor<T> FielddataFrequencyFilter(
      Func<FielddataFrequencyFilterDescriptor, IFielddataFrequencyFilter> selector)
    {
      return this.Assign<Func<FielddataFrequencyFilterDescriptor, IFielddataFrequencyFilter>>(selector, (Action<ITextProperty, Func<FielddataFrequencyFilterDescriptor, IFielddataFrequencyFilter>>) ((a, v) => a.FielddataFrequencyFilter = v != null ? v(new FielddataFrequencyFilterDescriptor()) : (IFielddataFrequencyFilter) null));
    }

    public TextPropertyDescriptor<T> IndexPrefixes(
      Func<TextIndexPrefixesDescriptor, ITextIndexPrefixes> selector)
    {
      return this.Assign<Func<TextIndexPrefixesDescriptor, ITextIndexPrefixes>>(selector, (Action<ITextProperty, Func<TextIndexPrefixesDescriptor, ITextIndexPrefixes>>) ((a, v) => a.IndexPrefixes = v != null ? v(new TextIndexPrefixesDescriptor()) : (ITextIndexPrefixes) null));
    }

    public TextPropertyDescriptor<T> Index(bool? index = true) => this.Assign<bool?>(index, (Action<ITextProperty, bool?>) ((a, v) => a.Index = v));

    public TextPropertyDescriptor<T> IndexPhrases(bool? indexPhrases = true) => this.Assign<bool?>(indexPhrases, (Action<ITextProperty, bool?>) ((a, v) => a.IndexPhrases = v));

    public TextPropertyDescriptor<T> IndexOptions(Nest.IndexOptions? indexOptions) => this.Assign<Nest.IndexOptions?>(indexOptions, (Action<ITextProperty, Nest.IndexOptions?>) ((a, v) => a.IndexOptions = v));

    public TextPropertyDescriptor<T> Norms(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<ITextProperty, bool?>) ((a, v) => a.Norms = v));

    public TextPropertyDescriptor<T> PositionIncrementGap(int? positionIncrementGap) => this.Assign<int?>(positionIncrementGap, (Action<ITextProperty, int?>) ((a, v) => a.PositionIncrementGap = v));

    public TextPropertyDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<ITextProperty, string>) ((a, v) => a.Analyzer = v));

    public TextPropertyDescriptor<T> SearchAnalyzer(string searchAnalyzer) => this.Assign<string>(searchAnalyzer, (Action<ITextProperty, string>) ((a, v) => a.SearchAnalyzer = v));

    public TextPropertyDescriptor<T> SearchQuoteAnalyzer(string searchQuoteAnalyzer) => this.Assign<string>(searchQuoteAnalyzer, (Action<ITextProperty, string>) ((a, v) => a.SearchQuoteAnalyzer = v));

    public TextPropertyDescriptor<T> TermVector(TermVectorOption? termVector) => this.Assign<TermVectorOption?>(termVector, (Action<ITextProperty, TermVectorOption?>) ((a, v) => a.TermVector = v));
  }
}
