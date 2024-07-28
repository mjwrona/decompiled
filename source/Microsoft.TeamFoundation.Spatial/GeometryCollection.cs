// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryCollection
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Spatial
{
  public abstract class GeometryCollection : Geometry
  {
    protected GeometryCollection(CoordinateSystem coordinateSystem, SpatialImplementation creator)
      : base(coordinateSystem, creator)
    {
    }

    public abstract ReadOnlyCollection<Geometry> Geometries { get; }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
    public bool Equals(GeometryCollection other) => this.BaseEquals((Geometry) other) ?? this.Geometries.SequenceEqual<Geometry>((IEnumerable<Geometry>) other.Geometries);

    public override bool Equals(object obj) => this.Equals(obj as GeometryCollection);

    public override int GetHashCode() => Geography.ComputeHashCodeFor<Geometry>(this.CoordinateSystem, (IEnumerable<Geometry>) this.Geometries);
  }
}
