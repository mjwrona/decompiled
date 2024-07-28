// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.MultiPoint
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Spatial
{
  [DataContract]
  internal sealed class MultiPoint : Geometry, IEquatable<MultiPoint>
  {
    public MultiPoint(IList<Position> points)
      : this(points, new GeometryParams())
    {
    }

    public MultiPoint(IList<Position> points, GeometryParams geometryParams)
      : base(GeometryType.MultiPoint, geometryParams)
    {
      this.Points = points != null ? new ReadOnlyCollection<Position>(points) : throw new ArgumentNullException(nameof (points));
    }

    internal MultiPoint()
      : base(GeometryType.MultiPoint, new GeometryParams())
    {
    }

    [DataMember(Name = "coordinates")]
    [JsonProperty("coordinates", Required = Required.Always, Order = 1)]
    public ReadOnlyCollection<Position> Points { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as MultiPoint);

    public override int GetHashCode() => this.Points.Aggregate<Position, int>(base.GetHashCode(), (Func<int, Position, int>) ((current, point) => current * 397 ^ point.GetHashCode()));

    public bool Equals(MultiPoint other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return base.Equals((object) other) && this.Points.SequenceEqual<Position>((IEnumerable<Position>) other.Points);
    }
  }
}
