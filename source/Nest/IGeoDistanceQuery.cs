// Decompiled with JetBrains decompiler
// Type: Nest.IGeoDistanceQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  [JsonFormatter(typeof (GeoDistanceQueryFormatter))]
  public interface IGeoDistanceQuery : IFieldNameQuery, IQuery
  {
    Distance Distance { get; set; }

    GeoDistanceType? DistanceType { get; set; }

    GeoLocation Location { get; set; }

    GeoValidationMethod? ValidationMethod { get; set; }

    bool? IgnoreUnmapped { get; set; }
  }
}
