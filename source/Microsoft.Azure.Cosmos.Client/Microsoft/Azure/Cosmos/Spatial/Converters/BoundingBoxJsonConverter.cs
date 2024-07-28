// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.Converters.BoundingBoxJsonConverter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Spatial.Converters
{
  internal sealed class BoundingBoxJsonConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      BoundingBox boundingBox = (BoundingBox) value;
      serializer.Serialize(writer, (object) boundingBox.Min.Coordinates.Concat<double>((IEnumerable<double>) boundingBox.Max.Coordinates));
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      double[] source = serializer.Deserialize<double[]>(reader);
      if (source == null)
        return (object) null;
      return source.Length % 2 == 0 && source.Length >= 4 ? (object) new BoundingBox(new Position((IList<double>) ((IEnumerable<double>) source).Take<double>(source.Length / 2).ToList<double>()), new Position((IList<double>) ((IEnumerable<double>) source).Skip<double>(source.Length / 2).ToList<double>())) : throw new JsonSerializationException(RMResources.SpatialBoundingBoxInvalidCoordinates);
    }

    public override bool CanConvert(Type objectType) => objectType == typeof (BoundingBox);
  }
}
