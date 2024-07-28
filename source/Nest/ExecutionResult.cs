// Decompiled with JetBrains decompiler
// Type: Nest.ExecutionResult
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class ExecutionResult
  {
    [DataMember(Name = "actions")]
    public IReadOnlyCollection<ExecutionResultAction> Actions { get; set; }

    [DataMember(Name = "condition")]
    public ExecutionResultCondition Condition { get; set; }

    [DataMember(Name = "execution_duration")]
    public int? ExecutionDuration { get; set; }

    [DataMember(Name = "execution_time")]
    public DateTimeOffset? ExecutionTime { get; set; }

    [DataMember(Name = "input")]
    public ExecutionResultInput Input { get; set; }
  }
}
