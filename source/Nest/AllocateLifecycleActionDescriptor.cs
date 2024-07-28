// Decompiled with JetBrains decompiler
// Type: Nest.AllocateLifecycleActionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class AllocateLifecycleActionDescriptor : 
    DescriptorBase<AllocateLifecycleActionDescriptor, IAllocateLifecycleAction>,
    IAllocateLifecycleAction,
    ILifecycleAction
  {
    IDictionary<string, string> IAllocateLifecycleAction.Exclude { get; set; }

    IDictionary<string, string> IAllocateLifecycleAction.Include { get; set; }

    int? IAllocateLifecycleAction.NumberOfReplicas { get; set; }

    IDictionary<string, string> IAllocateLifecycleAction.Require { get; set; }

    public AllocateLifecycleActionDescriptor NumberOfReplicas(int? numberOfReplicas) => this.Assign<int?>(numberOfReplicas, (Action<IAllocateLifecycleAction, int?>) ((a, v) => a.NumberOfReplicas = numberOfReplicas));

    public AllocateLifecycleActionDescriptor Include(
      Func<FluentDictionary<string, string>, FluentDictionary<string, string>> includeSelector)
    {
      return this.Assign<FluentDictionary<string, string>>(includeSelector(new FluentDictionary<string, string>()), (Action<IAllocateLifecycleAction, FluentDictionary<string, string>>) ((a, v) => a.Include = (IDictionary<string, string>) v));
    }

    public AllocateLifecycleActionDescriptor Exclude(
      Func<FluentDictionary<string, string>, FluentDictionary<string, string>> excludeSelector)
    {
      return this.Assign<FluentDictionary<string, string>>(excludeSelector(new FluentDictionary<string, string>()), (Action<IAllocateLifecycleAction, FluentDictionary<string, string>>) ((a, v) => a.Exclude = (IDictionary<string, string>) v));
    }

    public AllocateLifecycleActionDescriptor Require(
      Func<FluentDictionary<string, string>, FluentDictionary<string, string>> requireSelector)
    {
      return this.Assign<FluentDictionary<string, string>>(requireSelector(new FluentDictionary<string, string>()), (Action<IAllocateLifecycleAction, FluentDictionary<string, string>>) ((a, v) => a.Require = (IDictionary<string, string>) v));
    }
  }
}
