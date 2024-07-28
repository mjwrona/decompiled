// Decompiled with JetBrains decompiler
// Type: Nest.ICommonTermsQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [Obsolete("Deprecated in 7.3.0. Use MatchQuery instead, which skips blocks of documents efficiently, without any configuration, provided that the total number of hits is not tracked.")]
  [InterfaceDataContract]
  [JsonFormatter(typeof (FieldNameQueryFormatter<CommonTermsQuery, ICommonTermsQuery>))]
  public interface ICommonTermsQuery : IFieldNameQuery, IQuery
  {
    [DataMember(Name = "analyzer")]
    string Analyzer { get; set; }

    [DataMember(Name = "cutoff_frequency")]
    double? CutoffFrequency { get; set; }

    [DataMember(Name = "high_freq_operator")]
    Operator? HighFrequencyOperator { get; set; }

    [DataMember(Name = "low_freq_operator")]
    Operator? LowFrequencyOperator { get; set; }

    [DataMember(Name = "minimum_should_match")]
    MinimumShouldMatch MinimumShouldMatch { get; set; }

    [DataMember(Name = "query")]
    string Query { get; set; }
  }
}
