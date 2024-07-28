// Decompiled with JetBrains decompiler
// Type: Nest.GeoBoundingBoxQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class GeoBoundingBoxQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<GeoBoundingBoxQueryDescriptor<T>, IGeoBoundingBoxQuery, T>,
    IGeoBoundingBoxQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => GeoBoundingBoxQuery.IsConditionless((IGeoBoundingBoxQuery) this);

    IBoundingBox IGeoBoundingBoxQuery.BoundingBox { get; set; }

    GeoExecution? IGeoBoundingBoxQuery.Type { get; set; }

    GeoValidationMethod? IGeoBoundingBoxQuery.ValidationMethod { get; set; }

    bool? IGeoBoundingBoxQuery.IgnoreUnmapped { get; set; }

    public GeoBoundingBoxQueryDescriptor<T> BoundingBox(
      double topLeftLat,
      double topLeftLon,
      double bottomRightLat,
      double bottomRightLon)
    {
      return this.BoundingBox((Func<BoundingBoxDescriptor, IBoundingBox>) (f => (IBoundingBox) f.TopLeft(topLeftLat, topLeftLon).BottomRight(bottomRightLat, bottomRightLon)));
    }

    public GeoBoundingBoxQueryDescriptor<T> BoundingBox(
      GeoLocation topLeft,
      GeoLocation bottomRight)
    {
      return this.BoundingBox((Func<BoundingBoxDescriptor, IBoundingBox>) (f => (IBoundingBox) f.TopLeft(topLeft).BottomRight(bottomRight)));
    }

    public GeoBoundingBoxQueryDescriptor<T> BoundingBox(string wkt) => this.BoundingBox((Func<BoundingBoxDescriptor, IBoundingBox>) (f => (IBoundingBox) f.WellKnownText(wkt)));

    public GeoBoundingBoxQueryDescriptor<T> BoundingBox(
      Func<BoundingBoxDescriptor, IBoundingBox> boundingBoxSelector)
    {
      return this.Assign<Func<BoundingBoxDescriptor, IBoundingBox>>(boundingBoxSelector, (Action<IGeoBoundingBoxQuery, Func<BoundingBoxDescriptor, IBoundingBox>>) ((a, v) => a.BoundingBox = v != null ? v(new BoundingBoxDescriptor()) : (IBoundingBox) null));
    }

    public GeoBoundingBoxQueryDescriptor<T> Type(GeoExecution? type) => this.Assign<GeoExecution?>(type, (Action<IGeoBoundingBoxQuery, GeoExecution?>) ((a, v) => a.Type = v));

    public GeoBoundingBoxQueryDescriptor<T> ValidationMethod(GeoValidationMethod? validation) => this.Assign<GeoValidationMethod?>(validation, (Action<IGeoBoundingBoxQuery, GeoValidationMethod?>) ((a, v) => a.ValidationMethod = v));

    public GeoBoundingBoxQueryDescriptor<T> IgnoreUnmapped(bool? ignoreUnmapped = true) => this.Assign<bool?>(ignoreUnmapped, (Action<IGeoBoundingBoxQuery, bool?>) ((a, v) => a.IgnoreUnmapped = v));
  }
}
