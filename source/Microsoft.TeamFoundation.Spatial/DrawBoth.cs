// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.DrawBoth
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

namespace Microsoft.Spatial
{
  internal abstract class DrawBoth
  {
    public virtual GeographyPipeline GeographyPipeline => (GeographyPipeline) new DrawBoth.DrawGeographyInput(this);

    public virtual GeometryPipeline GeometryPipeline => (GeometryPipeline) new DrawBoth.DrawGeometryInput(this);

    public static implicit operator SpatialPipeline(DrawBoth both) => both != null ? new SpatialPipeline(both.GeographyPipeline, both.GeometryPipeline) : (SpatialPipeline) null;

    protected virtual GeographyPosition OnLineTo(GeographyPosition position) => position;

    protected virtual GeometryPosition OnLineTo(GeometryPosition position) => position;

    protected virtual GeographyPosition OnBeginFigure(GeographyPosition position) => position;

    protected virtual GeometryPosition OnBeginFigure(GeometryPosition position) => position;

    protected virtual SpatialType OnBeginGeography(SpatialType type) => type;

    protected virtual SpatialType OnBeginGeometry(SpatialType type) => type;

    protected virtual void OnEndFigure()
    {
    }

    protected virtual void OnEndGeography()
    {
    }

    protected virtual void OnEndGeometry()
    {
    }

    protected virtual void OnReset()
    {
    }

    protected virtual CoordinateSystem OnSetCoordinateSystem(CoordinateSystem coordinateSystem) => coordinateSystem;

    private class DrawGeographyInput : GeographyPipeline
    {
      private readonly DrawBoth both;

      public DrawGeographyInput(DrawBoth both) => this.both = both;

      public override void LineTo(GeographyPosition position) => this.both.OnLineTo(position);

      public override void BeginFigure(GeographyPosition position) => this.both.OnBeginFigure(position);

      public override void BeginGeography(SpatialType type)
      {
        int num = (int) this.both.OnBeginGeography(type);
      }

      public override void EndFigure() => this.both.OnEndFigure();

      public override void EndGeography() => this.both.OnEndGeography();

      public override void Reset() => this.both.OnReset();

      public override void SetCoordinateSystem(CoordinateSystem coordinateSystem) => this.both.OnSetCoordinateSystem(coordinateSystem);
    }

    private class DrawGeometryInput : GeometryPipeline
    {
      private readonly DrawBoth both;

      public DrawGeometryInput(DrawBoth both) => this.both = both;

      public override void LineTo(GeometryPosition position) => this.both.OnLineTo(position);

      public override void BeginFigure(GeometryPosition position) => this.both.OnBeginFigure(position);

      public override void BeginGeometry(SpatialType type)
      {
        int num = (int) this.both.OnBeginGeometry(type);
      }

      public override void EndFigure() => this.both.OnEndFigure();

      public override void EndGeometry() => this.both.OnEndGeometry();

      public override void Reset() => this.both.OnReset();

      public override void SetCoordinateSystem(CoordinateSystem coordinateSystem) => this.both.OnSetCoordinateSystem(coordinateSystem);
    }
  }
}
