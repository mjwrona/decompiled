// Decompiled with JetBrains decompiler
// Type: Nest.PutWatchDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.WatcherApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class PutWatchDescriptor : 
    RequestDescriptorBase<PutWatchDescriptor, PutWatchRequestParameters, IPutWatchRequest>,
    IPutWatchRequest,
    IRequest<PutWatchRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherPut;

    public PutWatchDescriptor(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected PutWatchDescriptor()
    {
    }

    Id IPutWatchRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public PutWatchDescriptor Active(bool? active = true) => this.Qs(nameof (active), (object) active);

    public PutWatchDescriptor IfPrimaryTerm(long? ifprimaryterm) => this.Qs("if_primary_term", (object) ifprimaryterm);

    public PutWatchDescriptor IfSequenceNumber(long? ifsequencenumber) => this.Qs("if_seq_no", (object) ifsequencenumber);

    public PutWatchDescriptor Version(long? version) => this.Qs(nameof (version), (object) version);

    Nest.Actions IPutWatchRequest.Actions { get; set; }

    ConditionContainer IPutWatchRequest.Condition { get; set; }

    InputContainer IPutWatchRequest.Input { get; set; }

    IDictionary<string, object> IPutWatchRequest.Metadata { get; set; }

    string IPutWatchRequest.ThrottlePeriod { get; set; }

    TransformContainer IPutWatchRequest.Transform { get; set; }

    TriggerContainer IPutWatchRequest.Trigger { get; set; }

    public PutWatchDescriptor Actions(Func<ActionsDescriptor, IPromise<Nest.Actions>> actions) => this.Assign<Func<ActionsDescriptor, IPromise<Nest.Actions>>>(actions, (Action<IPutWatchRequest, Func<ActionsDescriptor, IPromise<Nest.Actions>>>) ((a, v) => a.Actions = v != null ? v(new ActionsDescriptor())?.Value : (Nest.Actions) null));

    public PutWatchDescriptor Condition(
      Func<ConditionDescriptor, ConditionContainer> selector)
    {
      return this.Assign<ConditionContainer>(selector.InvokeOrDefault<ConditionDescriptor, ConditionContainer>(new ConditionDescriptor()), (Action<IPutWatchRequest, ConditionContainer>) ((a, v) => a.Condition = v));
    }

    public PutWatchDescriptor Input(Func<InputDescriptor, InputContainer> selector) => this.Assign<InputContainer>(selector.InvokeOrDefault<InputDescriptor, InputContainer>(new InputDescriptor()), (Action<IPutWatchRequest, InputContainer>) ((a, v) => a.Input = v));

    public PutWatchDescriptor Metadata(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> paramsDictionary)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(paramsDictionary, (Action<IPutWatchRequest, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Metadata = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }

    public PutWatchDescriptor Metadata(Dictionary<string, object> paramsDictionary) => this.Assign<Dictionary<string, object>>(paramsDictionary, (Action<IPutWatchRequest, Dictionary<string, object>>) ((a, v) => a.Metadata = (IDictionary<string, object>) v));

    public PutWatchDescriptor ThrottlePeriod(string throttlePeriod) => this.Assign<string>(throttlePeriod, (Action<IPutWatchRequest, string>) ((a, v) => a.ThrottlePeriod = v));

    public PutWatchDescriptor Transform(
      Func<TransformDescriptor, TransformContainer> selector)
    {
      return this.Assign<TransformContainer>(selector.InvokeOrDefault<TransformDescriptor, TransformContainer>(new TransformDescriptor()), (Action<IPutWatchRequest, TransformContainer>) ((a, v) => a.Transform = v));
    }

    public PutWatchDescriptor Trigger(Func<TriggerDescriptor, TriggerContainer> selector) => this.Assign<TriggerContainer>(selector.InvokeOrDefault<TriggerDescriptor, TriggerContainer>(new TriggerDescriptor()), (Action<IPutWatchRequest, TriggerContainer>) ((a, v) => a.Trigger = v));
  }
}
