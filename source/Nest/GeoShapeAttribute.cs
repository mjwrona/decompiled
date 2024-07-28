// Decompiled with JetBrains decompiler
// Type: Nest.GeoShapeAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class GeoShapeAttribute : 
    ElasticsearchDocValuesPropertyAttributeBase,
    IGeoShapeProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public GeoShapeAttribute()
      : base(FieldType.GeoShape)
    {
    }

    bool? IGeoShapeProperty.IgnoreMalformed { get; set; }

    bool? IGeoShapeProperty.IgnoreZValue { get; set; }

    GeoOrientation? IGeoShapeProperty.Orientation { get; set; }

    private IGeoShapeProperty Self => (IGeoShapeProperty) this;

    GeoStrategy? IGeoShapeProperty.Strategy { get; set; }

    bool? IGeoShapeProperty.Coerce { get; set; }

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

    public GeoOrientation Orientation
    {
      get => this.Self.Orientation.GetValueOrDefault(GeoOrientation.CounterClockWise);
      set => this.Self.Orientation = new GeoOrientation?(value);
    }

    public GeoStrategy Strategy
    {
      get => this.Self.Strategy.GetValueOrDefault(GeoStrategy.Recursive);
      set => this.Self.Strategy = new GeoStrategy?(value);
    }

    public bool Coerce
    {
      get => this.Self.Coerce.GetValueOrDefault(true);
      set => this.Self.Coerce = new bool?(value);
    }
  }
}
