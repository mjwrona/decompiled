// Decompiled with JetBrains decompiler
// Type: Nest.AlertingUsage
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class AlertingUsage : XPackUsage
  {
    [DataMember(Name = "count")]
    public AlertingUsage.AlertingCount Count { get; internal set; }

    [DataMember(Name = "execution")]
    public AlertingUsage.AlertingExecution Execution { get; internal set; }

    [DataMember(Name = "watch")]
    public AlertingUsage.AlertingInput Watch { get; internal set; }

    public class AlertingExecution
    {
      [DataMember(Name = "actions")]
      public IReadOnlyDictionary<string, AlertingUsage.ExecutionAction> Actions { get; internal set; } = EmptyReadOnly<string, AlertingUsage.ExecutionAction>.Dictionary;
    }

    public class AlertingInput
    {
      [DataMember(Name = "input")]
      public IReadOnlyDictionary<string, AlertingUsage.AlertingCount> Input { get; internal set; } = EmptyReadOnly<string, AlertingUsage.AlertingCount>.Dictionary;

      [DataMember(Name = "trigger")]
      public IReadOnlyDictionary<string, AlertingUsage.AlertingCount> Trigger { get; internal set; } = EmptyReadOnly<string, AlertingUsage.AlertingCount>.Dictionary;
    }

    public class ExecutionAction
    {
      [DataMember(Name = "total")]
      public long Total { get; internal set; }

      [DataMember(Name = "total_in_ms")]
      public long TotalInMilliseconds { get; internal set; }
    }

    public class AlertingCount
    {
      [DataMember(Name = "active")]
      public long Active { get; internal set; }

      [DataMember(Name = "total")]
      public long Total { get; internal set; }
    }
  }
}
