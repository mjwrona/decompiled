// Decompiled with JetBrains decompiler
// Type: Nest.IPhoneticTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IPhoneticTokenFilter : ITokenFilter
  {
    [DataMember(Name = "encoder")]
    PhoneticEncoder? Encoder { get; set; }

    [DataMember(Name = "languageset")]
    IEnumerable<PhoneticLanguage> LanguageSet { get; set; }

    [DataMember(Name = "max_code_len")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? MaxCodeLength { get; set; }

    [DataMember(Name = "name_type")]
    PhoneticNameType? NameType { get; set; }

    [DataMember(Name = "replace")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? Replace { get; set; }

    [DataMember(Name = "rule_type")]
    PhoneticRuleType? RuleType { get; set; }
  }
}
