// Decompiled with JetBrains decompiler
// Type: Nest.SearchAsYouTypePropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class SearchAsYouTypePropertyDescriptor<T> : 
    CorePropertyDescriptorBase<SearchAsYouTypePropertyDescriptor<T>, ISearchAsYouTypeProperty, T>,
    ISearchAsYouTypeProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public SearchAsYouTypePropertyDescriptor()
      : base(FieldType.SearchAsYouType)
    {
    }

    string ISearchAsYouTypeProperty.Analyzer { get; set; }

    bool? ISearchAsYouTypeProperty.Index { get; set; }

    Nest.IndexOptions? ISearchAsYouTypeProperty.IndexOptions { get; set; }

    int? ISearchAsYouTypeProperty.MaxShingleSize { get; set; }

    bool? ISearchAsYouTypeProperty.Norms { get; set; }

    string ISearchAsYouTypeProperty.SearchAnalyzer { get; set; }

    string ISearchAsYouTypeProperty.SearchQuoteAnalyzer { get; set; }

    TermVectorOption? ISearchAsYouTypeProperty.TermVector { get; set; }

    public SearchAsYouTypePropertyDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<ISearchAsYouTypeProperty, string>) ((a, v) => a.Analyzer = v));

    public SearchAsYouTypePropertyDescriptor<T> Index(bool? index = true) => this.Assign<bool?>(index, (Action<ISearchAsYouTypeProperty, bool?>) ((a, v) => a.Index = v));

    public SearchAsYouTypePropertyDescriptor<T> IndexOptions(Nest.IndexOptions? indexOptions) => this.Assign<Nest.IndexOptions?>(indexOptions, (Action<ISearchAsYouTypeProperty, Nest.IndexOptions?>) ((a, v) => a.IndexOptions = v));

    public SearchAsYouTypePropertyDescriptor<T> MaxShingleSize(int? maxShingleSize) => this.Assign<int?>(maxShingleSize, (Action<ISearchAsYouTypeProperty, int?>) ((a, v) => a.MaxShingleSize = v));

    public SearchAsYouTypePropertyDescriptor<T> Norms(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<ISearchAsYouTypeProperty, bool?>) ((a, v) => a.Norms = v));

    public SearchAsYouTypePropertyDescriptor<T> SearchAnalyzer(string searchAnalyzer) => this.Assign<string>(searchAnalyzer, (Action<ISearchAsYouTypeProperty, string>) ((a, v) => a.SearchAnalyzer = v));

    public SearchAsYouTypePropertyDescriptor<T> SearchQuoteAnalyzer(string searchQuoteAnalyzer) => this.Assign<string>(searchQuoteAnalyzer, (Action<ISearchAsYouTypeProperty, string>) ((a, v) => a.SearchQuoteAnalyzer = v));

    public SearchAsYouTypePropertyDescriptor<T> TermVector(TermVectorOption? termVector) => this.Assign<TermVectorOption?>(termVector, (Action<ISearchAsYouTypeProperty, TermVectorOption?>) ((a, v) => a.TermVector = v));
  }
}
