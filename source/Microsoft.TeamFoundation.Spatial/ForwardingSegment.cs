// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.ForwardingSegment
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;

namespace Microsoft.Spatial
{
  internal class ForwardingSegment : SpatialPipeline
  {
    internal static readonly SpatialPipeline SpatialPipelineNoOp = new SpatialPipeline((GeographyPipeline) new ForwardingSegment.NoOpGeographyPipeline(), (GeometryPipeline) new ForwardingSegment.NoOpGeometryPipeline());
    private readonly SpatialPipeline current;
    private SpatialPipeline next = ForwardingSegment.SpatialPipelineNoOp;
    private ForwardingSegment.GeographyForwarder geographyForwarder;
    private ForwardingSegment.GeometryForwarder geometryForwarder;

    public ForwardingSegment(SpatialPipeline current) => this.current = current;

    public ForwardingSegment(GeographyPipeline currentGeography, GeometryPipeline currentGeometry)
      : this(new SpatialPipeline(currentGeography, currentGeometry))
    {
    }

    public override GeographyPipeline GeographyPipeline => (GeographyPipeline) this.geographyForwarder ?? (GeographyPipeline) (this.geographyForwarder = new ForwardingSegment.GeographyForwarder(this));

    public override GeometryPipeline GeometryPipeline => (GeometryPipeline) this.geometryForwarder ?? (GeometryPipeline) (this.geometryForwarder = new ForwardingSegment.GeometryForwarder(this));

    public GeographyPipeline NextDrawGeography => (GeographyPipeline) this.next;

    public GeometryPipeline NextDrawGeometry => (GeometryPipeline) this.next;

    public override SpatialPipeline ChainTo(SpatialPipeline destination)
    {
      Util.CheckArgumentNull((object) destination, nameof (destination));
      this.next = destination;
      destination.StartingLink = this.StartingLink;
      return destination;
    }

    private static void DoAction(
      Action handler,
      Action handlerReset,
      Action delegation,
      Action delegationReset)
    {
      try
      {
        handler();
      }
      catch (Exception ex)
      {
        if (Util.IsCatchableExceptionType(ex))
        {
          handlerReset();
          delegationReset();
        }
        throw;
      }
      try
      {
        delegation();
      }
      catch (Exception ex)
      {
        if (Util.IsCatchableExceptionType(ex))
          handlerReset();
        throw;
      }
    }

    private static void DoAction<T>(
      Action<T> handler,
      Action handlerReset,
      Action<T> delegation,
      Action delegationReset,
      T argument)
    {
      try
      {
        handler(argument);
      }
      catch (Exception ex)
      {
        if (Util.IsCatchableExceptionType(ex))
        {
          handlerReset();
          delegationReset();
        }
        throw;
      }
      try
      {
        delegation(argument);
      }
      catch (Exception ex)
      {
        if (Util.IsCatchableExceptionType(ex))
          handlerReset();
        throw;
      }
    }

    internal class GeographyForwarder : GeographyPipeline
    {
      private readonly ForwardingSegment segment;

      public GeographyForwarder(ForwardingSegment segment) => this.segment = segment;

      private GeographyPipeline Current => (GeographyPipeline) this.segment.current;

      private GeographyPipeline Next => (GeographyPipeline) this.segment.next;

      public override void SetCoordinateSystem(CoordinateSystem coordinateSystem) => this.DoAction<CoordinateSystem>((Action<CoordinateSystem>) (val => this.Current.SetCoordinateSystem(val)), (Action<CoordinateSystem>) (val => this.Next.SetCoordinateSystem(val)), coordinateSystem);

      public override void BeginGeography(SpatialType type) => this.DoAction<SpatialType>((Action<SpatialType>) (val => this.Current.BeginGeography(val)), (Action<SpatialType>) (val => this.Next.BeginGeography(val)), type);

      public override void EndGeography() => this.DoAction(new Action(this.Current.EndGeography), new Action(this.Next.EndGeography));

      public override void BeginFigure(GeographyPosition position)
      {
        Util.CheckArgumentNull((object) position, nameof (position));
        this.DoAction<GeographyPosition>((Action<GeographyPosition>) (val => this.Current.BeginFigure(val)), (Action<GeographyPosition>) (val => this.Next.BeginFigure(val)), position);
      }

      public override void EndFigure() => this.DoAction(new Action(this.Current.EndFigure), new Action(this.Next.EndFigure));

      public override void LineTo(GeographyPosition position)
      {
        Util.CheckArgumentNull((object) position, nameof (position));
        this.DoAction<GeographyPosition>((Action<GeographyPosition>) (val => this.Current.LineTo(val)), (Action<GeographyPosition>) (val => this.Next.LineTo(val)), position);
      }

      public override void Reset() => this.DoAction(new Action(this.Current.Reset), new Action(this.Next.Reset));

      private void DoAction<T>(Action<T> handler, Action<T> delegation, T argument) => ForwardingSegment.DoAction<T>(handler, new Action(this.Current.Reset), delegation, new Action(this.Next.Reset), argument);

      private void DoAction(Action handler, Action delegation) => ForwardingSegment.DoAction(handler, new Action(this.Current.Reset), delegation, new Action(this.Next.Reset));
    }

    internal class GeometryForwarder : GeometryPipeline
    {
      private readonly ForwardingSegment segment;

      public GeometryForwarder(ForwardingSegment segment) => this.segment = segment;

      private GeometryPipeline Current => (GeometryPipeline) this.segment.current;

      private GeometryPipeline Next => (GeometryPipeline) this.segment.next;

      public override void SetCoordinateSystem(CoordinateSystem coordinateSystem) => this.DoAction<CoordinateSystem>((Action<CoordinateSystem>) (val => this.Current.SetCoordinateSystem(val)), (Action<CoordinateSystem>) (val => this.Next.SetCoordinateSystem(val)), coordinateSystem);

      public override void BeginGeometry(SpatialType type) => this.DoAction<SpatialType>((Action<SpatialType>) (val => this.Current.BeginGeometry(val)), (Action<SpatialType>) (val => this.Next.BeginGeometry(val)), type);

      public override void EndGeometry() => this.DoAction(new Action(this.Current.EndGeometry), new Action(this.Next.EndGeometry));

      public override void BeginFigure(GeometryPosition position)
      {
        Util.CheckArgumentNull((object) position, nameof (position));
        this.DoAction<GeometryPosition>((Action<GeometryPosition>) (val => this.Current.BeginFigure(val)), (Action<GeometryPosition>) (val => this.Next.BeginFigure(val)), position);
      }

      public override void EndFigure() => this.DoAction(new Action(this.Current.EndFigure), new Action(this.Next.EndFigure));

      public override void LineTo(GeometryPosition position)
      {
        Util.CheckArgumentNull((object) position, nameof (position));
        this.DoAction<GeometryPosition>((Action<GeometryPosition>) (val => this.Current.LineTo(val)), (Action<GeometryPosition>) (val => this.Next.LineTo(val)), position);
      }

      public override void Reset() => this.DoAction(new Action(this.Current.Reset), new Action(this.Next.Reset));

      private void DoAction<T>(Action<T> handler, Action<T> delegation, T argument) => ForwardingSegment.DoAction<T>(handler, new Action(this.Current.Reset), delegation, new Action(this.Next.Reset), argument);

      private void DoAction(Action handler, Action delegation) => ForwardingSegment.DoAction(handler, new Action(this.Current.Reset), delegation, new Action(this.Next.Reset));
    }

    private class NoOpGeographyPipeline : GeographyPipeline
    {
      public override void LineTo(GeographyPosition position)
      {
      }

      public override void BeginFigure(GeographyPosition position)
      {
      }

      public override void BeginGeography(SpatialType type)
      {
      }

      public override void EndFigure()
      {
      }

      public override void EndGeography()
      {
      }

      public override void Reset()
      {
      }

      public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
      {
      }
    }

    private class NoOpGeometryPipeline : GeometryPipeline
    {
      public override void LineTo(GeometryPosition position)
      {
      }

      public override void BeginFigure(GeometryPosition position)
      {
      }

      public override void BeginGeometry(SpatialType type)
      {
      }

      public override void EndFigure()
      {
      }

      public override void EndGeometry()
      {
      }

      public override void Reset()
      {
      }

      public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
      {
      }
    }
  }
}
