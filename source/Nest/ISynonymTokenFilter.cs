// Decompiled with JetBrains decompiler
// Type: Nest.ISynonymTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public interface ISynonymTokenFilter : ITokenFilter
  {
    [DataMember(Name = "expand")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? Expand { get; set; }

    [DataMember(Name = "format")]
    SynonymFormat? Format { get; set; }

    [DataMember(Name = "lenient")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? Lenient { get; set; }

    [DataMember(Name = "synonyms")]
    IEnumerable<string> Synonyms { get; set; }

    [DataMember(Name = "synonyms_path")]
    string SynonymsPath { get; set; }

    [DataMember(Name = "tokenizer")]
    string Tokenizer { get; set; }

    [DataMember(Name = "updateable")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? Updateable { get; set; }
  }
}
