// Decompiled with JetBrains decompiler
// Type: Nest.GeoTileGridGroupSourceDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class GeoTileGridGroupSourceDescriptor<T> : 
    SingleGroupSourceDescriptorBase<GeoTileGridGroupSourceDescriptor<T>, IGeoTileGridGroupSource, T>,
    IGeoTileGridGroupSource,
    ISingleGroupSource
  {
    GeoTilePrecision? IGeoTileGridGroupSource.Precision { get; set; }

    IBoundingBox IGeoTileGridGroupSource.Bounds { get; set; }

    public GeoTileGridGroupSourceDescriptor<T> Precision(GeoTilePrecision? precision) => this.Assign<GeoTilePrecision?>(precision, (Action<IGeoTileGridGroupSource, GeoTilePrecision?>) ((a, v) => a.Precision = v));

    public GeoTileGridGroupSourceDescriptor<T> Bounds(
      Func<BoundingBoxDescriptor, IBoundingBox> selector)
    {
      return this.Assign<Func<BoundingBoxDescriptor, IBoundingBox>>(selector, (Action<IGeoTileGridGroupSource, Func<BoundingBoxDescriptor, IBoundingBox>>) ((a, v) => a.Bounds = v != null ? v(new BoundingBoxDescriptor()) : (IBoundingBox) null));
    }
  }
}
