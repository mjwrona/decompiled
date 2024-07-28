// Decompiled with JetBrains decompiler
// Type: Nest.IClusterRerouteRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.ClusterApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("cluster.reroute.json")]
  [ReadAs(typeof (ClusterRerouteRequest))]
  [InterfaceDataContract]
  public interface IClusterRerouteRequest : IRequest<ClusterRerouteRequestParameters>, IRequest
  {
    [DataMember(Name = "commands")]
    IList<IClusterRerouteCommand> Commands { get; set; }
  }
}
