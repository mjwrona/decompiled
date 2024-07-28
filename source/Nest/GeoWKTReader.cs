// Decompiled with JetBrains decompiler
// Type: Nest.GeoWKTReader
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Nest
{
  public class GeoWKTReader
  {
    public static IGeoShape Read(string wellKnownText)
    {
      using (WellKnownTextTokenizer tokenizer = new WellKnownTextTokenizer((TextReader) new StringReader(wellKnownText)))
        return GeoWKTReader.Read(tokenizer, (string) null);
    }

    private static IGeoShape Read(WellKnownTextTokenizer tokenizer, string shapeType)
    {
      string str = tokenizer.NextToken() == TokenType.Word ? tokenizer.TokenValue.ToUpperInvariant() : throw new GeoWKTException("Expected word but found " + tokenizer.TokenString(), tokenizer.LineNumber, tokenizer.Position);
      if (shapeType != null && shapeType != "GEOMETRYCOLLECTION" && str != shapeType)
        throw new GeoWKTException("Expected geometry type " + shapeType + " but found " + str);
      switch (str)
      {
        case "BBOX":
          EnvelopeGeoShape boundingBox = GeoWKTReader.ParseBoundingBox(tokenizer);
          boundingBox.Format = GeoFormat.WellKnownText;
          return (IGeoShape) boundingBox;
        case "GEOMETRYCOLLECTION":
          GeometryCollection geometryCollection = GeoWKTReader.ParseGeometryCollection(tokenizer);
          geometryCollection.Format = GeoFormat.WellKnownText;
          return (IGeoShape) geometryCollection;
        case "LINESTRING":
          LineStringGeoShape lineString = GeoWKTReader.ParseLineString(tokenizer);
          lineString.Format = GeoFormat.WellKnownText;
          return (IGeoShape) lineString;
        case "MULTILINESTRING":
          MultiLineStringGeoShape multiLineString = GeoWKTReader.ParseMultiLineString(tokenizer);
          multiLineString.Format = GeoFormat.WellKnownText;
          return (IGeoShape) multiLineString;
        case "MULTIPOINT":
          MultiPointGeoShape multiPoint = GeoWKTReader.ParseMultiPoint(tokenizer);
          multiPoint.Format = GeoFormat.WellKnownText;
          return (IGeoShape) multiPoint;
        case "MULTIPOLYGON":
          MultiPolygonGeoShape multiPolygon = GeoWKTReader.ParseMultiPolygon(tokenizer);
          multiPolygon.Format = GeoFormat.WellKnownText;
          return (IGeoShape) multiPolygon;
        case "POINT":
          PointGeoShape point = GeoWKTReader.ParsePoint(tokenizer);
          point.Format = GeoFormat.WellKnownText;
          return (IGeoShape) point;
        case "POLYGON":
          PolygonGeoShape polygon = GeoWKTReader.ParsePolygon(tokenizer);
          polygon.Format = GeoFormat.WellKnownText;
          return (IGeoShape) polygon;
        default:
          throw new GeoWKTException("Unknown geometry type: " + str);
      }
    }

    private static PointGeoShape ParsePoint(WellKnownTextTokenizer tokenizer)
    {
      if (GeoWKTReader.NextEmptyOrOpen(tokenizer) == TokenType.Word)
        return (PointGeoShape) null;
      PointGeoShape point = new PointGeoShape(GeoWKTReader.ParseCoordinate(tokenizer));
      GeoWKTReader.NextCloser(tokenizer);
      return point;
    }

    private static MultiPointGeoShape ParseMultiPoint(WellKnownTextTokenizer tokenizer) => GeoWKTReader.NextEmptyOrOpen(tokenizer) == TokenType.Word ? (MultiPointGeoShape) null : new MultiPointGeoShape((IEnumerable<GeoCoordinate>) GeoWKTReader.ParseCoordinates(tokenizer));

    private static LineStringGeoShape ParseLineString(WellKnownTextTokenizer tokenizer) => GeoWKTReader.NextEmptyOrOpen(tokenizer) == TokenType.Word ? (LineStringGeoShape) null : new LineStringGeoShape((IEnumerable<GeoCoordinate>) GeoWKTReader.ParseCoordinates(tokenizer));

    private static MultiLineStringGeoShape ParseMultiLineString(WellKnownTextTokenizer tokenizer) => GeoWKTReader.NextEmptyOrOpen(tokenizer) == TokenType.Word ? (MultiLineStringGeoShape) null : new MultiLineStringGeoShape((IEnumerable<IEnumerable<GeoCoordinate>>) GeoWKTReader.ParseCoordinateLists(tokenizer));

    private static PolygonGeoShape ParsePolygon(WellKnownTextTokenizer tokenizer) => GeoWKTReader.NextEmptyOrOpen(tokenizer) == TokenType.Word ? (PolygonGeoShape) null : new PolygonGeoShape((IEnumerable<IEnumerable<GeoCoordinate>>) GeoWKTReader.ParseCoordinateLists(tokenizer));

    private static MultiPolygonGeoShape ParseMultiPolygon(WellKnownTextTokenizer tokenizer)
    {
      if (GeoWKTReader.NextEmptyOrOpen(tokenizer) == TokenType.Word)
        return (MultiPolygonGeoShape) null;
      List<IEnumerable<IEnumerable<GeoCoordinate>>> coordinates = new List<IEnumerable<IEnumerable<GeoCoordinate>>>()
      {
        (IEnumerable<IEnumerable<GeoCoordinate>>) GeoWKTReader.ParseCoordinateLists(tokenizer)
      };
      while (GeoWKTReader.NextCloserOrComma(tokenizer) == TokenType.Comma)
        coordinates.Add((IEnumerable<IEnumerable<GeoCoordinate>>) GeoWKTReader.ParseCoordinateLists(tokenizer));
      return new MultiPolygonGeoShape((IEnumerable<IEnumerable<IEnumerable<GeoCoordinate>>>) coordinates);
    }

    private static EnvelopeGeoShape ParseBoundingBox(WellKnownTextTokenizer tokenizer)
    {
      if (GeoWKTReader.NextEmptyOrOpen(tokenizer) == TokenType.Word)
        return (EnvelopeGeoShape) null;
      double longitude1 = GeoWKTReader.NextNumber(tokenizer);
      GeoWKTReader.NextComma(tokenizer);
      double longitude2 = GeoWKTReader.NextNumber(tokenizer);
      GeoWKTReader.NextComma(tokenizer);
      double latitude1 = GeoWKTReader.NextNumber(tokenizer);
      GeoWKTReader.NextComma(tokenizer);
      double latitude2 = GeoWKTReader.NextNumber(tokenizer);
      GeoWKTReader.NextCloser(tokenizer);
      return new EnvelopeGeoShape((IEnumerable<GeoCoordinate>) new GeoCoordinate[2]
      {
        new GeoCoordinate(latitude1, longitude1),
        new GeoCoordinate(latitude2, longitude2)
      });
    }

    private static GeometryCollection ParseGeometryCollection(WellKnownTextTokenizer tokenizer)
    {
      if (GeoWKTReader.NextEmptyOrOpen(tokenizer) == TokenType.Word)
        return (GeometryCollection) null;
      List<IGeoShape> geoShapeList = new List<IGeoShape>()
      {
        GeoWKTReader.Read(tokenizer, "GEOMETRYCOLLECTION")
      };
      while (GeoWKTReader.NextCloserOrComma(tokenizer) == TokenType.Comma)
        geoShapeList.Add(GeoWKTReader.Read(tokenizer, (string) null));
      return new GeometryCollection()
      {
        Geometries = (IEnumerable<IGeoShape>) geoShapeList
      };
    }

    private static List<IEnumerable<GeoCoordinate>> ParseCoordinateLists(
      WellKnownTextTokenizer tokenizer)
    {
      List<IEnumerable<GeoCoordinate>> coordinateLists = new List<IEnumerable<GeoCoordinate>>();
      int num1 = (int) GeoWKTReader.NextEmptyOrOpen(tokenizer);
      coordinateLists.Add((IEnumerable<GeoCoordinate>) GeoWKTReader.ParseCoordinates(tokenizer));
      while (GeoWKTReader.NextCloserOrComma(tokenizer) == TokenType.Comma)
      {
        int num2 = (int) GeoWKTReader.NextEmptyOrOpen(tokenizer);
        coordinateLists.Add((IEnumerable<GeoCoordinate>) GeoWKTReader.ParseCoordinates(tokenizer));
      }
      return coordinateLists;
    }

    private static List<GeoCoordinate> ParseCoordinates(WellKnownTextTokenizer tokenizer)
    {
      List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
      if (GeoWKTReader.IsNumberNext(tokenizer) || tokenizer.NextToken() == TokenType.LParen)
        coordinates.Add(GeoWKTReader.ParseCoordinate(tokenizer));
      while (GeoWKTReader.NextCloserOrComma(tokenizer) == TokenType.Comma)
      {
        bool flag = false;
        if (GeoWKTReader.IsNumberNext(tokenizer) || (flag = tokenizer.NextToken() == TokenType.LParen))
          coordinates.Add(GeoWKTReader.ParseCoordinate(tokenizer));
        if (flag)
          GeoWKTReader.NextCloser(tokenizer);
      }
      return coordinates;
    }

    private static GeoCoordinate ParseCoordinate(WellKnownTextTokenizer tokenizer)
    {
      double longitude = GeoWKTReader.NextNumber(tokenizer);
      double latitude = GeoWKTReader.NextNumber(tokenizer);
      double? nullable = new double?();
      if (GeoWKTReader.IsNumberNext(tokenizer))
        nullable = new double?(GeoWKTReader.NextNumber(tokenizer));
      return nullable.HasValue ? new GeoCoordinate(latitude, longitude, nullable.Value) : new GeoCoordinate(latitude, longitude);
    }

    internal static void NextCloser(WellKnownTextTokenizer tokenizer)
    {
      if (tokenizer.NextToken() != TokenType.RParen)
        throw new GeoWKTException(string.Format("Expected {0} ", (object) ')') + "but found: " + tokenizer.TokenString(), tokenizer.LineNumber, tokenizer.Position);
    }

    private static void NextComma(WellKnownTextTokenizer tokenizer)
    {
      if (tokenizer.NextToken() != TokenType.Comma)
        throw new GeoWKTException(string.Format("Expected {0} but found: {1}", (object) ',', (object) tokenizer.TokenString()), tokenizer.LineNumber, tokenizer.Position);
    }

    internal static TokenType NextEmptyOrOpen(WellKnownTextTokenizer tokenizer)
    {
      TokenType tokenType = tokenizer.NextToken();
      switch (tokenType)
      {
        case TokenType.Word:
          if (!tokenizer.TokenValue.Equals("EMPTY", StringComparison.OrdinalIgnoreCase))
            break;
          goto case TokenType.LParen;
        case TokenType.LParen:
          return tokenType;
      }
      throw new GeoWKTException(string.Format("Expected {0} or {1} ", (object) "EMPTY", (object) '(') + "but found: " + tokenizer.TokenString(), tokenizer.LineNumber, tokenizer.Position);
    }

    private static TokenType NextCloserOrComma(WellKnownTextTokenizer tokenizer)
    {
      TokenType tokenType = tokenizer.NextToken();
      switch (tokenType)
      {
        case TokenType.RParen:
        case TokenType.Comma:
          return tokenType;
        default:
          throw new GeoWKTException(string.Format("Expected {0} or {1} ", (object) ',', (object) ')') + "but found: " + tokenizer.TokenString(), tokenizer.LineNumber, tokenizer.Position);
      }
    }

    internal static double NextNumber(WellKnownTextTokenizer tokenizer)
    {
      if (tokenizer.NextToken() == TokenType.Word)
      {
        if (string.Equals(tokenizer.TokenValue, "NAN", StringComparison.OrdinalIgnoreCase))
          return double.NaN;
        double result;
        if (double.TryParse(tokenizer.TokenValue, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          return result;
      }
      throw new GeoWKTException("Expected number but found: " + tokenizer.TokenString(), tokenizer.LineNumber, tokenizer.Position);
    }

    internal static bool IsNumberNext(WellKnownTextTokenizer tokenizer) => tokenizer.PeekToken() == TokenType.Word;
  }
}
