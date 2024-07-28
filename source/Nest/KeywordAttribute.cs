// Decompiled with JetBrains decompiler
// Type: Nest.KeywordAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class KeywordAttribute : 
    ElasticsearchDocValuesPropertyAttributeBase,
    IKeywordProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public KeywordAttribute()
      : base(FieldType.Keyword)
    {
    }

    double? IKeywordProperty.Boost { get; set; }

    bool? IKeywordProperty.EagerGlobalOrdinals { get; set; }

    int? IKeywordProperty.IgnoreAbove { get; set; }

    bool? IKeywordProperty.Index { get; set; }

    IndexOptions? IKeywordProperty.IndexOptions { get; set; }

    string IKeywordProperty.Normalizer { get; set; }

    bool? IKeywordProperty.Norms { get; set; }

    string IKeywordProperty.NullValue { get; set; }

    private IKeywordProperty Self => (IKeywordProperty) this;

    bool? IKeywordProperty.SplitQueriesOnWhitespace { get; set; }

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

    public int IgnoreAbove
    {
      get => this.Self.IgnoreAbove.GetValueOrDefault();
      set => this.Self.IgnoreAbove = new int?(value);
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

    public string NullValue
    {
      get => this.Self.NullValue;
      set => this.Self.NullValue = value;
    }

    public bool Norms
    {
      get => this.Self.Norms.GetValueOrDefault(true);
      set => this.Self.Norms = new bool?(value);
    }

    public bool SplitQueriesOnWhitespace
    {
      get => this.Self.SplitQueriesOnWhitespace.GetValueOrDefault(false);
      set => this.Self.SplitQueriesOnWhitespace = new bool?(value);
    }

    public string Normalizer
    {
      get => this.Self.Normalizer;
      set => this.Self.Normalizer = value;
    }
  }
}
