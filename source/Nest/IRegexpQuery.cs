// Decompiled with JetBrains decompiler
// Type: Nest.IRegexpQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (FieldNameQueryFormatter<RegexpQuery, IRegexpQuery>))]
  public interface IRegexpQuery : IFieldNameQuery, IQuery
  {
    [DataMember(Name = "flags")]
    string Flags { get; set; }

    [DataMember(Name = "max_determinized_states")]
    int? MaximumDeterminizedStates { get; set; }

    [DataMember(Name = "value")]
    string Value { get; set; }

    [DataMember(Name = "rewrite")]
    MultiTermQueryRewrite Rewrite { get; set; }
  }
}
