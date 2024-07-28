// Decompiled with JetBrains decompiler
// Type: Nest.GeoPointAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class GeoPointAttribute : 
    ElasticsearchDocValuesPropertyAttributeBase,
    IGeoPointProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public GeoPointAttribute()
      : base(FieldType.GeoPoint)
    {
    }

    public bool IgnoreMalformed
    {
      get => this.Self.IgnoreMalformed.GetValueOrDefault();
      set => this.Self.IgnoreMalformed = new bool?(value);
    }

    public bool IgnoreZValue
    {
      get => this.Self.IgnoreZValue.GetValueOrDefault(true);
      set => this.Self.IgnoreZValue = new bool?(value);
    }

    bool? IGeoPointProperty.IgnoreMalformed { get; set; }

    bool? IGeoPointProperty.IgnoreZValue { get; set; }

    GeoLocation IGeoPointProperty.NullValue { get; set; }

    private IGeoPointProperty Self => (IGeoPointProperty) this;
  }
}
