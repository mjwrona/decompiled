// Decompiled with JetBrains decompiler
// Type: Nest.SearchAsYouTypeAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class SearchAsYouTypeAttribute : 
    ElasticsearchCorePropertyAttributeBase,
    ISearchAsYouTypeProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public SearchAsYouTypeAttribute()
      : base(FieldType.SearchAsYouType)
    {
    }

    public string Analyzer { get; set; }

    public bool Index
    {
      get => this.Self.Index.GetValueOrDefault(true);
      set => this.Self.Index = new bool?(value);
    }

    public IndexOptions IndexOptions
    {
      get => this.Self.IndexOptions.GetValueOrDefault(IndexOptions.Positions);
      set => this.Self.IndexOptions = new IndexOptions?(value);
    }

    public int MaxShingleSize
    {
      get => this.Self.MaxShingleSize.GetValueOrDefault();
      set => this.Self.MaxShingleSize = new int?(value);
    }

    public bool Norms
    {
      get => this.Self.Norms.GetValueOrDefault(true);
      set => this.Self.Norms = new bool?(value);
    }

    public string SearchAnalyzer { get; set; }

    public string SearchQuoteAnalyzer { get; set; }

    public TermVectorOption TermVector
    {
      get => this.Self.TermVector.GetValueOrDefault(TermVectorOption.No);
      set => this.Self.TermVector = new TermVectorOption?(value);
    }

    bool? ISearchAsYouTypeProperty.Index { get; set; }

    IndexOptions? ISearchAsYouTypeProperty.IndexOptions { get; set; }

    int? ISearchAsYouTypeProperty.MaxShingleSize { get; set; }

    bool? ISearchAsYouTypeProperty.Norms { get; set; }

    private ISearchAsYouTypeProperty Self => (ISearchAsYouTypeProperty) this;

    TermVectorOption? ISearchAsYouTypeProperty.TermVector { get; set; }
  }
}
