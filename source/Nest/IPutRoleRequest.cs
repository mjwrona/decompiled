// Decompiled with JetBrains decompiler
// Type: Nest.IPutRoleRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("security.put_role.json")]
  public interface IPutRoleRequest : IRequest<PutRoleRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Name Name { get; }

    [DataMember(Name = "applications")]
    IEnumerable<IApplicationPrivileges> Applications { get; set; }

    [DataMember(Name = "cluster")]
    IEnumerable<string> Cluster { get; set; }

    [DataMember(Name = "global")]
    [JsonFormatter(typeof (VerbatimDictionaryInterfaceKeysPreservingNullFormatter<string, object>))]
    IDictionary<string, object> Global { get; set; }

    [DataMember(Name = "indices")]
    IEnumerable<IIndicesPrivileges> Indices { get; set; }

    [DataMember(Name = "metadata")]
    [JsonFormatter(typeof (VerbatimDictionaryInterfaceKeysPreservingNullFormatter<string, object>))]
    IDictionary<string, object> Metadata { get; set; }

    [DataMember(Name = "run_as")]
    IEnumerable<string> RunAs { get; set; }
  }
}
