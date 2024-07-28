// Decompiled with JetBrains decompiler
// Type: Nest.HasPrivilegesResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class HasPrivilegesResponse : ResponseBase
  {
    [DataMember(Name = "application")]
    [JsonFormatter(typeof (ApplicationsPrivilegesFormatter))]
    public IReadOnlyDictionary<string, IReadOnlyCollection<ResourcePrivileges>> Applications { get; internal set; } = EmptyReadOnly<string, IReadOnlyCollection<ResourcePrivileges>>.Dictionary;

    [DataMember(Name = "cluster")]
    public IReadOnlyDictionary<string, bool> Clusters { get; internal set; } = EmptyReadOnly<string, bool>.Dictionary;

    [DataMember(Name = "has_all_requested")]
    public bool HasAllRequested { get; internal set; }

    [DataMember(Name = "index")]
    [JsonFormatter(typeof (IndicesPrivilegesFormatter))]
    public IReadOnlyCollection<ResourcePrivileges> Indices { get; internal set; } = EmptyReadOnly<ResourcePrivileges>.Collection;

    [DataMember(Name = "username")]
    public string Username { get; internal set; }
  }
}
