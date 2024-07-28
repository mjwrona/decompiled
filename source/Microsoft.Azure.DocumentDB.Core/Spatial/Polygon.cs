// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.Polygon
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
  public sealed class Polygon : Geometry, IEquatable<Polygon>
  {
    public Polygon(IList<Position> externalRingPositions)
      : this((IList<LinearRing>) new LinearRing[1]
      {
        new LinearRing(externalRingPositions)
      }, new GeometryParams())
    {
    }

    public Polygon(IList<LinearRing> rings)
      : this(rings, new GeometryParams())
    {
    }

    public Polygon(IList<LinearRing> rings, GeometryParams geometryParams)
      : base(GeometryType.Polygon, geometryParams)
    {
      this.Rings = rings != null ? new ReadOnlyCollection<LinearRing>(rings) : throw new ArgumentNullException(nameof (rings));
    }

    internal Polygon()
      : base(GeometryType.Polygon, new GeometryParams())
    {
    }

    [JsonProperty("coordinates", Required = Required.Always, Order = 1)]
    public ReadOnlyCollection<LinearRing> Rings { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as Polygon);

    public override int GetHashCode() => this.Rings.Aggregate<LinearRing, int>(base.GetHashCode(), (Func<int, LinearRing, int>) ((current, value) => current * 397 ^ value.GetHashCode()));

    public bool Equals(Polygon other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return base.Equals((object) other) && this.Rings.SequenceEqual<LinearRing>((IEnumerable<LinearRing>) other.Rings);
    }
  }
}
