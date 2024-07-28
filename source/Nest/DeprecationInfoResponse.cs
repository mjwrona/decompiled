// Decompiled with JetBrains decompiler
// Type: Nest.DeprecationInfoResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class DeprecationInfoResponse : ResponseBase
  {
    [DataMember(Name = "cluster_settings")]
    public IReadOnlyCollection<DeprecationInfo> ClusterSettings { get; internal set; } = EmptyReadOnly<DeprecationInfo>.Collection;

    [DataMember(Name = "index_settings")]
    public IReadOnlyDictionary<string, IReadOnlyCollection<DeprecationInfo>> IndexSettings { get; internal set; } = EmptyReadOnly<string, IReadOnlyCollection<DeprecationInfo>>.Dictionary;

    [DataMember(Name = "node_settings")]
    public IReadOnlyCollection<DeprecationInfo> NodeSettings { get; internal set; } = EmptyReadOnly<DeprecationInfo>.Collection;
  }
}
