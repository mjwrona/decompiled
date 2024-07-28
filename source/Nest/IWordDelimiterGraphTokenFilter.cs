// Decompiled with JetBrains decompiler
// Type: Nest.IWordDelimiterGraphTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IWordDelimiterGraphTokenFilter : ITokenFilter
  {
    [DataMember(Name = "adjust_offsets")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? AdjustOffsets { get; set; }

    [DataMember(Name = "catenate_all")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? CatenateAll { get; set; }

    [DataMember(Name = "catenate_numbers")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? CatenateNumbers { get; set; }

    [DataMember(Name = "catenate_words")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? CatenateWords { get; set; }

    [DataMember(Name = "generate_number_parts")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? GenerateNumberParts { get; set; }

    [DataMember(Name = "generate_word_parts")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? GenerateWordParts { get; set; }

    [DataMember(Name = "ignore_keywords")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? IgnoreKeywords { get; set; }

    [DataMember(Name = "preserve_original")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? PreserveOriginal { get; set; }

    [DataMember(Name = "protected_words")]
    IEnumerable<string> ProtectedWords { get; set; }

    [DataMember(Name = "protected_words_path ")]
    string ProtectedWordsPath { get; set; }

    [DataMember(Name = "split_on_case_change")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? SplitOnCaseChange { get; set; }

    [DataMember(Name = "split_on_numerics")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? SplitOnNumerics { get; set; }

    [DataMember(Name = "stem_english_possessive")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? StemEnglishPossessive { get; set; }

    [DataMember(Name = "type_table")]
    IEnumerable<string> TypeTable { get; set; }

    [DataMember(Name = "type_table_path")]
    string TypeTablePath { get; set; }
  }
}
