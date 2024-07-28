// Decompiled with JetBrains decompiler
// Type: Nest.ICharGroupTokenizer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public interface ICharGroupTokenizer : ITokenizer
  {
    [DataMember(Name = "tokenize_on_chars")]
    IEnumerable<string> TokenizeOnCharacters { get; set; }

    [DataMember(Name = "max_token_length")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? MaxTokenLength { get; set; }
  }
}
