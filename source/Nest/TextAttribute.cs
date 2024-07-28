// Decompiled with JetBrains decompiler
// Type: Nest.TextAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class TextAttribute : 
    ElasticsearchCorePropertyAttributeBase,
    ITextProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public TextAttribute()
      : base(FieldType.Text)
    {
    }

    protected TextAttribute(FieldType fieldType)
      : base(fieldType)
    {
    }

    public string Analyzer
    {
      get => this.Self.Analyzer;
      set => this.Self.Analyzer = value;
    }

    public double Boost
    {
      get => this.Self.Boost.GetValueOrDefault();
      set => this.Self.Boost = new double?(value);
    }

    public bool EagerGlobalOrdinals
    {
      get => this.Self.EagerGlobalOrdinals.GetValueOrDefault();
      set => this.Self.EagerGlobalOrdinals = new bool?(value);
    }

    public bool Fielddata
    {
      get => this.Self.Fielddata.GetValueOrDefault();
      set => this.Self.Fielddata = new bool?(value);
    }

    public bool Index
    {
      get => this.Self.Index.GetValueOrDefault();
      set => this.Self.Index = new bool?(value);
    }

    public IndexOptions IndexOptions
    {
      get => this.Self.IndexOptions.GetValueOrDefault();
      set => this.Self.IndexOptions = new IndexOptions?(value);
    }

    public bool IndexPhrases
    {
      get => this.Self.IndexPhrases.GetValueOrDefault();
      set => this.Self.IndexPhrases = new bool?(value);
    }

    public bool Norms
    {
      get => this.Self.Norms.GetValueOrDefault(true);
      set => this.Self.Norms = new bool?(value);
    }

    public int PositionIncrementGap
    {
      get => this.Self.PositionIncrementGap.GetValueOrDefault();
      set => this.Self.PositionIncrementGap = new int?(value);
    }

    public string SearchAnalyzer
    {
      get => this.Self.SearchAnalyzer;
      set => this.Self.SearchAnalyzer = value;
    }

    public string SearchQuoteAnalyzer
    {
      get => this.Self.SearchQuoteAnalyzer;
      set => this.Self.SearchQuoteAnalyzer = value;
    }

    public TermVectorOption TermVector
    {
      get => this.Self.TermVector.GetValueOrDefault();
      set => this.Self.TermVector = new TermVectorOption?(value);
    }

    string ITextProperty.Analyzer { get; set; }

    double? ITextProperty.Boost { get; set; }

    bool? ITextProperty.EagerGlobalOrdinals { get; set; }

    bool? ITextProperty.Fielddata { get; set; }

    IFielddataFrequencyFilter ITextProperty.FielddataFrequencyFilter { get; set; }

    bool? ITextProperty.Index { get; set; }

    IndexOptions? ITextProperty.IndexOptions { get; set; }

    bool? ITextProperty.IndexPhrases { get; set; }

    ITextIndexPrefixes ITextProperty.IndexPrefixes { get; set; }

    bool? ITextProperty.Norms { get; set; }

    int? ITextProperty.PositionIncrementGap { get; set; }

    string ITextProperty.SearchAnalyzer { get; set; }

    string ITextProperty.SearchQuoteAnalyzer { get; set; }

    private ITextProperty Self => (ITextProperty) this;

    TermVectorOption? ITextProperty.TermVector { get; set; }
  }
}
