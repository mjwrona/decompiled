// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.Polygon
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

    [DataMember(Name = "coordinates")]
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
