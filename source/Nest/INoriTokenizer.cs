// Decompiled with JetBrains decompiler
// Type: Nest.INoriTokenizer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public interface INoriTokenizer : ITokenizer
  {
    [DataMember(Name = "decompound_mode")]
    NoriDecompoundMode? DecompoundMode { get; set; }

    [DataMember(Name = "discard_punctuation")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? DiscardPunctuation { get; set; }

    [DataMember(Name = "user_dictionary")]
    string UserDictionary { get; set; }

    [DataMember(Name = "user_dictionary_rules")]
    IEnumerable<string> UserDictionaryRules { get; set; }
  }
}
