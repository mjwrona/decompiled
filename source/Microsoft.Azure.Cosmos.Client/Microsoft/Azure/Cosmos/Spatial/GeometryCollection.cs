// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.GeometryCollection
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
  internal sealed class GeometryCollection : Geometry, IEquatable<GeometryCollection>
  {
    public GeometryCollection(IList<Geometry> geometries)
      : this(geometries, new GeometryParams())
    {
    }

    public GeometryCollection(IList<Geometry> geometries, GeometryParams geometryParams)
      : base(GeometryType.GeometryCollection, geometryParams)
    {
      this.Geometries = geometries != null ? new ReadOnlyCollection<Geometry>(geometries) : throw new ArgumentNullException(nameof (geometries));
    }

    internal GeometryCollection()
      : base(GeometryType.GeometryCollection, new GeometryParams())
    {
    }

    [DataMember(Name = "geometries")]
    [JsonProperty("geometries", Required = Required.Always, Order = 1)]
    public ReadOnlyCollection<Geometry> Geometries { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as GeometryCollection);

    public override int GetHashCode() => this.Geometries.Aggregate<Geometry, int>(base.GetHashCode(), (Func<int, Geometry, int>) ((current, value) => current * 397 ^ value.GetHashCode()));

    public bool Equals(GeometryCollection other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return base.Equals((object) other) && this.Geometries.SequenceEqual<Geometry>((IEnumerable<Geometry>) other.Geometries);
    }
  }
}
