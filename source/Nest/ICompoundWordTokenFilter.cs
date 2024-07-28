// Decompiled with JetBrains decompiler
// Type: Nest.ICompoundWordTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public interface ICompoundWordTokenFilter : ITokenFilter
  {
    [DataMember(Name = "hyphenation_patterns_path")]
    string HyphenationPatternsPath { get; set; }

    [DataMember(Name = "max_subword_size")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? MaxSubwordSize { get; set; }

    [DataMember(Name = "min_subword_size")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? MinSubwordSize { get; set; }

    [DataMember(Name = "min_word_size")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? MinWordSize { get; set; }

    [DataMember(Name = "only_longest_match")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? OnlyLongestMatch { get; set; }

    [DataMember(Name = "word_list")]
    IEnumerable<string> WordList { get; set; }

    [DataMember(Name = "word_list_path")]
    string WordListPath { get; set; }
  }
}
