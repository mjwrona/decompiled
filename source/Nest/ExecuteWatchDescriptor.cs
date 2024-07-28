// Decompiled with JetBrains decompiler
// Type: Nest.ExecuteWatchDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.WatcherApi;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class ExecuteWatchDescriptor : 
    RequestDescriptorBase<ExecuteWatchDescriptor, ExecuteWatchRequestParameters, IExecuteWatchRequest>,
    IExecuteWatchRequest,
    IRequest<ExecuteWatchRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherExecute;

    public ExecuteWatchDescriptor(Nest.Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    public ExecuteWatchDescriptor()
    {
    }

    Nest.Id IExecuteWatchRequest.Id => this.Self.RouteValues.Get<Nest.Id>("id");

    public ExecuteWatchDescriptor Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<IExecuteWatchRequest, Nest.Id>) ((a, v) => a.RouteValues.Optional(nameof (id), (IUrlParameter) v)));

    public ExecuteWatchDescriptor Debug(bool? debug = true) => this.Qs(nameof (debug), (object) debug);

    IDictionary<string, ActionExecutionMode> IExecuteWatchRequest.ActionModes { get; set; }

    IDictionary<string, object> IExecuteWatchRequest.AlternativeInput { get; set; }

    bool? IExecuteWatchRequest.IgnoreCondition { get; set; }

    bool? IExecuteWatchRequest.RecordExecution { get; set; }

    Nest.SimulatedActions IExecuteWatchRequest.SimulatedActions { get; set; }

    IScheduleTriggerEvent IExecuteWatchRequest.TriggerData { get; set; }

    IWatch IExecuteWatchRequest.Watch { get; set; }

    public ExecuteWatchDescriptor TriggerData(
      Func<ScheduleTriggerEventDescriptor, IScheduleTriggerEvent> selector)
    {
      return this.Assign<Func<ScheduleTriggerEventDescriptor, IScheduleTriggerEvent>>(selector, (Action<IExecuteWatchRequest, Func<ScheduleTriggerEventDescriptor, IScheduleTriggerEvent>>) ((a, v) => a.TriggerData = v != null ? v.InvokeOrDefault<ScheduleTriggerEventDescriptor, IScheduleTriggerEvent>(new ScheduleTriggerEventDescriptor()) : (IScheduleTriggerEvent) null));
    }

    public ExecuteWatchDescriptor IgnoreCondition(bool? ignore = true) => this.Assign<bool?>(ignore, (Action<IExecuteWatchRequest, bool?>) ((a, v) => a.IgnoreCondition = v));

    public ExecuteWatchDescriptor RecordExecution(bool? record = true) => this.Assign<bool?>(record, (Action<IExecuteWatchRequest, bool?>) ((a, v) => a.RecordExecution = v));

    public ExecuteWatchDescriptor ActionModes(
      Func<FluentDictionary<string, ActionExecutionMode>, FluentDictionary<string, ActionExecutionMode>> actionModesDictionary)
    {
      return this.Assign<FluentDictionary<string, ActionExecutionMode>>(actionModesDictionary(new FluentDictionary<string, ActionExecutionMode>()), (Action<IExecuteWatchRequest, FluentDictionary<string, ActionExecutionMode>>) ((a, v) => a.ActionModes = (IDictionary<string, ActionExecutionMode>) v));
    }

    public ExecuteWatchDescriptor ActionModes(
      Dictionary<string, ActionExecutionMode> actionModesDictionary)
    {
      return this.Assign<Dictionary<string, ActionExecutionMode>>(actionModesDictionary, (Action<IExecuteWatchRequest, Dictionary<string, ActionExecutionMode>>) ((a, v) => a.ActionModes = (IDictionary<string, ActionExecutionMode>) v));
    }

    public ExecuteWatchDescriptor AlternativeInput(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> alternativeInputDictionary)
    {
      return this.Assign<FluentDictionary<string, object>>(alternativeInputDictionary(new FluentDictionary<string, object>()), (Action<IExecuteWatchRequest, FluentDictionary<string, object>>) ((a, v) => a.AlternativeInput = (IDictionary<string, object>) v));
    }

    public ExecuteWatchDescriptor AlternativeInput(
      Dictionary<string, object> alternativeInputDictionary)
    {
      return this.Assign<Dictionary<string, object>>(alternativeInputDictionary, (Action<IExecuteWatchRequest, Dictionary<string, object>>) ((a, v) => a.AlternativeInput = (IDictionary<string, object>) v));
    }

    public ExecuteWatchDescriptor SimulatedActions(Nest.SimulatedActions simulatedActions) => this.Assign<Nest.SimulatedActions>(simulatedActions, (Action<IExecuteWatchRequest, Nest.SimulatedActions>) ((a, v) => a.SimulatedActions = v));

    public ExecuteWatchDescriptor Watch(Func<WatchDescriptor, IWatch> watch) => this.Assign<Func<WatchDescriptor, IWatch>>(watch, (Action<IExecuteWatchRequest, Func<WatchDescriptor, IWatch>>) ((a, v) => a.Watch = v != null ? v.InvokeOrDefault<WatchDescriptor, IWatch>(new WatchDescriptor()) : (IWatch) null));
  }
}
