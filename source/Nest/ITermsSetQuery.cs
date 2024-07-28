// Decompiled with JetBrains decompiler
// Type: Nest.ITermsSetQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (FieldNameQueryFormatter<TermsSetQuery, ITermsSetQuery>))]
  public interface ITermsSetQuery : IFieldNameQuery, IQuery
  {
    [DataMember(Name = "minimum_should_match_field")]
    Field MinimumShouldMatchField { get; set; }

    [DataMember(Name = "minimum_should_match_script")]
    IScript MinimumShouldMatchScript { get; set; }

    [DataMember(Name = "terms")]
    [JsonFormatter(typeof (SourceWriteFormatter<IEnumerable<object>>))]
    IEnumerable<object> Terms { get; set; }
  }
}
