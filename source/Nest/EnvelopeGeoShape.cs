// Decompiled with JetBrains decompiler
// Type: Nest.EnvelopeGeoShape
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  [JsonFormatter(typeof (GeoShapeFormatter<EnvelopeGeoShape>))]
  public class EnvelopeGeoShape : GeoShapeBase, IEnvelopeGeoShape, IGeoShape
  {
    internal EnvelopeGeoShape()
      : base("envelope")
    {
    }

    public EnvelopeGeoShape(IEnumerable<GeoCoordinate> coordinates)
      : this()
    {
      this.Coordinates = coordinates ?? throw new ArgumentNullException(nameof (coordinates));
    }

    public IEnumerable<GeoCoordinate> Coordinates { get; set; }
  }
}
