// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.MultiPoint
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Azure.Documents.Spatial
{
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
