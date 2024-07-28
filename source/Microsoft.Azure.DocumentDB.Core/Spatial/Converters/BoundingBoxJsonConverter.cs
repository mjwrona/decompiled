// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.Converters.BoundingBoxJsonConverter
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Documents.Spatial.Converters
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

    public override bool CanConvert(Type objectType) => (object) objectType == (object) typeof (BoundingBox);
  }
}
