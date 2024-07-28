// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyHelperMethods
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.ObjectModel;

namespace Microsoft.Spatial
{
  internal static class GeographyHelperMethods
  {
    internal static void SendFigure(this GeographyLineString lineString, GeographyPipeline pipeline)
    {
      ReadOnlyCollection<GeographyPoint> points = lineString.Points;
      for (int index = 0; index < points.Count; ++index)
      {
        if (index == 0)
          pipeline.BeginFigure(new GeographyPosition(points[index].Latitude, points[index].Longitude, points[index].Z, points[index].M));
        else
          pipeline.LineTo(new GeographyPosition(points[index].Latitude, points[index].Longitude, points[index].Z, points[index].M));
      }
      if (points.Count <= 0)
        return;
      pipeline.EndFigure();
    }
  }
}
