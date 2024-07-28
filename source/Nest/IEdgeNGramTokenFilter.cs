// Decompiled with JetBrains decompiler
// Type: Nest.IEdgeNGramTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IEdgeNGramTokenFilter : ITokenFilter
  {
    [DataMember(Name = "max_gram")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? MaxGram { get; set; }

    [DataMember(Name = "min_gram")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? MinGram { get; set; }

    [DataMember(Name = "side")]
    EdgeNGramSide? Side { get; set; }

    [DataMember(Name = "preserve_original")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? PreserveOriginal { get; set; }
  }
}
