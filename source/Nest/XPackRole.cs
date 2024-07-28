// Decompiled with JetBrains decompiler
// Type: Nest.XPackRole
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class XPackRole
  {
    [DataMember(Name = "applications")]
    public IReadOnlyCollection<IApplicationPrivileges> Applications { get; private set; } = EmptyReadOnly<IApplicationPrivileges>.Collection;

    [DataMember(Name = "cluster")]
    public IReadOnlyCollection<string> Cluster { get; private set; } = EmptyReadOnly<string>.Collection;

    [DataMember(Name = "indices")]
    public IReadOnlyCollection<IIndicesPrivileges> Indices { get; private set; } = EmptyReadOnly<IIndicesPrivileges>.Collection;

    [DataMember(Name = "metadata")]
    public IReadOnlyDictionary<string, object> Metadata { get; internal set; } = EmptyReadOnly<string, object>.Dictionary;

    [DataMember(Name = "run_as")]
    public IReadOnlyCollection<string> RunAs { get; private set; } = EmptyReadOnly<string>.Collection;
  }
}
