// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.Converters.LineStringCoordinatesJsonConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Spatial.Converters
{
  internal sealed class LineStringCoordinatesJsonConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      LineStringCoordinates stringCoordinates = (LineStringCoordinates) value;
      serializer.Serialize(writer, (object) stringCoordinates.Positions);
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return (object) new LineStringCoordinates((IList<Position>) serializer.Deserialize<Position[]>(reader));
    }

    public override bool CanConvert(Type objectType) => typeof (IEnumerable<LinearRing>).IsAssignableFrom(objectType);
  }
}
