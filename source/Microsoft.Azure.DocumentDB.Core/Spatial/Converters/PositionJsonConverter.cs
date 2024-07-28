// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.Converters.PositionJsonConverter
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Spatial.Converters
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

    public override bool CanConvert(Type objectType) => (object) objectType == (object) typeof (Position);
  }
}
