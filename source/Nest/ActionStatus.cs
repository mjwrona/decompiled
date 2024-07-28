// Decompiled with JetBrains decompiler
// Type: Nest.ActionStatus
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class ActionStatus
  {
    [DataMember(Name = "ack")]
    public AcknowledgeState Acknowledgement { get; set; }

    [DataMember(Name = "last_execution")]
    public ExecutionState LastExecution { get; set; }

    [DataMember(Name = "last_successful_execution")]
    public ExecutionState LastSuccessfulExecution { get; set; }

    [DataMember(Name = "last_throttle")]
    public ThrottleState LastThrottle { get; set; }
  }
}
