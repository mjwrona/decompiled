// Decompiled with JetBrains decompiler
// Type: Nest.WatchDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class WatchDescriptor : DescriptorBase<WatchDescriptor, IWatch>, IWatch
  {
    Nest.Actions IWatch.Actions { get; set; }

    ConditionContainer IWatch.Condition { get; set; }

    InputContainer IWatch.Input { get; set; }

    IDictionary<string, object> IWatch.Metadata { get; set; }

    string IWatch.ThrottlePeriod { get; set; }

    TransformContainer IWatch.Transform { get; set; }

    TriggerContainer IWatch.Trigger { get; set; }

    WatchStatus IWatch.Status { get; set; }

    public WatchDescriptor Actions(Func<ActionsDescriptor, IPromise<Nest.Actions>> actions) => this.Assign<Func<ActionsDescriptor, IPromise<Nest.Actions>>>(actions, (Action<IWatch, Func<ActionsDescriptor, IPromise<Nest.Actions>>>) ((a, v) => a.Actions = v != null ? v(new ActionsDescriptor())?.Value : (Nest.Actions) null));

    public WatchDescriptor Condition(
      Func<ConditionDescriptor, ConditionContainer> selector)
    {
      return this.Assign<Func<ConditionDescriptor, ConditionContainer>>(selector, (Action<IWatch, Func<ConditionDescriptor, ConditionContainer>>) ((a, v) => a.Condition = v.InvokeOrDefault<ConditionDescriptor, ConditionContainer>(new ConditionDescriptor())));
    }

    public WatchDescriptor Input(Func<InputDescriptor, InputContainer> selector) => this.Assign<Func<InputDescriptor, InputContainer>>(selector, (Action<IWatch, Func<InputDescriptor, InputContainer>>) ((a, v) => a.Input = v.InvokeOrDefault<InputDescriptor, InputContainer>(new InputDescriptor())));

    public WatchDescriptor Metadata(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> paramsDictionary)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(paramsDictionary, (Action<IWatch, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Metadata = (IDictionary<string, object>) v(new FluentDictionary<string, object>())));
    }

    public WatchDescriptor Metadata(Dictionary<string, object> paramsDictionary) => this.Assign<Dictionary<string, object>>(paramsDictionary, (Action<IWatch, Dictionary<string, object>>) ((a, v) => a.Metadata = (IDictionary<string, object>) v));

    public WatchDescriptor ThrottlePeriod(string throttlePeriod) => this.Assign<string>(throttlePeriod, (Action<IWatch, string>) ((a, v) => a.ThrottlePeriod = v));

    public WatchDescriptor Transform(
      Func<TransformDescriptor, TransformContainer> selector)
    {
      return this.Assign<Func<TransformDescriptor, TransformContainer>>(selector, (Action<IWatch, Func<TransformDescriptor, TransformContainer>>) ((a, v) => a.Transform = v.InvokeOrDefault<TransformDescriptor, TransformContainer>(new TransformDescriptor())));
    }

    public WatchDescriptor Trigger(Func<TriggerDescriptor, TriggerContainer> selector) => this.Assign<Func<TriggerDescriptor, TriggerContainer>>(selector, (Action<IWatch, Func<TriggerDescriptor, TriggerContainer>>) ((a, v) => a.Trigger = v.InvokeOrDefault<TriggerDescriptor, TriggerContainer>(new TriggerDescriptor())));
  }
}
