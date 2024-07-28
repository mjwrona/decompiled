// Decompiled with JetBrains decompiler
// Type: Nest.TriggerEventDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TriggerEventDescriptor : TriggerEventContainer
  {
    private TriggerEventDescriptor Assign<TValue>(
      TValue value,
      Action<ITriggerEventContainer, TValue> assigner)
    {
      return Fluent.Assign<TriggerEventDescriptor, ITriggerEventContainer, TValue>(this, value, assigner);
    }

    public TriggerEventDescriptor Schedule(
      Func<ScheduleTriggerEventDescriptor, IScheduleTriggerEvent> selector)
    {
      return this.Assign<Func<ScheduleTriggerEventDescriptor, IScheduleTriggerEvent>>(selector, (Action<ITriggerEventContainer, Func<ScheduleTriggerEventDescriptor, IScheduleTriggerEvent>>) ((a, v) => a.Schedule = v != null ? v(new ScheduleTriggerEventDescriptor()) : (IScheduleTriggerEvent) null));
    }
  }
}
