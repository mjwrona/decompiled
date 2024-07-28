// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyPipeline
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

namespace Microsoft.Spatial
{
  public abstract class GeographyPipeline
  {
    public abstract void BeginGeography(SpatialType type);

    public abstract void BeginFigure(GeographyPosition position);

    public abstract void LineTo(GeographyPosition position);

    public abstract void EndFigure();

    public abstract void EndGeography();

    public abstract void SetCoordinateSystem(CoordinateSystem coordinateSystem);

    public abstract void Reset();
  }
}
