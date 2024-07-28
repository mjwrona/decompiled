// Decompiled with JetBrains decompiler
// Type: Nest.IExecuteWatchRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.WatcherApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("watcher.execute_watch.json")]
  public interface IExecuteWatchRequest : IRequest<ExecuteWatchRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Id Id { get; }

    [DataMember(Name = "action_modes")]
    [JsonFormatter(typeof (VerbatimDictionaryInterfaceKeysFormatter<string, ActionExecutionMode>))]
    IDictionary<string, ActionExecutionMode> ActionModes { get; set; }

    [DataMember(Name = "alternative_input")]
    [JsonFormatter(typeof (VerbatimDictionaryInterfaceKeysFormatter<string, object>))]
    IDictionary<string, object> AlternativeInput { get; set; }

    [DataMember(Name = "ignore_condition")]
    bool? IgnoreCondition { get; set; }

    [DataMember(Name = "record_execution")]
    bool? RecordExecution { get; set; }

    [DataMember(Name = "simulated_actions")]
    SimulatedActions SimulatedActions { get; set; }

    [DataMember(Name = "trigger_data")]
    IScheduleTriggerEvent TriggerData { get; set; }

    [DataMember(Name = "watch")]
    IWatch Watch { get; set; }
  }
}
