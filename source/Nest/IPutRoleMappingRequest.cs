// Decompiled with JetBrains decompiler
// Type: Nest.IPutRoleMappingRequest
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
  [MapsApi("security.put_role_mapping.json")]
  public interface IPutRoleMappingRequest : IRequest<PutRoleMappingRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Name Name { get; }

    [DataMember(Name = "enabled")]
    bool? Enabled { get; set; }

    [DataMember(Name = "metadata")]
    [JsonFormatter(typeof (VerbatimDictionaryInterfaceKeysPreservingNullFormatter<string, object>))]
    IDictionary<string, object> Metadata { get; set; }

    [DataMember(Name = "roles")]
    IEnumerable<string> Roles { get; set; }

    [DataMember(Name = "rules")]
    RoleMappingRuleBase Rules { get; set; }

    [DataMember(Name = "run_as")]
    IEnumerable<string> RunAs { get; set; }
  }
}
