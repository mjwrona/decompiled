// Decompiled with JetBrains decompiler
// Type: Nest.IAutoFollowPattern
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (AutoFollowPattern))]
  public interface IAutoFollowPattern
  {
    [DataMember(Name = "follow_index_pattern")]
    string FollowIndexPattern { get; set; }

    [DataMember(Name = "leader_index_patterns")]
    IEnumerable<string> LeaderIndexPatterns { get; set; }

    [DataMember(Name = "leader_index_exclusion_patterns")]
    IEnumerable<string> LeaderIndexExclusionPatterns { get; set; }

    [DataMember(Name = "settings")]
    IIndexSettings Settings { get; set; }

    [DataMember(Name = "max_outstanding_read_requests")]
    long? MaxOutstandingReadRequests { get; set; }

    [DataMember(Name = "max_outstanding_write_requests")]
    int? MaxOutstandingWriteRequests { get; set; }

    [DataMember(Name = "read_poll_timeout")]
    Time MaxPollTimeout { get; set; }

    [DataMember(Name = "max_read_request_operation_count")]
    int? MaxReadRequestOperationCount { get; set; }

    [DataMember(Name = "max_read_request_size")]
    string MaxReadRequestSize { get; set; }

    [DataMember(Name = "max_retry_delay")]
    Time MaxRetryDelay { get; set; }

    [DataMember(Name = "max_write_buffer_count")]
    int? MaxWriteBufferCount { get; set; }

    [DataMember(Name = "max_write_buffer_size")]
    string MaxWriteBufferSize { get; set; }

    [DataMember(Name = "max_write_request_operation_count")]
    int? MaxWriteRequestOperationCount { get; set; }

    [DataMember(Name = "max_write_request_size")]
    string MaxWriteRequestSize { get; set; }

    [DataMember(Name = "remote_cluster")]
    string RemoteCluster { get; set; }
  }
}
