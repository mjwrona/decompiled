// Decompiled with JetBrains decompiler
// Type: Nest.IWatch
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (Watch))]
  public interface IWatch
  {
    [DataMember(Name = "actions")]
    Actions Actions { get; set; }

    [DataMember(Name = "condition")]
    ConditionContainer Condition { get; set; }

    [DataMember(Name = "input")]
    InputContainer Input { get; set; }

    [DataMember(Name = "metadata")]
    IDictionary<string, object> Metadata { get; set; }

    [DataMember(Name = "status")]
    WatchStatus Status { get; set; }

    [DataMember(Name = "throttle_period")]
    string ThrottlePeriod { get; set; }

    [DataMember(Name = "transform")]
    TransformContainer Transform { get; set; }

    [DataMember(Name = "trigger")]
    TriggerContainer Trigger { get; set; }
  }
}
