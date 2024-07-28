// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyFullGlobeImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class GeographyFullGlobeImplementation : GeographyFullGlobe
  {
    internal GeographyFullGlobeImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator)
      : base(coordinateSystem, creator)
    {
    }

    internal GeographyFullGlobeImplementation(SpatialImplementation creator)
      : this(CoordinateSystem.DefaultGeography, creator)
    {
    }

    public override bool IsEmpty => false;

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
    public override void SendTo(GeographyPipeline pipeline)
    {
      base.SendTo(pipeline);
      pipeline.BeginGeography(SpatialType.FullGlobe);
      pipeline.EndGeography();
    }
  }
}
