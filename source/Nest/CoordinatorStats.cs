// Decompiled with JetBrains decompiler
// Type: Nest.CoordinatorStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class CoordinatorStats
  {
    [DataMember(Name = "node_id")]
    public string NodeId { get; internal set; }

    [DataMember(Name = "queue_size")]
    public int QueueSize { get; internal set; }

    [DataMember(Name = "remote_requests_current")]
    public int RemoteRequestsCurrent { get; internal set; }

    [DataMember(Name = "remote_requests_total")]
    public long RemoteRequestsTotal { get; internal set; }

    [DataMember(Name = "executed_searches_total")]
    public long ExecutedSearchesTotal { get; internal set; }
  }
}
