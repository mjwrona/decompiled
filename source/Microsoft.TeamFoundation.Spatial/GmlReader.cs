// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GmlReader
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Spatial
{
  internal class GmlReader : SpatialReader<XmlReader>
  {
    public GmlReader(SpatialPipeline destination)
      : base(destination)
    {
    }

    protected override void ReadGeographyImplementation(XmlReader input) => new GmlReader.Parser(input, (TypeWashedPipeline) new TypeWashedToGeographyLatLongPipeline(this.Destination)).Read();

    protected override void ReadGeometryImplementation(XmlReader input) => new GmlReader.Parser(input, (TypeWashedPipeline) new TypeWashedToGeometryPipeline(this.Destination)).Read();

    private class Parser
    {
      private static readonly char[] coordinateDelimiter = new char[4]
      {
        ' ',
        '\t',
        '\r',
        '\n'
      };
      private static readonly Dictionary<string, string> skippableElements = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal)
      {
        {
          "name",
          "name"
        },
        {
          "description",
          "description"
        },
        {
          "metaDataProperty",
          "metaDataProperty"
        },
        {
          "descriptionReference",
          "descriptionReference"
        },
        {
          "identifier",
          "identifier"
        }
      };
      private readonly string gmlNamespace;
      private readonly string fullGlobeNamespace;
      private readonly TypeWashedPipeline pipeline;
      private readonly XmlReader reader;
      private int points;

      internal Parser(XmlReader reader, TypeWashedPipeline pipeline)
      {
        this.reader = reader;
        this.pipeline = pipeline;
        XmlNameTable nameTable = this.reader.NameTable;
        this.gmlNamespace = nameTable.Add("http://www.opengis.net/gml");
        this.fullGlobeNamespace = nameTable.Add("http://schemas.microsoft.com/sqlserver/2011/geography");
      }

      public void Read() => this.ParseGmlGeometry(true);

      private void ParseGmlGeometry(bool readCoordinateSystem)
      {
        if (!this.reader.IsStartElement())
          throw new FormatException(Strings.GmlReader_ExpectReaderAtElement);
        if ((object) this.reader.NamespaceURI == (object) this.gmlNamespace)
        {
          this.ReadAttributes(readCoordinateSystem);
          switch (this.reader.LocalName)
          {
            case "LineString":
              this.ParseGmlLineStringShape();
              break;
            case "MultiCurve":
              this.ParseGmlMultiCurveShape();
              break;
            case "MultiGeometry":
              this.ParseGmlMultiGeometryShape();
              break;
            case "MultiPoint":
              this.ParseGmlMultiPointShape();
              break;
            case "MultiSurface":
              this.ParseGmlMultiSurfaceShape();
              break;
            case "Point":
              this.ParseGmlPointShape();
              break;
            case "Polygon":
              this.ParseGmlPolygonShape();
              break;
            default:
              throw new FormatException(Strings.GmlReader_InvalidSpatialType((object) this.reader.LocalName));
          }
        }
        else
        {
          if ((object) this.reader.NamespaceURI != (object) this.fullGlobeNamespace || !this.reader.LocalName.Equals("FullGlobe"))
            throw new FormatException(Strings.GmlReader_ExpectReaderAtElement);
          this.ReadAttributes(readCoordinateSystem);
          this.ParseGmlFullGlobeElement();
        }
      }

      private void ReadAttributes(bool expectSrsName)
      {
        bool flag = false;
        int content = (int) this.reader.MoveToContent();
        if (this.reader.MoveToFirstAttribute())
        {
          do
          {
            if (!this.reader.NamespaceURI.Equals("http://www.w3.org/2000/xmlns/", StringComparison.Ordinal))
            {
              string localName = this.reader.LocalName;
              switch (localName)
              {
                case "axisLabels":
                case "uomLabels":
                case "count":
                case "id":
                  break;
                case "srsName":
                  if (!expectSrsName)
                  {
                    this.reader.MoveToElement();
                    throw new FormatException(Strings.GmlReader_InvalidAttribute((object) localName, (object) this.reader.Name));
                  }
                  string str = this.reader.Value;
                  if (!str.StartsWith("http://www.opengis.net/def/crs/EPSG/0/", StringComparison.Ordinal))
                    throw new FormatException(Strings.GmlReader_InvalidSrsName((object) "http://www.opengis.net/def/crs/EPSG/0/"));
                  this.pipeline.SetCoordinateSystem(new int?(XmlConvert.ToInt32(str.Substring("http://www.opengis.net/def/crs/EPSG/0/".Length))));
                  flag = true;
                  break;
                default:
                  this.reader.MoveToElement();
                  throw new FormatException(Strings.GmlReader_InvalidAttribute((object) localName, (object) this.reader.Name));
              }
            }
          }
          while (this.reader.MoveToNextAttribute());
          this.reader.MoveToElement();
        }
        if (!expectSrsName || flag)
          return;
        this.pipeline.SetCoordinateSystem(new int?());
      }

      private void ParseGmlPointShape()
      {
        this.pipeline.BeginGeo(SpatialType.Point);
        this.PrepareFigure();
        this.ParseGmlPointElement(true);
        this.EndFigure();
        this.pipeline.EndGeo();
      }

      private void ParseGmlLineStringShape()
      {
        this.pipeline.BeginGeo(SpatialType.LineString);
        this.PrepareFigure();
        this.ParseGmlLineString();
        this.EndFigure();
        this.pipeline.EndGeo();
      }

      private void ParseGmlPolygonShape()
      {
        this.pipeline.BeginGeo(SpatialType.Polygon);
        if (this.ReadStartOrEmptyElement("Polygon"))
        {
          this.ReadSkippableElements();
          if (!this.IsEndElement("Polygon"))
          {
            this.PrepareFigure();
            this.ParseGmlRingElement("exterior");
            this.EndFigure();
            this.ReadSkippableElements();
            while (this.IsStartElement("interior"))
            {
              this.PrepareFigure();
              this.ParseGmlRingElement("interior");
              this.EndFigure();
              this.ReadSkippableElements();
            }
          }
          this.ReadSkippableElements();
          this.ReadEndElement();
        }
        this.pipeline.EndGeo();
      }

      private void ParseGmlMultiPointShape()
      {
        this.pipeline.BeginGeo(SpatialType.MultiPoint);
        this.ParseMultiItemElement("MultiPoint", "pointMember", "pointMembers", new Action(this.ParseGmlPointShape));
        this.pipeline.EndGeo();
      }

      private void ParseGmlMultiCurveShape()
      {
        this.pipeline.BeginGeo(SpatialType.MultiLineString);
        this.ParseMultiItemElement("MultiCurve", "curveMember", "curveMembers", new Action(this.ParseGmlLineStringShape));
        this.pipeline.EndGeo();
      }

      private void ParseGmlMultiSurfaceShape()
      {
        this.pipeline.BeginGeo(SpatialType.MultiPolygon);
        this.ParseMultiItemElement("MultiSurface", "surfaceMember", "surfaceMembers", new Action(this.ParseGmlPolygonShape));
        this.pipeline.EndGeo();
      }

      private void ParseGmlMultiGeometryShape()
      {
        this.pipeline.BeginGeo(SpatialType.Collection);
        this.ParseMultiItemElement("MultiGeometry", "geometryMember", "geometryMembers", (Action) (() => this.ParseGmlGeometry(false)));
        this.pipeline.EndGeo();
      }

      private void ParseGmlFullGlobeElement()
      {
        this.pipeline.BeginGeo(SpatialType.FullGlobe);
        if (this.ReadStartOrEmptyElement("FullGlobe") && this.IsEndElement("FullGlobe"))
          this.ReadEndElement();
        this.pipeline.EndGeo();
      }

      private void ParseGmlPointElement(bool allowEmpty)
      {
        if (!this.ReadStartOrEmptyElement("Point"))
          return;
        this.ReadSkippableElements();
        this.ParseGmlPosElement(allowEmpty);
        this.ReadSkippableElements();
        this.ReadEndElement();
      }

      private void ParseGmlLineString()
      {
        if (!this.ReadStartOrEmptyElement("LineString"))
          return;
        this.ReadSkippableElements();
        if (this.IsPosListStart())
          this.ParsePosList(false);
        else
          this.ParseGmlPosListElement(true);
        this.ReadSkippableElements();
        this.ReadEndElement();
      }

      private void ParseGmlRingElement(string ringTag)
      {
        if (!this.ReadStartOrEmptyElement(ringTag))
          return;
        if (!this.IsEndElement(ringTag))
          this.ParseGmlLinearRingElement();
        this.ReadEndElement();
      }

      private void ParseGmlLinearRingElement()
      {
        if (!this.ReadStartOrEmptyElement("LinearRing"))
          return;
        if (this.IsEndElement("LinearRing"))
          throw new FormatException(Strings.GmlReader_EmptyRingsNotAllowed);
        if (this.IsPosListStart())
          this.ParsePosList(false);
        else
          this.ParseGmlPosListElement(false);
        this.ReadEndElement();
      }

      private void ParseMultiItemElement(
        string header,
        string member,
        string members,
        Action parseItem)
      {
        if (!this.ReadStartOrEmptyElement(header))
          return;
        this.ReadSkippableElements();
        if (!this.IsEndElement(header))
        {
          while (this.IsStartElement(member))
          {
            if (this.ReadStartOrEmptyElement(member) && !this.IsEndElement(member))
            {
              parseItem();
              this.ReadEndElement();
              this.ReadSkippableElements();
            }
          }
          if (this.IsStartElement(members) && this.ReadStartOrEmptyElement(members))
          {
            while (this.reader.IsStartElement())
              parseItem();
            this.ReadEndElement();
          }
        }
        this.ReadSkippableElements();
        this.ReadEndElement();
      }

      private void ParseGmlPosElement(bool allowEmpty)
      {
        this.ReadAttributes(false);
        if (this.ReadStartOrEmptyElement("pos"))
        {
          double[] numArray = this.ReadContentAsDoubleArray();
          if (numArray.Length != 0)
          {
            if (numArray.Length < 2)
              throw new FormatException(Strings.GmlReader_PosNeedTwoNumbers);
            this.AddPoint(numArray[0], numArray[1], numArray.Length > 2 ? new double?(numArray[2]) : new double?(), numArray.Length > 3 ? new double?(numArray[3]) : new double?());
          }
          else if (!allowEmpty)
            throw new FormatException(Strings.GmlReader_PosNeedTwoNumbers);
          this.ReadEndElement();
        }
        else if (!allowEmpty)
          throw new FormatException(Strings.GmlReader_PosNeedTwoNumbers);
      }

      private void ParsePosList(bool allowEmpty)
      {
        do
        {
          if (this.IsStartElement("pos"))
            this.ParseGmlPosElement(allowEmpty);
          else
            this.ParseGmlPointPropertyElement(allowEmpty);
        }
        while (this.IsPosListStart());
      }

      private void ParseGmlPointPropertyElement(bool allowEmpty)
      {
        if (!this.ReadStartOrEmptyElement("pointProperty"))
          return;
        this.ParseGmlPointElement(allowEmpty);
        this.ReadEndElement();
      }

      private void ParseGmlPosListElement(bool allowEmpty)
      {
        if (this.ReadStartOrEmptyElement("posList"))
        {
          if (!this.IsEndElement("posList"))
          {
            double[] numArray = this.ReadContentAsDoubleArray();
            if (numArray.Length == 0)
              throw new FormatException(Strings.GmlReader_PosListNeedsEvenCount);
            if (numArray.Length % 2 != 0)
              throw new FormatException(Strings.GmlReader_PosListNeedsEvenCount);
            for (int index = 0; index < numArray.Length; index += 2)
              this.AddPoint(numArray[index], numArray[index + 1], new double?(), new double?());
          }
          else if (!allowEmpty)
            throw new FormatException(Strings.GmlReader_PosListNeedsEvenCount);
          this.ReadEndElement();
        }
        else if (!allowEmpty)
          throw new FormatException(Strings.GmlReader_PosListNeedsEvenCount);
      }

      private double[] ReadContentAsDoubleArray()
      {
        string[] strArray = this.reader.ReadContentAsString().Split(GmlReader.Parser.coordinateDelimiter, StringSplitOptions.RemoveEmptyEntries);
        double[] numArray = new double[strArray.Length];
        for (int index = 0; index < strArray.Length; ++index)
          numArray[index] = XmlConvert.ToDouble(strArray[index]);
        return numArray;
      }

      private bool ReadStartOrEmptyElement(string element)
      {
        bool isEmptyElement = this.reader.IsEmptyElement;
        if (element != "FullGlobe")
          this.reader.ReadStartElement(element, this.gmlNamespace);
        else
          this.reader.ReadStartElement(element, "http://schemas.microsoft.com/sqlserver/2011/geography");
        return !isEmptyElement;
      }

      private bool IsStartElement(string element) => this.reader.IsStartElement(element, this.gmlNamespace);

      private bool IsEndElement(string element)
      {
        int content = (int) this.reader.MoveToContent();
        return this.reader.NodeType == XmlNodeType.EndElement && this.reader.LocalName.Equals(element, StringComparison.Ordinal);
      }

      private void ReadEndElement()
      {
        int content = (int) this.reader.MoveToContent();
        if (this.reader.NodeType != XmlNodeType.EndElement)
          throw new FormatException(Strings.GmlReader_UnexpectedElement((object) this.reader.Name));
        this.reader.ReadEndElement();
      }

      private void ReadSkippableElements()
      {
        bool flag = true;
        while (flag)
        {
          int content = (int) this.reader.MoveToContent();
          if (this.reader.NodeType == XmlNodeType.Element && (object) this.reader.NamespaceURI == (object) this.gmlNamespace)
          {
            string localName = this.reader.LocalName;
            flag = GmlReader.Parser.skippableElements.ContainsKey(localName);
          }
          else
            flag = false;
          if (flag)
            this.reader.Skip();
        }
      }

      private bool IsPosListStart() => this.IsStartElement("pos") || this.IsStartElement("pointProperty");

      private void PrepareFigure() => this.points = 0;

      private void AddPoint(double x, double y, double? z, double? m)
      {
        if (z.HasValue && double.IsNaN(z.Value))
          z = new double?();
        if (m.HasValue && double.IsNaN(m.Value))
          m = new double?();
        if (this.points == 0)
          this.pipeline.BeginFigure(x, y, z, m);
        else
          this.pipeline.LineTo(x, y, z, m);
        ++this.points;
      }

      private void EndFigure()
      {
        if (this.points <= 0)
          return;
        this.pipeline.EndFigure();
      }
    }
  }
}
