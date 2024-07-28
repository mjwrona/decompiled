// Decompiled with JetBrains decompiler
// Type: Nest.Decision
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum Decision
  {
    [EnumMember(Value = "yes")] Yes,
    [EnumMember(Value = "no")] No,
    [EnumMember(Value = "worse_balance")] WorseBalance,
    [EnumMember(Value = "throttled")] Throttled,
    [EnumMember(Value = "awaiting_info")] AwaitingInfo,
    [EnumMember(Value = "allocation_delayed")] AllocationDelayed,
    [EnumMember(Value = "no_valid_shard_copy")] NoValidShardCopy,
    [EnumMember(Value = "no_attempt")] NoAttempt,
  }
}
