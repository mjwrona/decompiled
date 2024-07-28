// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.MultiPolygon
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
  internal sealed class MultiPolygon : Geometry, IEquatable<MultiPolygon>
  {
    public MultiPolygon(IList<PolygonCoordinates> polygons)
      : this(polygons, new GeometryParams())
    {
    }

    public MultiPolygon(IList<PolygonCoordinates> polygons, GeometryParams geometryParams)
      : base(GeometryType.MultiPolygon, geometryParams)
    {
      this.Polygons = polygons != null ? new ReadOnlyCollection<PolygonCoordinates>(polygons) : throw new ArgumentNullException(nameof (polygons));
    }

    internal MultiPolygon()
      : base(GeometryType.MultiPolygon, new GeometryParams())
    {
    }

    [JsonProperty("coordinates", Required = Required.Always, Order = 1)]
    public ReadOnlyCollection<PolygonCoordinates> Polygons { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as MultiPolygon);

    public override int GetHashCode() => this.Polygons.Aggregate<PolygonCoordinates, int>(base.GetHashCode(), (Func<int, PolygonCoordinates, int>) ((current, value) => current * 397 ^ value.GetHashCode()));

    public bool Equals(MultiPolygon other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return base.Equals((object) other) && this.Polygons.SequenceEqual<PolygonCoordinates>((IEnumerable<PolygonCoordinates>) other.Polygons);
    }
  }
}
