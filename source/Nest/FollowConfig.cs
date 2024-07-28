// Decompiled with JetBrains decompiler
// Type: Nest.FollowConfig
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class FollowConfig
  {
    [DataMember(Name = "max_read_request_operation_count")]
    public int MaximumReadRequestOperationCount { get; internal set; }

    [DataMember(Name = "max_read_request_size")]
    public string MaximumReadRequestSize { get; internal set; }

    [DataMember(Name = "max_outstanding_read_requests")]
    public int MaximumOutstandingReadRequests { get; internal set; }

    [DataMember(Name = "max_write_request_operation_count")]
    public int MaximumWriteRequestOperationCount { get; internal set; }

    [DataMember(Name = "max_write_request_size")]
    public string MaximumWriteRequestSize { get; internal set; }

    [DataMember(Name = "max_outstanding_write_requests")]
    public int MaximumOutstandingWriteRequests { get; internal set; }

    [DataMember(Name = "max_write_buffer_count")]
    public int MaximumWriteBufferCount { get; internal set; }

    [DataMember(Name = "max_write_buffer_size")]
    public string MaximumWriteBufferSize { get; internal set; }

    [DataMember(Name = "max_retry_delay")]
    public Time MaximumRetryDelay { get; internal set; }

    [DataMember(Name = "read_poll_timeout")]
    public Time ReadPollTimeout { get; internal set; }
  }
}
