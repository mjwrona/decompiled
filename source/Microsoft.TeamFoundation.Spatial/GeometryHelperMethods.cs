// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeometryHelperMethods
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

namespace Microsoft.Spatial
{
  internal static class GeometryHelperMethods
  {
    internal static void SendFigure(
      this GeometryLineString GeometryLineString,
      GeometryPipeline pipeline)
    {
      Util.CheckArgumentNull((object) GeometryLineString, nameof (GeometryLineString));
      for (int index = 0; index < GeometryLineString.Points.Count; ++index)
      {
        GeometryPoint point = GeometryLineString.Points[index];
        GeometryPosition position = new GeometryPosition(point.X, point.Y, point.Z, point.M);
        if (index == 0)
          pipeline.BeginFigure(position);
        else
          pipeline.LineTo(position);
      }
      if (GeometryLineString.Points.Count <= 0)
        return;
      pipeline.EndFigure();
    }
  }
}
