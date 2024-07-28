// Decompiled with JetBrains decompiler
// Type: Nest.ShapeQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class ShapeQuery : FieldNameQueryBase, IShapeQuery, IFieldNameQuery, IQuery
  {
    public bool? IgnoreUnmapped { get; set; }

    public IFieldLookup IndexedShape { get; set; }

    public ShapeRelation? Relation { get; set; }

    public IGeoShape Shape { get; set; }

    protected override bool Conditionless => ShapeQuery.IsConditionless((IShapeQuery) this);

    internal static bool IsConditionless(IShapeQuery q)
    {
      if (q.Field.IsConditionless())
        return true;
      switch (q.Shape)
      {
        case ICircleGeoShape circleGeoShape:
          return circleGeoShape.Coordinates == null || string.IsNullOrEmpty(circleGeoShape?.Radius);
        case IEnvelopeGeoShape envelopeGeoShape:
          return envelopeGeoShape.Coordinates == null;
        case IGeometryCollection geometryCollection:
          return geometryCollection.Geometries == null;
        case ILineStringGeoShape lineStringGeoShape1:
          return lineStringGeoShape1.Coordinates == null;
        case IMultiLineStringGeoShape lineStringGeoShape2:
          return lineStringGeoShape2.Coordinates == null;
        case IMultiPointGeoShape multiPointGeoShape:
          return multiPointGeoShape.Coordinates == null;
        case IMultiPolygonGeoShape multiPolygonGeoShape:
          return multiPolygonGeoShape.Coordinates == null;
        case IPointGeoShape pointGeoShape:
          return pointGeoShape.Coordinates == null;
        case IPolygonGeoShape polygonGeoShape:
          return polygonGeoShape.Coordinates == null;
        case null:
          return q.IndexedShape.IsConditionless();
        default:
          return true;
      }
    }

    internal override void InternalWrapInContainer(IQueryContainer container) => container.Shape = (IShapeQuery) this;
  }
}
