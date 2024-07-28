// Decompiled with JetBrains decompiler
// Type: Nest.ActionExecutionState
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum ActionExecutionState
  {
    [EnumMember(Value = "awaits_execution")] AwaitsExecution,
    [EnumMember(Value = "checking")] Checking,
    [EnumMember(Value = "execution_not_needed")] ExecutionNotNeeded,
    [EnumMember(Value = "throttled")] Throttled,
    [EnumMember(Value = "executed")] Executed,
    [EnumMember(Value = "failed")] Failed,
    [EnumMember(Value = "deleted_while_queued")] DeletedWhileQueued,
    [EnumMember(Value = "not_executed_already_queued")] NotExecutedAlreadyQueued,
  }
}
