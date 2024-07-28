// Decompiled with JetBrains decompiler
// Type: Nest.FollowIndexShardStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class FollowIndexShardStats
  {
    [DataMember(Name = "bytes_read")]
    public long BytesRead { get; set; }

    [DataMember(Name = "failed_read_requests")]
    public long FailedReadRequests { get; set; }

    [DataMember(Name = "failed_write_requests")]
    public long FailedWriteRequests { get; set; }

    [DataMember(Name = "follower_global_checkpoint")]
    public long FollowerGlobalCheckpoint { get; set; }

    [DataMember(Name = "follower_index")]
    public string FollowerIndex { get; set; }

    [DataMember(Name = "follower_mapping_version")]
    public long FollowerMappingVersion { get; set; }

    [DataMember(Name = "follower_max_seq_no")]
    public long FollowerMaxSequenceNumber { get; set; }

    [DataMember(Name = "follower_settings_version")]
    public long FollowerSettingsVersion { get; set; }

    [DataMember(Name = "follower_aliases_version")]
    public long FollowerAliasesVersion { get; set; }

    [DataMember(Name = "last_requested_seq_no")]
    public long LastRequestedSequenceNumber { get; set; }

    [DataMember(Name = "leader_global_checkpoint")]
    public long LeaderGlobalCheckpoint { get; set; }

    [DataMember(Name = "leader_index")]
    public string LeaderIndex { get; set; }

    [DataMember(Name = "leader_max_seq_no")]
    public long LeaderMaxSequenceNumber { get; set; }

    [DataMember(Name = "operations_read")]
    public long OperationsRead { get; set; }

    [DataMember(Name = "operations_written")]
    public long OperationsWritten { get; set; }

    [DataMember(Name = "outstanding_read_requests")]
    public int OutstandingReadRequests { get; set; }

    [DataMember(Name = "outstanding_write_requests")]
    public int OutstandingWriteRequest { get; set; }

    [DataMember(Name = "remote_cluster")]
    public string RemoteCluster { get; set; }

    [DataMember(Name = "shard_id")]
    public int ShardId { get; set; }

    [DataMember(Name = "successful_read_requests")]
    public long SuccessfulReadRequests { get; set; }

    [DataMember(Name = "successful_write_requests")]
    public long SuccessfulWriteRequests { get; set; }

    [DataMember(Name = "total_read_remote_exec_time_millis")]
    public long TotalReadRemoteExecutionTimeInMilliseconds { get; set; }

    [DataMember(Name = "total_read_time_millis")]
    public long TotalReadTimeInMilliseconds { get; set; }

    [DataMember(Name = "total_write_time_millis")]
    public long TotalWriteTimeInMilliseconds { get; set; }

    [DataMember(Name = "write_buffer_operation_count")]
    public long WriteBufferOperationCount { get; set; }

    [DataMember(Name = "write_buffer_size_in_bytes")]
    public long WriteBufferSizeInBytes { get; set; }

    [DataMember(Name = "read_exceptions")]
    public IReadOnlyCollection<FollowIndexReadException> ReadExceptions { get; internal set; } = EmptyReadOnly<FollowIndexReadException>.Collection;

    [DataMember(Name = "time_since_last_read_millis")]
    public long TimeSinceLastReadInMilliseconds { get; set; }

    [DataMember(Name = "fatal_exception")]
    public ErrorCause FatalException { get; set; }
  }
}
