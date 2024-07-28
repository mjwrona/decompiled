// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.SpatialTypeExtensions
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

namespace Microsoft.Spatial
{
  public static class SpatialTypeExtensions
  {
    public static void SendTo(this ISpatial shape, SpatialPipeline destination)
    {
      if (shape == null)
        return;
      if (shape.GetType().IsSubclassOf(typeof (Geometry)))
        ((Geometry) shape).SendTo((GeometryPipeline) destination);
      else
        ((Geography) shape).SendTo((GeographyPipeline) destination);
    }
  }
}
