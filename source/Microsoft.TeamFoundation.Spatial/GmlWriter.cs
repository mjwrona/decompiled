// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GmlWriter
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Microsoft.Spatial
{
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gml", Justification = "Gml is the common name for this format")]
  internal sealed class GmlWriter : DrawBoth
  {
    private XmlWriter writer;
    private Stack<SpatialType> parentStack;
    private bool coordinateSystemWritten;
    private CoordinateSystem currentCoordinateSystem;
    private bool figureWritten;
    private bool shouldWriteContainerWrapper;

    public GmlWriter(XmlWriter writer)
    {
      this.writer = writer;
      this.OnReset();
    }

    protected override SpatialType OnBeginGeography(SpatialType type)
    {
      this.BeginGeo(type);
      return type;
    }

    protected override GeographyPosition OnLineTo(GeographyPosition position)
    {
      this.WritePoint(position.Latitude, position.Longitude, position.Z, position.M);
      return position;
    }

    protected override void OnEndGeography() => this.EndGeo();

    protected override SpatialType OnBeginGeometry(SpatialType type)
    {
      this.BeginGeo(type);
      return type;
    }

    protected override GeometryPosition OnLineTo(GeometryPosition position)
    {
      this.WritePoint(position.X, position.Y, position.Z, position.M);
      return position;
    }

    protected override void OnEndGeometry() => this.EndGeo();

    protected override CoordinateSystem OnSetCoordinateSystem(CoordinateSystem coordinateSystem)
    {
      this.currentCoordinateSystem = coordinateSystem;
      return coordinateSystem;
    }

    protected override GeographyPosition OnBeginFigure(GeographyPosition position)
    {
      this.BeginFigure(position.Latitude, position.Longitude, position.Z, position.M);
      return position;
    }

    protected override GeometryPosition OnBeginFigure(GeometryPosition position)
    {
      this.BeginFigure(position.X, position.Y, position.Z, position.M);
      return position;
    }

    protected override void OnEndFigure()
    {
      if (this.parentStack.Peek() != SpatialType.Polygon)
        return;
      this.writer.WriteEndElement();
      this.writer.WriteEndElement();
    }

    protected override void OnReset()
    {
      this.parentStack = new Stack<SpatialType>();
      this.coordinateSystemWritten = false;
      this.currentCoordinateSystem = (CoordinateSystem) null;
      this.figureWritten = false;
      this.shouldWriteContainerWrapper = false;
    }

    private void BeginFigure(double x, double y, double? z, double? m)
    {
      if (this.parentStack.Peek() == SpatialType.Polygon)
      {
        this.WriteStartElement(this.figureWritten ? "interior" : "exterior");
        this.WriteStartElement("LinearRing");
      }
      this.figureWritten = true;
      this.WritePoint(x, y, z, m);
    }

    private void BeginGeo(SpatialType type)
    {
      if (this.shouldWriteContainerWrapper)
      {
        switch (this.parentStack.Peek())
        {
          case SpatialType.MultiPoint:
            this.WriteStartElement("pointMembers");
            break;
          case SpatialType.MultiLineString:
            this.WriteStartElement("curveMembers");
            break;
          case SpatialType.MultiPolygon:
            this.WriteStartElement("surfaceMembers");
            break;
          case SpatialType.Collection:
            this.WriteStartElement("geometryMembers");
            break;
        }
        this.shouldWriteContainerWrapper = false;
      }
      this.figureWritten = false;
      this.parentStack.Push(type);
      switch (type)
      {
        case SpatialType.Point:
          this.WriteStartElement("Point");
          break;
        case SpatialType.LineString:
          this.WriteStartElement("LineString");
          break;
        case SpatialType.Polygon:
          this.WriteStartElement("Polygon");
          break;
        case SpatialType.MultiPoint:
          this.shouldWriteContainerWrapper = true;
          this.WriteStartElement("MultiPoint");
          break;
        case SpatialType.MultiLineString:
          this.shouldWriteContainerWrapper = true;
          this.WriteStartElement("MultiCurve");
          break;
        case SpatialType.MultiPolygon:
          this.shouldWriteContainerWrapper = true;
          this.WriteStartElement("MultiSurface");
          break;
        case SpatialType.Collection:
          this.shouldWriteContainerWrapper = true;
          this.WriteStartElement("MultiGeometry");
          break;
        case SpatialType.FullGlobe:
          this.writer.WriteStartElement("FullGlobe", "http://schemas.microsoft.com/sqlserver/2011/geography");
          break;
        default:
          throw new NotSupportedException(Strings.Validator_InvalidType((object) type));
      }
      this.WriteCoordinateSystem();
    }

    private void WriteStartElement(string elementName) => this.writer.WriteStartElement("gml", elementName, "http://www.opengis.net/gml");

    private void WriteCoordinateSystem()
    {
      if (this.coordinateSystemWritten || this.currentCoordinateSystem == null)
        return;
      this.coordinateSystemWritten = true;
      this.writer.WriteAttributeString("srsName", "http://www.opengis.net/def/crs/EPSG/0/" + this.currentCoordinateSystem.Id);
    }

    private void WritePoint(double x, double y, double? z, double? m)
    {
      this.WriteStartElement("pos");
      this.writer.WriteValue(x);
      this.writer.WriteValue(" ");
      this.writer.WriteValue(y);
      if (z.HasValue)
      {
        this.writer.WriteValue(" ");
        this.writer.WriteValue(z.Value);
        if (m.HasValue)
        {
          this.writer.WriteValue(" ");
          this.writer.WriteValue(m.Value);
        }
      }
      else if (m.HasValue)
      {
        this.writer.WriteValue(" ");
        this.writer.WriteValue(double.NaN);
        this.writer.WriteValue(" ");
        this.writer.WriteValue(m.Value);
      }
      this.writer.WriteEndElement();
    }

    private void EndGeo()
    {
      switch (this.parentStack.Pop())
      {
        case SpatialType.Point:
          if (!this.figureWritten)
          {
            this.WriteStartElement("pos");
            this.writer.WriteEndElement();
          }
          this.writer.WriteEndElement();
          break;
        case SpatialType.LineString:
          if (!this.figureWritten)
          {
            this.WriteStartElement("posList");
            this.writer.WriteEndElement();
          }
          this.writer.WriteEndElement();
          break;
        case SpatialType.Polygon:
        case SpatialType.FullGlobe:
          this.writer.WriteEndElement();
          break;
        case SpatialType.MultiPoint:
        case SpatialType.MultiLineString:
        case SpatialType.MultiPolygon:
        case SpatialType.Collection:
          if (!this.shouldWriteContainerWrapper)
            this.writer.WriteEndElement();
          this.writer.WriteEndElement();
          this.shouldWriteContainerWrapper = false;
          break;
      }
    }
  }
}
