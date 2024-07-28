// Decompiled with JetBrains decompiler
// Type: Nest.GeoLineAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class GeoLineAggregation : AggregationBase, IGeoLineAggregation, IAggregation
  {
    internal GeoLineAggregation()
    {
    }

    public GeoLineAggregation(string name, Field point, Field sort)
      : base(name)
    {
      this.Point = new GeoLinePoint() { Field = point };
      this.Sort = new GeoLineSort() { Field = sort };
    }

    internal override void WrapInContainer(AggregationContainer c) => c.GeoLine = (IGeoLineAggregation) this;

    public GeoLinePoint Point { get; set; }

    public GeoLineSort Sort { get; set; }

    public bool? IncludeSort { get; set; }

    public string SortOrder { get; set; }

    public int? Size { get; set; }
  }
}
