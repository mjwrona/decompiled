// Decompiled with JetBrains decompiler
// Type: Nest.PhaseDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PhaseDescriptor : DescriptorBase<PhaseDescriptor, IPhase>, IPhase
  {
    ILifecycleActions IPhase.Actions { get; set; }

    Time IPhase.MinimumAge { get; set; }

    public PhaseDescriptor MinimumAge(string minimumAge) => this.Assign<string>(minimumAge, (Action<IPhase, string>) ((a, v) => a.MinimumAge = (Time) v));

    public PhaseDescriptor Actions(
      Func<LifecycleActionsDescriptor, IPromise<ILifecycleActions>> selector)
    {
      return this.Assign<Func<LifecycleActionsDescriptor, IPromise<ILifecycleActions>>>(selector, (Action<IPhase, Func<LifecycleActionsDescriptor, IPromise<ILifecycleActions>>>) ((a, v) => a.Actions = v != null ? v(new LifecycleActionsDescriptor())?.Value : (ILifecycleActions) null));
    }
  }
}
