// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.SpatialTreeBuilder`1
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Spatial
{
  internal abstract class SpatialTreeBuilder<T> : TypeWashedPipeline where T : class, ISpatial
  {
    private List<T> currentFigure;
    private SpatialTreeBuilder<T>.SpatialBuilderNode currentNode;
    private SpatialTreeBuilder<T>.SpatialBuilderNode lastConstructedNode;

    public event Action<T> ProduceInstance;

    public T ConstructedInstance
    {
      get
      {
        if (this.lastConstructedNode == null || (object) this.lastConstructedNode.Instance == null || this.lastConstructedNode.Parent != null)
          throw new InvalidOperationException(Strings.SpatialBuilder_CannotCreateBeforeDrawn);
        return this.lastConstructedNode.Instance;
      }
    }

    public override bool IsGeography => typeof (Geography).IsAssignableFrom(typeof (T));

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
    internal override void LineTo(double x, double y, double? z, double? m) => this.currentFigure.Add(this.CreatePoint(false, x, y, z, m));

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "z and m are meaningful")]
    internal override void BeginFigure(
      double coordinate1,
      double coordinate2,
      double? coordinate3,
      double? coordinate4)
    {
      if (this.currentFigure == null)
        this.currentFigure = new List<T>();
      this.currentFigure.Add(this.CreatePoint(false, coordinate1, coordinate2, coordinate3, coordinate4));
    }

    internal override void BeginGeo(SpatialType type)
    {
      if (this.currentNode == null)
      {
        this.currentNode = new SpatialTreeBuilder<T>.SpatialBuilderNode()
        {
          Type = type
        };
        this.lastConstructedNode = (SpatialTreeBuilder<T>.SpatialBuilderNode) null;
      }
      else
        this.currentNode = this.currentNode.CreateChildren(type);
    }

    internal override void EndFigure()
    {
      if (this.currentFigure.Count == 1)
        this.currentNode.CreateChildren(SpatialType.Point).Instance = this.currentFigure[0];
      else
        this.currentNode.CreateChildren(SpatialType.LineString).Instance = this.CreateShapeInstance(SpatialType.LineString, (IEnumerable<T>) this.currentFigure);
      this.currentFigure = (List<T>) null;
    }

    internal override void EndGeo()
    {
      switch (this.currentNode.Type)
      {
        case SpatialType.Point:
          this.currentNode.Instance = this.currentNode.Children.Count > 0 ? this.currentNode.Children[0].Instance : this.CreatePoint(true, double.NaN, double.NaN, new double?(), new double?());
          break;
        case SpatialType.LineString:
          this.currentNode.Instance = this.currentNode.Children.Count > 0 ? this.currentNode.Children[0].Instance : this.CreateShapeInstance(SpatialType.LineString, (IEnumerable<T>) new T[0]);
          break;
        case SpatialType.Polygon:
        case SpatialType.MultiPoint:
        case SpatialType.MultiLineString:
        case SpatialType.MultiPolygon:
        case SpatialType.Collection:
          this.currentNode.Instance = this.CreateShapeInstance(this.currentNode.Type, this.currentNode.Children.Select<SpatialTreeBuilder<T>.SpatialBuilderNode, T>((Func<SpatialTreeBuilder<T>.SpatialBuilderNode, T>) (node => node.Instance)));
          break;
        case SpatialType.FullGlobe:
          this.currentNode.Instance = this.CreateShapeInstance(SpatialType.FullGlobe, (IEnumerable<T>) new T[0]);
          break;
      }
      this.TraverseUpTheTree();
      this.NotifyIfWeJustFinishedBuildingSomething();
    }

    internal override void Reset()
    {
      this.currentNode = (SpatialTreeBuilder<T>.SpatialBuilderNode) null;
      this.currentFigure = (List<T>) null;
    }

    protected abstract T CreatePoint(bool isEmpty, double x, double y, double? z, double? m);

    protected abstract T CreateShapeInstance(SpatialType type, IEnumerable<T> spatialData);

    private void NotifyIfWeJustFinishedBuildingSomething()
    {
      if (this.currentNode != null || this.ProduceInstance == null)
        return;
      this.ProduceInstance(this.lastConstructedNode.Instance);
    }

    private void TraverseUpTheTree()
    {
      this.lastConstructedNode = this.currentNode;
      this.currentNode = this.currentNode.Parent;
    }

    private class SpatialBuilderNode
    {
      public SpatialBuilderNode() => this.Children = new List<SpatialTreeBuilder<T>.SpatialBuilderNode>();

      public List<SpatialTreeBuilder<T>.SpatialBuilderNode> Children { get; private set; }

      public T Instance { get; set; }

      public SpatialTreeBuilder<T>.SpatialBuilderNode Parent { get; private set; }

      public SpatialType Type { get; set; }

      internal SpatialTreeBuilder<T>.SpatialBuilderNode CreateChildren(SpatialType type)
      {
        SpatialTreeBuilder<T>.SpatialBuilderNode children = new SpatialTreeBuilder<T>.SpatialBuilderNode()
        {
          Parent = this,
          Type = type
        };
        this.Children.Add(children);
        return children;
      }
    }
  }
}
