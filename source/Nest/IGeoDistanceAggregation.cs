// Decompiled with JetBrains decompiler
// Type: Nest.IGeoDistanceAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (GeoDistanceAggregation))]
  public interface IGeoDistanceAggregation : IBucketAggregation, IAggregation
  {
    [DataMember(Name = "distance_type")]
    GeoDistanceType? DistanceType { get; set; }

    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "origin")]
    GeoLocation Origin { get; set; }

    [DataMember(Name = "ranges")]
    IEnumerable<IAggregationRange> Ranges { get; set; }

    [DataMember(Name = "unit")]
    DistanceUnit? Unit { get; set; }
  }
}
