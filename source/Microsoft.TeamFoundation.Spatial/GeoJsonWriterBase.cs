// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeoJsonWriterBase
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Spatial
{
  internal abstract class GeoJsonWriterBase : DrawBoth
  {
    private readonly Stack<SpatialType> stack;
    private CoordinateSystem currentCoordinateSystem;
    private bool figureDrawn;

    public GeoJsonWriterBase() => this.stack = new Stack<SpatialType>();

    private bool ShapeHasObjectScope => this.IsTopLevel || this.stack.Peek() == SpatialType.Collection;

    private bool IsTopLevel => this.stack.Count == 0;

    private bool FigureHasArrayScope => this.stack.Peek() != SpatialType.Point;

    protected override GeographyPosition OnLineTo(GeographyPosition position)
    {
      this.WriteControlPoint(position.Longitude, position.Latitude, position.Z, position.M);
      return position;
    }

    protected override GeometryPosition OnLineTo(GeometryPosition position)
    {
      this.WriteControlPoint(position.X, position.Y, position.Z, position.M);
      return position;
    }

    protected override SpatialType OnBeginGeography(SpatialType type)
    {
      this.BeginShape(type, CoordinateSystem.DefaultGeography);
      return type;
    }

    protected override SpatialType OnBeginGeometry(SpatialType type)
    {
      this.BeginShape(type, CoordinateSystem.DefaultGeometry);
      return type;
    }

    protected override GeographyPosition OnBeginFigure(GeographyPosition position)
    {
      this.BeginFigure();
      this.WriteControlPoint(position.Longitude, position.Latitude, position.Z, position.M);
      return position;
    }

    protected override GeometryPosition OnBeginFigure(GeometryPosition position)
    {
      this.BeginFigure();
      this.WriteControlPoint(position.X, position.Y, position.Z, position.M);
      return position;
    }

    protected override void OnEndFigure() => this.EndFigure();

    protected override void OnEndGeography() => this.EndShape();

    protected override void OnEndGeometry() => this.EndShape();

    protected override CoordinateSystem OnSetCoordinateSystem(CoordinateSystem coordinateSystem)
    {
      this.SetCoordinateSystem(coordinateSystem);
      return coordinateSystem;
    }

    protected override void OnReset() => this.Reset();

    protected abstract void AddPropertyName(string name);

    protected abstract void AddValue(string value);

    protected abstract void AddValue(double value);

    protected abstract void StartObjectScope();

    protected abstract void StartArrayScope();

    protected abstract void EndObjectScope();

    protected abstract void EndArrayScope();

    protected virtual void Reset()
    {
      this.stack.Clear();
      this.currentCoordinateSystem = (CoordinateSystem) null;
    }

    private static string GetSpatialTypeName(SpatialType type)
    {
      switch (type)
      {
        case SpatialType.Point:
          return "Point";
        case SpatialType.LineString:
          return "LineString";
        case SpatialType.Polygon:
          return "Polygon";
        case SpatialType.MultiPoint:
          return "MultiPoint";
        case SpatialType.MultiLineString:
          return "MultiLineString";
        case SpatialType.MultiPolygon:
          return "MultiPolygon";
        case SpatialType.Collection:
          return "GeometryCollection";
        default:
          throw new NotImplementedException();
      }
    }

    private static string GetDataName(SpatialType type)
    {
      switch (type)
      {
        case SpatialType.Point:
        case SpatialType.LineString:
        case SpatialType.Polygon:
        case SpatialType.MultiPoint:
        case SpatialType.MultiLineString:
        case SpatialType.MultiPolygon:
          return "coordinates";
        case SpatialType.Collection:
          return "geometries";
        default:
          throw new NotImplementedException();
      }
    }

    private static bool TypeHasArrayScope(SpatialType type) => type != SpatialType.Point && type != SpatialType.LineString;

    private void SetCoordinateSystem(CoordinateSystem coordinateSystem) => this.currentCoordinateSystem = coordinateSystem;

    private void BeginShape(SpatialType type, CoordinateSystem defaultCoordinateSystem)
    {
      if (this.currentCoordinateSystem == null)
        this.currentCoordinateSystem = defaultCoordinateSystem;
      if (this.ShapeHasObjectScope)
        this.WriteShapeHeader(type);
      if (GeoJsonWriterBase.TypeHasArrayScope(type))
        this.StartArrayScope();
      this.stack.Push(type);
      this.figureDrawn = false;
    }

    private void WriteShapeHeader(SpatialType type)
    {
      this.StartObjectScope();
      this.AddPropertyName(nameof (type));
      this.AddValue(GeoJsonWriterBase.GetSpatialTypeName(type));
      this.AddPropertyName(GeoJsonWriterBase.GetDataName(type));
    }

    private void BeginFigure()
    {
      if (this.FigureHasArrayScope)
        this.StartArrayScope();
      this.figureDrawn = true;
    }

    private void WriteControlPoint(double first, double second, double? z, double? m)
    {
      this.StartArrayScope();
      this.AddValue(first);
      this.AddValue(second);
      if (z.HasValue)
      {
        this.AddValue(z.Value);
        if (m.HasValue)
          this.AddValue(m.Value);
      }
      else if (m.HasValue)
      {
        this.AddValue((string) null);
        this.AddValue(m.Value);
      }
      this.EndArrayScope();
    }

    private void EndFigure()
    {
      if (!this.FigureHasArrayScope)
        return;
      this.EndArrayScope();
    }

    private void EndShape()
    {
      if (GeoJsonWriterBase.TypeHasArrayScope(this.stack.Pop()))
        this.EndArrayScope();
      else if (!this.figureDrawn)
      {
        this.StartArrayScope();
        this.EndArrayScope();
      }
      if (this.IsTopLevel)
        this.WriteCrs();
      if (!this.ShapeHasObjectScope)
        return;
      this.EndObjectScope();
    }

    private void WriteCrs()
    {
      this.AddPropertyName("crs");
      this.StartObjectScope();
      this.AddPropertyName("type");
      this.AddValue("name");
      this.AddPropertyName("properties");
      this.StartObjectScope();
      this.AddPropertyName("name");
      this.AddValue("EPSG" + (object) ':' + this.currentCoordinateSystem.Id);
      this.EndObjectScope();
      this.EndObjectScope();
    }
  }
}
