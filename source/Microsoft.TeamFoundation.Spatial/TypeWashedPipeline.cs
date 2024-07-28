// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.TypeWashedPipeline
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

namespace Microsoft.Spatial
{
  internal abstract class TypeWashedPipeline
  {
    public abstract bool IsGeography { get; }

    internal abstract void SetCoordinateSystem(int? epsgId);

    internal abstract void Reset();

    internal abstract void BeginGeo(SpatialType type);

    internal abstract void BeginFigure(
      double coordinate1,
      double coordinate2,
      double? coordinate3,
      double? coordinate4);

    internal abstract void LineTo(
      double coordinate1,
      double coordinate2,
      double? coordinate3,
      double? coordinate4);

    internal abstract void EndFigure();

    internal abstract void EndGeo();
  }
}
