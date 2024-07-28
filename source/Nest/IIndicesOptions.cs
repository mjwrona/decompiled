// Decompiled with JetBrains decompiler
// Type: Nest.IIndicesOptions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (IndicesOptions))]
  public interface IIndicesOptions
  {
    [DataMember(Name = "allow_no_indices")]
    bool? AllowNoIndices { get; set; }

    [DataMember(Name = "expand_wildcards")]
    [JsonFormatter(typeof (ExpandWildcardsFormatter))]
    IEnumerable<Elasticsearch.Net.ExpandWildcards> ExpandWildcards { get; set; }

    [DataMember(Name = "ignore_unavailable")]
    bool? IgnoreUnavailable { get; set; }
  }
}
