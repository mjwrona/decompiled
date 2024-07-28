// Decompiled with JetBrains decompiler
// Type: Nest.TermsOrder
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  [JsonFormatter(typeof (SortOrderFormatter<TermsOrder>))]
  public class TermsOrder : ISortOrder
  {
    public static TermsOrder CountAscending => new TermsOrder()
    {
      Key = "_count",
      Order = SortOrder.Ascending
    };

    public static TermsOrder CountDescending => new TermsOrder()
    {
      Key = "_count",
      Order = SortOrder.Descending
    };

    public string Key { get; set; }

    public static TermsOrder KeyAscending => new TermsOrder()
    {
      Key = "_key",
      Order = SortOrder.Ascending
    };

    public static TermsOrder KeyDescending => new TermsOrder()
    {
      Key = "_key",
      Order = SortOrder.Descending
    };

    public SortOrder Order { get; set; }
  }
}
