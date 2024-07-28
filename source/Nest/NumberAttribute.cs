// Decompiled with JetBrains decompiler
// Type: Nest.NumberAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class NumberAttribute : 
    ElasticsearchDocValuesPropertyAttributeBase,
    INumberProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public NumberAttribute()
      : base(FieldType.Float)
    {
    }

    public NumberAttribute(NumberType type)
      : base(type.ToFieldType())
    {
    }

    public double Boost
    {
      get => this.Self.Boost.GetValueOrDefault();
      set => this.Self.Boost = new double?(value);
    }

    public bool Coerce
    {
      get => this.Self.Coerce.GetValueOrDefault();
      set => this.Self.Coerce = new bool?(value);
    }

    public bool IgnoreMalformed
    {
      get => this.Self.IgnoreMalformed.GetValueOrDefault();
      set => this.Self.IgnoreMalformed = new bool?(value);
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

    public double ScalingFactor
    {
      get => this.Self.ScalingFactor.GetValueOrDefault();
      set => this.Self.ScalingFactor = new double?(value);
    }

    public IInlineScript Script
    {
      get => this.Self.Script;
      set => this.Self.Script = value;
    }

    public OnScriptError OnScriptError
    {
      get => this.Self.OnScriptError.GetValueOrDefault();
      set => this.Self.OnScriptError = new OnScriptError?(value);
    }

    double? INumberProperty.Boost { get; set; }

    bool? INumberProperty.Coerce { get; set; }

    INumericFielddata INumberProperty.Fielddata { get; set; }

    bool? INumberProperty.IgnoreMalformed { get; set; }

    bool? INumberProperty.Index { get; set; }

    double? INumberProperty.NullValue { get; set; }

    double? INumberProperty.ScalingFactor { get; set; }

    IInlineScript INumberProperty.Script { get; set; }

    OnScriptError? INumberProperty.OnScriptError { get; set; }

    private INumberProperty Self => (INumberProperty) this;
  }
}
