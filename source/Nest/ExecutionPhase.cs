// Decompiled with JetBrains decompiler
// Type: Nest.ExecutionPhase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum ExecutionPhase
  {
    [EnumMember(Value = "awaits_execution")] AwaitsExecution,
    [EnumMember(Value = "started")] Started,
    [EnumMember(Value = "input")] Input,
    [EnumMember(Value = "condition")] Condition,
    [EnumMember(Value = "actions")] Actions,
    [EnumMember(Value = "watch_transform")] WatchTransform,
    [EnumMember(Value = "aborted")] Aborted,
    [EnumMember(Value = "finished")] Finished,
  }
}
