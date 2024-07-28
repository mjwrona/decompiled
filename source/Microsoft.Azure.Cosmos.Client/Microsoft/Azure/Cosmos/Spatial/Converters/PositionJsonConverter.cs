// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.Converters.PositionJsonConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Spatial.Converters
{
  internal sealed class PositionJsonConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      Position position = (Position) value;
      serializer.Serialize(writer, (object) position.Coordinates);
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      double[] coordinates = serializer.Deserialize<double[]>(reader);
      return coordinates != null && coordinates.Length >= 2 ? (object) new Position((IList<double>) coordinates) : throw new JsonSerializationException(RMResources.SpatialInvalidPosition);
    }

    public override bool CanConvert(Type objectType) => objectType == typeof (Position);
  }
}
