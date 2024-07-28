// Decompiled with JetBrains decompiler
// Type: Nest.UnassignedInformationReason
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum UnassignedInformationReason
  {
    [EnumMember(Value = "INDEX_CREATED")] IndexCreated,
    [EnumMember(Value = "CLUSTER_RECOVERED")] ClusterRecovered,
    [EnumMember(Value = "INDEX_REOPENED")] IndexReopened,
    [EnumMember(Value = "DANGLING_INDEX_IMPORTED")] DanglingIndexImported,
    [EnumMember(Value = "NEW_INDEX_RESTORED")] NewIndexRestored,
    [EnumMember(Value = "EXISTING_INDEX_RESTORED")] ExistingIndexRestored,
    [EnumMember(Value = "REPLICA_ADDED")] ReplicaAdded,
    [EnumMember(Value = "ALLOCATION_FAILED")] AllocationFailed,
    [EnumMember(Value = "NODE_LEFT")] NodeLeft,
    [EnumMember(Value = "REROUTE_CANCELLED")] RerouteCancelled,
    [EnumMember(Value = "REINITIALIZED")] Reinitialized,
    [EnumMember(Value = "REALLOCATED_REPLICA")] ReallocatedReplica,
    [EnumMember(Value = "PRIMARY_FAILED")] PrimaryFailed,
    [EnumMember(Value = "FORCED_EMPTY_PRIMARY")] ForcedEmptyPrimary,
    [EnumMember(Value = "MANUAL_ALLOCATION")] ManualAllocation,
  }
}
