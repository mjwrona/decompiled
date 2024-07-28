// Decompiled with JetBrains decompiler
// Type: Nest.IAnalysisConfig
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (AnalysisConfig))]
  public interface IAnalysisConfig
  {
    [DataMember(Name = "bucket_span")]
    Time BucketSpan { get; set; }

    [DataMember(Name = "categorization_field_name")]
    Field CategorizationFieldName { get; set; }

    [DataMember(Name = "categorization_filters")]
    IEnumerable<string> CategorizationFilters { get; set; }

    [DataMember(Name = "detectors")]
    IEnumerable<IDetector> Detectors { get; set; }

    [DataMember(Name = "influencers")]
    Fields Influencers { get; set; }

    [DataMember(Name = "latency")]
    Time Latency { get; set; }

    [DataMember(Name = "multivariate_by_fields")]
    bool? MultivariateByFields { get; set; }

    [DataMember(Name = "summary_count_field_name")]
    Field SummaryCountFieldName { get; set; }
  }
}
