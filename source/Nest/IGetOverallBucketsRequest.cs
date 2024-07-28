// Decompiled with JetBrains decompiler
// Type: Nest.IGetOverallBucketsRequest
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
  [MapsApi("ml.get_overall_buckets.json")]
  public interface IGetOverallBucketsRequest : IRequest<GetOverallBucketsRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Id JobId { get; }

    [DataMember(Name = "allow_no_jobs")]
    bool? AllowNoJobs { get; set; }

    [DataMember(Name = "bucket_span")]
    Time BucketSpan { get; set; }

    [DataMember(Name = "end")]
    DateTimeOffset? End { get; set; }

    [DataMember(Name = "exclude_interim")]
    bool? ExcludeInterim { get; set; }

    [DataMember(Name = "overall_score")]
    double? OverallScore { get; set; }

    [DataMember(Name = "start")]
    DateTimeOffset? Start { get; set; }

    [DataMember(Name = "top_n")]
    int? TopN { get; set; }
  }
}
