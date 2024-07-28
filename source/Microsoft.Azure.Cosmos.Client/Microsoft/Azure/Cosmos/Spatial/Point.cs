// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.Point
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Spatial
{
  [DataContract]
  public sealed class Point : Geometry, IEquatable<Point>
  {
    public Point(double longitude, double latitude)
      : this(new Position(longitude, latitude), new GeometryParams())
    {
    }

    public Point(Position position)
      : this(position, new GeometryParams())
    {
    }

    public Point(Position position, GeometryParams geometryParams)
      : base(GeometryType.Point, geometryParams)
    {
      this.Position = position != null ? position : throw new ArgumentNullException(nameof (position));
    }

    internal Point()
      : base(GeometryType.Point, new GeometryParams())
    {
    }

    [DataMember(Name = "coordinates")]
    [JsonProperty("coordinates", Required = Required.Always, Order = 1)]
    public Position Position { get; private set; }

    public bool Equals(Point other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return base.Equals((object) other) && this.Position.Equals(other.Position);
    }

    public override bool Equals(object obj) => this.Equals(obj as Point);

    public override int GetHashCode() => base.GetHashCode() * 397 ^ this.Position.GetHashCode();
  }
}
