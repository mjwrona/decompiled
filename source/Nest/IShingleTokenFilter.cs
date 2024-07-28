// Decompiled with JetBrains decompiler
// Type: Nest.IShingleTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IShingleTokenFilter : ITokenFilter
  {
    [DataMember(Name = "filler_token")]
    string FillerToken { get; set; }

    [DataMember(Name = "max_shingle_size")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? MaxShingleSize { get; set; }

    [DataMember(Name = "min_shingle_size")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? MinShingleSize { get; set; }

    [DataMember(Name = "output_unigrams")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? OutputUnigrams { get; set; }

    [DataMember(Name = "output_unigrams_if_no_shingles")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? OutputUnigramsIfNoShingles { get; set; }

    [DataMember(Name = "token_separator")]
    string TokenSeparator { get; set; }
  }
}
