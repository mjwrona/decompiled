// Decompiled with JetBrains decompiler
// Type: Nest.BooleanAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class BooleanAttribute : 
    ElasticsearchDocValuesPropertyAttributeBase,
    IBooleanProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public BooleanAttribute()
      : base(FieldType.Boolean)
    {
    }

    public double Boost
    {
      get => this.Self.Boost.GetValueOrDefault();
      set => this.Self.Boost = new double?(value);
    }

    public bool Index
    {
      get => this.Self.Index.GetValueOrDefault();
      set => this.Self.Index = new bool?(value);
    }

    public bool NullValue
    {
      get => this.Self.NullValue.GetValueOrDefault();
      set => this.Self.NullValue = new bool?(value);
    }

    double? IBooleanProperty.Boost { get; set; }

    INumericFielddata IBooleanProperty.Fielddata { get; set; }

    bool? IBooleanProperty.Index { get; set; }

    bool? IBooleanProperty.NullValue { get; set; }

    private IBooleanProperty Self => (IBooleanProperty) this;
  }
}
