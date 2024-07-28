// Decompiled with JetBrains decompiler
// Type: Nest.HistogramOrder
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  [JsonFormatter(typeof (SortOrderFormatter<HistogramOrder>))]
  public class HistogramOrder : ISortOrder
  {
    public static HistogramOrder CountAscending => new HistogramOrder()
    {
      Key = "_count",
      Order = SortOrder.Ascending
    };

    public static HistogramOrder CountDescending => new HistogramOrder()
    {
      Key = "_count",
      Order = SortOrder.Descending
    };

    public string Key { get; set; }

    public static HistogramOrder KeyAscending => new HistogramOrder()
    {
      Key = "_key",
      Order = SortOrder.Ascending
    };

    public static HistogramOrder KeyDescending => new HistogramOrder()
    {
      Key = "_key",
      Order = SortOrder.Descending
    };

    public SortOrder Order { get; set; }
  }
}
