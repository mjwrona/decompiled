// Decompiled with JetBrains decompiler
// Type: Nest.IKeyValueProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IKeyValueProcessor : IProcessor
  {
    [DataMember(Name = "exclude_keys")]
    IEnumerable<string> ExcludeKeys { get; set; }

    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "field_split")]
    string FieldSplit { get; set; }

    [DataMember(Name = "ignore_missing")]
    bool? IgnoreMissing { get; set; }

    [DataMember(Name = "include_keys")]
    IEnumerable<string> IncludeKeys { get; set; }

    [DataMember(Name = "prefix")]
    string Prefix { get; set; }

    [DataMember(Name = "strip_brackets")]
    bool? StripBrackets { get; set; }

    [DataMember(Name = "target_field")]
    Field TargetField { get; set; }

    [DataMember(Name = "trim_key")]
    string TrimKey { get; set; }

    [DataMember(Name = "trim_value")]
    string TrimValue { get; set; }

    [DataMember(Name = "value_split")]
    string ValueSplit { get; set; }
  }
}
