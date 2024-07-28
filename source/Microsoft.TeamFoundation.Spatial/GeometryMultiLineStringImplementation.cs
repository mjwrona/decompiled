// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryMultiLineStringImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class GeometryMultiLineStringImplementation : GeometryMultiLineString
  {
    private GeometryLineString[] lineStrings;

    internal GeometryMultiLineStringImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator,
      params GeometryLineString[] lineStrings)
      : base(coordinateSystem, creator)
    {
      this.lineStrings = lineStrings ?? new GeometryLineString[0];
    }

    internal GeometryMultiLineStringImplementation(
      SpatialImplementation creator,
      params GeometryLineString[] lineStrings)
      : this(CoordinateSystem.DefaultGeometry, creator, lineStrings)
    {
    }

    public override bool IsEmpty => this.lineStrings.Length == 0;

    public override ReadOnlyCollection<Geometry> Geometries => new ReadOnlyCollection<Geometry>((IList<Geometry>) this.lineStrings);

    public override ReadOnlyCollection<GeometryLineString> LineStrings => new ReadOnlyCollection<GeometryLineString>((IList<GeometryLineString>) this.lineStrings);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
    public override void SendTo(GeometryPipeline pipeline)
    {
      base.SendTo(pipeline);
      pipeline.BeginGeometry(SpatialType.MultiLineString);
      for (int index = 0; index < this.lineStrings.Length; ++index)
        this.lineStrings[index].SendTo(pipeline);
      pipeline.EndGeometry();
    }
  }
}
