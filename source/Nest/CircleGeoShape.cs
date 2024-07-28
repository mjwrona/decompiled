// Decompiled with JetBrains decompiler
// Type: Nest.CircleGeoShape
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  [JsonFormatter(typeof (GeoShapeFormatter<CircleGeoShape>))]
  public class CircleGeoShape : GeoShapeBase, ICircleGeoShape, IGeoShape
  {
    internal CircleGeoShape()
      : base("circle")
    {
    }

    public CircleGeoShape(GeoCoordinate coordinates, string radius)
      : this()
    {
      this.Coordinates = coordinates ?? throw new ArgumentNullException(nameof (coordinates));
      switch (radius)
      {
        case null:
          throw new ArgumentNullException(nameof (radius));
        case "":
          throw new ArgumentException("cannot be empty", nameof (radius));
        default:
          this.Radius = radius;
          break;
      }
    }

    public GeoCoordinate Coordinates { get; set; }

    public string Radius { get; set; }
  }
}
