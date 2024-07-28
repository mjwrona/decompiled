// Decompiled with JetBrains decompiler
// Type: Nest.TriggerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TriggerDescriptor : TriggerContainer
  {
    private TriggerDescriptor Assign<TValue>(
      TValue value,
      Action<ITriggerContainer, TValue> assigner)
    {
      return Fluent.Assign<TriggerDescriptor, ITriggerContainer, TValue>(this, value, assigner);
    }

    public TriggerDescriptor Schedule(
      Func<ScheduleDescriptor, IScheduleContainer> selector)
    {
      return this.Assign<Func<ScheduleDescriptor, IScheduleContainer>>(selector, (Action<ITriggerContainer, Func<ScheduleDescriptor, IScheduleContainer>>) ((a, v) => a.Schedule = v != null ? v.InvokeOrDefault<ScheduleDescriptor, IScheduleContainer>(new ScheduleDescriptor()) : (IScheduleContainer) null));
    }
  }
}
