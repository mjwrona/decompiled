// Decompiled with JetBrains decompiler
// Type: Nest.TokenCountAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class TokenCountAttribute : 
    ElasticsearchDocValuesPropertyAttributeBase,
    ITokenCountProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public TokenCountAttribute()
      : base(FieldType.TokenCount)
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

    public bool EnablePositionIncrements
    {
      get => this.Self.EnablePositionIncrements.GetValueOrDefault(true);
      set => this.Self.EnablePositionIncrements = new bool?(value);
    }

    public bool Index
    {
      get => this.Self.Index.GetValueOrDefault();
      set => this.Self.Index = new bool?(value);
    }

    public double NullValue
    {
      get => this.Self.NullValue.GetValueOrDefault();
      set => this.Self.NullValue = new double?(value);
    }

    string ITokenCountProperty.Analyzer { get; set; }

    double? ITokenCountProperty.Boost { get; set; }

    bool? ITokenCountProperty.EnablePositionIncrements { get; set; }

    bool? ITokenCountProperty.Index { get; set; }

    double? ITokenCountProperty.NullValue { get; set; }

    private ITokenCountProperty Self => (ITokenCountProperty) this;
  }
}
