// Decompiled with JetBrains decompiler
// Type: Nest.DateAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DateAttribute : 
    ElasticsearchDocValuesPropertyAttributeBase,
    IDateProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public DateAttribute()
      : base(FieldType.Date)
    {
    }

    public double Boost
    {
      get => this.Self.Boost.GetValueOrDefault();
      set => this.Self.Boost = new double?(value);
    }

    public string Format
    {
      get => this.Self.Format;
      set => this.Self.Format = value;
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

    public DateTime NullValue
    {
      get => this.Self.NullValue.GetValueOrDefault();
      set => this.Self.NullValue = new DateTime?(value);
    }

    double? IDateProperty.Boost { get; set; }

    INumericFielddata IDateProperty.Fielddata { get; set; }

    string IDateProperty.Format { get; set; }

    bool? IDateProperty.IgnoreMalformed { get; set; }

    bool? IDateProperty.Index { get; set; }

    DateTime? IDateProperty.NullValue { get; set; }

    private IDateProperty Self => (IDateProperty) this;
  }
}
