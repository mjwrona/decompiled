// Decompiled with JetBrains decompiler
// Type: Nest.GeoDistanceSort
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class GeoDistanceSort : SortBase, IGeoDistanceSort, ISort
  {
    public GeoDistanceType? DistanceType { get; set; }

    public Field Field { get; set; }

    public DistanceUnit? Unit { get; set; }

    public bool? IgnoreUnmapped { get; set; }

    public IEnumerable<GeoLocation> Points { get; set; }

    protected override Field SortKey => (Field) "_geo_distance";
  }
}
