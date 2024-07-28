// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyFullGlobe
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public abstract class GeographyFullGlobe : GeographySurface
  {
    protected GeographyFullGlobe(CoordinateSystem coordinateSystem, SpatialImplementation creator)
      : base(coordinateSystem, creator)
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "null is a valid value")]
    [SuppressMessage("Microsoft.Design", "CA1011", Justification = "Method should not be declared on base")]
    public bool Equals(GeographyFullGlobe other) => this.BaseEquals((Geography) other) ?? true;

    public override bool Equals(object obj) => this.Equals(obj as GeographyFullGlobe);

    public override int GetHashCode() => Geography.ComputeHashCodeFor<int>(this.CoordinateSystem, (IEnumerable<int>) new int[1]);
  }
}
