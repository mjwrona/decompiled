// Decompiled with JetBrains decompiler
// Type: Nest.IResumeFollowIndexRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("ccr.resume_follow.json")]
  [ReadAs(typeof (ResumeFollowIndexRequest))]
  public interface IResumeFollowIndexRequest : IRequest<ResumeFollowIndexRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    IndexName Index { get; }

    [DataMember(Name = "max_read_request_operation_count")]
    long? MaxReadRequestOperationCount { get; set; }

    [DataMember(Name = "max_outstanding_read_requests")]
    long? MaxOutstandingReadRequests { get; set; }

    [DataMember(Name = "max_read_request_size")]
    string MaxRequestSize { get; set; }

    [DataMember(Name = "max_write_request_operation_count")]
    long? MaxWriteRequestOperationCount { get; set; }

    [DataMember(Name = "max_write_request_size")]
    string MaxWriteRequestSize { get; set; }

    [DataMember(Name = "max_outstanding_write_requests")]
    long? MaxOutstandingWriteRequests { get; set; }

    [DataMember(Name = "max_write_buffer_count")]
    long? MaxWriteBufferCount { get; set; }

    [DataMember(Name = "max_write_buffer_size")]
    string MaxWriteBufferSize { get; set; }

    [DataMember(Name = "max_retry_delay")]
    Time MaxRetryDelay { get; set; }

    [DataMember(Name = "read_poll_timeout")]
    Time ReadPollTimeout { get; set; }
  }
}
