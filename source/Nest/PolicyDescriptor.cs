// Decompiled with JetBrains decompiler
// Type: Nest.PolicyDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class PolicyDescriptor : DescriptorBase<PolicyDescriptor, IPolicy>, IPolicy
  {
    IDictionary<string, object> IPolicy.Meta { get; set; }

    IPhases IPolicy.Phases { get; set; }

    public PolicyDescriptor Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> metaSelector)
    {
      return this.Assign<FluentDictionary<string, object>>(metaSelector(new FluentDictionary<string, object>()), (Action<IPolicy, FluentDictionary<string, object>>) ((a, v) => a.Meta = (IDictionary<string, object>) v));
    }

    public PolicyDescriptor Meta(Dictionary<string, object> metaDictionary) => this.Assign<Dictionary<string, object>>(metaDictionary, (Action<IPolicy, Dictionary<string, object>>) ((a, v) => a.Meta = (IDictionary<string, object>) v));

    public PolicyDescriptor Phases(Func<PhasesDescriptor, IPhases> selector) => this.Assign<Func<PhasesDescriptor, IPhases>>(selector, (Action<IPolicy, Func<PhasesDescriptor, IPhases>>) ((a, v) => a.Phases = v != null ? v.InvokeOrDefault<PhasesDescriptor, IPhases>(new PhasesDescriptor()) : (IPhases) null));
  }
}
