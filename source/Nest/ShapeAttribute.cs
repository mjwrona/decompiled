// Decompiled with JetBrains decompiler
// Type: Nest.ShapeAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class ShapeAttribute : 
    ElasticsearchDocValuesPropertyAttributeBase,
    IShapeProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public ShapeAttribute()
      : base(FieldType.Shape)
    {
    }

    bool? IShapeProperty.IgnoreMalformed { get; set; }

    bool? IShapeProperty.IgnoreZValue { get; set; }

    ShapeOrientation? IShapeProperty.Orientation { get; set; }

    private IShapeProperty Self => (IShapeProperty) this;

    bool? IShapeProperty.Coerce { get; set; }

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

    public ShapeOrientation Orientation
    {
      get => this.Self.Orientation.GetValueOrDefault(ShapeOrientation.CounterClockWise);
      set => this.Self.Orientation = new ShapeOrientation?(value);
    }

    public bool Coerce
    {
      get => this.Self.Coerce.GetValueOrDefault(true);
      set => this.Self.Coerce = new bool?(value);
    }
  }
}
