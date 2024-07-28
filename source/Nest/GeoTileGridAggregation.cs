// Decompiled with JetBrains decompiler
// Type: Nest.GeoTileGridAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class GeoTileGridAggregation : 
    BucketAggregationBase,
    IGeoTileGridAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal GeoTileGridAggregation()
    {
    }

    public GeoTileGridAggregation(string name)
      : base(name)
    {
    }

    public Field Field { get; set; }

    public GeoTilePrecision? Precision { get; set; }

    public int? ShardSize { get; set; }

    public int? Size { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.GeoTile = (IGeoTileGridAggregation) this;
  }
}
