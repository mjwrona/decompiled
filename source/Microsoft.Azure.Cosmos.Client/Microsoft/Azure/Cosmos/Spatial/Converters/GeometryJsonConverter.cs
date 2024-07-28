// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.Converters.GeometryJsonConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Azure.Cosmos.Spatial.Converters
{
  internal sealed class GeometryJsonConverter : JsonConverter
  {
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JToken jtoken1 = JToken.Load(reader);
      if (jtoken1.Type == JTokenType.Null)
        return (object) null;
      JToken jtoken2 = jtoken1.Type == JTokenType.Object ? jtoken1[(object) "type"] : throw new JsonSerializationException(RMResources.SpatialInvalidGeometryType);
      if (jtoken2 == null || jtoken2.Type != JTokenType.String)
        throw new JsonSerializationException(RMResources.SpatialInvalidGeometryType);
      Geometry target;
      switch (jtoken2.Value<string>())
      {
        case "GeometryCollection":
          target = (Geometry) new GeometryCollection();
          break;
        case "LineString":
          target = (Geometry) new LineString();
          break;
        case "MultiLineString":
          target = (Geometry) new MultiLineString();
          break;
        case "MultiPoint":
          target = (Geometry) new MultiPoint();
          break;
        case "MultiPolygon":
          target = (Geometry) new MultiPolygon();
          break;
        case "Point":
          target = (Geometry) new Point();
          break;
        case "Polygon":
          target = (Geometry) new Polygon();
          break;
        default:
          throw new JsonSerializationException(RMResources.SpatialInvalidGeometryType);
      }
      serializer.Populate(jtoken1.CreateReader(), (object) target);
      return (object) target;
    }

    public override bool CanConvert(Type objectType) => typeof (Geometry) == objectType;
  }
}
