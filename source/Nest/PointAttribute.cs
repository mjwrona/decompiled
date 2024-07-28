// Decompiled with JetBrains decompiler
// Type: Nest.PointAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class PointAttribute : 
    ElasticsearchPropertyAttributeBase,
    IPointProperty,
    IProperty,
    IFieldMapping
  {
    public PointAttribute()
      : base(FieldType.Point)
    {
    }

    bool? IPointProperty.IgnoreMalformed { get; set; }

    bool? IPointProperty.IgnoreZValue { get; set; }

    CartesianPoint IPointProperty.NullValue { get; set; }

    private IPointProperty Self => (IPointProperty) this;

    public bool IgnoreMalformed
    {
      get => this.Self.IgnoreMalformed.GetValueOrDefault(false);
      set => this.Self.IgnoreMalformed = new bool?(value);
    }

    public bool IgnoreZValue
    {
      get => this.Self.IgnoreZValue.GetValueOrDefault(true);
      set => this.Self.IgnoreZValue = new bool?(value);
    }

    public CartesianPoint NullValue
    {
      get => this.Self.NullValue;
      set => this.Self.NullValue = value;
    }
  }
}
