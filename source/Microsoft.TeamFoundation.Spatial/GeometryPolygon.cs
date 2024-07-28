// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryPolygon
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Spatial
{
  public abstract class GeometryPolygon : GeometrySurface
  {
    protected GeometryPolygon(CoordinateSystem coordinateSystem, SpatialImplementation creator)
      : base(coordinateSystem, creator)
    {
    }

    public abstract ReadOnlyCollection<GeometryLineString> Rings { get; }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
    public bool Equals(GeometryPolygon other) => this.BaseEquals((Geometry) other) ?? this.Rings.SequenceEqual<GeometryLineString>((IEnumerable<GeometryLineString>) other.Rings);

    public override bool Equals(object obj) => this.Equals(obj as GeometryPolygon);

    public override int GetHashCode() => Geography.ComputeHashCodeFor<GeometryLineString>(this.CoordinateSystem, (IEnumerable<GeometryLineString>) this.Rings);
  }
}
