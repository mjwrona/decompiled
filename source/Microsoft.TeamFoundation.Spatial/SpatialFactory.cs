// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.SpatialFactory
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public abstract class SpatialFactory
  {
    private Stack<SpatialType> containers;
    private bool figureDrawn;
    private bool inRing;
    private bool ringClosed;
    private double ringStartX;
    private double ringStartY;
    private double? ringStartZ;
    private double? ringStartM;

    internal SpatialFactory() => this.containers = new Stack<SpatialType>();

    private SpatialType CurrentType => this.containers.Count == 0 ? SpatialType.Unknown : this.containers.Peek();

    protected virtual void BeginGeo(SpatialType type)
    {
      while (!this.CanContain(type))
        this.EndGeo();
      this.containers.Push(type);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    protected virtual void BeginFigure(double x, double y, double? z, double? m) => this.figureDrawn = true;

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    protected virtual void AddLine(double x, double y, double? z, double? m)
    {
      if (!this.inRing)
        return;
      this.ringClosed = x == this.ringStartX && y == this.ringStartY;
    }

    protected virtual void EndFigure()
    {
      if (this.inRing)
      {
        if (!this.ringClosed)
          this.AddLine(this.ringStartX, this.ringStartY, this.ringStartZ, this.ringStartM);
        this.inRing = false;
        this.ringClosed = true;
      }
      this.figureDrawn = false;
    }

    protected virtual void EndGeo()
    {
      if (this.figureDrawn)
        this.EndFigure();
      int num = (int) this.containers.Pop();
    }

    protected virtual void Finish()
    {
      while (this.containers.Count > 0)
        this.EndGeo();
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    protected virtual void AddPos(double x, double y, double? z, double? m)
    {
      if (!this.figureDrawn)
        this.BeginFigure(x, y, z, m);
      else
        this.AddLine(x, y, z, m);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
    protected virtual void StartRing(double x, double y, double? z, double? m)
    {
      if (this.figureDrawn)
        this.EndFigure();
      this.BeginFigure(x, y, z, m);
      this.ringStartX = x;
      this.ringStartY = y;
      this.ringStartM = m;
      this.ringStartZ = z;
      this.inRing = true;
      this.ringClosed = false;
    }

    private bool CanContain(SpatialType type)
    {
      switch (this.CurrentType)
      {
        case SpatialType.Unknown:
        case SpatialType.Collection:
          return true;
        case SpatialType.MultiPoint:
          return type == SpatialType.Point;
        case SpatialType.MultiLineString:
          return type == SpatialType.LineString;
        case SpatialType.MultiPolygon:
          return type == SpatialType.Polygon;
        default:
          return false;
      }
    }
  }
}
