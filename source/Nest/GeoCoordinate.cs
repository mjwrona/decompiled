// Decompiled with JetBrains decompiler
// Type: Nest.GeoCoordinate
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  [JsonFormatter(typeof (GeoCoordinateFormatter))]
  public class GeoCoordinate : GeoLocation
  {
    public GeoCoordinate(double latitude, double longitude)
      : base(latitude, longitude)
    {
    }

    public GeoCoordinate(double latitude, double longitude, double z)
      : base(latitude, longitude)
    {
      this.Z = new double?(z);
    }

    public double? Z { get; set; }

    public static implicit operator GeoCoordinate(double[] coordinates)
    {
      if (coordinates == null)
        return (GeoCoordinate) null;
      switch (coordinates.Length)
      {
        case 2:
          return new GeoCoordinate(coordinates[0], coordinates[1]);
        case 3:
          return new GeoCoordinate(coordinates[0], coordinates[1], coordinates[2]);
        default:
          throw new ArgumentOutOfRangeException(nameof (coordinates), "Cannot create a GeoCoordinate from an array that does not contain 2 or 3 values");
      }
    }
  }
}
