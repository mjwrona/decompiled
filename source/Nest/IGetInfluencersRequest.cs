// Decompiled with JetBrains decompiler
// Type: Nest.IGetInfluencersRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("ml.get_influencers.json")]
  public interface IGetInfluencersRequest : IRequest<GetInfluencersRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Id JobId { get; }

    bool? Descending { get; set; }

    [DataMember(Name = "end")]
    [JsonFormatter(typeof (NullableDateTimeOffsetEpochMillisecondsFormatter))]
    DateTimeOffset? End { get; set; }

    [DataMember(Name = "exclude_interim")]
    bool? ExcludeInterim { get; set; }

    [DataMember(Name = "influencer_score")]
    double? InfluencerScore { get; set; }

    [DataMember(Name = "page")]
    IPage Page { get; set; }

    [DataMember(Name = "sort")]
    Field Sort { get; set; }

    [DataMember(Name = "start")]
    [JsonFormatter(typeof (NullableDateTimeOffsetEpochMillisecondsFormatter))]
    DateTimeOffset? Start { get; set; }
  }
}
