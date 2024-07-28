// Decompiled with JetBrains decompiler
// Type: Nest.GeoTileGridCompositeAggregationSourceDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class GeoTileGridCompositeAggregationSourceDescriptor<T> : 
    CompositeAggregationSourceDescriptorBase<GeoTileGridCompositeAggregationSourceDescriptor<T>, IGeoTileGridCompositeAggregationSource, T>,
    IGeoTileGridCompositeAggregationSource,
    ICompositeAggregationSource
  {
    public GeoTileGridCompositeAggregationSourceDescriptor(string name)
      : base(name, "geotile_grid")
    {
    }

    GeoTilePrecision? IGeoTileGridCompositeAggregationSource.Precision { get; set; }

    public GeoTileGridCompositeAggregationSourceDescriptor<T> Precision(GeoTilePrecision? precision) => this.Assign<GeoTilePrecision?>(precision, (Action<IGeoTileGridCompositeAggregationSource, GeoTilePrecision?>) ((a, v) => a.Precision = v));
  }
}
