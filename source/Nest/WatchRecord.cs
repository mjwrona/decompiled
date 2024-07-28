// Decompiled with JetBrains decompiler
// Type: Nest.WatchRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class WatchRecord
  {
    [DataMember(Name = "condition")]
    public ConditionContainer Condition { get; set; }

    [DataMember(Name = "input")]
    public InputContainer Input { get; set; }

    [DataMember(Name = "messages")]
    public IReadOnlyCollection<string> Messages { get; set; }

    [DataMember(Name = "metadata")]
    public IReadOnlyDictionary<string, object> Metadata { get; set; }

    [DataMember(Name = "result")]
    public ExecutionResult Result { get; set; }

    [DataMember(Name = "state")]
    public ActionExecutionState? State { get; set; }

    [DataMember(Name = "trigger_event")]
    public TriggerEventResult TriggerEvent { get; set; }

    [DataMember(Name = "user")]
    public string User { get; set; }

    [DataMember(Name = "node")]
    public string Node { get; set; }

    [DataMember(Name = "watch_id")]
    public string WatchId { get; set; }
  }
}
