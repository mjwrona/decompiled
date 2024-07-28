// Decompiled with JetBrains decompiler
// Type: Nest.GetUserPrivilegesResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetUserPrivilegesResponse : ResponseBase
  {
    [DataMember(Name = "applications")]
    public IReadOnlyCollection<ApplicationResourcePrivileges> Applications { get; internal set; } = EmptyReadOnly<ApplicationResourcePrivileges>.Collection;

    [DataMember(Name = "cluster")]
    public IReadOnlyCollection<string> Cluster { get; internal set; } = EmptyReadOnly<string>.Collection;

    [DataMember(Name = "global")]
    public IReadOnlyCollection<GlobalPrivileges> Global { get; internal set; } = EmptyReadOnly<GlobalPrivileges>.Collection;

    [DataMember(Name = "indices")]
    public IReadOnlyCollection<UserIndicesPrivileges> Indices { get; internal set; } = EmptyReadOnly<UserIndicesPrivileges>.Collection;

    [DataMember(Name = "run_as")]
    public IReadOnlyCollection<string> RunAs { get; internal set; } = EmptyReadOnly<string>.Collection;
  }
}
