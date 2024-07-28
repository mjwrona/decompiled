// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeoJsonObjectReader
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Spatial
{
  internal class GeoJsonObjectReader : SpatialReader<IDictionary<string, object>>
  {
    internal GeoJsonObjectReader(SpatialPipeline destination)
      : base(destination)
    {
    }

    protected override void ReadGeographyImplementation(IDictionary<string, object> input) => new GeoJsonObjectReader.SendToTypeWashedPipeline((TypeWashedPipeline) new TypeWashedToGeographyLongLatPipeline(this.Destination)).SendToPipeline(input, true);

    protected override void ReadGeometryImplementation(IDictionary<string, object> input) => new GeoJsonObjectReader.SendToTypeWashedPipeline((TypeWashedPipeline) new TypeWashedToGeometryPipeline(this.Destination)).SendToPipeline(input, true);

    private class SendToTypeWashedPipeline
    {
      private TypeWashedPipeline pipeline;

      internal SendToTypeWashedPipeline(TypeWashedPipeline pipeline) => this.pipeline = pipeline;

      internal void SendToPipeline(IDictionary<string, object> members, bool requireSetCoordinates)
      {
        SpatialType spatialType = GeoJsonObjectReader.SendToTypeWashedPipeline.GetSpatialType(members);
        int? epsgId;
        if (!GeoJsonObjectReader.SendToTypeWashedPipeline.TryGetCoordinateSystemId(members, out epsgId))
          epsgId = new int?();
        if (requireSetCoordinates || epsgId.HasValue)
          this.pipeline.SetCoordinateSystem(epsgId);
        string memberName = spatialType != SpatialType.Collection ? "coordinates" : "geometries";
        IEnumerable valueAsJsonArray = GeoJsonObjectReader.SendToTypeWashedPipeline.GetMemberValueAsJsonArray(members, memberName);
        this.SendShape(spatialType, valueAsJsonArray);
      }

      private static void SendArrayOfArray(IEnumerable array, Action<IEnumerable> send)
      {
        foreach (object obj in array)
        {
          IEnumerable enumerable = GeoJsonObjectReader.SendToTypeWashedPipeline.ValueAsJsonArray(obj);
          send(enumerable);
        }
      }

      private static double? ValueAsNullableDouble(object value) => value != null ? new double?(GeoJsonObjectReader.SendToTypeWashedPipeline.ValueAsDouble(value)) : new double?();

      private static double ValueAsDouble(object value)
      {
        switch (value)
        {
          case null:
            throw new FormatException(Strings.GeoJsonReader_InvalidNullElement);
          case string _:
          case IDictionary<string, object> _:
          case IEnumerable _:
          case bool _:
            throw new FormatException(Strings.GeoJsonReader_ExpectedNumeric);
          default:
            return Convert.ToDouble(value, (IFormatProvider) CultureInfo.InvariantCulture);
        }
      }

      private static IEnumerable ValueAsJsonArray(object value)
      {
        switch (value)
        {
          case null:
            return (IEnumerable) null;
          case string _:
            throw new FormatException(Strings.GeoJsonReader_ExpectedArray);
          case IDictionary _:
          case IDictionary<string, object> _:
            throw new FormatException(Strings.GeoJsonReader_ExpectedArray);
          case IEnumerable enumerable:
            return enumerable;
          default:
            throw new FormatException(Strings.GeoJsonReader_ExpectedArray);
        }
      }

      private static IDictionary<string, object> ValueAsJsonObject(object value)
      {
        if (value == null)
          return (IDictionary<string, object>) null;
        return value is IDictionary<string, object> dictionary ? dictionary : throw new FormatException(Strings.JsonReaderExtensions_CannotReadValueAsJsonObject(value));
      }

      private static string ValueAsString(string propertyName, object value)
      {
        if (value == null)
          return (string) null;
        return value is string str ? str : throw new FormatException(Strings.JsonReaderExtensions_CannotReadPropertyValueAsString(value, (object) propertyName));
      }

      private static SpatialType GetSpatialType(IDictionary<string, object> geoJsonObject)
      {
        object obj;
        if (geoJsonObject.TryGetValue("type", out obj))
          return GeoJsonObjectReader.SendToTypeWashedPipeline.ReadTypeName(GeoJsonObjectReader.SendToTypeWashedPipeline.ValueAsString("type", obj));
        throw new FormatException(Strings.GeoJsonReader_MissingRequiredMember((object) "type"));
      }

      private static bool TryGetCoordinateSystemId(
        IDictionary<string, object> geoJsonObject,
        out int? epsgId)
      {
        object obj;
        if (!geoJsonObject.TryGetValue("crs", out obj))
        {
          epsgId = new int?();
          return false;
        }
        IDictionary<string, object> crsJsonObject = GeoJsonObjectReader.SendToTypeWashedPipeline.ValueAsJsonObject(obj);
        epsgId = new int?(GeoJsonObjectReader.SendToTypeWashedPipeline.GetCoordinateSystemIdFromCrs(crsJsonObject));
        return true;
      }

      private static int GetCoordinateSystemIdFromCrs(IDictionary<string, object> crsJsonObject)
      {
        object obj1;
        if (!crsJsonObject.TryGetValue("type", out obj1))
          throw new FormatException(Strings.GeoJsonReader_MissingRequiredMember((object) "type"));
        string str = GeoJsonObjectReader.SendToTypeWashedPipeline.ValueAsString("type", obj1);
        if (!string.Equals(str, "name", StringComparison.Ordinal))
          throw new FormatException(Strings.GeoJsonReader_InvalidCrsType((object) str));
        object obj2;
        if (!crsJsonObject.TryGetValue("properties", out obj2))
          throw new FormatException(Strings.GeoJsonReader_MissingRequiredMember((object) "properties"));
        object obj3;
        if (!GeoJsonObjectReader.SendToTypeWashedPipeline.ValueAsJsonObject(obj2).TryGetValue("name", out obj3))
          throw new FormatException(Strings.GeoJsonReader_MissingRequiredMember((object) "name"));
        string p0 = GeoJsonObjectReader.SendToTypeWashedPipeline.ValueAsString("name", obj3);
        int length = "EPSG".Length;
        int result;
        if (p0 == null || !p0.StartsWith("EPSG", StringComparison.Ordinal) || p0.Length == length || p0[length] != ':' || !int.TryParse(p0.Substring(length + 1), out result))
          throw new FormatException(Strings.GeoJsonReader_InvalidCrsName((object) p0));
        return result;
      }

      private static IEnumerable GetMemberValueAsJsonArray(
        IDictionary<string, object> geoJsonObject,
        string memberName)
      {
        object obj;
        if (geoJsonObject.TryGetValue(memberName, out obj))
          return GeoJsonObjectReader.SendToTypeWashedPipeline.ValueAsJsonArray(obj);
        throw new FormatException(Strings.GeoJsonReader_MissingRequiredMember((object) memberName));
      }

      private static bool EnumerableAny(IEnumerable enumerable) => enumerable.GetEnumerator().MoveNext();

      private static SpatialType ReadTypeName(string typeName)
      {
        switch (typeName)
        {
          case "GeometryCollection":
            return SpatialType.Collection;
          case "LineString":
            return SpatialType.LineString;
          case "MultiLineString":
            return SpatialType.MultiLineString;
          case "MultiPoint":
            return SpatialType.MultiPoint;
          case "MultiPolygon":
            return SpatialType.MultiPolygon;
          case "Point":
            return SpatialType.Point;
          case "Polygon":
            return SpatialType.Polygon;
          default:
            throw new FormatException(Strings.GeoJsonReader_InvalidTypeName((object) typeName));
        }
      }

      private void SendShape(SpatialType spatialType, IEnumerable contentMembers)
      {
        this.pipeline.BeginGeo(spatialType);
        this.SendCoordinates(spatialType, contentMembers);
        this.pipeline.EndGeo();
      }

      private void SendCoordinates(SpatialType spatialType, IEnumerable contentMembers)
      {
        if (!GeoJsonObjectReader.SendToTypeWashedPipeline.EnumerableAny(contentMembers))
          return;
        switch (spatialType)
        {
          case SpatialType.Point:
            this.SendPoint(contentMembers);
            break;
          case SpatialType.LineString:
            this.SendLineString(contentMembers);
            break;
          case SpatialType.Polygon:
            this.SendPolygon(contentMembers);
            break;
          case SpatialType.MultiPoint:
            this.SendMultiShape(SpatialType.Point, contentMembers);
            break;
          case SpatialType.MultiLineString:
            this.SendMultiShape(SpatialType.LineString, contentMembers);
            break;
          case SpatialType.MultiPolygon:
            this.SendMultiShape(SpatialType.Polygon, contentMembers);
            break;
          case SpatialType.Collection:
            IEnumerator enumerator = contentMembers.GetEnumerator();
            try
            {
              while (enumerator.MoveNext())
                this.SendToPipeline((IDictionary<string, object>) enumerator.Current, false);
              break;
            }
            finally
            {
              if (enumerator is IDisposable disposable)
                disposable.Dispose();
            }
        }
      }

      private void SendPoint(IEnumerable coordinates)
      {
        this.SendPosition(coordinates, true);
        this.pipeline.EndFigure();
      }

      private void SendLineString(IEnumerable coordinates) => this.SendPositionArray(coordinates);

      private void SendPolygon(IEnumerable coordinates) => GeoJsonObjectReader.SendToTypeWashedPipeline.SendArrayOfArray(coordinates, (Action<IEnumerable>) (positionArray => this.SendPositionArray(positionArray)));

      private void SendMultiShape(SpatialType containedSpatialType, IEnumerable coordinates) => GeoJsonObjectReader.SendToTypeWashedPipeline.SendArrayOfArray(coordinates, (Action<IEnumerable>) (containedShapeCoordinates => this.SendShape(containedSpatialType, containedShapeCoordinates)));

      private void SendPositionArray(IEnumerable positionArray)
      {
        bool first = true;
        GeoJsonObjectReader.SendToTypeWashedPipeline.SendArrayOfArray(positionArray, (Action<IEnumerable>) (array =>
        {
          this.SendPosition(array, first);
          if (!first)
            return;
          first = false;
        }));
        this.pipeline.EndFigure();
      }

      private void SendPosition(IEnumerable positionElements, bool first)
      {
        int num = 0;
        double coordinate1 = 0.0;
        double coordinate2 = 0.0;
        double? coordinate3 = new double?();
        double? coordinate4 = new double?();
        foreach (object positionElement in positionElements)
        {
          ++num;
          switch (num)
          {
            case 1:
              coordinate1 = GeoJsonObjectReader.SendToTypeWashedPipeline.ValueAsDouble(positionElement);
              continue;
            case 2:
              coordinate2 = GeoJsonObjectReader.SendToTypeWashedPipeline.ValueAsDouble(positionElement);
              continue;
            case 3:
              coordinate3 = GeoJsonObjectReader.SendToTypeWashedPipeline.ValueAsNullableDouble(positionElement);
              continue;
            case 4:
              coordinate4 = GeoJsonObjectReader.SendToTypeWashedPipeline.ValueAsNullableDouble(positionElement);
              continue;
            default:
              continue;
          }
        }
        if (num < 2 || num > 4)
          throw new FormatException(Strings.GeoJsonReader_InvalidPosition);
        if (first)
          this.pipeline.BeginFigure(coordinate1, coordinate2, coordinate3, coordinate4);
        else
          this.pipeline.LineTo(coordinate1, coordinate2, coordinate3, coordinate4);
      }
    }
  }
}
