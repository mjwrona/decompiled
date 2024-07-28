// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.Point
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Documents.Spatial
{
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
