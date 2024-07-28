// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.Converters.PolygonCoordinatesJsonConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Spatial.Converters
{
  internal sealed class PolygonCoordinatesJsonConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      PolygonCoordinates polygonCoordinates = (PolygonCoordinates) value;
      writer.WriteStartArray();
      foreach (LinearRing ring in polygonCoordinates.Rings)
        serializer.Serialize(writer, (object) ring.Positions);
      writer.WriteEndArray();
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return (object) new PolygonCoordinates((IList<LinearRing>) ((IEnumerable<Position[]>) serializer.Deserialize<Position[][]>(reader)).Select<Position[], LinearRing>((Func<Position[], LinearRing>) (c => new LinearRing((IList<Position>) c))).ToList<LinearRing>());
    }

    public override bool CanConvert(Type objectType) => typeof (IEnumerable<LinearRing>).IsAssignableFrom(objectType);
  }
}
