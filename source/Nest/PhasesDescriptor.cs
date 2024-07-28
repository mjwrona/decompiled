// Decompiled with JetBrains decompiler
// Type: Nest.PhasesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PhasesDescriptor : DescriptorBase<PhasesDescriptor, IPhases>, IPhases
  {
    IPhase IPhases.Cold { get; set; }

    IPhase IPhases.Delete { get; set; }

    IPhase IPhases.Hot { get; set; }

    IPhase IPhases.Warm { get; set; }

    IPhase IPhases.Frozen { get; set; }

    public PhasesDescriptor Warm(Func<PhaseDescriptor, IPhase> selector) => this.Assign<Func<PhaseDescriptor, IPhase>>(selector, (Action<IPhases, Func<PhaseDescriptor, IPhase>>) ((a, v) => a.Warm = v != null ? v.InvokeOrDefault<PhaseDescriptor, IPhase>(new PhaseDescriptor()) : (IPhase) null));

    public PhasesDescriptor Hot(Func<PhaseDescriptor, IPhase> selector) => this.Assign<Func<PhaseDescriptor, IPhase>>(selector, (Action<IPhases, Func<PhaseDescriptor, IPhase>>) ((a, v) => a.Hot = v != null ? v.InvokeOrDefault<PhaseDescriptor, IPhase>(new PhaseDescriptor()) : (IPhase) null));

    public PhasesDescriptor Cold(Func<PhaseDescriptor, IPhase> selector) => this.Assign<Func<PhaseDescriptor, IPhase>>(selector, (Action<IPhases, Func<PhaseDescriptor, IPhase>>) ((a, v) => a.Cold = v != null ? v.InvokeOrDefault<PhaseDescriptor, IPhase>(new PhaseDescriptor()) : (IPhase) null));

    public PhasesDescriptor Delete(Func<PhaseDescriptor, IPhase> selector) => this.Assign<Func<PhaseDescriptor, IPhase>>(selector, (Action<IPhases, Func<PhaseDescriptor, IPhase>>) ((a, v) => a.Delete = v != null ? v.InvokeOrDefault<PhaseDescriptor, IPhase>(new PhaseDescriptor()) : (IPhase) null));

    public PhasesDescriptor Frozen(Func<PhaseDescriptor, IPhase> selector) => this.Assign<Func<PhaseDescriptor, IPhase>>(selector, (Action<IPhases, Func<PhaseDescriptor, IPhase>>) ((a, v) => a.Frozen = v != null ? v.InvokeOrDefault<PhaseDescriptor, IPhase>(new PhaseDescriptor()) : (IPhase) null));
  }
}
